namespace Serenity.Tests.Web;

public class CssBundleRewriteTests
{
    [Theory]
    [InlineData("Test", @"Content/oneup.css", @"""../fonts/oneup.eot?#iefix""", @"""../fonts/oneup.eot?#iefix""")]
    [InlineData("Test", @"Content/one/two/twoup.css", @"""../../images/o.png""", @"""../Content/images/o.png""")]
    [InlineData("Test", @"one/two/twoup.css", @"""../../images/o.png""", @"""../images/o.png""")]
    [InlineData("Test", @"one/two/twoup.css", @"'../../images/o.png'", @"'../images/o.png'")]
    [InlineData("Test", @"one/two/twoup.css", @"../../images/o.png?v2", @"../images/o.png?v2")]
    [InlineData("Test", @"dotslash.css", @"./test.png", @"../test.png")]
    [InlineData("Test", @"sub/dotslash.css", @"./test.png", @"../sub/test.png")]
    [InlineData("Test", @"sub/same.css", @"same.png", @"../sub/same.png")]
    [InlineData("Test", @"http.css", @"http://some.png", @"http://some.png")]
    [InlineData("Test", @"https.css", @"https://some.png", @"https://some.png")]
    [InlineData("Test", @"data.css", @"data:abc", @"data:abc")]
    [InlineData("Test", @"dataquote.css", @"'data:abc'", @"'data:abc'")]
    [InlineData("Test", @"datadouble.css", @"""data:abc""", @"""data:abc""")]
    [InlineData("A/B", @"Content/oneup.css", @"""../fonts/oneup.eot?#iefix""", @"""../../fonts/oneup.eot?#iefix""")]
    [InlineData("A/B", @"Content/one/two/twoup.css", @"""../../images/o.png""", @"""../../Content/images/o.png""")]
    [InlineData("A/B", @"one/two/twoup.css", @"""../../images/o.png""", @"""../../images/o.png""")]
    [InlineData("A/B", @"one/two/twoup.css", @"'../../images/o.png'", @"'../../images/o.png'")]
    [InlineData("A/B", @"one/two/twoup.css", @"../../images/o.png?v2", @"../../images/o.png?v2")]
    [InlineData("A/B", @"dotslash.css", @"./test.png", @"../../test.png")]
    [InlineData("A/B", @"sub/dotslash.css", @"./test.png", @"../../sub/test.png")]
    [InlineData("A/B", @"sub/same.css", @"same.png", @"../../sub/same.png")]
    [InlineData("A/B", @"http.css", @"http://some.png", @"http://some.png")]
    [InlineData("A/B", @"https.css", @"https://some.png", @"https://some.png")]
    [InlineData("A/B", @"data.css", @"data:abc", @"data:abc")]
    [InlineData("A/B", @"dataquote.css", @"'data:abc'", @"'data:abc'")]
    [InlineData("A/B", @"datadouble.css", @"""data:abc""", @"""data:abc""")]
    [InlineData("A/B/C", @"Content/oneup.css", @"""../fonts/oneup.eot?#iefix""", @"""../../../fonts/oneup.eot?#iefix""")]
    [InlineData("A/B/C", @"Content/one/two/twoup.css", @"""../../images/o.png""", @"""../../../Content/images/o.png""")]
    [InlineData("A/B/C", @"one/two/twoup.css", @"""../../images/o.png""", @"""../../../images/o.png""")]
    [InlineData("A/B/C", @"one/two/twoup.css", @"'../../images/o.png'", @"'../../../images/o.png'")]
    [InlineData("A/B/C", @"one/two/twoup.css", @"../../images/o.png?v2", @"../../../images/o.png?v2")]
    [InlineData("A/B/C", @"dotslash.css", @"./test.png", @"../../../test.png")]
    [InlineData("A/B/C", @"sub/dotslash.css", @"./test.png", @"../../../sub/test.png")]
    [InlineData("A/B/C", @"sub/same.css", @"same.png", @"../../../sub/same.png")]
    [InlineData("A/B/C", @"http.css", @"http://some.png", @"http://some.png")]
    [InlineData("A/B/C", @"https.css", @"https://some.png", @"https://some.png")]
    [InlineData("A/B/C", @"data.css", @"data:abc", @"data:abc")]
    [InlineData("A/B/C", @"dataquote.css", @"'data:abc'", @"'data:abc'")]
    [InlineData("A/B/C", @"datadouble.css", @"""data:abc""", @"""data:abc""")]
    public void Css_Paths_Are_Rewritten_Properly(string bundleName, string path, string url, string expected)
    {
        var env = new MockHostEnvironment();
        env.AddWebFile(path, "url(" + url + ")");

        var container = new ServiceCollection();
        container.AddSingleton<IWebHostEnvironment>(env);
        container.AddSingleton<IPermissionService, MockPermissions>();
        var cssUrl = "~/" + PathHelper.ToUrl(path);
        container.AddCssBundling(options =>
        {
            options.Enabled = true;
            options.Bundles[bundleName] = new string[]
            {
                cssUrl
            };
        });
        var services = container.BuildServiceProvider();
        var scriptManager = services.GetRequiredService<IDynamicScriptManager>();
        var cssBundleManager = services.GetRequiredService<ICssBundleManager>();
        cssBundleManager.GetCssBundle(cssUrl);
        var actual = scriptManager.GetScriptText("CssBundle." + bundleName)?.Trim();
        Assert.Equal("url(" + expected + ")", actual);
    }
}
