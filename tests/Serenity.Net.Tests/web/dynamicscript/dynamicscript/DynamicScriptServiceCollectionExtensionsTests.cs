using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Serenity.PropertyGrid;

namespace Serenity.Extensions.DependencyInjection;

public class DynamicScriptServiceCollectionExtensionsTests
{
    [Fact]
    public void AddDynamicScriptManager_Throws_For_Null()
    {
        Assert.Throws<ArgumentNullException>(() =>
            ((IServiceCollection)null!).AddDynamicScriptManager());
    }

    [Fact]
    public void AddDynamicScriptManager_Registers_Manager()
    {
        var services = new ServiceCollection();
        services.AddDynamicScriptManager();

        Assert.Contains(services, x => x.ServiceType == typeof(IDynamicScriptManager));
    }

    [Fact]
    public void AddDynamicScripts_Registers_Property_Item_Provider()
    {
        var services = new ServiceCollection();
        services.AddDynamicScripts();

        Assert.Contains(services, x => x.ServiceType == typeof(IPropertyItemProvider));
        Assert.Contains(services, x => x.ServiceType == typeof(IDynamicScriptManager));
    }

    [Fact]
    public void AddFileWatcherFactory_Registers_Factory()
    {
        var services = new ServiceCollection();
        services.AddFileWatcherFactory();

        Assert.Contains(services, x => x.ServiceType == typeof(IFileWatcherFactory));
        Assert.Throws<ArgumentNullException>(() =>
            ((IServiceCollection)null!).AddFileWatcherFactory());
    }

    [Fact]
    public void AddContentHashCache_Registers_Cache()
    {
        var services = new ServiceCollection();
        services.AddContentHashCache();

        Assert.Contains(services, x => x.ServiceType == typeof(IContentHashCache));
        Assert.Contains(services, x => x.ServiceType == typeof(IFileWatcherFactory));
        Assert.Throws<ArgumentNullException>(() =>
            ((IServiceCollection)null!).AddContentHashCache());
    }

    [Fact]
    public void AddCssBundling_Registers_Manager()
    {
        var services = new ServiceCollection();
        services.AddCssBundling();

        Assert.Contains(services, x => x.ServiceType == typeof(ICssBundleManager));
        Assert.Throws<ArgumentNullException>(() =>
            ((IServiceCollection)null!).AddCssBundling());
    }

    [Fact]
    public void AddScriptBundling_Registers_Manager()
    {
        var services = new ServiceCollection();
        services.AddScriptBundling();

        Assert.Contains(services, x => x.ServiceType == typeof(IScriptBundleManager));
        Assert.Throws<ArgumentNullException>(() =>
            ((IServiceCollection)null!).AddScriptBundling());
    }

    [Fact]
    public void AddCssAndScriptBundling_Registers_Both()
    {
        var services = new ServiceCollection();
        services.AddCssAndScriptBundling();

        Assert.Contains(services, x => x.ServiceType == typeof(ICssBundleManager));
        Assert.Contains(services, x => x.ServiceType == typeof(IScriptBundleManager));
    }

    [Fact]
    public void AddCssBundling_With_SetupAction_Configures_Options()
    {
        var services = new ServiceCollection();
        services.AddCssBundling(options => options.Enabled = true);

        Assert.Throws<ArgumentNullException>(() =>
            ((IServiceCollection)null!).AddCssBundling(_ => { }));
        Assert.Throws<ArgumentNullException>(() =>
            new ServiceCollection().AddCssBundling(null!));

        Assert.True(services.BuildServiceProvider()
            .GetRequiredService<IOptions<CssBundlingOptions>>().Value.Enabled);
    }

