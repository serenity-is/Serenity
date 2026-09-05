namespace Serenity.Services;

/// <summary>
/// Interface for async retrieve request handlers with custom retrieve request / response types.
/// </summary>
/// <typeparam name="TRow">Row type</typeparam>
/// <typeparam name="TRetrieveRequest">Retrieve request type</typeparam>
/// <typeparam name="TRetrieveResponse">Retrieve response type</typeparam>
public interface IRetrieveHandlerAsync<TRow, TRetrieveRequest, TRetrieveResponse>
    : IRequestHandler<TRow, TRetrieveRequest, TRetrieveResponse>
    where TRow : class, IRow, new()
    where TRetrieveRequest : RetrieveRequest
    where TRetrieveResponse : RetrieveResponse<TRow>, new()
{
    /// <summary>
    /// Processes a Retrieve request asynchronously.
    /// </summary>
    /// <param name="connection">Connection</param>
    /// <param name="request">Retrieve request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The retrieve response.</returns>
    Task<TRetrieveResponse> RetrieveAsync(IDbConnection connection, TRetrieveRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for async retrieve request handlers that use <see cref="RetrieveRequest"/> as request,
/// and <see cref="RetrieveResponse{T}"/> as response types.
/// </summary>
/// <typeparam name="TRow">Row type</typeparam>
public interface IRetrieveHandlerAsync<TRow>
    : IRetrieveHandlerAsync<TRow, RetrieveRequest, RetrieveResponse<TRow>>
    where TRow : class, IRow, new()
{
}
