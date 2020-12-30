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
    public class ScriptBundleWatchTests
    {
        [Fact]
        public void When_Script_File_Changes_It_Reloads_Bundle()
        {
            var tempDir = CreateTempDir();
            try
            {
                var testFile = Path.Combine(tempDir, "test.js");
                File.WriteAllText(testFile, "before");
                var container = new ServiceCollection();
                container.AddSingleton<IWebHostEnvironment>(new FakeWebHostEnvironment(tempDir));
                container.AddSingleton<IPermissionService, FakePermissions>();
                container.AddScriptBundling(options =>
                {
                    options.Enabled = true;
                    options.Bundles["Test"] = new string[]
                    {
                        "~/" + Path.GetFileName(testFile)
                    };
                });
                var services = container.BuildServiceProvider();
                services.UseScriptWatching(tempDir);
                var scriptManager = services.GetRequiredService<IDynamicScriptManager>();
                var before = scriptManager.GetScriptText("Bundle.Test");
                Assert.Equal("before", before?.Replace(";", "").Trim());
                File.WriteAllText(testFile, "after");
                var fileWatcherFactory = services.GetRequiredService<IFileWatcherFactory>();
                fileWatcherFactory.Watchers.Single().RaiseChanged("test.js");
                var after = scriptManager.GetScriptText("Bundle.Test");
                Assert.Equal("after", after?.Replace(";", "").Trim());
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        internal class FakePermissions : IPermissionService
        {
            public bool HasPermission(string permission)
            {
                return true;
            }
        }

        internal class FakeWebHostEnvironment : IWebHostEnvironment
        {
            public FakeWebHostEnvironment(string webRootPath)
            {
                WebRootPath = webRootPath;
                WebRootFileProvider = new PhysicalFileProvider(webRootPath);
            }

            public string WebRootPath { get; set; }
            public IFileProvider WebRootFileProvider { get; set; }
            public string ApplicationName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public IFileProvider ContentRootFileProvider { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public string ContentRootPath { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public string EnvironmentName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        }

        internal static string CreateTempDir()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), ".serenitytests", Path.GetRandomFileName());
            if (Directory.Exists(tempDir))
                throw new InvalidOperationException();
            Directory.CreateDirectory(tempDir);
            return tempDir;
        }
    }
}
