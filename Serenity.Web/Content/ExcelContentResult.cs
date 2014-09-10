using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Mvc;
using System;

namespace Serenity.Web
{
    public static class ExcelContentResult
    {
        public static FileContentResult Create(byte[] data)
        {
            return Create(data, null);
        }

        public static FileContentResult Create(byte[] data, string downloadName)
        {
            var result = new FileContentResult(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            result.FileDownloadName = downloadName ?? ("report" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx");
            return result;
        }
    }
}
