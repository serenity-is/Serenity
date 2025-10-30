namespace Serenity.ComponentModel;

/// <summary>
/// Sets column width
/// </summary>
/// <seealso cref="Attribute" />
/// <remarks>
/// Initializes a new instance of the <see cref="WidthAttribute"/> class.
/// </remarks>
/// <param name="value">The value.</param>
public class WidthAttribute(int value) : Attribute
{

    /// <summary>
    /// Gets the column width.
    /// </summary>
    /// <value>
    /// The column width.
    /// </value>
    public int Value { get; private set; } = value;

    /// <summary>
    /// Gets or sets the minimum width.
    /// </summary>
    /// <value>
    /// The minimum width.
    /// </value>
    public int Min { get; set; }

    /// <summary>
    /// Gets or sets the maximum width.
    /// </summary>
    /// <value>
    /// The maximum width.
    /// </value>
    public int Max { get; set; }
}