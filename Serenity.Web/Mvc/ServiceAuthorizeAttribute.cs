using System;
using System.Web;
using System.Web.Mvc;

namespace Serenity.Services
{
    public class ServiceAuthorizeAttribute : AuthorizeAttribute
    {
        public ServiceAuthorizeAttribute()
        {
        }

        public ServiceAuthorizeAttribute(object permission)
            : this()
        {
            this.Permission = permission == null ? null : permission.ToString();
        }

        public ServiceAuthorizeAttribute(object module, object permission)
            : this(module.ToString() + ":" + permission)
        {
        }

        public ServiceAuthorizeAttribute(object module, object submodule, object permission)
            : this(module.ToString() + ":" + submodule + ":" + permission)
        {
        }

        public string Permission { get; private set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!base.AuthorizeCore(httpContext))
                return false;

            return Permission.IsEmptyOrNull() || Authorization.HasPermission(Permission);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (Authorization.IsLoggedIn)
            {
                filterContext.Result = new Result<ServiceResponse>(new ServiceResponse
                {
                    Error = new ServiceError
                    {
                        Code = "AccessDenied",
                        Message = LocalText.Get("Authorization.AccessDenied")
                    }
                });
            }
            else
            {
                filterContext.Result = new Result<ServiceResponse>(new ServiceResponse
                {
                    Error = new ServiceError
                    {
                        Code = "NotLoggedIn",
                        Message = LocalText.Get("Authorization.NotLoggedIn")
                    }
                });
            }

            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = 400;
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
        }
    }
}