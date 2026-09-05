namespace Serenity.Services;

/// <summary>
/// Wraps an <see cref="IUndeleteBehaviorSync"/> implementation and exposes it as an
/// <see cref="IUndeleteBehaviorAsync"/>. This allows asynchronous undelete request handlers
/// to run synchronous undelete behaviors.
/// </summary>
/// <remarks>
/// A behavior instance is always cached and reused across requests, so make 
/// sure you don't store anything in private variables, and its operation 
/// is thread-safe. If you need to pass some state between events, 
/// use handler's StateBag.
/// </remarks>
public class SyncToAsyncUndeleteBehaviorWrapper(IUndeleteBehaviorSync syncBehavior) : IUndeleteBehaviorAsync, IWrappedBehavior
{
    private readonly IUndeleteBehaviorSync syncBehavior = syncBehavior ?? throw new ArgumentNullException(nameof(syncBehavior));

    /// <inheritdoc/>
    public Task OnPrepareQueryAsync(IUndeleteRequestHandler handler, SqlQuery query, CancellationToken cancellationToken = default)
    {
        syncBehavior.OnPrepareQuery(handler, query);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task OnValidateRequestAsync(IUndeleteRequestHandler handler, CancellationToken cancellationToken = default)
    {
        syncBehavior.OnValidateRequest(handler);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task OnBeforeUndeleteAsync(IUndeleteRequestHandler handler, CancellationToken cancellationToken = default)
    {
        syncBehavior.OnBeforeUndelete(handler);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task OnAfterUndeleteAsync(IUndeleteRequestHandler handler, CancellationToken cancellationToken = default)
    {
        syncBehavior.OnAfterUndelete(handler);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task OnAuditAsync(IUndeleteRequestHandler handler, CancellationToken cancellationToken = default)
    {
        syncBehavior.OnAudit(handler);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task OnReturnAsync(IUndeleteRequestHandler handler, CancellationToken cancellationToken = default)
    {
        syncBehavior.OnReturn(handler);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public object WrappedBehavior => syncBehavior;
}
