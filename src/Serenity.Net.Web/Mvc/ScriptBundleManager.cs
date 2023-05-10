using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Serenity.Web;

/// <summary>
/// Default implementation for <see cref="IScriptBundleManager"/>
/// </summary>
public class ScriptBundleManager : IScriptBundleManager
{
    private static readonly object sync = new();

    private bool isEnabled;
    private Dictionary<string, string> bundleKeyBySourceUrl;
    private Dictionary<string, HashSet<string>> bundleKeysBySourceUrl;
    private HashSet<string> bundleKeys;
    private Dictionary<string, List<string>> bundleIncludes;

    private const string errorLines = "\r\n//\r\n//!!!ERROR: {0}!!!\r\n//\r\n";
    private readonly IDynamicScriptManager scriptManager;
    private readonly IWebHostEnvironment hostEnvironment;
    private readonly IHttpContextAccessor contextAccessor;
    private readonly ILogger<ScriptBundleManager> logger;
    private readonly ScriptBundlingOptions options;

    [ThreadStatic]
    private static HashSet<string> recursionCheck;

    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    /// <param name="options">Options</param>
    /// <param name="scriptManager">Dynamic script manager</param>
    /// <param name="hostEnvironment">Web host environment</param>
    /// <param name="contextAccessor">HTTP context accessor</param>
    /// <param name="logger">Logger</param>
    /// <exception cref="ArgumentNullException">One of arguments is null</exception>
    public ScriptBundleManager(IOptions<ScriptBundlingOptions> options, IDynamicScriptManager scriptManager, IWebHostEnvironment hostEnvironment,
        IHttpContextAccessor contextAccessor = null, ILogger<ScriptBundleManager> logger = null)
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
            var bundles = settings.Bundles ?? new Dictionary<string, string[]>();

            bundles = bundles.Keys.ToDictionary(k => k,
                k => (bundles[k] ?? Array.Empty<string>())
                    .Select(u => BundleUtils.DoReplacements(u, settings.Replacements))
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
                var bundleName = "Bundle." + bundleKey;
                var bundleParts = new List<Func<string>>();
                var scriptNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                void registerInBundle(string sourceFile)
                {
                    if (bundleKey.IndexOf('/', StringComparison.Ordinal) < 0 && !bundleKeyBySourceUrl.ContainsKey(sourceFile))
                        bundleKeyBySourceUrl[sourceFile] = bundleKey;

                    if (!bundleKeysBySourceUrl.TryGetValue(sourceFile, out HashSet<string> bundleKeys))
                    {
                        bundleKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                        bundleKeysBySourceUrl[sourceFile] = new HashSet<string>();
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
                                        var result = NUglify.Uglify.Js(code);
                                        if (!result.HasErrors)
                                            code = result.Code;
                                    }
                                    catch (Exception ex)
                                    {
                                        logger?.LogError(ex, "Error minifying script: {script}", scriptName);
                                    }
                                }

                                return code;
                            }
                            finally
                            {
                                recursionCheck.Remove(scriptName);
                            }
                        });

                        continue;
                    }

                    string sourceUrl = BundleUtils.ExpandVersionVariable(hostEnvironment.WebRootFileProvider, sourceFile);
                    sourceUrl = VirtualPathUtility.ToAbsolute(contextAccessor, sourceUrl);
                    var rootUrl = VirtualPathUtility.ToAbsolute(contextAccessor, "~/");

                    if (sourceUrl.IsNullOrEmpty() || !sourceUrl.StartsWith(rootUrl, StringComparison.Ordinal))
                        continue;

                    registerInBundle(sourceUrl);

                    bundleParts.Add(() =>
                    {
                        var sourcePath = sourceUrl[rootUrl.Length..];
                        var sourceInfo = hostEnvironment.WebRootFileProvider.GetFileInfo(sourcePath);
                        if (!sourceInfo.Exists)
                            return string.Format(CultureInfo.CurrentCulture, errorLines, string.Format(CultureInfo.CurrentCulture, 
                                "File {0} is not found!", sourcePath));

                        if (minimize &&
                            !noMinimize.Contains(sourceFile) &&
                            !sourceFile.EndsWith(".min.js", StringComparison.OrdinalIgnoreCase))
                        {
                            if (settings.UseMinJS == true)
                            {
                                var minPath = System.IO.Path.ChangeExtension(sourcePath, ".min.js");
                                var minInfo = hostEnvironment.WebRootFileProvider.GetFileInfo(minPath);
                                if (minInfo.Exists)
                                {
                                    sourcePath = minPath;
                                    using var sr = new System.IO.StreamReader(minInfo.CreateReadStream());
                                    return sr.ReadToEnd();
                                }
                            }

                            string code;
                            using (var sr = new System.IO.StreamReader(sourceInfo.CreateReadStream()))
                                code = sr.ReadToEnd();

                            try
                            {
                                var result = NUglify.Uglify.Js(code, new NUglify.JavaScript.CodeSettings 
                                { 
                                    LineBreakThreshold = 1000 
                                });
                                if (result.HasErrors)
                                    return code;
                                return result.Code;
                            }
                            catch (Exception ex)
                            {
                                logger?.LogError(ex, "Error minifying script: {script}", sourceFile);
                                return code;
                            }
                        }

                        using (var sr = new System.IO.StreamReader(sourceInfo.CreateReadStream()))
                            return sr.ReadToEnd();
                    });
                }

                var bundle = new ConcatenatedScript(bundleParts, checkRights: (permissions, localizer) =>
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
            if (bi != null && bi.TryGetValue(bundleKey, out List<string> includes) && includes != null)
                return includes;

            return Array.Empty<string>();
        }
    }

    /// <inheritdoc/>
    public string GetScriptBundle(string scriptUrl)
    {
        Dictionary<string, string> bySrcUrl;
        lock (sync)
        {
            bySrcUrl = bundleKeyBySourceUrl;
        }

        string bundleKey;
        if (scriptUrl != null && scriptUrl.StartsWith("dynamic://",
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