using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Serenity.Extensions;

public class AccountSessionTests
{
    private sealed class MemoryStore : IAccountSessionStore
    {
        public Dictionary<string, AccountSession> Sessions { get; } = [];
        public int Touches { get; private set; }

        public void Create(AccountSession session) => Sessions[session.SessionId] = session;
        public AccountSession? Get(string sessionId) => Sessions.GetValueOrDefault(sessionId);

        public void Touch(string sessionId, DateTime lastSeen, string? ipAddress)
        {
            Touches++;
            Sessions[sessionId].LastSeenDate = lastSeen;
        }

        public bool End(string sessionId, AccountSessionEndReason reason, DateTime endedDate)
        {
            if (Get(sessionId) is not { EndedDate: null } session)
                return false;
            session.EndedDate = endedDate;
            session.EndReason = reason;
            return true;
        }

        public int EndAll(string userId, AccountSessionEndReason reason, DateTime endedDate, string? exceptSessionId = null) =>
            Sessions.Values.Where(x => x.UserId == userId && x.SessionId != exceptSessionId).ToArray()
                .Count(x => End(x.SessionId, reason, endedDate));

        public List<AccountSession> ListActive(string userId, DateTime seenSince) =>
            [.. Sessions.Values.Where(x => x.UserId == userId && x.EndedDate == null && x.LastSeenDate >= seenSince)];
    }

    private sealed class TestOptionsMonitor<T>(T value) : IOptionsMonitor<T>
    {
        public T CurrentValue => value;
        public T Get(string? name) => value;
        public IDisposable? OnChange(Action<T, string?> listener) => null;
    }

    private sealed class TestTime : TimeProvider
    {
        public DateTimeOffset Now { get; set; } = new(2026, 11, 28, 10, 0, 0, TimeSpan.Zero);
        public override DateTimeOffset GetUtcNow() => Now;
    }

    private sealed class User(string id, string username, short isActive = 1) : IUserDefinition
    {
        public string Id => id;
        public string DisplayName => username;
        public string Username => username;
        public string? Email => null;
        public short IsActive { get; set; } = isActive;
    }

    private sealed class Users(params User[] users) : IUserRetrieveService
    {
        public IUserDefinition? ById(string id) => users.FirstOrDefault(x => x.Id == id);
        public IUserDefinition? ByUsername(string username) => users.FirstOrDefault(x => x.Username == username);
    }

    private sealed class Revoker : IAccountTokenRevoker
    {
        public List<string> Revoked { get; } = [];
        public void RevokeAll(string userId) => Revoked.Add(userId);
    }

    private sealed class Publisher : IAccountEventPublisher
    {
        public List<AccountEvent> Events { get; } = [];
        public void Publish(AccountEvent accountEvent, IUnitOfWork? unitOfWork = null) => Events.Add(accountEvent);
    }

    private sealed class SignOuts : IAuthenticationService
    {
        public int Count { get; private set; }
        public Task<AuthenticateResult> AuthenticateAsync(HttpContext context, string? scheme) => Task.FromResult(AuthenticateResult.NoResult());
        public Task ChallengeAsync(HttpContext context, string? scheme, AuthenticationProperties? properties) => Task.CompletedTask;
        public Task ForbidAsync(HttpContext context, string? scheme, AuthenticationProperties? properties) => Task.CompletedTask;
        public Task SignInAsync(HttpContext context, string? scheme, ClaimsPrincipal principal, AuthenticationProperties? properties) => Task.CompletedTask;

        public Task SignOutAsync(HttpContext context, string? scheme, AuthenticationProperties? properties)
        {
            Count++;
            return Task.CompletedTask;
        }
    }

    private sealed class Setup
    {
        public MemoryStore Store { get; } = new();
        public TestTime Time { get; } = new();
        public User Nancy { get; } = new("5", "nancy");
        public Revoker Revoker { get; } = new();
        public Publisher Publisher { get; } = new();
        public SignOuts SignOuts { get; } = new();
        public AccountSessionOptions Options { get; } = new();
        public AccountSessionManager Manager { get; }
        public AccountSessionCookieValidator Validator { get; }

        public Setup()
        {
            var options = new TestOptionsMonitor<AccountSessionOptions>(Options);
            var cookie = new TestOptionsMonitor<CookieAuthenticationOptions>(new CookieAuthenticationOptions { ExpireTimeSpan = TimeSpan.FromMinutes(30) });
            Manager = new AccountSessionManager(options, cookie, [Revoker], Store, Publisher, Time);
            Validator = new AccountSessionCookieValidator(options, new Users(Nancy), Manager, Store, timeProvider: Time);
        }

