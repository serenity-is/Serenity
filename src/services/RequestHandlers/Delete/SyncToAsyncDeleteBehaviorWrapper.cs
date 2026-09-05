namespace Serenity.Services;

/// <summary>
/// Wraps an <see cref="IDeleteBehaviorSync"/> implementation and exposes it as an
/// <see cref="IDeleteBehaviorAsync"/>. This allows asynchronous delete request handlers
/// to run synchronous delete behaviors.
/// </summary>
/// <remarks>
/// A behavior instance is always cached and reused across requests, so make 
/// sure you don't store anything in private variables, and its operation 
/// is thread-safe. If you need to pass some state between events, 
/// use handler's StateBag.
/// </remarks>
public class SyncToAsyncDeleteBehaviorWrapper(IDeleteBehaviorSync syncBehavior) : IDeleteBehaviorAsync, IWrappedBehavior
{
    private readonly IDeleteBehaviorSync syncBehavior = syncBehavior ?? throw new ArgumentNullException(nameof(syncBehavior));

    /// <inheritdoc/>
    public Task OnPrepareQueryAsync(IDeleteRequestHandler handler, SqlQuery query, CancellationToken cancellationToken = default)
    {
        syncBehavior.OnPrepareQuery(handler, query);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task OnValidateRequestAsync(IDeleteRequestHandler handler, CancellationToken cancellationToken = default)
    {
        syncBehavior.OnValidateRequest(handler);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task OnBeforeDeleteAsync(IDeleteRequestHandler handler, CancellationToken cancellationToken = default)
    {
        syncBehavior.OnBeforeDelete(handler);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task OnAfterDeleteAsync(IDeleteRequestHandler handler, CancellationToken cancellationToken = default)
    {
        syncBehavior.OnAfterDelete(handler);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task OnAuditAsync(IDeleteRequestHandler handler, CancellationToken cancellationToken = default)
    {
        syncBehavior.OnAudit(handler);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task OnReturnAsync(IDeleteRequestHandler handler, CancellationToken cancellationToken = default)
    {
        syncBehavior.OnReturn(handler);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public object WrappedBehavior => syncBehavior;
}
