namespace Serenity.Data.Mapping;

/// <summary>
/// Determines minimum selection level for this field.
/// </summary>
/// <seealso cref="Attribute" />
public class MinSelectLevelAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MinSelectLevelAttribute"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    public MinSelectLevelAttribute(SelectLevel value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public SelectLevel Value { get; private set; }
}