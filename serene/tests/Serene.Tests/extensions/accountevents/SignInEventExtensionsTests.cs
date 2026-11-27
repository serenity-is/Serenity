using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;

namespace Serenity.Extensions;

public class SignInEventExtensionsTests
{
    private sealed class CapturePublisher : IAccountEventPublisher
    {
        public List<AccountEvent> Events { get; } = [];
        public void Publish(AccountEvent accountEvent, IUnitOfWork? unitOfWork = null) => Events.Add(accountEvent);
    }

    private sealed class Users(params IUserDefinition[] users) : IUserRetrieveService
    {
        public IUserDefinition? ById(string id) => users.FirstOrDefault(x => x.Id == id);
        public IUserDefinition? ByUsername(string username) => users.FirstOrDefault(x => x.Username == username);
    }

    private static (IServiceProvider, CapturePublisher) Services()
    {
        var publisher = new CapturePublisher();
        var services = new ServiceCollection();
        services.AddSingleton<IAccountEventPublisher>(publisher);
        services.AddSingleton<IUserRetrieveService>(new Users(new MockUserDefinition("5", "nancy")));
        return (services.BuildServiceProvider(), publisher);
    }

    [Fact]
    public void A_Sign_In_And_A_Failure_Of_A_Known_User_Are_Published_With_The_User()
    {
        var (services, publisher) = Services();
        services.PublishSignIn("nancy", SignInEventExtensions.PasswordMethod);
        services.PublishSignInFailed("nancy", SignInEventExtensions.PasswordMethod, PasswordValidationResult.Invalid);

        Assert.Collection(publisher.Events,
            e =>
            {
                Assert.Equal(AccountEventKind.SignedIn, e.Kind);
                Assert.Equal("5", e.UserId);
                Assert.Equal("Password", e.Method);
                Assert.Null(e.Reason);
            },
            e =>
            {
                Assert.Equal(AccountEventKind.SignInFailed, e.Kind);
                Assert.Equal("5", e.UserId);
                Assert.Equal(SignInFailureReasons.InvalidPassword, e.Reason);
            });
    }

    [Fact]
    public void An_Unknown_Username_Is_Not_Stored_As_It_May_Be_A_Password()
    {
        var (services, publisher) = Services();
        services.PublishSignInFailed("MySecret123!", SignInEventExtensions.PasswordMethod, PasswordValidationResult.Invalid);
        // only the failures are published without a user, an empty username not at all
        services.PublishSignIn("MySecret123!", SignInEventExtensions.PasswordMethod);
        services.Publish(AccountEventKind.Elevated, "MySecret123!", null, null);
        services.PublishSignInFailed("", SignInEventExtensions.PasswordMethod, PasswordValidationResult.EmptyUsername);

        var e = Assert.Single(publisher.Events);
        Assert.Null(e.UserId);
        Assert.Null(e.Username);
        Assert.Equal(SignInFailureReasons.UnknownUser, e.Reason);
        Assert.DoesNotContain(publisher.Events, x => x.Username == "MySecret123!" || x.OldValue != null || x.NewValue != null);
    }

    [Fact]
    public void A_Sign_Out_Is_Published_For_A_Signed_In_User_Only()
    {
        var (services, publisher) = Services();
        services.PublishSignOut(new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Name, "nancy")], "Password")));
        services.PublishSignOut(new ClaimsPrincipal(new ClaimsIdentity()));
        services.PublishSignOut(null);

        var e = Assert.Single(publisher.Events);
        Assert.Equal(AccountEventKind.SignedOut, e.Kind);
        Assert.Equal("Password", e.Method);
    }

    [Fact]
    public void Nothing_Happens_Without_A_Publisher()
    {
        new ServiceCollection().BuildServiceProvider().PublishSignIn("nancy", SignInEventExtensions.PasswordMethod);
    }

    [Fact]
    public void The_Reasons_Of_The_Password_Validation_Results()
    {
        Assert.Equal(SignInFailureReasons.InvalidPassword, SignInFailureReasons.From(PasswordValidationResult.Invalid));
        Assert.Equal(SignInFailureReasons.InactiveUser, SignInFailureReasons.From(PasswordValidationResult.InactiveUser));
        Assert.Equal(SignInFailureReasons.Throttled, SignInFailureReasons.From(PasswordValidationResult.Throttle));
        Assert.Equal(SignInFailureReasons.Other, SignInFailureReasons.From(PasswordValidationResult.DirectoryError));
    }
}
