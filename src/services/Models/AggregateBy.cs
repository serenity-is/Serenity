namespace Serenity.Services;

/// <summary>
/// A value to calculate from all the rows a list request matches, and from the rows of each of its groups,
/// e.g. the sum of a field. It is in the response under the key of its type, then the name of its field,
/// see <see cref="AggregateGroup.Values"/>.
/// </summary>
public class AggregateBy
{
    /// <summary>
    /// Creates an empty AggregateBy object
    /// </summary>
    public AggregateBy()
    {
    }

    /// <summary>
    /// Creates an AggregateBy object for the specified field name and type.
    /// </summary>
    /// <param name="field">The field name.</param>
    /// <param name="type">The type of the aggregate.</param>
    public AggregateBy(string field, AggregateType type)
    {
        Field = field;
        Type = type;
    }

    /// <summary>
    /// Gets/sets the field name. It is only optional for <see cref="AggregateType.Count"/>,
    /// which is the number of the rows without one, like <see cref="AggregateGroup.Count"/>.
    /// </summary>
    public string? Field { get; set; }

    /// <summary>
    /// Gets/sets the type of the aggregate
    /// </summary>
    public AggregateType Type { get; set; }
}
