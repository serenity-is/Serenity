
namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that the target property should not get written when a form or grid is saved.
/// This means, the editor of the property reads the value from the entity, but it doesn't get
/// written back to the entity being saved. Useful for read-only or computed properties 
/// that shouldn't update the entity.
/// </summary>
public class SkipOnSaveAttribute : Attribute
{
}