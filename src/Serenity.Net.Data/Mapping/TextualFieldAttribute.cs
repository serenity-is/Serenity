namespace Serenity.Data.Mapping;

/// <summary>
/// Determines textual field for this field. This is placed on foreign keys and specifies which field
/// in joined table (view fields in this row that originates from foreign table) should be used
/// for display / filtering.
/// </summary>
/// <seealso cref="Attribute" />
public class TextualFieldAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TextualFieldAttribute"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    public TextualFieldAttribute(string value)
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