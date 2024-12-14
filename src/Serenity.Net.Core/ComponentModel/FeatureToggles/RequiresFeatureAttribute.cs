namespace Serenity.ComponentModel;

/// <summary>
/// An attribute that can be placed on targets to toggle them at runtime based features.
/// Use FeatureBarrierAttribute for MVC controllers, controller actions, or Razor pages.
/// </summary>
[AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
public class RequiresFeatureAttribute : Attribute
{
    /// <summary>
    /// Creates an attribute that can be used to toggle targets. The toggle can be configured to require all or any of the provided feature(s) to pass.
    /// </summary>
    /// <param name="features">The names of the features that the attribute will represent.</param>
    public RequiresFeatureAttribute(params string[] features)
    {
        if (features == null || features.Length == 0)
            throw new ArgumentNullException(nameof(features));

        Features = features;
    }

    /// <summary>
    /// Creates an attribute that can be used to toggle targets. The toggle can be configured to require all or any of the provided feature(s) to pass.
    /// </summary>
    /// <param name="features">A set of enums representing the features that the attribute will represent.</param>
    public RequiresFeatureAttribute(params object[] features)
    {
        if (features == null || features.Length == 0)
            throw new ArgumentNullException(nameof(features));

        Features = features.Select(FeatureTogglesExtensions.ToFeatureKey);
    }

    /// <summary>
    /// The name of the features that the feature attribute will activate for.
    /// </summary>
    public IEnumerable<string> Features { get; }

    /// <summary>
    /// Controls whether any (true) or all (false, default) features in <see cref="Features"/> should be enabled to pass.
    /// </summary>
    public bool RequireAny { get; set; } = false;
}
