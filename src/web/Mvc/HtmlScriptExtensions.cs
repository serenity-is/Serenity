using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net;
using System.Security.Cryptography;
using System.Web;

namespace Serenity.Web;

/// <summary>
/// Contains Serenity related helper methods that can be used in Razor CSHTML files
/// </summary>
public static partial class HtmlScriptExtensions
{

    /// <summary>
    /// Renders a CSS stylesheet link element. If bundling is enabled, it may contain
    /// the bundle URL instead of the CSS URL. If the bundle containing the CSS file
    /// is already rendered in this context, it will return an empty string.
    /// </summary>
    /// <param name="helper">HTML helper</param>
    /// <param name="cssUrl">CSS Url</param>
    /// <exception cref="ArgumentNullException">HTML helper or cssUrl is null</exception>
    public static HtmlString Stylesheet(this IHtmlHelper helper, string cssUrl)
    {
        ArgumentNullException.ThrowIfNull(helper);

        ArgumentNullException.ThrowIfNull(cssUrl);

        if (cssUrl.EndsWith(".js"))
            cssUrl = cssUrl[..^3] + ".css";

        var context = helper.ViewContext.HttpContext;
        var css = context.RequestServices.GetRequiredService<ICssBundleManager>()
            .GetCssBundle(cssUrl);

        if (!IsAlreadyIncluded(context.Items, css))
        {
            return new HtmlString(string.Format(CultureInfo.InvariantCulture,
                "    <link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\"/>\n",
                WebUtility.HtmlEncode(context.RequestServices.GetRequiredService<IContentHashCache>()
                    .ResolveWithHash(context.Request.PathBase, css))));
        }
        else
            return HtmlString.Empty;
    }

    /// <summary>
    /// Automatically includes corresponding .css file for an ES module if it exists next to
    /// the .js file
    /// </summary>
    /// <param name="helper">HTML helper</param>
    /// <param name="module">ES module</param>
    /// <returns></returns>
    public static HtmlString AutoIncludeModuleCss(this IHtmlHelper helper, string module)
    {
        if (string.IsNullOrEmpty(module))
            return HtmlString.Empty;

        if (module.EndsWith(".js", StringComparison.Ordinal) == true &&
            module.StartsWith("~/", StringComparison.Ordinal) == true &&
            helper.ViewContext.HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>()
                .WebRootFileProvider?.GetFileInfo(module[2..^3] + ".css")?.Exists == true)
        {
            return Stylesheet(helper, module);
        }

        return HtmlString.Empty;
    }

    /// <summary>
    /// Executes default export of a module page, usually pageInit
    /// </summary>
    /// <param name="html"></param>
    /// <param name="module"></param>
    /// <param name="options"></param>
    /// <param name="css"></param>
    /// <returns></returns>
    public static HtmlString ModulePageInit(this IHtmlHelper html, string module, object options = null, bool css = true)
    {
        html.ViewData["ModulePageScript"] ??= module;
        return new HtmlString(
            (css ? AutoIncludeModuleCss(html, module) : HtmlString.Empty).Value +
            $"<script type=\"module\" nonce=\"{html.CspNonce()}\">\n" +
            $"import pageInit from '{html.ResolveWithHash(module)}';\n" +
            $"pageInit({(options != null ? JSON.StringifyIndented(options) : "")});\n" +
            $"</script>");
    }

