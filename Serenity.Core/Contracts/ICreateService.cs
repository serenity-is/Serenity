namespace Serenity.Services
{
    public interface ICreateService<TEntity, TCreateRequest, TCreateResponse>
        where TCreateRequest: SaveRequest<TEntity>
        where TCreateResponse: CreateResponse
    {
        TCreateResponse Create(TCreateRequest request);
    }

    public interface ICreateService<TEntity> : 
        ICreateService<TEntity, SaveRequest<TEntity>, CreateResponse>
    {
    }
}