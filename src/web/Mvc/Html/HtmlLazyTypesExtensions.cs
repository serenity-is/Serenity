using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace Serenity.Web;

/// <summary>
/// Contains extensions for the client side types that are loaded when they are first requested.
/// </summary>
public static class HtmlLazyTypesExtensions
{
    /// <summary>
    /// Renders the types declared with <see cref="LazyLoadTypesAttribute"/> as a JSON block that maps the
    /// content hashed module urls to their type keys, which the default <c>Config.lazyTypeLoader</c> reads when a type
    /// is not found in a type registry. Should be included in the layout head, like the <c>ScriptCulture</c> block.
    /// Returns an empty string if there are no such types.
    /// </summary>
    /// <param name="helper">The HTML helper.</param>
    /// <returns>The rendered script element.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="helper"/> is <c>null</c>.</exception>
    public static HtmlString LazyTypes(this IHtmlHelper helper)
    {
        ArgumentNullException.ThrowIfNull(helper);

        var context = helper.ViewContext.HttpContext;
        var modules = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        var attributes = context.RequestServices.GetService<ITypeSource>()?.GetAssemblyAttributes<LazyLoadTypesAttribute>();
        if (attributes != null)
        {
            var contentHashCache = context.RequestServices.GetRequiredService<IContentHashCache>();
            foreach (var attr in attributes)
            {
                if (attr.Keys.Length == 0)
                    continue;

                var url = contentHashCache.ResolveWithHash(context.Request.PathBase, attr.Path);
                if (!modules.TryGetValue(url, out var keys))
                    modules[url] = keys = [];
                keys.AddRange(attr.Keys);
            }
        }

        if (modules.Count == 0)
            return HtmlString.Empty;

        return new HtmlString($"<script type=\"application/json\" id=\"LazyTypes\">{JSON.Stringify(modules)}</script>\n");
    }
}