    /// <summary>
    /// Renders individual link elements for all CSS files in a bundle if bundling is disabled,
    /// and renders a single link element containing the bundle URL if it is enabled.
    /// </summary>
    /// <param name="helper">HTML helper</param>
    /// <param name="bundleKey">Bundle key</param>
    /// <exception cref="ArgumentNullException">Helper or bundleKey is null</exception>
    public static HtmlString StyleBundle(this IHtmlHelper helper, string bundleKey)
    {
        ArgumentNullException.ThrowIfNull(helper);

        if (string.IsNullOrEmpty(bundleKey))
            throw new ArgumentNullException(nameof(bundleKey));

        var context = helper.ViewContext.HttpContext;
        var bundleManager = context.RequestServices.GetRequiredService<ICssBundleManager>();
        var scriptManager = context.RequestServices.GetRequiredService<IDynamicScriptManager>();
        var hostEnvironment = context.RequestServices.GetRequiredService<IWebHostEnvironment>();
        var contentHashCache = context.RequestServices.GetRequiredService<IContentHashCache>();

        if (bundleManager.IsEnabled)
            return Stylesheet(helper, "dynamic://CssBundle." + bundleKey);

        StringBuilder sb = new();
        foreach (var include in bundleManager.GetBundleIncludes(bundleKey))
        {
            var cssUrl = include;
            if (string.IsNullOrEmpty(cssUrl))
                continue;

            if (cssUrl.StartsWith("dynamic://", StringComparison.OrdinalIgnoreCase))
            {
                var scriptName = cssUrl[10..];
                cssUrl = scriptManager.GetScriptInclude(scriptName, ".css");
                cssUrl = VirtualPathUtility.ToAbsolute(context, "~/DynJS.axd/" + cssUrl);
            }
            else
            {
                cssUrl = BundleUtils.ExpandVersionVariable(hostEnvironment.WebRootFileProvider, cssUrl);
                cssUrl = VirtualPathUtility.ToAbsolute(context, cssUrl);
            }

            if (!IsAlreadyIncluded(context.Items, cssUrl))
            {
                sb.AppendLine(string.Format(CultureInfo.InvariantCulture,
                    "    <link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\"/>\n",
                    WebUtility.HtmlEncode(contentHashCache.ResolveWithHash(context.Request.PathBase, cssUrl))));
            }
        }

        return new HtmlString(sb.ToString());
    }

    /// <summary>
    /// Resolves a content URL by adding its hash with "?v=" prefix.
    /// </summary>
    /// <param name="helper">HTML helper</param>
    /// <param name="contentUrl">Content URL</param>
    /// <exception cref="ArgumentNullException">Helper or contentUrl is null</exception>
    public static HtmlString ResolveWithHash(this IHtmlHelper helper, string contentUrl)
    {
        ArgumentNullException.ThrowIfNull(helper);

        if (string.IsNullOrEmpty(contentUrl))
            throw new ArgumentNullException(nameof(contentUrl));

        var context = helper.ViewContext.HttpContext;

        return new HtmlString(WebUtility.HtmlEncode(context.RequestServices.GetRequiredService<IContentHashCache>()
            .ResolveWithHash(context.Request.PathBase, contentUrl)));
    }

    /// <summary>
    /// Renders a script include element. If bundling is enabled, it may contain
    /// the bundle URL instead of the script URL. If the bundle containing the script file
    /// is already rendered in this context, it will return an empty string.
    /// </summary>
    /// <param name="helper">HTML helper</param>
    /// <param name="includeJS">Script url</param>
    /// <exception cref="ArgumentNullException">HTML helper or includeJS is null</exception>
    public static HtmlString Script(this IHtmlHelper helper, string includeJS)
    {
        ArgumentNullException.ThrowIfNull(helper);

        if (string.IsNullOrEmpty(includeJS))
            throw new ArgumentNullException(nameof(includeJS));

        var context = helper.ViewContext.HttpContext;
        var script = context.RequestServices.GetRequiredService<IScriptBundleManager>()
            .GetScriptBundle(includeJS);
        if (!IsAlreadyIncluded(context.Items, script))
        {
            return new HtmlString(string.Format(CultureInfo.InvariantCulture,
                "    <script src=\"{0}\" type=\"text/javascript\"></script>\n",
                WebUtility.HtmlEncode(context.RequestServices.GetRequiredService<IContentHashCache>()
                    .ResolveWithHash(context.Request.PathBase, script))));
        }
        else
            return new HtmlString("");
    }

