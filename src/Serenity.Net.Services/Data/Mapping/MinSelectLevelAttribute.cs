namespace Serenity.Data.Mapping;

/// <summary>
/// Determines minimum selection level for this field.
/// </summary>
/// <seealso cref="Attribute" />
/// <remarks>
/// Initializes a new instance of the <see cref="MinSelectLevelAttribute"/> class.
/// </remarks>
/// <param name="value">The value.</param>
public class MinSelectLevelAttribute(SelectLevel value) : Attribute
{

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public SelectLevel Value { get; private set; } = value;
}