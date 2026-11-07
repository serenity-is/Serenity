namespace Serenity.Services;

/// <summary>
/// Abstraction for list request handlers with a Process method.
/// </summary>
[GenericHandlerType(typeof(ListRequestHandler<>))]
[CompanionHandlerType(typeof(IListRequestProcessorAsync), typeof(AsyncToSyncListRequestProcessorWrapper<>))]
public interface IListRequestProcessor : IListRequestHandler
{
    /// <summary>
    /// Processes the <see cref="ListRequest"/> and returns a <see cref="ListResponse{T}"/>
    /// </summary>
    /// <param name="connection">Connection</param>
    /// <param name="request">List request</param>
    IListResponse Process(IDbConnection connection, ListRequest request);

    /// <summary>
    /// Builds the query the handler would execute for the request, without executing it,
    /// to use it as a source of another query. See <see cref="ListRequestHandler{TRow, TListRequest, TListResponse}.CaptureQuery"/>.
    /// The default implementation throws <see cref="NotSupportedException"/>.
    /// </summary>
    /// <param name="connection">Connection</param>
    /// <param name="request">List request</param>
    /// <exception cref="NotSupportedException">The handler does not support capturing its query.</exception>
    SqlQuery CaptureQuery(IDbConnection connection, ListRequest request) =>
        throw new NotSupportedException($"The list handler '{GetType().FullName}' does not support capturing its query.");
}