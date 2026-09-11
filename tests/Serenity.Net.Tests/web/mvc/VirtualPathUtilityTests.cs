namespace Serenity.Web;

public class VirtualPathUtilityTests
{
    [Fact]
    public void ToAbsolute_With_Null_Accessor_Resolves_Without_PathBase()
    {
        Assert.Equal("/content/site.css",
            VirtualPathUtility.ToAbsolute((IHttpContextAccessor?)null, "~/content/site.css"));
    }

    [Fact]
    public void ToAbsolute_With_Null_Context_Resolves_Without_PathBase()
    {
        Assert.Equal("/content/site.css",
            VirtualPathUtility.ToAbsolute((HttpContext?)null, "~/content/site.css"));
    }

    [Fact]
    public void ToAbsolute_With_Accessor_Uses_Request_PathBase()
    {
        var context = new DefaultHttpContext();
        context.Request.PathBase = "/app";
        var accessor = new MockHttpContextAccessor { HttpContext = context };

        Assert.Equal("/app/content/site.css", VirtualPathUtility.ToAbsolute(accessor, "~/content/site.css"));
    }

    [Fact]
    public void ToAbsolute_With_Context_Uses_Request_PathBase()
    {
        var context = new DefaultHttpContext();
        context.Request.PathBase = "/app";

        Assert.Equal("/app/content/site.css", VirtualPathUtility.ToAbsolute(context, "~/content/site.css"));
    }

    [Fact]
    public void ToAbsolute_With_Empty_PathBase_Returns_Resolved_Path()
    {
        Assert.Equal("/content/site.css",
            VirtualPathUtility.ToAbsolute(PathString.Empty, "~/content/site.css"));
    }

    [Fact]
    public void ToAbsolute_With_PathBase_Combines_Paths()
    {
        Assert.Equal("/app/content/site.css",
            VirtualPathUtility.ToAbsolute(new PathString("/app"), "~/content/site.css"));
    }

    [Fact]
    public void ToAbsolute_Returns_Path_As_Is_When_Not_App_Relative()
    {
        Assert.Equal("http://example.com/site.css",
            VirtualPathUtility.ToAbsolute(new PathString("/app"), "http://example.com/site.css"));
    }
}
