namespace Serenity.Services;

/// <summary>
/// The request model for a Retrieve service
/// </summary>
public class RetrieveRequest : ServiceRequest, IIncludeExcludeColumns
{
    /// <summary>
    /// The entity ID to retrieve
    /// </summary>
    public object EntityId { get; set; }

    /// <summary>
    /// The group of columns to select. This is 
    /// RetrieveColumnSelection.Details by default, e.g.
    /// all the table / view columns, but not unmapped or
    /// complex object style (list etc.) columns.
    /// </summary>
    public RetrieveColumnSelection ColumnSelection { get; set; }

    /// <inheritdoc/>
    [JsonConverter(typeof(JsonStringHashSetConverter))]
    public HashSet<string> IncludeColumns { get; set; }

    /// <inheritdoc/>
    [JsonConverter(typeof(JsonStringHashSetConverter))]
    public HashSet<string> ExcludeColumns { get; set; }
}