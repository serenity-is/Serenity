using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serenity.Localization;
using Serenity.Reflection;

namespace Serenity.Extensions.DependencyInjection;

/// <summary>
/// Contains extensions to register core services.
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
    /// <param name="assemblies">List of assemblies.</param>
    /// <param name="featureToggles">Feature toggles.</param>
    public static void AddTypeSource(this IServiceCollection services, Assembly[] assemblies, IFeatureToggles? featureToggles = null)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));

        services.TryAddSingleton<ITypeSource>(new DefaultTypeSource(assemblies, featureToggles));
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

    private static T? GetServiceFromCollection<T>(IServiceCollection services)
        where T : class
    {
        return (T?)services.LastOrDefault(d =>
            d.ServiceType == typeof(T))?.ImplementationInstance;
    }

    /// <summary>
    /// Adds the IFeatureToggles service to the registry.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="configuration">Configuration source.</param>
    /// <param name="disableByDefault">Features to disable by default, pass ["*"] to disable
    /// all features by default.</param>
    /// <param name="dependencyMap">Feature dependency map. Features are dictionary
    /// keys and the list of features that they depend on (e.g. all must be enabled)
    /// for that feature to be enabled.</param>
    public static IServiceCollection AddFeatureToggles(this IServiceCollection services, 
        IConfiguration? configuration = null, 
        object[]? disableByDefault = null,
        Dictionary<string, List<RequiresFeatureAttribute>>? dependencyMap = null)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));

        configuration ??= GetServiceFromCollection<IConfiguration>(services);
        if (configuration != null)
        {
            services.TryAddSingleton<IFeatureToggles>(new ConfigurationFeatureToggles(configuration, disableByDefault, dependencyMap));
        }
        else
        {
            services.TryAddSingleton<IFeatureToggles>(s => new ConfigurationFeatureToggles(s.GetRequiredService<IConfiguration>(), disableByDefault, dependencyMap));
        }

        return services;
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

    /// <summary>
    /// Registers all the types with an RegisterService attribute 
    /// (RegisterSingleton, RegisterScoped, RegisterTransient etc.)
    /// from the type source in the service collection if available, 
    /// or using the provided typeSource, optionally filtering implementation types
    /// via a provided predicate. Use this at the end of InitializeServices in Startup.cs
    /// so that services can be overridden before others.
    /// </summary>
    /// <param name="collection">Service collection</param>
    /// <param name="typeSource">Type source. Should be provided if it is not already
    /// registered in the service collection. Pass an empty type source
    /// if you want to disable auto registrations.</param>
    /// <param name="predicate">Predicate to filter implementation types to register.
    /// The first argument is registration type (the interface or the implementation type itself),
    /// and the second argument is the implementation type (e.g. the concrete type), 
    /// and third argument is the registration attribute.</param>
    /// <exception cref="ArgumentNullException">collection is null or typeSource can't be found in the collection</exception>
    public static IServiceCollection AddAutoRegisteredServices(this IServiceCollection? collection,
        ITypeSource? typeSource = null, Func<Type, Type, RegisterServiceAttribute, bool>? predicate = null)
    {
        if (collection is null)
            throw new ArgumentNullException(nameof(collection));

        typeSource ??= collection.FirstOrDefault(x => x.ServiceType == typeof(ITypeSource))?.ImplementationInstance as ITypeSource;
        if (typeSource is null)
            throw new ArgumentNullException(nameof(typeSource));

        var servicesToAdd = new Dictionary<(Type serviceType, string? key), List<(Type implType, RegisterServiceAttribute attr)>>();

        void enqueue(Type serviceType, Type implementationType, RegisterServiceAttribute attr)
        {
            if (predicate != null &&
                !predicate(serviceType, implementationType, attr))
                return;

            var dictKey = (serviceType, attr.Key);
            if (!servicesToAdd.TryGetValue(dictKey, out var list))
                servicesToAdd[dictKey] = list = [];
            
            if (list.Any(x => x.implType == implementationType && x.attr == attr))
                return;
            
            list.Add((implementationType, attr));
        }

        foreach (var impl in typeSource.GetTypes())
        {
            if (impl.IsAbstract || 
                impl.IsInterface || 
                impl.IsGenericType)
                continue;

            var attrs = impl.GetCustomAttributes<RegisterServiceAttribute>(inherit: false);
            if (!attrs.Any())
                continue;

            foreach (var attr in attrs)
            {
                if (attr.AsSelf)
                    enqueue(impl, impl, attr);

                if (attr.Types != null)
                {
                    foreach (var t in attr.Types)
                    {
                        if (!t.IsAssignableFrom(impl))
                            throw new InvalidOperationException($"{impl.FullName} type has [{attr.GetType().Name}] but " +
                                $"the interface type specified ({t.FullName}) is not assignable from the implementation type! " +
                                $"Please check the attribute configuration.");

                        enqueue(t, impl, attr);
                    }

                    continue;
                }

                var intfs = impl.GetInterfaces().Where(x =>
                    x != typeof(IDisposable) &&
                    !typeof(IRequestHandler).IsAssignableFrom(x) &&
                    (string.IsNullOrEmpty(x.Namespace) ||
                     !x.Namespace.StartsWith("System.", StringComparison.Ordinal) &&
                     !x.Namespace.StartsWith("Microsoft.", StringComparison.Ordinal)))
                    .ToArray();

                if (!intfs.Any())
                    throw new InvalidProgramException($"{impl.FullName} type has [{attr.GetType().Name}] but " +
                        $"no suitable interfaces found! Please specify the interface types explicitly on attribute constructor.");

                if (intfs.Length > 1)
                {
                    var nameMatch = intfs.SingleOrDefault(x => x.Name == "I" + impl.Name);
                    if (nameMatch != null)
                    {
                        enqueue(nameMatch, impl, attr);
                        continue;
                    }

                    throw new InvalidProgramException($"{impl.FullName} type has [{attr.GetType().Name}] but " +
                        $"multiple suitable interfaces found and none matches the 'I{{ClassName}}' convention! Please specify the interface types explicitly on attribute constructor.");
                }

                enqueue(intfs[0], impl, attr);
            }
        }

        foreach (var pair in servicesToAdd)
        {
            var serviceType = pair.Key.serviceType;
            var serviceKey = pair.Key.key;

            foreach (var (implType, attr) in pair.Value.OrderBy(x => x.attr.Order))
            {
                var descriptor = new ServiceDescriptor(serviceType,
                    implType != serviceType ? serviceKey : null, implType, attr.Lifetime);

                if (attr.ReplaceExisting)
                {
                    collection.Replace(descriptor);
                }
                else if (attr.SkipExisting)
                {
                    collection.TryAdd(descriptor);
                }
                else
                {
                    collection.Add(descriptor);
                }
            }
        }

        return collection;
    }


}