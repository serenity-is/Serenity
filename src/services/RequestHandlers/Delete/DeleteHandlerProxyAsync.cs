namespace Serenity.Services;

/// <summary>
/// Generic handler proxy that resolves the delete handler for <typeparamref name="TRow"/> through
/// <see cref="IDefaultHandlerFactory"/> and forwards calls to it.
/// </summary>
/// <remarks>
/// See <see cref="Serenity.Extensions.DependencyInjection.ServiceCollectionExtensions.AddProxyRequestHandlers"/> for why these exist.
/// </remarks>
internal class DeleteHandlerProxyAsync<TRow, TDeleteRequest, TDeleteResponse>
    : IDeleteHandlerAsync<TRow, TDeleteRequest, TDeleteResponse>
    where TRow : class, IRow, IIdRow, new()
    where TDeleteRequest : DeleteRequest
    where TDeleteResponse : DeleteResponse, new()
{
    private readonly IDeleteHandlerAsync<TRow, TDeleteRequest, TDeleteResponse> handler;

    public DeleteHandlerProxyAsync(IDefaultHandlerFactory factory)
    {
        ArgumentNullException.ThrowIfNull(factory);

        handler = factory.CreateHandlerForProxy<IDeleteRequestProcessorAsync, IDeleteHandlerAsync<TRow, TDeleteRequest, TDeleteResponse>>(typeof(TRow));
    }

    public Task<TDeleteResponse> DeleteAsync(IUnitOfWork uow, TDeleteRequest request, CancellationToken cancellationToken = default)
    {
        return handler.DeleteAsync(uow, request, cancellationToken);
    }
}

internal class DeleteHandlerProxyAsync<TRow>(IDefaultHandlerFactory factory)
    : DeleteHandlerProxyAsync<TRow, DeleteRequest, DeleteResponse>(factory), IDeleteHandlerAsync<TRow>
    where TRow : class, IRow, IIdRow, new()
{
}
