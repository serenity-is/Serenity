using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Serenity.Demo.Northwind;

/// <summary>
/// HTML helpers for the order pages, shared with pages in other projects that show an extended order grid.
/// </summary>
public static class OrderHtmlExtensions
{
    /// <summary>
    /// Renders the script includes of the lookups used by the order grid and dialog.
    /// </summary>
    /// <param name="html">The HTML helper</param>
    public static IHtmlContent OrderLookupScripts(this IHtmlHelper html)
    {
        ArgumentNullException.ThrowIfNull(html);

        var builder = new HtmlContentBuilder();
        foreach (var lookupKey in new[] {
            "Northwind.Customer",
            "Northwind.CustomerCountry",
            "Northwind.CustomerCity",
            "Northwind.Employee",
            "Northwind.OrderShipCountry",
            "Northwind.OrderShipCity",
            "Northwind.Product",
            "Northwind.Shipper"
        })
        {
            builder.AppendHtml(html.Script("dynamic://Lookup." + lookupKey));
            builder.AppendHtmlLine("");
        }

        return builder;
    }
}
