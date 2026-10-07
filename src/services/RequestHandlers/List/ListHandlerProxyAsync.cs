namespace Serenity.Services;

/// <summary>
/// Generic handler proxy that resolves the list handler for <typeparamref name="TRow"/> through
/// <see cref="IDefaultHandlerFactory"/> and forwards calls to it.
/// </summary>
/// <remarks>
/// See <see cref="Serenity.Extensions.DependencyInjection.ServiceCollectionExtensions.AddProxyRequestHandlers"/> for why these exist.
/// </remarks>
internal class ListHandlerProxyAsync<TRow, TListRequest, TListResponse>
    : IListHandlerAsync<TRow, TListRequest, TListResponse>
    where TRow : class, IRow, IIdRow, new()
    where TListRequest : ListRequest
    where TListResponse : ListResponse<TRow>, new()
{
    private readonly IListHandlerAsync<TRow, TListRequest, TListResponse> handler;

    public ListHandlerProxyAsync(IDefaultHandlerFactory factory)
    {
        ArgumentNullException.ThrowIfNull(factory);

        handler = factory.CreateHandlerForProxy<IListRequestProcessorAsync, IListHandlerAsync<TRow, TListRequest, TListResponse>>(typeof(TRow));
    }

    public Task<TListResponse> ListAsync(IDbConnection connection, TListRequest request, CancellationToken cancellationToken = default)
    {
        return handler.ListAsync(connection, request, cancellationToken);
    }
}

internal class ListHandlerProxyAsync<TRow, TListRequest>(IDefaultHandlerFactory factory)
    : ListHandlerProxyAsync<TRow, TListRequest, ListResponse<TRow>>(factory), IListHandlerAsync<TRow, TListRequest>
    where TRow : class, IRow, IIdRow, new()
    where TListRequest : ListRequest
{
}

internal class ListHandlerProxyAsync<TRow>(IDefaultHandlerFactory factory)
    : ListHandlerProxyAsync<TRow, ListRequest, ListResponse<TRow>>(factory), IListHandlerAsync<TRow>
    where TRow : class, IRow, IIdRow, new()
{
}
