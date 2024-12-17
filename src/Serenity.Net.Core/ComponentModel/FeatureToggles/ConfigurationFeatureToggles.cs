using Microsoft.Extensions.Configuration;

namespace Serenity;

/// <summary>
/// Default implementation for IFeatureToggles that reads from configuration's
/// FeatureToggles section. Unless the value is explicitly set to "false" for a feature,
/// it is considered enabled.
/// </summary>
/// <param name="configuration">Configuration</param>
/// <param name="disableByDefault">Features to disable by default</param>
/// <param name="dependencyMap">Feature dependency map. Features are dictionary
/// keys and the list of features that they depend on (e.g. all must be enabled)
/// for that feature to be enabled.</param>
public class ConfigurationFeatureToggles(IConfiguration configuration,
    object[]? disableByDefault = null,
    Dictionary<string, List<RequiresFeatureAttribute>>? dependencyMap = null) : IFeatureToggles
{
    private readonly HashSet<string>? disableByDefault = disableByDefault != null ?
        new HashSet<string>(disableByDefault.Select(FeatureTogglesExtensions.ToFeatureKey)) : null;

    private readonly Dictionary<string, List<RequiresFeatureAttribute>>? dependencyMap =
        dependencyMap?.ToDictionary(kvp => kvp.Key, kvp => new List<RequiresFeatureAttribute>(kvp.Value));

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

                var features = dependency.Features.Where(x => x != feature);
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
}


