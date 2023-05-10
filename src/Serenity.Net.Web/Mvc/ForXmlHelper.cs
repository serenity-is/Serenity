using System.Xml.Linq;

namespace Serenity.Data;

/// <summary>
/// Helper to generate T-SQL FOR XML RAW statements
/// </summary>
public static class ForXmlHelper
{
    /// <summary>
    /// FOR XML RAW Statement
    /// </summary>
    public const string Statement = " FOR XML RAW";

    /// <summary>
    /// Enumerates data returned from a FOR XML statement
    /// </summary>
    /// <param name="forXml">String returned from FOR XML statement</param>
    /// <returns></returns>
    public static IEnumerable<XElement> Enumerate(string forXml)
    {
        forXml = forXml.TrimToNull();
        if (forXml == null)
            return new List<XElement>();

        var node = XElement.Parse("<root>" + forXml + "</root>");
        return node.Descendants("row");
    }

    /// <summary>
    /// Converts data returned from FOR XML statement to a row type
    /// </summary>
    /// <typeparam name="TRow">Target row type</typeparam>
    /// <param name="forXml">Returned FOR XML data</param>
    /// <param name="readRow">Action to read a row</param>
    /// <returns></returns>
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
    /// Gets attr value from an XElement
    /// </summary>
    /// <param name="e">XElement</param>
    /// <param name="attr">Attr name</param>
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