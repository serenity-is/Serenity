using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Serenity.Web;

namespace Serenity.Extensions.DependencyInjection;

/// <summary>
/// DI extension methods related to application parts and type source.
/// </summary>
public static class ApplicationPartsServiceCollectionExtensions
{
    /// <summary>
    /// Adds the <see cref="IFeatureToggles"/> service to the registry, scanning for
    /// <see cref="FeatureKeySetAttribute"/> in application parts and disabling by default
    /// features that have a <c>[DefaultValue(false)]</c> attribute.
    /// Note that this also calls <c>AddMvcCore()</c> to get the part manager if it is
    /// not provided and not found in the collection.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="configuration">The configuration source.</param>
    /// <param name="applicationPartManager">Optional application part manager to use.</param>
    /// <param name="disableByDefault">Features to disable by default; pass <c>["*"]</c> to disable
    /// all features by default.</param>
    /// <param name="dependencyMap">Feature dependency map. Features are dictionary
    /// keys and the list of features that they depend on (all must be enabled)
    /// for that feature to be enabled.</param>
    /// <param name="tryPartRecovery">Whether to try recovering application parts when they
    /// were not discovered at build time. Defaults to <c>true</c>.</param>
    /// <param name="knownFeatures">Additional known feature keys, e.g. ones used by
    /// <see cref="RequiresFeatureAttribute"/> that are not declared by a
    /// <see cref="FeatureKeySetAttribute"/> enum. These are merged with the ones found by the
    /// scan, so they are not reported as unknown and are enabled by default.</param>
    /// <param name="disableUnknown">Whether to disable unknown feature keys, i.e. the ones that
    /// are not declared in a scanned <see cref="FeatureKeySetAttribute"/> enum, not in the
    /// dependency map, not disabled by default and not set in the configuration.
    /// Defaults to <c>false</c>, which only reports them and keeps them enabled. The Serene and
    /// StartSharp templates pass <c>true</c>.</param>
    /// <param name="onUnknownFeature">Callback that is invoked once for each feature key that is
    /// queried but not declared anywhere. Defaults to logging a warning.</param>
    /// <returns>The same service collection so that calls can be chained.</returns>
    /// <remarks>
    /// The scanned <see cref="FeatureKeySetAttribute"/> members are also used as the known
    /// features, so a feature key that is not declared anywhere is reported through
    /// <paramref name="onUnknownFeature"/>, and disabled if <paramref name="disableUnknown"/> is
    /// <c>true</c>. A key that is only referenced as a
    /// dependency by a <see cref="RequiresFeatureAttribute"/> is not a declaration, so it is
    /// unknown too, and disables the features that depend on it when
    /// <paramref name="disableUnknown"/> is <c>true</c>: declare it in a
    /// <see cref="FeatureKeySetAttribute"/> enum, pass it in <paramref name="knownFeatures"/>, or
    /// set it in the configuration.
    /// <para>
    /// Disabling unknown keys can break existing applications that use string feature keys
    /// without declaring them, so it is opt-in. Review the logged feature keys first, then
    /// pass <c>disableUnknown: true</c> to catch typos in feature keys.
    /// </para>
    /// </remarks>
    public static IServiceCollection AddApplicationPartsFeatureToggles(this IServiceCollection services,
        IConfiguration configuration,
        ApplicationPartManager? applicationPartManager = null,
        object[]? disableByDefault = null,
        Dictionary<string, List<RequiresFeatureAttribute>>? dependencyMap = null,
        bool tryPartRecovery = true,
        IEnumerable<string>? knownFeatures = null,
        bool disableUnknown = false,
        Action<string>? onUnknownFeature = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        applicationPartManager ??= GetServiceFromCollection<ApplicationPartManager>(services)
            ?? services.AddMvcCore().PartManager;

        var tempSource = new ApplicationPartsTypeSource(applicationPartManager,
            topologicalSort: true, featureToggles: null, tryPartRecovery: tryPartRecovery);
        var knownFeatureSet = ScanFeatureKeySets(tempSource.GetAssemblies(), ref disableByDefault,
            ref dependencyMap, knownFeatures);

        // the service provider is needed to resolve the logger, so register via a factory
        services.TryAddSingleton<IFeatureToggles>(s => new ConfigurationFeatureToggles(
            configuration, disableByDefault, dependencyMap, knownFeatureSet, disableUnknown,
            onUnknownFeature ?? (featureKey => s.GetService<ILogger<ConfigurationFeatureToggles>>()?
                .LogWarning("Feature key '{FeatureKey}' is not declared anywhere, so it is {Status}. " +
                    "Declare it in a [FeatureKeySet] enum, pass it as a known feature, or set it " +
                    "in the configuration.", featureKey,
                    disableUnknown ? "disabled" : "enabled, but will be disabled when disableUnknown is true"))));

        return services;
    }

