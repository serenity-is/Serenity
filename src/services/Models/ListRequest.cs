namespace Serenity.Services;

/// <summary>
/// The request model for a List service.
/// </summary>
public class ListRequest : ServiceRequest, IIncludeExcludeColumns
{
    /// <summary>
    /// When true, paging limits (<see cref="RequestHandlerSettings.DefaultPageSize"/> /
    /// <see cref="RequestHandlerSettings.MaxPageSize"/>) are ignored for this request. Set via
    /// the <c>SuppressPagingLimits()</c> extension from server side code. Not bound from JSON,
    /// so clients cannot set it.
    /// </summary>
    internal bool pagingLimitsSuppressed;

    /// <summary>
    /// Number of records to skip
    /// </summary>
    [JsonConverter(typeof(JsonConverters.NullAsDefaultJsonConverter))]
    public int Skip { get; set; }

    /// <summary>
    /// Number of records to take
    /// </summary>
    [JsonConverter(typeof(JsonConverters.NullAsDefaultJsonConverter))]
    public int Take { get; set; }
    
    /// <summary>
    /// Include whether more records are available beyond the requested page in the response.
    /// </summary>
    [JsonConverter(typeof(JsonConverters.NullAsDefaultJsonConverter))]
    public bool IncludeMore { get; set; }

    /// <summary>
    /// Columns to sort returned records by
    /// </summary>
    public SortBy[]? Sort { get; set; }

    /// <summary>
    /// The text to search in columns with the 
    /// <see cref="QuickSearchAttribute"/>.
    /// </summary>
    public string? ContainsText { get; set; }

    /// <summary>
    /// If specified, the text is only searched in this 
    /// column. The column should still have a
    /// <see cref="QuickSearchAttribute"/>.
    /// </summary>
    public string? ContainsField { get; set; }

    /// <summary>
    /// The where criteria for the query. This is passed
    /// as an array of arrays in the JSON.
    /// </summary>
    [Newtonsoft.Json.JsonConverter(typeof(JsonSafeCriteriaConverter))]
    [JsonConverter(typeof(JsonConverters.SafeCriteriaJsonConverter))]
    public BaseCriteria? Criteria { get; set; }

    /// <summary>
    /// Include the deleted records. Default is false.
    /// This is only supported by services and entities
    /// that implement soft delete, e.g. IsActive etc.
    /// The value is ignored unless the current user has the
    /// undelete permission (<see cref="UndeletePermissionAttribute"/>,
    /// falling back to delete, modify and read permissions).
    /// </summary>
    [JsonConverter(typeof(JsonConverters.NullAsDefaultJsonConverter))]
    public bool IncludeDeleted { get; set; }

    /// <summary>
    /// Exclude the total count from the result. When <c>true</c>, the total number of records
    /// is not calculated (no extra <c>COUNT(*)</c>) and <see cref="ListResponse{T}.TotalCount"/>
    /// is left as zero. When <c>false</c>, the total is calculated for paged requests (when
    /// <see cref="Take"/> is greater than zero). When <c>null</c> (the default, i.e. not sent),
    /// the total is calculated for paged requests unless <see cref="IncludeMore"/> is true or
    /// the handler's <c>RequestHandlerSettings.ExcludeTotalCountByDefault</c> is enabled. A
    /// zero <see cref="Take"/> that paging limits turn into an actual page size still returns
    /// the total, regardless of that setting, unless <see cref="IncludeMore"/> is requested.
    /// </summary>
    public bool? ExcludeTotalCount { get; set; }

    /// <summary>
    /// A dictionary of field name / value pairs used to 
    /// filter those fields by the passed value. 
    /// Please note that "NULL" values are ignored, so you can't
    /// filter a field with a NULL value.
    /// </summary>
    public Dictionary<string, object?>? EqualityFilter { get; set; }

    /// <summary>
    /// Group of columns to select. This is ColumnSelection.List,
    /// e.g. only the table fields, not view fields by default.
    /// </summary>
    [JsonConverter(typeof(JsonConverters.NullAsDefaultJsonConverter))]
    public ColumnSelection ColumnSelection { get; set; }

    /// <inheritdoc/>
    [Newtonsoft.Json.JsonConverter(typeof(JsonStringHashSetConverter))]
    [JsonConverter(typeof(JsonConverters.HashSetStringJsonConverter))]
    public HashSet<string>? IncludeColumns { get; set; }

    /// <inheritdoc/>
    [Newtonsoft.Json.JsonConverter(typeof(JsonStringHashSetConverter))]
    [JsonConverter(typeof(JsonConverters.HashSetStringJsonConverter))]
    public HashSet<string>? ExcludeColumns { get; set; }

    /// <summary>
    /// Distinct set of columns. If set a DISTINCT query
    /// is used, and only these columns can be returned
    /// from the query.
    /// </summary>
    public SortBy[]? DistinctFields { get; set; }

    /// <summary>
    /// Gets or sets the set of columns to export. 
    /// This should only be used to specify list of columns
    /// for contexts like Excel export etc.
    /// </summary>
    public List<string>? ExportColumns { get; set; }

    /// <summary>
    /// Set to "true" to localize fields with the Localize attribute.
    /// Note that this will also affect filtering and sorting as the field's
    /// expression will be changed to a COALESCE one. To get a localized
    /// version in a specific language, pass the language code as the value.
    /// </summary>
    public string? Localize { get; set; }
}