namespace Serenity.ComponentModel;

/// <summary>
/// Sets local text prefix for the row.
/// </summary>
/// <seealso cref="Attribute" />
public class LocalTextPrefixAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LocalTextPrefixAttribute"/> class.
    /// </summary>
    /// <param name="value">The prefix.</param>
    public LocalTextPrefixAttribute(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the local text prefix.
    /// </summary>
    /// <value>
    /// The local text prefix.
    /// </value>
    public string Value { get; private set; }
}