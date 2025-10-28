namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that the target property should not get read from the source entity when a form or grid 
/// is loading. This means, the editor of the property saves the value to the entity on save, 
/// but it doesn't get read back from it when loading first time or refreshing. Useful for 
/// properties that should retain editor state without syncing to the entity on load.
/// </summary>
public class SkipOnLoadAttribute : Attribute
{
}