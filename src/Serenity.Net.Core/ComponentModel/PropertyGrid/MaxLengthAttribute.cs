namespace Serenity.ComponentModel;

/// <summary>
/// Sets the maximum length of an editor attached to the target property.
/// </summary>
/// <seealso cref="Attribute" />
public class MaxLengthAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MaxLengthAttribute"/> class.
    /// </summary>
    /// <param name="maxLength">The maximum length.</param>
    public MaxLengthAttribute(int maxLength)
    {
        MaxLength = maxLength;
    }

    /// <summary>
    /// Gets the maximum length.
    /// </summary>
    /// <value>
    /// The maximum length.
    /// </value>
    public int MaxLength { get; private set; }
}
