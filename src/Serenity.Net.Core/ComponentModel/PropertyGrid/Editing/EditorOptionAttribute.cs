namespace Serenity.ComponentModel;

/// <summary>
/// Sets an editor option for target property editor.
/// Avoid using this where possible as option keys and values 
/// are not checked.
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
public class EditorOptionAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EditorOptionAttribute"/> class.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public EditorOptionAttribute(string key, object? value)
    {
        Key = key;
        Value = value;
    }

    /// <summary>
    /// Gets the key.
    /// </summary>
    /// <value>
    /// The key.
    /// </value>
    public string Key { get; private set; }

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public object? Value { get; private set; }
}
