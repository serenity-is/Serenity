#if !NET45

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serenity.Abstractions;
using Serenity.Caching;
using Serenity.Localization;
using Serenity.Logging;
using Serenity.Reflection;

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
            services.TryAddSingleton<ILocalCache, MemoryLocalCache>();
            services.TryAddSingleton<IDistributedCache, DistributedCacheEmulator>();
        }

        /// <summary>
        /// Adds the simple file logging.
        /// </summary>
        /// <param name="services">The services.</param>
        public static void AddFileLogging(this IServiceCollection services)
        {
            services.TryAddSingleton<ILogger, FileLogger>();
        }

        /// <summary>
        /// Adds the local text registry.
        /// </summary>
        /// <param name="services">The services.</param>
        public static void AddTextRegistry(this IServiceCollection services)
        {
            services.TryAddSingleton<ILocalTextRegistry, LocalTextRegistry>();
        }

        /// <summary>
        /// Adds the annotation type registry.
        /// </summary>
        /// <param name="services">The services.</param>
        public static void AddAnnotationTypes(this IServiceCollection services)
        {
            services.TryAddSingleton<IAnnotationTypeRegistry, AnnotationTypeRegistry>();
        }
    }
}
#endif