namespace Serenity.Services;

/// <summary>
/// Generic handler proxy that resolves the delete handler for <typeparamref name="TRow"/> through
/// <see cref="IDefaultHandlerFactory"/> and forwards calls to it.
/// </summary>
/// <remarks>
/// See <see cref="ServiceCollectionExtensions.AddProxyRequestHandlers"/> for why these exist.
/// </remarks>
internal class DeleteHandlerProxy<TRow, TDeleteRequest, TDeleteResponse>
    : IDeleteHandler<TRow, TDeleteRequest, TDeleteResponse>
    where TRow : class, IRow, IIdRow, new()
    where TDeleteRequest : DeleteRequest
    where TDeleteResponse : DeleteResponse, new()
{
    private readonly IDeleteHandler<TRow, TDeleteRequest, TDeleteResponse> handler;

    public DeleteHandlerProxy(IDefaultHandlerFactory factory)
    {
        ArgumentNullException.ThrowIfNull(factory);

        handler = factory.CreateHandlerForProxy<IDeleteRequestProcessor, IDeleteHandler<TRow, TDeleteRequest, TDeleteResponse>>(typeof(TRow));
    }

    public TDeleteResponse Delete(IUnitOfWork uow, TDeleteRequest request)
    {
        return handler.Delete(uow, request);
    }
}

internal class DeleteHandlerProxy<TRow>(IDefaultHandlerFactory factory)
    : DeleteHandlerProxy<TRow, DeleteRequest, DeleteResponse>(factory), IDeleteHandler<TRow>
    where TRow : class, IRow, IIdRow, new()
{
}