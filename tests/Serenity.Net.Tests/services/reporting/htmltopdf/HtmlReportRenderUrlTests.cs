namespace Serenity.Reporting;

public class HtmlReportRenderUrlTests
{
    private class TestRenderUrl : HtmlReportRenderUrl
    {
        public int CleanupCount { get; private set; }
        public bool ThrowOnCleanup { get; set; }

        protected override void Cleanup()
        {
            CleanupCount++;
            if (ThrowOnCleanup)
                throw new InvalidOperationException("cleanup");
        }
    }

    [Fact]
    public void New_Instance_Has_Empty_Cookies()
    {
        var url = new HtmlReportRenderUrl();
        Assert.Empty(url.CookiesToForward);
        Assert.Null(url.Url);
        Assert.Null(url.FooterUrl);
        Assert.Null(url.HeaderUrl);
    }

    [Fact]
    public void GetTemporaryFolders_Returns_Empty()
    {
        var url = new HtmlReportRenderUrl();
        Assert.Empty(url.GetTemporaryFolders());
    }

    [Fact]
    public void Properties_Are_Settable()
    {
        var cookie = new System.Net.Cookie("name", "value");
        var url = new HtmlReportRenderUrl
        {
            Url = "main",
            FooterUrl = "footer",
            HeaderUrl = "header"
        };
        url.CookiesToForward.Add(cookie);

        Assert.Equal("main", url.Url);
        Assert.Equal("footer", url.FooterUrl);
        Assert.Equal("header", url.HeaderUrl);
        Assert.Single(url.CookiesToForward);
    }

    [Fact]
    public void Dispose_Calls_Cleanup_Once()
    {
        var url = new TestRenderUrl();
        url.Dispose();
        url.Dispose();

        Assert.Equal(1, url.CleanupCount);
    }

    [Fact]
    public void Dispose_Propagates_Cleanup_Exception_And_Stays_Disposed()
    {
        var url = new TestRenderUrl { ThrowOnCleanup = true };

        Assert.Throws<InvalidOperationException>(() => url.Dispose());
        Assert.Equal(1, url.CleanupCount);

        url.ThrowOnCleanup = false;
        url.Dispose();
        Assert.Equal(1, url.CleanupCount);
    }
}
