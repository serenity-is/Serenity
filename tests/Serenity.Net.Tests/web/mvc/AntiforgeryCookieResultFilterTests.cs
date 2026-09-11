namespace Serenity.Services;

public class AntiforgeryCookieResultFilterTests
{
    private static ResultExecutingContext CreateContext(IActionResult result, DefaultHttpContext httpContext)
    {
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        return new ResultExecutingContext(actionContext, [], result, new object());
    }

    [Fact]
    public void OnResultExecuting_Appends_Csrf_Cookie_For_ViewResult()
    {
        var antiforgery = new MockAntiforgery();
        var filter = new AntiforgeryCookieResultFilterAttribute(antiforgery);
        var httpContext = new DefaultHttpContext();
        var context = CreateContext(new ViewResult(), httpContext);

        filter.OnResultExecuting(context);

        Assert.Equal(1, antiforgery.GetTokensCalls);
        Assert.Contains("CSRF-TOKEN", httpContext.Response.Headers.SetCookie.ToString());
    }

    [Fact]
    public void OnResultExecuting_Does_Nothing_For_Non_ViewResult()
    {
        var antiforgery = new MockAntiforgery();
        var filter = new AntiforgeryCookieResultFilterAttribute(antiforgery);
        var httpContext = new DefaultHttpContext();
        var context = CreateContext(new OkResult(), httpContext);

        filter.OnResultExecuting(context);

        Assert.Equal(0, antiforgery.GetTokensCalls);
        Assert.Empty(httpContext.Response.Headers.SetCookie.ToString());
    }
}
