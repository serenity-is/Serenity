namespace Serenity.ComponentModel;

/// <summary>
/// Sets local text prefix for the row.
/// </summary>
/// <seealso cref="Attribute" />
/// <remarks>
/// Initializes a new instance of the <see cref="LocalTextPrefixAttribute"/> class.
/// </remarks>
/// <param name="value">The prefix.</param>
public class LocalTextPrefixAttribute(string value) : Attribute
{

    /// <summary>
    /// Gets the local text prefix.
    /// </summary>
    /// <value>
    /// The local text prefix.
    /// </value>
    public string Value { get; private set; } = value;
}