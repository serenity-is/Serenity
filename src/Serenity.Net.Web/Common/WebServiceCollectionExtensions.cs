using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serenity.Abstractions;
using Serenity.Caching;
using Serenity.Data;
using Serenity.Localization;
using System.Collections.Generic;
using System.Reflection;

namespace Serenity.Extensions.DependencyInjection
{
    public static class WebServiceCollectionExtensions
    {
        public static void AddConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.TryAddSingleton(configuration);
        }

        public static void AddCaching(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.TryAddSingleton<IDistributedCache, DistributedCacheEmulator>();
        }

        public static void AddTextRegistry(this IServiceCollection services)
        {
            services.TryAddSingleton<ILocalTextRegistry, LocalTextRegistry>();
            services.TryAddSingleton<ITextLocalizer, DefaultTextLocalizer>();
        }

        public static void AddNestedTexts(this ILocalTextRegistry registry, IEnumerable<Assembly> assemblies)
        {
            NestedLocalTextRegistration.AddNestedTexts(registry, assemblies);
        }

        public static void AddEnumTexts(this ILocalTextRegistry registry, IEnumerable<Assembly> assemblies,
            string languageID = LocalText.InvariantLanguageID)
        {
            EnumLocalTextRegistration.AddEnumTexts(registry, assemblies, languageID);
        }

        public static void AddRowTexts(this ILocalTextRegistry registry, IEnumerable<Assembly> assemblies, 
            string languageID = LocalText.InvariantLanguageID)
        {
            EntityLocalTexts.AddRowTexts(registry, RowRegistry.EnumerateRowInstances(assemblies), languageID);
        }

        public static void AddJsonTexts(this ILocalTextRegistry registry, string path)
        {
            JsonLocalTextRegistration.AddJsonTexts(registry, path);
        }
    }
}