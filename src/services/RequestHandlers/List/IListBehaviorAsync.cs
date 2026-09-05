namespace Serenity.Services;

/// <summary>
/// An asynchronous list behavior that can be used as a mixin within a ListRequestHandlerAsync lifecycle
/// </summary>
/// <remarks>
/// A list behavior instance is always cached and reused across requests, so make 
/// sure you don't store anything in private variables, and its operation 
/// is thread-safe. If you need to pass some state between events, 
/// use handler's StateBag.
/// </remarks>
public interface IListBehaviorAsync : IListBehavior
{
    /// <summary>Called when list request is validated</summary>
    /// <param name="handler">Calling list request handler</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task OnValidateRequestAsync(IListRequestHandler handler, CancellationToken cancellationToken = default) => Task.CompletedTask;

    /// <summary>Called when query is built</summary>
    /// <param name="handler">Calling list request handler</param>
    /// <param name="query">Query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task OnPrepareQueryAsync(IListRequestHandler handler, SqlQuery query, CancellationToken cancellationToken = default) => Task.CompletedTask;

    /// <summary>Called when filters are applied to query</summary>
    /// <param name="handler">Calling list request handler</param>
    /// <param name="query">Query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task OnApplyFiltersAsync(IListRequestHandler handler, SqlQuery query, CancellationToken cancellationToken = default) => Task.CompletedTask;

    /// <summary>Called just before query is sent to database</summary>
    /// <param name="handler">Calling list request handler</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task OnBeforeExecuteQueryAsync(IListRequestHandler handler, CancellationToken cancellationToken = default) => Task.CompletedTask;

    /// <summary>Called after query is sent to database</summary>
    /// <param name="handler">Calling list request handler</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task OnAfterExecuteQueryAsync(IListRequestHandler handler, CancellationToken cancellationToken = default) => Task.CompletedTask;

    /// <summary>Called before handler is returning the result</summary>
    /// <param name="handler">Calling list request handler</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task OnReturnAsync(IListRequestHandler handler, CancellationToken cancellationToken = default) => Task.CompletedTask;
}
