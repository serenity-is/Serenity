namespace Serenity.Services;

/// <summary>
/// Generic handler proxy that resolves the create handler for <typeparamref name="TRow"/> through
/// <see cref="IDefaultHandlerFactory"/> and forwards calls to it.
/// </summary>
/// <remarks>
/// See <see cref="Serenity.Extensions.DependencyInjection.ServiceCollectionExtensions.AddProxyRequestHandlers"/> for why these exist.
/// </remarks>
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

        handler = factory.CreateHandlerForProxy<ISaveRequestProcessorAsync, ICreateHandlerAsync<TRow, TSaveRequest, TSaveResponse>>(typeof(TRow));
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
