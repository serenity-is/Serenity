using Serenity.Abstractions;
using Serenity.Caching;
using Serenity.Configuration;
using Serenity.Extensibility;
using Serenity.Localization;
using Serenity.Logging;
using Serenity.Services;
using System.Reflection;
using System.Web.Hosting;
#if ASPNETCORE
using Microsoft.Extensions.Caching.Memory;
#else
using System.Linq;
using System.Web.Compilation;
#endif

namespace Serenity.Web
{
    public static class CommonInitialization
    {
        public static void Run()
        {
#if !COREFX
            InitializeServiceLocator();
#endif
            InitializeSelfAssemblies();
            InitializeCaching();
            InitializeConfigurationSystem();
            InitializeLogging();
            InitializeLocalTexts();
            InitializeDynamicScripts();
            InitializeRequestContext();
            InitializeRequestBehaviors();
        }

#if !COREFX
        public static void InitializeServiceLocator()
        {
            if (!Dependency.HasResolver)
            {
                var container = new MunqContainer();
                Dependency.SetResolver(container);
            }
        }
#endif

        public static void InitializeLogging()
        {
            var registrar = Dependency.Resolve<IDependencyRegistrar>();

            if (Dependency.TryResolve<ILogger>() == null)
                registrar.RegisterInstance<ILogger>(new FileLogger());
        }

        public static void InitializeSelfAssemblies()
        {
#if !ASPNETCORE
            var selfAssemblies = BuildManager.GetReferencedAssemblies()
                .Cast<Assembly>()
                .Where(x =>
                    x.FullName.Contains("Serenity") ||
                    x.GetReferencedAssemblies().Any(a => a.Name.Contains("Serenity")));

            ExtensibilityHelper.SelfAssemblies = Reflection.AssemblySorter.Sort(selfAssemblies).ToArray();
#endif
        }

        public static void InitializeCaching()
        {
            var registrar = Dependency.Resolve<IDependencyRegistrar>();

            if (Dependency.TryResolve<ILocalCache>() == null)
#if ASPNETCORE
                registrar.RegisterInstance<ILocalCache>(new Serenity.Caching.MemoryCache(Dependency.Resolve<IMemoryCache>()));
#else
                registrar.RegisterInstance<ILocalCache>(new HttpRuntimeCache());
#endif
            if (Dependency.TryResolve<IDistributedCache>() == null)
                registrar.RegisterInstance<IDistributedCache>(new DistributedCacheEmulator());
        }

        public static void InitializeRequestContext()
        {
            var registrar = Dependency.Resolve<IDependencyRegistrar>();

            if (Dependency.TryResolve<IRequestContext>() == null)
                registrar.RegisterInstance<IRequestContext>(new RequestContext());
        }

        public static void InitializeRequestBehaviors()
        {
            var registrar = Dependency.Resolve<IDependencyRegistrar>();

            if (Dependency.TryResolve<IImplicitBehaviorRegistry>() == null)
                registrar.RegisterInstance<IImplicitBehaviorRegistry>(DefaultImplicitBehaviorRegistry.Instance);
        }

        public static void InitializeConfigurationSystem()
        {
            var registrar = Dependency.Resolve<IDependencyRegistrar>();

            if (Dependency.TryResolve<IConfigurationRepository>() == null)
#if COREFX
                registrar.RegisterInstance<IConfigurationRepository>(new AppSettingsJsonConfigRepository());
#else
                registrar.RegisterInstance<IConfigurationRepository>("Application", new AppSettingsJsonConfigRepository());
#endif
        }

        public static void InitializeLocalTexts()
        {
            var registrar = Dependency.Resolve<IDependencyRegistrar>();

            if (Dependency.TryResolve<ILocalTextRegistry>() == null)
                registrar.RegisterInstance<ILocalTextRegistry>(new LocalTextRegistry());

            NestedLocalTextRegistration.Initialize(ExtensibilityHelper.SelfAssemblies);
            EnumLocalTextRegistration.Initialize(ExtensibilityHelper.SelfAssemblies);
            EntityLocalTexts.Initialize();
            JsonLocalTextRegistration.AddFromFilesInFolder(HostingEnvironment.MapPath("~/Scripts/serenity/texts/"));
            JsonLocalTextRegistration.AddFromFilesInFolder(HostingEnvironment.MapPath("~/Scripts/site/texts/"));
            JsonLocalTextRegistration.AddFromFilesInFolder(HostingEnvironment.MapPath("~/App_Data/texts/"));
        }

        public static void InitializeDynamicScripts()
        {
            DynamicScriptRegistration.Initialize(ExtensibilityHelper.SelfAssemblies);
            LookupScriptRegistration.RegisterLookupScripts();
            RunStartupRegistrars<ScriptRegistrarAttribute>();
            FormScriptRegistration.RegisterFormScripts();
            ColumnsScriptRegistration.RegisterColumnsScripts();

            new TemplateScriptRegistrar()
                .Initialize(new[] { "~/Views/Templates", "~/Modules" }, watchForChanges: true);

            ScriptFileWatcher.WatchForChanges();
            CssFileWatcher.WatchForChanges();
        }

        private static void RunStartupRegistrars<TAttribute>() 
            where TAttribute : BaseRegistrarAttribute
        {
            foreach (var assembly in ExtensibilityHelper.SelfAssemblies)
                foreach (TAttribute attr in assembly.GetCustomAttributes(typeof(TAttribute)))
                    Serenity.Extensibility.ExtensibilityHelper.RunClassConstructor(attr.Type);
        }
    }
}