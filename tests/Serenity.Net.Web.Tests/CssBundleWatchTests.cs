using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serenity.Abstractions;
using Serenity.Extensions.DependencyInjection;
using Serenity.Web;
using System.IO;
using System.Linq;
using Xunit;

namespace Serenity.Net.Entity.Tests
{
    public class CssBundleWatchTests
    {
        [Fact]
        public void When_Css_File_Changes_It_Reloads_Bundle()
        {
            var tempDir = ScriptBundleWatchTests.CreateTempDir();
            try
            {
                var testFile = Path.Combine(tempDir, "test.css");
                File.WriteAllText(testFile, "before");
                var container = new ServiceCollection();
                container.AddSingleton<IWebHostEnvironment>(
                    new ScriptBundleWatchTests.FakeWebHostEnvironment(tempDir));
                container.AddSingleton<IPermissionService, 
                    ScriptBundleWatchTests.FakePermissions>();
                container.AddCssBundling(options =>
                {
                    options.Enabled = true;
                    options.Bundles["Test"] = new string[]
                    {
                        "~/" + Path.GetFileName(testFile)
                    };
                });
                var services = container.BuildServiceProvider();
                services.UseCssWatching(tempDir);
                var scriptManager = services.GetRequiredService<IDynamicScriptManager>();
                var before = scriptManager.GetScriptText("CssBundle.Test");
                Assert.Equal("before", before?.Replace(";", "").Trim());
                File.WriteAllText(testFile, "after");
                var fileWatcherFactory = services.GetRequiredService<IFileWatcherFactory>();
                fileWatcherFactory.Watchers.Single().RaiseChanged(Path.GetFileName(testFile));
                var after = scriptManager.GetScriptText("CssBundle.Test");
                Assert.Equal("after", after?.Replace(";", "").Trim());
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }
    }
}
