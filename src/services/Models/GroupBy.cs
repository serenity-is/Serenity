namespace Serenity.Services;

/// <summary>
/// A field the rows a list request matches are grouped by, for the aggregates of each group,
/// see <see cref="ListRequest.GroupBy"/>.
/// </summary>
public class GroupBy
{
    /// <summary>
    /// Creates an empty GroupBy object
    /// </summary>
    public GroupBy()
    {
    }

    /// <summary>
    /// Creates a GroupBy object for the specified field name.
    /// </summary>
    /// <param name="field">The field name.</param>
    /// <param name="descending">Whether the groups are in descending order of the field.</param>
    /// <param name="interval">The interval the values of a date field are grouped by.</param>
    public GroupBy(string field, bool descending = false, DateGroupInterval? interval = null)
    {
        Field = field;
        Descending = descending;
        Interval = interval;
    }

    /// <summary>
    /// Gets/sets the field name
    /// </summary>
    public string Field { get; set; } = null!;

    /// <summary>
    /// Gets/sets whether the groups are in descending order of the field
    /// </summary>
    public bool Descending { get; set; }

    /// <summary>
    /// Gets/sets the interval the values of a date field are grouped by, e.g. the months. The key of a group
    /// is then the start of its interval, see <see cref="AggregateGroup.Key"/>. It can only be used with a date field, and
    /// the rows are not grouped at all otherwise.
    /// </summary>
    public DateGroupInterval? Interval { get; set; }
}
