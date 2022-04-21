namespace Serenity.Services
{
    public interface ICreateHandler<TRow, TSaveRequest, TSaveResponse>
        : IRequestHandler<TRow, TSaveRequest, TSaveResponse>
        where TRow : class, IRow, IIdRow, new()
        where TSaveRequest : SaveRequest<TRow>, new()
        where TSaveResponse : SaveResponse, new()
    {
        TSaveResponse Create(IUnitOfWork uow, TSaveRequest request);
    }

    public interface ICreateHandler<TRow>
        : ICreateHandler<TRow, SaveRequest<TRow>, SaveResponse>
        where TRow : class, IRow, IIdRow, new()
    {
    }
}