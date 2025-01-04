using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Serene.Membership.Pages;

[Route("Account/[action]")]
public partial class AccountPage(ITwoLevelCache cache, ITextLocalizer localizer) : Controller
{
    protected ITwoLevelCache Cache { get; } = cache ?? throw new ArgumentNullException(nameof(cache));
    protected ITextLocalizer Localizer { get; } = localizer ?? throw new ArgumentNullException(nameof(localizer));

    [HttpGet]
    public ActionResult Login(int? denied, string activated, string returnUrl)
    {
        if (denied == 1)
            return View(MVC.Views.Errors.AccessDenied,
                ("~/Account/Login?returnUrl=" + Uri.EscapeDataString(returnUrl)));

        ViewData["Activated"] = activated;
        ViewData["HideLeftNavigation"] = true;
        return View(MVC.Views.Membership.Account.Login.LoginPage);
    }

    [HttpGet]
    public ActionResult AccessDenied(string returnURL)
    {
        ViewData["HideLeftNavigation"] = !User.IsLoggedIn();

        return View(MVC.Views.Errors.AccessDenied, (object)returnURL);
    }

    [HttpPost, JsonRequest]
    public Result<ServiceResponse> Login(LoginRequest request,
        [FromServices] IUserPasswordValidator passwordValidator,
        [FromServices] IUserClaimCreator userClaimCreator)
    {

        return this.ExecuteMethod(() =>
        {
            ArgumentNullException.ThrowIfNull(request);

            if (string.IsNullOrEmpty(request.Username))
                throw new ArgumentNullException(nameof(request.Username));

            ArgumentNullException.ThrowIfNull(passwordValidator);

            ArgumentNullException.ThrowIfNull(userClaimCreator);

            var username = request.Username;
            var result = passwordValidator.Validate(ref username, request.Password);
            if (result == PasswordValidationResult.Valid)
            {
                var principal = userClaimCreator.CreatePrincipal(username, authType: "Password");
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal).GetAwaiter().GetResult();
                return new ServiceResponse();
            }

            if (result == PasswordValidationResult.InactiveUser)
            {
                throw new ValidationError("InactivatedAccount", MembershipValidationTexts.AuthenticationError.ToString(Localizer));
            }

            throw new ValidationError("AuthenticationError", MembershipValidationTexts.AuthenticationError.ToString(Localizer));
        });
    }

    private ActionResult Error(string message)
    {
        return View(MVC.Views.Errors.ValidationError, new ValidationError(message));
    }

    public string KeepAlive()
    {
        return "OK";
    }

    public ActionResult Signout()
    {
        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        HttpContext.RequestServices.GetService<IElevationHandler>()?.DeleteToken();
        return new RedirectResult("~/");
    }
}