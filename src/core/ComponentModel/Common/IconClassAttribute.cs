namespace Serenity.ComponentModel;

/// <summary>
/// Sets the icon class.
/// </summary>
/// <seealso cref="Attribute" />
/// <remarks>
/// Initializes a new instance of the <see cref="IconClassAttribute"/> class.
/// </remarks>
/// <param name="value">The icon class.</param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Enum | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
public class IconClassAttribute(string value) : Attribute
{

    /// <summary>
    /// Gets the icon class.
    /// </summary>
    public string Value { get; private set; } = value;
}