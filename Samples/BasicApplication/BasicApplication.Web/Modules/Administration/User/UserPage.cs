
[assembly:Serenity.Navigation.NavigationLink(91000, "Administration/User Management", url: "~/Administration/User", permission: "Administration", icon: "icon-users")]

namespace BasicApplication.Administration.Pages
{
    using Serenity;
    using System.Web.Mvc;

    [RoutePrefix("Administration/User"), Route("{action=index}")]
    public class UserController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            WebSecurityHelper.EnsurePermission("Administration");
            return View("~/Modules/Administration/User/UserIndex.cshtml");
        }
    }
}