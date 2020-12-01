using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serenity.Abstractions;
using Serenity.Extensions.DependencyInjection;
using Serenity.PropertyGrid;
using Serenity.Web.Middleware;
using System;

namespace Serenity.Web
{
    public static class DynamicScriptServiceCollectionExtensions
    {
        public static void AddDynamicScripts(this IServiceCollection collection)
        {
            collection.AddCaching();
            collection.AddTextRegistry();
            collection.TryAddSingleton<IDynamicScriptManager, DynamicScriptManager>();
            collection.TryAddSingleton<IPropertyItemProvider, DefaultPropertyItemProvider>();
        }

        public static void AddCssBundling(this IServiceCollection collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            collection.AddOptions();
            collection.TryAddSingleton<IContentHashCache, ContentHashCache>();
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
            collection.TryAddSingleton<IContentHashCache, ContentHashCache>();
            collection.Configure(setupAction);
        }

        public static void AddScriptBundling(this IServiceCollection collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            collection.AddOptions();
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

        public static IApplicationBuilder UseDynamicScripts(this IApplicationBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            var serviceProvider = builder.ApplicationServices;
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

            var hostEnvironment = builder.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
            new TemplateScriptRegistrar()
                .Initialize(scriptManager, new[] 
                {
                    System.IO.Path.Combine(hostEnvironment.ContentRootPath, "Views", "Templates"),
                    System.IO.Path.Combine(hostEnvironment.ContentRootPath, "Modules")
                }, watchForChanges: true);

            var scriptsPath = System.IO.Path.Combine(hostEnvironment.WebRootPath, "Scripts");
            var scriptWatcher = new FileWatcher(scriptsPath, "*.js");
            scriptWatcher.Changed += (name) =>
            {
                builder.ApplicationServices.GetService<IScriptBundleManager>()?.ScriptsChanged();
                builder.ApplicationServices.GetService<IContentHashCache>()?.ScriptsChanged();
            };

            var contentPath = System.IO.Path.Combine(hostEnvironment.WebRootPath, "Content");
            var cssWatcher = new FileWatcher(contentPath, "*.css");
            scriptWatcher.Changed += (name) =>
            {
                builder.ApplicationServices.GetService<ICssBundleManager>()?.CssChanged();
                builder.ApplicationServices.GetService<IContentHashCache>()?.ScriptsChanged();
            };

            return builder.UseMiddleware<DynamicScriptMiddleware>();
        }
    }
}
