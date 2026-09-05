namespace Serenity.Services;

/// <summary>
/// Wraps an <see cref="IRetrieveBehaviorAsync"/> implementation and exposes it as an
/// <see cref="IRetrieveBehaviorSync"/> by blocking on its async methods. This allows
/// synchronous retrieve request handlers to run asynchronous retrieve behaviors.
/// </summary>
/// <remarks>
/// A behavior instance is always cached and reused across requests, so make 
/// sure you don't store anything in private variables, and its operation 
/// is thread-safe. If you need to pass some state between events, 
/// use handler's StateBag.
/// </remarks>
public class AsyncToSyncRetrieveBehaviorWrapper(IRetrieveBehaviorAsync asyncBehavior) : IRetrieveBehaviorSync, IWrappedBehavior
{
    private readonly IRetrieveBehaviorAsync asyncBehavior = asyncBehavior ?? throw new ArgumentNullException(nameof(asyncBehavior));

    /// <inheritdoc/>
    public void OnValidateRequest(IRetrieveRequestHandler handler)
    {
        asyncBehavior.OnValidateRequestAsync(handler).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public void OnPrepareQuery(IRetrieveRequestHandler handler, SqlQuery query)
    {
        asyncBehavior.OnPrepareQueryAsync(handler, query).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public void OnBeforeExecuteQuery(IRetrieveRequestHandler handler)
    {
        asyncBehavior.OnBeforeExecuteQueryAsync(handler).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public void OnAfterExecuteQuery(IRetrieveRequestHandler handler)
    {
        asyncBehavior.OnAfterExecuteQueryAsync(handler).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public void OnReturn(IRetrieveRequestHandler handler)
    {
        asyncBehavior.OnReturnAsync(handler).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public object WrappedBehavior => asyncBehavior;
}
