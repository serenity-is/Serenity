using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported(ObeysTypeSystem = true)]
    public class FileDownloadFormatter : ISlickFormatter, IInitializeColumn
    {
        public string Format(SlickFormatterContext ctx)
        {
            var dbFile = ctx.Value as string;

            if (string.IsNullOrEmpty(dbFile))
                return "";

            var downloadUrl = DbFileUrl(dbFile);
            var originalName = !string.IsNullOrEmpty(OriginalNameProperty) ?
                ctx.Item[OriginalNameProperty] as string : null;
            originalName = originalName ?? "";

            var text = string.Format(DisplayFormat ?? "{0}",
                originalName,
                dbFile,
                downloadUrl);

            return "<a class='file-download-link' target='_blank' " + 
                "href='" + Q.AttrEncode(downloadUrl) + "'>" +
                Q.HtmlEncode(text) +
                "</a>";
        }

        public static string DbFileUrl(string filename)
        {
            filename = (filename ?? "").Replace("\\", "/");
            return Q.ResolveUrl("~/upload/") + filename;
        }

        [Option, IntrinsicProperty]
        public string DisplayFormat { get; set; }

        [Option, IntrinsicProperty]
        public string OriginalNameProperty { get; set; }

        public void InitializeColumn(SlickColumn column)
        {
            column.ReferencedFields = column.ReferencedFields ?? new List<string>();

            if (!string.IsNullOrEmpty(OriginalNameProperty))
            {
                column.ReferencedFields.Add(OriginalNameProperty);
                return;
            }
        }
    }
}