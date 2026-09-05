namespace Serenity.Services;

internal class UpdateHandlerProxyAsync<TRow, TSaveRequest, TSaveResponse>
    : IUpdateHandlerAsync<TRow, TSaveRequest, TSaveResponse>
    where TRow : class, IRow, IIdRow, new()
    where TSaveResponse : SaveResponse, new()
    where TSaveRequest : SaveRequest<TRow>, new()
{
    private readonly IUpdateHandlerAsync<TRow, TSaveRequest, TSaveResponse> handler;

    public UpdateHandlerProxyAsync(IDefaultHandlerFactory factory)
    {
        ArgumentNullException.ThrowIfNull(factory);

        handler = (IUpdateHandlerAsync<TRow, TSaveRequest, TSaveResponse>) factory.CreateHandler<ISaveRequestProcessorAsync>(typeof(TRow));
    }

    public Task<TSaveResponse> UpdateAsync(IUnitOfWork uow, TSaveRequest request, CancellationToken cancellationToken = default)
    {
        return handler.UpdateAsync(uow, request, cancellationToken);
    }
}

internal class UpdateHandlerProxyAsync<TRow>(IDefaultHandlerFactory factory)
    : UpdateHandlerProxyAsync<TRow, SaveRequest<TRow>, SaveResponse>(factory), IUpdateHandlerAsync<TRow>
    where TRow : class, IRow, IIdRow, new()
{
}