    /// <summary>
    /// Scans the assemblies for <see cref="FeatureKeySetAttribute"/> enums, appends the
    /// features that are disabled by default and their dependencies to the provided arguments.
    /// </summary>
    /// <param name="assemblies">Assemblies to scan.</param>
    /// <param name="disableByDefault">Features to disable by default, appended with the ones that
    /// have a <c>[DefaultValue(false)]</c> attribute.</param>
    /// <param name="dependencyMap">Feature dependency map, appended with the dependencies
    /// declared by <see cref="RequiresFeatureAttribute"/>.</param>
    /// <param name="knownFeatures">Additional known feature keys, merged with the ones found
    /// by the scan.</param>
    /// <returns>The names of all the feature keys declared in the scanned assemblies, merged
    /// with <paramref name="knownFeatures"/>.</returns>
    internal static HashSet<string> ScanFeatureKeySets(IEnumerable<Assembly> assemblies,
        ref object[]? disableByDefault,
        ref Dictionary<string, List<RequiresFeatureAttribute>>? dependencyMap,
        IEnumerable<string>? knownFeatures = null)
    {
        var disableSet = new HashSet<string>(disableByDefault?.Select(FeatureTogglesExtensions.ToFeatureKey) ?? []);
        var knownSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var dependencyMapDict = new Dictionary<string, List<RequiresFeatureAttribute>>();
        if (dependencyMap != null)
        {
            foreach (var kvp in dependencyMap)
                dependencyMapDict[kvp.Key] = [.. kvp.Value];
        }

        // explicitly disabled features and the ones passed by the caller are known by definition
        knownSet.UnionWith(disableSet);
        if (knownFeatures != null)
            knownSet.UnionWith(knownFeatures);

        foreach (var assembly in assemblies)
        foreach (var keySetType in assembly.GetTypes().Where(x => x.GetCustomAttribute<FeatureKeySetAttribute>(inherit: false) is not null))
        {
            var commonDeps = keySetType.GetCustomAttribute<RequiresFeatureAttribute>();

            foreach (var member in keySetType.GetMembers(BindingFlags.Static | BindingFlags.Public).Where(x => x.MemberType == MemberTypes.Field))
            {
                knownSet.Add(member.Name);

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
        return knownSet;
    }

    /// <summary>
    /// Adds an application part type source to the service collection.
    /// Note that this also calls <c>AddMvcCore()</c> to get the part manager if it is
    /// not provided and not found in the collection.
    /// </summary>
    /// <param name="collection">The service collection.</param>
    /// <param name="partManager">The <see cref="ApplicationPartManager"/> instance.</param>
    /// <param name="featureToggles">The feature toggles.</param>
    /// <param name="topologicalSort">Whether to sort assemblies topologically by references.</param>
    /// <param name="tryPartRecovery">Whether to try recovering application parts when they
    /// were not discovered at build time. Defaults to <c>true</c>.</param>
    /// <returns>The created <see cref="ApplicationPartsTypeSource"/>.</returns>
    public static ApplicationPartsTypeSource AddApplicationPartsTypeSource(this IServiceCollection collection,
        ApplicationPartManager? partManager = null, IFeatureToggles? featureToggles = null, bool topologicalSort = true,
        bool tryPartRecovery = true)
    {
        ArgumentNullException.ThrowIfNull(collection);
        if (GetServiceFromCollection<ITypeSource>(collection) != null)
            ArgumentExceptions.OutOfRange(nameof(collection), "ITypeSource already registered");

        partManager ??= GetServiceFromCollection<ApplicationPartManager>(collection)
            ?? collection.AddMvcCore().PartManager;

        featureToggles ??= GetServiceFromCollection<IFeatureToggles>(collection);

        var typeSource = new ApplicationPartsTypeSource(partManager, topologicalSort, featureToggles, tryPartRecovery);
        collection.AddSingleton<ITypeSource>(typeSource);
        return typeSource;
    }

    private static T? GetServiceFromCollection<T>(IServiceCollection services)
        where T: class
    {
        return services.LastOrDefault(d => 
            d.ServiceType == typeof(T))?.ImplementationInstance as T;
    }
}
