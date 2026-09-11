namespace Serenity.Services;

public class HandleControllerExceptionAttributeTests
{
    private static ExceptionContext CreateContext(Exception exception)
    {
        var httpContext = new DefaultHttpContext();
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        return new ExceptionContext(actionContext, []) { Exception = exception };
    }

    [Fact]
    public void OnException_Returns_ValidationError_View()
    {
        var context = CreateContext(new ValidationError("test error") { ErrorCode = "TestCode" });
        new HandleControllerExceptionAttribute().OnException(context);

        Assert.True(context.ExceptionHandled);
        var result = Assert.IsType<ViewResult>(context.Result);
        Assert.Equal("~/Views/Errors/ValidationError.cshtml", result.ViewName);
        Assert.Equal("test error", Assert.IsType<ValidationError>(result.ViewData.Model).Message);
        Assert.Equal("TestCode", Assert.IsType<ValidationError>(result.ViewData.Model).ErrorCode);
    }

    [Fact]
    public void OnException_Uses_Generic_Message_When_No_Error()
    {
        var context = CreateContext(new InvalidOperationException());
        new HandleControllerExceptionAttribute().OnException(context);

        var result = Assert.IsType<ViewResult>(context.Result);
        Assert.Equal("An error occurred while processing your request.",
            Assert.IsType<ValidationError>(result.ViewData.Model).Message);
    }
}
