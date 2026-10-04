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
    /// <summary>
    /// Fixed set of baseline capabilities. This value will not be expanded with future
    /// capabilities added to <c>ListRequest</c> or <c>ListRequestHandler</c> implementations.
    /// </summary>
    Baseline = ContainsText | ContainsField | EqualityFilter | Criteria | Sort | Skip | Take |
        IncludeDeleted | ColumnSelection | IncludeColumns | ExcludeColumns | DistinctFields |
        ExportColumns | Localize | ExcludeTotalCount,
    /// <summary>
    /// Currently equivalent to <see cref="Baseline"/>. May include capabilities
    /// added to <c>ListRequest</c> and <c>ListRequestHandler</c> implementations in the future.
    /// </summary>
    All = Baseline
}
