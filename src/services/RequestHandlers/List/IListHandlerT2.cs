namespace Serenity.Services;

/// <summary>
/// Interface for list request handlers with a custom list request type.
/// </summary>
/// <typeparam name="TRow">Row type</typeparam>
/// <typeparam name="TListRequest">List request type</typeparam>
public interface IListHandler<TRow, TListRequest>
    : IListHandler<TRow, TListRequest, ListResponse<TRow>>
    where TRow : class, IRow, new()
    where TListRequest : ListRequest
{
}