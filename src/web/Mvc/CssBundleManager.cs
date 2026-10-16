using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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

    private readonly IDynamicScriptManager scriptManager;
    private readonly ICssBundleBuilder bundleBuilder;
    private readonly IWebHostEnvironment hostEnvironment;
    private readonly IHttpContextAccessor? contextAccessor;
    private readonly CssBundlingOptions options;

    /// <summary>
    /// Initializes a new instance of the <see cref="CssBundleManager"/> class.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="scriptManager">The dynamic script manager.</param>
    /// <param name="bundleBuilder">The CSS bundle builder.</param>
    /// <param name="hostEnvironment">The web host environment.</param>
    /// <param name="contextAccessor">The HTTP context accessor.</param>
    /// <exception cref="ArgumentNullException">One of the required arguments is <c>null</c>.</exception>
    public CssBundleManager(IOptions<CssBundlingOptions> options,
        IDynamicScriptManager scriptManager,
        ICssBundleBuilder bundleBuilder,
        IWebHostEnvironment hostEnvironment,
        IHttpContextAccessor? contextAccessor = null)
    {
        this.options = (options ?? throw new ArgumentNullException(nameof(options))).Value;
        this.scriptManager = scriptManager ?? throw new ArgumentNullException(nameof(scriptManager));
        this.bundleBuilder = bundleBuilder ?? throw new ArgumentNullException(nameof(bundleBuilder));
        this.hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
        this.contextAccessor = contextAccessor;

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
                        continue;
                    }

                    string sourceUrl = BundleUtils.ExpandVersionVariable(hostEnvironment.WebRootFileProvider, sourceFile);
                    if (!sourceUrl.StartsWith("~/", StringComparison.Ordinal))
                        continue;

                    registerInBundle(sourceUrl);
                }

                scriptManager.Register(bundleName, bundleBuilder.BuildBundle(bundleRewriteRoot, sourceFiles));
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
}
