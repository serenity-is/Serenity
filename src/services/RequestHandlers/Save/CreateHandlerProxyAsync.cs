namespace Serenity.Services;

internal class CreateHandlerProxyAsync<TRow, TSaveRequest, TSaveResponse>
    : ICreateHandlerAsync<TRow, TSaveRequest, TSaveResponse>
    where TRow : class, IRow, IIdRow, new()
    where TSaveResponse : SaveResponse, new()
    where TSaveRequest : SaveRequest<TRow>, new()
{
    private readonly ICreateHandlerAsync<TRow, TSaveRequest, TSaveResponse> handler;

    public CreateHandlerProxyAsync(IDefaultHandlerFactory factory)
    {
        ArgumentNullException.ThrowIfNull(factory);

        handler = (ICreateHandlerAsync<TRow, TSaveRequest, TSaveResponse>) factory.CreateHandler<ISaveRequestProcessorAsync>(typeof(TRow));
    }

    public Task<TSaveResponse> CreateAsync(IUnitOfWork uow, TSaveRequest request, CancellationToken cancellationToken = default)
    {
        return handler.CreateAsync(uow, request, cancellationToken);
    }
}

internal class CreateHandlerProxyAsync<TRow>(IDefaultHandlerFactory factory)
    : CreateHandlerProxyAsync<TRow, SaveRequest<TRow>, SaveResponse>(factory), ICreateHandlerAsync<TRow>
    where TRow : class, IRow, IIdRow, new()
{
}
