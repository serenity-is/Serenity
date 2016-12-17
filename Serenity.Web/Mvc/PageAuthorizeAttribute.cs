using Serenity.Data;
using System;
using System.Linq;
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

        protected PageAuthorizeAttribute(Type sourceType, params Type[] attributeTypes)
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
                    "PageAuthorize attribute is created with source type of " +
                    sourceType.Name + ", but it has no " +
                    string.Join(" OR ", attributeTypes.Select(x => x.Name)) + " attribute(s)");
            }

            this.Permission = attr.Permission;
        }

        public PageAuthorizeAttribute(Type sourceType)
            : this(sourceType, typeof(ReadPermissionAttribute))
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
                filterContext.Result = new RedirectResult(FormsAuthentication.LoginUrl + 
                    "?returnUrl=" + Uri.EscapeDataString(HttpContext.Current.Request.Url.PathAndQuery) +
                    "&denied=1");

                return;
            }

            base.HandleUnauthorizedRequest(filterContext);
        }

        public string Permission { get; private set; }
    }
}