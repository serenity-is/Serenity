namespace Serenity.Services;

/// <summary>
/// Wraps a synchronous <see cref="IDeleteRequestProcessor"/> custom handler and exposes it
/// as an asynchronous <see cref="IDeleteRequestProcessorAsync"/>. Used by
/// <see cref="DefaultHandlerFactory"/> when an async delete handler is requested for a row
/// that only has a synchronous custom delete handler.
/// </summary>
/// <typeparam name="TRow">Row type</typeparam>
internal class SyncToAsyncDeleteRequestProcessorWrapper<TRow>(IDeleteRequestProcessor handler) : IDeleteRequestProcessorAsync,
    IDeleteHandlerAsync<TRow>
    where TRow : class, IRow, IIdRow, new()
{
    private readonly IDeleteRequestProcessor handler = handler ??
        throw new ArgumentNullException(nameof(handler));

    /// <summary>
    /// Gets the wrapped synchronous handler.
    /// </summary>
    public IDeleteRequestProcessor WrappedHandler => handler;

    /// <inheritdoc/>
    public Task<DeleteResponse> ProcessAsync(IUnitOfWork uow, DeleteRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(handler.Process(uow, request));
    }

    /// <inheritdoc/>
    public Task<DeleteResponse> DeleteAsync(IUnitOfWork uow, DeleteRequest request, CancellationToken cancellationToken = default)
    {
        return ProcessAsync(uow, request, cancellationToken);
    }

    IRow IDeleteRequestHandler.Row => handler.Row;
    DeleteRequest IDeleteRequestHandler.Request => handler.Request;
    DeleteResponse IDeleteRequestHandler.Response => handler.Response;
    IDictionary<string, object> IDeleteRequestHandler.StateBag => handler.StateBag;
    IDbConnection IDeleteRequestHandler.Connection => handler.Connection;
    IUnitOfWork IDeleteRequestHandler.UnitOfWork => handler.UnitOfWork;
    IRequestContext IDeleteRequestHandler.Context => handler.Context;
}