    [Fact]
    public void AddScriptBundling_With_SetupAction_Configures_Options()
    {
        var services = new ServiceCollection();
        services.AddScriptBundling(options => options.Enabled = true);

        Assert.Throws<ArgumentNullException>(() =>
            ((IServiceCollection)null!).AddScriptBundling(_ => { }));
        Assert.Throws<ArgumentNullException>(() =>
            new ServiceCollection().AddScriptBundling(null!));

        Assert.True(services.BuildServiceProvider()
            .GetRequiredService<IOptions<ScriptBundlingOptions>>().Value.Enabled);
    }

    [Fact]
    public void UseCssWatching_Throws_For_Null_Or_Empty()
    {
        var provider = new ServiceCollection().BuildServiceProvider();
        Assert.Throws<ArgumentNullException>(() =>
            DynamicScriptServiceCollectionExtensions.UseCssWatching(null!, "x"));
        Assert.Throws<ArgumentNullException>(() =>
            provider.UseCssWatching([]));
    }

    [Fact]
    public void UseCssWatching_Throws_When_No_Manager_Or_Cache()
    {
        var provider = new ServiceCollection().BuildServiceProvider();
        Assert.Throws<InvalidOperationException>(() => provider.UseCssWatching("path"));
    }

    [Fact]
    public void UseCssWatching_Creates_Watcher_And_Handles_Changes()
    {
        var bundleManager = new MockCssBundleManager();
        var hashCache = new MockContentHashCache();
        var watcherFactory = new MockFileWatcherFactory(new MockHostEnvironment().FileSystem);
        var provider = new ServiceCollection()
            .AddSingleton<ICssBundleManager>(bundleManager)
            .AddSingleton<IContentHashCache>(hashCache)
            .AddSingleton<IFileWatcherFactory>(watcherFactory)
            .BuildServiceProvider();

        provider.UseCssWatching("Content");
        var watcher = Assert.Single(watcherFactory.Watchers);
        watcher.RaiseChanged("a.css");

        Assert.Equal(1, bundleManager.CssChangedCalls);
        Assert.Equal(1, hashCache.ScriptsChangedCalls);
    }

    [Fact]
    public void UseScriptWatching_Throws_For_Null_Or_Empty()
    {
        var provider = new ServiceCollection().BuildServiceProvider();
        Assert.Throws<ArgumentNullException>(() =>
            DynamicScriptServiceCollectionExtensions.UseScriptWatching(null!, "x"));
        Assert.Throws<ArgumentNullException>(() => provider.UseScriptWatching([]));
    }

    [Fact]
    public void UseScriptWatching_Throws_When_No_Manager_Or_Cache()
    {
        var provider = new ServiceCollection().BuildServiceProvider();
        Assert.Throws<InvalidOperationException>(() => provider.UseScriptWatching("path"));
    }

    [Fact]
    public void UseScriptWatching_Creates_Watcher_And_Handles_Changes()
    {
        var bundleManager = new MockScriptBundleManager();
        var hashCache = new MockContentHashCache();
        var watcherFactory = new MockFileWatcherFactory(new MockHostEnvironment().FileSystem);
        var provider = new ServiceCollection()
            .AddSingleton<IScriptBundleManager>(bundleManager)
            .AddSingleton<IContentHashCache>(hashCache)
            .AddSingleton<IFileWatcherFactory>(watcherFactory)
            .BuildServiceProvider();

        provider.UseScriptWatching("Scripts");
        var watcher = Assert.Single(watcherFactory.Watchers);
        watcher.RaiseChanged("a.js");

        Assert.Equal(1, bundleManager.ScriptsChangedCalls);
        Assert.Equal(1, hashCache.ScriptsChangedCalls);
    }

    [Fact]
    public void AddLocalTextInitializer_Registers_Initializer()
    {
        var services = new ServiceCollection();
        services.AddLocalTextInitializer();

        Assert.Contains(services, x => x.ServiceType == typeof(ILocalTextInitializer));
    }

