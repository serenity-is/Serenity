using Microsoft.AspNetCore.Antiforgery;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Serenity.Web;

public class AutoValidateAntiforgeryIgnoreBearerFilterTests
{
    [Fact]
    public void CanCreateInstance()
    {
        var filter = new AutoValidateAntiforgeryIgnoreBearerFilter(
            new MockAntiforgery(), null, Options.Create(new AntiforgeryFilterOptions()));

        Assert.NotNull(filter);
    }

    [Fact]
    public void ShouldValidate_ReturnsFalse_ForGetMethod()
    {
        var (filter, _) = CreateFilter();
        var context = CreateContext(HttpMethods.Get, filter);

        Assert.False(filter.ShouldValidate(context));
    }

    [Fact]
    public void ShouldValidate_ReturnsFalse_ForHeadMethod()
    {
        var (filter, _) = CreateFilter();
        var context = CreateContext(HttpMethods.Head, filter);

        Assert.False(filter.ShouldValidate(context));
    }

    [Fact]
    public void ShouldValidate_ReturnsFalse_ForTraceMethod()
    {
        var (filter, _) = CreateFilter();
        var context = CreateContext(HttpMethods.Trace, filter);

        Assert.False(filter.ShouldValidate(context));
    }

    [Fact]
    public void ShouldValidate_ReturnsFalse_ForOptionsMethod()
    {
        var (filter, _) = CreateFilter();
        var context = CreateContext(HttpMethods.Options, filter);

        Assert.False(filter.ShouldValidate(context));
    }

    [Fact]
    public void ShouldValidate_ReturnsFalse_ForBearerWithoutCookie()
    {
        var (filter, _) = CreateFilter();
        var context = CreateContext(HttpMethods.Post, filter);
        context.HttpContext.Request.Headers.Authorization = "Bearer token";
        context.HttpContext.Request.Headers.Remove("Cookie");

        Assert.False(filter.ShouldValidate(context));
    }

    [Fact]
    public void ShouldValidate_ReturnsTrue_ForBearerWithCookie()
    {
        var (filter, _) = CreateFilter();
        var context = CreateContext(HttpMethods.Post, filter);
        context.HttpContext.Request.Headers.Authorization = "Bearer token";
        context.HttpContext.Request.Headers.Cookie = "some=cookie";

        Assert.True(filter.ShouldValidate(context));
    }

    [Fact]
    public void ShouldValidate_ReturnsFalse_WhenSkipHeaderPresentWithDefaults()
    {
        var options = new AntiforgeryFilterOptions();
        var (filter, _) = CreateFilter(options);
        var context = CreateContext(HttpMethods.Post, filter);
        context.HttpContext.Request.Headers["X-CSRF-SKIP"] = "true";

        Assert.False(filter.ShouldValidate(context));
    }

    [Fact]
    public void ShouldValidate_ReturnsFalse_WhenSkipHeaderPresent_WithCaseInsensitiveKey()
    {
        var options = new AntiforgeryFilterOptions();
        var (filter, _) = CreateFilter(options);
        var context = CreateContext(HttpMethods.Post, filter);
        context.HttpContext.Request.Headers["X-csrf-skiP"] = "true";

        Assert.False(filter.ShouldValidate(context));
    }

    [Fact]
    public void ShouldValidate_ReturnsFalse_WhenSkipHeaderPresent_WithCaseInsensitiveValue()
    {
        var options = new AntiforgeryFilterOptions();
        var (filter, _) = CreateFilter(options);
        var context = CreateContext(HttpMethods.Post, filter);
        context.HttpContext.Request.Headers["X-CSRF-SKIP"] = "TruE";

        Assert.False(filter.ShouldValidate(context));
    }

    [Fact]
    public void ShouldValidate_ReturnsFalse_WhenSkipHeaderPresent()
    {
        var options = new AntiforgeryFilterOptions
        {
            SkipValidationHeaderName = "X-Skip-Antiforgery",
            SkipValidationHeaderValue = "true"
        };
        var (filter, _) = CreateFilter(options);
        var context = CreateContext(HttpMethods.Post, filter);
        context.HttpContext.Request.Headers["X-Skip-Antiforgery"] = "true";

        Assert.False(filter.ShouldValidate(context));
    }

    [Fact]
    public void ShouldValidate_ReturnsTrue_WhenSkipHeaderNotPresent()
    {
        var options = new AntiforgeryFilterOptions
        {
            SkipValidationHeaderName = "X-Skip-Antiforgery"
        };
        var (filter, _) = CreateFilter(options);
        var context = CreateContext(HttpMethods.Post, filter);

        Assert.True(filter.ShouldValidate(context));
    }

    [Fact]
    public void ShouldValidate_ReturnsTrue_ForPostMethod()
    {
        var (filter, _) = CreateFilter();
        var context = CreateContext(HttpMethods.Post, filter);

        Assert.True(filter.ShouldValidate(context));
    }

    [Fact]
    public async Task OnAuthorizationAsync_Validates_WhenShouldValidate()
    {
        var (filter, mock) = CreateFilter();
        var context = CreateContext(HttpMethods.Post, filter);

        await filter.OnAuthorizationAsync(context);

        Assert.Equal(1, mock.ValidateRequestAsyncCalls);
        Assert.Null(context.Result);
    }

    [Fact]
    public async Task OnAuthorizationAsync_SetsResult_OnValidationFailure()
    {
        var antiforgery = new MockAntiforgery { ValidateRequestAsyncThrows = true };
        var (filter, mock) = CreateFilter(antiforgery);
        var context = CreateContext(HttpMethods.Post, filter);

        await filter.OnAuthorizationAsync(context);

        Assert.Equal(1, mock.ValidateRequestAsyncCalls);
        Assert.IsType<AntiforgeryValidationFailedResult>(context.Result);
    }

    private static (TestFilter filter, MockAntiforgery mock) CreateFilter(AntiforgeryFilterOptions options = null)
    {
        var mock = new MockAntiforgery();
        var filter = new TestFilter(mock, null, Options.Create(options ?? new AntiforgeryFilterOptions()));
        return (filter, mock);
    }

    private static (TestFilter filter, MockAntiforgery mock) CreateFilter(MockAntiforgery antiforgery, AntiforgeryFilterOptions options = null)
    {
        var filter = new TestFilter(antiforgery, null, Options.Create(options ?? new AntiforgeryFilterOptions()));
        return (filter, antiforgery);
    }

    private static AuthorizationFilterContext CreateContext(string method, TestFilter filter)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Method = method;
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        var context = new AuthorizationFilterContext(actionContext, []);
        if (filter != null)
            context.Filters.Add(filter);
        return context;
    }

    private class TestFilter(IAntiforgery antiforgery, ILoggerFactory loggerFactory, IOptions<AntiforgeryFilterOptions> options) : AutoValidateAntiforgeryIgnoreBearerFilter(antiforgery, loggerFactory, options)
    {
        public new bool ShouldValidate(AuthorizationFilterContext context) => base.ShouldValidate(context);
    }
}
