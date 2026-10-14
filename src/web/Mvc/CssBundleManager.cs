using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Serenity.Web;

/// <summary>
/// Default implementation of <see cref="ICssBundleManager"/>.
/// </summary>
public partial class CssBundleManager : ICssBundleManager
{
    private readonly Lock sync = new();

    private bool isEnabled;
    private Dictionary<string, string>? bundleKeyBySourceUrl;
    private Dictionary<string, HashSet<string>>? bundleKeysBySourceUrl;
    private HashSet<string>? bundleKeys;
    private Dictionary<string, List<string>>? bundleIncludes;

    private const string errorLines = "\r\n/*\r\n!!!ERROR: {0}!!!\r\n*/\r\n";
    private readonly IDynamicScriptManager scriptManager;
    private readonly ICssMinifier cssMinifier;
    private readonly IWebHostEnvironment hostEnvironment;
    private readonly IHttpContextAccessor? contextAccessor;
    private readonly ILogger<CssBundleManager>? logger;
    private readonly CssBundlingOptions options;

    [ThreadStatic]
    private static HashSet<string>? recursionCheck;

    /// <summary>
    /// Initializes a new instance of the <see cref="CssBundleManager"/> class.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="scriptManager">The dynamic script manager.</param>
    /// <param name="cssMinifier">The CSS minifier.</param>
    /// <param name="hostEnvironment">The web host environment.</param>
    /// <param name="contextAccessor">The HTTP context accessor.</param>
    /// <param name="logger">The exception logger.</param>
    /// <exception cref="ArgumentNullException">One of the required arguments is <c>null</c>.</exception>
    public CssBundleManager(IOptions<CssBundlingOptions> options,
        IDynamicScriptManager scriptManager,
        ICssMinifier cssMinifier,
        IWebHostEnvironment hostEnvironment,
        IHttpContextAccessor? contextAccessor = null,
        ILogger<CssBundleManager>? logger = null)
    {
        this.options = (options ?? throw new ArgumentNullException(nameof(options))).Value;
        this.scriptManager = scriptManager ?? throw new ArgumentNullException(nameof(scriptManager));
        this.cssMinifier = cssMinifier ?? throw new ArgumentNullException(nameof(cssMinifier));
        this.hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
        this.contextAccessor = contextAccessor;
        this.logger = logger;

        Reset();
        scriptManager.ScriptChanged += name =>
        {
            HashSet<string>? bundleKeys;
            lock (sync)
            {
                if (bundleKeysBySourceUrl == null ||
                    !bundleKeysBySourceUrl.TryGetValue("dynamic://" + name, out bundleKeys))
                    bundleKeys = null;
            }

            if (bundleKeys != null)
                foreach (var bundleKey in bundleKeys)
                    scriptManager.Changed("CssBundle." + bundleKey);
        };
    }

