using Microsoft.Extensions.DependencyInjection;

namespace Serenity.Extensions;

public class PasswordSignInPolicyTests
{
    private sealed class DenyUser(string username, string message) : IPasswordSignInPolicy
    {
        public string? GetDenial(IUserDefinition user) => user.Username == username ? message : null;
    }

    private sealed class Users(params IUserDefinition[] users) : IUserRetrieveService
    {
        public IUserDefinition? ById(string id) => users.FirstOrDefault(x => x.Id == id);
        public IUserDefinition? ByUsername(string username) => users.FirstOrDefault(x => x.Username == username);
    }

    private static IServiceProvider Services(params IPasswordSignInPolicy[] policies)
    {
        var services = new ServiceCollection();
        services.AddSingleton<IUserRetrieveService>(new Users(new MockUserDefinition("5", "nancy"), new MockUserDefinition("6", "andrew")));
        foreach (var policy in policies)
            services.AddSingleton(policy);
        return services.BuildServiceProvider();
    }

    [Fact]
    public void The_First_Policy_That_Refuses_A_User_Tells_Why()
    {
        var services = Services(new DenyUser("andrew", "first"), new DenyUser("nancy", "Use Google"), new DenyUser("nancy", "second"));
        Assert.Equal("Use Google", services.GetPasswordSignInDenial("nancy"));
        Assert.Null(services.GetPasswordSignInDenial("nobody"));

        var error = Assert.Throws<ValidationError>(() => services.ValidatePasswordSignIn("nancy"));
        Assert.Equal(PasswordSignInPolicyExtensions.PasswordSignInDeniedCode, error.ErrorCode);
        Assert.Equal("Use Google", error.Message);
    }

    [Fact]
    public void Without_Policies_Anyone_Can_Sign_In_With_The_Password()
    {
        var services = Services();
        Assert.Null(services.GetPasswordSignInDenial("nancy"));
        services.ValidatePasswordSignIn("nancy");
    }
}
