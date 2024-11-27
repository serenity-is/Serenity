
namespace Serenity.Data.Mapping;

/// <summary>
/// Marks the field as Identity, a combination of PrimaryKey, AutoIncrement and NotNull flags.
/// </summary>
/// <seealso cref="SetFieldFlagsAttribute" />
public class IdentityAttribute : SetFieldFlagsAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityAttribute"/> class.
    /// </summary>
    public IdentityAttribute()
        : base(FieldFlags.Identity, FieldFlags.Insertable | FieldFlags.Updatable)
    {
    }
}