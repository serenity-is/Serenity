namespace Serenity.Services;

/// <summary>
/// Specifies the capabilities of a list request handler.
/// </summary>
[Flags]
public enum ListRequestCapabilities
{
    /// <summary>No list operations are handled by the source.</summary>
    None = 0,
    /// <summary>The source applies ContainsText.</summary>
    ContainsText = 1 << 0,
    /// <summary>The source applies ContainsField together with ContainsText.</summary>
    ContainsField = 1 << 1,
    /// <summary>The source applies EqualityFilter.</summary>
    EqualityFilter = 1 << 2,
    /// <summary>The source applies Criteria.</summary>
    Criteria = 1 << 3,
    /// <summary>Combined text-search, equality-filter, and criteria options.</summary>
    FilteringOptions = ContainsText | ContainsField | EqualityFilter | Criteria,
    /// <summary>The source applies sorting.</summary>
    Sort = 1 << 4,
    /// <summary>The source applies Skip.</summary>
    Skip = 1 << 5,
    /// <summary>The source applies Take.</summary>
    Take = 1 << 6,
    /// <summary>The source applies IncludeDeleted.</summary>
    IncludeDeleted = 1 << 7,
    /// <summary>The source applies ColumnSelection.</summary>
    ColumnSelection = 1 << 8,
    /// <summary>The source applies IncludeColumns.</summary>
    IncludeColumns = 1 << 9,
    /// <summary>The source applies ExcludeColumns.</summary>
    ExcludeColumns = 1 << 10,
    /// <summary>Combined column-selection, include-column, and exclude-column options.</summary>
    ColumnOptions = ColumnSelection | IncludeColumns | ExcludeColumns,
    /// <summary>The source applies DistinctFields.</summary>
    DistinctFields = 1 << 11,
    /// <summary>The source applies ExportColumns.</summary>
    ExportColumns = 1 << 12,
    /// <summary>The source applies Localize.</summary>
    Localize = 1 << 13,
    /// <summary>The source honors ExcludeTotalCount.</summary>
    ExcludeTotalCount = 1 << 14,
    /// <summary>The handler supports the IncludeMore request option.</summary>
    IncludeMore = 1 << 15,
    /// <summary>Combined paging options, including the IncludeMore option.</summary>
    PagingOptions = Skip | Take | ExcludeTotalCount | IncludeMore,
    /// <summary>The handler calculates Aggregates from all the rows the request matches.</summary>
    Aggregates = 1 << 16,
    /// <summary>The handler calculates the aggregates of the groups of GroupBy.</summary>
    GroupBy = 1 << 17,
    /// <summary>The handler honors ExcludeEntities.</summary>
    ExcludeEntities = 1 << 18,
    /// <summary>The handler groups the dates by the days of the TimeZone of the request.</summary>
    TimeZone = 1 << 19,
    /// <summary>
    /// Combined aggregate, group, entity exclusion and time zone options. Only the entity exclusion is in
    /// <see cref="ListHandler"/>, and all of them are in <see cref="ProListHandler"/>.
    /// </summary>
    AggregateOptions = Aggregates | GroupBy | ExcludeEntities | TimeZone,
    /// <summary>Capabilities supported by the in-memory list request source adapter by default.</summary>
    SourceAdapter = FilteringOptions | Sort | PagingOptions | DistinctFields | AggregateOptions,
    /// <summary>
    /// Fixed set of baseline capabilities. This value will not be expanded with future
    /// capabilities added to <c>ListRequest</c> or <c>ListRequestHandler</c> implementations.
    /// </summary>
    Baseline = ContainsText | ContainsField | EqualityFilter | Criteria | Sort | Skip | Take |
        IncludeDeleted | ColumnSelection | IncludeColumns | ExcludeColumns | DistinctFields |
        ExportColumns | Localize | ExcludeTotalCount | IncludeMore | ExcludeEntities,
    /// <summary>
    /// The capabilities of <c>ListRequestHandler</c> itself, without the behaviors of Serenity.Pro.Extensions.
    /// Currently equivalent to <see cref="Baseline"/>. May include capabilities added to it in the future.
    /// </summary>
    ListHandler = Baseline,
    /// <summary>
    /// The capabilities of <c>ListRequestHandler</c> with the behaviors of Serenity.Pro.Extensions, which are the ones
    /// of <see cref="ListHandler"/> and <see cref="AggregateOptions"/>. May include capabilities added to them in the future.
    /// </summary>
    ProListHandler = ListHandler | AggregateOptions,
    /// <summary>
    /// All the capabilities, which are currently the ones of <see cref="ProListHandler"/>. May include capabilities
    /// added to <c>ListRequest</c> and <c>ListRequestHandler</c> implementations in the future.
    /// </summary>
    All = ProListHandler
}
