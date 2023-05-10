
namespace Serenity.Data.Mapping;

/// <summary>
/// Specifies that this property is not mapped to an SQL column/expression
/// </summary>
public class NotMappedAttribute : SetFieldFlagsAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotMappedAttribute"/> class.
    /// </summary>
    public NotMappedAttribute()
        : base(FieldFlags.NotMapped)
    {
    }
}