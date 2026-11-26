using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Serenity.Web;

/// <summary>
/// Abstraction for a provider of layout script files declared with
/// <see cref="LayoutScriptFileAttribute"/>.
/// </summary>
public interface ILayoutScriptFileProvider
{
    /// <summary>
    /// Gets the list of app relative (<c>~/</c>) JS file urls to include in the
    /// layout, in the order they should be included, with duplicates removed.
    /// Files with a permission the current user lacks are left out.
    /// These don't include the ES modules, see <see cref="GetModuleFiles"/>.
    /// </summary>
    /// <returns>The list of JS file urls.</returns>
    IReadOnlyList<string> GetFiles();

    /// <summary>
    /// Gets the list of app relative (<c>~/</c>) urls of the ES modules to include in the
    /// layout, in the order they should be included, with duplicates removed.
    /// Files with a permission the current user lacks are left out.
    /// </summary>
    /// <returns>The list of ES module urls.</returns>
    IReadOnlyList<string> GetModuleFiles() => [];

    /// <summary>
    /// Gets the list of files like <see cref="GetFiles"/> or <see cref="GetModuleFiles"/>, but
    /// regardless of the permissions of the current user, e.g. for file watchers.
    /// </summary>
    /// <param name="module">True to get the ES modules, false for the other files.</param>
    IReadOnlyList<string> GetAllFiles(bool module) => module ? GetModuleFiles() : GetFiles();

    /// <summary>
    /// Gets the permission required to include the file, see <see cref="LayoutScriptFileAttribute.Permission"/>,
    /// or "*" (public) for a provider that does not support permissions.
    /// </summary>
    /// <param name="path">The app relative file url.</param>
    string? GetPermission(string path) => SpecialPermissionKeys.Public;
}

/// <summary>
/// Default implementation of <see cref="ILayoutScriptFileProvider"/>, which
/// discovers <see cref="LayoutScriptFileAttribute"/>s from the type source,
/// filters them by the registered <see cref="IFeatureToggles"/> and an optional
/// filter, and returns them ordered with duplicates removed.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="LayoutScriptFileProvider"/> class.
/// </remarks>
/// <param name="typeSource">The type source.</param>
/// <param name="serviceProvider">The service provider, used to resolve the
/// optional <see cref="IFeatureToggles"/>.</param>
/// <param name="options">The layout script file options.</param>
/// <exception cref="ArgumentNullException">One of the required arguments is <c>null</c>.</exception>
public class LayoutScriptFileProvider(ITypeSource typeSource,
    IServiceProvider serviceProvider,
    IOptions<LayoutScriptFileOptions> options) : ILayoutScriptFileProvider
{
    private readonly ITypeSource typeSource = typeSource ?? throw new ArgumentNullException(nameof(typeSource));
    private readonly IServiceProvider serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    private readonly LayoutScriptFileOptions options = (options ?? throw new ArgumentNullException(nameof(options))).Value;

    /// <inheritdoc/>
    public IReadOnlyList<string> GetFiles() => GetFiles(module: false, checkPermissions: true);

    /// <inheritdoc/>
    public IReadOnlyList<string> GetModuleFiles() => GetFiles(module: true, checkPermissions: true);

    /// <inheritdoc/>
    public IReadOnlyList<string> GetAllFiles(bool module) => GetFiles(module, checkPermissions: false);

    /// <inheritdoc/>
    public string? GetPermission(string path)
    {
        ArgumentNullException.ThrowIfNull(path);
        permissions ??= LayoutFilePermissions.ByPath(typeSource.GetAssemblyAttributes<LayoutScriptFileAttribute>()
            .Select(x => ((string?)x.Path, (string?)x.Permission)));
        return permissions.TryGetValue(path.Trim(), out var permission) ? permission : null;
    }

    private IReadOnlyDictionary<string, string>? permissions;

    private List<string> GetFiles(bool module, bool checkPermissions)
    {
        var featureToggles = serviceProvider.GetService<IFeatureToggles>();
        var permissionService = checkPermissions ? serviceProvider.GetService<IPermissionService>() : null;
        var result = new List<(decimal Order, string Path)>();

        foreach (var attr in typeSource.GetAssemblyAttributes<LayoutScriptFileAttribute>())
        {
            if (attr.Module != module)
                continue;

            if (attr.RequireFeatures?.Length > 0 &&
                featureToggles != null &&
                !featureToggles.IsEnabled(attr.RequireFeatures, attr.RequireAnyFeature))
                continue;

            if (options.Filter != null && !options.Filter(attr))
                continue;

            if (checkPermissions && !LayoutFilePermissions.IsGranted(permissionService, attr.Permission))
                continue;

            var path = (attr.Path ?? "").Trim();
            if (path.Length == 0 ||
                !path.StartsWith("~/", StringComparison.Ordinal))
                continue;

            result.Add((attr.Order, path));
        }

        return result
            .OrderBy(x => x.Order)
            .ThenBy(x => x.Path, StringComparer.OrdinalIgnoreCase)
            .Select(x => x.Path)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}
