namespace Serenity.Services;

/// <summary>
/// A save behavior that can be used as a mixin within a SaveRequestHandler lifecycle
/// </summary>
/// <remarks>
/// A save behavior instance is always cached and reused across requests, so make 
/// sure you don't store anything in private variables, and its operation 
/// is thread-safe. If you need to pass some state between events, 
/// use handler's StateBag.
/// </remarks>
public interface ISaveBehavior
{
    /// <summary>Called when query to load old entity is built</summary>
    /// <param name="handler">Calling save request handler</param>
    /// <param name="query">Query</param>
    void OnPrepareQuery(ISaveRequestHandler handler, SqlQuery query);

    /// <summary>Called when save request is validated</summary>
    /// <param name="handler">Calling save request handler</param>
    void OnValidateRequest(ISaveRequestHandler handler);

    /// <summary>Called when internal fields in row is being set</summary>
    /// <param name="handler">Calling save request handler</param>
    void OnSetInternalFields(ISaveRequestHandler handler);

    /// <summary>Called just before row is inserted to / updated in database</summary>
    /// <param name="handler">Calling save request handler</param>
    void OnBeforeSave(ISaveRequestHandler handler);

    /// <summary>Called after row is inserted to / updated in database</summary>
    /// <param name="handler">Calling save request handler</param>
    void OnAfterSave(ISaveRequestHandler handler);

    /// <summary>Called after row is inserted to / updated and auditing should be performed</summary>
    /// <param name="handler">Calling save request handler</param>
    void OnAudit(ISaveRequestHandler handler);

    /// <summary>Called before handler is returning the result</summary>
    /// <param name="handler">Calling save request handler</param>
    void OnReturn(ISaveRequestHandler handler);
}