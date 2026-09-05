namespace Serenity.Services;

/// <summary>
/// Wraps an <see cref="IListBehaviorSync"/> implementation and exposes it as an
/// <see cref="IListBehaviorAsync"/>. This allows asynchronous list request handlers
/// to run synchronous list behaviors.
/// </summary>
/// <remarks>
/// A behavior instance is always cached and reused across requests, so make 
/// sure you don't store anything in private variables, and its operation 
/// is thread-safe. If you need to pass some state between events, 
/// use handler's StateBag.
/// </remarks>
public class SyncToAsyncListBehaviorWrapper(IListBehaviorSync syncBehavior) : IListBehaviorAsync, IWrappedBehavior
{
    private readonly IListBehaviorSync syncBehavior = syncBehavior ?? throw new ArgumentNullException(nameof(syncBehavior));

    /// <inheritdoc/>
    public Task OnValidateRequestAsync(IListRequestHandler handler, CancellationToken cancellationToken = default)
    {
        syncBehavior.OnValidateRequest(handler);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task OnPrepareQueryAsync(IListRequestHandler handler, SqlQuery query, CancellationToken cancellationToken = default)
    {
        syncBehavior.OnPrepareQuery(handler, query);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task OnApplyFiltersAsync(IListRequestHandler handler, SqlQuery query, CancellationToken cancellationToken = default)
    {
        syncBehavior.OnApplyFilters(handler, query);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task OnBeforeExecuteQueryAsync(IListRequestHandler handler, CancellationToken cancellationToken = default)
    {
        syncBehavior.OnBeforeExecuteQuery(handler);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task OnAfterExecuteQueryAsync(IListRequestHandler handler, CancellationToken cancellationToken = default)
    {
        syncBehavior.OnAfterExecuteQuery(handler);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task OnReturnAsync(IListRequestHandler handler, CancellationToken cancellationToken = default)
    {
        syncBehavior.OnReturn(handler);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public object WrappedBehavior => syncBehavior;
}
