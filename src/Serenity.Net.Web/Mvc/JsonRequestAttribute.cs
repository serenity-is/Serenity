using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Serenity.Services
{
    public class JsonRequestAttribute : ActionFilterAttribute
    {
        public JsonRequestAttribute()
        {
            ParamName = "request";
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException(nameof(filterContext));

            var request = filterContext.HttpContext.Request;
            string method = request.Method ?? "";
            var prms = filterContext.ActionDescriptor
                .Parameters
                .Where(x => !x.ParameterType.IsInterface);

            if (!prms.Any() || string.IsNullOrEmpty(ParamName))
                return;

            if (prms.Count() != 1)
            {
                prms = prms.Where(x => x.Name == ParamName);

                if (prms.Count() != 1)
                    throw new ArgumentOutOfRangeException(string.Format(CultureInfo.CurrentCulture,
                        "Method {0} has {1} parameters. [JsonRequest] requires an action method with only one parameter," + 
                        "or a parameter with name '{2}'!",
                            ((ControllerActionDescriptor)filterContext.ActionDescriptor).ActionName, 
                            filterContext.ActionDescriptor.Parameters.Count,
                            ParamName));
            }

            var prm = prms.Single();

            if (method.Equals("POST", StringComparison.OrdinalIgnoreCase) ||
                method.Equals("PUT", StringComparison.OrdinalIgnoreCase))
            {
                if ((request.ContentType ?? string.Empty)
                    .Contains("application/json", StringComparison.OrdinalIgnoreCase))
                {                   
                    if (filterContext.HttpContext.Request.Body.CanSeek)
                        filterContext.HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);

                    using var sr = new StreamReader(filterContext.HttpContext.Request.Body,
                        System.Text.Encoding.GetEncoding((string)filterContext.HttpContext
                            .Request.Headers["Content-Encoding"] ?? "utf-8"), true, 4096, true);
                    var js = JsonSerializer.Create(JsonSettings.Strict);
                    using var jr = new JsonTextReader(sr);
                    var obj = js.Deserialize(jr, prm.ParameterType);
                    filterContext.ActionArguments[prm.Name] = obj;
                }
                else 
                {
                    var req = FromFormOrQuery(request, prm.Name);
                    if (req != null)
                    {
                        var obj = JsonConvert.DeserializeObject(req, prm.ParameterType, JsonSettings.Strict);
                        filterContext.ActionArguments[prm.Name] = obj;
                    }
                }
            }
            else if (AllowGet && 
                method.Equals("GET", StringComparison.OrdinalIgnoreCase))
            {
                var req = FromFormOrQuery(request, prm.Name);
                if (req != null)
                {
                    var obj = JsonConvert.DeserializeObject(req, prm.ParameterType, JsonSettings.Strict);
                    filterContext.ActionArguments[prm.Name] = obj;
                }
            }
        }

        private string FromFormOrQuery(HttpRequest request, string name)
        {
            var allowForm = AllowForm && request.HasFormContentType;
            var allowQuery = AllowQuery;
            if (!allowForm && !allowQuery)
                return null;

            string value;
            if (allowForm)
            {
                value = request.Form[name];
                if (value != null)
                    return value;
            }

            if (allowQuery)
            {
                value = request.Query[name];
                if (value != null)
                    return value;
            }

            if (name != ParamName && !string.IsNullOrEmpty(ParamName))
                return FromFormOrQuery(request, ParamName);

            if (name != "request")
                return FromFormOrQuery(request, "request");

            return null;
        }

        public string ParamName { get; set; }

        private bool? allowGet;

        public bool AllowGet
        {
            get => allowGet ?? DefaultAllowGet;
            set => allowGet = value;
        }

        private bool? allowQuery;

        public bool AllowQuery
        {
            get => allowQuery ?? DefaultAllowQuery;
            set => allowQuery = value;
        }

        private bool? allowForm;

        public bool AllowForm
        {
            get => allowForm ?? DefaultAllowForm;
            set => allowForm = value;
        }

        public static bool DefaultAllowGet { get; set; } = true;
        public static bool DefaultAllowQuery { get; set; } = true;
        public static bool DefaultAllowForm { get; set; } = true;
    }

    [Obsolete("Prefer [JsonRequest] attribute")]
    public class JsonFilter : JsonRequestAttribute
    {
    }
}