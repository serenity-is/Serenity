using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serenity.Web;

namespace Serenity.Extensions.DependencyInjection;

/// <summary>
/// DI extension methods related to application part and type source
/// </summary>
public static class ApplicationPartsServiceCollectionExtensions
{
    /// <summary>
    /// Adds IFeatureToggles service to the registry scanning for FeatureKeySetAttribute
    /// in application parts, setting disabled by default for features that has a [DefaultValue(false)]
    /// attribute.
    /// Note that this also calls AddMvcCore() to get the part manager if not provided,
    /// and is not found in the collection.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="configuration">Configuration source</param>
    /// <param name="disableByDefault">Features to disable by default, pass ["*"] to disable
    /// all features by default</param>
    /// <param name="applicationPartManager">Optional application part manager to use</param>
    public static IServiceCollection AddApplicationPartsFeatureToggles(this IServiceCollection services,
        IConfiguration configuration,
        ApplicationPartManager applicationPartManager = null,
        object[] disableByDefault = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        applicationPartManager ??= GetServiceFromCollection<ApplicationPartManager>(services)
            ?? services.AddMvcCore().PartManager;
        var tempSource = new ApplicationPartsTypeSource(applicationPartManager, topologicalSort: false, featureToggles: null);
        var disableSet = new HashSet<string>(disableByDefault?.Select(FeatureTogglesExtensions.ToFeatureKey) ?? []);
        foreach (var keySetType in tempSource.GetTypesWithAttribute(typeof(FeatureKeySetAttribute)))
        {
            foreach (var member in keySetType.GetMembers())
            {
                var attr = member.GetCustomAttribute<DefaultValueAttribute>();
                if (attr != null && attr.Value is bool b && !b)
                    disableSet.Add(member.Name);
            }
        }
        disableByDefault = [.. disableSet];

        return CoreServiceCollectionExtensions.AddFeatureToggles(services, configuration, disableByDefault);
    }

    /// <summary>
    /// Adds an application part type source to the service collection.
    /// Note that this also calls AddMvcCore() to get the part manager if not provided,
    /// and is not found in the collection.
    /// </summary>
    /// <param name="collection">Collection</param>
    /// <param name="partManager">ApplicationPartManager instance.</param>
    /// <param name="featureToggles">Feature toggles</param>
    /// <param name="topologicalSort">Whether to sort assemblies topologically by references</param>
    public static ApplicationPartsTypeSource AddApplicationPartsTypeSource(this IServiceCollection collection,
        ApplicationPartManager partManager = null, IFeatureToggles featureToggles = null, bool topologicalSort = true)
    {
        ArgumentNullException.ThrowIfNull(collection);
        if (GetServiceFromCollection<ITypeSource>(collection) != null)
            ArgumentExceptions.OutOfRange(nameof(collection), "ITypeSource already registered");

        partManager ??= GetServiceFromCollection<ApplicationPartManager>(collection)
            ?? collection.AddMvcCore().PartManager;

        featureToggles ??= GetServiceFromCollection<IFeatureToggles>(collection);

        var typeSource = new ApplicationPartsTypeSource(partManager, topologicalSort, featureToggles);
        collection.AddSingleton<ITypeSource>(typeSource);
        return typeSource;
    }

    private static T GetServiceFromCollection<T>(IServiceCollection services)
        where T: class
    {
        return (T)services.LastOrDefault(d => 
            d.ServiceType == typeof(T))?.ImplementationInstance;
    }
}
