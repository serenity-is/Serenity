namespace Serenity.Services;

/// <summary>
/// Abstraction for save request handlers with a Process method.
/// </summary>
[GenericHandlerType(typeof(SaveRequestHandler<>))]
public interface ISaveRequestProcessor : ISaveRequestHandler
{
    /// <summary>
    /// Processes the <see cref="ISaveRequest"/> and returns a <see cref="SaveResponse"/>
    /// </summary>
    /// <param name="uow">Unit of work</param>
    /// <param name="request">List request</param>
    /// <param name="type">Save request type, Create or Update</param>
    SaveResponse Process(IUnitOfWork uow, ISaveRequest request, SaveRequestType type);
}