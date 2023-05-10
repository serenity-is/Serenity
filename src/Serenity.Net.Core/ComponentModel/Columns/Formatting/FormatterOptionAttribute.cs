namespace Serenity.ComponentModel;

/// <summary>
/// Adds a formatter option
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
public class FormatterOptionAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FormatterOptionAttribute"/> class.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public FormatterOptionAttribute(string key, object? value)
    {
        Key = key;
        Value = value;
    }

    /// <summary>
    /// Gets the option key.
    /// </summary>
    /// <value>
    /// The option key.
    /// </value>
    public string Key { get; private set; }

    /// <summary>
    /// Gets the option value.
    /// </summary>
    /// <value>
    /// The option value.
    /// </value>
    public object? Value { get; private set; }
}
