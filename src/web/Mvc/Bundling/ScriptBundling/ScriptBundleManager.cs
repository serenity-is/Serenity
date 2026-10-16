using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Serenity.Web;

/// <summary>
/// Default implementation of <see cref="IScriptBundleManager"/>.
/// </summary>
public class ScriptBundleManager : IScriptBundleManager
{
    private static readonly Lock sync = new();

    private bool isEnabled;
    private Dictionary<string, string>? bundleKeyBySourceUrl;
    private Dictionary<string, HashSet<string>>? bundleKeysBySourceUrl;
    private HashSet<string>? bundleKeys;
    private Dictionary<string, List<string>>? bundleIncludes;

    private readonly IScriptBundleBuilder bundleBuilder;
    private readonly IDynamicScriptManager scriptManager;
    private readonly IWebHostEnvironment hostEnvironment;
    private readonly IHttpContextAccessor? contextAccessor;
    private readonly ScriptBundlingOptions options;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScriptBundleManager"/> class.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="bundleBuilder">The script bundle builder.</param>
    /// <param name="scriptManager">The dynamic script manager.</param>
    /// <param name="hostEnvironment">The web host environment.</param>
    /// <param name="contextAccessor">The HTTP context accessor.</param>
    /// <exception cref="ArgumentNullException">One of the required arguments is <c>null</c>.</exception>
    public ScriptBundleManager(IOptions<ScriptBundlingOptions> options,
        IScriptBundleBuilder bundleBuilder,
        IDynamicScriptManager scriptManager,
        IWebHostEnvironment hostEnvironment,
        IHttpContextAccessor? contextAccessor = null)
    {
        this.options = (options ?? throw new ArgumentNullException(nameof(options))).Value;
        this.bundleBuilder = bundleBuilder ?? throw new ArgumentNullException(nameof(bundleBuilder));
        this.scriptManager = scriptManager ?? throw new ArgumentNullException(nameof(scriptManager));
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
                {
                }
            }

            if (bundleKeys != null)
                foreach (var bundleKey in bundleKeys)
                    scriptManager.Changed("Bundle." + bundleKey);
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

            bundles = bundles.Keys.ToDictionary(k => k,
                k => (bundles[k] ?? [])
                    .Select(u => BundleUtils.DoReplacements(u, settings.Replacements))
                    .OfType<string>()
                    .Where(u => !string.IsNullOrEmpty(u))
                    .ToArray());

            bundleIncludes = BundleUtils.ExpandBundleIncludes(bundles, "dynamic://Bundle.", "script");

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
                var bundleName = "Bundle." + bundleKey;

                void registerInBundle(string sourceFile)
                {
                    if (bundleKey.IndexOf('/', StringComparison.Ordinal) < 0 && !bundleKeyBySourceUrl.ContainsKey(sourceFile))
                        bundleKeyBySourceUrl[sourceFile] = bundleKey;

                    if (!bundleKeysBySourceUrl.TryGetValue(sourceFile, out HashSet<string>? bundleKeys))
                    {
                        bundleKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                        bundleKeysBySourceUrl[sourceFile] = bundleKeys;
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
                    sourceUrl = VirtualPathUtility.ToAbsolute(contextAccessor, sourceUrl);
                    var rootUrl = VirtualPathUtility.ToAbsolute(contextAccessor, "~/");

                    if (string.IsNullOrEmpty(sourceUrl) || !sourceUrl.StartsWith(rootUrl, StringComparison.Ordinal))
                        continue;

                    registerInBundle(sourceUrl);
                }

                scriptManager.Register(bundleName, bundleBuilder.BuildBundle(sourceFiles));
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
    public void ScriptsChanged()
    {
        BundleUtils.ClearVersionCache();

        lock (sync)
        {
            if (bundleKeys == null)
                return;

            foreach (var bundleKey in bundleKeys)
                scriptManager.Changed("Bundle." + bundleKey);

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
    public string GetScriptBundle(string scriptUrl)
    {
        ArgumentNullException.ThrowIfNull(scriptUrl);

        Dictionary<string, string>? bySrcUrl;
        lock (sync)
        {
            bySrcUrl = bundleKeyBySourceUrl;
        }

        string? bundleKey;
        if (scriptUrl.StartsWith("dynamic://",
            StringComparison.OrdinalIgnoreCase))
        {
            var scriptName = scriptUrl[10..];
            if (bySrcUrl == null || !bySrcUrl.TryGetValue(scriptUrl, out bundleKey))
            {
                scriptUrl = scriptManager.GetScriptInclude(scriptName);
                return VirtualPathUtility.ToAbsolute(contextAccessor, "~/DynJS.axd/" + scriptUrl);
            }
        }
        else
        {
            scriptUrl = BundleUtils.ExpandVersionVariable(hostEnvironment.WebRootFileProvider, scriptUrl);
            scriptUrl = VirtualPathUtility.ToAbsolute(contextAccessor, scriptUrl);

            if (bySrcUrl == null ||
                !bySrcUrl.TryGetValue(scriptUrl, out bundleKey))
                return scriptUrl;
        }

        string include = scriptManager.GetScriptInclude("Bundle." + bundleKey);
        return VirtualPathUtility.ToAbsolute(contextAccessor, "~/DynJS.axd/" + include);
    }
}
