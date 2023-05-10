using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serenity.PropertyGrid;
using Serenity.Web;
using Serenity.Web.Middleware;

namespace Serenity.Extensions.DependencyInjection;

/// <summary>
/// Contains DI extension methods related to dynamic script services
/// </summary>
public static class DynamicScriptServiceCollectionExtensions
{
    /// <summary>
    /// Registers he default <see cref="IDynamicScriptManager"/> implementation.
    /// </summary>
    /// <param name="collection">Service collection</param>
    /// <returns></returns>
    public static IServiceCollection AddDynamicScriptManager(this IServiceCollection collection)
    {
        if (collection == null)
            throw new ArgumentNullException(nameof(collection));

        collection.AddCaching();
        collection.AddTextRegistry();
        collection.TryAddSingleton<IDynamicScriptManager, DynamicScriptManager>();
        return collection;
    }

    /// <summary>
    /// Registers the default <see cref="IDynamicScriptManager" /> implementation
    /// in addition to the <see cref="IPropertyItemProvider"/> implementation.
    /// </summary>
    /// <param name="collection">Service collection</param>
    /// <returns></returns>
    public static IServiceCollection AddDynamicScripts(this IServiceCollection collection)
    {
        AddDynamicScriptManager(collection);
        collection.TryAddSingleton<IPropertyItemProvider, DefaultPropertyItemProvider>();
        return collection;
    }

    /// <summary>
    /// Registers the default <see cref="IFileWatcherFactory"/> implementation.
    /// </summary>
    /// <param name="collection">Service collection</param>
    /// <returns></returns>
    public static IServiceCollection AddFileWatcherFactory(this IServiceCollection collection)
    {
        if (collection == null)
            throw new ArgumentNullException(nameof(collection));

        collection.TryAddSingleton<IFileWatcherFactory, DefaultFileWatcherFactory>();
        return collection;
    }

    /// <summary>
    /// Registers the default <see cref="IContentHashCache"/> implementation.
    /// </summary>
    /// <param name="collection">Service collection</param>
    public static IServiceCollection AddContentHashCache(this IServiceCollection collection)
    {
        if (collection == null)
            throw new ArgumentNullException(nameof(collection));

        AddFileWatcherFactory(collection);
        collection.AddOptions();
        collection.TryAddSingleton<IContentHashCache, ContentHashCache>();
        return collection;
    }

    /// <summary>
    /// Registers the default service types related to CSS bundling, including
    /// <see cref="ICssBundleManager"/>.
    /// </summary>
    /// <param name="collection">Service collection</param>
    /// <exception cref="ArgumentNullException">Collection is null</exception>
    public static void AddCssBundling(this IServiceCollection collection)
    {
        if (collection == null)
            throw new ArgumentNullException(nameof(collection));

        collection.AddDynamicScriptManager();
        collection.AddContentHashCache();
        collection.TryAddSingleton<ICssBundleManager, CssBundleManager>();
    }

    /// <summary>
    /// Registers the default service types related to CSS bundling, including
    /// <see cref="ICssBundleManager"/>.
    /// </summary>
    /// <param name="collection">Service collection</param>
    /// <param name="setupAction">Action to edit options</param>
    /// <exception cref="ArgumentNullException">Collection is null</exception>
    public static void AddCssBundling(this IServiceCollection collection,
        Action<CssBundlingOptions> setupAction)
    {
        if (collection == null)
            throw new ArgumentNullException(nameof(collection));

        if (setupAction == null)
            throw new ArgumentNullException(nameof(setupAction));

        collection.AddCssBundling();
        collection.Configure(setupAction);
    }

    /// <summary>
    /// Registers the default service types related to Script bundling, including
    /// <see cref="IScriptBundleManager"/>.
    /// </summary>
    /// <param name="collection">Service collection</param>
    /// <exception cref="ArgumentNullException">Collection is null</exception>
    public static void AddScriptBundling(this IServiceCollection collection)
    {
        if (collection == null)
            throw new ArgumentNullException(nameof(collection));

        collection.AddDynamicScriptManager();
        collection.AddContentHashCache();
        collection.TryAddSingleton<IScriptBundleManager, ScriptBundleManager>();
    }

