namespace Serenity.Abstractions;

/// <summary>
/// An interface to access feature toggle values
/// </summary>
public interface IFeatureToggles
{
    /// <summary>
    /// Gets if a feature is enabled
    /// </summary>
    /// <param name="feature">Feature name</param>
    bool IsEnabled(string feature);
}