namespace Serenity.Services;

/// <summary>
/// Abstraction for list request handlers with an async Process method.
/// </summary>
[GenericHandlerType(typeof(ListRequestHandlerAsync<>))]
[CompanionHandlerType(typeof(IListRequestProcessor), typeof(AsyncToSyncListRequestProcessorWrapper<>))]
public interface IListRequestProcessorAsync : IListRequestHandler
{
    /// <summary>
    /// Processes the <see cref="ListRequest"/> asynchronously and returns a <see cref="IListResponse"/>
    /// </summary>
    /// <param name="connection">Connection</param>
    /// <param name="request">List request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<IListResponse> ProcessAsync(IDbConnection connection, ListRequest request, CancellationToken cancellationToken = default);
}
