using System.Data;

namespace Serenity.Services
{
    [GenericHandlerType(typeof(RetrieveRequestHandler<>))]
    public interface IRetrieveRequestProcessor : IRetrieveRequestHandler
    {
        IRetrieveResponse Process(IDbConnection connection, RetrieveRequest request);
    }
}