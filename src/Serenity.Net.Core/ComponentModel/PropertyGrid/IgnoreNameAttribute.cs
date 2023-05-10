namespace Serenity.ComponentModel;

/// <summary>
/// Skips checking a property name. This is usually used along with BasedOnRow attribute
/// CheckNames = true to skip checking property name if it matches a field property name in the row.
/// </summary>
public class IgnoreNameAttribute : Attribute
{
}