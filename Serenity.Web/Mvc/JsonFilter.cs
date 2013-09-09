using System;
using System.IO;
using System.Web.Mvc;
using Newtonsoft.Json;
using Serenity.Data;

namespace Serenity.Services
{
    public class JsonFilter : ActionFilterAttribute
    {
        public string Param { get; set; }

        public JsonFilter()
        {
            Param = "request";
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            string method = request.HttpMethod ?? "";

            Type prmType = null;
            if (!Param.IsEmptyOrNull())
            {
                foreach (var desc in filterContext.ActionDescriptor.GetParameters())
                    if (desc.ParameterName == Param)
                        prmType = desc.ParameterType;
            }

            if (prmType == null)
                throw new ArgumentOutOfRangeException("JsonFilter.Param");

            if (method.Equals("POST", StringComparison.InvariantCultureIgnoreCase) ||
                method.Equals("PUT", StringComparison.InvariantCultureIgnoreCase))
            {
                if ((request.ContentType ?? string.Empty)
                    .Contains("application/json"))
                {                   
                    if (request.ContentLength == 0 &&
                        !request.ServerVariables["HTTP_VIA"].IsTrimmedEmpty())
                        throw new InvalidDataException("Sunucuya gelen isteğin gövdesi boş! " +
                            "Sisteme bir vekil sunucu (proxy) üzerinden bağlandınız. Sorun bundan kaynaklanıyor olabilir. " +
                            "Lütfen sunucu adresini tarayıcınızın istisna listesine ekleyiniz.");

                    if (filterContext.HttpContext.Request.InputStream.CanSeek)
                        filterContext.HttpContext.Request.InputStream.Seek(0, SeekOrigin.Begin);

                    using (var sr = new StreamReader(filterContext.HttpContext.Request.InputStream,
                        filterContext.HttpContext.Request.ContentEncoding, true, 4096, true))
                    {
                        var js = JsonSerializer.Create(JsonSettings.Strict);
                        using (var jr = new JsonTextReader(sr))
                        {
                            var obj = js.Deserialize(jr, prmType);
                            filterContext.ActionParameters[Param] = obj;
                        }
                    }
                }
                else
                {
                    string req = request.Form[Param] ?? request.QueryString[Param];
                    if (req != null)
                    {
                        var obj = JsonConvert.DeserializeObject(req, prmType, JsonSettings.Strict);
                        filterContext.ActionParameters[Param] = obj;
                    }
                }
            }
            else
            {
                string req = request.QueryString[Param];
                if (req != null)
                {
                    var obj = JsonConvert.DeserializeObject(req, prmType, JsonSettings.Strict);
                    filterContext.ActionParameters[Param] = obj;
                }
            }
        }
    }
}