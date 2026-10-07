namespace Serenity.Services;

/// <summary>
/// Generic handler proxy that resolves the list handler for <typeparamref name="TRow"/> through
/// <see cref="IDefaultHandlerFactory"/> and forwards calls to it.
/// </summary>
/// <remarks>
/// See <see cref="Serenity.Extensions.DependencyInjection.ServiceCollectionExtensions.AddProxyRequestHandlers"/> for why these exist.
/// </remarks>
internal class ListHandlerProxy<TRow, TListRequest, TListResponse>
    : IListHandler<TRow, TListRequest, TListResponse>
    where TRow : class, IRow, IIdRow, new()
    where TListRequest : ListRequest
    where TListResponse : ListResponse<TRow>, new()
{
    private readonly IListHandler<TRow, TListRequest, TListResponse> handler;

    public ListHandlerProxy(IDefaultHandlerFactory factory)
    {
        ArgumentNullException.ThrowIfNull(factory);

        handler = factory.CreateHandlerForProxy<IListRequestProcessor, IListHandler<TRow, TListRequest, TListResponse>>(typeof(TRow));
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