    /// <summary>
    /// Renders individual script elements for all JS files in a bundle if bundling is disabled,
    /// and renders a single script element containing the bundle URL if it is enabled.
    /// </summary>
    /// <param name="helper">HTML helper</param>
    /// <param name="bundleKey">Bundle key</param>
    /// <exception cref="ArgumentNullException">Helper or bundleKey is null</exception>
    public static HtmlString ScriptBundle(this IHtmlHelper helper, string bundleKey)
    {
        ArgumentNullException.ThrowIfNull(helper);

        if (string.IsNullOrEmpty(bundleKey))
            throw new ArgumentNullException(nameof(bundleKey));

        var context = helper.ViewContext.HttpContext;
        var bundleManager = context.RequestServices.GetRequiredService<IScriptBundleManager>();
        var scriptManager = context.RequestServices.GetRequiredService<IDynamicScriptManager>();
        var hostEnvironment = context.RequestServices.GetRequiredService<IWebHostEnvironment>();
        var contentHashCache = context.RequestServices.GetRequiredService<IContentHashCache>();

        if (bundleManager.IsEnabled)
            return Script(helper, "dynamic://Bundle." + bundleKey);

        StringBuilder sb = new();
        foreach (var include in bundleManager.GetBundleIncludes(bundleKey))
        {
            var scriptUrl = include;
            if (string.IsNullOrEmpty(scriptUrl))
                continue;

            if (scriptUrl.StartsWith("dynamic://", StringComparison.OrdinalIgnoreCase))
            {
                var scriptName = scriptUrl[10..];
                scriptUrl = scriptManager.GetScriptInclude(scriptName);
                scriptUrl = VirtualPathUtility.ToAbsolute(context, "~/DynJS.axd/" + scriptUrl);
            }
            else
            {
                scriptUrl = BundleUtils.ExpandVersionVariable(hostEnvironment.WebRootFileProvider, scriptUrl);
                scriptUrl = VirtualPathUtility.ToAbsolute(context, scriptUrl);
            }

            if (!IsAlreadyIncluded(context.Items, scriptUrl))
            {
                sb.AppendLine(string.Format(CultureInfo.InvariantCulture,
                    "    <script src=\"{0}\" type=\"text/javascript\"></script>\n",
                    WebUtility.HtmlEncode(contentHashCache.ResolveWithHash(context.Request.PathBase, scriptUrl))));
            }
        }

        return new HtmlString(sb.ToString());
    }

    const string IncludedScriptsAndCssKey = "HtmlScriptExtensions:IncludedScriptsAndCss";
    const string NonceKey = "HtmlScriptExtensions:Nonce";

    static readonly Regex EndingWithVersionRegex = EndingWithVersionRegexGen();

    [GeneratedRegex(@"\?v=[0-9a-zA-Z_-]*$", RegexOptions.Compiled)]
    private static partial Regex EndingWithVersionRegexGen();

    private static bool IsAlreadyIncluded(IDictionary<object, object> contextItems, string url)
    {
        if (string.IsNullOrEmpty(url))
            return false;

        var included = (HashSet<string>)contextItems[IncludedScriptsAndCssKey];
        if (included == null)
        {
            included = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            contextItems[IncludedScriptsAndCssKey] = included;
        }

        var urlWithoutVer = EndingWithVersionRegex.Replace(url, "");
        if (included.Contains(url) || included.Contains(urlWithoutVer))
            return true;

        included.Add(url);
        included.Add(urlWithoutVer);
        return false;
    }

    private static string CspNonce(IDictionary<object, object> contextItems)
    {
        ArgumentNullException.ThrowIfNull(contextItems);

        if (contextItems[NonceKey] is string nonce)
            return nonce;

        using var rng = RandomNumberGenerator.Create();
        var nonceBytes = new byte[32];
        rng.GetBytes(nonceBytes);
        contextItems[NonceKey] = nonce = WebEncoders.Base64UrlEncode(nonceBytes);
        return nonce;
    }

    /// <summary>
    /// Gets a nonce value for use in script and style elements.
    /// Automatically generates and stores it in the current HTTP context items and
    /// adds it to the CSP directives.
    /// </summary>
    /// <param name="html">Html helper</param>
    /// <param name="addDirectives">True to add CSP directives for script-src, style-src, font-src</param>
    public static string CspNonce(this IHtmlHelper html, bool addDirectives = true)
    {
        ArgumentNullException.ThrowIfNull(html);
        var cspNonce = CspNonce(html.ViewContext?.HttpContext?.Items);
        if (addDirectives)
        {
            var directiveValue = $"'nonce-{cspNonce}'";
            AddCspDirective(html, "script-src", directiveValue);
            AddCspDirective(html, "style-src", directiveValue);
        }
        return cspNonce;
    }

