using System.Data;

namespace Serenity.Services
{
    public interface IListRequestProcessor : IListRequestHandler
    {
        IListResponse Process(IDbConnection connection, ListRequest request);
    }
}