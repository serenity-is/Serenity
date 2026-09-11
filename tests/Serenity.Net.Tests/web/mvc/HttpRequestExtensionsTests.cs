namespace Serenity.Web;

public class HttpRequestExtensionsTests
{
    [Fact]
    public void GetBaseUri_Throws_When_Request_Is_Null()
    {
        Assert.Throws<ArgumentNullException>(() => HttpRequestExtensions.GetBaseUri(null!));
    }

    [Fact]
    public void GetBaseUri_Includes_PathBase_By_Default()
    {
        var context = new DefaultHttpContext();
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("example.com");
        context.Request.PathBase = "/app";

        var uri = context.Request.GetBaseUri();

        Assert.Equal("https://example.com/app", uri.AbsoluteUri);
    }

    [Fact]
    public void GetBaseUri_Excludes_PathBase_When_False()
    {
        var context = new DefaultHttpContext();
        context.Request.Scheme = "http";
        context.Request.Host = new HostString("example.com");
        context.Request.PathBase = "/app";

        var uri = context.Request.GetBaseUri(pathBase: false);

        Assert.Equal("http://example.com/", uri.AbsoluteUri);
    }

    [Fact]
    public void GetBaseUri_Keeps_Non_Default_Port()
    {
        var context = new DefaultHttpContext();
        context.Request.Scheme = "http";
        context.Request.Host = new HostString("example.com", 8080);

        var uri = context.Request.GetBaseUri();

        Assert.Equal(8080, uri.Port);
    }

    [Fact]
    public void GetBaseUri_Removes_Default_Port()
    {
        var context = new DefaultHttpContext();
        context.Request.Scheme = "http";
        context.Request.Host = new HostString("example.com", 80);

        var uri = context.Request.GetBaseUri();

        Assert.True(uri.IsDefaultPort);
        Assert.Equal("http://example.com/", uri.AbsoluteUri);
    }
}
