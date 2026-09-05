namespace Serenity.Services;

/// <summary>
/// Abstraction for delete request handlers with a Process method.
/// </summary>
[GenericHandlerType(typeof(DeleteRequestHandler<>))]
[CompanionHandlerType(typeof(IDeleteRequestProcessorAsync), typeof(AsyncToSyncDeleteRequestProcessorWrapper<>))]
public interface IDeleteRequestProcessor : IDeleteRequestHandler
{
    /// <summary>
    /// Processes the <see cref="DeleteRequest"/> and returns a <see cref="DeleteResponse"/>
    /// </summary>
    /// <param name="uow">Unit of work</param>
    /// <param name="request">Delete request</param>
    DeleteResponse Process(IUnitOfWork uow, DeleteRequest request);
}