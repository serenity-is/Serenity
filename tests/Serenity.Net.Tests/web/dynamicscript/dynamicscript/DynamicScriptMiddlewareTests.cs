using System.IO.Compression;

namespace Serenity.Web.Middleware;

public class DynamicScriptMiddlewareTests
{
    private static (DynamicScriptMiddleware middleware, MockDynamicScriptManager manager, DefaultHttpContext context, Func<bool> nextCalled) Create(string path)
    {
        var manager = new MockDynamicScriptManager();
        var services = new ServiceCollection()
            .AddSingleton<IDynamicScriptManager>(manager)
            .BuildServiceProvider();
        var context = new DefaultHttpContext { RequestServices = services };
        context.Request.Path = path;
        context.Response.Body = new System.IO.MemoryStream();
        bool called = false;
        var middleware = new DynamicScriptMiddleware(_ => { called = true; return Task.CompletedTask; });
        return (middleware, manager, context, () => called);
    }

    private static string ReadBody(HttpContext context)
    {
        context.Response.Body.Position = 0;
        using var reader = new System.IO.StreamReader(context.Response.Body);
        return reader.ReadToEnd();
    }

    [Fact]
    public async Task Invoke_Calls_Next_For_Other_Paths()
    {
        var (middleware, _, context, nextCalled) = Create("/other");

        await middleware.Invoke(context);

        Assert.True(nextCalled());
    }

    [Fact]
    public async Task Invoke_Serves_JavaScript_For_DynJS()
    {
        var (middleware, manager, context, nextCalled) = Create("/DynJS.axd/MyScript.js");
        manager.ReadScriptContentCallback = (name, json) =>
        {
            Assert.Equal("MyScript", name);
            Assert.False(json);
            return new ScriptContent("alert(1)"u8.ToArray(), DateTime.UtcNow, CompressionLevel.NoCompression);
        };

        await middleware.Invoke(context);

        Assert.False(nextCalled());
        Assert.Contains("text/javascript", context.Response.ContentType);
        Assert.Equal("alert(1)", ReadBody(context));
    }

    [Fact]
    public async Task Invoke_Serves_Css_For_DynJS_Css()
    {
        var (middleware, manager, context, _) = Create("/DynJS.axd/MyStyle.css");
        manager.ReadScriptContentCallback = (name, _) =>
        {
            Assert.Equal("MyStyle", name);
            return new ScriptContent("body{}"u8.ToArray(), DateTime.UtcNow, CompressionLevel.NoCompression);
        };

        await middleware.Invoke(context);

        Assert.Contains("text/css", context.Response.ContentType);
    }

    [Fact]
    public async Task Invoke_Serves_Json_For_DynamicData()
    {
        var (middleware, manager, context, _) = Create("/DynamicData/MyData");
        manager.ReadScriptContentCallback = (name, json) =>
        {
            Assert.Equal("MyData", name);
            Assert.True(json);
            return new ScriptContent("{}"u8.ToArray(), DateTime.UtcNow, CompressionLevel.NoCompression);
        };

        await middleware.Invoke(context);

        Assert.Contains("application/json", context.Response.ContentType);
    }

    [Fact]
    public async Task ReturnScript_Returns_NotFound_When_Content_Null()
    {
        var (_, manager, context, _) = Create("/DynJS.axd/Unknown.js");

        await DynamicScriptMiddleware.ReturnScript(context, "Unknown", "text/javascript", json: false);

        Assert.Equal(404, context.Response.StatusCode);
        Assert.Equal("File not found!", ReadBody(context));
    }

    [Fact]
    public async Task ReturnScript_Returns_Forbidden_On_AccessDenied()
    {
        var (_, manager, context, _) = Create("/DynJS.axd/X.js");
        manager.ReadScriptContentCallback = (_, _) => throw new ValidationError("AccessDenied", null, "denied");

        await DynamicScriptMiddleware.ReturnScript(context, "X", "text/javascript", json: false);

        Assert.Equal(403, context.Response.StatusCode);
    }

