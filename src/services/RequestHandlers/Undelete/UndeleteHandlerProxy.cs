namespace Serenity.Services;

/// <summary>
/// Generic handler proxy that resolves the undelete handler for <typeparamref name="TRow"/> through
/// <see cref="IDefaultHandlerFactory"/> and forwards calls to it.
/// </summary>
/// <remarks>
/// See <see cref="Serenity.Extensions.DependencyInjection.ServiceCollectionExtensions.AddProxyRequestHandlers"/> for why these exist.
/// </remarks>
internal class UndeleteHandlerProxy<TRow, TUndeleteRequest, TUndeleteResponse>
    : IUndeleteHandler<TRow, TUndeleteRequest, TUndeleteResponse>
    where TRow : class, IRow, IIdRow, new()
    where TUndeleteRequest : UndeleteRequest
    where TUndeleteResponse : UndeleteResponse, new()
{
    private readonly IUndeleteHandler<TRow, TUndeleteRequest, TUndeleteResponse> handler;

    public UndeleteHandlerProxy(IDefaultHandlerFactory factory)
    {
        ArgumentNullException.ThrowIfNull(factory);

        handler = factory.CreateHandlerForProxy<IUndeleteRequestProcessor, IUndeleteHandler<TRow, TUndeleteRequest, TUndeleteResponse>>(typeof(TRow));
    }

    public TUndeleteResponse Undelete(IUnitOfWork uow, TUndeleteRequest request)
    {
        return handler.Undelete(uow, request);
    }
}

internal class UndeleteHandlerProxy<TRow>(IDefaultHandlerFactory factory)
    : UndeleteHandlerProxy<TRow, UndeleteRequest, UndeleteResponse>(factory), IUndeleteHandler<TRow>
    where TRow : class, IRow, IIdRow, new()
{
}