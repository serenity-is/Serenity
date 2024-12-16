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
    /// <param name="applicationPartManager">Optional application part manager to use</param>
    /// <param name="disableByDefault">Features to disable by default, pass ["*"] to disable
    /// all features by default</param>
    /// <param name="dependencyMap">Feature dependency map. Features are dictionary
    /// keys and the list of features that they depend on (e.g. all must be enabled)
    /// for that feature to be enabled.</param>/// 
    public static IServiceCollection AddApplicationPartsFeatureToggles(this IServiceCollection services,
        IConfiguration configuration,
        ApplicationPartManager applicationPartManager = null,
        object[] disableByDefault = null,
        Dictionary<string, List<RequiresFeatureAttribute>> dependencyMap = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        applicationPartManager ??= GetServiceFromCollection<ApplicationPartManager>(services)
            ?? services.AddMvcCore().PartManager;

        var tempSource = new ApplicationPartsTypeSource(applicationPartManager, topologicalSort: true, featureToggles: null);
        ScanFeatureKeySets(tempSource.GetAssemblies(), ref disableByDefault, ref dependencyMap);
        return CoreServiceCollectionExtensions.AddFeatureToggles(services, configuration, disableByDefault, dependencyMap);
    }

    internal static void ScanFeatureKeySets(IEnumerable<Assembly> assemblies,
        ref object[] disableByDefault,
        ref Dictionary<string, List<RequiresFeatureAttribute>> dependencyMap)
    {
        var disableSet = new HashSet<string>(disableByDefault?.Select(FeatureTogglesExtensions.ToFeatureKey) ?? []);
        var dependencyMapDict = new Dictionary<string, List<RequiresFeatureAttribute>>();
        if (dependencyMap != null)
        {
            foreach (var kvp in dependencyMap)
                dependencyMapDict[kvp.Key] = new(kvp.Value);
        }

        foreach (var assembly in assemblies)
        foreach (var keySetType in assembly.GetTypes().Where(x => x.GetAttribute<FeatureKeySetAttribute>() is not null))
        {
            var commonDeps = keySetType.GetCustomAttribute<RequiresFeatureAttribute>();

            foreach (var member in keySetType.GetMembers(BindingFlags.Static | BindingFlags.Public).Where(x => x.MemberType == MemberTypes.Field))
            {
                var defAttr = member.GetCustomAttribute<DefaultValueAttribute>();
                if (defAttr != null && defAttr.Value is bool b && !b)
                    disableSet.Add(member.Name);

                var memberDeps = member.GetCustomAttribute<RequiresFeatureAttribute>();

                if (commonDeps != null || memberDeps != null)
                {
                    if (!dependencyMapDict.TryGetValue(member.Name, out var deps))
                        dependencyMapDict[member.Name] = deps = [];

                    if (commonDeps != null &&
                        commonDeps.Features.Any(x => x != member.Name))
                        deps.Add(commonDeps);

                    if (memberDeps != null &&
                        memberDeps.Features.Any(x => x != member.Name))
                        deps.Add(memberDeps);
                }
            }
        }

        disableByDefault = [.. disableSet];
        dependencyMap = dependencyMapDict;
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
