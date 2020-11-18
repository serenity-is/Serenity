using System.Data;

namespace Serenity.Services
{
    public interface IRetrieveRequestProcessor : IRetrieveRequestHandler
    {
        IRetrieveResponse Process(IDbConnection connection, RetrieveRequest request);
    }
}