namespace Serenity.ComponentModel;

/// <summary>
/// Please prefer using <see cref="SkipNameCheckAttribute"/> instead.
/// Skips checking a property name. This is usually used along with BasedOnRow attribute
/// CheckNames = true to skip checking property name if it matches a field property name in the row.
/// </summary>
[Obsolete("Prefer SkipNameCheckAttribute")]
public class IgnoreNameAttribute : SkipNameCheckAttribute
{
}