namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that the target property should not 
/// get serialized when a property grid is saved.
/// This means, the editor of the property reads 
/// the value from the entity, but it doesn't get
/// written back to save entity.
/// </summary>
/// <seealso cref="Attribute" />
public class OneWayAttribute : Attribute
{

    /// <summary>
    /// Initializes a new instance of the <see cref="OneWayAttribute"/> class.
    /// </summary>
    public OneWayAttribute()
    {
    }
}