    /// <inheritdoc/>
    public void Reset()
    {
        lock (sync)
        {
            isEnabled = false;
            var settings = options.Value;
            var bundles = settings.Bundles ?? [];

            foreach (var key in bundles.Keys.ToArray())
            {
                var parts = bundles[key];
                if (parts != null &&
                    !bundles.ContainsKey(key + ".rtl") &&
                    parts.Any(x => x != null &&
                        x.Contains("{.rtl}", StringComparison.OrdinalIgnoreCase)))
                {
                    bundles[key + ".rtl"] = [.. parts.Select(x => x
                        .Replace("{.rtl}", ".rtl", StringComparison.OrdinalIgnoreCase))];
                    bundles[key] = [.. parts.Select(x => x
                        .Replace("{.rtl}", "", StringComparison.OrdinalIgnoreCase))];
                }
            }

            bundles = bundles.Keys.ToDictionary(k => k,
                k => (bundles[k] ?? [])
                    .Select(u => BundleUtils.DoReplacements(u, settings.Replacements))
                    .OfType<string>()
                    .Where(u => !string.IsNullOrEmpty(u))
                    .ToArray());

            bundleIncludes = BundleUtils.ExpandBundleIncludes(bundles, "dynamic://CssBundle.", "css");

            if (bundles.Count == 0 ||
                settings.Enabled != true)
            {
                bundleKeyBySourceUrl = null;
                bundleKeysBySourceUrl = null;
                bundleKeys = null;
                return;
            }

            bundleKeyBySourceUrl = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            bundleKeysBySourceUrl = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
            bundleKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            bool minimize = settings.Minimize == true;
            bool processImports = settings.ProcessImports != false;
            bool dedupImports = settings.DedupImports == true;
            var noMinimize = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (settings.NoMinimize != null)
                noMinimize.AddRange(settings.NoMinimize);

            foreach (var pair in bundles)
            {
                var sourceFiles = pair.Value;
                if (sourceFiles == null ||
                    sourceFiles.Length == 0)
                    continue;

                var bundleKey = pair.Key;
                var bundleName = "CssBundle." + bundleKey;
                var bundleRewriteRoot = string.Concat(Enumerable.Repeat("../",
                    Math.Max(bundleKey.Split('/', StringSplitOptions.RemoveEmptyEntries).Length, 1)));
                var bundleParts = new List<Func<string>>();
                var parts = new List<Func<string>>();
                var dedupHolder = new HashSet<string>?[1];
                var scriptNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                void registerInBundle(string appRelativeUrl)
                {
                    if (bundleKey.IndexOf('/', StringComparison.Ordinal) < 0 &&
                        !bundleKeyBySourceUrl.ContainsKey(appRelativeUrl))
                        bundleKeyBySourceUrl[appRelativeUrl] = bundleKey;

                    if (!bundleKeysBySourceUrl.TryGetValue(appRelativeUrl, out HashSet<string>? bundleKeys))
                    {
                        bundleKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                        bundleKeysBySourceUrl[appRelativeUrl] = bundleKeys;
                    }

                    bundleKeys.Add(bundleKey);
                }

                foreach (var sourceFile in sourceFiles)
                {
                    if (string.IsNullOrEmpty(sourceFile))
                        continue;

                    if (sourceFile.StartsWith("dynamic://", StringComparison.OrdinalIgnoreCase))
                    {
                        registerInBundle(sourceFile);

                        var scriptName = sourceFile[10..];
                        scriptNames.Add(scriptName);
                        parts.Add(() =>
                        {
                            if (recursionCheck != null)
                            {
                                if (recursionCheck.Contains(scriptName) || recursionCheck.Count > 100)
                                    return string.Format(CultureInfo.CurrentCulture, errorLines,
                                        string.Format(CultureInfo.CurrentCulture, "Caught infinite recursion with dynamic scripts '{0}'!",
                                            string.Join(", ", recursionCheck)));
                            }
                            else
                                recursionCheck = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                            recursionCheck.Add(scriptName);
                            try
                            {
                                var code = scriptManager.GetScriptText(scriptName);
                                if (code == null)
                                    return string.Format(CultureInfo.CurrentCulture, errorLines,
                                        string.Format(CultureInfo.CurrentCulture, "Dynamic script with name '{0}' is not found!", scriptName));

                                if (minimize &&
                                    !scriptName.StartsWith("Bundle.", StringComparison.OrdinalIgnoreCase))
                                {
                                    try
                                    {
                                        var result = cssMinifier.MinifyCss(code, new()
                                        {
                                            LineBreakThreshold = 1000
                                        });

                                        if (!result.HasErrors)
                                            code = result.Code;
                                    }
                                    catch (Exception ex)
                                    {
                                        logger?.LogError(ex, "Error while minifying {script}", scriptName);
                                    }
                                }

                                return RewriteUrls("~/DynJS.axd/" + scriptName, code, "../");
                            }
                            finally
                            {
                                recursionCheck.Remove(scriptName);
                            }
                        });

                        continue;
                    }

                    string sourceUrl = BundleUtils.ExpandVersionVariable(hostEnvironment.WebRootFileProvider, sourceFile);
                    if (!sourceUrl.StartsWith("~/", StringComparison.Ordinal))
                        continue;

                    registerInBundle(sourceUrl);
                    var sourcePath = sourceUrl[2..];

                    parts.Add(() =>
                    {
                        var stack = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { sourceUrl };
                        return LoadCssFile(sourcePath, sourceUrl, minimize,
                            settings.UseMinCSS == true, bundleRewriteRoot,
                            processImports, noMinimize, dedupHolder[0], stack);
                    });
                }

                if (parts.Count > 0)
                {
                    bundleParts.Add(() =>
                    {
                        dedupHolder[0] = dedupImports
                            ? new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                            : null;

                        try
                        {
                            var sb = new StringBuilder();
                            foreach (var part in parts)
                            {
                                if (sb.Length > 0)
                                    sb.Append("\n\n");

                                sb.Append(part());
                            }

                            return sb.ToString();
                        }
                        finally
                        {
                            dedupHolder[0] = null;
                        }
                    });
                }

                var bundle = new ConcatenatedScript(bundleParts, "\n\n", checkRights: (permissions, localizer) =>
                {
                    foreach (var scriptName in scriptNames)
                    {
                        if (recursionCheck != null)
                        {
                            if (recursionCheck.Contains(scriptName) || recursionCheck.Count > 100)
                                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                                    "Caught infinite recursion with dynamic scripts '{0}'!",
                                        string.Join(", ", recursionCheck)));
                        }
                        else
                            recursionCheck = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                        recursionCheck.Add(scriptName);
                        try
                        {
                            scriptManager.CheckScriptRights(scriptName);
                        }
                        finally
                        {
                            recursionCheck.Remove(scriptName);
                        }
                    }
                })
                {
                    Permission = SpecialPermissionKeys.Public
                };

                scriptManager.Register(bundleName, bundle);
                bundleKeys.Add(bundleKey);
            }

