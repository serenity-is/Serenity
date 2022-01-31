using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serenity.Abstractions;
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
        /// <param name="collection">The service collection.</param>
        public static IServiceCollection AddCaching(this IServiceCollection collection)
        {
            collection.AddMemoryCache();
            collection.AddDistributedMemoryCache();
            collection.TryAddSingleton<ITwoLevelCache, TwoLevelCache>();
            return collection;
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

        /// <summary>
        /// Adds on demand service resolver to the registry.
        /// </summary>
        /// <param name="services">The services.</param>
        public static void AddServiceResolver(this IServiceCollection services)
        {
            services.TryAddSingleton(typeof(IServiceResolver<>), typeof(ServiceResolver<>));
        }
    }
}