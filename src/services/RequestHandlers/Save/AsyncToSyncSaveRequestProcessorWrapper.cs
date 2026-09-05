namespace Serenity.Services;

/// <summary>
/// Wraps an asynchronous <see cref="ISaveRequestProcessorAsync"/> custom handler and exposes it
/// as a synchronous <see cref="ISaveRequestProcessor"/>. Used by
/// <see cref="DefaultHandlerFactory"/> when a synchronous save handler is requested for a row
/// that only has an asynchronous custom save handler.
/// </summary>
/// <typeparam name="TRow">Row type</typeparam>
internal class AsyncToSyncSaveRequestProcessorWrapper<TRow>(ISaveRequestProcessorAsync handler) : ISaveRequestProcessor,
    ICreateHandler<TRow>, IUpdateHandler<TRow>
    where TRow : class, IRow, IIdRow, new()
{
    private readonly ISaveRequestProcessorAsync handler = handler ??
        throw new ArgumentNullException(nameof(handler));

    /// <summary>
    /// Gets the wrapped asynchronous handler.
    /// </summary>
    public ISaveRequestProcessorAsync WrappedHandler => handler;

    /// <inheritdoc/>
    public SaveResponse Process(IUnitOfWork uow, ISaveRequest request, SaveRequestType type)
    {
        return handler.ProcessAsync(uow, request, type).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public SaveResponse Create(IUnitOfWork uow, SaveRequest<TRow> request)
    {
        return Process(uow, request, SaveRequestType.Create);
    }

    /// <inheritdoc/>
    public SaveResponse Update(IUnitOfWork uow, SaveRequest<TRow> request)
    {
        return Process(uow, request, SaveRequestType.Update);
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
