using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Serenity.Web;

/// <summary>
/// Default implementation of <see cref="IScriptBundleBuilder"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ScriptBundleBuilder"/> class.
/// </remarks>
/// <param name="options">The script bundling options.</param>
/// <param name="scriptMinifier">The script minifier.</param>
/// <param name="scriptManager">The dynamic script manager.</param>
/// <param name="hostEnvironment">The web host environment.</param>
/// <param name="contextAccessor">The HTTP context accessor.</param>
/// <param name="logger">The logger.</param>
/// <exception cref="ArgumentNullException">One of the required arguments is <c>null</c>.</exception>
public class ScriptBundleBuilder(IOptions<ScriptBundlingOptions> options,
    IScriptMinifier scriptMinifier,
    IDynamicScriptManager scriptManager,
    IWebHostEnvironment hostEnvironment,
    IHttpContextAccessor? contextAccessor = null,
    ILogger<ScriptBundleBuilder>? logger = null) : IScriptBundleBuilder
{
    private const string errorLines = "\r\n//\r\n//!!!ERROR: {0}!!!\r\n//\r\n";

    private readonly IScriptMinifier scriptMinifier = scriptMinifier ?? throw new ArgumentNullException(nameof(scriptMinifier));
    private readonly IDynamicScriptManager scriptManager = scriptManager ?? throw new ArgumentNullException(nameof(scriptManager));
    private readonly IWebHostEnvironment hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
    private readonly IHttpContextAccessor? contextAccessor = contextAccessor;
    private readonly ILogger<ScriptBundleBuilder>? logger = logger;
    private readonly ScriptBundlingOptions options = (options ?? throw new ArgumentNullException(nameof(options))).Value;

    [ThreadStatic]
    private static HashSet<string>? recursionCheck;

    /// <inheritdoc/>
    public DynamicScript BuildBundle(IEnumerable<string> sourceFiles)
    {
        ArgumentNullException.ThrowIfNull(sourceFiles);

        bool minimize = options.Minimize == true;
        var noMinimize = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (options.NoMinimize != null)
            noMinimize.AddRange(options.NoMinimize);

        var bundleParts = new List<Func<string>>();
        var scriptNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var sourceFile in sourceFiles)
        {
            if (string.IsNullOrEmpty(sourceFile))
                continue;

            if (sourceFile.StartsWith("dynamic://", StringComparison.OrdinalIgnoreCase))
            {
                var scriptName = sourceFile[10..];
                scriptNames.Add(scriptName);
                bundleParts.Add(() => LoadDynamicScript(scriptName, minimize));
                continue;
            }

            string sourceUrl = BundleUtils.ExpandVersionVariable(hostEnvironment.WebRootFileProvider, sourceFile);
            sourceUrl = VirtualPathUtility.ToAbsolute(contextAccessor, sourceUrl);
            var rootUrl = VirtualPathUtility.ToAbsolute(contextAccessor, "~/");

            if (string.IsNullOrEmpty(sourceUrl) || !sourceUrl.StartsWith(rootUrl, StringComparison.Ordinal))
                continue;

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
                    if (options.UseMinJS == true)
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
                        var result = scriptMinifier.MinifyScript(code, new()
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

        return new ConcatenatedScript(bundleParts, checkRights: (permissions, localizer) =>
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
                    recursionCheck = NewHashSet();

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
    }

    private static HashSet<string> NewHashSet()
    {
        var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        return set;
    }

    private string LoadDynamicScript(string scriptName, bool minimize)
    {
        if (recursionCheck != null)
        {
            if (recursionCheck.Contains(scriptName) || recursionCheck.Count > 100)
                return string.Format(CultureInfo.CurrentCulture, errorLines,
                    string.Format(CultureInfo.CurrentCulture, "Caught infinite recursion with dynamic scripts '{0}'!",
                        string.Join(", ", recursionCheck)));
        }
        else
            recursionCheck = NewHashSet();

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
                    var result = scriptMinifier.MinifyScript(code, new()
                    {
                        LineBreakThreshold = 1000
                    });
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
    }
}
