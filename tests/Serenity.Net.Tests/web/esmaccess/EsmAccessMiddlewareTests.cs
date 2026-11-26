using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Security.Principal;
using Serenity.Web.Middleware;

namespace Serenity.Web;

public class EsmAccessMiddlewareTests
{
    private const string Login = "Modules/Membership/Account/Login/LoginPage.js";
    private const string User = "Modules/Administration/User/UserPage.js";

    private static EsmAccessPolicy CreatePolicy()
    {
        var env = new MockHostEnvironment();
        env.AddWebFile("esm/" + EsmManifest.FileName, JSON.Stringify(new
        {
            version = 1,
            entries = new[] { Login, User },
            files = new Dictionary<string, int[]> { [Login] = [0], [User] = [1] }
        }));
        return new EsmAccessPolicy(env.WebRootFileProvider, new EsmAccessControlOptions { Enabled = true });
    }

    private class TestAuthenticationService(ClaimsPrincipal? principal) : IAuthenticationService
    {
        public int Calls { get; private set; }

        public Task<AuthenticateResult> AuthenticateAsync(HttpContext context, string? scheme)
        {
            Calls++;
            return Task.FromResult(principal is null ? AuthenticateResult.NoResult() :
                AuthenticateResult.Success(new AuthenticationTicket(principal, "Test")));
        }

        public Task ChallengeAsync(HttpContext context, string? scheme, AuthenticationProperties? properties) => Task.CompletedTask;
        public Task ForbidAsync(HttpContext context, string? scheme, AuthenticationProperties? properties) => Task.CompletedTask;
        public Task SignInAsync(HttpContext context, string? scheme, ClaimsPrincipal principal, AuthenticationProperties? properties) => Task.CompletedTask;
        public Task SignOutAsync(HttpContext context, string? scheme, AuthenticationProperties? properties) => Task.CompletedTask;
    }

    private static async Task<(DefaultHttpContext context, bool nextCalled, TestAuthenticationService auth)> Invoke(
        string path, ClaimsPrincipal? authenticated = null, string method = "GET", string? cacheControl = null)
    {
        var auth = new TestAuthenticationService(authenticated);
        var services = new ServiceCollection();
        services.AddSingleton<IAuthenticationService>(auth);
        var contextHolder = new DefaultHttpContext();
        // logged-in users have "?", like BasePermissionService
        services.AddSingleton<IPermissionService>(new MockPermissions(p =>
            p == "?" && contextHolder.User.Identity?.IsAuthenticated == true));
        contextHolder.RequestServices = services.BuildServiceProvider();
        contextHolder.Request.Method = method;
        contextHolder.Request.Path = path;
        contextHolder.Response.Body = new MemoryStream();

        var nextCalled = false;
        var middleware = new EsmAccessMiddleware(ctx =>
        {
            nextCalled = true;
            if (cacheControl != null)
                ctx.Response.Headers.CacheControl = cacheControl;
            return Task.CompletedTask;
        }, CreatePolicy());

        await middleware.InvokeAsync(contextHolder);
        return (contextHolder, nextCalled, auth);
    }

    private static ClaimsPrincipal LoggedIn() => new(new GenericIdentity("user", "Test"));

    [Fact]
    public async Task Public_File_Is_Served_Without_Authenticating()
    {
        var (context, nextCalled, auth) = await Invoke("/esm/" + Login);
        Assert.True(nextCalled);
        Assert.Equal(200, context.Response.StatusCode);
        Assert.Equal(0, auth.Calls);
    }

    [Fact]
    public async Task Restricted_File_Is_404_For_Anonymous()
    {
        var (context, nextCalled, auth) = await Invoke("/esm/" + User);
        Assert.False(nextCalled);
        Assert.Equal(404, context.Response.StatusCode);
        Assert.Equal("private, no-store", context.Response.Headers.CacheControl.ToString());
        Assert.Equal(1, auth.Calls);
    }

    [Fact]
    public async Task Restricted_File_Is_Served_To_Authenticated_User()
    {
        var (context, nextCalled, _) = await Invoke("/esm/" + User, LoggedIn());
        Assert.True(nextCalled);
        Assert.Equal(200, context.Response.StatusCode);
    }

    [Fact]
    public async Task Manifest_Is_404_Even_For_Authenticated_User()
    {
        var (context, nextCalled, _) = await Invoke("/esm/" + EsmManifest.FileName, LoggedIn());
        Assert.False(nextCalled);
        Assert.Equal(404, context.Response.StatusCode);
    }

    [Fact]
    public async Task Non_Get_Requests_Are_Passed_Through()
    {
        var (_, nextCalled, auth) = await Invoke("/esm/" + User, method: "POST");
        Assert.True(nextCalled);
        Assert.Equal(0, auth.Calls);
    }

    [Theory]
    [InlineData(null, "private")]
    [InlineData("public, max-age=31536000", "private, max-age=31536000")]
    [InlineData("max-age=60", "private, max-age=60")]
    [InlineData("private, max-age=60", "private, max-age=60")]
    public void MakePrivate_Replaces_Public(string? cacheControl, string expected)
    {
        IHeaderDictionary headers = new HeaderDictionary();
        if (cacheControl != null)
            headers.CacheControl = cacheControl;
        EsmAccessMiddleware.MakePrivate(headers);
        Assert.Equal(expected, headers.CacheControl.ToString());
    }
}
