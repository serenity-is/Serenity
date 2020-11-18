using Serenity.Data;

namespace Serenity.Services
{
    public interface IDeleteRequestProcessor : IDeleteRequestHandler
    {
        DeleteResponse Process(IUnitOfWork uow, DeleteRequest request);
    }
}