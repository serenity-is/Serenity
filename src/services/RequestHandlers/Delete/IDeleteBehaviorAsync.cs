namespace Serenity.Services;

/// <summary>
/// An asynchronous delete behavior that can be used as a mixin within a DeleteRequestHandlerAsync lifecycle
/// </summary>
/// <remarks>
/// A delete behavior instance is always cached and reused across requests, so make 
/// sure you don't store anything in private variables, and its operation 
/// is thread-safe. If you need to pass some state between events, 
/// use handler's StateBag.
/// </remarks>
public interface IDeleteBehaviorAsync : IDeleteBehavior
{
    /// <summary>Called when query to load old entity is built</summary>
    /// <param name="handler">Calling delete request handler</param>
    /// <param name="query">Query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task OnPrepareQueryAsync(IDeleteRequestHandler handler, SqlQuery query, CancellationToken cancellationToken = default);

    /// <summary>Called when delete request is validated</summary>
    /// <param name="handler">Calling delete request handler</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task OnValidateRequestAsync(IDeleteRequestHandler handler, CancellationToken cancellationToken = default);

    /// <summary>Called just before row is deleted from database</summary>
    /// <param name="handler">Calling delete request handler</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task OnBeforeDeleteAsync(IDeleteRequestHandler handler, CancellationToken cancellationToken = default);

    /// <summary>Called after row is deleted from database</summary>
    /// <param name="handler">Calling delete request handler</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task OnAfterDeleteAsync(IDeleteRequestHandler handler, CancellationToken cancellationToken = default);

    /// <summary>Called after row is deleted and auditing should be performed</summary>
    /// <param name="handler">Calling delete request handler</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task OnAuditAsync(IDeleteRequestHandler handler, CancellationToken cancellationToken = default);

    /// <summary>Called before handler is returning the result</summary>
    /// <param name="handler">Calling delete request handler</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task OnReturnAsync(IDeleteRequestHandler handler, CancellationToken cancellationToken = default);
}
