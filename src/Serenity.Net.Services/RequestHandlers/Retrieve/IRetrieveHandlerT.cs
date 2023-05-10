namespace Serenity.Services;

/// <summary>
/// Interface for list request handlers
/// </summary>
/// <typeparam name="TRow">Row type</typeparam>
public interface IRetrieveHandler<TRow>
    : IRetrieveHandler<TRow, RetrieveRequest, RetrieveResponse<TRow>>
    where TRow : class, IRow, new()
{
}