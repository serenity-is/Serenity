namespace Serenity.Services;

/// <summary>
/// An asynchronous undelete behavior that can be used as a mixin within a UndeleteRequestHandlerAsync lifecycle
/// </summary>
/// <remarks>
/// A undelete behavior instance is always cached and reused across requests, so make 
/// sure you don't store anything in private variables, and its operation 
/// is thread-safe. If you need to pass some state between events, 
/// use handler's StateBag.
/// </remarks>
public interface IUndeleteBehaviorAsync : IUndeleteBehavior
{
    /// <summary>Called when query to load old entity is built</summary>
    /// <param name="handler">Calling undelete request handler</param>
    /// <param name="query">Query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task OnPrepareQueryAsync(IUndeleteRequestHandler handler, SqlQuery query, CancellationToken cancellationToken = default);

    /// <summary>Called when undelete request is validated</summary>
    /// <param name="handler">Calling undelete request handler</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task OnValidateRequestAsync(IUndeleteRequestHandler handler, CancellationToken cancellationToken = default);

    /// <summary>Called just before row is undeleted in database</summary>
    /// <param name="handler">Calling undelete request handler</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task OnBeforeUndeleteAsync(IUndeleteRequestHandler handler, CancellationToken cancellationToken = default);

    /// <summary>Called after row is undeleted in database</summary>
    /// <param name="handler">Calling undelete request handler</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task OnAfterUndeleteAsync(IUndeleteRequestHandler handler, CancellationToken cancellationToken = default);

    /// <summary>Called after row is undeleted and auditing should be performed</summary>
    /// <param name="handler">Calling undelete request handler</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task OnAuditAsync(IUndeleteRequestHandler handler, CancellationToken cancellationToken = default);

    /// <summary>Called before handler is returning the result</summary>
    /// <param name="handler">Calling undelete request handler</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task OnReturnAsync(IUndeleteRequestHandler handler, CancellationToken cancellationToken = default);
}
