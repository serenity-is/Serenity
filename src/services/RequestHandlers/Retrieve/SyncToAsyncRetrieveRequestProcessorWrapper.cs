namespace Serenity.Services;

/// <summary>
/// Wraps a synchronous <see cref="IRetrieveRequestProcessor"/> custom handler and exposes it
/// as an asynchronous <see cref="IRetrieveRequestProcessorAsync"/>. Used by
/// <see cref="DefaultHandlerFactory"/> when an async retrieve handler is requested for a row
/// that only has a synchronous custom retrieve handler.
/// </summary>
/// <typeparam name="TRow">Row type</typeparam>
internal class SyncToAsyncRetrieveRequestProcessorWrapper<TRow>(IRetrieveRequestProcessor handler) : IRetrieveRequestProcessorAsync,
    IRetrieveHandlerAsync<TRow>
    where TRow : class, IRow, IIdRow, new()
{
    private readonly IRetrieveRequestProcessor handler = handler ??
        throw new ArgumentNullException(nameof(handler));

    /// <summary>
    /// Gets the wrapped synchronous handler.
    /// </summary>
    public IRetrieveRequestProcessor WrappedHandler => handler;

    /// <inheritdoc/>
    public Task<IRetrieveResponse> ProcessAsync(IDbConnection connection, RetrieveRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(handler.Process(connection, request));
    }

    /// <inheritdoc/>
    public Task<RetrieveResponse<TRow>> RetrieveAsync(IDbConnection connection, RetrieveRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult((RetrieveResponse<TRow>)handler.Process(connection, request));
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
