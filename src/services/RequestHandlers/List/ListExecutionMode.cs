namespace Serenity.Services;

/// <summary>
/// Specifies why a list request handler is being executed. Handlers and list
/// behaviors can check it through <see cref="IListRequestHandler.ExecutionMode"/>
/// and act differently, or refuse, when they are not executed to return rows
/// to a list request.
/// </summary>
public enum ListExecutionMode
{
    /// <summary>
    /// The handler processes a list request and returns its rows.
    /// </summary>
    Normal = 0,

    /// <summary>
    /// The handler is executed only to build its query, e.g. to use it as a
    /// source in another query like an entity query of a data source. Any filter
    /// the handler or its behaviors would apply after the query is executed
    /// (e.g. in ProcessEntity or OnReturn) will not take effect, so such logic
    /// should apply its filter to the query instead, or throw an exception.
    /// </summary>
    QueryCapture = 1
}