    [Fact]
    public void UseDynamicScriptTypes_Registers_Bundles()
    {
        var scriptManager = new MockDynamicScriptManager();
        var services = new ServiceCollection()
            .AddSingleton<IDynamicScriptManager>(scriptManager)
            .AddSingleton<IPropertyItemProvider>(new MockPropertyItemProvider())
            .AddSingleton<ITypeSource>(new MockTypeSource())
            .BuildServiceProvider();

        services.UseDynamicScriptTypes();

        Assert.True(scriptManager.IsRegistered("ColumnAndFormBundle"));
        Assert.True(scriptManager.IsRegistered("ColumnsBundle"));
        Assert.True(scriptManager.IsRegistered("FormBundle"));
    }

    [Fact]
    public void UseCssWatching_Default_Path_Watches_Content()
    {
        var env = new MockHostEnvironment();
        var watcherFactory = new MockFileWatcherFactory(env.FileSystem);
        var provider = new ServiceCollection()
            .AddSingleton<IWebHostEnvironment>(env)
            .AddSingleton<ICssBundleManager>(new MockCssBundleManager())
            .AddSingleton<IContentHashCache>(new MockContentHashCache())
            .AddSingleton<IFileWatcherFactory>(watcherFactory)
            .BuildServiceProvider();

        provider.UseCssWatching();

        var watcher = Assert.Single(watcherFactory.Watchers);
        Assert.Contains("Content", watcher.Path);
        Assert.Equal("*.css", watcher.Filter);
    }

    [Fact]
    public void UseScriptWatching_Default_Path_Watches_Scripts()
    {
        var env = new MockHostEnvironment();
        var watcherFactory = new MockFileWatcherFactory(env.FileSystem);
        var provider = new ServiceCollection()
            .AddSingleton<IWebHostEnvironment>(env)
            .AddSingleton<IScriptBundleManager>(new MockScriptBundleManager())
            .AddSingleton<IContentHashCache>(new MockContentHashCache())
            .AddSingleton<IFileWatcherFactory>(watcherFactory)
            .BuildServiceProvider();

        provider.UseScriptWatching();

        var watcher = Assert.Single(watcherFactory.Watchers);
        Assert.Contains("Scripts", watcher.Path);
        Assert.Equal("*.js", watcher.Filter);
    }

    [Fact]
    public void UseDynamicScripts_Configures_Everything()
    {
        var env = new MockHostEnvironment();
        var watcherFactory = new MockFileWatcherFactory(env.FileSystem);
        var services = new ServiceCollection()
            .AddSingleton<IDynamicScriptManager>(new MockDynamicScriptManager())
            .AddSingleton<IPropertyItemProvider>(new MockPropertyItemProvider())
            .AddSingleton<ITypeSource>(new MockTypeSource())
            .AddSingleton<IWebHostEnvironment>(env)
            .AddSingleton<ICssBundleManager>(new MockCssBundleManager())
            .AddSingleton<IScriptBundleManager>(new MockScriptBundleManager())
            .AddSingleton<IContentHashCache>(new MockContentHashCache())
            .AddSingleton<IFileWatcherFactory>(watcherFactory)
            .BuildServiceProvider();
        var app = new ApplicationBuilder(services);

        app.UseDynamicScripts();

        Assert.Equal(2, watcherFactory.Watchers.Count());
    }

    [Fact]
    public void InitializeLocalTexts_Throws_For_Null_App()
    {
        Assert.Throws<ArgumentNullException>(() =>
            DynamicScriptServiceCollectionExtensions.InitializeLocalTexts(null!));
    }

    [Fact]
    public void InitializeLocalTexts_Initializes_Registry()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ITypeSource>(new MockTypeSource());
        services.AddSingleton<ILocalTextRegistry>(new MockLocalTextRegistry());
        services.AddSingleton<ILocalTextInitializer, DefaultLocalTextInitializer>();
        var provider = services.BuildServiceProvider();
        var app = new ApplicationBuilder(provider);

        app.InitializeLocalTexts();
    }
}
