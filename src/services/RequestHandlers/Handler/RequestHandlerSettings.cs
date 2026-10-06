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
    /// Gets or sets the maximum page size (take) allowed for List requests. When greater than
    /// zero, a requested page size larger than this is clamped to this value, and an unspecified
    /// page size is also limited to this value. The response returns the applied take. Zero means
    /// no limit.
    /// </summary>
    /// <remarks>
    /// WARNING: This has the same risks as <see cref="DefaultPageSize"/> (truncation of server
    /// side enumerations and grids without a pager, and an added COUNT(*) on every list
    /// request). Server side callers that need all rows must call
    /// <see cref="RequestHandlerExtensions.SuppressPagingLimits{TRequest}"/>. Do not enable
    /// this without auditing existing List usages, and avoid low values.
    /// </remarks>
    public int MaxPageSize { get; set; }

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

    /// <summary>
    /// Applies these settings to a requested skip / take pair. A zero take is replaced by
    /// <see cref="DefaultPageSize"/>, then clamped to <see cref="MaxPageSize"/> when set.
    /// Paging limits are bypassed when <paramref name="suppressPagingLimits"/> is true.
    /// This method does not validate negative values; callers should validate them according
    /// to their own request contract before calling it.
    /// </summary>
    /// <param name="skip">Requested number of records to skip.</param>
    /// <param name="take">Requested number of records to take.</param>
    /// <param name="suppressPagingLimits">True to bypass configured paging limits.</param>
    /// <returns>The effective skip / take values.</returns>
    public (int Skip, int Take) GetEffectiveSkipTake(
        int skip, int take, bool suppressPagingLimits = false)
    {
        if (suppressPagingLimits)
            return (skip, take);

        if (take == 0)
            take = DefaultPageSize;

        if (MaxPageSize > 0 && (take == 0 || take > MaxPageSize))
            take = MaxPageSize;

        return (skip, take);
    }
}
