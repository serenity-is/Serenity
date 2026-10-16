using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net;

namespace Serenity.Web;

/// <summary>
/// Contains extensions for including layout CSS and JS files declared with
/// <see cref="LayoutCssFileAttribute"/> / <see cref="LayoutScriptFileAttribute"/>.
/// </summary>
public static class HtmlLayoutFilesExtensions
{
    /// <summary>
    /// Renders stylesheet links for all the CSS files declared with
    /// <see cref="LayoutCssFileAttribute"/> and resolved by <see cref="ILayoutCssFileProvider"/>,
    /// so that dynamically loaded libraries can contribute layout styles without a page
    /// specific entry point. When CSS bundling is enabled (<see cref="CssBundlingOptions.Enabled"/>),
    /// the files are concatenated into one dynamic CSS bundle served from <c>~/DynJS.axd/</c>;
    /// otherwise a link is rendered for each file. Returns an empty string if there are no
    /// files, or if the result was already included in this context.
    /// </summary>
    /// <param name="helper">The HTML helper.</param>
    /// <param name="filter">An optional predicate applied to each file url (e.g.
    /// <c>~/path/file.css</c>). Return <c>false</c> to exclude a file.</param>
    /// <returns>The rendered stylesheet link elements.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="helper"/> is <c>null</c>.</exception>
    public static HtmlString LayoutCssFiles(this IHtmlHelper helper,
        Func<string, bool>? filter = null)
    {
        ArgumentNullException.ThrowIfNull(helper);

        var context = helper.ViewContext.HttpContext;
        var files = context.RequestServices.GetService<ILayoutCssFileProvider>()?.GetFiles();
        if (files == null || files.Count == 0)
            return HtmlString.Empty;

        if (filter != null)
        {
            files = [.. files.Where(filter)];
            if (files.Count == 0)
                return HtmlString.Empty;
        }

        // when bundling is disabled (e.g. development), render one link per file; each
        // link is content hashed, so file changes are picked up without any watcher
        if (context.RequestServices.GetService<IOptions<CssBundlingOptions>>()?.Value.Enabled != true)
        {
            var links = new StringBuilder();
            foreach (var file in files)
                links.Append(helper.Stylesheet(file).Value);

            return new HtmlString(links.ToString());
        }

        var scriptManager = context.RequestServices.GetRequiredService<IDynamicScriptManager>();
        var scriptName = GetLayoutCssScriptName(files);
        scriptManager.IfNotRegistered(scriptName, () =>
            context.RequestServices.GetRequiredService<ICssBundleBuilder>()
                .BuildBundle("../", files));

        context.RequestServices.GetService<IWebRootFileWatcher>()?.RegisterCallback(
            scriptName, files, () => scriptManager.Changed(scriptName));

        var include = scriptManager.GetScriptInclude(scriptName, ".css");
        var cssUrl = VirtualPathUtility.ToAbsolute(context, "~/DynJS.axd/" + include);

        if (HtmlScriptExtensions.IsAlreadyIncluded(context.Items, cssUrl))
            return HtmlString.Empty;

        return new HtmlString(string.Format(CultureInfo.InvariantCulture,
            "    <link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\"/>\n",
            WebUtility.HtmlEncode(context.RequestServices.GetRequiredService<IContentHashCache>()
                .ResolveWithHash(context.Request.PathBase, cssUrl))));
    }

    /// <summary>
    /// Renders script tags for all the JS files declared with
    /// <see cref="LayoutScriptFileAttribute"/> and resolved by <see cref="ILayoutScriptFileProvider"/>,
    /// so that dynamically loaded libraries can contribute scripts without a page
    /// specific entry point. When script bundling is enabled (<see cref="ScriptBundlingOptions.Enabled"/>),
    /// the files are concatenated into one dynamic script bundle served from <c>~/DynJS.axd/</c>;
    /// otherwise a tag is rendered for each file. Returns an empty string if there are no
    /// files, or if the result was already included in this context.
    /// </summary>
    /// <param name="helper">The HTML helper.</param>
    /// <param name="filter">An optional predicate applied to each file url (e.g.
    /// <c>~/path/file.js</c>). Return <c>false</c> to exclude a file.</param>
    /// <returns>The rendered script elements.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="helper"/> is <c>null</c>.</exception>
    public static HtmlString LayoutScriptFiles(this IHtmlHelper helper,
        Func<string, bool>? filter = null)
    {
        ArgumentNullException.ThrowIfNull(helper);

        var context = helper.ViewContext.HttpContext;
        var files = context.RequestServices.GetService<ILayoutScriptFileProvider>()?.GetFiles();
        if (files == null || files.Count == 0)
            return HtmlString.Empty;

        if (filter != null)
        {
            files = [.. files.Where(filter)];
            if (files.Count == 0)
                return HtmlString.Empty;
        }

        // when bundling is disabled (e.g. development), render one tag per file; each
        // tag is content hashed, so file changes are picked up without any watcher
        if (context.RequestServices.GetService<IOptions<ScriptBundlingOptions>>()?.Value.Enabled != true)
        {
            var tags = new StringBuilder();
            foreach (var file in files)
                tags.Append(helper.Script(file).Value);

            return new HtmlString(tags.ToString());
        }

        var scriptManager = context.RequestServices.GetRequiredService<IDynamicScriptManager>();
        var scriptName = GetLayoutScriptScriptName(files);
        scriptManager.IfNotRegistered(scriptName, () =>
            context.RequestServices.GetRequiredService<IScriptBundleBuilder>()
                .BuildBundle(files));

        context.RequestServices.GetService<IWebRootFileWatcher>()?.RegisterCallback(
            scriptName, files, () => scriptManager.Changed(scriptName));

        var include = scriptManager.GetScriptInclude(scriptName);
        var scriptUrl = VirtualPathUtility.ToAbsolute(context, "~/DynJS.axd/" + include);

        if (HtmlScriptExtensions.IsAlreadyIncluded(context.Items, scriptUrl))
            return HtmlString.Empty;

        return new HtmlString(string.Format(CultureInfo.InvariantCulture,
            "    <script src=\"{0}\" type=\"text/javascript\"></script>\n",
            WebUtility.HtmlEncode(context.RequestServices.GetRequiredService<IContentHashCache>()
                .ResolveWithHash(context.Request.PathBase, scriptUrl))));
    }

    private static string GetLayoutCssScriptName(IReadOnlyList<string> files)
    {
        byte[] hash;
        using (var md5 = System.Security.Cryptography.MD5.Create())
            hash = md5.ComputeHash(Encoding.UTF8.GetBytes(string.Join("\n", files)));

        return "LayoutCssFiles." + Convert.ToHexString(hash);
    }

    private static string GetLayoutScriptScriptName(IReadOnlyList<string> files)
    {
        byte[] hash;
        using (var md5 = System.Security.Cryptography.MD5.Create())
            hash = md5.ComputeHash(Encoding.UTF8.GetBytes(string.Join("\n", files)));

        return "LayoutScriptFiles." + Convert.ToHexString(hash);
    }
}
