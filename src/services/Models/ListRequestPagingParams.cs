namespace Serenity.Services;

/// <summary>
/// Parameters that describe how a list query should be paged, including whether the
/// total record count is required. Used by <see cref="ServiceQueryHelper.ApplyPagingParams"/>.
/// </summary>
public sealed class ListRequestPagingParams
{
    /// <summary>Gets the number of records to skip.</summary>
    public required int Skip { get; init; }

    /// <summary>Gets or sets the number of records to take. Zero means no paging.</summary>
    public required int Take { get; set; }

    /// <summary>
    /// Gets or sets the requested <see cref="ListRequest.ExcludeTotalCount"/> value.
    /// <c>true</c> excludes the total count, <c>false</c> requests it, and <c>null</c>
    /// (the default, unspecified) leaves the decision to <see cref="IncludeMore"/> and
    /// <see cref="ExcludeTotalCountByDefault"/>.
    /// </summary>
    public bool? ExcludeTotalCount { get; set; }

    /// <summary>
    /// Gets or sets whether a More indicator was requested. When the total count is not
    /// explicitly requested, this asks for an extra sentinel row instead of a <c>COUNT(*)</c>.
    /// </summary>
    public bool IncludeMore { get; set; }

    /// <summary>
    /// Gets or sets whether an unspecified <see cref="ExcludeTotalCount"/> should be treated
    /// as excluded.
    /// </summary>
    public bool ExcludeTotalCountByDefault { get; set; }

    /// <summary>
    /// Gets whether the total count should be excluded, resolving
    /// <see cref="ExcludeTotalCount"/> (an explicit value wins) against
    /// <see cref="IncludeMore"/> and <see cref="ExcludeTotalCountByDefault"/>.
    /// </summary>
    public bool ShouldExcludeTotalCount => ExcludeTotalCount == true ||
        (ExcludeTotalCount is null && (IncludeMore || ExcludeTotalCountByDefault));
}
