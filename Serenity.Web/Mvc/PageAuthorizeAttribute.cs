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
            this.Permission = permission.ToString();
        }

        public PageAuthorizeAttribute(object applicationId, object permission)
            : this(applicationId.ToString() + ":" + permission.ToString())
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