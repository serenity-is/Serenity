namespace Serenity.Data.Mapping;

/// <summary>
/// Determines size (max length or numeric precision for) for the field.
/// </summary>
/// <seealso cref="Attribute" />
/// <remarks>
/// Initializes a new instance of the <see cref="SizeAttribute"/> class.
/// </remarks>
/// <param name="value">The value.</param>
public class SizeAttribute(int value) : Attribute
{

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public int Value { get; set; } = value;
}