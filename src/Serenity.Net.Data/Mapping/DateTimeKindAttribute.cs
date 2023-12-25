namespace Serenity.Data;

/// <summary>
/// Determines Time kind for a DateTime field.
/// </summary>
/// <seealso cref="Attribute" />
/// <remarks>
/// Initializes a new instance of the <see cref="DateTimeKindAttribute"/> class.
/// </remarks>
/// <param name="value">The value. If Unspecified, it means this is a DateTime field but no time zone conversions should be done.
/// Local means dates should be converted to server local time. Utc means dates should be converted to UTC timezone.</param>
public class DateTimeKindAttribute(DateTimeKind value) : Attribute
{

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public DateTimeKind Value { get; private set; } = value;
}