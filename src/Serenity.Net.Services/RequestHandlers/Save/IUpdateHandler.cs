namespace Serenity.Services
{
    public interface IUpdateHandler<TRow, TSaveRequest, TSaveResponse>
        : IRequestHandler<TRow, TSaveRequest, TSaveResponse>
        where TRow : class, IRow, IIdRow, new()
        where TSaveRequest : SaveRequest<TRow>, new()
        where TSaveResponse : SaveResponse, new()
    {
        TSaveResponse Update(IUnitOfWork uow, TSaveRequest request);
    }

    public interface IUpdateHandler<TRow>
        : IUpdateHandler<TRow, SaveRequest<TRow>, SaveResponse>
        where TRow : class, IRow, IIdRow, new()
    {
    }
}