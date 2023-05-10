namespace Serenity.ComponentModel;

/// <summary>
/// Sets column width
/// </summary>
/// <seealso cref="Attribute" />
public class WidthAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WidthAttribute"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    public WidthAttribute(int value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the column width.
    /// </summary>
    /// <value>
    /// The column width.
    /// </value>
    public int Value { get; private set; }

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