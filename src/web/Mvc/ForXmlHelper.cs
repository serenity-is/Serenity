using System.Xml.Linq;

namespace Serenity.Data;

/// <summary>
/// Helper to generate T-SQL <c>FOR XML RAW</c> statements.
/// </summary>
public static class ForXmlHelper
{
    /// <summary>
    /// The <c>FOR XML RAW</c> statement.
    /// </summary>
    public const string Statement = " FOR XML RAW";

    /// <summary>
    /// Enumerates data returned from a <c>FOR XML</c> statement.
    /// </summary>
    /// <param name="forXml">The string returned from the <c>FOR XML</c> statement.</param>
    /// <returns>The enumerated row elements.</returns>
    public static IEnumerable<XElement> Enumerate(string forXml)
    {
        forXml = forXml.TrimToNull();
        if (forXml == null)
            return [];

        var node = XElement.Parse("<root>" + forXml + "</root>");
        return node.Descendants("row");
    }

    /// <summary>
    /// Converts data returned from a <c>FOR XML</c> statement to a row type.
    /// </summary>
    /// <typeparam name="TRow">The target row type.</typeparam>
    /// <param name="forXml">The returned <c>FOR XML</c> data.</param>
    /// <param name="readRow">The action to read a row.</param>
    /// <returns>The list of rows.</returns>
    public static List<TRow> ToRows<TRow>(string forXml,
        Action<XElement, TRow> readRow) where TRow : class, IRow, new()
    {
        var result = new List<TRow>();
        forXml = forXml.TrimToNull();

        if (forXml == null)
            return result;

        var node = XElement.Parse("<root>" + forXml + "</root>");
        foreach (var r in node.Descendants("row"))
        {
            var row = new TRow();
            readRow(r, row);
            result.Add(row);
        }

        return result;
    }

    /// <summary>
    /// Gets the attribute value from an <see cref="XElement"/>.
    /// </summary>
    /// <param name="e">The <see cref="XElement"/>.</param>
    /// <param name="attr">The attribute name.</param>
    /// <returns>The attribute value, or <c>null</c> if not found.</returns>
    public static string Attr(this XElement e, string attr)
    {
        if (e == null)
            return null;

        var a = e.Attribute(attr);
        if (a == null)
            return null;

        return a.Value;
    }
}