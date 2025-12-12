using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace Serenity.Web;

/// <summary>
/// Contains import map related extension methods for HTML helpers and HTTP contexts.
/// </summary>
public static partial class HtmlImportMapExtensions
{
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
}