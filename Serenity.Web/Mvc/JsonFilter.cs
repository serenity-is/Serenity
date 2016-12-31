using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
#if ASPNETCORE
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
#else
using System.Web.Mvc;
#endif

namespace Serenity.Services
{
    public class JsonFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
#if ASPNETCORE
            string method = request.Method ?? "";
#else
            string method = request.HttpMethod ?? "";
#endif
            var prms = filterContext.ActionDescriptor
#if ASPNETCORE
                .Parameters
#else
                .GetParameters()
#endif
                .Where(x => !x.ParameterType.GetIsInterface());

            if (!prms.Any())
                return;

            if (prms.Count() != 1)
            {
#if ASPNETCORE
                prms = prms.Where(x => x.Name == "request");
#else
                prms = prms.Where(x => x.ParameterName == "request");
#endif

                if (prms.Count() != 1)
                    throw new ArgumentOutOfRangeException(String.Format(
                        "Method {0} has {1} parameters. JsonFilter requires an action method with only one parameter," + 
                        "or a parameter with name 'request'!",
#if ASPNETCORE
                            ((ControllerActionDescriptor)filterContext.ActionDescriptor).ActionName, filterContext.ActionDescriptor.Parameters.Count));
#else
                            filterContext.ActionDescriptor.ActionName, filterContext.ActionDescriptor.GetParameters().Length));
#endif
            }

            var prm = prms.Single();

#if ASPNETCORE
            if (method.Equals("POST", StringComparison.OrdinalIgnoreCase) ||
                method.Equals("PUT", StringComparison.OrdinalIgnoreCase))
#else
            if (method.Equals("POST", StringComparison.InvariantCultureIgnoreCase) ||
                method.Equals("PUT", StringComparison.InvariantCultureIgnoreCase))
#endif
            {
                if ((request.ContentType ?? string.Empty)
                    .Contains("application/json"))
                {                   
                    if (request.ContentLength == 0 &&
#if ASPNETCORE
                        !((string)request.Headers["HTTP_VIA"]).IsTrimmedEmpty())
#else
                        !request.Headers["HTTP_VIA"].IsTrimmedEmpty())
#endif
                        throw new InvalidDataException("Sunucuya gelen isteğin gövdesi boş! " +
                            "Sisteme bir vekil sunucu (proxy) üzerinden bağlandınız. Sorun bundan kaynaklanıyor olabilir. " +
                            "Lütfen sunucu adresini tarayıcınızın istisna listesine ekleyiniz.");

#if ASPNETCORE
                    if (filterContext.HttpContext.Request.Body.CanSeek)
                        filterContext.HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);

                    using (var sr = new StreamReader(filterContext.HttpContext.Request.Body,
                        System.Text.Encoding.GetEncoding((string)filterContext.HttpContext.Request.Headers["Content-Encoding"] ?? "utf-8"), true, 4096, true))
#else
                    if (filterContext.HttpContext.Request.InputStream.CanSeek)
                        filterContext.HttpContext.Request.InputStream.Seek(0, SeekOrigin.Begin);

                    using (var sr = new StreamReader(filterContext.HttpContext.Request.InputStream,
                        filterContext.HttpContext.Request.ContentEncoding, true, 4096, true))
#endif

                    {
                        var js = JsonSerializer.Create(JsonSettings.Strict);
                        using (var jr = new JsonTextReader(sr))
                        {
                            var obj = js.Deserialize(jr, prm.ParameterType);
#if ASPNETCORE
                            filterContext.ActionArguments[prm.Name] = obj;
#else
                            filterContext.ActionParameters[prm.ParameterName] = obj;
#endif
                        }
                    }
                }
                else
                {
#if ASPNETCORE
                    string req = (string)request.Form[prm.Name] ?? (string)request.Query[prm.Name] ??
                            (string)request.Form["request"] ?? request.Query["request"];
#else
                    string req = request.Form[prm.ParameterName] ?? request.QueryString[prm.ParameterName] ??
                            request.Form["request"] ?? request.QueryString["request"];
#endif
                    if (req != null)
                    {
                        var obj = JsonConvert.DeserializeObject(req, prm.ParameterType, JsonSettings.Strict);
#if ASPNETCORE
                        filterContext.ActionArguments[prm.Name] = obj;
#else
                        filterContext.ActionParameters[prm.ParameterName] = obj;
#endif
                    }
                }
            }
            else
            {
#if ASPNETCORE
                string req = (string)request.Form[prm.Name] ?? (string)request.Query[prm.Name] ??
                        (string)request.Form["request"] ?? request.Query["request"];
#else
                string req = request.Form[prm.ParameterName] ?? request.QueryString[prm.ParameterName] ??
                        request.Form["request"] ?? request.QueryString["request"];
#endif

                if (req != null)
                {
                    var obj = JsonConvert.DeserializeObject(req, prm.ParameterType, JsonSettings.Strict);
#if ASPNETCORE
                    filterContext.ActionArguments[prm.Name] = obj;
#else
                    filterContext.ActionParameters[prm.ParameterName] = obj;
#endif
                }
            }
        }
    }
}