        public HttpContext Http(ClaimsPrincipal? user = null)
        {
            var services = new ServiceCollection();
            services.AddSingleton<IAuthenticationService>(SignOuts);
            return new DefaultHttpContext { RequestServices = services.BuildServiceProvider(), User = user ?? new ClaimsPrincipal() };
        }

        public static ClaimsPrincipal Principal(string username, string id, string? sessionId = null)
        {
            var identity = new ClaimsIdentity([new Claim(ClaimTypes.Name, username), new Claim(ClaimTypes.NameIdentifier, id)], "Password");
            if (sessionId != null)
                identity.AddClaim(new Claim(AccountSessionClaimTypes.SessionId, sessionId));
            return new ClaimsPrincipal(identity);
        }

        private static AuthenticationScheme Scheme => new(CookieAuthenticationDefaults.AuthenticationScheme, null, typeof(CookieAuthenticationHandler));

        public ClaimsPrincipal SignIn(ClaimsPrincipal? current = null)
        {
            var principal = Principal("nancy", "5");
            Validator.SigningIn(new CookieSigningInContext(Http(current), Scheme, new CookieAuthenticationOptions(), principal,
                new AuthenticationProperties(), new CookieOptions()));
            return principal;
        }

        public CookieValidatePrincipalContext Validate(ClaimsPrincipal principal, DateTime? validated)
        {
            var properties = new AuthenticationProperties();
            if (validated != null)
                properties.Items[AccountSessionCookieValidator.ValidatedItem] = validated.Value.Ticks.ToString(System.Globalization.CultureInfo.InvariantCulture);
            var context = new CookieValidatePrincipalContext(Http(principal), Scheme, new CookieAuthenticationOptions(),
                new AuthenticationTicket(principal, properties, Scheme.Name));
            Validator.ValidatePrincipalAsync(context).GetAwaiter().GetResult();
            return context;
        }
    }

    [Fact]
    public void A_Sign_In_Starts_A_Session_Whose_Id_The_Cookie_Has()
    {
        var setup = new Setup();
        var principal = setup.SignIn();

        var session = Assert.Single(setup.Store.Sessions.Values);
        Assert.Equal(session.SessionId, principal.GetSessionId());
        Assert.Equal(32, session.SessionId.Length);
        Assert.Equal("5", session.UserId);
        Assert.Equal("Password", session.Method);

        // another sign-in in the same browser ends the session of its cookie
        setup.SignIn(principal);
        Assert.Equal(AccountSessionEndReason.SignedOut, setup.Store.Sessions[session.SessionId].EndReason);
        Assert.Equal(1, setup.Store.Sessions.Values.Count(x => x.EndedDate == null));
    }

    [Fact]
    public void A_Session_Is_Checked_Only_After_The_Interval_And_Then_Renewed()
    {
        var setup = new Setup();
        var principal = setup.SignIn();
        var now = setup.Time.Now.UtcDateTime;

        var context = setup.Validate(principal, now.AddSeconds(-30));
        Assert.Equal(0, setup.Store.Touches);
        Assert.False(context.ShouldRenew);

        setup.Time.Now = setup.Time.Now.AddMinutes(2);
        context = setup.Validate(principal, now);
        Assert.Equal(1, setup.Store.Touches);
        Assert.True(context.ShouldRenew);
        Assert.NotNull(context.Principal);
        Assert.Equal(0, setup.SignOuts.Count);
    }

    [Fact]
    public void A_Cookie_Of_A_Session_That_Ended_Or_Of_A_Deactivated_User_Is_Signed_Out()
    {
        var setup = new Setup();
        var principal = setup.SignIn();
        var sessionId = principal.GetSessionId()!;

        setup.Manager.EndAll("5", AccountSessionEndReason.RevokedByAdmin);
        var context = setup.Validate(principal, validated: null);
        Assert.Null(context.Principal);
        Assert.Equal(1, setup.SignOuts.Count);

        var other = setup.SignIn();
        setup.Nancy.IsActive = 0;
        context = setup.Validate(other, validated: null);
        Assert.Null(context.Principal);
        Assert.Equal(AccountSessionEndReason.Deactivated, setup.Store.Sessions[other.GetSessionId()!].EndReason);
        Assert.Equal(AccountSessionEndReason.RevokedByAdmin, setup.Store.Sessions[sessionId].EndReason);
    }

    [Fact]
    public void A_Cookie_Of_Before_The_Sessions_Gets_One()
    {
        var setup = new Setup();
        var context = setup.Validate(Setup.Principal("nancy", "5"), validated: null);

        var session = Assert.Single(setup.Store.Sessions.Values);
        Assert.Equal(session.SessionId, context.Principal.GetSessionId());
        Assert.True(context.ShouldRenew);
        Assert.Equal(0, setup.SignOuts.Count);
    }

