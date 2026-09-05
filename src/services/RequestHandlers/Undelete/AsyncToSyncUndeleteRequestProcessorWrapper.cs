namespace Serenity.Services;

/// <summary>
/// Wraps an asynchronous <see cref="IUndeleteRequestProcessorAsync"/> custom handler and exposes it
/// as a synchronous <see cref="IUndeleteRequestProcessor"/>. Used by
/// <see cref="DefaultHandlerFactory"/> when a synchronous undelete handler is requested for a row
/// that only has an asynchronous custom undelete handler.
/// </summary>
/// <typeparam name="TRow">Row type</typeparam>
internal class AsyncToSyncUndeleteRequestProcessorWrapper<TRow>(IUndeleteRequestProcessorAsync handler) : IUndeleteRequestProcessor,
    IUndeleteHandler<TRow>
    where TRow : class, IRow, IIdRow, new()
{
    private readonly IUndeleteRequestProcessorAsync handler = handler ??
        throw new ArgumentNullException(nameof(handler));

    /// <summary>
    /// Gets the wrapped asynchronous handler.
    /// </summary>
    public IUndeleteRequestProcessorAsync WrappedHandler => handler;

    /// <inheritdoc/>
    public UndeleteResponse Process(IUnitOfWork uow, UndeleteRequest request)
    {
        return handler.ProcessAsync(uow, request).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public UndeleteResponse Undelete(IUnitOfWork uow, UndeleteRequest request)
    {
        return Process(uow, request);
    }

    IRow IUndeleteRequestHandler.Row => handler.Row;
    UndeleteRequest IUndeleteRequestHandler.Request => handler.Request;
    UndeleteResponse IUndeleteRequestHandler.Response => handler.Response;
    IDictionary<string, object> IUndeleteRequestHandler.StateBag => handler.StateBag;
    IDbConnection IUndeleteRequestHandler.Connection => handler.Connection;
    IUnitOfWork IUndeleteRequestHandler.UnitOfWork => handler.UnitOfWork;
    IRequestContext IUndeleteRequestHandler.Context => handler.Context;
}
