using Microsoft.AspNetCore.DataProtection;
using System.Net;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Serenity.Reporting;

public class HtmlReportCallbackUrlBuilderTests
{
    private class StubSiteUrl : ISiteAbsoluteUrl
    {
        public string InternalUrl { get; set; } = "https://internal.example.com";
        public string GetInternalUrl() => InternalUrl;
        public string GetExternalUrl() => InternalUrl;
    }

    private class TestReport : IReport
    {
        public object? GetData() => new { A = 1 };
    }

    [Report("My.Report")]
    private class AttributedReport : IReport
    {
        public object? GetData() => null;
    }

    [Fact]
    public void Constructor_Throws_For_Null_SiteAbsoluteUrl()
    {
        Assert.Throws<ArgumentNullException>(() => new HtmlReportCallbackUrlBuilder(null!));
    }

    [Fact]
    public void GetRenderUrl_Throws_For_Null_Arguments()
    {
        var builder = new HtmlReportCallbackUrlBuilder(new StubSiteUrl());
        Assert.Throws<ArgumentNullException>(() => builder.GetRenderUrl(null!, new ReportRenderOptions()));
        Assert.Throws<ArgumentNullException>(() => builder.GetRenderUrl(new TestReport(), null!));
    }

    [Fact]
    public void GetRenderUrl_Builds_Url_With_Type_FullName()
    {
        var builder = new HtmlReportCallbackUrlBuilder(new StubSiteUrl());
        var result = builder.GetRenderUrl(new TestReport(), new ReportRenderOptions());

        Assert.Contains("https://internal.example.com", result.Url);
        Assert.Contains("Serenity.Extensions/Report/Render?key=", result.Url);
        Assert.Contains(Uri.EscapeDataString(typeof(TestReport).FullName!), result.Url);
        Assert.Contains("&print=1", result.Url);
        Assert.Empty(result.CookiesToForward);
    }

    [Fact]
    public void GetRenderUrl_Uses_Report_Attribute_Key_And_Options()
    {
        var builder = new HtmlReportCallbackUrlBuilder(new StubSiteUrl());
        var result = builder.GetRenderUrl(new AttributedReport(), new ReportRenderOptions
        {
            ReportParams = "{\"A\":1}"
        });

        Assert.Contains("key=My.Report", result.Url);
        Assert.Contains("&opt=" + Uri.EscapeDataString("{\"A\":1}"), result.Url);
    }

    [Fact]
    public void GetRenderUrl_Uses_Provided_ReportKey()
    {
        var builder = new HtmlReportCallbackUrlBuilder(new StubSiteUrl());
        var result = builder.GetRenderUrl(new TestReport(), new ReportRenderOptions { ReportKey = "override" });
        Assert.Contains("key=override", result.Url);
    }

    [Fact]
    public void GetRenderUrl_Adds_ReportAuth_Cookie_When_User_LoggedIn()
    {
        var accessor = new MockUserAccessor(() => "user");
        var builder = new HtmlReportCallbackUrlBuilder(new StubSiteUrl(),
            userAccessor: accessor, dataProtectionProvider: new EphemeralDataProtectionProvider());

        var result = builder.GetRenderUrl(new TestReport(), new ReportRenderOptions());

        var cookie = Assert.Single(result.CookiesToForward);
        Assert.Equal(".ReportAuth", cookie.Name);
    }

    [Fact]
    public void GetRenderUrl_Adds_ReportAuth_Cookie_When_Grants_Exist()
    {
        var grantor = new TransientGrantingPermissionService();
        grantor.Grant("perm1");
        var builder = new HtmlReportCallbackUrlBuilder(new StubSiteUrl(),
            permissionService: grantor, dataProtectionProvider: new EphemeralDataProtectionProvider());

        var result = builder.GetRenderUrl(new TestReport(), new ReportRenderOptions());
        Assert.Single(result.CookiesToForward);
    }

    [Fact]
    public void GetRenderUrl_No_ReportAuth_Cookie_When_No_User_Or_Grants()
    {
        var builder = new HtmlReportCallbackUrlBuilder(new StubSiteUrl(),
            dataProtectionProvider: new EphemeralDataProtectionProvider());

        var result = builder.GetRenderUrl(new TestReport(), new ReportRenderOptions());
        Assert.Empty(result.CookiesToForward);
    }

    [Fact]
    public void GetRenderUrl_Forwards_Auth_And_Language_Cookies()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.Cookie = "authcookie=value; LanguagePreference=tr";
        var options = new MockOptionsMonitor<CookieAuthenticationOptions>(new CookieAuthenticationOptions
        {
            Cookie = { Name = "authcookie" }
        });
        var builder = new HtmlReportCallbackUrlBuilder(new StubSiteUrl(),
            cookieOptions: options,
            httpContextAccessor: new MockHttpContextAccessor { HttpContext = httpContext });

        var result = builder.GetRenderUrl(new TestReport(), new ReportRenderOptions());

        Assert.Equal(2, result.CookiesToForward.Count);
        Assert.Contains(result.CookiesToForward, x => x.Name == "authcookie" && x.Value == "value");
        Assert.Contains(result.CookiesToForward, x => x.Name == "LanguagePreference" && x.Value == "tr");
    }

    [Fact]
    public void GetRenderUrl_Forwards_Chunked_Auth_Cookies()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.Cookie = "authcookie=chunks-1; authcookieC1=chunkvalue";
        var options = new MockOptionsMonitor<CookieAuthenticationOptions>(new CookieAuthenticationOptions
        {
            Cookie = { Name = "authcookie" }
        });
        var builder = new HtmlReportCallbackUrlBuilder(new StubSiteUrl(),
            cookieOptions: options,
            httpContextAccessor: new MockHttpContextAccessor { HttpContext = httpContext });

        var result = builder.GetRenderUrl(new TestReport(), new ReportRenderOptions());

        Assert.Contains(result.CookiesToForward, x => x.Name == "authcookieC1" && x.Value == "chunkvalue");
    }
}

