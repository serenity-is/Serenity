namespace Serenity.Services;

/// <summary>
/// A undelete behavior that can be used as a mixin within a UndeleteRequestHandler lifecycle
/// </summary>
/// <remarks>
/// A behavior instance is always cached and reused across requests, so make 
/// sure you don't store anything in private variables, and its operation 
/// is thread-safe. If you need to pass some state between events, 
/// use handler's StateBag.
/// </remarks>
public interface IUndeleteBehavior
{
    /// <summary>Called when query to load old entity is built</summary>
    /// <param name="handler">Calling save request handler</param>
    /// <param name="query">The query</param>
    void OnPrepareQuery(IUndeleteRequestHandler handler, SqlQuery query);

    /// <summary>Called when undelete request is validated</summary>
    /// <param name="handler">Calling undelete request handler</param>
    void OnValidateRequest(IUndeleteRequestHandler handler);

    /// <summary>Called just before row is undeleted in database</summary>
    /// <param name="handler">Calling undelete request handler</param>
    void OnBeforeUndelete(IUndeleteRequestHandler handler);

    /// <summary>Called after row is undeleted in database</summary>
    /// <param name="handler">Calling undelete request handler</param>
    void OnAfterUndelete(IUndeleteRequestHandler handler);

    /// <summary>Called after row is undeleted and auditing should be performed</summary>
    /// <param name="handler">Calling undelete request handler</param>
    void OnAudit(IUndeleteRequestHandler handler);

    /// <summary>Called before handler is returning the result</summary>
    /// <param name="handler">Calling undelete request handler</param>
    void OnReturn(IUndeleteRequestHandler handler);
}