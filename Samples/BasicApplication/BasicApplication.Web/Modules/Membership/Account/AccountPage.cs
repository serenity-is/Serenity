
namespace BasicApplication.Membership.Pages
{
    using Serenity;
    using Serenity.Services;
    using System;
    using System.Web.Mvc;
    using System.Web.Security;

    [RoutePrefix("Account"), Route("{action=index}")]
    public class AccountController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            ViewBag.HideLeftNavigation = true;
            return View("~/Modules/Membership/Account/AccountLogin.cshtml");
        }

        [HttpPost, JsonFilter]
        public Result<ServiceResponse> Login(LoginRequest request)
        {
            return this.ExecuteMethod(() =>
            {
                request.CheckNotNull();

                if (request.Username == null)
                    throw new ArgumentNullException("username");

                var username = request.Username;
                if (Dependency.Resolve<IAuthenticationService>().Validate(ref username, request.Password))
                    return new ServiceResponse();

                throw new ValidationError("AuthenticationError", null, "Invalid username or password!");
            });
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