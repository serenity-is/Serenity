namespace Serenity.ComponentModel;

/// <summary>
/// Sets an editor option for target property editor.
/// Avoid using this where possible as option keys and values 
/// are not checked.
/// </summary>
/// <seealso cref="Attribute" />
/// <remarks>
/// Initializes a new instance of the <see cref="EditorOptionAttribute"/> class.
/// </remarks>
/// <param name="key">The key.</param>
/// <param name="value">The value.</param>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
public class EditorOptionAttribute(string key, object? value) : Attribute
{

    /// <summary>
    /// Gets the key.
    /// </summary>
    /// <value>
    /// The key.
    /// </value>
    public string Key { get; private set; } = key;

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public object? Value { get; private set; } = value;
}
