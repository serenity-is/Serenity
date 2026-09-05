namespace Serenity.Services;

/// <summary>
/// Wraps an <see cref="ISaveBehaviorAsync"/> implementation and exposes it as an
/// <see cref="ISaveBehaviorSync"/> by blocking on its async methods. This allows
/// synchronous save request handlers to run asynchronous save behaviors.
/// </summary>
/// <remarks>
/// A behavior instance is always cached and reused across requests, so make 
/// sure you don't store anything in private variables, and its operation 
/// is thread-safe. If you need to pass some state between events, 
/// use handler's StateBag.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the class.
/// </remarks>
/// <param name="asyncBehavior">Asynchronous save behavior to wrap</param>
/// <exception cref="ArgumentNullException"><paramref name="asyncBehavior"/> is <c>null</c>.</exception>
public class AsyncToSyncSaveBehaviorWrapper(ISaveBehaviorAsync asyncBehavior) : ISaveBehaviorSync, IWrappedBehavior
{
    private readonly ISaveBehaviorAsync asyncBehavior = asyncBehavior ?? throw new ArgumentNullException(nameof(asyncBehavior));

    /// <inheritdoc/>
    public void OnPrepareQuery(ISaveRequestHandler handler, SqlQuery query)
    {
        asyncBehavior.OnPrepareQueryAsync(handler, query).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public void OnValidateRequest(ISaveRequestHandler handler)
    {
        asyncBehavior.OnValidateRequestAsync(handler).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public void OnSetInternalFields(ISaveRequestHandler handler)
    {
        asyncBehavior.OnSetInternalFieldsAsync(handler).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public void OnBeforeSave(ISaveRequestHandler handler)
    {
        asyncBehavior.OnBeforeSaveAsync(handler).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public void OnAfterSave(ISaveRequestHandler handler)
    {
        asyncBehavior.OnAfterSaveAsync(handler).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public void OnAudit(ISaveRequestHandler handler)
    {
        asyncBehavior.OnAuditAsync(handler).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public void OnReturn(ISaveRequestHandler handler)
    {
        asyncBehavior.OnReturnAsync(handler).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public object WrappedBehavior => asyncBehavior;
}
