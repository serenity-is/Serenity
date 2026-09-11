namespace Serenity.Services;

public class ResultWithStatusTests
{
    private static ActionContext CreateContext()
    {
        var httpContext = new DefaultHttpContext
        {
            RequestServices = new ServiceCollection().AddLogging().BuildServiceProvider()
        };
        httpContext.Response.Body = new System.IO.MemoryStream();
        return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
    }

    private static string ReadBody(ActionContext context)
    {
        var stream = context.HttpContext.Response.Body;
        stream.Position = 0;
        using var reader = new System.IO.StreamReader(stream);
        return reader.ReadToEnd();
    }

    [Fact]
    public async Task ExecuteResultAsync_Sets_Status_Code_And_Writes_Json()
    {
        var result = new ResultWithStatus<ServiceResponse>(404, new ServiceResponse());
        var context = CreateContext();

        await result.ExecuteResultAsync(context);

        Assert.Equal(404, context.HttpContext.Response.StatusCode);
        Assert.Equal("application/json", context.HttpContext.Response.ContentType);
        Assert.Equal("{}", ReadBody(context));
    }

    [Fact]
    public async Task ExecuteResultAsync_Uses_Custom_ContentType_And_Encoding()
    {
        var result = new ResultWithStatus<ServiceResponse>(400, new ServiceResponse())
        {
            ContentType = "text/json",
            ContentEncoding = Encoding.UTF8
        };
        var context = CreateContext();

        await result.ExecuteResultAsync(context);

        Assert.Equal(400, context.HttpContext.Response.StatusCode);
        Assert.Equal("text/json", context.HttpContext.Response.ContentType);
        Assert.Equal("utf-8", context.HttpContext.Response.Headers.ContentEncoding.ToString());
    }

    [Fact]
    public async Task ExecuteResultAsync_Writes_Nothing_When_Data_Is_Null()
    {
        var result = new ResultWithStatus<ServiceResponse?>(500, null);
        var context = CreateContext();

        await result.ExecuteResultAsync(context);

        Assert.Equal(500, context.HttpContext.Response.StatusCode);
        Assert.Empty(ReadBody(context));
    }
}
