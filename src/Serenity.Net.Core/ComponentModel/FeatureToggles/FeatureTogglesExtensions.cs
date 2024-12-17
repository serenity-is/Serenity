namespace Serenity;

/// <summary>
/// Extensions for IFeatureToggles
/// </summary>
public static class FeatureTogglesExtensions
{
    /// <summary>
    /// Gets if a feature is enabled
    /// </summary>
    /// <param name="featureToggles">Feature toggles</param>
    /// <param name="feature">Feature enum</param>
    public static bool IsEnabled(this IFeatureToggles featureToggles, Enum feature) =>
        featureToggles.IsEnabled(feature.GetName());

    /// <summary>
    /// Gets if a set of features are enabled, requiring all to be enabled
    /// </summary>
    /// <param name="featureToggles">Feature toggles</param>
    /// <param name="features">Features</param>
    public static bool IsEnabled(this IFeatureToggles featureToggles, IEnumerable<string> features) =>
        features != null && features.Any() && features.All(featureToggles.IsEnabled);

    /// <summary>
    /// Gets if a set of features are enabled, requiring just one to be enabled if requireAny is true
    /// </summary>
    /// <param name="featureToggles">Feature toggles</param>
    /// <param name="features">Features</param>
    /// <param name="requireAny">Require any (true) or all (false) feature</param>
    public static bool IsEnabled(this IFeatureToggles featureToggles, IEnumerable<string> features, bool requireAny) =>
        features != null && features.Any() && (requireAny ? features.Any(featureToggles.IsEnabled) : features.All(featureToggles.IsEnabled));

    /// <summary>
    /// Converts a feature (enum or string) to feature key string
    /// </summary>
    /// <param name="feature">Feature enum or string</param>
    /// <exception cref="ArgumentNullException">Feature is null</exception>
    /// <exception cref="ArgumentException">Feature is not a string or enum</exception>
    public static string ToFeatureKey(object feature)
    {
        if (feature == null)
            throw new ArgumentNullException(nameof(feature));

        if (feature is string s)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentException("Feature key cannot be empty.", nameof(feature));

            return s;
        }

        var type = feature.GetType();

        if (!type.IsEnum)
            throw new ArgumentException("The provided features must be enums.", nameof(feature));

        return Enum.GetName(feature.GetType(), feature);
    }
}