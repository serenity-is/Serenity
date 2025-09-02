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

internal class ListHandlerProxy<TRow, TListRequest>(IDefaultHandlerFactory factory)
    : ListHandlerProxy<TRow, TListRequest, ListResponse<TRow>>(factory), IListHandler<TRow, TListRequest>
    where TRow : class, IRow, IIdRow, new()
    where TListRequest : ListRequest
{
}

internal class ListHandlerProxy<TRow>(IDefaultHandlerFactory factory)
    : ListHandlerProxy<TRow, ListRequest, ListResponse<TRow>>(factory), IListHandler<TRow>
    where TRow : class, IRow, IIdRow, new()
{
}