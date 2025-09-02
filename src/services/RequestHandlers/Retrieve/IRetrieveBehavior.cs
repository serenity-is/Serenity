namespace Serenity.Services;

/// <summary>
/// A retrieve behavior that can be used as a mixin within a RetrieveRequestHandler lifecycle
/// </summary>
/// <remarks>
/// A retrieve behavior instance is always cached and reused across requests, so make 
/// sure you don't store anything in private variables, and its operation 
/// is thread-safe. If you need to pass some state between events, 
/// use handler's StateBag.
/// </remarks>
public interface IRetrieveBehavior
{
    /// <summary>Called when retrieve request is validated</summary>
    /// <param name="handler">Calling retrieve request handler</param>
    void OnValidateRequest(IRetrieveRequestHandler handler);

    /// <summary>Called when query to is built</summary>
    /// <param name="handler">Calling retrieve request handler</param>
    /// <param name="query">Query</param>
    void OnPrepareQuery(IRetrieveRequestHandler handler, SqlQuery query);

    /// <summary>Called just before query is sent to database</summary>
    /// <param name="handler">Calling retrieve request handler</param>
    void OnBeforeExecuteQuery(IRetrieveRequestHandler handler);

    /// <summary>Called after query is sent to database</summary>
    /// <param name="handler">Calling retrieve request handler</param>
    void OnAfterExecuteQuery(IRetrieveRequestHandler handler);

    /// <summary>Called before handler is returning the result</summary>
    /// <param name="handler">Calling retrieve request handler</param>
    void OnReturn(IRetrieveRequestHandler handler);
}