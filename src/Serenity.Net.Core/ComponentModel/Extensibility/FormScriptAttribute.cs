namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that this type should generate a form script, 
/// which contains information about properties in this type and 
/// is an array of PropertyItem objects. Form scripts can be
/// accessed from client side using Q.getForm("Key")
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class FormScriptAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FormScriptAttribute"/> class.
    /// The key of the form script will be the full name of the type this is placed on.
    /// </summary>
    /// <exception cref="ArgumentNullException">key</exception>
    public FormScriptAttribute()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FormScriptAttribute"/> class.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <exception cref="ArgumentNullException">key</exception>
    public FormScriptAttribute(string key)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException("key");

        Key = key;
    }

    /// <summary>
    /// Gets the key.
    /// </summary>
    /// <value>
    /// The key.
    /// </value>
    public string? Key { get; private set; }
}