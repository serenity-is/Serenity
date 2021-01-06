using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serenity.Abstractions;
using Serenity.Extensions.DependencyInjection;
using Serenity.Web;
using Xunit;

namespace Serenity.Tests.Web
{
    public class CssBundleRewriteTests
    {
        [Theory]
        [InlineData(@"Content\oneup.css", @"""../fonts/oneup.eot?#iefix""", @"""../fonts/oneup.eot?#iefix""")]
        [InlineData(@"Content\one\two\twoup.css", @"""../../images/o.png""", @"""../Content/images/o.png""")]
        [InlineData(@"one\two\twoup.css", @"""../../images/o.png""", @"""../images/o.png""")]
        [InlineData(@"one\two\twoup.css", @"'../../images/o.png'", @"'../images/o.png'")]
        [InlineData(@"one\two\twoup.css", @"../../images/o.png?v2", @"../images/o.png?v2")]
        [InlineData(@"dotslash.css", @"./test.png", @"../test.png")]
        [InlineData(@"sub\dotslash.css", @"./test.png", @"../sub/test.png")]
        [InlineData(@"sub\same.css", @"same.png", @"../sub/same.png")]
        [InlineData(@"http.css", @"http://some.png", @"http://some.png")]
        [InlineData(@"https.css", @"https://some.png", @"https://some.png")]
        [InlineData(@"data.css", @"data:abc", @"data:abc")]
        [InlineData(@"dataquote.css", @"'data:abc'", @"'data:abc'")]
        [InlineData(@"datadouble.css", @"""data:abc""", @"""data:abc""")]
        public void Css_Paths_Are_Rewritten_Properly(string path, string url, string expected)
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
                options.Bundles["Test"] = new string[]
                {
                    cssUrl
                };
            });
            var services = container.BuildServiceProvider();
            var scriptManager = services.GetRequiredService<IDynamicScriptManager>();
            var cssBundleManager = services.GetRequiredService<ICssBundleManager>();
            cssBundleManager.GetCssBundle(cssUrl);
            var actual = scriptManager.GetScriptText("CssBundle.Test")?.Trim();
            Assert.Equal("url(" + expected + ")", actual);
        }
    }
}
