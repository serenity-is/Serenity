namespace Serenity.Data;

/// <summary>
/// Determines Time kind for a DateTime field.
/// </summary>
/// <seealso cref="Attribute" />
public class DateTimeKindAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DateTimeKindAttribute"/> class.
    /// </summary>
    /// <param name="value">The value. If Unspecified, it means this is a DateTime field but no time zone conversions should be done.
    /// Local means dates should be converted to server local time. Utc means dates should be converted to UTC timezone.</param>
    public DateTimeKindAttribute(DateTimeKind value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public DateTimeKind Value { get; private set; }
}