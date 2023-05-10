namespace Serenity.Data.Mapping;

/// <summary>
/// Determines numeric scale (decimal places) for the field.
/// </summary>
/// <seealso cref="Attribute" />
public class ScaleAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ScaleAttribute"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    public ScaleAttribute(int value)
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