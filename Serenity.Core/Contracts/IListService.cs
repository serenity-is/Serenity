
namespace Serenity.Services
{
    public interface IListService<TEntity, TListRequest, TListResponse>
        where TListRequest: ListRequest
        where TListResponse: ListResponse<TEntity>
    {
        TListResponse List(TListRequest request);
    }

    public interface IListService<TEntity> :
        IListService<TEntity, ListRequest, ListResponse<TEntity>>
    {
        
    }
}