using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net;

namespace Serenity.Web;

/// <summary>
/// Contains extensions related to LayoutCssFiles.
/// </summary>
public static class HtmlLayoutCssFilesExtensions
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

        context.RequestServices.GetService<ILayoutCssFileWatcher>()?.Watch(scriptName, files);

        var include = scriptManager.GetScriptInclude(scriptName, ".css");
        var cssUrl = VirtualPathUtility.ToAbsolute(context, "~/DynJS.axd/" + include);

        if (HtmlScriptExtensions.IsAlreadyIncluded(context.Items, cssUrl))
            return HtmlString.Empty;

        return new HtmlString(string.Format(CultureInfo.InvariantCulture,
            "    <link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\"/>\n",
            WebUtility.HtmlEncode(context.RequestServices.GetRequiredService<IContentHashCache>()
                .ResolveWithHash(context.Request.PathBase, cssUrl))));
    }

    private static string GetLayoutCssScriptName(IReadOnlyList<string> files)
    {
        byte[] hash;
        using (var md5 = System.Security.Cryptography.MD5.Create())
            hash = md5.ComputeHash(Encoding.UTF8.GetBytes(string.Join("\n", files)));

        return "LayoutCssFiles." + Convert.ToHexString(hash);
    }
}