#if !ASPNETMVC
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Serenity.Services
{
    public class AntiforgeryCookieResultFilterAttribute : ResultFilterAttribute
    {
        private readonly IAntiforgery antiforgery;

        public AntiforgeryCookieResultFilterAttribute(IAntiforgery antiforgery)
        {
            this.antiforgery = antiforgery;
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ViewResult)
            {
                var tokens = antiforgery.GetAndStoreTokens(context.HttpContext);
                context.HttpContext.Response.Cookies.Append("CSRF-TOKEN", tokens.RequestToken, 
                    new CookieOptions() { HttpOnly = false });
            }
        }
    }
}
#else
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Serenity.Services
{
    public class AntiforgeryCookieResultFilter : IResultFilter
    {
        public AntiforgeryCookieResultFilter()
        {
        }

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ViewResult)
            {
                var html = AntiForgery.GetHtml().ToString();
                var start = html.IndexOf("value=\"");
                var end = html.IndexOf('"', start + 7);
                var token = html.Substring(start + 7, end - start - 7).Trim();
                context.HttpContext.Response.Cookies.Add(new HttpCookie("CSRF-TOKEN", token));
            }
        }
    }
}
#endif