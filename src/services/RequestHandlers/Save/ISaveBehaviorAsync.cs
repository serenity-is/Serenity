namespace Serenity.Services;

/// <summary>
/// An asynchronous save behavior that can be used as a mixin within a SaveRequestHandlerAsync lifecycle
/// </summary>
/// <remarks>
/// A save behavior instance is always cached and reused across requests, so make 
/// sure you don't store anything in private variables, and its operation 
/// is thread-safe. If you need to pass some state between events, 
/// use handler's StateBag.
/// </remarks>
public interface ISaveBehaviorAsync : ISaveBehavior
{
    /// <summary>Called when query to load old entity is built</summary>
    /// <param name="handler">Calling save request handler</param>
    /// <param name="query">Query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task OnPrepareQueryAsync(ISaveRequestHandler handler, SqlQuery query, CancellationToken cancellationToken = default);

    /// <summary>Called when save request is validated</summary>
    /// <param name="handler">Calling save request handler</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task OnValidateRequestAsync(ISaveRequestHandler handler, CancellationToken cancellationToken = default);

    /// <summary>Called when internal fields in row is being set</summary>
    /// <param name="handler">Calling save request handler</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task OnSetInternalFieldsAsync(ISaveRequestHandler handler, CancellationToken cancellationToken = default);

    /// <summary>Called just before row is inserted to / updated in database</summary>
    /// <param name="handler">Calling save request handler</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task OnBeforeSaveAsync(ISaveRequestHandler handler, CancellationToken cancellationToken = default);

    /// <summary>Called after row is inserted to / updated in database</summary>
    /// <param name="handler">Calling save request handler</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task OnAfterSaveAsync(ISaveRequestHandler handler, CancellationToken cancellationToken = default);

    /// <summary>Called after row is inserted to / updated and auditing should be performed</summary>
    /// <param name="handler">Calling save request handler</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task OnAuditAsync(ISaveRequestHandler handler, CancellationToken cancellationToken = default);

    /// <summary>Called before handler is returning the result</summary>
    /// <param name="handler">Calling save request handler</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task OnReturnAsync(ISaveRequestHandler handler, CancellationToken cancellationToken = default);
}
