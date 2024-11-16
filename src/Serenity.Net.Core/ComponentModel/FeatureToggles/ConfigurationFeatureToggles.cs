using Microsoft.Extensions.Configuration;

namespace Serenity;

/// <summary>
/// Default implementation for IFeatureToggles that reads from configuration's
/// FeatureToggles section. Unless the value is explicitly set to "false" for a feature,
/// it is considered enabled.
/// </summary>
/// <param name="configuration">Configuration</param>
public class ConfigurationFeatureToggles(IConfiguration configuration) : IFeatureToggles
{
    /// <inheritdoc />
    public bool IsEnabled(string feature)
    {
        var value = configuration[$"FeatureToggles:{feature}"];
        if (value == null || !bool.TryParse(value, out var b) || b)
            return true;

        return false;
    }
}