using Serenity.Data;

namespace Serenity.Services
{
    [GenericHandlerType(typeof(DeleteRequestHandler<>))]
    public interface IDeleteRequestProcessor : IDeleteRequestHandler
    {
        DeleteResponse Process(IUnitOfWork uow, DeleteRequest request);
    }
}