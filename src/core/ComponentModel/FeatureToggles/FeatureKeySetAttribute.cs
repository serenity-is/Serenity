namespace Serenity.ComponentModel;

/// <summary>
/// Marks an enum as a set of feature toggles.
/// </summary>
[AttributeUsage(AttributeTargets.Enum, AllowMultiple = false)]
public class FeatureKeySetAttribute : Attribute
{
}