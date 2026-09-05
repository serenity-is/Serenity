namespace Serenity.Services;

/// <summary>
/// Wraps an <see cref="ISaveBehaviorSync"/> implementation and exposes it as an
/// <see cref="ISaveBehaviorAsync"/>. This allows asynchronous save request handlers
/// to run synchronous save behaviors.
/// </summary>
/// <remarks>
/// A behavior instance is always cached and reused across requests, so make 
/// sure you don't store anything in private variables, and its operation 
/// is thread-safe. If you need to pass some state between events, 
/// use handler's StateBag.
/// </remarks>
public class SyncToAsyncSaveBehaviorWrapper : ISaveBehaviorAsync, IWrappedBehavior
{
    private readonly ISaveBehaviorSync syncBehavior;

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="syncBehavior">Synchronous save behavior to wrap</param>
    /// <exception cref="ArgumentNullException"><paramref name="syncBehavior"/> is <c>null</c>.</exception>
    public SyncToAsyncSaveBehaviorWrapper(ISaveBehaviorSync syncBehavior) => this.syncBehavior = syncBehavior ?? throw new ArgumentNullException(nameof(syncBehavior));

    /// <inheritdoc/>
    public Task OnPrepareQueryAsync(ISaveRequestHandler handler, SqlQuery query, CancellationToken cancellationToken = default)
    {
        syncBehavior.OnPrepareQuery(handler, query);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task OnValidateRequestAsync(ISaveRequestHandler handler, CancellationToken cancellationToken = default)
    {
        syncBehavior.OnValidateRequest(handler);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task OnSetInternalFieldsAsync(ISaveRequestHandler handler, CancellationToken cancellationToken = default)
    {
        syncBehavior.OnSetInternalFields(handler);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task OnBeforeSaveAsync(ISaveRequestHandler handler, CancellationToken cancellationToken = default)
    {
        syncBehavior.OnBeforeSave(handler);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task OnAfterSaveAsync(ISaveRequestHandler handler, CancellationToken cancellationToken = default)
    {
        syncBehavior.OnAfterSave(handler);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task OnAuditAsync(ISaveRequestHandler handler, CancellationToken cancellationToken = default)
    {
        syncBehavior.OnAudit(handler);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task OnReturnAsync(ISaveRequestHandler handler, CancellationToken cancellationToken = default)
    {
        syncBehavior.OnReturn(handler);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public object WrappedBehavior => syncBehavior;
}
