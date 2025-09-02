namespace Serenity.Services;

/// <summary>
/// A list behavior that can be used as a mixin within a ListRequestHandler lifecycle
/// </summary>
/// <remarks>
/// A list behavior instance is always cached and reused across requests, so make 
/// sure you don't store anything in private variables, and its operation 
/// is thread-safe. If you need to pass some state between events, 
/// use handler's StateBag.
/// </remarks>
public interface IListBehavior
{
    /// <summary>Called when list request is validated</summary>
    /// <param name="handler">Calling list request handler</param>
    void OnValidateRequest(IListRequestHandler handler);

    /// <summary>Called when query to is built</summary>
    /// <param name="handler">Calling list request handler</param>
    /// <param name="query">Query</param>
    void OnPrepareQuery(IListRequestHandler handler, SqlQuery query);

    /// <summary>Called when filters are applied to query</summary>
    /// <param name="handler">Calling list request handler</param>
    /// <param name="query">Query</param>
    void OnApplyFilters(IListRequestHandler handler, SqlQuery query);

    /// <summary>Called just before query is sent to database</summary>
    /// <param name="handler">Calling list request handler</param>
    void OnBeforeExecuteQuery(IListRequestHandler handler);

    /// <summary>Called after query is sent to database</summary>
    /// <param name="handler">Calling list request handler</param>
    void OnAfterExecuteQuery(IListRequestHandler handler);

    /// <summary>Called before handler is returning the result</summary>
    /// <param name="handler">Calling list request handler</param>
    void OnReturn(IListRequestHandler handler);
}