namespace Serenity.Services;

public class ResultTests
{
    private class TestData
    {
        public string? Name { get; set; }
    }

    private static ActionContext CreateContext(HttpContext? httpContext = null)
    {
        httpContext ??= new DefaultHttpContext();
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
    public void Data_Is_Set_From_Constructor()
    {
        var data = new TestData { Name = "test" };
        var result = new Result<TestData>(data);

        Assert.Same(data, result.Data);
    }

    [Fact]
    public async Task ExecuteResultAsync_Writes_Json_With_Default_ContentType()
    {
        var result = new Result<TestData>(new TestData { Name = "test" });
        var context = CreateContext();

        await result.ExecuteResultAsync(context);

        Assert.Equal("application/json", context.HttpContext.Response.ContentType);
        Assert.Contains("test", ReadBody(context));
    }

    [Fact]
    public async Task ExecuteResultAsync_Uses_Custom_ContentType()
    {
        var result = new Result<TestData>(new TestData()) { ContentType = "text/json" };
        var context = CreateContext();

        await result.ExecuteResultAsync(context);

        Assert.Equal("text/json", context.HttpContext.Response.ContentType);
    }

    [Fact]
    public async Task ExecuteResultAsync_Sets_ContentEncoding_Header()
    {
        var result = new Result<TestData>(new TestData()) { ContentEncoding = Encoding.UTF8 };
        var context = CreateContext();

        await result.ExecuteResultAsync(context);

        Assert.Equal("utf-8", context.HttpContext.Response.Headers.ContentEncoding.ToString());
    }

    [Fact]
    public async Task ExecuteResultAsync_Writes_Nothing_When_Data_Is_Null()
    {
        var result = new Result<TestData?>(null);
        var context = CreateContext();

        await result.ExecuteResultAsync(context);

        Assert.Empty(ReadBody(context));
    }

    [Fact]
    public async Task ExecuteResultAsync_Throws_When_Context_Is_Null()
    {
        var result = new Result<TestData>(new TestData());
        await Assert.ThrowsAsync<ArgumentNullException>(() => result.ExecuteResultAsync(null!));
    }
}
