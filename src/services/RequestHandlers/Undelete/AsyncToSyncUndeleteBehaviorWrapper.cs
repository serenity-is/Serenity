namespace Serenity.Services;

/// <summary>
/// Wraps an <see cref="IUndeleteBehaviorAsync"/> implementation and exposes it as an
/// <see cref="IUndeleteBehaviorSync"/> by blocking on its async methods. This allows
/// synchronous undelete request handlers to run asynchronous undelete behaviors.
/// </summary>
/// <remarks>
/// A behavior instance is always cached and reused across requests, so make 
/// sure you don't store anything in private variables, and its operation 
/// is thread-safe. If you need to pass some state between events, 
/// use handler's StateBag.
/// </remarks>
/// <remarks>This is a bridging adapter that blocks on the wrapped asynchronous
/// behavior (sync-over-async). The synchronous interface exposes no
/// <see cref="System.Threading.CancellationToken"/>, so a cancellation token from
/// an asynchronous call site cannot be observed by the wrapped behavior. Prefer
/// invoking the asynchronous behavior directly whenever a cancellation token is
/// available.</remarks>
public class AsyncToSyncUndeleteBehaviorWrapper(IUndeleteBehaviorAsync asyncBehavior) : IUndeleteBehaviorSync, IWrappedBehavior
{
    private readonly IUndeleteBehaviorAsync asyncBehavior = asyncBehavior ?? throw new ArgumentNullException(nameof(asyncBehavior));

    /// <inheritdoc/>
    public void OnPrepareQuery(IUndeleteRequestHandler handler, SqlQuery query)
    {
        asyncBehavior.OnPrepareQueryAsync(handler, query).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public void OnValidateRequest(IUndeleteRequestHandler handler)
    {
        asyncBehavior.OnValidateRequestAsync(handler).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public void OnBeforeUndelete(IUndeleteRequestHandler handler)
    {
        asyncBehavior.OnBeforeUndeleteAsync(handler).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public void OnAfterUndelete(IUndeleteRequestHandler handler)
    {
        asyncBehavior.OnAfterUndeleteAsync(handler).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public void OnAudit(IUndeleteRequestHandler handler)
    {
        asyncBehavior.OnAuditAsync(handler).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public void OnReturn(IUndeleteRequestHandler handler)
    {
        asyncBehavior.OnReturnAsync(handler).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public object WrappedBehavior => asyncBehavior;
}
