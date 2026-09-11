namespace Serenity.Services;

public class HandleServiceExceptionAttributeTests
{
    private static ExceptionContext CreateContext(Exception exception)
    {
        var httpContext = new DefaultHttpContext();
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        return new ExceptionContext(actionContext, []) { Exception = exception };
    }

    [Fact]
    public void OnException_Returns_400_For_ValidationError()
    {
        var context = CreateContext(new ValidationError("test error"));
        new HandleServiceExceptionAttribute().OnException(context);

        Assert.True(context.ExceptionHandled);
        var result = Assert.IsType<ResultWithStatus<ServiceResponse>>(context.Result);
        Assert.Equal(400, result.StatusCode);
        Assert.NotNull(result.Data?.Error);
        Assert.Equal("test error", result.Data!.Error!.Message);
    }

    [Fact]
    public void OnException_Returns_500_For_Other_Exceptions()
    {
        var context = CreateContext(new InvalidOperationException("boom"));
        new HandleServiceExceptionAttribute().OnException(context);

        Assert.True(context.ExceptionHandled);
        var result = Assert.IsType<ResultWithStatus<ServiceResponse>>(context.Result);
        Assert.Equal(500, result.StatusCode);
        Assert.NotNull(result.Data?.Error);
    }
}
