namespace Serenity.ComponentModel;

/// <summary>
/// Sets icon class
/// </summary>
/// <seealso cref="Attribute" />
/// <remarks>
/// Initializes a new instance of the <see cref="IconClassAttribute"/> class.
/// </remarks>
/// <param name="value">The icon class.</param>
public class IconClassAttribute(string value) : Attribute
{

    /// <summary>
    /// Gets the icon class
    /// </summary>
    public string Value { get; private set; } = value;
}