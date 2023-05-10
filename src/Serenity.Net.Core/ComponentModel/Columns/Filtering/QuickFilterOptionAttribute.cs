namespace Serenity.ComponentModel;

/// <summary>
/// Add an option to the quick filtering.
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
public class QuickFilterOptionAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QuickFilterOptionAttribute"/> class.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public QuickFilterOptionAttribute(string key, object value)
    {
        Key = key;
        Value = value;
    }

    /// <summary>
    /// Gets the key of the quick filter option.
    /// </summary>
    /// <value>
    /// The key.
    /// </value>
    public string Key { get; private set; }

    /// <summary>
    /// Gets the value of the quick filter option.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public object? Value { get; private set; }
}
