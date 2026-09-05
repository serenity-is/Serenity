namespace Serenity.Services;

/// <summary>
/// Wraps an asynchronous <see cref="IDeleteRequestProcessorAsync"/> custom handler and exposes it
/// as a synchronous <see cref="IDeleteRequestProcessor"/>. Used by
/// <see cref="DefaultHandlerFactory"/> when a synchronous delete handler is requested for a row
/// that only has an asynchronous custom delete handler.
/// </summary>
/// <typeparam name="TRow">Row type</typeparam>
internal class AsyncToSyncDeleteRequestProcessorWrapper<TRow>(IDeleteRequestProcessorAsync handler) : IDeleteRequestProcessor,
    IDeleteHandler<TRow>
    where TRow : class, IRow, IIdRow, new()
{
    private readonly IDeleteRequestProcessorAsync handler = handler ??
        throw new ArgumentNullException(nameof(handler));

    /// <summary>
    /// Gets the wrapped asynchronous handler.
    /// </summary>
    public IDeleteRequestProcessorAsync WrappedHandler => handler;

    /// <inheritdoc/>
    public DeleteResponse Process(IUnitOfWork uow, DeleteRequest request)
    {
        return handler.ProcessAsync(uow, request).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public DeleteResponse Delete(IUnitOfWork uow, DeleteRequest request)
    {
        return Process(uow, request);
    }

    IRow IDeleteRequestHandler.Row => handler.Row;
    DeleteRequest IDeleteRequestHandler.Request => handler.Request;
    DeleteResponse IDeleteRequestHandler.Response => handler.Response;
    IDictionary<string, object> IDeleteRequestHandler.StateBag => handler.StateBag;
    IDbConnection IDeleteRequestHandler.Connection => handler.Connection;
    IUnitOfWork IDeleteRequestHandler.UnitOfWork => handler.UnitOfWork;
    IRequestContext IDeleteRequestHandler.Context => handler.Context;
}
