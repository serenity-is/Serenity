namespace Serenity.Services;

/// <summary>
/// Describes the paging strategy selected for a list request, as returned by
/// <see cref="ServiceQueryHelper.ApplyPagingParams"/>.
/// </summary>
public sealed class ListRequestPagingState
{
    internal ListRequestPagingState(ListRequestPagingParams paging, int appliedTake, bool usesSentinel)
    {
        Params = paging;
        AppliedTake = appliedTake;
        UsesSentinel = usesSentinel;
    }

    /// <summary>Gets the paging parameters the query was applied with.</summary>
    public ListRequestPagingParams Params { get; }

    /// <summary>Gets the query take value when this paging state was created.</summary>
    public int AppliedTake { get; }

    /// <summary>Gets whether an extra row was requested as a sentinel.</summary>
    public bool UsesSentinel { get; }
}
