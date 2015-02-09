using System;
using System.Web;
using System.Web.Mvc;

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

        public string Permission { get; private set; }
    }
}