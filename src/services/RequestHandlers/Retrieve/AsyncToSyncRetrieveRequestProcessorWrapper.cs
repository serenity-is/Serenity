namespace Serenity.Services;

/// <summary>
/// Wraps an asynchronous <see cref="IRetrieveRequestProcessorAsync"/> custom handler and exposes it
/// as a synchronous <see cref="IRetrieveRequestProcessor"/>. Used by
/// <see cref="DefaultHandlerFactory"/> when a synchronous retrieve handler is requested for a row
/// that only has an asynchronous custom retrieve handler.
/// </summary>
/// <typeparam name="TRow">Row type</typeparam>
internal class AsyncToSyncRetrieveRequestProcessorWrapper<TRow>(IRetrieveRequestProcessorAsync handler) : IRetrieveRequestProcessor,
    IRetrieveHandler<TRow>
    where TRow : class, IRow, IIdRow, new()
{
    private readonly IRetrieveRequestProcessorAsync handler = handler ??
        throw new ArgumentNullException(nameof(handler));

    /// <summary>
    /// Gets the wrapped asynchronous handler.
    /// </summary>
    public IRetrieveRequestProcessorAsync WrappedHandler => handler;

    /// <inheritdoc/>
    public IRetrieveResponse Process(IDbConnection connection, RetrieveRequest request)
    {
        return handler.ProcessAsync(connection, request).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public RetrieveResponse<TRow> Retrieve(IDbConnection connection, RetrieveRequest request)
    {
        return (RetrieveResponse<TRow>)Process(connection, request);
    }

    IRow IRetrieveRequestHandler.Row => handler.Row;
    RetrieveRequest IRetrieveRequestHandler.Request => handler.Request;
    IRetrieveResponse IRetrieveRequestHandler.Response => handler.Response;
    IDictionary<string, object> IRetrieveRequestHandler.StateBag => handler.StateBag;
    IDbConnection IRetrieveRequestHandler.Connection => handler.Connection;
    bool IRetrieveRequestHandler.AllowSelectField(Field field) => handler.AllowSelectField(field);
    bool IRetrieveRequestHandler.ShouldSelectField(Field field) => handler.ShouldSelectField(field);
    IRequestContext IRetrieveRequestHandler.Context => handler.Context;
}
