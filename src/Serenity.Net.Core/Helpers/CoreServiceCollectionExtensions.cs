using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serenity.Localization;
using Serenity.Reflection;

namespace Serenity.Extensions.DependencyInjection;

/// <summary>
/// Contains extensions to register core services
/// </summary>
public static class CoreServiceCollectionExtensions
{
    /// <summary>
    /// Adds memory caching as ILocalCache implementation and a distributed cache
    /// emulator as IDistributed cache implementation.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static IServiceCollection AddCaching(this IServiceCollection services)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));

        services.AddMemoryCache();
        services.AddDistributedMemoryCache();
        services.TryAddSingleton<ITwoLevelCache, TwoLevelCache>();
        return services;
    }

    /// <summary>
    /// Adds the local text registry.
    /// </summary>
    /// <param name="services">The services.</param>
    public static void AddTextRegistry(this IServiceCollection services)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));

        services.TryAddSingleton<ILocalTextRegistry, LocalTextRegistry>();
        services.TryAddSingleton<ITextLocalizer, DefaultTextLocalizer>();
    }

    /// <summary>
    /// Adds the annotation type registry.
    /// </summary>
    /// <param name="services">The services.</param>
    public static void AddAnnotationTypes(this IServiceCollection services)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));

        services.TryAddSingleton<IAnnotationTypeRegistry, AnnotationTypeRegistry>();
    }

    /// <summary>
    /// Adds a type source to the registry.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="assemblies">List of assembles</param>
    public static void AddTypeSource(this IServiceCollection services, Assembly[] assemblies)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));

        services.TryAddSingleton<ITypeSource>(new DefaultTypeSource(assemblies));
    }

    /// <summary>
    /// Adds on demand service resolver to the registry.
    /// </summary>
    /// <param name="services">The services.</param>
    public static void AddServiceResolver(this IServiceCollection services)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));

        services.TryAddSingleton(typeof(IServiceResolver<>), typeof(ServiceResolver<>));
    }
}