namespace Serenity.Services;

/// <summary>
/// Abstraction for list request handlers with an async Process method.
/// </summary>
[GenericHandlerType(typeof(ListRequestHandlerAsync<>))]
[CompanionHandlerType(typeof(IListRequestProcessor), typeof(SyncToAsyncListRequestProcessorWrapper<>))]
public interface IListRequestProcessorAsync : IListRequestHandler
{
    /// <summary>
    /// Processes the <see cref="ListRequest"/> asynchronously and returns a <see cref="IListResponse"/>
    /// </summary>
    /// <param name="connection">Connection</param>
    /// <param name="request">List request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<IListResponse> ProcessAsync(IDbConnection connection, ListRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Builds the query the handler would execute for the request, without executing it,
    /// to use it as a source of another query. See <see cref="ListRequestHandlerAsync{TRow, TListRequest, TListResponse}.CaptureQueryAsync"/>.
    /// The default implementation throws <see cref="NotSupportedException"/>.
    /// </summary>
    /// <param name="connection">Connection</param>
    /// <param name="request">List request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <exception cref="NotSupportedException">The handler does not support capturing its query.</exception>
    Task<SqlQuery> CaptureQueryAsync(IDbConnection connection, ListRequest request, CancellationToken cancellationToken = default) =>
        Task.FromException<SqlQuery>(new NotSupportedException(
            $"The list handler '{GetType().FullName}' does not support capturing its query."));
}
