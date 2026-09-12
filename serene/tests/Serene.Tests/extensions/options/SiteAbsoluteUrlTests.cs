namespace Serenity.Extensions;

public class SiteAbsoluteUrlTests
{
    [Fact]
    public void Constructor_Throws_For_Null_EnvironmentSettings()
    {
        Assert.Throws<ArgumentNullException>(() => new SiteAbsoluteUrl(null!));
    }

    [Fact]
    public void GetExternalUrl_Throws_When_Unavailable()
    {
        var siteUrl = new SiteAbsoluteUrl(new MockOptionsMonitor<EnvironmentSettings>(new EnvironmentSettings()));
        Assert.Throws<InvalidOperationException>(() => siteUrl.GetExternalUrl());
    }

    [Fact]
    public void GetExternalUrl_Uses_SiteExternalUrl_When_No_HttpContext()
    {
        var settings = new EnvironmentSettings { SiteExternalUrl = "https://external.example.com" };
        var siteUrl = new SiteAbsoluteUrl(new MockOptionsMonitor<EnvironmentSettings>(settings));
        Assert.Equal("https://external.example.com", siteUrl.GetExternalUrl());
    }

    [Fact]
    public void GetExternalUrl_Prefers_HttpContext_BaseUri()
    {
        var context = new DefaultHttpContext();
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("request.example.com");
        var accessor = new MockHttpContextAccessor { HttpContext = context };
        var settings = new EnvironmentSettings { SiteExternalUrl = "https://external.example.com" };
        var siteUrl = new SiteAbsoluteUrl(new MockOptionsMonitor<EnvironmentSettings>(settings), accessor);
        Assert.Equal("https://request.example.com/", siteUrl.GetExternalUrl());
    }

    [Fact]
    public void GetInternalUrl_Uses_SiteInternalUrl()
    {
        var settings = new EnvironmentSettings { SiteInternalUrl = "http://internal.example.com" };
        var siteUrl = new SiteAbsoluteUrl(new MockOptionsMonitor<EnvironmentSettings>(settings));
        Assert.Equal("http://internal.example.com", siteUrl.GetInternalUrl());
    }

    [Fact]
    public void GetInternalUrl_Falls_Back_To_External()
    {
        var settings = new EnvironmentSettings { SiteExternalUrl = "https://external.example.com" };
        var siteUrl = new SiteAbsoluteUrl(new MockOptionsMonitor<EnvironmentSettings>(settings));
        Assert.Equal("https://external.example.com", siteUrl.GetInternalUrl());
    }

    [Fact]
    public void GetInternalUrl_Throws_When_Unavailable()
    {
        var siteUrl = new SiteAbsoluteUrl(new MockOptionsMonitor<EnvironmentSettings>(new EnvironmentSettings()));
        Assert.Throws<InvalidOperationException>(() => siteUrl.GetInternalUrl());
    }
}
