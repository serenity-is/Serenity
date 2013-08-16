namespace Serenity.Services
{
    public interface IRetrieveService<TEntity, TRetrieveRequest, TRetrieveResponse>
    {
        TRetrieveResponse Retrieve(TRetrieveRequest request);
    }

    public interface IRetrieveService<TEntity> :
        IRetrieveService<TEntity, RetrieveRequest, RetrieveResponse<TEntity>>
    {
    }
}