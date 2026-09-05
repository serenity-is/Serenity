namespace Serenity.Services;

/// <summary>
/// Wraps an <see cref="IDeleteBehaviorAsync"/> implementation and exposes it as an
/// <see cref="IDeleteBehaviorSync"/> by blocking on its async methods. This allows
/// synchronous delete request handlers to run asynchronous delete behaviors.
/// </summary>
/// <remarks>
/// A behavior instance is always cached and reused across requests, so make 
/// sure you don't store anything in private variables, and its operation 
/// is thread-safe. If you need to pass some state between events, 
/// use handler's StateBag.
/// </remarks>
public class AsyncToSyncDeleteBehaviorWrapper(IDeleteBehaviorAsync asyncBehavior) : IDeleteBehaviorSync, IWrappedBehavior
{
    private readonly IDeleteBehaviorAsync asyncBehavior = asyncBehavior ?? throw new ArgumentNullException(nameof(asyncBehavior));

    /// <inheritdoc/>
    public void OnPrepareQuery(IDeleteRequestHandler handler, SqlQuery query)
    {
        asyncBehavior.OnPrepareQueryAsync(handler, query).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public void OnValidateRequest(IDeleteRequestHandler handler)
    {
        asyncBehavior.OnValidateRequestAsync(handler).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public void OnBeforeDelete(IDeleteRequestHandler handler)
    {
        asyncBehavior.OnBeforeDeleteAsync(handler).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public void OnAfterDelete(IDeleteRequestHandler handler)
    {
        asyncBehavior.OnAfterDeleteAsync(handler).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public void OnAudit(IDeleteRequestHandler handler)
    {
        asyncBehavior.OnAuditAsync(handler).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public void OnReturn(IDeleteRequestHandler handler)
    {
        asyncBehavior.OnReturnAsync(handler).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public object WrappedBehavior => asyncBehavior;
}
