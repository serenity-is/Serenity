namespace Serenity.ComponentModel;

/// <summary>
/// Declares a new filtering option
/// </summary>
/// <seealso cref="Attribute" />
/// <remarks>
/// Initializes a new instance of the <see cref="FilteringOptionAttribute"/> class.
/// </remarks>
/// <param name="key">The key.</param>
/// <param name="value">The value.</param>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
public class FilteringOptionAttribute(string key, object value) : Attribute
{

    /// <summary>
    /// Gets the key.
    /// </summary>
    public string Key { get; private set; } = key;

    /// <summary>
    /// Gets the value.
    /// </summary>
    public object Value { get; private set; } = value;
}
