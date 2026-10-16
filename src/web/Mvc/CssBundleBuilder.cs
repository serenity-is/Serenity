using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Serenity.Web;

/// <summary>
/// Default implementation of <see cref="ICssBundleBuilder"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="CssBundleBuilder"/> class.
/// </remarks>
/// <param name="options">The CSS bundling options.</param>
/// <param name="scriptManager">The dynamic script manager.</param>
/// <param name="cssMinifier">The CSS minifier.</param>
/// <param name="hostEnvironment">The web host environment.</param>
/// <param name="logger">The logger.</param>
/// <exception cref="ArgumentNullException">One of the required arguments is <c>null</c>.</exception>
public partial class CssBundleBuilder(IOptions<CssBundlingOptions> options,
    IDynamicScriptManager scriptManager,
    ICssMinifier cssMinifier,
    IWebHostEnvironment hostEnvironment,
    ILogger<CssBundleBuilder>? logger = null) : ICssBundleBuilder
{
    private const string errorLines = "\r\n/*\r\n!!!ERROR: {0}!!!\r\n*/\r\n";

    private readonly IDynamicScriptManager scriptManager = scriptManager ?? throw new ArgumentNullException(nameof(scriptManager));
    private readonly ICssMinifier cssMinifier = cssMinifier ?? throw new ArgumentNullException(nameof(cssMinifier));
    private readonly IWebHostEnvironment hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
    private readonly ILogger<CssBundleBuilder>? logger = logger;
    private readonly CssBundlingOptions options = (options ?? throw new ArgumentNullException(nameof(options))).Value;

    [ThreadStatic]
    private static HashSet<string>? recursionCheck;

    /// <inheritdoc/>
    public DynamicScript BuildBundle(string rootPrefix, IEnumerable<string> sourceFiles)
    {
        ArgumentNullException.ThrowIfNull(sourceFiles);

        rootPrefix ??= "";

        bool minimize = options.Minimize == true;
        bool processImports = options.ProcessImports != false;
        bool dedupImports = options.DedupImports == true;
        var noMinimize = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (options.NoMinimize != null)
            noMinimize.AddRange(options.NoMinimize);

        var bundleParts = new List<Func<string>>();
        var parts = new List<Func<string>>();
        var dedupHolder = new HashSet<string>?[1];
        var scriptNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var sourceFile in sourceFiles)
        {
            if (string.IsNullOrEmpty(sourceFile))
                continue;

            if (sourceFile.StartsWith("dynamic://", StringComparison.OrdinalIgnoreCase))
            {
                var scriptName = sourceFile[10..];
                scriptNames.Add(scriptName);
                parts.Add(() => LoadDynamicScript(scriptName, minimize, rootPrefix));
                continue;
            }

            string sourceUrl = BundleUtils.ExpandVersionVariable(hostEnvironment.WebRootFileProvider, sourceFile);
            if (!sourceUrl.StartsWith("~/", StringComparison.Ordinal))
                continue;

            var sourcePath = sourceUrl[2..];

            parts.Add(() =>
            {
                var stack = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { sourceUrl };
                return LoadCssFile(sourcePath, sourceUrl, minimize,
                    options.UseMinCSS == true, rootPrefix,
                    processImports, noMinimize, dedupHolder[0], stack);
            });
        }

        if (parts.Count > 0)
        {
            bundleParts.Add(() =>
            {
                dedupHolder[0] = dedupImports ? NewHashSet() : null;

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

        return new ConcatenatedScript(bundleParts, "\n\n", checkRights: (permissions, localizer) =>
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

    private string LoadDynamicScript(string scriptName, bool minimize, string rootPrefix)
    {
        if (recursionCheck != null)
        {
            if (recursionCheck.Contains(scriptName) || recursionCheck.Count > 100)
                return string.Format(CultureInfo.CurrentCulture, errorLines,
                    string.Format(CultureInfo.CurrentCulture, "Caught infinite recursion with dynamic scripts '{0}'!",
                        string.Join(", ", recursionCheck)));
        }
        else
            recursionCheck = new(StringComparer.OrdinalIgnoreCase);

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

            return RewriteUrls("~/DynJS.axd/" + scriptName, code, rootPrefix);
        }
        finally
        {
            recursionCheck.Remove(scriptName);
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

    [GeneratedRegex(@"^\s*\/\*\s*[#@]\s?(source(?:Mapping)?URL)=\s*(\S+)\s*\*\/\s*$", RegexOptions.Multiline)]
    private static partial Regex SourceMappingRegex();
    [GeneratedRegex(@"\G(?:/\*.*?\*/|\s)*@import\s+(?<st>url\([^)]*\)|""[^""]*""|'[^']*')\s*(?<media>[^;]*);", RegexOptions.Singleline | RegexOptions.IgnoreCase)]
    private static partial Regex ImportRegex();
    [GeneratedRegex("url\\((?<prefix>['\"]?)(?<url>[^)]+?)(?<suffix>['\"]?)\\)")]
    private static partial Regex CssUrlRegex();
}
