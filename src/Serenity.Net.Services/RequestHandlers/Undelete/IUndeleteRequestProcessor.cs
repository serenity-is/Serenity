namespace Serenity.Services;

/// <summary>
/// Abstraction for undelete request handlers with a Process method.
/// </summary>
[GenericHandlerType(typeof(UndeleteRequestHandler<>))]
public interface IUndeleteRequestProcessor : IUndeleteRequestHandler
{
    /// <summary>
    /// Processes the <see cref="UndeleteRequest"/> and returns a <see cref="UndeleteResponse"/>
    /// </summary>
    /// <param name="uow">Unit of work</param>
    /// <param name="request">Delete request</param>
    UndeleteResponse Process(IUnitOfWork uow, UndeleteRequest request);
}