using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Serenity.Web
{
    public class PageAuthorizeAttribute : AuthorizeAttribute
    {
        public PageAuthorizeAttribute()
            : base()
        {
        }

        public PageAuthorizeAttribute(object permission)
        {
            this.Permission = permission == null ? null : permission.ToString();
        }

        public PageAuthorizeAttribute(object module, object permission)
            : this(module.ToString() + ":" + permission)
        {
        }

        public PageAuthorizeAttribute(object module, object submodule, object permission)
            : this(module.ToString() + ":" + submodule + ":" + permission)
        {
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!base.AuthorizeCore(httpContext))
                return false;

            return Permission.IsEmptyOrNull() || Authorization.HasPermission(Permission);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!string.IsNullOrEmpty(Permission) &&
                Authorization.IsLoggedIn &&
                FormsAuthentication.IsEnabled)
            {
                var loginUrl = FormsAuthentication.LoginUrl;
                if (loginUrl.IndexOf('?') >= 0)
                    loginUrl += "&";
                else
                    loginUrl += "?";

                filterContext.Result = new RedirectResult(loginUrl + 
                    "returnUrl=" + Uri.EscapeDataString(HttpContext.Current.Request.Url.PathAndQuery) +
                    "&denied=1");

                return;
            }

            base.HandleUnauthorizedRequest(filterContext);
        }

        public string Permission { get; private set; }
    }
}