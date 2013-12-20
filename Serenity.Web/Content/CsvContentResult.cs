using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Mvc;
using System;

namespace Serenity.Web
{
    public static class CsvContentResult
    {
        public static FileContentResult Create(ICollection<string> captions, ICollection<string[]> table)
        {
            return Create(captions, table, null);
        }

        public static FileContentResult Create(ICollection<string> captions, ICollection<string[]> table, string downloadName)
        {
            byte[] contents;
            using (var sw = new StringWriter())
                using (var csvWriter = new CsvWriter(sw))
                {
                    csvWriter.ExportTabularized(captions, table);
                    sw.Flush();
                    var encoding = Encoding.GetEncoding("iso-8859-9"); // utf-8 excel 2003 de karakterleri bozuyor, 2007 de sorun yok, şimdilik 8859-9
                    var actual = encoding.GetBytes(sw.GetStringBuilder().ToString());
                    contents = actual;
                }

            var result = new FileContentResult(contents, "text/csv; encoding=iso-8859-9");
            result.FileDownloadName = downloadName ?? ("data" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv");

            return result;
        }
    }
}
