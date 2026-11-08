namespace Serenity.ComponentModel;

/// <summary>
/// Sets the enumeration key of an enum, or of a <see cref="StringEnum"/>, which is used as the
/// <c>Enums.&lt;EnumKey&gt;.&lt;Name&gt;</c> prefix of its member texts.
/// </summary>
/// <seealso cref="Attribute" />
/// <remarks>
/// Initializes a new instance of the <see cref="EnumKeyAttribute"/> class.
/// </remarks>
/// <param name="value">The value.</param>
[AttributeUsage(AttributeTargets.Enum | AttributeTargets.Class, AllowMultiple = false)]
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