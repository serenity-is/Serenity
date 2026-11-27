using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace Serenity.Extensions;

public class AccountSetupFilterTests
{
    private sealed class TestRequirement(int order, string url, Func<ClaimsPrincipal, bool> isRequired) : IAccountSetupRequirement
    {
        public int Order => order;
        public string SetupUrl => url;
        public bool IsRequired(ClaimsPrincipal user) => isRequired(user);
        public string GetMessage(ITextLocalizer localizer) => "do it: " + url;
    }

    private sealed class ChangeUser(bool hasPassword, bool changeRequired) : IUserDefinition, IHasPassword, IHasPasswordChangeRequired
    {
        public string Id => "5";
        public string Username => "nancy";
        public string DisplayName => "Nancy";
        public string? Email => null;
        public short IsActive => 1;
        public bool HasPassword => hasPassword;
        public bool PasswordChangeRequired => changeRequired;
    }

    private sealed class Users(IUserDefinition user) : IUserRetrieveService
    {
        public IUserDefinition? ById(string id) => user;
        public IUserDefinition? ByUsername(string username) => user;
    }

    private static ClaimsPrincipal Principal(string authType = "Password") =>
        new(new ClaimsIdentity([new Claim(ClaimTypes.Name, "nancy"), new Claim(ClaimTypes.NameIdentifier, "5")], authType));

    private static AuthorizationFilterContext Context(ClaimsPrincipal? user, string method = "GET", string path = "/Northwind/Customer",
        bool allowed = false)
    {
        var http = new DefaultHttpContext();
        http.Request.Method = method;
        http.Request.Path = path;
        http.Request.PathBase = "/app";
        if (user != null)
            http.User = user;
        var action = new ActionDescriptor { EndpointMetadata = allowed ? [new AllowDuringAccountSetupAttribute()] : [] };
        return new AuthorizationFilterContext(new ActionContext(http, new RouteData(), action), []);
    }

    [Fact]
    public void Sends_A_User_To_The_First_Requirement_In_Order()
    {
        var filter = new AccountSetupFilter([
            new TestRequirement(20, "~/2FA/UserSettings", _ => true),
            new TestRequirement(10, "~/Account/ChangePassword", _ => true)
        ], NullTextLocalizer.Instance);

        var page = Context(Principal());
        filter.OnAuthorization(page);
        Assert.Equal("~/Account/ChangePassword", Assert.IsType<LocalRedirectResult>(page.Result).Url);

        var service = Context(Principal(), "POST", "/Services/Northwind/Customer/List");
        filter.OnAuthorization(service);
        var json = Assert.IsType<JsonResult>(service.Result);
        Assert.Equal(400, json.StatusCode);
        var error = Assert.IsType<ServiceResponse>(json.Value).Error!;
        Assert.Equal(AccountSetupFilter.SetupRequiredCode, error.Code);
        // the path of the page, with the path base, which a page goes to
        Assert.Equal("/app/Account/ChangePassword", error.Arguments);
        Assert.Equal("do it: ~/Account/ChangePassword", error.Message);
    }

    [Fact]
    public void Lets_The_Allowed_Actions_The_Users_Without_A_Requirement_And_The_Anonymous_Ones_Go()
    {
        var filter = new AccountSetupFilter([new TestRequirement(10, "~/Account/ChangePassword", u => u.Identity?.Name == "nancy")],
            NullTextLocalizer.Instance);
        var other = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Name, "andrew")], "Password"));

        foreach (var context in new[] { Context(Principal(), allowed: true), Context(other), Context(null) })
        {
            filter.OnAuthorization(context);
            Assert.Null(context.Result);
        }
    }

    [Fact]
    public void A_Password_Change_Is_Required_For_The_Password_Sign_Ins_Of_The_Users_Whose_Definitions_Say_So()
    {
        Assert.True(new PasswordChangeRequirement(new Users(new ChangeUser(true, true))).IsRequired(Principal()));
        Assert.False(new PasswordChangeRequirement(new Users(new ChangeUser(true, false))).IsRequired(Principal()));
        Assert.False(new PasswordChangeRequirement(new Users(new ChangeUser(true, true))).IsRequired(Principal("Google")));
    }
}