    private static string CspAutoQuote(string value)
    {
        ArgumentException.ThrowIfNullOrEmpty(value);

        // Auto-quote if it looks like a keyword (alphanumeric, no existing quotes)
        if (!string.IsNullOrEmpty(value) &&
            !value.StartsWith('\'') && !value.StartsWith('\"') &&
            value.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_'))
        {
            return $"'{value}'";
        }
        return value;
    }

    const string CspDirectivesKey = "HtmlScriptExtensions:CspDirectives";

    /// <summary>
    /// Adds a Content Security Policy directive to the current HTTP context items.
    /// </summary>
    /// <param name="html">Html helper</param>
    /// <param name="directiveName">CSP directive name</param>
    /// <param name="values">CSP directive values. Note that these values will be automatically quoted if they look like keywords ([A-Za-z0-9_-] only)
    /// and are not already quoted.</param>
    public static void AddCspDirective(this IHtmlHelper html, string directiveName, params string[] values)
    {
        AddCspDirective(html.ViewContext?.HttpContext?.Items, directiveName, values);
    }

    /// <summary>
    /// Adds a Content Security Policy directive to the current HTTP context items.
    /// </summary>
    /// <param name="context">Http context</param>
    /// <param name="directiveName">CSP directive name</param>
    /// <param name="values">CSP directive values. Note that these values will be automatically quoted if they look like keywords ([A-Za-z0-9_-] only)
    /// and are not already quoted.</param>
    public static void AddCspDirective(this HttpContext context, string directiveName, params string[] values)
    {
        AddCspDirective(context.Items, directiveName, values);
    }

    /// <summary>
    /// Adds a Content Security Policy directive to the current HTTP context items.
    /// </summary>
    /// <param name="controller">Controller</param>
    /// <param name="directiveName">CSP directive name</param>
    /// <param name="values">CSP directive values. Note that these values will be automatically quoted if they look like keywords ([A-Za-z0-9_-] only)
    /// and are not already quoted.</param>
    public static void AddCspDirective(this ControllerBase controller, string directiveName, params string[] values)
    {
        AddCspDirective(controller.HttpContext.Items, directiveName, values);
    }

    private static readonly HashSet<string> allowedCspDirectiveNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "default-src",
        "script-src",
        "style-src",
        "img-src",
        "connect-src",
        "font-src",
        "object-src",
        "media-src",
        "frame-src",
        "worker-src",
        "base-uri",
        "form-action",
        "frame-ancestors",
        "manifest-src",
        "prefetch-src",
        "script-src-attr",
        "style-src-attr"
    };

    private static void AddCspDirective(IDictionary<object, object> contextItems, string directiveName, params string[] values)
    {
        ArgumentNullException.ThrowIfNull(contextItems);
        ArgumentNullException.ThrowIfNull(directiveName);
        ArgumentNullException.ThrowIfNull(values);
        if (!allowedCspDirectiveNames.Contains(directiveName))
            throw new ArgumentException($"Directive name '{directiveName}' is not allowed.", nameof(directiveName));

        if (contextItems[CspDirectivesKey] is not Dictionary<string, HashSet<string>> directives)
            contextItems[CspDirectivesKey] = directives = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

        if (!directives.TryGetValue(directiveName, out var directiveValues))
            directives[directiveName] = directiveValues = new HashSet<string>(StringComparer.Ordinal);

        foreach (var value in values.Where(x => !string.IsNullOrEmpty(x)))
            directiveValues.Add(CspAutoQuote(value));
    }

    /// <summary>
    /// Adds a Content Security Policy script-src directive for the given URL
    /// and returns the URL. This can be used in script include helpers for external URLs.
    /// </summary>
    /// <param name="html">Html helper</param>
    /// <param name="url">Url</param>
    /// <returns></returns>
    public static string AddCspScriptUrl(this IHtmlHelper html, string url)
    {
        AddCspDirective(html, "script-src", url);
        return url;
    }

    /// <summary>
    /// Gets a Content Security Policy directive added via AddCspDirective
    /// merged with any manual values provided. The string includes the final semicolon.
    /// This can be used to render parts of the CSP header or meta tag content.
    /// </summary>
    /// <param name="html">Html helper</param>
    /// <param name="directiveName">CSP directive name</param>
    /// <param name="manualValues">Manual values to include in the directive.
    /// Note that these values will be automatically quoted if they look like keywords ([A-Za-z0-9_-] only)
    /// and are not already quoted.</param>
    public static HtmlString GetCspDirective(this IHtmlHelper html, string directiveName, params string[] manualValues)
    {
        ArgumentNullException.ThrowIfNull(html);
        var directive = GetCspDirective(html.ViewContext?.HttpContext?.Items, directiveName, manualValues);
        return new HtmlString(HttpUtility.HtmlAttributeEncode(directive)
            .Replace("&#39;", "'"));
    }

