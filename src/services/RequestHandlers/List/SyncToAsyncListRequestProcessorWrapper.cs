namespace Serenity.Services;

/// <summary>
/// Wraps a synchronous <see cref="IListRequestProcessor"/> custom handler and exposes it
/// as an asynchronous <see cref="IListRequestProcessorAsync"/>. Used by
/// <see cref="DefaultHandlerFactory"/> when an async list handler is requested for a row
/// that only has a synchronous custom list handler.
/// </summary>
/// <typeparam name="TRow">Row type</typeparam>
internal class SyncToAsyncListRequestProcessorWrapper<TRow>(IListRequestProcessor handler) : IListRequestProcessorAsync,
    IListHandlerAsync<TRow>
    where TRow : class, IRow, IIdRow, new()
{
    private readonly IListRequestProcessor handler = handler ??
        throw new ArgumentNullException(nameof(handler));

    /// <summary>
    /// Gets the wrapped synchronous handler.
    /// </summary>
    public IListRequestProcessor WrappedHandler => handler;

    /// <inheritdoc/>
    public Task<IListResponse> ProcessAsync(IDbConnection connection, ListRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(handler.Process(connection, request));
    }

    /// <inheritdoc/>
    public Task<ListResponse<TRow>> ListAsync(IDbConnection connection, ListRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult((ListResponse<TRow>)handler.Process(connection, request));
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
