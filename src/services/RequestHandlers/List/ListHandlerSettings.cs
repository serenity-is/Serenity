namespace Serenity.Services;

/// <summary>
/// Settings that control how List services apply paging (skip / take).
/// </summary>
public class ListHandlerSettings
{
    /// <summary>
    /// Gets the default settings (no page size limits).
    /// </summary>
    public static readonly ListHandlerSettings Default = new();

    /// <summary>
    /// Gets or sets the default page size (take) applied when the request does not specify one
    /// (e.g. take is zero). Zero means no default, so all records may be returned.
    /// </summary>
    /// <remarks>
    /// WARNING: Enabling this applies to every List request, including server side code that
    /// constructs a <see cref="ListRequest"/> without a take and expects all records (for
    /// example exports, reports, background jobs, or nested relation loads). Such callers may
    /// silently operate on a partial list; they must call
    /// <see cref="RequestHandlerExtensions.SuppressPagingLimits{TRequest}"/> to ignore the
    /// limits. Do not enable this without auditing existing List usages. Also, grids that do
    /// not render a pager (e.g. <c>usePager() =&gt; false</c>) will be truncated with no way to
    /// reach the remaining rows, so avoid low values. Enabling this (or <see cref="MaxPageSize"/>)
    /// also makes the effective take non-zero for every request, which — with the default
    /// <see cref="ListRequest.ExcludeTotalCount"/> of false — adds a COUNT(*) to every list
    /// request, including internal ones. Callers that don't need the total should set
    /// <see cref="ListRequest.ExcludeTotalCount"/> to true, or call
    /// <see cref="RequestHandlerExtensions.SuppressPagingLimits{TRequest}"/> (which also
    /// restores take to zero).
    /// </remarks>
    public int DefaultPageSize { get; set; }

    /// <summary>
    /// Gets or sets the maximum page size (take) allowed. When greater than zero, a requested
    /// page size larger than this is clamped to this value, and an unspecified page size is
    /// also limited to this value. The response returns the applied take. Zero means no limit.
    /// </summary>
    /// <remarks>
    /// WARNING: This has the same risks as <see cref="DefaultPageSize"/> (truncation of server
    /// side enumerations and grids without a pager, and an added COUNT(*) on every list
    /// request). Server side callers that need all rows must call
    /// <see cref="RequestHandlerExtensions.SuppressPagingLimits{TRequest}"/>. Do not enable
    /// this without auditing existing List usages, and avoid low values.
    /// </remarks>
    public int MaxPageSize { get; set; }
}
