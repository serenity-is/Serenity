namespace Serenity.ComponentModel;

/// <summary>
/// Adds a formatter option
/// </summary>
/// <seealso cref="Attribute" />
/// <remarks>
/// Initializes a new instance of the <see cref="FormatterOptionAttribute"/> class.
/// </remarks>
/// <param name="key">The key.</param>
/// <param name="value">The value.</param>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
public class FormatterOptionAttribute(string key, object? value) : Attribute
{

    /// <summary>
    /// Gets the option key.
    /// </summary>
    /// <value>
    /// The option key.
    /// </value>
    public string Key { get; private set; } = key;

    /// <summary>
    /// Gets the option value.
    /// </summary>
    /// <value>
    /// The option value.
    /// </value>
    public object? Value { get; private set; } = value;
}
