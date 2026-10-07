namespace Serenity.Services;

/// <summary>
/// Generic handler proxy that resolves the update handler for <typeparamref name="TRow"/> through
/// <see cref="IDefaultHandlerFactory"/> and forwards calls to it.
/// </summary>
/// <remarks>
/// See <see cref="Serenity.Extensions.DependencyInjection.ServiceCollectionExtensions.AddProxyRequestHandlers"/> for why these exist.
/// </remarks>
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

        handler = factory.CreateHandlerForProxy<ISaveRequestProcessorAsync, IUpdateHandlerAsync<TRow, TSaveRequest, TSaveResponse>>(typeof(TRow));
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
