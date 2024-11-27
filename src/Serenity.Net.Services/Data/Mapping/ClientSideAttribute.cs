namespace Serenity.Data.Mapping;

/// <summary>
/// Marks the field as NotMapped, obsolete, prefer [NotMapped] attribute.
/// </summary>
/// <seealso cref="SetFieldFlagsAttribute" />
[Obsolete("Prefer NotMapped attribute")]
public class ClientSideAttribute : SetFieldFlagsAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClientSideAttribute"/> class.
    /// </summary>
    public ClientSideAttribute()
        : base(FieldFlags.ClientSide)
    {
    }
}