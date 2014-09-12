
namespace BasicApplication.Membership.Pages
{
    using System.Web.Mvc;
    using System.Web.Security;

    [RoutePrefix("Account"), Route("{action=index}")]
    public class AccountController : Controller
    {
        public ActionResult Login()
        {
            ViewBag.HideLeftNavigation = true;
            return View("~/Modules/Membership/Account/AccountLogin.cshtml");
        }

        public ActionResult Signup()
        {
            return View("~/Modules/Membership/Account/AccountSignup.cshtml");
        }

        public ActionResult Signout()
        {
            Session.Abandon();
            FormsAuthentication.SignOut();
            return new RedirectResult("~/");
        }
    }
}