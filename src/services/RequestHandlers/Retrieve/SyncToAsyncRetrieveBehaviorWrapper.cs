namespace Serenity.Services;

/// <summary>
/// Wraps an <see cref="IRetrieveBehaviorSync"/> implementation and exposes it as an
/// <see cref="IRetrieveBehaviorAsync"/>. This allows asynchronous retrieve request handlers
/// to run synchronous retrieve behaviors.
/// </summary>
/// <remarks>
/// A behavior instance is always cached and reused across requests, so make 
/// sure you don't store anything in private variables, and its operation 
/// is thread-safe. If you need to pass some state between events, 
/// use handler's StateBag.
/// </remarks>
public class SyncToAsyncRetrieveBehaviorWrapper(IRetrieveBehaviorSync syncBehavior) : IRetrieveBehaviorAsync, IWrappedBehavior
{
    private readonly IRetrieveBehaviorSync syncBehavior = syncBehavior ?? throw new ArgumentNullException(nameof(syncBehavior));

    /// <inheritdoc/>
    public Task OnValidateRequestAsync(IRetrieveRequestHandler handler, CancellationToken cancellationToken = default)
    {
        syncBehavior.OnValidateRequest(handler);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task OnPrepareQueryAsync(IRetrieveRequestHandler handler, SqlQuery query, CancellationToken cancellationToken = default)
    {
        syncBehavior.OnPrepareQuery(handler, query);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task OnBeforeExecuteQueryAsync(IRetrieveRequestHandler handler, CancellationToken cancellationToken = default)
    {
        syncBehavior.OnBeforeExecuteQuery(handler);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task OnAfterExecuteQueryAsync(IRetrieveRequestHandler handler, CancellationToken cancellationToken = default)
    {
        syncBehavior.OnAfterExecuteQuery(handler);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task OnReturnAsync(IRetrieveRequestHandler handler, CancellationToken cancellationToken = default)
    {
        syncBehavior.OnReturn(handler);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public object WrappedBehavior => syncBehavior;
}
