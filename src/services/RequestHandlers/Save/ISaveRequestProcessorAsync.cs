namespace Serenity.Services;

/// <summary>
/// Abstraction for save request handlers with an async Process method.
/// </summary>
[GenericHandlerType(typeof(SaveRequestHandlerAsync<>))]
[CompanionHandlerType(typeof(ISaveRequestProcessor), typeof(SyncToAsyncSaveRequestProcessorWrapper<>))]
public interface ISaveRequestProcessorAsync : ISaveRequestHandler
{
    /// <summary>
    /// Processes the <see cref="ISaveRequest"/> asynchronously and returns a <see cref="SaveResponse"/>.
    /// </summary>
    /// <param name="uow">Unit of work</param>
    /// <param name="request">Save request</param>
    /// <param name="type">Save request type, Create or Update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<SaveResponse> ProcessAsync(IUnitOfWork uow, ISaveRequest request, SaveRequestType type,
        CancellationToken cancellationToken = default);
}
