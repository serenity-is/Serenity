namespace Serenity.Services;

/// <summary>
/// A delete behavior that can be used as a mixin within a DeleteRequestHandler lifecycle
/// </summary>
/// <remarks>
/// A behavior instance is always cached and reused across requests, so make 
/// sure you don't store anything in private variables, and its operation 
/// is thread-safe. If you need to pass some state between events, 
/// use handler's StateBag.
/// </remarks>
public interface IDeleteBehavior
{
    /// <summary>Called when query to load old entity is built</summary>
    /// <param name="handler">Calling save request handler</param>
    /// <param name="query">The query</param>
    void OnPrepareQuery(IDeleteRequestHandler handler, SqlQuery query);

    /// <summary>Called when delete request is validated</summary>
    /// <param name="handler">Calling delete request handler</param>
    void OnValidateRequest(IDeleteRequestHandler handler);

    /// <summary>Called just before row is inserted to / updated in database</summary>
    /// <param name="handler">Calling delete request handler</param>
    void OnBeforeDelete(IDeleteRequestHandler handler);

    /// <summary>Called after row is inserted to / updated in database</summary>
    /// <param name="handler">Calling delete request handler</param>
    void OnAfterDelete(IDeleteRequestHandler handler);

    /// <summary>Called after row is inserted to / updated and auditing should be performed</summary>
    /// <param name="handler">Calling delete request handler</param>
    void OnAudit(IDeleteRequestHandler handler);

    /// <summary>Called before handler is returning the result</summary>
    /// <param name="handler">Calling delete request handler</param>
    void OnReturn(IDeleteRequestHandler handler);
}