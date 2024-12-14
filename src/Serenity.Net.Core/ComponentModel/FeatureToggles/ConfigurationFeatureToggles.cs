using Microsoft.Extensions.Configuration;

namespace Serenity;

/// <summary>
/// Default implementation for IFeatureToggles that reads from configuration's
/// FeatureToggles section. Unless the value is explicitly set to "false" for a feature,
/// it is considered enabled.
/// </summary>
/// <param name="configuration">Configuration</param>
/// <param name="disableByDefault">Features to disable by default</param>
public class ConfigurationFeatureToggles(IConfiguration configuration,
    object[]? disableByDefault = null) : IFeatureToggles
{
    private readonly HashSet<string>? disableByDefault = disableByDefault != null ?
        new HashSet<string>(disableByDefault.Select(FeatureTogglesExtensions.ToFeatureKey)) : null;

    /// <inheritdoc />
    public bool IsEnabled(string feature)
    {
        var value = configuration[$"FeatureToggles:{feature}"];
        if (value == null || !bool.TryParse(value, out var b))
        {
            if (disableByDefault != null &&
                (disableByDefault.Contains(feature) ||
                 disableByDefault.Contains("*")))
                return false;

            value = configuration["FeatureToggles:*"];
            return value == null || !bool.TryParse(value, out b) || b;
        }

        return b;
    }
}


