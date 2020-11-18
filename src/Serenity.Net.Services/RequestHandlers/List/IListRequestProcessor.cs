namespace Serenity.Services
{
    using System.Data;

    public interface IListRequestProcessor : IListRequestHandler
    {
        IListResponse Process(IDbConnection connection, ListRequest request);
    }
}