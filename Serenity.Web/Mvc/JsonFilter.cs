using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace Serenity.Services
{
    public class JsonFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            string method = request.HttpMethod ?? "";

            var prms = filterContext.ActionDescriptor
                .GetParameters()
                .Where(x => !x.ParameterType.IsInterface);

            if (!prms.Any())
                return;

            if (prms.Count() != 1)
            {
                prms = prms.Where(x => x.ParameterName == "request");

                if (prms.Count() != 1)
                    throw new ArgumentOutOfRangeException(String.Format(
                        "Method {0} has {1} parameters. JsonFilter requires an action method with only one parameter," + 
                        "or a parameter with name 'request'!",
                            filterContext.ActionDescriptor.ActionName, filterContext.ActionDescriptor.GetParameters().Length));
            }

            var prm = prms.Single();

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
                            var obj = js.Deserialize(jr, prm.ParameterType);
                            filterContext.ActionParameters[prm.ParameterName] = obj;
                        }
                    }
                }
                else
                {
                    string req = request.Form[prm.ParameterName] ?? request.QueryString[prm.ParameterName] ??
                            request.Form["request"] ?? request.QueryString["request"];
                    if (req != null)
                    {
                        var obj = JsonConvert.DeserializeObject(req, prm.ParameterType, JsonSettings.Strict);
                        filterContext.ActionParameters[prm.ParameterName] = obj;
                    }
                }
            }
            else
            {
                string req = request.Form[prm.ParameterName] ?? request.QueryString[prm.ParameterName] ??
                        request.Form["request"] ?? request.QueryString["request"];

                if (req != null)
                {
                    var obj = JsonConvert.DeserializeObject(req, prm.ParameterType, JsonSettings.Strict);
                    filterContext.ActionParameters[prm.ParameterName] = obj;
                }
            }
        }
    }
}