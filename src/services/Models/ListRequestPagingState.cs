namespace Serenity.Services;

/// <summary>
/// Describes the paging strategy selected for a list request.
/// </summary>
public sealed class ListRequestPagingState
{
    internal ListRequestPagingState(int requestedTake, int appliedTake,
        bool includeMore, bool usesSentinel)
    {
        RequestedTake = requestedTake;
        AppliedTake = appliedTake;
        IncludeMore = includeMore;
        UsesSentinel = usesSentinel;
    }

    /// <summary>Gets the requested page size before an optional sentinel row.</summary>
    public int RequestedTake { get; }

    /// <summary>Gets the query take value when this paging state was created.</summary>
    public int AppliedTake { get; }

    /// <summary>Gets whether the request asked for a More indicator.</summary>
    public bool IncludeMore { get; }

    /// <summary>Gets whether an extra row was requested as a sentinel.</summary>
    public bool UsesSentinel { get; }
}