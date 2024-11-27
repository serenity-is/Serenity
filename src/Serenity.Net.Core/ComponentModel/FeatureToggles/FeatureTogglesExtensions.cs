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
}