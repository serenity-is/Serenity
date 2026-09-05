namespace Serenity.Services;

/// <summary>
/// An asynchronous retrieve behavior that can be used as a mixin within a RetrieveRequestHandlerAsync lifecycle
/// </summary>
/// <remarks>
/// A retrieve behavior instance is always cached and reused across requests, so make 
/// sure you don't store anything in private variables, and its operation 
/// is thread-safe. If you need to pass some state between events, 
/// use handler's StateBag.
/// </remarks>
public interface IRetrieveBehaviorAsync : IRetrieveBehavior
{
    /// <summary>Called when retrieve request is validated</summary>
    /// <param name="handler">Calling retrieve request handler</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task OnValidateRequestAsync(IRetrieveRequestHandler handler, CancellationToken cancellationToken = default) => Task.CompletedTask;

    /// <summary>Called when query is built</summary>
    /// <param name="handler">Calling retrieve request handler</param>
    /// <param name="query">Query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task OnPrepareQueryAsync(IRetrieveRequestHandler handler, SqlQuery query, CancellationToken cancellationToken = default) => Task.CompletedTask;

    /// <summary>Called just before query is sent to database</summary>
    /// <param name="handler">Calling retrieve request handler</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task OnBeforeExecuteQueryAsync(IRetrieveRequestHandler handler, CancellationToken cancellationToken = default) => Task.CompletedTask;

    /// <summary>Called after query is sent to database</summary>
    /// <param name="handler">Calling retrieve request handler</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task OnAfterExecuteQueryAsync(IRetrieveRequestHandler handler, CancellationToken cancellationToken = default) => Task.CompletedTask;

    /// <summary>Called before handler is returning the result</summary>
    /// <param name="handler">Calling retrieve request handler</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task OnReturnAsync(IRetrieveRequestHandler handler, CancellationToken cancellationToken = default) => Task.CompletedTask;
}
