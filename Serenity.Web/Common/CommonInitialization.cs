using Serenity.Abstractions;
using Serenity.Caching;
using Serenity.Extensibility;
using Serenity.Localization;
using System.Linq;
using System.Reflection;
using System.Web.Compilation;

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

            ExtensibilityHelper.SelfAssemblies = BuildManager.GetReferencedAssemblies()
                .Cast<Assembly>()
                .Where(x => x.GetReferencedAssemblies().Any(a => a.Name.Contains("Serenity")))
                .ToArray();

            var registrar = Dependency.Resolve<IDependencyRegistrar>();

            if (Dependency.TryResolve<IConfigurationRepository>() == null)
                registrar.RegisterInstance<IConfigurationRepository>("Application", new ApplicationConfigurationRepository());

            if (Dependency.TryResolve<ICache>() == null)
                registrar.RegisterInstance<ICache>(new HttpRuntimeCache());

            if (Dependency.TryResolve<IDistributedCache>() == null)
                registrar.RegisterInstance<IDistributedCache>(new DistributedCacheEmulator());

            if (Dependency.TryResolve<ILocalTextProvider>() == null)
                registrar.RegisterInstance<ILocalTextProvider>(new LocalTextRegistry());

            NestedLocalTextRegistration.Initialize();
            EnumLocalTexts.Initialize(ExtensibilityHelper.SelfAssemblies);
            EntityLocalTexts.Initialize();

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