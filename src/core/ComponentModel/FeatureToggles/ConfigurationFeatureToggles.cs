using Microsoft.Extensions.Configuration;

namespace Serenity;

/// <summary>
/// Default implementation for IFeatureToggles that reads from configuration's
/// FeatureToggles section. Unless the value is explicitly set to "false" for a feature,
/// it is considered enabled. When <paramref name="knownFeatures"/> is passed, features
/// that are not declared anywhere are considered unknown, and reported through
/// <see cref="UnknownFeatures"/>. They are also disabled when <paramref name="disableUnknown"/>
/// is <c>true</c>.
/// </summary>
/// <param name="configuration">Configuration</param>
/// <param name="disableByDefault">Features to disable by default</param>
/// <param name="dependencyMap">Feature dependency map. Features are dictionary
/// keys and the list of features that they depend on (e.g. all must be enabled)
/// for that feature to be enabled.</param>
/// <param name="knownFeatures">Known feature keys, e.g. the members of the enums annotated
/// with <c>FeatureKeySetAttribute</c>. When this is not <c>null</c>, a feature that is not
/// declared anywhere is considered an unknown feature, and disabled if
/// <paramref name="disableUnknown"/> is <c>true</c>. Pass <c>null</c> when the known features
/// cannot be determined, which keeps the behavior where every feature is enabled by default.</param>
/// <param name="disableUnknown">Whether to disable unknown features, i.e. ones that are not
/// declared anywhere. Defaults to <c>false</c>, which only reports them through
/// <see cref="UnknownFeatures"/> and <paramref name="onUnknownFeature"/> while keeping them
/// enabled, e.g. to review them in an existing installation. Pass <c>true</c> to disable them.
/// It only has an effect when <paramref name="knownFeatures"/> is not <c>null</c>.</param>
/// <param name="onUnknownFeature">Callback that is invoked once for each feature key that is
/// queried but not declared anywhere, e.g. to log it.</param>
/// <remarks>
/// Feature names are matched case insensitively, like the configuration keys. A feature is
/// considered declared when it is in <paramref name="knownFeatures"/>, in
/// <paramref name="disableByDefault"/>, in <paramref name="dependencyMap"/>, or when it appears
/// in the configuration, even if its value cannot be parsed. Note that only the feature keys
/// themselves are declarations, so a key that is merely referenced by a
/// <c>RequiresFeatureAttribute</c> as a dependency is still unknown unless it is declared.
/// </remarks>
public class ConfigurationFeatureToggles(IConfiguration configuration,
    object[]? disableByDefault = null,
    Dictionary<string, List<RequiresFeatureAttribute>>? dependencyMap = null,
    IEnumerable<string>? knownFeatures = null,
    bool disableUnknown = false,
    Action<string>? onUnknownFeature = null) : IFeatureToggles
{
    private readonly HashSet<string>? disableByDefault = disableByDefault != null ?
        new(disableByDefault.Select(FeatureTogglesExtensions.ToFeatureKey), StringComparer.OrdinalIgnoreCase) :
        null;

    private readonly Dictionary<string, List<RequiresFeatureAttribute>>? dependencyMap =
        BuildDependencyMap(dependencyMap);

    private readonly HashSet<string>? knownFeatures = knownFeatures != null ?
        new(knownFeatures, StringComparer.OrdinalIgnoreCase) : null;

    private readonly ConcurrentDictionary<string, byte> unknownFeatures =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly Action<string>? onUnknownFeature = onUnknownFeature;

    /// <summary>
    /// Gets the names of the features that were queried, but were not declared anywhere, so
    /// they are most likely due to a typo in a feature key or a missing declaration. They are
    /// disabled when <c>disableUnknown</c> is <c>true</c>, and reported in either case, so it is a good idea
    /// to log them at startup. Note that this only keeps the keys that were queried, and is
    /// meant for diagnostics, not as a registry.
    /// </summary>
    public IEnumerable<string> UnknownFeatures => [.. unknownFeatures.Keys];

    /// <inheritdoc />
    public bool IsEnabled(string feature)
    {
        return IsEnabled(feature, 0);
    }

    private bool IsEnabled(string feature, int depth)
    {
        var value = configuration[$"FeatureToggles:{feature}"];
        if (value == null || !bool.TryParse(value, out var b))
        {
            // a feature that appears in the configuration is declared, even if its value cannot
            // be parsed, in which case the value is treated as unset
            if (value == null &&
                knownFeatures != null &&
                !HasGlobalDefault() &&
                !IsKnownFeature(feature))
            {
                if (unknownFeatures.TryAdd(feature, 0))
                    InvokeOnUnknownFeature(feature);

                if (disableUnknown)
                    return false;
            }

            if (disableByDefault != null &&
                (disableByDefault.Contains(feature) ||
                 disableByDefault.Contains("*")))
                return false;

            value = configuration["FeatureToggles:*"];
            if (value != null && bool.TryParse(value, out b) && !b)
                return false;
        }
        else if (!b)
            return false;

        if (dependencyMap != null &&
            dependencyMap.TryGetValue(feature, out var dependencies))
        {
            if (depth > 10)
                throw new InvalidOperationException($"Feature toggle '{feature}' has circular dependencies!");

            foreach (var dependency in dependencies)
            {
                if (dependency == null ||
                    dependency.Features == null)
                    continue;

                var features = dependency.Features
                    .Where(x => !string.Equals(x, feature, StringComparison.OrdinalIgnoreCase));

                if (!features.Any())
                    continue;

                if (dependency.RequireAny)
                {
                    if (!features.Any(f => IsEnabled(f, depth + 1)))
                        return false;
                }
                else
                {
                    if (!features.All(f => IsEnabled(f, depth + 1)))
                        return false;
                }
            }
        }

        return true;
    }

    private bool IsKnownFeature(string feature)
    {
        return knownFeatures?.Contains(feature) == true ||
            disableByDefault?.Contains(feature) == true ||
            dependencyMap?.ContainsKey(feature) == true;
    }

    private void InvokeOnUnknownFeature(string feature)
    {
        try
        {
            onUnknownFeature?.Invoke(feature);
        }
        catch
        {
            // a failing diagnostics callback, e.g. a logger, must not break feature checks
        }
    }

    private bool HasGlobalDefault()
    {
        // a global default, that can be parsed, means the operator handles the feature keys
        // generically, so unknown features are governed by it instead of being disabled
        var value = configuration["FeatureToggles:*"];
        return value != null && bool.TryParse(value, out _);
    }

    private static Dictionary<string, List<RequiresFeatureAttribute>>? BuildDependencyMap(
        Dictionary<string, List<RequiresFeatureAttribute>>? dependencyMap)
    {
        if (dependencyMap == null)
            return null;

        var result = new Dictionary<string, List<RequiresFeatureAttribute>>(StringComparer.OrdinalIgnoreCase);
        foreach (var kvp in dependencyMap)
        {
            // keys that only differ by case, or entries with null values are tolerated here,
            // as they would otherwise throw and prevent the host from starting
            if (!result.TryGetValue(kvp.Key, out var dependencies))
                result[kvp.Key] = dependencies = [];

            if (kvp.Value != null)
                dependencies.AddRange(kvp.Value);
        }

        return result;
    }
}
