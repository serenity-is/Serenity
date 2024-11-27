
namespace Serenity.Data.Mapping;

/// <summary>
/// Marks the field as auto increment, e.g. generated on insert in SQL side
/// </summary>
/// <seealso cref="SetFieldFlagsAttribute" />
public class AutoIncrementAttribute : SetFieldFlagsAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AutoIncrementAttribute"/> class.
    /// </summary>
    public AutoIncrementAttribute()
        : base(FieldFlags.AutoIncrement, FieldFlags.Insertable | FieldFlags.Updatable)
    {
    }
}