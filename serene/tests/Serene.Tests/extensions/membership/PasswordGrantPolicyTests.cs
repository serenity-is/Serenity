using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using Serenity.Extensions.DependencyInjection;

namespace Serenity.Extensions;

public class PasswordGrantPolicyTests
{
    private sealed class DenySignIn(string username, string message) : IPasswordSignInPolicy
    {
        public string? GetDenial(IUserDefinition user) => user.Username == username ? message : null;
    }

    private sealed class DenyGrant(string username, string message) : IPasswordGrantPolicy
    {
        public string? GetDenial(IUserDefinition user) => user.Username == username ? message : null;
    }

    private sealed class Users(params IUserDefinition[] users) : IUserRetrieveService
    {
        public IUserDefinition? ById(string id) => users.FirstOrDefault(x => x.Id == id);
        public IUserDefinition? ByUsername(string username) => users.FirstOrDefault(x => x.Username == username);
    }

    private sealed class ChangeUser(string id, string username, bool changeRequired) : IUserDefinition, IHasPasswordChangeRequired
    {
        public string Id => id;
        public string DisplayName => username;
        public string Username => username;
        public string? Email => null;
        public short IsActive => 1;
        public bool PasswordChangeRequired => changeRequired;
    }

    private sealed class ClaimCreator : IUserClaimCreator
    {
        public ClaimsPrincipal CreatePrincipal(string username, string authType) =>
            new(new ClaimsIdentity([new Claim(ClaimTypes.Name, username)], authType));
    }

    private static readonly Users TwoUsers = new(new ChangeUser("5", "nancy", changeRequired: true), new ChangeUser("6", "andrew", false));

    [Fact]
    public void The_Sign_In_Policies_Refuse_A_Token_First_Then_The_Grant_Policies()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IUserRetrieveService>(TwoUsers);
        services.AddSingleton<IPasswordSignInPolicy>(new DenySignIn("nancy", "Use Google"));
        services.AddSingleton<IPasswordGrantPolicy>(new DenyGrant("nancy", "has 2FA"));
        services.AddSingleton<IPasswordGrantPolicy>(new DenyGrant("andrew", "has 2FA too"));
        var provider = services.BuildServiceProvider();

        Assert.Equal("Use Google", provider.GetPasswordGrantDenial("nancy"));
        Assert.Equal("has 2FA too", provider.GetPasswordGrantDenial("andrew"));
        Assert.Null(provider.GetPasswordGrantDenial("nobody"));
        // the sign-in page doesn't ask the grant policies, it can ask for the code
        Assert.Null(provider.GetPasswordSignInDenial("andrew"));
    }

    [Fact]
    public void A_User_Who_Must_Change_The_Password_Gets_No_Token_With_It()
    {
        var policy = new AccountSetupPasswordGrantPolicy([new PasswordChangeRequirement(TwoUsers)], new ClaimCreator(),
            NullTextLocalizer.Instance);
        Assert.Equal(AccountSetupTexts.PasswordChangeRequired.ToString(NullTextLocalizer.Instance), policy.GetDenial(TwoUsers.ById("5")!));
        Assert.Null(policy.GetDenial(TwoUsers.ById("6")!));
    }

    [Fact]
    public void Account_Setup_Registers_The_Grant_Policy()
    {
        var services = new ServiceCollection();
        services.AddAccountSetup();
        services.AddAccountSetup();
        Assert.Single(services, x => x.ServiceType == typeof(IPasswordGrantPolicy) &&
            x.ImplementationType == typeof(AccountSetupPasswordGrantPolicy));
    }
}
