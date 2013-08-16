namespace Serenity.Services
{
    public interface IDeleteService<TEntity, TDeleteRequest, TDeleteResponse>
    {
        TDeleteResponse Delete(TDeleteRequest request);
    }

    public interface IDeleteService<TEntity> :
        IDeleteService<TEntity, DeleteRequest, DeleteResponse>
    {
    }
}