namespace Serenity.Tests.Web;

public class ScriptBundleWatchTests
{
    [Fact]
    public void When_Script_File_Changes_It_Reloads_Bundle()
    {
        var env = new MockHostEnvironment();
        var testFile = env.Path.Combine(env.WebRootPath, "test.js");
        env.AddWebFile(testFile, "before");
        var fileWatcherFactory = new MockFileWatcherFactory(env.FileSystem);
        var container = new ServiceCollection();
        container.AddSingleton<IWebHostEnvironment>(env);
        container.AddSingleton<IPermissionService, MockPermissions>();
        container.AddSingleton<IFileWatcherFactory>(fileWatcherFactory);
        container.AddScriptBundling(options =>
        {
            options.Enabled = true;
            options.Bundles["Test"] = new string[]
            {
                "~/" + env.Path.GetFileName(testFile)
            };
        });
        var services = container.BuildServiceProvider();
        services.UseScriptWatching(env.Path.GetDirectoryName(testFile));
        var scriptManager = services.GetRequiredService<IDynamicScriptManager>();

        var before = scriptManager.GetScriptText("Bundle.Test");
        Assert.Equal("before", before?.Replace(";", "").Trim());

        env.File.WriteAllText(testFile, "after");
        fileWatcherFactory.Watchers.Single().RaiseChanged("test.js");

        var after = scriptManager.GetScriptText("Bundle.Test");
        Assert.Equal("after", after?.Replace(";", "").Trim());
    }
}
