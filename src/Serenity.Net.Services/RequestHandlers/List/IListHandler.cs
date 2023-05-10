namespace Serenity.Services;

/// <summary>
/// Interface for list request handlers with custom list request / response types.
/// </summary>
/// <typeparam name="TRow">Row type</typeparam>
/// <typeparam name="TListRequest">List request type</typeparam>
/// <typeparam name="TListResponse">List response type</typeparam>
public interface IListHandler<TRow, TListRequest, TListResponse>
    : IRequestHandler<TRow, TListRequest, TListResponse>
    where TRow : class, IRow, new()
    where TListRequest : ListRequest
    where TListResponse : ListResponse<TRow>, new()
{
    /// <summary>
    /// Processes a List request
    /// </summary>
    /// <param name="connection">Connection</param>
    /// <param name="request">List request</param>
    TListResponse List(IDbConnection connection, TListRequest request);
}