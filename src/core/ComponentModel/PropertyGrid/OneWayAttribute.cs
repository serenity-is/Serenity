namespace Serenity.ComponentModel;

/// <summary>
/// Please prefer using <see cref="SkipOnSaveAttribute"/> instead.
/// Indicates that the target property should not get serialized when a property grid is saved.
/// This means, the editor of the property reads the value from the entity, but it doesn't get
/// written back to save entity.
/// </summary>
[Obsolete("Use SkipOnSaveAttribute instead")]
public class OneWayAttribute : SkipOnSaveAttribute
{
}