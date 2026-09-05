namespace Serenity.Services;

/// <summary>
/// Abstraction for undelete request handlers with an async Process method.
/// </summary>
[GenericHandlerType(typeof(UndeleteRequestHandlerAsync<>))]
[CompanionHandlerType(typeof(IUndeleteRequestProcessor), typeof(AsyncToSyncUndeleteRequestProcessorWrapper<>))]
public interface IUndeleteRequestProcessorAsync : IUndeleteRequestHandler
{
    /// <summary>
    /// Processes the <see cref="UndeleteRequest"/> asynchronously and returns a <see cref="UndeleteResponse"/>
    /// </summary>
    /// <param name="uow">Unit of work</param>
    /// <param name="request">Undelete request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<UndeleteResponse> ProcessAsync(IUnitOfWork uow, UndeleteRequest request, CancellationToken cancellationToken = default);
}
