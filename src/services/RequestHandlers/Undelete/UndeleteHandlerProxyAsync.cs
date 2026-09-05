namespace Serenity.Services;

internal class UndeleteHandlerProxyAsync<TRow, TUndeleteRequest, TUndeleteResponse>
    : IUndeleteHandlerAsync<TRow, TUndeleteRequest, TUndeleteResponse>
    where TRow : class, IRow, IIdRow, new()
    where TUndeleteRequest : UndeleteRequest
    where TUndeleteResponse : UndeleteResponse, new()
{
    private readonly IUndeleteHandlerAsync<TRow, TUndeleteRequest, TUndeleteResponse> handler;

    public UndeleteHandlerProxyAsync(IDefaultHandlerFactory factory)
    {
        ArgumentNullException.ThrowIfNull(factory);

        handler = (IUndeleteHandlerAsync<TRow, TUndeleteRequest, TUndeleteResponse>) factory.CreateHandler<IUndeleteRequestProcessorAsync>(typeof(TRow));
    }

    public Task<TUndeleteResponse> UndeleteAsync(IUnitOfWork uow, TUndeleteRequest request, CancellationToken cancellationToken = default)
    {
        return handler.UndeleteAsync(uow, request, cancellationToken);
    }
}

internal class UndeleteHandlerProxyAsync<TRow>(IDefaultHandlerFactory factory)
    : UndeleteHandlerProxyAsync<TRow, UndeleteRequest, UndeleteResponse>(factory), IUndeleteHandlerAsync<TRow>
    where TRow : class, IRow, IIdRow, new()
{
}