            isEnabled = true;
        }
    }

    /// <inheritdoc/>
    public bool IsEnabled
    {
        get
        {
            lock (sync)
                return isEnabled;
        }
    }

    /// <inheritdoc/>
    public void CssChanged()
    {
        BundleUtils.ClearVersionCache();

        lock (sync)
        {
            if (bundleKeys == null)
                return;

            foreach (var bundleKey in bundleKeys)
                scriptManager.Changed("CssBundle." + bundleKey);

            Reset();
        }
    }

    /// <inheritdoc/>
    public IEnumerable<string> GetBundleIncludes(string bundleKey)
    {
        lock (sync)
        {
            var bi = bundleIncludes;
            if (bi != null && bi.TryGetValue(bundleKey, out List<string>? includes) && includes != null)
                return includes;

            return [];
        }
    }

    private static string RewriteUrl(string contentPath, string contentRelative, string rootPrefix)
    {
        if (string.IsNullOrWhiteSpace(contentRelative) ||
            (contentRelative.Contains("://", StringComparison.Ordinal)))
            return contentRelative;

        contentRelative = contentRelative.TrimStart();
        if (string.IsNullOrWhiteSpace(contentRelative) || contentRelative[0] == '/')
            return contentRelative;

        if (contentRelative.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            return contentRelative;

        var question = contentRelative.IndexOf('?', StringComparison.Ordinal);
        string? query = null;
        if (question >= 0)
        {
            query = contentRelative[question..];
            contentRelative = contentRelative[..question];
        }

        var rewrittenUrl = new Uri("x:" + contentPath + contentRelative).AbsolutePath[2..];
        if (question >= 0)
            rewrittenUrl += query;

        if (rootPrefix != null && rewrittenUrl.StartsWith('/'))
            rewrittenUrl = rootPrefix + rewrittenUrl[1..];

        return rewrittenUrl;
    }

    private static string RewriteUrls(string contentPath, string content, string rootPrefix)
    {
        if (string.IsNullOrEmpty(content) ||
            string.IsNullOrEmpty(contentPath))
            return content;

        contentPath = (System.IO.Path.GetDirectoryName(contentPath) ?? "")
            .Replace('\\', '/');

        if (string.IsNullOrWhiteSpace(contentPath))
            return content;

        if (contentPath.StartsWith('~'))
            contentPath = contentPath[1..];

        if (!contentPath.EndsWith("/", StringComparison.OrdinalIgnoreCase))
            contentPath += "/";

        var regex = CssUrlRegex();
        return regex.Replace(content, match => "url(" +
            match.Groups["prefix"].Value +
            RewriteUrl(contentPath, match.Groups["url"].Value, rootPrefix) +
            match.Groups["suffix"].Value + ")");
    }

    private string LoadCssFile(string sourcePath, string sourceUrl,
        bool minimize, bool useMinCSS, string rootPrefix, bool processImports,
        HashSet<string> noMinimize, HashSet<string>? dedup, HashSet<string> stack)
    {
        var info = hostEnvironment.WebRootFileProvider.GetFileInfo(sourcePath);
        if (!info.Exists)
            return string.Format(CultureInfo.CurrentCulture, errorLines,
                string.Format(CultureInfo.CurrentCulture, "File {0} is not found!", sourcePath));

        var minifyThis = minimize &&
            !noMinimize.Contains(sourceUrl) &&
            !sourcePath.EndsWith(".min.css", StringComparison.OrdinalIgnoreCase);

        string code;
        var usedMin = false;

        if (minifyThis && useMinCSS)
        {
            var minPath = System.IO.Path.ChangeExtension(sourcePath, ".min.css");
            var minInfo = hostEnvironment.WebRootFileProvider.GetFileInfo(minPath);
            if (minInfo.Exists)
            {
                using var sr = new System.IO.StreamReader(minInfo.CreateReadStream());
                code = sr.ReadToEnd();
                usedMin = true;
            }
            else
            {
                using var sr = new System.IO.StreamReader(info.CreateReadStream());
                code = sr.ReadToEnd();
            }
        }
        else
        {
            using var sr = new System.IO.StreamReader(info.CreateReadStream());
            code = sr.ReadToEnd();
        }

        code = SourceMappingRegex().Replace(code, "");

        if (!processImports)
        {
            if (minifyThis && !usedMin)
                code = MinifyCss(code, sourceUrl);

            return RewriteUrls(sourceUrl, code, rootPrefix);
        }

        var pos = 0;
        var imports = new List<(string Raw, string Url, string Media)>();
        var match = ImportRegex().Match(code);
        while (match.Success && match.Index == pos)
        {
            imports.Add((match.Value, ExtractImportUrl(match.Groups["st"].Value),
                match.Groups["media"].Value.Trim()));
            pos = match.Index + match.Length;
            match = ImportRegex().Match(code, pos);
        }

        // only the file's own content (excluding the leading @import lines) is
        // minified here; imported files are minified independently on their own
        var rest = RewriteUrls(sourceUrl, code[pos..], rootPrefix);

        if (minifyThis && !usedMin)
            rest = MinifyCss(rest, sourceUrl);

        if (imports.Count == 0)
            return rest;

        var sb = new StringBuilder();

        foreach (var import in imports)
        {
            if (import.Media.Length > 0 || !IsRelativeImportUrl(import.Url))
            {
                sb.Append(import.Raw);
                continue;
            }

            var resolvedPath = ResolveRelativePath(sourcePath, import.Url);
            if (string.IsNullOrEmpty(resolvedPath))
            {
                sb.Append(import.Raw);
                continue;
            }

            var resolvedUrl = "~/" + resolvedPath;

            if (dedup != null && !dedup.Add(resolvedUrl))
                continue;

            if (!stack.Add(resolvedUrl))
                continue;

            try
            {
                sb.Append(LoadCssFile(resolvedPath, resolvedUrl, minimize, useMinCSS,
                    rootPrefix, true, noMinimize, dedup, stack));
            }
            finally
            {
                stack.Remove(resolvedUrl);
            }

            sb.Append('\n');
        }

        sb.Append(rest);
        return sb.ToString();
    }

    private string MinifyCss(string code, string sourceUrl)
    {
        try
        {
            var result = cssMinifier.MinifyCss(code, new()
            {
                LineBreakThreshold = 1000
            });

            if (!result.HasErrors)
                return result.Code;
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error minifying CSS: {sourceFile}", sourceUrl);
        }

        return code;
    }

    private static string ExtractImportUrl(string statement)
    {
        statement = statement.Trim();
        if (statement.StartsWith("url(", StringComparison.OrdinalIgnoreCase))
            statement = statement[4..^1].Trim();

        if (statement.Length >= 2 &&
            ((statement[0] == '"' && statement[^1] == '"') ||
             (statement[0] == '\'' && statement[^1] == '\'')))
            statement = statement[1..^1];

        return statement.Trim();
    }

    private static bool IsRelativeImportUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        return !url.StartsWith("data:", StringComparison.OrdinalIgnoreCase) &&
            !url.StartsWith("//", StringComparison.Ordinal) &&
            !url.StartsWith('/') &&
            !url.Contains("://", StringComparison.Ordinal);
    }

    private static string ResolveRelativePath(string sourcePath, string relativePath)
    {
        var slash = sourcePath.LastIndexOf('/');
        var baseDir = slash >= 0 ? sourcePath[..slash] : "";
        var combined = (baseDir.Length > 0 ? baseDir + "/" : "") + relativePath;

        var segments = new List<string>();
        foreach (var part in combined.Replace('\\', '/').Split('/'))
        {
            if (part.Length == 0 || part == ".")
                continue;

            if (part == "..")
            {
                if (segments.Count > 0)
                    segments.RemoveAt(segments.Count - 1);
                continue;
            }

            segments.Add(part);
        }

        return string.Join("/", segments);
    }

    /// <inheritdoc/>
    public string GetCssBundle(string cssUrl)
    {
        ArgumentNullException.ThrowIfNull(cssUrl);
        Dictionary<string, string>? bySrcUrl;
        lock (sync)
        {
            bySrcUrl = bundleKeyBySourceUrl;
        }

        string? bundleKey;

        if (cssUrl.StartsWith("dynamic://",
            StringComparison.OrdinalIgnoreCase))
        {
            var scriptName = cssUrl[10..];
            if (bySrcUrl == null || !bySrcUrl.TryGetValue(cssUrl, out bundleKey))
            {
                cssUrl = scriptManager.GetScriptInclude(scriptName, ".css");
                return VirtualPathUtility.ToAbsolute(contextAccessor, "~/DynJS.axd/" + cssUrl);
            }
        }
        else
        {
            cssUrl = BundleUtils.ExpandVersionVariable(hostEnvironment.WebRootFileProvider, cssUrl);
            if (cssUrl.StartsWith('/'))
            {
                var rootUrl = VirtualPathUtility.ToAbsolute(contextAccessor, "~/");
                if (cssUrl.StartsWith(rootUrl))
                    cssUrl = "~" + cssUrl;
                else
                    return cssUrl;
            }

            if (bySrcUrl == null ||
                !bySrcUrl.TryGetValue(cssUrl, out bundleKey))
                return cssUrl;
        }

        string include = scriptManager.GetScriptInclude("CssBundle." + bundleKey, ".css");
        return VirtualPathUtility.ToAbsolute(contextAccessor, "~/DynJS.axd/" + include);
    }

    [GeneratedRegex(@"^\s*\/\*\s*[#@]\s?(source(?:Mapping)?URL)=\s*(\S+)\s*\*\/\s*$", RegexOptions.Multiline)]
    private static partial Regex SourceMappingRegex();
    [GeneratedRegex(@"\G(?:/\*.*?\*/|\s)*@import\s+(?<st>url\([^)]*\)|""[^""]*""|'[^']*')\s*(?<media>[^;]*);", RegexOptions.Singleline | RegexOptions.IgnoreCase)]
    private static partial Regex ImportRegex();
    [GeneratedRegex("url\\((?<prefix>['\"]?)(?<url>[^)]+?)(?<suffix>['\"]?)\\)")]
    private static partial Regex CssUrlRegex();
}