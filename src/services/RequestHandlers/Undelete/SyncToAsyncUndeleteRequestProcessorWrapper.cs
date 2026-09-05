namespace Serenity.Services;

/// <summary>
/// Wraps a synchronous <see cref="IUndeleteRequestProcessor"/> custom handler and exposes it
/// as an asynchronous <see cref="IUndeleteRequestProcessorAsync"/>. Used by
/// <see cref="DefaultHandlerFactory"/> when an async undelete handler is requested for a row
/// that only has a synchronous custom undelete handler.
/// </summary>
/// <typeparam name="TRow">Row type</typeparam>
internal class SyncToAsyncUndeleteRequestProcessorWrapper<TRow>(IUndeleteRequestProcessor handler) : IUndeleteRequestProcessorAsync,
    IUndeleteHandlerAsync<TRow>
    where TRow : class, IRow, IIdRow, new()
{
    private readonly IUndeleteRequestProcessor handler = handler ??
        throw new ArgumentNullException(nameof(handler));

    /// <summary>
    /// Gets the wrapped synchronous handler.
    /// </summary>
    public IUndeleteRequestProcessor WrappedHandler => handler;

    /// <inheritdoc/>
    public Task<UndeleteResponse> ProcessAsync(IUnitOfWork uow, UndeleteRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(handler.Process(uow, request));
    }

    /// <inheritdoc/>
    public Task<UndeleteResponse> UndeleteAsync(IUnitOfWork uow, UndeleteRequest request, CancellationToken cancellationToken = default)
    {
        return ProcessAsync(uow, request, cancellationToken);
    }

    IRow IUndeleteRequestHandler.Row => handler.Row;
    UndeleteRequest IUndeleteRequestHandler.Request => handler.Request;
    UndeleteResponse IUndeleteRequestHandler.Response => handler.Response;
    IDictionary<string, object> IUndeleteRequestHandler.StateBag => handler.StateBag;
    IDbConnection IUndeleteRequestHandler.Connection => handler.Connection;
    IUnitOfWork IUndeleteRequestHandler.UnitOfWork => handler.UnitOfWork;
    IRequestContext IUndeleteRequestHandler.Context => handler.Context;
}
