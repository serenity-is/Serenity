#if NET45
using Serenity.Abstractions;
using Serenity.Caching;
using Serenity.Configuration;
using Serenity.Extensibility;
using Serenity.Localization;
using Serenity.Logging;
using Serenity.Services;
using System.Reflection;
using System.Web.Hosting;
using Serenity.Reflection;
#if !ASPNETMVC
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
#if NET45
            InitializeServiceLocator();
#endif
            InitializeSelfAssemblies();
            InitializeAnnotatedTypes();
            InitializeCaching();
            InitializeConfigurationSystem();
            InitializeLogging();
            InitializeLocalTexts();
            InitializeDynamicScripts();
            InitializeRequestContext();
            InitializeRequestBehaviors();
        }

#if NET45
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
#if ASPNETMVC
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
#if !ASPNETMVC
                registrar.RegisterInstance<ILocalCache>(new Serenity.Caching.MemoryLocalCache(Dependency.Resolve<IMemoryCache>()));
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

        public static void InitializeAnnotatedTypes()
        {
            var registrar = Dependency.Resolve<IDependencyRegistrar>();

            if (Dependency.TryResolve<IAnnotationTypeRegistry>() == null)
                registrar.RegisterInstance<IAnnotationTypeRegistry>(new AnnotationTypeRegistry());
        }

        public static void InitializeConfigurationSystem()
        {
            var registrar = Dependency.Resolve<IDependencyRegistrar>();

            if (Dependency.TryResolve<IConfigurationManager>() == null)
                registrar.RegisterInstance<IConfigurationManager>(new WebConfigurationWrapper());

            if (Dependency.TryResolve<IConfigurationRepository>() == null)
#if !NET45
                registrar.RegisterInstance<IConfigurationRepository>(new AppSettingsJsonConfigRepository());
#else
                registrar.RegisterInstance<IConfigurationRepository>("Application", new AppSettingsJsonConfigRepository());
#endif
        }

        public static void InitializeLocalTexts()
        {
            var registrar = Dependency.Resolve<IDependencyRegistrar>();

            var registry = Dependency.TryResolve<ILocalTextRegistry>();
            if (registry == null)
            {
                registry = new LocalTextRegistry();
                registrar.RegisterInstance<ILocalTextRegistry>(registry);
            }

            NestedLocalTextRegistration.Initialize(ExtensibilityHelper.SelfAssemblies);
            NestedPermissionKeyRegistration.AddNestedPermissions(registry, ExtensibilityHelper.SelfAssemblies);
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
            DistinctValuesRegistration.RegisterDistinctValueScripts();
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
#endif

#if !ASPNETMVC
#if NET45
namespace Serenity.Caching
{
    using Serenity.Abstractions;
    using System;
    using Microsoft.Extensions.Caching.Memory;

    public class MemoryLocalCache : ILocalCache
    {
        private IMemoryCache cache;

        public MemoryLocalCache(IMemoryCache cache)
        {
            this.cache = cache;
        }

        public void Add(string key, object value, TimeSpan expiration)
        {
            if (expiration == TimeSpan.Zero)
                cache.Set(key, value);
            else if (expiration > TimeSpan.Zero)
                cache.Set(key, value, expiration);
        }

        public TItem Get<TItem>(string key)
        {
            return cache.Get<TItem>(key);
        }

        public object Remove(string key)
        {
            cache.Remove(key);
            return null;
        }

        public void RemoveAll()
        {
            throw new NotImplementedException();
        }
    }
}
#endif
#endif