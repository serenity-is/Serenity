using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serenity.Abstractions;
using Serenity.Caching;
using Serenity.Localization;
using Serenity.Reflection;
using System.Reflection;

namespace Serenity.Extensions.DependencyInjection
{
    /// <summary>
    /// Contains extensions to register core services
    /// </summary>
    public static class CoreServiceCollectionExtensions
    {
        /// <summary>
        /// Adds memory caching as ILocalCache implementation and a distributed cache
        /// emulator as IDistributed cache implementation.
        /// </summary>
        /// <param name="services">The services.</param>
        public static void AddCaching(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.TryAddSingleton<IDistributedCache, DistributedCacheEmulator>();
            services.TryAddSingleton<ITwoLevelCache, TwoLevelCache>();
        }

        /// <summary>
        /// Adds the local text registry.
        /// </summary>
        /// <param name="services">The services.</param>
        public static void AddTextRegistry(this IServiceCollection services)
        {
            services.TryAddSingleton<ILocalTextRegistry, LocalTextRegistry>();
            services.TryAddSingleton<ITextLocalizer, DefaultTextLocalizer>();
        }

        /// <summary>
        /// Adds the annotation type registry.
        /// </summary>
        /// <param name="services">The services.</param>
        public static void AddAnnotationTypes(this IServiceCollection services)
        {
            services.TryAddSingleton<IAnnotationTypeRegistry, AnnotationTypeRegistry>();
        }

        /// <summary>
        /// Adds a type source to the registry.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="assemblies">List of assembles</param>
        public static void AddTypeSource(this IServiceCollection services, Assembly[] assemblies)
        {
            services.TryAddSingleton<ITypeSource>(new DefaultTypeSource(assemblies));
        }
    }
}