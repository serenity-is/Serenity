using Serenity.Data;

namespace Serenity.Services
{
    public interface IDeleteHandler<TRow, TDeleteRequest, TDeleteResponse>
        : IRequestHandler<TRow, TDeleteRequest, TDeleteResponse>
        where TRow : class, IRow, new()
        where TDeleteRequest : DeleteRequest
        where TDeleteResponse : DeleteResponse, new()
    {
        TDeleteResponse Delete(IUnitOfWork uow, TDeleteRequest request);
    }

    public interface IDeleteHandler<TRow>
        : IDeleteHandler<TRow, DeleteRequest, DeleteResponse>
        where TRow : class, IRow, new()
    {
    }
}