    private static string GetCspDirective(IDictionary<object, object> contextItems, string directiveName, params string[] manualValues)
    {
        ArgumentNullException.ThrowIfNull(contextItems);
        ArgumentException.ThrowIfNullOrEmpty(directiveName);
        if (!allowedCspDirectiveNames.Contains(directiveName))
            throw new ArgumentException($"Directive name '{directiveName}' is not allowed.", nameof(directiveName));

        var finalSet = new HashSet<string>(StringComparer.Ordinal);
        if (manualValues != null)
            finalSet.UnionWith(manualValues.Where(x => !string.IsNullOrEmpty(x)).Select(CspAutoQuote));

        if ((contextItems[CspDirectivesKey] is Dictionary<string, HashSet<string>> directives) &&
            directives.TryGetValue(directiveName, out var directiveValues))
            finalSet.UnionWith(directiveValues);

        // If 'unsafe-inline' is present, remove any nonce or hash values to avoid CSP ignoring 'unsafe-inline'
        if (finalSet.Contains("'unsafe-inline'"))
            finalSet.RemoveWhere(x => x.StartsWith("'nonce-", StringComparison.Ordinal) ||
                                      x.StartsWith("'sha256-", StringComparison.Ordinal) ||
                                      x.StartsWith("'sha384-", StringComparison.Ordinal) ||
                                      x.StartsWith("'sha512-", StringComparison.Ordinal));

        return directiveName + " " + string.Join(' ', finalSet) + ";";
    }

    /// <summary>
    /// Gets the text content of a local text script
    /// </summary>
    /// <param name="page">HTML helper</param>
    /// <param name="package">Package key</param>
    /// <param name="isPending">True to include pending texts</param>
    public static string GetLocalTextContent(this IHtmlHelper page, string package, bool isPending = false)
    {
        string languageId = CultureInfo.CurrentUICulture.Name.TrimToNull() ?? "invariant";
        string scriptName = Web.LocalTextScript.GetScriptName(package, languageId, isPending);
        var scriptManager = page.ViewContext.HttpContext.RequestServices.GetRequiredService<IDynamicScriptManager>();
        scriptManager.IfNotRegistered(scriptName, () =>
        {
            var services = page.ViewContext.HttpContext.RequestServices;
            var registry = services.GetRequiredService<ILocalTextRegistry>();
            var packages = services.GetRequiredService<IOptions<LocalTextPackages>>();
            var includes = packages.Value.TryGetValue(package, out var s) ? s : "";
            return new LocalTextScript(registry, package, includes, languageId, isPending);
        });

        return scriptManager.GetScriptText(scriptName);
    }

    const string importMapKey = "HtmlScriptExtensions:ImportMap";

    class ImportMap
    {
        public Dictionary<string, string> Imports { get; set; }
        public Dictionary<string, Dictionary<string, string>> Scopes { get; set; }
        public Dictionary<string, string> Integrity { get; set; }
    }

    /// <summary>
    /// Adds an entry to the import map for the current HTML view, associating a module specifier with its address and
    /// optional integrity value.
    /// </summary>
    /// <remarks>If the import map does not exist in the current HTTP context, a new one is created. This
    /// method is typically used in server-side rendering scenarios to manage JavaScript module imports and CSP
    /// headers.</remarks>
    /// <param name="context">Http context.</param>
    /// <param name="specifier">The module specifier to map, such as a package name or relative path. Cannot be null.</param>
    /// <param name="address">The address or URL where the module can be loaded from. Cannot be null.</param>
    /// <param name="integrity">An optional integrity hash for the module, used to verify its contents. If null, no integrity value is set.</param>
    /// <param name="csp">Indicates whether to add a Content Security Policy directive for the module address. Set to <see
    /// langword="true"/> to add the directive; otherwise, <see langword="false"/>.</param>
    /// <param name="overwrite">True (default) to overwrite existing entries if any</param>
    public static void AddImportMapEntry(this HttpContext context, string specifier, string address, string integrity = null,
        bool? csp = null, bool overwrite = true)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(specifier);
        ArgumentNullException.ThrowIfNull(address);
        var contextItems = context.Items;
        if (contextItems == null)
            return;
        if (contextItems[importMapKey] is not ImportMap importMap)
            contextItems[importMapKey] = importMap = new ImportMap();
        importMap.Imports ??= new Dictionary<string, string>(StringComparer.Ordinal);

        if (!overwrite && importMap.Imports.ContainsKey(specifier))
            return;

        if (address.StartsWith("~/", StringComparison.Ordinal) ||
            address.StartsWith('/'))
        {
            address = context.RequestServices.GetService<IContentHashCache>()?
                .ResolveWithHash(context.Request.PathBase, address) ?? address;
            csp ??= false;
        }
        else csp ??= address.StartsWith("http://", StringComparison.Ordinal) ||
            address.StartsWith("https://", StringComparison.Ordinal) ||
            address.StartsWith("data:", StringComparison.Ordinal) ||
            address.StartsWith("blob:", StringComparison.Ordinal);

