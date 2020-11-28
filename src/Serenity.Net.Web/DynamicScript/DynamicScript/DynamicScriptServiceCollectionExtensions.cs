using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serenity.Data;
using Serenity.PropertyGrid;
using Serenity.Web.Middleware;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Serenity.Web
{
    public static class DynamicScriptServiceCollectionExtensions
    {
        public static IApplicationBuilder UseDynamicScripts(this IApplicationBuilder builder, IEnumerable<Assembly> assemblies)
        {
            if (assemblies == null)
                throw new ArgumentNullException(nameof(assemblies));

            var scriptManager = builder.ApplicationServices.GetRequiredService<IDynamicScriptManager>();
            var connections = builder.ApplicationServices.GetRequiredService<IConnectionFactory>();
            var propertyItems = builder.ApplicationServices.GetRequiredService<IPropertyItemProvider>();

            DynamicScriptRegistration.Initialize(scriptManager, connections, assemblies);
            LookupScriptRegistration.RegisterLookupScripts(scriptManager, assemblies);
            DistinctValuesRegistration.RegisterDistinctValueScripts(scriptManager, assemblies);
            FormScriptRegistration.RegisterFormScripts(scriptManager, propertyItems, assemblies);
            ColumnsScriptRegistration.RegisterColumnsScripts(scriptManager, propertyItems, assemblies);

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
