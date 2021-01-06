using System;
using System.Globalization;
#if !ASPNETMVC
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

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
            var result = new FileContentResult(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = downloadName ?? ("report" +
                DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture) + ".xlsx")
            };
            return result;
        }
    }
}
