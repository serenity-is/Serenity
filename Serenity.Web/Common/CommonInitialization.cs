using Serenity.Abstractions;
using Serenity.Caching;
using Serenity.Configuration;
using Serenity.Extensibility;
using Serenity.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Compilation;
using System.Web.Hosting;

namespace Serenity.Web
{
    public static class CommonInitialization
    {
        public static void Run()
        {
            if (!Dependency.HasResolver)
            {
                var container = new MunqContainer();
                Dependency.SetResolver(container);
            }

            var selfAssemblies = BuildManager.GetReferencedAssemblies()
                .Cast<Assembly>()
                .Where(x => 
                    x.FullName.Contains("Serenity") ||
                    x.GetReferencedAssemblies().Any(a => a.Name.Contains("Serenity")));

            ExtensibilityHelper.SelfAssemblies = AssemblySorter.Sort(selfAssemblies).ToArray();

            var registrar = Dependency.Resolve<IDependencyRegistrar>();

            if (Dependency.TryResolve<IConfigurationRepository>() == null)
                registrar.RegisterInstance<IConfigurationRepository>("Application", new ApplicationConfigurationRepository());

            if (Dependency.TryResolve<ICache>() == null)
                registrar.RegisterInstance<ICache>(new HttpRuntimeCache());

            if (Dependency.TryResolve<IDistributedCache>() == null)
                registrar.RegisterInstance<IDistributedCache>(new DistributedCacheEmulator());

            if (Dependency.TryResolve<ILocalTextRegistry>() == null)
                registrar.RegisterInstance<ILocalTextRegistry>(new LocalTextRegistry());

            NestedLocalTextRegistration.Initialize();
            EnumLocalTexts.Initialize(ExtensibilityHelper.SelfAssemblies);
            EntityLocalTexts.Initialize();
            NestedLocalTextRegistration.AddFromScripts(HostingEnvironment.MapPath("~/Scripts/serenity/texts/"));
            NestedLocalTextRegistration.AddFromScripts(HostingEnvironment.MapPath("~/Scripts/site/texts/"));

            RunStartupRegistrars<EnumRegistrarAttribute>();
            RunStartupRegistrars<ScriptRegistrarAttribute>();
            FormScriptRegistration.RegisterFormScripts();

            var templateRegistrar = new TemplateScriptRegistrar();
            templateRegistrar.Initialize("~/Views/Templates", watchForChanges: true);
            templateRegistrar.Initialize("~/Modules", watchForChanges: true);

            ScriptFileWatcher.WatchForChanges();
            CssFileWatcher.WatchForChanges();
        }

        private static void RunStartupRegistrars<TAttribute>() 
            where TAttribute : BaseRegistrarAttribute
        {
            foreach (var assembly in ExtensibilityHelper.SelfAssemblies)
                foreach (TAttribute attr in assembly.GetCustomAttributes(typeof(TAttribute), false))
                    Serenity.Extensibility.ExtensibilityHelper.RunClassConstructor(attr.Type);
        }
    }
}