namespace Serenity.Services;

/// <summary>
/// Wraps an <see cref="IListBehaviorAsync"/> implementation and exposes it as an
/// <see cref="IListBehaviorSync"/> by blocking on its async methods. This allows
/// synchronous list request handlers to run asynchronous list behaviors.
/// </summary>
/// <remarks>
/// A behavior instance is always cached and reused across requests, so make 
/// sure you don't store anything in private variables, and its operation 
/// is thread-safe. If you need to pass some state between events, 
/// use handler's StateBag.
/// </remarks>
public class AsyncToSyncListBehaviorWrapper(IListBehaviorAsync asyncBehavior) : IListBehaviorSync, IWrappedBehavior
{
    private readonly IListBehaviorAsync asyncBehavior = asyncBehavior ?? throw new ArgumentNullException(nameof(asyncBehavior));

    /// <inheritdoc/>
    public void OnValidateRequest(IListRequestHandler handler)
    {
        asyncBehavior.OnValidateRequestAsync(handler).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public void OnPrepareQuery(IListRequestHandler handler, SqlQuery query)
    {
        asyncBehavior.OnPrepareQueryAsync(handler, query).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public void OnApplyFilters(IListRequestHandler handler, SqlQuery query)
    {
        asyncBehavior.OnApplyFiltersAsync(handler, query).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public void OnBeforeExecuteQuery(IListRequestHandler handler)
    {
        asyncBehavior.OnBeforeExecuteQueryAsync(handler).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public void OnAfterExecuteQuery(IListRequestHandler handler)
    {
        asyncBehavior.OnAfterExecuteQueryAsync(handler).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public void OnReturn(IListRequestHandler handler)
    {
        asyncBehavior.OnReturnAsync(handler).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public object WrappedBehavior => asyncBehavior;
}
