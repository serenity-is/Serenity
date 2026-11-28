using Microsoft.AspNetCore.Mvc.Abstractions;

namespace Serenity.Web;

public class RequiresElevationAttributeTests
{
    private class StubElevationHandler : IElevationHandler
    {
        public Action? OnValidate { get; set; }
        public int ValidateCalls { get; private set; }

        public void AppendElevationTokenToCookies()
        {
        }

        public void DeleteToken()
        {
        }

        public void ValidateElevationToken()
        {
            ValidateCalls++;
            OnValidate?.Invoke();
        }
    }

    private class PasswordUserDefinition : MockUserDefinition, IHasPassword
    {
        public bool HasPassword { get; set; }
    }

    private class StubExternalProviders(params string[] names) : Serenity.Extensions.IExternalElevationProviders
    {
        public IReadOnlyList<Serenity.Extensions.ExternalElevationProvider> GetProviders(ClaimsPrincipal user) =>
            [.. names.Select(x => new Serenity.Extensions.ExternalElevationProvider { Name = x, DisplayName = x })];
    }

    private static ResourceExecutingContext CreateContext(string method, string path,
        StubElevationHandler handler, IUserDefinition? user = null, bool loggedIn = true, Serenity.Extensions.IExternalElevationProviders? providers = null)
    {
        var httpContext = new DefaultHttpContext();
        if (loggedIn)
        {
            httpContext.User = new ClaimsPrincipal(new GenericIdentity("user", "Test"));
        }

        httpContext.Request.Method = method;
        httpContext.Request.Path = path;

        var services = new ServiceCollection();
        services.AddSingleton<IElevationHandler>(handler);
        services.AddSingleton<IUserRetrieveService>(new MockUserRetrieveService(user!));
        if (providers != null)
            services.AddSingleton(providers);
        httpContext.RequestServices = services.BuildServiceProvider();

        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        return new ResourceExecutingContext(actionContext, [], []);
    }

    [Fact]
    public void OnResourceExecuting_Passes_When_Token_Valid()
    {
        var handler = new StubElevationHandler();
        var context = CreateContext("POST", "/some/path", handler);

        new RequiresElevationAttribute().OnResourceExecuting(context);

        Assert.Null(context.Result);
        Assert.Equal(1, handler.ValidateCalls);
    }

    [Fact]
    public void OnResourceExecuting_Redirects_To_SetPassword_When_No_Password_And_Get()
    {
        var handler = new StubElevationHandler { OnValidate = () => throw new ValidationError("x") };
        var user = new PasswordUserDefinition { Id = "1", Username = "user", HasPassword = false };
        var context = CreateContext("GET", "/some/path", handler, user);

        new RequiresElevationAttribute().OnResourceExecuting(context);

        var redirect = Assert.IsType<LocalRedirectResult>(context.Result);
        Assert.Contains("/Account/SetPassword?reason=elevate", redirect.Url);
    }

    [Fact]
    public void OnResourceExecuting_Redirects_To_Elevate_When_No_Password_But_A_Provider_Can_Confirm()
    {
        var handler = new StubElevationHandler { OnValidate = () => throw new ValidationError("x") };
        var user = new PasswordUserDefinition { Id = "1", Username = "user", HasPassword = false };

        var context = CreateContext("GET", "/Account/ExternalLogins", handler, user, providers: new StubExternalProviders("Google"));
        new RequiresElevationAttribute().OnResourceExecuting(context);
        var redirect = Assert.IsType<LocalRedirectResult>(context.Result);
        Assert.Contains("/Account/Elevate?returnUrl=", redirect.Url);

        // none of the providers of the user can
        context = CreateContext("GET", "/Account/ExternalLogins", handler, user, providers: new StubExternalProviders());
        new RequiresElevationAttribute().OnResourceExecuting(context);
        Assert.Contains("/Account/SetPassword?reason=elevate", Assert.IsType<LocalRedirectResult>(context.Result).Url);
    }

    [Fact]
    public void OnResourceExecuting_Redirects_To_Elevate_When_Password_Exists_And_Get()
    {
        var handler = new StubElevationHandler { OnValidate = () => throw new ValidationError("x") };
        var user = new PasswordUserDefinition { Id = "1", Username = "user", HasPassword = true };
        var context = CreateContext("GET", "/some/path", handler, user);

        new RequiresElevationAttribute().OnResourceExecuting(context);

        var redirect = Assert.IsType<LocalRedirectResult>(context.Result);
        Assert.Contains("/Account/Elevate?returnUrl=%2Fsome%2Fpath", redirect.Url);
    }

    [Fact]
    public void OnResourceExecuting_Redirects_With_Root_When_Path_Empty()
    {
        var handler = new StubElevationHandler { OnValidate = () => throw new ValidationError("x") };
        var user = new PasswordUserDefinition { Id = "1", Username = "user", HasPassword = true };
        var context = CreateContext("GET", "", handler, user);

        new RequiresElevationAttribute().OnResourceExecuting(context);

        var redirect = Assert.IsType<LocalRedirectResult>(context.Result);
        Assert.Contains("returnUrl=%2F", redirect.Url);
    }

    [Fact]
    public void OnResourceExecuting_Redirects_To_Elevate_When_Not_LoggedIn_And_Get()
    {
        var handler = new StubElevationHandler { OnValidate = () => throw new ValidationError("x") };
        var context = CreateContext("GET", "/some/path", handler, loggedIn: false);

        new RequiresElevationAttribute().OnResourceExecuting(context);

        var redirect = Assert.IsType<LocalRedirectResult>(context.Result);
        Assert.Contains("/Account/Elevate?returnUrl=", redirect.Url);
    }

    [Fact]
    public void OnResourceExecuting_Rethrows_When_Not_Get()
    {
        var handler = new StubElevationHandler { OnValidate = () => throw new ValidationError("x") };
        var context = CreateContext("POST", "/some/path", handler);

        Assert.Throws<ValidationError>(() =>
            new RequiresElevationAttribute().OnResourceExecuting(context));
    }

    [Fact]
    public void OnResourceExecuted_Does_Nothing()
    {
        var httpContext = new DefaultHttpContext();
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        var context = new ResourceExecutedContext(actionContext, []);

        new RequiresElevationAttribute().OnResourceExecuted(context);
    }
}
