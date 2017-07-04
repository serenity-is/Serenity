#if ASPNETCORE

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serenity.Abstractions;
using Serenity.Caching;
using Serenity.Configuration;

namespace Serenity.Extensions.DependencyInjection
{
    public static class WebServiceCollectionExtensions
    {
        public static void AddConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.TryAddSingleton<IConfiguration>(configuration);
            services.TryAddSingleton<IConfigurationRepository, AppSettingsJsonConfigRepository>();
            services.TryAddSingleton<IConfigurationManager, WebConfigurationWrapper>();
        }

#if !COREFX
        public static void AddCaching(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.TryAddSingleton<ILocalCache, MemoryLocalCache>();
            services.TryAddSingleton<IDistributedCache, DistributedCacheEmulator>();
        }

        public static void AddFileLogging(this IServiceCollection services)
        {
            services.TryAddSingleton<ILogger, Serenity.Logging.FileLogger>();
        }

        public static void AddTextRegistry(this IServiceCollection services)
        {
            services.TryAddSingleton<ILocalTextRegistry, Serenity.Localization.LocalTextRegistry>();
        }

        public static void AddNestedTexts(this ILocalTextRegistry registry, System.Collections.Generic.IEnumerable<System.Reflection.Assembly> assemblies = null)
        {
            Serenity.Localization.NestedLocalTextRegistration.Initialize(assemblies, registry);
        }

        public static void AddEnumTexts(this ILocalTextRegistry registry, System.Collections.Generic.IEnumerable<System.Reflection.Assembly> assemblies = null,
            string languageID = LocalText.InvariantLanguageID)
        {
            Serenity.Localization.EnumLocalTextRegistration.Initialize(assemblies, languageID);
        }

        public static void AddRowTexts(this ILocalTextRegistry registry,
            string languageID = LocalText.InvariantLanguageID)
        {
            Serenity.Localization.EntityLocalTexts.Initialize(languageID, registry);
        }

        public static void AddJsonTexts(this ILocalTextRegistry registry, string path)
        {
            Serenity.Localization.JsonLocalTextRegistration.AddFromFilesInFolder(path, registry);
        }
#endif
    }
}
#endif