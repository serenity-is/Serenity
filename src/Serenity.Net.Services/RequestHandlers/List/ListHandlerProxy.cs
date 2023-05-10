namespace Serenity.Services;

internal class ListHandlerProxy<TRow, TListRequest, TListResponse>
    : IListHandler<TRow, TListRequest, TListResponse>
    where TRow : class, IRow, IIdRow, new()
    where TListRequest : ListRequest
    where TListResponse : ListResponse<TRow>, new()
{
    private readonly IListHandler<TRow, TListRequest, TListResponse> handler;

    public ListHandlerProxy(IDefaultHandlerFactory factory)
    {
        if (factory is null)
            throw new ArgumentNullException(nameof(factory));

        handler = (IListHandler<TRow, TListRequest, TListResponse>) factory.CreateHandler<IListRequestProcessor>(typeof(TRow));
    }

    public TListResponse List(IDbConnection connection, TListRequest request)
    {
        return handler.List(connection, request);
    }
}

internal class ListHandlerProxy<TRow, TListRequest>
    : ListHandlerProxy<TRow, TListRequest, ListResponse<TRow>>, IListHandler<TRow, TListRequest>
    where TRow : class, IRow, IIdRow, new()
    where TListRequest : ListRequest
{
    public ListHandlerProxy(IDefaultHandlerFactory factory) : base(factory) { }
}

internal class ListHandlerProxy<TRow>
    : ListHandlerProxy<TRow, ListRequest, ListResponse<TRow>>, IListHandler<TRow>
    where TRow : class, IRow, IIdRow, new()
{
    public ListHandlerProxy(IDefaultHandlerFactory factory) : base(factory) { }
}