namespace Serenity.Data.Mapping;

/// <summary>
/// Determines numeric scale (decimal places) for the field.
/// </summary>
/// <seealso cref="Attribute" />
public class ScaleAttribute : Attribute
{
    private int value;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScaleAttribute"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <exception cref="ArgumentOutOfRangeException">value is negative.</exception>
    public ScaleAttribute(int value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value);
        this.value = value;
    }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    /// <exception cref="ArgumentOutOfRangeException">value is negative.</exception>
    public int Value
    {
        get => value;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            this.value = value;
        }
    }
}