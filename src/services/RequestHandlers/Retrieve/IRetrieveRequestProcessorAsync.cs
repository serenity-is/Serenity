namespace Serenity.Services;

/// <summary>
/// Abstraction for retrieve request handlers with an async Process method.
/// </summary>
[GenericHandlerType(typeof(RetrieveRequestHandlerAsync<>))]
[CompanionHandlerType(typeof(IRetrieveRequestProcessor), typeof(AsyncToSyncRetrieveRequestProcessorWrapper<>))]
public interface IRetrieveRequestProcessorAsync : IRetrieveRequestHandler
{
    /// <summary>
    /// Processes the <see cref="RetrieveRequest"/> asynchronously and returns a <see cref="IRetrieveResponse"/>
    /// </summary>
    /// <param name="connection">Connection</param>
    /// <param name="request">Retrieve request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<IRetrieveResponse> ProcessAsync(IDbConnection connection, RetrieveRequest request, CancellationToken cancellationToken = default);
}