        importMap.Imports[specifier] = address;
        if (integrity != null)
        {
            importMap.Integrity ??= new Dictionary<string, string>(StringComparer.Ordinal);
            importMap.Integrity[address] = integrity;
        }

        if (csp == true)
            context.AddCspDirective("script-src", address);
    }

    /// <summary>
    /// Adds import map entries for modules provided via Serenity.Assets like tiptap, jspdf, etc.
    /// </summary>
    /// <param name="context">Http context</param>
    /// <param name="overwrite">True (default) to overwrite existing entries if any</param>
    public static void AddSerenityAssetsImportMapEntries(this HttpContext context, bool overwrite = true)
    {
        context.AddImportMapEntry("jspdf", "~/Serenity.Assets/jspdf/jspdf-autotable.bundle.js", overwrite: overwrite);
        context.AddImportMapEntry("jspdf-autotable", "~/Serenity.Assets/jspdf/jspdf-autotable.bundle.js", overwrite: overwrite);
        context.AddImportMapEntry("@serenity-is/tiptap", "~/Serenity.Assets/tiptap/tiptap.bundle.js", overwrite: overwrite);
    }

    /// <summary>
    /// Renders an HTML import map script element based on the current view's import map configuration.
    /// </summary>
    /// <remarks>Use this method in a Razor view to emit an import map for JavaScript module loading. The
    /// import map is retrieved from the current HTTP context and serialized to JSON. If no import map is configured,
    /// the method returns an empty result.</remarks>
    /// <param name="html">The HTML helper instance used to access the current view context and import map data. Cannot be null.</param>
    /// <returns>An HtmlString containing a <script type="importmap" /> element with the serialized import map, or an empty
    /// HtmlString if no import map is available.</returns>
    public static HtmlString RenderImportMap(this IHtmlHelper html)
    {
        ArgumentNullException.ThrowIfNull(html);
        var contextItems = html.ViewContext?.HttpContext?.Items;
        if (contextItems == null ||
            contextItems[importMapKey] is not ImportMap importMap)
            return HtmlString.Empty;

        var json = JSON.StringifyIndented(new
        {
            imports = importMap.Imports,
            scopes = importMap.Scopes,
            integrity = importMap.Integrity
        });

        return new HtmlString($"<script type=\"importmap\" nonce=\"{html.CspNonce()}\">\n{json}\n</script>\n");
    }

    /// <summary>
    /// Gets the script URL for a local text script
    /// </summary>
    /// <param name="page">HTML helper</param>
    /// <param name="package">Package key</param>
    /// <param name="isPending">True to include pending texts</param>
    public static string GetLocalTextInclude(this IHtmlHelper page, string package, bool isPending = false)
    {
        string languageId = CultureInfo.CurrentUICulture.Name.TrimToNull() ?? "invariant";
        string scriptName = Web.LocalTextScript.GetScriptName(package, languageId, isPending);
        var scriptManager = page.ViewContext.HttpContext.RequestServices.GetRequiredService<IDynamicScriptManager>();
        scriptManager.IfNotRegistered(scriptName, () =>
        {
            var services = page.ViewContext.HttpContext.RequestServices;
            var registry = services.GetRequiredService<ILocalTextRegistry>();
            var packages = services.GetRequiredService<IOptions<LocalTextPackages>>();
            return new LocalTextScript(registry, package, packages.Value[package], languageId, isPending);
        });

        return scriptManager.GetScriptInclude(scriptName);
    }

    /// <summary>
    /// Gets a script element for including a local text script
    /// </summary>
    /// <param name="page">HTML helper</param>
    /// <param name="package">Package key</param>
    /// <param name="isPending">True to include pending texts</param>
    public static HtmlString LocalTextScript(this IHtmlHelper page, string package, bool isPending = false)
    {
        string languageId = CultureInfo.CurrentUICulture.Name.TrimToNull() ?? "invariant";
        string scriptName = Web.LocalTextScript.GetScriptName(package, languageId, isPending);
        var scriptManager = page.ViewContext.HttpContext.RequestServices.GetRequiredService<IDynamicScriptManager>();
        scriptManager.IfNotRegistered(scriptName, () =>
        {
            var services = page.ViewContext.HttpContext.RequestServices;
            var registry = services.GetRequiredService<ILocalTextRegistry>();
            var packages = services.GetRequiredService<IOptions<LocalTextPackages>>();
            return new LocalTextScript(registry, package, packages.Value[package], languageId, isPending);
        });

        return Script(page, "dynamic://" + scriptName);
    }
}