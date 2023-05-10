namespace Serenity.Services;

internal class UndeleteHandlerProxy<TRow, TUndeleteRequest, TUndeleteResponse>
    : IUndeleteHandler<TRow, TUndeleteRequest, TUndeleteResponse>
    where TRow : class, IRow, IIdRow, new()
    where TUndeleteRequest : UndeleteRequest
    where TUndeleteResponse : UndeleteResponse, new()
{
    private readonly IUndeleteHandler<TRow, TUndeleteRequest, TUndeleteResponse> handler;

    public UndeleteHandlerProxy(IDefaultHandlerFactory factory)
    {
        if (factory is null)
            throw new ArgumentNullException(nameof(factory));

        handler = (IUndeleteHandler<TRow, TUndeleteRequest, TUndeleteResponse>) factory.CreateHandler<IUndeleteRequestProcessor>(typeof(TRow));
    }

    public TUndeleteResponse Undelete(IUnitOfWork uow, TUndeleteRequest request)
    {
        return handler.Undelete(uow, request);
    }
}

internal class UndeleteHandlerProxy<TRow>
    : UndeleteHandlerProxy<TRow, UndeleteRequest, UndeleteResponse>, IUndeleteHandler<TRow>
    where TRow : class, IRow, IIdRow, new()
{
    public UndeleteHandlerProxy(IDefaultHandlerFactory factory) : base(factory) { }
}