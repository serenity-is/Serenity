namespace Serenity.Services;

/// <summary>
/// Wraps an asynchronous <see cref="IListRequestProcessorAsync"/> custom handler and exposes it
/// as a synchronous <see cref="IListRequestProcessor"/>. Used by
/// <see cref="DefaultHandlerFactory"/> when a synchronous list handler is requested for a row
/// that only has an asynchronous custom list handler.
/// </summary>
/// <typeparam name="TRow">Row type</typeparam>
internal class AsyncToSyncListRequestProcessorWrapper<TRow>(IListRequestProcessorAsync handler) : IListRequestProcessor,
    IListHandler<TRow>
    where TRow : class, IRow, IIdRow, new()
{
    private readonly IListRequestProcessorAsync handler = handler ??
        throw new ArgumentNullException(nameof(handler));

    /// <summary>
    /// Gets the wrapped asynchronous handler.
    /// </summary>
    public IListRequestProcessorAsync WrappedHandler => handler;

    /// <inheritdoc/>
    public IListResponse Process(IDbConnection connection, ListRequest request)
    {
        return handler.ProcessAsync(connection, request).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public ListResponse<TRow> List(IDbConnection connection, ListRequest request)
    {
        return (ListResponse<TRow>)Process(connection, request);
    }

    IRow IListRequestHandler.Row => handler.Row;
    ListRequest IListRequestHandler.Request => handler.Request;
    IListResponse IListRequestHandler.Response => handler.Response;
    IDictionary<string, object> IListRequestHandler.StateBag => handler.StateBag;
    IDbConnection IListRequestHandler.Connection => handler.Connection;
    IRequestContext IListRequestHandler.Context => handler.Context;
    bool IListRequestHandler.AllowSelectField(Field field) => handler.AllowSelectField(field);
    bool IListRequestHandler.ShouldSelectField(Field field) => handler.ShouldSelectField(field);
    void IListRequestHandler.IgnoreEqualityFilter(string field) => handler.IgnoreEqualityFilter(field);
}
