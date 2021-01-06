using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serenity.Abstractions;
using Serenity.PropertyGrid;
using Serenity.Web;
using Serenity.Web.Middleware;
using System;
using System.Linq;

namespace Serenity.Extensions.DependencyInjection
{
    public static class DynamicScriptServiceCollectionExtensions
    {
        public static IServiceCollection AddDynamicScriptManager(this IServiceCollection collection)
        {
            collection.AddCaching();
            collection.AddTextRegistry();
            collection.TryAddSingleton<IDynamicScriptManager, DynamicScriptManager>();
            return collection;
        }

        public static IServiceCollection AddDynamicScripts(this IServiceCollection collection)
        {
            AddDynamicScriptManager(collection);
            collection.TryAddSingleton<IPropertyItemProvider, DefaultPropertyItemProvider>();
            return collection;
        }

        public static IServiceCollection AddFileWatcherFactory(this IServiceCollection collection)
        {
            collection.TryAddSingleton<IFileWatcherFactory, DefaultFileWatcherFactory>();
            return collection;
        }

        public static IServiceCollection AddContentHashCache(this IServiceCollection collection)
        {
            AddFileWatcherFactory(collection);
            collection.AddOptions();
            collection.TryAddSingleton<IContentHashCache, ContentHashCache>();
            return collection;
        }

        public static void AddCssBundling(this IServiceCollection collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            collection.AddDynamicScriptManager();
            collection.AddContentHashCache();
            collection.TryAddSingleton<ICssBundleManager, CssBundleManager>();
        }

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

        public static void AddScriptBundling(this IServiceCollection collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            collection.AddDynamicScriptManager();
            collection.AddContentHashCache();
            collection.TryAddSingleton<IScriptBundleManager, ScriptBundleManager>();
        }

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

        public static IApplicationBuilder UseDynamicScriptMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DynamicScriptMiddleware>();
        }

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

            return serviceProvider;
        }

        public static IServiceProvider UseCssWatching(this IServiceProvider serviceProvider)
        {
            var hostEnvironment = serviceProvider.GetRequiredService<IWebHostEnvironment>();
            var CssPaths = new[] { System.IO.Path.Combine(hostEnvironment.WebRootPath, "Content") };
            UseCssWatching(serviceProvider, CssPaths);
            return serviceProvider;
        }

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

                var CssWatcher = new FileWatcher(path, "*.css");
                CssWatcher.Changed += (name) =>
                {
                    bundleManager?.CssChanged();
                    contentHashCache?.ScriptsChanged();
                };

                fileWatcherFactory.KeepAlive(CssWatcher);
            }

            return serviceProvider;
        }

        public static IServiceProvider UseScriptWatching(this IServiceProvider serviceProvider)
        {
            var hostEnvironment = serviceProvider.GetRequiredService<IWebHostEnvironment>();
            var scriptPaths = new[] { System.IO.Path.Combine(hostEnvironment.WebRootPath, "Scripts") };
            UseScriptWatching(serviceProvider, scriptPaths);
            return serviceProvider;
        }

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

                var scriptWatcher = new FileWatcher(path, "*.js");
                scriptWatcher.Changed += (name) =>
                {
                    bundleManager?.ScriptsChanged();
                    contentHashCache?.ScriptsChanged();
                };

                fileWatcherFactory.KeepAlive(scriptWatcher);
            }

            return serviceProvider;
        }

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
}
