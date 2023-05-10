namespace Serenity.ComponentModel;

/// <summary>
/// Sets the enumeration key of an enum
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Enum, AllowMultiple = false)]
public class EnumKeyAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EnumKeyAttribute"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    public EnumKeyAttribute(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the enum key.
    /// </summary>
    /// <value>
    /// The enum key.
    /// </value>
    public string Value { get; private set; }
}