using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Serenity.Abstractions;
using Serenity.Extensions.DependencyInjection;
using Serenity.Web;
using System;
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
            var tempDir = CreateTempDir();
            try
            {
                var testFile = Path.Combine(tempDir, "test.css");
                File.WriteAllText(testFile, "before");
                var container = new ServiceCollection();
                container.AddSingleton<IWebHostEnvironment>(new FakeWebHostEnvironment(tempDir));
                container.AddSingleton<IPermissionService, FakePermissions>();
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

        private class FakePermissions : IPermissionService
        {
            public bool HasPermission(string permission)
            {
                return true;
            }
        }

        private class FakeWebHostEnvironment : IWebHostEnvironment
        {
            public FakeWebHostEnvironment(string webRootPath)
            {
                WebRootPath = webRootPath;
            }

            public string WebRootPath { get; set; }
            public IFileProvider WebRootFileProvider { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public string ApplicationName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public IFileProvider ContentRootFileProvider { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public string ContentRootPath { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public string EnvironmentName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        }

        private static string CreateTempDir()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), ".serenitytests", Path.GetRandomFileName());
            if (Directory.Exists(tempDir))
                throw new InvalidOperationException();
            Directory.CreateDirectory(tempDir);
            return tempDir;
        }
    }
}
