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

    /// <summary>
    /// Adds the <see cref="DefaultUserProvider"/> as <see cref="IUserProvider"/> implementation to the service collection.
    /// Note that it requires IUserRetrieveService, IUserAcessor to be
    /// registered in the service collection. It also tries to register the DefaultUserClaimCreator.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static IServiceCollection AddUserProvider(this IServiceCollection services)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));

        services.TryAddSingleton<IUserClaimCreator, DefaultUserClaimCreator>();
        services.TryAddSingleton<IUserProvider, DefaultUserProvider>();
        return services;
    }

    /// <summary>
    /// Adds the <see cref="DefaultUserProvider"/> as <see cref="IUserProvider"/> implementation to the service collection.
    /// Also registers the given IUserAccessor and IUserRetrieveService implementations and
    /// tries to register the DefaultUserClaimCreator implementation.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static IServiceCollection AddUserProvider<TUserAccessor, TUserRetrieveService>(this IServiceCollection services)
        where TUserAccessor: class, IUserAccessor
        where TUserRetrieveService : class, IUserRetrieveService
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));

        services.AddSingleton<IUserAccessor, TUserAccessor>();
        services.AddSingleton<IUserRetrieveService, TUserRetrieveService>();
        services.AddUserProvider();
        return services;
    }

    /// <summary>
    /// Adds the <see cref="DefaultUserProvider"/> as <see cref="IUserProvider"/> implementation to the service collection.
    /// Also registers the given IUserAccessor, IUserRetrieveService and IUserClaimCreator implementations.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static IServiceCollection AddUserProvider<TUserAccessor, TUserRetrieveService, TUserClaimCreator>(this IServiceCollection services)
        where TUserAccessor : class, IUserAccessor
        where TUserRetrieveService : class, IUserRetrieveService
        where TUserClaimCreator : class, IUserClaimCreator
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));

        services.AddSingleton<IUserClaimCreator, TUserClaimCreator>();
        return AddUserProvider<TUserAccessor, TUserRetrieveService>(services);
    }

    /// <summary>
    /// Adds a singleton service of the type <typeparamref name="TService"/> with the
    /// implementation type <typeparamref name="TWrapper"/> that wraps the <typeparamref name="TImplementation"/>
    /// to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <typeparam name="TWrapper">The type of the wrapper to use.</typeparam>
    /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <seealso cref="ServiceLifetime.Singleton"/>
    public static IServiceCollection AddSingletonWrapped<TService, TWrapper, TImplementation>(
        this IServiceCollection services)
        where TService : class
        where TWrapper : TService
        where TImplementation: TService
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));

        return services.AddSingleton<TService>(services =>
            ActivatorUtilities.CreateInstance<TWrapper>(services,
                ActivatorUtilities.CreateInstance<TImplementation>(services)));
    }

    /// <summary>
    /// Adds a singleton service of the type <typeparamref name="TService"/> with the
    /// implementation type <typeparamref name="TWrapper1"/> that wraps the <typeparamref name="TWrapper2"/>
    /// which itself wraps the <typeparamref name="TImplementation"/> to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <typeparam name="TWrapper1">The type of the wrapper 1 to use.</typeparam>
    /// <typeparam name="TWrapper2">The type of the wrapper 2 to use.</typeparam>
    /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <seealso cref="ServiceLifetime.Singleton"/>
    public static IServiceCollection AddSingletonWrapped<TService, TWrapper1, TWrapper2, TImplementation>(
        this IServiceCollection services)
        where TService : class
        where TWrapper1 : TService
        where TWrapper2 : TService
        where TImplementation : TService
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));

        return services.AddSingleton<TService>(services =>
            ActivatorUtilities.CreateInstance<TWrapper1>(services,
                ActivatorUtilities.CreateInstance<TWrapper2>(services,
                    ActivatorUtilities.CreateInstance<TImplementation>(services))));
    }

}