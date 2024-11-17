
namespace Serenity.Data.Mapping;

/// <summary>
/// Marks the property as part of the primary key.
/// </summary>
/// <seealso cref="SetFieldFlagsAttribute" />
public class PrimaryKeyAttribute : SetFieldFlagsAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PrimaryKeyAttribute"/> class.
    /// </summary>
    public PrimaryKeyAttribute()
        : base(FieldFlags.PrimaryKey)
    {
    }
}