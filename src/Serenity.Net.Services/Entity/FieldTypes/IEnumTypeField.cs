namespace Serenity.Data;

/// <summary>
/// Interface for fields with an enum type property
/// </summary>
public interface IEnumTypeField
{
    /// <summary>
    /// Gets or sets the type of the enum.
    /// </summary>
    /// <value>
    /// The type of the enum.
    /// </value>
    Type EnumType { get; set; }
}
