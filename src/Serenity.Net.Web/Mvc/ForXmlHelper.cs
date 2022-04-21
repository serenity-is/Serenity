using System.Xml.Linq;

namespace Serenity.Data
{
    public static class ForXmlHelper
    {
        public const string Statement = " FOR XML RAW";

        public static IEnumerable<XElement> Enumerate(string forXml)
        {
            forXml = forXml.TrimToNull();
            if (forXml == null)
                return new List<XElement>();

            var node = XElement.Parse("<root>" + forXml + "</root>");
            return node.Descendants("row");
        }

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
}