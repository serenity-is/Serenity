namespace Serenity.Services;

/// <summary>
/// Interface for async list request handlers with custom list request / response types.
/// </summary>
/// <typeparam name="TRow">Row type</typeparam>
/// <typeparam name="TListRequest">List request type</typeparam>
/// <typeparam name="TListResponse">List response type</typeparam>
public interface IListHandlerAsync<TRow, TListRequest, TListResponse>
    : IRequestHandler<TRow, TListRequest, TListResponse>
    where TRow : class, IRow, new()
    where TListRequest : ListRequest
    where TListResponse : ListResponse<TRow>, new()
{
    /// <summary>
    /// Processes a List request asynchronously
    /// </summary>
    /// <param name="connection">Connection</param>
    /// <param name="request">List request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<TListResponse> ListAsync(IDbConnection connection, TListRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for async list request handlers that use <see cref="ListRequest"/> as request,
/// and <see cref="ListResponse{T}"/> as response types.
/// </summary>
/// <typeparam name="TRow">Row type</typeparam>
public interface IListHandlerAsync<TRow> : IListHandlerAsync<TRow, ListRequest, ListResponse<TRow>>,
    IListHandlerAsync<TRow, ListRequest>
    where TRow : class, IRow, new()
{
}
