namespace Serenity.Data.Mapping;

/// <summary>
/// Determines size (max length or numeric precision for) for the field.
/// </summary>
/// <seealso cref="Attribute" />
public class SizeAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SizeAttribute"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    public SizeAttribute(int value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public int Value { get; set; }
}