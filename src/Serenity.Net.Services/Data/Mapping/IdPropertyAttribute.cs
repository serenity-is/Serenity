namespace Serenity.Data;

/// <summary>
/// Determines that the attached property is the ID field of the table.
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class IdPropertyAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IdPropertyAttribute"/> class.
    /// </summary>
    public IdPropertyAttribute()
    {
    }
}