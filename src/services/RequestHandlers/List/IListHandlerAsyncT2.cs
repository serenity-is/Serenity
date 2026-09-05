namespace Serenity.Services;

/// <summary>
/// Interface for async list request handlers with a custom list request type.
/// </summary>
/// <typeparam name="TRow">Row type</typeparam>
/// <typeparam name="TListRequest">List request type</typeparam>
public interface IListHandlerAsync<TRow, TListRequest>
    : IListHandlerAsync<TRow, TListRequest, ListResponse<TRow>>
    where TRow : class, IRow, new()
    where TListRequest : ListRequest
{
}
