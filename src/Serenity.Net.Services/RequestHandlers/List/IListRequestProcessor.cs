using System.Data;

namespace Serenity.Services
{
    [GenericHandlerType(typeof(ListRequestHandler<>))]
    public interface IListRequestProcessor : IListRequestHandler
    {
        IListResponse Process(IDbConnection connection, ListRequest request);
    }
}