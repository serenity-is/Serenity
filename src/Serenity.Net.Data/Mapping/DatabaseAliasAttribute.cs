namespace Serenity.Data;

/// <summary>
/// Determines database alias, used for unit test database contexts.
/// </summary>
/// <seealso cref="Attribute" />
public class DatabaseAliasAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseAliasAttribute"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    public DatabaseAliasAttribute(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public string Value { get; private set; }
}