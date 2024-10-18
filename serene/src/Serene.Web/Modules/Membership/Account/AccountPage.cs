using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Serene.Membership.Pages;

[Route("Account/[action]")]
public partial class AccountPage : Controller
{
    protected ITwoLevelCache Cache { get; }
    protected ITextLocalizer Localizer { get; }
    public AccountPage(ITwoLevelCache cache, ITextLocalizer localizer)
    {
        Localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        Cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

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
        [FromServices] IUserRetrieveService userRetriever,
        [FromServices] IUserClaimCreator userClaimCreator)
    {

        return this.ExecuteMethod(() =>
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrEmpty(request.Username))
                throw new ArgumentNullException(nameof(request.Username));

            if (passwordValidator is null)
                throw new ArgumentNullException(nameof(passwordValidator));

            if (userRetriever is null)
                throw new ArgumentNullException(nameof(userRetriever));

            if (userClaimCreator is null)
                throw new ArgumentNullException(nameof(userClaimCreator));

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
                throw new ValidationError("InactivatedAccount", Texts.Validation.AuthenticationError.ToString(Localizer));
            }

            throw new ValidationError("AuthenticationError", Texts.Validation.AuthenticationError.ToString(Localizer));
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