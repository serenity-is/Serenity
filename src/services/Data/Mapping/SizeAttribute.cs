namespace Serenity.Data.Mapping;

/// <summary>
/// Determines size (max length or numeric precision for) for the field.
/// </summary>
/// <seealso cref="Attribute" />
public class SizeAttribute : Attribute
{
    private int value;

    /// <summary>
    /// Initializes a new instance of the <see cref="SizeAttribute"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <exception cref="ArgumentOutOfRangeException">value is negative.</exception>
    public SizeAttribute(int value)
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