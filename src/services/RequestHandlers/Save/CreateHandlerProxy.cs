namespace Serenity.Services;

/// <summary>
/// Generic handler proxy that resolves the create handler for <typeparamref name="TRow"/> through
/// <see cref="IDefaultHandlerFactory"/> and forwards calls to it.
/// </summary>
/// <remarks>
/// See <see cref="Serenity.Extensions.DependencyInjection.ServiceCollectionExtensions.AddProxyRequestHandlers"/> for why these exist.
/// </remarks>
internal class CreateHandlerProxy<TRow, TSaveRequest, TSaveResponse>
    : ICreateHandler<TRow, TSaveRequest, TSaveResponse>
    where TRow : class, IRow, IIdRow, new()
    where TSaveResponse : SaveResponse, new()
    where TSaveRequest : SaveRequest<TRow>, new()
{
    private readonly ICreateHandler<TRow, TSaveRequest, TSaveResponse> handler;

    public CreateHandlerProxy(IDefaultHandlerFactory factory)
    {
        ArgumentNullException.ThrowIfNull(factory);

        handler = factory.CreateHandlerForProxy<ISaveRequestProcessor, ICreateHandler<TRow, TSaveRequest, TSaveResponse>>(typeof(TRow));
    }

    public TSaveResponse Create(IUnitOfWork uow, TSaveRequest request)
    {
        return handler.Create(uow, request);
    }
}

internal class CreateHandlerProxy<TRow>(IDefaultHandlerFactory factory)
    : CreateHandlerProxy<TRow, SaveRequest<TRow>, SaveResponse>(factory), ICreateHandler<TRow>
    where TRow : class, IRow, IIdRow, new()
{
}