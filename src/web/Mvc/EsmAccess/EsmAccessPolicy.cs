using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace Serenity.Web;

/// <summary>
/// Determines who can load the files under an <c>esm</c> folder from the <c>build.esm-manifest</c>
/// in the folder and <see cref="EsmAccessControlOptions"/>. See <see cref="EsmAccessControlOptions"/> for the rules.
/// </summary>
public class EsmAccessPolicy
{
    private const string EsmSegment = "/esm/";

    private readonly IFileProvider fileProvider;
    private readonly EsmAccessControlOptions options;
    private readonly ILayoutScriptFileProvider? layoutScripts;
    private readonly ILogger? logger;
    private readonly List<EsmPathPattern> anonymousPatterns;
    private readonly List<(EsmPathPattern Pattern, string Permission)> permissionPatterns;
    private readonly ConcurrentDictionary<string, EsmRootState> roots = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, bool> missingManifestLogged = new(StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, string>? layoutModules;

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="fileProvider">The file provider of the web root, which serves the <c>esm</c> folders</param>
    /// <param name="options">The options</param>
    /// <param name="layoutScripts">The layout script file provider, for the permissions of the layout modules</param>
    /// <param name="logger">The logger</param>
    public EsmAccessPolicy(IFileProvider fileProvider, EsmAccessControlOptions options,
        ILayoutScriptFileProvider? layoutScripts = null, ILogger? logger = null)
    {
        this.fileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.layoutScripts = layoutScripts;
        this.logger = logger;
        anonymousPatterns = EsmPathPattern.ParseList(options.AnonymousPaths);
        permissionPatterns = [.. (options.Permissions ?? [])
            .Where(x => !string.IsNullOrWhiteSpace(x.Key))
            .Select(x => (new EsmPathPattern(x.Key), string.IsNullOrWhiteSpace(x.Value) ? SpecialPermissionKeys.Deny : x.Value.Trim()))
            // the longest pattern wins, as it is the most specific
            .OrderByDescending(x => x.Item1.Pattern.Length)];
    }

    /// <summary>
    /// How often the manifest of an <c>esm</c> folder is checked for changes, e.g. while tsbuild is watching
    /// in development. Default is 2 seconds.
    /// </summary>
    public TimeSpan ManifestCheckInterval { get; set; } = TimeSpan.FromSeconds(2);

    /// <summary>
    /// Gets the access to the file at the request path.
    /// </summary>
    /// <param name="requestPath">The request path, excluding the path base</param>
    public EsmFileAccess GetAccess(string? requestPath)
    {
        if (string.IsNullOrEmpty(requestPath))
            return EsmFileAccess.NotHandled;

        var fullPath = NormalizePath(requestPath);
        var index = fullPath.IndexOf(EsmSegment, StringComparison.OrdinalIgnoreCase);
        if (index < 0)
            return EsmFileAccess.NotHandled;

        var root = fullPath[..(index + EsmSegment.Length)];
        var relativePath = fullPath[(index + EsmSegment.Length)..];
        if (relativePath.Length == 0)
            return EsmFileAccess.NotHandled;

        if (string.Equals(relativePath, EsmManifest.FileName, StringComparison.OrdinalIgnoreCase))
            return EsmFileAccess.Hidden;

        // a precompressed variant of a file has the same access as the file
        if (relativePath.EndsWith(".gz", StringComparison.OrdinalIgnoreCase) ||
            relativePath.EndsWith(".br", StringComparison.OrdinalIgnoreCase))
        {
            relativePath = relativePath[..^3];
            fullPath = fullPath[..^3];
        }

        var state = GetRootState(root);
        if (state.Manifest is null)
            return EsmFileAccess.NotHandled;

        return state.Access.GetOrAdd(relativePath, _ => ComputeAccess(state.Manifest, root, relativePath, fullPath));
    }

    private static string NormalizePath(string path)
    {
        path = path.Replace('\\', '/');
        while (path.Contains("//", StringComparison.Ordinal))
            path = path.Replace("//", "/", StringComparison.Ordinal);
        return path;
    }

    private EsmFileAccess ComputeAccess(EsmManifest manifest, string root, string relativePath, string fullPath)
    {
        IEnumerable<string> permissions;
        if (manifest.Files.TryGetValue(relativePath, out var entryIndexes))
        {
            // a file that no entry point reaches, e.g. a font or image
            if (entryIndexes is null || entryIndexes.Length == 0)
                return EsmFileAccess.Public;

            permissions = entryIndexes
                .Where(i => i >= 0 && i < manifest.Entries.Length)
                .Select(i => manifest.Entries[i])
                .Select(entry => GetEntryPermission(entry, root + entry));
        }
        else
        {
            // not in the manifest, e.g. a global bundle or a file added manually
            permissions = [GetEntryPermission(relativePath, fullPath)];
        }

        var list = permissions.Distinct(StringComparer.Ordinal).ToArray();
        if (list.Length == 0)
            return EsmFileAccess.Restricted([GetDefaultPermission()]);

        if (list.Any(LayoutFilePermissions.IsPublic))
            return EsmFileAccess.Public;

        return EsmFileAccess.Restricted(list);
    }

    /// <summary>
    /// Gets the permission of an entry point, see <see cref="EsmAccessControlOptions"/> for the rules.
    /// </summary>
    /// <param name="relativePath">The path relative to the <c>esm</c> folder</param>
    /// <param name="fullPath">The full request path</param>
    public string GetEntryPermission(string relativePath, string fullPath)
    {
        ArgumentNullException.ThrowIfNull(relativePath);
        ArgumentNullException.ThrowIfNull(fullPath);

        if (GetLayoutModules().TryGetValue(fullPath, out var layoutPermission))
            return layoutPermission;

        if (anonymousPatterns.Any(x => x.IsMatch(relativePath, fullPath)))
            return SpecialPermissionKeys.Public;

        foreach (var (pattern, permission) in permissionPatterns)
        {
            if (pattern.IsMatch(relativePath, fullPath))
                return permission;
        }

        return GetDefaultPermission();
    }

    // a null or empty permission is invalid, and denies like elsewhere
    private string GetDefaultPermission() => string.IsNullOrWhiteSpace(options.DefaultPermission) ?
        SpecialPermissionKeys.Deny : options.DefaultPermission.Trim();

    private Dictionary<string, string> GetLayoutModules()
    {
        if (layoutModules is not null)
            return layoutModules;

        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (layoutScripts is not null)
        {
            foreach (var file in layoutScripts.GetAllFiles(module: true))
            {
                if (!file.StartsWith("~/", StringComparison.Ordinal))
                    continue;

                // a null or empty permission is invalid, and denies like elsewhere
                var permission = layoutScripts.GetPermission(file);
                result[NormalizePath(file[1..])] = string.IsNullOrWhiteSpace(permission) ?
                    SpecialPermissionKeys.Deny : permission.Trim();
            }
        }

        return layoutModules = result;
    }

    private EsmRootState GetRootState(string root)
    {
        var now = DateTime.UtcNow;
        if (roots.TryGetValue(root, out var state) &&
            now - state.CheckedAt < ManifestCheckInterval)
            return state;

        // the manifest is checked for changes at most every few seconds, e.g. while watching in development
        // without the leading slash, which all file providers accept
        var fileInfo = fileProvider.GetFileInfo(root.TrimStart('/') + EsmManifest.FileName);
        DateTimeOffset? lastModified = fileInfo.Exists ? fileInfo.LastModified : null;
        if (state is not null && state.LastModified == lastModified)
        {
            state.CheckedAt = now;
            return state;
        }

        EsmManifest? manifest = null;
        if (fileInfo.Exists)
        {
            try
            {
                using var stream = fileInfo.CreateReadStream();
                using var reader = new System.IO.StreamReader(stream);
                manifest = EsmManifest.Parse(reader.ReadToEnd());
            }
            catch (Exception ex)
            {
                // a manifest that can't be read denies by default rather than serving everything
                logger?.LogError(ex, "Error reading ESM manifest {Path}!", root + EsmManifest.FileName);
                manifest = new EsmManifest();
            }
        }
        else if (missingManifestLogged.TryAdd(root, true))
        {
            logger?.LogWarning("ESM access control: no {FileName} in {Root}, so its files are served without " +
                "access control. Rebuild the project (or package) with a tsbuild version that writes the manifest.",
                EsmManifest.FileName, root);
        }

        state = new EsmRootState(manifest, lastModified, now);
        roots[root] = state;
        return state;
    }
}
