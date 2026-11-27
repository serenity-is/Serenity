using System.Security.Claims;

namespace Serenity.Extensions;

public class PasswordMyAccountLinkProviderTests
{
    private sealed class PasswordUser(string username, bool hasPassword, bool changeRequired) : IUserDefinition, IHasPassword, IHasPasswordChangeRequired
    {
        public string Id => username;
        public string DisplayName => username;
        public string Username => username;
        public string? Email => null;
        public short IsActive => 1;
        public bool HasPassword => hasPassword;
        public bool PasswordChangeRequired => changeRequired;
    }

    private sealed class Users(params IUserDefinition[] users) : IUserRetrieveService
    {
        public IUserDefinition? ById(string id) => users.FirstOrDefault(x => x.Id == id);
        public IUserDefinition? ByUsername(string username) => users.FirstOrDefault(x => x.Username == username);
    }

    private static ClaimsPrincipal Principal(string username) =>
        new(new ClaimsIdentity([new Claim(ClaimTypes.Name, username)], "Password"));

    [Fact]
    public void Links_To_Change_The_Password_Or_Set_One_And_Tells_When_A_Change_Is_Required()
    {
        var provider = new PasswordMyAccountLinkProvider(new Users(new PasswordUser("nancy", true, false),
            new PasswordUser("google", false, false), new PasswordUser("andrew", true, true)), NullTextLocalizer.Instance);
        string? Text(LocalText text) => text.ToString(NullTextLocalizer.Instance);

        var nancy = Assert.Single(provider.GetLinks(Principal("nancy")));
        Assert.Equal("~/Account/ChangePassword", nancy.Url);
        Assert.Equal(Text(MyAccountTexts.ChangePasswordDescription), nancy.Description);
        Assert.Equal(MyAccountLinkState.None, nancy.State);

        Assert.Equal(Text(MyAccountTexts.SetPasswordDescription), Assert.Single(provider.GetLinks(Principal("google"))).Description);

        var andrew = Assert.Single(provider.GetLinks(Principal("andrew")));
        Assert.Equal(MyAccountLinkState.Warning, andrew.State);
        Assert.Equal(Text(MyAccountTexts.ChangePasswordRequired), andrew.StateText);
    }
}
