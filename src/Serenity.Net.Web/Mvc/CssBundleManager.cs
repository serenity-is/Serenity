using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Serenity.Web;

/// <summary>
/// Default implementation for <see cref="ICssBundleManager"/>
/// </summary>
public class CssBundleManager : ICssBundleManager
{
    private readonly object sync = new();

    private bool isEnabled;
    private Dictionary<string, string> bundleKeyBySourceUrl;
    private Dictionary<string, HashSet<string>> bundleKeysBySourceUrl;
    private HashSet<string> bundleKeys;
    private Dictionary<string, List<string>> bundleIncludes;

    private const string errorLines = "\r\n/*\r\n!!!ERROR: {0}!!!\r\n*/\r\n";
    private readonly IDynamicScriptManager scriptManager;
    private readonly IWebHostEnvironment hostEnvironment;
    private readonly IHttpContextAccessor contextAccessor;
    private readonly ILogger<CssBundleManager> logger;
    private readonly CssBundlingOptions options;

    [ThreadStatic]
    private static HashSet<string> recursionCheck;

    /// <summary>
    /// Creates an instance of the class
    /// </summary>
    /// <param name="options">Options</param>
    /// <param name="scriptManager">Dynamic script manager</param>
    /// <param name="hostEnvironment">Web host environment</param>
    /// <param name="contextAccessor">HTTP context accessor</param>
    /// <param name="logger">Exception logger</param>
    /// <exception cref="ArgumentNullException"></exception>
    public CssBundleManager(IOptions<CssBundlingOptions> options, IDynamicScriptManager scriptManager, IWebHostEnvironment hostEnvironment,
        IHttpContextAccessor contextAccessor = null, ILogger<CssBundleManager> logger = null)
    {
        this.options = (options ?? throw new ArgumentNullException(nameof(options))).Value;
        this.scriptManager = scriptManager ?? throw new ArgumentNullException(nameof(scriptManager));
        this.hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
        this.contextAccessor = contextAccessor;
        this.logger = logger;

        Reset();
        scriptManager.ScriptChanged += name =>
        {
            HashSet<string> bundleKeys;
            lock (sync)
            {
                if (bundleKeysBySourceUrl == null ||
                    !bundleKeysBySourceUrl.TryGetValue("dynamic://" + name, out bundleKeys))
                    bundleKeys = null;
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
            var bundles = settings.Bundles ?? new Dictionary<string, string[]>();

            foreach (var key in bundles.Keys.ToArray())
            {
                var parts = bundles[key];
                if (parts != null &&
                    !bundles.Keys.Contains(key + ".rtl") &&
                    parts.Any(x => x != null &&
                        x.Contains("{.rtl}", StringComparison.OrdinalIgnoreCase)))
                {
                    bundles[key + ".rtl"] = parts.Select(x => x
                        .Replace("{.rtl}", ".rtl", StringComparison.OrdinalIgnoreCase)).ToArray();
                    bundles[key] = parts.Select(x => x
                        .Replace("{.rtl}", "", StringComparison.OrdinalIgnoreCase)).ToArray();
                }
            }

            bundles = bundles.Keys.ToDictionary(k => k,
                k => (bundles[k] ?? Array.Empty<string>())
                    .Select(u => BundleUtils.DoReplacements(u, settings.Replacements))
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
                var scriptNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                void registerInBundle(string appRelativeUrl)
                {
                    if (bundleKey.IndexOf('/', StringComparison.Ordinal) < 0 && 
                        !bundleKeyBySourceUrl.ContainsKey(appRelativeUrl))
                        bundleKeyBySourceUrl[appRelativeUrl] = bundleKey;

                    if (!bundleKeysBySourceUrl.TryGetValue(appRelativeUrl, out HashSet<string> bundleKeys))
                    {
                        bundleKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                        bundleKeysBySourceUrl[appRelativeUrl] = new HashSet<string>();
                    }

                    bundleKeys.Add(bundleKey);
                }

                foreach (var sourceFile in sourceFiles)
                {
                    if (sourceFile.IsNullOrEmpty())
                        continue;

                    if (sourceFile.StartsWith("dynamic://", StringComparison.OrdinalIgnoreCase))
                    {
                        registerInBundle(sourceFile);

                        var scriptName = sourceFile[10..];
                        scriptNames.Add(scriptName);
                        bundleParts.Add(() =>
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
                                        var result = NUglify.Uglify.Css(code);
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

                    bundleParts.Add(() =>
                    {
                        var sourceInfo = hostEnvironment.WebRootFileProvider.GetFileInfo(sourcePath);
                        if (!sourceInfo.Exists)
                            return string.Format(CultureInfo.CurrentCulture, errorLines, 
                                string.Format(CultureInfo.CurrentCulture, "File {0} is not found!", sourcePath));

                        string code = null;

                        if (minimize &&
                            !noMinimize.Contains(sourceFile) &&
                            !sourceFile.EndsWith(".min.css", StringComparison.OrdinalIgnoreCase))
                        {
                            if (settings.UseMinCSS == true)
                            {
                                var minPath = System.IO.Path.ChangeExtension(sourcePath, ".min.css");
                                var minInfo = hostEnvironment.WebRootFileProvider.GetFileInfo(minPath);
                                if (minInfo.Exists)
                                {
                                    sourcePath = minPath;
                                    using var sr = new System.IO.StreamReader(minInfo.CreateReadStream());
                                    code = sr.ReadToEnd();
                                }
                            }

                            if (code == null)
                            {
                                using (var sr = new System.IO.StreamReader(sourceInfo.CreateReadStream()))
                                    code = sr.ReadToEnd();

                                try
                                {
                                    var result = NUglify.Uglify.Css(code, new NUglify.Css.CssSettings
                                    {
                                        LineBreakThreshold = 1000
                                    });
                                    if (!result.HasErrors)
                                        code = result.Code;
                                }
                                catch (Exception ex)
                                {
                                    logger?.LogError(ex, "Error minifying CSS: {sourceFile}", sourceFile);
                                }
                            }
                        }
                        else
                        {
                            using var sr = new System.IO.StreamReader(sourceInfo.CreateReadStream());
                            code = sr.ReadToEnd();
                        }

                        code = new Regex(@"^\s*\/\*\s*[#@]\s?(source(?:Mapping)?URL)=\s*(\S+)\s*\*\/\s*$", RegexOptions.Multiline)
                            .Replace(code, "");

                        code = RewriteUrls(sourceUrl, code, bundleRewriteRoot);
                        return code;
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
                });

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
            if (bi != null && bi.TryGetValue(bundleKey, out List<string> includes) && includes != null)
                return includes;

            return Array.Empty<string>();
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
        string query = null;
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

        contentPath = System.IO.Path.GetDirectoryName(contentPath)
            .Replace('\\', '/');

        if (string.IsNullOrWhiteSpace(contentPath))
            return content;

        if (contentPath.StartsWith("~", StringComparison.Ordinal))
            contentPath = contentPath[1..];

        if (!contentPath.EndsWith("/", StringComparison.OrdinalIgnoreCase))
            contentPath += "/";

        var regex = new Regex("url\\((?<prefix>['\"]?)(?<url>[^)]+?)(?<suffix>['\"]?)\\)");
        return regex.Replace(content, (Match match) => "url(" +
            match.Groups["prefix"].Value +
            RewriteUrl(contentPath, match.Groups["url"].Value, rootPrefix) +
            match.Groups["suffix"].Value + ")");
    }

    /// <inheritdoc/>
    public string GetCssBundle(string cssUrl)
    {
        Dictionary<string, string> bySrcUrl;
        lock (sync)
        {
            bySrcUrl = bundleKeyBySourceUrl;
        }

        string bundleKey;

        if (cssUrl != null && cssUrl.StartsWith("dynamic://",
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
            if (cssUrl.StartsWith("/", StringComparison.Ordinal))
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