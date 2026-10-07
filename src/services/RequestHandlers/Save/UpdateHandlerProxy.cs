namespace Serenity.Services;

/// <summary>
/// Generic handler proxy that resolves the update handler for <typeparamref name="TRow"/> through
/// <see cref="IDefaultHandlerFactory"/> and forwards calls to it.
/// </summary>
/// <remarks>
/// See <see cref="Serenity.Extensions.DependencyInjection.ServiceCollectionExtensions.AddProxyRequestHandlers"/> for why these exist.
/// </remarks>
internal class UpdateHandlerProxy<TRow, TSaveRequest, TSaveResponse>
    : IUpdateHandler<TRow, TSaveRequest, TSaveResponse>
    where TRow : class, IRow, IIdRow, new()
    where TSaveResponse : SaveResponse, new()
    where TSaveRequest : SaveRequest<TRow>, new()
{
    private readonly IUpdateHandler<TRow, TSaveRequest, TSaveResponse> handler;

    public UpdateHandlerProxy(IDefaultHandlerFactory factory)
    {
        ArgumentNullException.ThrowIfNull(factory);

        handler = factory.CreateHandlerForProxy<ISaveRequestProcessor, IUpdateHandler<TRow, TSaveRequest, TSaveResponse>>(typeof(TRow));
    }

    public TSaveResponse Update(IUnitOfWork uow, TSaveRequest request)
    {
        return handler.Update(uow, request);
    }
}

internal class UpdateHandlerProxy<TRow>(IDefaultHandlerFactory factory)
    : UpdateHandlerProxy<TRow, SaveRequest<TRow>, SaveResponse>(factory), IUpdateHandler<TRow>
    where TRow : class, IRow, IIdRow, new()
{
}