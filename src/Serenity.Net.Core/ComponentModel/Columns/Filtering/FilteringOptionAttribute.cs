namespace Serenity.ComponentModel;

/// <summary>
/// Declares a new filtering option
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
public class FilteringOptionAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FilteringOptionAttribute"/> class.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public FilteringOptionAttribute(string key, object value)
    {
        Key = key;
        Value = value;
    }

    /// <summary>
    /// Gets the key.
    /// </summary>
    public string Key { get; private set; }

    /// <summary>
    /// Gets the value.
    /// </summary>
    public object Value { get; private set; }
}