    [Fact]
    public async Task ReturnScript_Rethrows_Other_Validation_Errors()
    {
        var (_, manager, context, _) = Create("/DynJS.axd/X.js");
        manager.ReadScriptContentCallback = (_, _) => throw new ValidationError("Other", null, "other");

        await Assert.ThrowsAsync<ValidationError>(() =>
            DynamicScriptMiddleware.ReturnScript(context, "X", "text/javascript", json: false));
    }

    [Fact]
    public async Task ReturnScript_Compresses_With_Brotli()
    {
        var (_, manager, context, _) = Create("/DynJS.axd/X.js");
        context.Request.Headers.AcceptEncoding = "br";
        manager.ReadScriptContentCallback = (_, _) =>
            new ScriptContent(System.Text.Encoding.UTF8.GetBytes(new string('a', 2000)), DateTime.UtcNow, CompressionLevel.Optimal);

        await DynamicScriptMiddleware.ReturnScript(context, "X", "text/javascript", json: false);

        Assert.Equal("br", context.Response.Headers.ContentEncoding.ToString());
    }

    [Fact]
    public async Task ReturnScript_Compresses_With_Gzip()
    {
        var (_, manager, context, _) = Create("/DynJS.axd/X.js");
        context.Request.Headers.AcceptEncoding = "gzip";
        manager.ReadScriptContentCallback = (_, _) =>
            new ScriptContent(System.Text.Encoding.UTF8.GetBytes(new string('a', 2000)), DateTime.UtcNow, CompressionLevel.Optimal);

        await DynamicScriptMiddleware.ReturnScript(context, "X", "text/javascript", json: false);

        Assert.Equal("gzip", context.Response.Headers.ContentEncoding.ToString());
    }

    [Fact]
    public async Task ReturnScript_Marks_Public_Cache_For_Anonymous_With_Version()
    {
        var (_, manager, context, _) = Create("/DynJS.axd/X.js");
        context.Request.QueryString = new QueryString("?v=1");
        manager.ReadScriptContentCallback = (_, _) =>
            new ScriptContent("x"u8.ToArray(), DateTime.UtcNow, CompressionLevel.NoCompression);

        await DynamicScriptMiddleware.ReturnScript(context, "X", "text/javascript", json: false);

        Assert.Contains("public", context.Response.Headers.CacheControl.ToString());
    }

    [Fact]
    public async Task ReturnScript_Marks_Private_Cache_Otherwise()
    {
        var (_, manager, context, _) = Create("/DynJS.axd/X.js");
        manager.ReadScriptContentCallback = (_, _) =>
            new ScriptContent("x"u8.ToArray(), DateTime.UtcNow, CompressionLevel.NoCompression);

        await DynamicScriptMiddleware.ReturnScript(context, "X", "text/javascript", json: false);

        Assert.Contains("private", context.Response.Headers.CacheControl.ToString());
    }

    [Fact]
    public async Task WriteWithIfModifiedSinceControl_Throws_For_Null_Context()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            DynamicScriptMiddleware.WriteWithIfModifiedSinceControl(null!, [1], DateTime.UtcNow));
    }

    [Fact]
    public async Task WriteWithIfModifiedSinceControl_Returns_NotModified()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new System.IO.MemoryStream();
        var lastWrite = new DateTime(2024, 1, 2, 3, 4, 5, DateTimeKind.Utc);
        context.Request.Headers.IfModifiedSince = lastWrite.ToString("R", System.Globalization.CultureInfo.InvariantCulture);

        await DynamicScriptMiddleware.WriteWithIfModifiedSinceControl(context, [1, 2], lastWrite);

        Assert.Equal(304, context.Response.StatusCode);
    }

    [Fact]
    public async Task WriteWithIfModifiedSinceControl_Writes_Content_And_LastModified()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new System.IO.MemoryStream();

        await DynamicScriptMiddleware.WriteWithIfModifiedSinceControl(context, [1, 2, 3],
            new DateTime(2024, 1, 2, 3, 4, 5, DateTimeKind.Utc));

        Assert.Equal(200, context.Response.StatusCode);
        Assert.Equal(3, context.Response.Body.Length);
        Assert.False(string.IsNullOrEmpty(context.Response.Headers.LastModified.ToString()));
    }
}
