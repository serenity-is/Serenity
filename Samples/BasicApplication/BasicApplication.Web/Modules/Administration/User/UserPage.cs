

[assembly:Serenity.Navigation.NavigationLink(int.MaxValue, "Administration/User", url: "~/Administration/User", permission: "Administration", icon: null)]

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