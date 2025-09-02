namespace Serenity.ComponentModel;

/// <summary>
/// Sets the enumeration key of an enum
/// </summary>
/// <seealso cref="Attribute" />
/// <remarks>
/// Initializes a new instance of the <see cref="EnumKeyAttribute"/> class.
/// </remarks>
/// <param name="value">The value.</param>
[AttributeUsage(AttributeTargets.Enum, AllowMultiple = false)]
public class EnumKeyAttribute(string value) : Attribute
{

    /// <summary>
    /// Gets the enum key.
    /// </summary>
    /// <value>
    /// The enum key.
    /// </value>
    public string Value { get; private set; } = value;
}