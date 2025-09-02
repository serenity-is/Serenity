namespace Serenity.ComponentModel;

/// <summary>
/// Add an option to the quick filtering.
/// </summary>
/// <seealso cref="Attribute" />
/// <remarks>
/// Initializes a new instance of the <see cref="QuickFilterOptionAttribute"/> class.
/// </remarks>
/// <param name="key">The key.</param>
/// <param name="value">The value.</param>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
public class QuickFilterOptionAttribute(string key, object value) : Attribute
{

    /// <summary>
    /// Gets the key of the quick filter option.
    /// </summary>
    /// <value>
    /// The key.
    /// </value>
    public string Key { get; private set; } = key;

    /// <summary>
    /// Gets the value of the quick filter option.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public object? Value { get; private set; } = value;
}
