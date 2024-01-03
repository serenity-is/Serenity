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
    [JsonConverter(typeof(JsonConverters.NullAsDefaultJsonConverter))]
    public RetrieveColumnSelection ColumnSelection { get; set; }

    /// <inheritdoc/>
    [Newtonsoft.Json.JsonConverter(typeof(JsonStringHashSetConverter))]
    [JsonConverter(typeof(JsonConverters.HashSetStringJsonConverter))]
    public HashSet<string> IncludeColumns { get; set; }

    /// <inheritdoc/>
    [Newtonsoft.Json.JsonConverter(typeof(JsonStringHashSetConverter))]
    [JsonConverter(typeof(JsonConverters.HashSetStringJsonConverter))]
    public HashSet<string> ExcludeColumns { get; set; }
}