namespace Serenity.Services
{
    public interface IUpdateService<TEntity, TUpdateRequest, TUpdateResponse>
        where TUpdateRequest : SaveRequest<TEntity>
        where TUpdateResponse : UpdateResponse
    {
        TUpdateResponse Update(TUpdateRequest request);
    }

    public interface IUpdateService<TEntity> : 
        IUpdateService<TEntity, SaveRequest<TEntity>, UpdateResponse>
    {
    }
}