namespace Serenity.Data;

/// <summary>
/// Determines database alias, used for unit test database contexts.
/// </summary>
/// <seealso cref="Attribute" />
/// <remarks>
/// Initializes a new instance of the <see cref="DatabaseAliasAttribute"/> class.
/// </remarks>
/// <param name="value">The value.</param>
public class DatabaseAliasAttribute(string value) : Attribute
{

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public string Value { get; private set; } = value;
}