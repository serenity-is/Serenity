using Serenity.Data;
using System;
using System.Web;
using System.Linq;
using System.Web.Mvc;

namespace Serenity.Services
{
    public class ServiceAuthorizeAttribute : AuthorizeAttribute
    {
        public ServiceAuthorizeAttribute()
        {
        }

        protected ServiceAuthorizeAttribute(Type sourceType, params Type[] attributeTypes)
        {
            if (sourceType == null)
                throw new ArgumentNullException("sourceType");

            if (attributeTypes.IsEmptyOrNull())
                throw new ArgumentNullException("attributeTypes");

            PermissionAttributeBase attr = null;
            foreach (var attributeType in attributeTypes)
            {
                var lst = sourceType.GetCustomAttributes(attributeType, true);
                if (lst.Length > 0)
                {
                    attr = lst[0] as PermissionAttributeBase;
                    if (attr == null)
                        throw new ArgumentOutOfRangeException(attributeType.Name + 
                            " is not a subclass of PermissionAttributeBase!");

                    break;
                }
            }

            if (attr == null)
            {
                throw new ArgumentOutOfRangeException("sourceType",
                    "ServiceAuthorize attribute is created with source type of " +
                    sourceType.Name + ", but it has no " +
                    string.Join(" OR ", attributeTypes.Select(x => x.Name)) + " attribute(s)");
            }

            this.Permission = attr.Permission;
        }

        public ServiceAuthorizeAttribute(Type sourceType)
            : this(sourceType, typeof(ReadPermissionAttribute))
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