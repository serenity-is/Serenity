namespace Serenity.Services;

/// <summary>
/// Wraps a synchronous <see cref="ISaveRequestProcessor"/> custom handler and exposes it
/// as an asynchronous <see cref="ISaveRequestProcessorAsync"/>. Used by
/// <see cref="DefaultHandlerFactory"/> when an async save handler is requested for a row
/// that only has a synchronous custom save handler.
/// </summary>
/// <typeparam name="TRow">Row type</typeparam>
internal class SyncToAsyncSaveRequestProcessorWrapper<TRow>(ISaveRequestProcessor handler) : ISaveRequestProcessorAsync,
    ICreateHandlerAsync<TRow>, IUpdateHandlerAsync<TRow>
    where TRow : class, IRow, IIdRow, new()
{
    private readonly ISaveRequestProcessor handler = handler ??
        throw new ArgumentNullException(nameof(handler));

    /// <summary>
    /// Gets the wrapped synchronous handler.
    /// </summary>
    public ISaveRequestProcessor WrappedHandler => handler;

    /// <inheritdoc/>
    public Task<SaveResponse> ProcessAsync(IUnitOfWork uow, ISaveRequest request, SaveRequestType type,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(handler.Process(uow, request, type));
    }

    /// <inheritdoc/>
    public Task<SaveResponse> CreateAsync(IUnitOfWork uow, SaveRequest<TRow> request, CancellationToken cancellationToken = default)
    {
        return ProcessAsync(uow, request, SaveRequestType.Create, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<SaveResponse> UpdateAsync(IUnitOfWork uow, SaveRequest<TRow> request, CancellationToken cancellationToken = default)
    {
        return ProcessAsync(uow, request, SaveRequestType.Update, cancellationToken);
    }

    IRow ISaveRequestHandler.Old => handler.Old;
    IRow ISaveRequestHandler.Row => handler.Row;
    bool ISaveRequestHandler.IsCreate => handler.IsCreate;
    bool ISaveRequestHandler.IsUpdate => handler.IsUpdate;
    ISaveRequest ISaveRequestHandler.Request => handler.Request;
    SaveResponse ISaveRequestHandler.Response => handler.Response;
    IDictionary<string, object> ISaveRequestHandler.StateBag => handler.StateBag;
    IDbConnection ISaveRequestHandler.Connection => handler.Connection;
    IUnitOfWork ISaveRequestHandler.UnitOfWork => handler.UnitOfWork;
    IRequestContext ISaveRequestHandler.Context => handler.Context;
}
