namespace Serenity.Services;

internal class UpdateHandlerProxy<TRow, TSaveRequest, TSaveResponse>
    : IUpdateHandler<TRow, TSaveRequest, TSaveResponse>
    where TRow : class, IRow, IIdRow, new()
    where TSaveResponse : SaveResponse, new()
    where TSaveRequest : SaveRequest<TRow>, new()
{
    private readonly IUpdateHandler<TRow, TSaveRequest, TSaveResponse> handler;

    public UpdateHandlerProxy(IDefaultHandlerFactory factory)
    {
        if (factory is null)
            throw new ArgumentNullException(nameof(factory));

        handler = (IUpdateHandler<TRow, TSaveRequest, TSaveResponse>) factory.CreateHandler<ISaveRequestProcessor>(typeof(TRow));
    }

    public TSaveResponse Update(IUnitOfWork uow, TSaveRequest request)
    {
        return handler.Update(uow, request);
    }
}

internal class UpdateHandlerProxy<TRow>
    : UpdateHandlerProxy<TRow, SaveRequest<TRow>, SaveResponse>, IUpdateHandler<TRow>
    where TRow : class, IRow, IIdRow, new()
{
    public UpdateHandlerProxy(IDefaultHandlerFactory factory) : base(factory) { }
}