    /// <summary>
    /// Registers the default service types related to Script bundling, including
    /// <see cref="IScriptBundleManager"/>.
    /// </summary>
    /// <param name="collection">Service collection</param>
    /// <param name="setupAction">Action to edit options</param>
    /// <exception cref="ArgumentNullException">Collection is null</exception>
    public static void AddScriptBundling(this IServiceCollection collection, 
        Action<ScriptBundlingOptions> setupAction)
    {
        if (collection == null)
            throw new ArgumentNullException(nameof(collection));

        if (setupAction == null)
            throw new ArgumentNullException(nameof(setupAction));

        collection.AddScriptBundling();
        collection.Configure(setupAction);
    }

    /// <summary>
    /// Adds <see cref="DynamicScriptMiddleware"/> to the application pipeline
    /// </summary>
    /// <param name="builder">Application builder</param>
    public static IApplicationBuilder UseDynamicScriptMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<DynamicScriptMiddleware>();
    }

    /// <summary>
    /// Adds dynamic script related services to the application including
    /// dynamic script types, css watching, script watching, template scripts, and
    /// dynamic script middleware
    /// </summary>
    /// <param name="builder">Application builder</param>
    public static IApplicationBuilder UseDynamicScripts(this IApplicationBuilder builder)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        UseDynamicScriptTypes(builder.ApplicationServices);
        UseCssWatching(builder.ApplicationServices);
        UseScriptWatching(builder.ApplicationServices);
        UseTemplateScripts(builder.ApplicationServices);

        return UseDynamicScriptMiddleware(builder);
    }

    /// <summary>
    /// Executes registration of dynamic script types including data scripts,
    /// lookup scripts, distinct values, columns and forms.
    /// </summary>
    /// <param name="serviceProvider">Service provider</param>
    /// <returns></returns>
    public static IServiceProvider UseDynamicScriptTypes(this IServiceProvider serviceProvider)
    {
        var scriptManager = serviceProvider.GetRequiredService<IDynamicScriptManager>();
        var propertyProvider = serviceProvider.GetRequiredService<IPropertyItemProvider>();
        var typeSource = serviceProvider.GetRequiredService<ITypeSource>();

        DataScriptRegistration.RegisterDataScripts(scriptManager,
            typeSource, serviceProvider);

        LookupScriptRegistration.RegisterLookupScripts(scriptManager,
            typeSource, serviceProvider);

        DistinctValuesRegistration.RegisterDistinctValueScripts(scriptManager,
            typeSource, serviceProvider);

        ColumnsScriptRegistration.RegisterColumnsScripts(scriptManager,
            typeSource, propertyProvider, serviceProvider);

        FormScriptRegistration.RegisterFormScripts(scriptManager,
            typeSource, propertyProvider, serviceProvider);

        scriptManager.Register("ColumnAndFormBundle", new ConcatenatedScript(
            new Func<string>[]
            {
                () => scriptManager.GetScriptText("ColumnsBundle"),
                () => scriptManager.GetScriptText("FormBundle")
            }));

        return serviceProvider;
    }

    /// <summary>
    /// Actives CSS file watching
    /// </summary>
    /// <param name="serviceProvider">Service provider</param>
    public static IServiceProvider UseCssWatching(this IServiceProvider serviceProvider)
    {
        var hostEnvironment = serviceProvider.GetRequiredService<IWebHostEnvironment>();
        var CssPaths = new[] { System.IO.Path.Combine(hostEnvironment.WebRootPath, "Content") };
        UseCssWatching(serviceProvider, CssPaths);
        return serviceProvider;
    }

    /// <summary>
    /// Activates CSS file watching
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="cssPaths">CSS paths</param>
    /// <exception cref="ArgumentNullException">serviceProvider is null</exception>
    /// <exception cref="InvalidOperationException">CSS bundle manager is not registered</exception>
    public static IServiceProvider UseCssWatching(this IServiceProvider serviceProvider,
        params string[] cssPaths)
    {
        if (serviceProvider == null)
            throw new ArgumentNullException(nameof(serviceProvider));

        if (cssPaths == null || cssPaths.Length == 0)
            throw new ArgumentNullException(nameof(cssPaths));

        var bundleManager = serviceProvider.GetService<ICssBundleManager>();
        var contentHashCache = serviceProvider.GetService<IContentHashCache>();

        if (bundleManager == null && contentHashCache == null)
            throw new InvalidOperationException("CSS watching has no use when " +
                "there is no CSS bundle manager or content hash cache!");

        foreach (var path in cssPaths)
        {
            var fileWatcherFactory = serviceProvider.GetRequiredService<IFileWatcherFactory>();
            if (fileWatcherFactory.Watchers.Any(x =>
                    x.Path == path && x.Filter == "*.css") == true)
                continue;

            var CssWatcher = fileWatcherFactory.Create(path, "*.css");
            CssWatcher.Changed += (name) =>
            {
                bundleManager?.CssChanged();
                contentHashCache?.ScriptsChanged();
            };

            fileWatcherFactory.KeepAlive(CssWatcher);
        }

        return serviceProvider;
    }

    /// <summary>
    /// Activates script file watching
    /// </summary>
    /// <param name="serviceProvider">Service provider</param>
    public static IServiceProvider UseScriptWatching(this IServiceProvider serviceProvider)
    {
        var hostEnvironment = serviceProvider.GetRequiredService<IWebHostEnvironment>();
        var scriptPaths = new[] { System.IO.Path.Combine(hostEnvironment.WebRootPath, "Scripts") };
        UseScriptWatching(serviceProvider, scriptPaths);
        return serviceProvider;
    }

    /// <summary>
    /// Activates script file watching
    /// </summary>
    /// <param name="serviceProvider">Service provider</param>
    /// <param name="scriptPaths">List of script paths to watch</param>
    /// <exception cref="ArgumentNullException">serviceProvider or scriptPaths is null</exception>
    /// <exception cref="InvalidOperationException">Script bundle manager is not registered</exception>
    public static IServiceProvider UseScriptWatching(this IServiceProvider serviceProvider,
        params string[] scriptPaths)
    {
        if (serviceProvider == null)
            throw new ArgumentNullException(nameof(serviceProvider));

        if (scriptPaths == null || scriptPaths.Length == 0)
            throw new ArgumentNullException(nameof(scriptPaths));

        var bundleManager = serviceProvider.GetService<IScriptBundleManager>();
        var contentHashCache = serviceProvider.GetService<IContentHashCache>();

        if (bundleManager == null && contentHashCache == null)
            throw new InvalidOperationException("Script watching has no use when " +
                "there is no script bundle manager or content hash cache!");

        foreach (var path in scriptPaths)
        {
            var fileWatcherFactory = serviceProvider.GetRequiredService<IFileWatcherFactory>();
            if (fileWatcherFactory.Watchers.Any(x =>
                    x.Path == path && x.Filter == "*.js") == true)
                continue;

            var scriptWatcher = fileWatcherFactory.Create(path, "*.js");
            scriptWatcher.Changed += (name) =>
            {
                bundleManager?.ScriptsChanged();
                contentHashCache?.ScriptsChanged();
            };

            fileWatcherFactory.KeepAlive(scriptWatcher);
        }

        return serviceProvider;
    }

    /// <summary>
    /// Registers template scripts
    /// </summary>
    /// <param name="serviceProvider">Service provider</param>
    public static IServiceProvider UseTemplateScripts(this IServiceProvider serviceProvider)
    {
        var hostEnvironment = serviceProvider.GetRequiredService<IWebHostEnvironment>();
        var templateRoots = new[]
        {
            System.IO.Path.Combine(hostEnvironment.ContentRootPath, "Views", "Templates"),
            System.IO.Path.Combine(hostEnvironment.ContentRootPath, "Modules")
        };
        UseTemplateScripts(serviceProvider, templateRoots);
        return serviceProvider;
    }

    /// <summary>
    /// Registers template scripts
    /// </summary>
    /// <param name="serviceProvider">Service provider</param>
    /// <param name="templateRoots">Root paths for templates</param>
    /// <exception cref="ArgumentNullException">Service provider or template roots is null</exception>
    public static IServiceProvider UseTemplateScripts(this IServiceProvider serviceProvider, 
        params string[] templateRoots)
    {
        if (serviceProvider == null)
            throw new ArgumentNullException(nameof(serviceProvider));

        if (templateRoots == null || templateRoots.Length == 0)
            throw new ArgumentNullException(nameof(templateRoots));

        var scriptManager = serviceProvider.GetRequiredService<IDynamicScriptManager>();

        var templateWatchers = new TemplateScriptRegistrar()
            .Initialize(scriptManager, templateRoots, watchForChanges: true);

        foreach (var templateWatcher in templateWatchers)
            serviceProvider.GetRequiredService<IFileWatcherFactory>()
                .KeepAlive(templateWatcher);

        return serviceProvider;
    }
}
