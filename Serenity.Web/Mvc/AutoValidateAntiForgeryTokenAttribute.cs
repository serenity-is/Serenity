#if ASPNETMVC
using System;
using System.Reflection;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Serenity.Services
{
    public class AutoValidateAntiforgeryTokenAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var method = context.RequestContext.HttpContext.Request.HttpMethod;
            if (string.Equals("GET", method, StringComparison.OrdinalIgnoreCase) ||
                string.Equals("HEAD", method, StringComparison.OrdinalIgnoreCase) ||
                string.Equals("TRACE", method, StringComparison.OrdinalIgnoreCase) ||
                string.Equals("OPTIONS", method, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (context.ActionDescriptor != null &&
                context.ActionDescriptor.IsDefined(typeof(IgnoreAntiforgeryTokenAttribute), false))
                return;

            if (context.Controller != null &&
                context.Controller.GetType().GetCustomAttribute<IgnoreAntiforgeryTokenAttribute>() != null)
                return;

            // no need to validate anti forgery token if there is no logged user
            if (!Authorization.IsLoggedIn)
                return;

            var cookieToken = context.RequestContext.HttpContext.Request.Cookies[AntiForgeryConfig.CookieName];

            // this is probably a non-browser client, e.g. some mobile device calling services
            if (cookieToken == null)
                return;

            var headerToken = context.RequestContext.HttpContext.Request.Headers["X-CSRF-TOKEN"];
            if (!string.IsNullOrEmpty(headerToken))
            {
                AntiForgery.Validate(cookieToken == null ? null : cookieToken.Value, headerToken);
                return;
            }

            AntiForgery.Validate();
        }
    }
}
#endif