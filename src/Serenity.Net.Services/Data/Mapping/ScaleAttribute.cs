namespace Serenity.Data.Mapping;

/// <summary>
/// Determines numeric scale (decimal places) for the field.
/// </summary>
/// <seealso cref="Attribute" />
/// <remarks>
/// Initializes a new instance of the <see cref="ScaleAttribute"/> class.
/// </remarks>
/// <param name="value">The value.</param>
public class ScaleAttribute(int value) : Attribute
{

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public int Value { get; set; } = value;
}