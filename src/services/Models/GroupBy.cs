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
    public GroupBy(string field, bool descending = false)
    {
        Field = field;
        Descending = descending;
    }

    /// <summary>
    /// Gets/sets the field name
    /// </summary>
    public string Field { get; set; } = null!;

    /// <summary>
    /// Gets/sets whether the groups are in descending order of the field
    /// </summary>
    public bool Descending { get; set; }
}
