namespace Serenity.Services;

/// <summary>
/// Abstraction for delete request handlers with an async Process method.
/// </summary>
[GenericHandlerType(typeof(DeleteRequestHandlerAsync<>))]
[CompanionHandlerType(typeof(IDeleteRequestProcessor), typeof(AsyncToSyncDeleteRequestProcessorWrapper<>))]
public interface IDeleteRequestProcessorAsync : IDeleteRequestHandler
{
    /// <summary>
    /// Processes the <see cref="DeleteRequest"/> asynchronously and returns a <see cref="DeleteResponse"/>
    /// </summary>
    /// <param name="uow">Unit of work</param>
    /// <param name="request">Delete request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<DeleteResponse> ProcessAsync(IUnitOfWork uow, DeleteRequest request, CancellationToken cancellationToken = default);
}
