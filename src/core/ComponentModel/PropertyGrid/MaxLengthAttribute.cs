namespace Serenity.ComponentModel;

/// <summary>
/// Sets the maximum length of an editor attached to the target property.
/// </summary>
/// <seealso cref="Attribute" />
/// <remarks>
/// Initializes a new instance of the <see cref="MaxLengthAttribute"/> class.
/// </remarks>
/// <param name="maxLength">The maximum length.</param>
public class MaxLengthAttribute(int maxLength) : Attribute
{

    /// <summary>
    /// Gets the maximum length.
    /// </summary>
    /// <value>
    /// The maximum length.
    /// </value>
    public int MaxLength { get; private set; } = maxLength;
}
