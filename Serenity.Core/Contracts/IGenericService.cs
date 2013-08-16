namespace Serenity.Services
{
    public interface IGenericService<TEntity> : 
        ICreateService<TEntity>,
        IRetrieveService<TEntity>,
        IUpdateService<TEntity>,
        IDeleteService<TEntity>,
        IListService<TEntity>
    {
    }
}