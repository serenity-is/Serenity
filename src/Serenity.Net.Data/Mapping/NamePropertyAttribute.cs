namespace Serenity.Data;

/// <summary>
/// Determines that the attached property is the name field of the table.
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class NamePropertyAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NamePropertyAttribute"/> class.
    /// </summary>
    public NamePropertyAttribute()
    {
    }
}