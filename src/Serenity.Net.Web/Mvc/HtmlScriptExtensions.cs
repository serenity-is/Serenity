using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net;

namespace Serenity.Web;

/// <summary>
/// Contains Serenity related helper methods that can be used in Razor CSHTML files
/// </summary>
public static class HtmlScriptExtensions
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
        if (helper == null)
            throw new ArgumentNullException(nameof(helper));

        if (cssUrl == null)
            throw new ArgumentNullException(nameof(cssUrl));

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
    /// Renders individual link elements for all CSS files in a bundle if bundling is disabled,
    /// and renders a single link element containing the bundle URL if it is enabled.
    /// </summary>
    /// <param name="helper">HTML helper</param>
    /// <param name="bundleKey">Bundle key</param>
    /// <exception cref="ArgumentNullException">Helper or bundleKey is null</exception>
    public static HtmlString StyleBundle(this IHtmlHelper helper, string bundleKey)
    {
        if (helper == null)
            throw new ArgumentNullException(nameof(helper));

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
        if (helper == null)
            throw new ArgumentNullException(nameof(helper));

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
        if (helper == null)
            throw new ArgumentNullException(nameof(helper));

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
        if (helper == null)
            throw new ArgumentNullException(nameof(helper));

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
    static readonly Regex EndingWithVersionRegex = new(@"\?v=[0-9a-zA-Z_-]*$", RegexOptions.Compiled);

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