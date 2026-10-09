using Microsoft.Extensions.Options;

namespace Serenity.Services;

/// <summary>
/// Contains settings that are applied to request handlers. Bound from the
/// <c>RequestHandlerSettings</c> section in appsettings.json when available.
/// </summary>
[DefaultSectionKey(SectionKey)]
public class RequestHandlerSettings : IOptions<RequestHandlerSettings>
{
    /// <summary>
    /// The default section key in appsettings.json
    /// </summary>
    public const string SectionKey = "RequestHandlerSettings";

    /// <summary>
    /// Gets the default settings (no limits).
    /// </summary>
    public static readonly RequestHandlerSettings Default = new();

    /// <summary>
    /// Gets or sets the default page size (take) applied when a List request does not specify
    /// one (e.g. take is zero). Zero means no default, so all records may be returned.
    /// </summary>
    /// <remarks>
    /// WARNING: Enabling this applies to every List request, including server side code that
    /// constructs a <see cref="ListRequest"/> without a take and expects all records (for
    /// example exports, reports, background jobs, or nested relation loads). Such callers may
    /// silently operate on a partial list; they must call
    /// <see cref="RequestHandlerExtensions.SuppressPagingLimits{TRequest}"/> to ignore the
    /// limits. Do not enable this without auditing existing List usages. Also, grids that do
    /// not render a pager (e.g. <c>usePager() =&gt; false</c>) will be truncated with no way to
    /// reach the remaining rows, so avoid low values. A take of zero that this setting (or
    /// <see cref="MaxPageSize"/>) turns into an actual page size still returns the total
    /// (a <c>COUNT(*)</c>) even when <see cref="ExcludeTotalCountByDefault"/> is enabled,
    /// because the caller did not ask to page. Callers that don't need the total should set
    /// <see cref="ListRequest.ExcludeTotalCount"/> to true, request
    /// <see cref="ListRequest.IncludeMore"/> (which returns a More indicator instead), or call
    /// <see cref="RequestHandlerExtensions.SuppressPagingLimits{TRequest}"/> (which also
    /// restores take to zero).
    /// </remarks>
    public int DefaultPageSize { get; set; }

    /// <summary>
    /// Gets or sets the maximum page size (take) allowed for List requests. When greater than
    /// zero, a requested page size larger than this is clamped to this value, and an unspecified
    /// page size is also limited to this value. The response returns the applied take. Zero means
    /// no limit.
    /// </summary>
    /// <remarks>
    /// WARNING: This has the same risks as <see cref="DefaultPageSize"/> (truncation of server
    /// side enumerations and grids without a pager, and an added total count whenever the take
    /// becomes non-zero, even when <see cref="ExcludeTotalCountByDefault"/> is enabled). Server
    /// side callers that need all rows must call
    /// <see cref="RequestHandlerExtensions.SuppressPagingLimits{TRequest}"/>. Do not enable
    /// this without auditing existing List usages, and avoid low values.
    /// </remarks>
    public int MaxPageSize { get; set; }

    /// <summary>
    /// Gets or sets whether the total record count is excluded by default for list requests
    /// that do not explicitly specify <see cref="ListRequest.ExcludeTotalCount"/> (i.e. it is
    /// <c>null</c> / not sent). Default is false, which preserves the historical behavior of
    /// calculating the total (and issuing a <c>COUNT(*)</c>) for paged requests. When a request
    /// has a zero take that <see cref="DefaultPageSize"/> / <see cref="MaxPageSize"/> turns into
    /// an actual page size, the total is still returned as if <see cref="ListRequest.ExcludeTotalCount"/>
    /// was false, unless <see cref="ListRequest.IncludeMore"/> was requested (a More indicator
    /// is returned instead).
    /// </summary>
    /// <remarks>
    /// WARNING: Enabling this changes the default behavior of every List request that does not
    /// explicitly send <see cref="ListRequest.ExcludeTotalCount"/>, including grid requests that
    /// rely on <see cref="ListResponse{T}.TotalCount"/> for their pager. Such grids will report
    /// a zero total until they explicitly send <c>ExcludeTotalCount = false</c>. Audit existing
    /// usages before enabling. Requests that explicitly send <c>ExcludeTotalCount = false</c> are
    /// unaffected, which is why the request property is nullable. Also note that requests with
    /// <see cref="ListRequest.IncludeMore"/> already skip the count.
    /// </remarks>
    public bool ExcludeTotalCountByDefault { get; set; }

    /// <summary>
    /// Gets this instance.
    /// </summary>
    public RequestHandlerSettings Value => this;

    /// <summary>
    /// Creates a copy of these settings with the specified changes applied, without modifying
    /// this instance. Useful for returning different settings per request / entity, e.g. a
    /// limited page size for public access and unlimited for administrators.
    /// </summary>
    /// <param name="configure">Action that modifies the copied settings.</param>
    /// <returns>The copied settings.</returns>
    public RequestHandlerSettings With(Action<RequestHandlerSettings> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var clone = (RequestHandlerSettings)MemberwiseClone();
        configure(clone);
        return clone;
    }
}