    [Fact]
    public void Only_One_Session_Ends_The_Others_Without_Revoking_The_Tokens()
    {
        var setup = new Setup();
        setup.Options.Concurrent = AccountSessionConcurrency.SignOutOthers;
        var first = setup.SignIn();
        var second = setup.SignIn();

        Assert.Equal(AccountSessionEndReason.Replaced, setup.Store.Sessions[first.GetSessionId()!].EndReason);
        Assert.Null(setup.Store.Sessions[second.GetSessionId()!].EndedDate);
        Assert.Empty(setup.Revoker.Revoked);
    }

    [Fact]
    public void Ending_The_Sessions_Revokes_The_Tokens_And_Tells_The_Activity()
    {
        var setup = new Setup();
        var first = setup.SignIn();
        setup.SignIn();

        // a session of another user is not ended
        Assert.False(setup.Manager.End("6", first.GetSessionId()!, AccountSessionEndReason.RevokedByUser));
        Assert.Equal(2, setup.Manager.ListActive("5").Count);

        Assert.Equal(2, setup.Manager.EndAll("5", AccountSessionEndReason.PasswordChanged));
        Assert.Equal(["5"], setup.Revoker.Revoked);
        var e = Assert.Single(setup.Publisher.Events);
        Assert.Equal(AccountEventKind.SessionsRevoked, e.Kind);
        Assert.Equal("PasswordChanged", e.Reason);
        Assert.Equal("2", e.NewValue);
        Assert.Empty(setup.Manager.ListActive("5"));
    }

    [Fact]
    public void A_Password_Change_Ends_The_Other_Sessions_Of_The_User_And_All_For_An_Administrator()
    {
        var setup = new Setup();
        var current = setup.SignIn();
        var other = setup.SignIn(null);
        var accessor = new HttpContextAccessor { HttpContext = setup.Http(current) };
        var services = new ServiceCollection().AddSingleton<IAccountSessionManager>(setup.Manager).BuildServiceProvider();
        var handler = new AccountSessionEventHandler(services, accessor);

        handler.HandleAsync(new AccountEvent { Kind = AccountEventKind.PasswordChanged, UserId = "5" }, TestContext.Current.CancellationToken);
        Assert.Null(setup.Store.Sessions[current.GetSessionId()!].EndedDate);
        Assert.Equal(AccountSessionEndReason.PasswordChanged, setup.Store.Sessions[other.GetSessionId()!].EndReason);

        // an administrator, or a reset link, ends all of them
        accessor.HttpContext = setup.Http(Setup.Principal("admin", "1"));
        handler.HandleAsync(new AccountEvent { Kind = AccountEventKind.TwoFactorReset, UserId = "5" }, TestContext.Current.CancellationToken);
        Assert.Equal(AccountSessionEndReason.TwoFactorReset, setup.Store.Sessions[current.GetSessionId()!].EndReason);

        // the other events end nothing
        var last = setup.SignIn(null);
        handler.HandleAsync(new AccountEvent { Kind = AccountEventKind.EmailChanged, UserId = "5" }, TestContext.Current.CancellationToken);
        Assert.Null(setup.Store.Sessions[last.GetSessionId()!].EndedDate);
    }

    [Fact]
    public void The_Registrations_Resolve_Without_A_Cycle()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IUserRetrieveService>(new Users());
        services.AddSingleton<IUserAccessor>(new MockUserAccessor(() => (ClaimsPrincipal?)null));
        services.AddSingleton<ITextLocalizer>(NullTextLocalizer.Instance);
        services.AddSingleton<IAccountSessionStore>(new MemoryStore());
        Serenity.Extensions.DependencyInjection.AccountSessionServiceCollectionExtensions.AddAccountSessions(services);

        using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true });
        Assert.NotNull(provider.GetRequiredService<IAccountEventPublisher>());
        Assert.True(provider.GetRequiredService<IAccountSessionManager>().IsEnabled);
        Assert.NotNull(provider.GetRequiredService<AccountSessionCookieValidator>());
        Assert.Contains(provider.GetServices<IAccountEventHandler>(), x => x is AccountSessionEventHandler);
    }

    [Fact]
    public void Without_A_Store_Nothing_Changes()
    {
        var options = new TestOptionsMonitor<AccountSessionOptions>(new AccountSessionOptions());
        var manager = new AccountSessionManager(options, new TestOptionsMonitor<CookieAuthenticationOptions>(new()), []);
        Assert.False(manager.IsEnabled);
        Assert.Empty(manager.ListActive("5"));
        Assert.Equal(0, manager.EndAll("5", AccountSessionEndReason.RevokedByAdmin));
    }
}
