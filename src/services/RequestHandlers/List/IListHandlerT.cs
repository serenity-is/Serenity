namespace Serenity.Services;

/// <summary>
/// Interface for list request handlers
/// </summary>
/// <typeparam name="TRow">Row type</typeparam>
public interface IListHandler<TRow>
    : IListHandler<TRow, ListRequest, ListResponse<TRow>>
    where TRow : class, IRow, new()
{
}