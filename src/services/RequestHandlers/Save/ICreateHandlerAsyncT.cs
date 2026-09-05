namespace Serenity.Services;

/// <summary>
/// Interface for async create request handlers that use <see cref="SaveRequest{TEntity}"/> as
/// the request type, and <see cref="SaveResponse"/> as the response type.
/// </summary>
/// <typeparam name="TRow">Row type</typeparam>
public interface ICreateHandlerAsync<TRow>
    : ICreateHandlerAsync<TRow, SaveRequest<TRow>, SaveResponse>
    where TRow : class, IRow, IIdRow, new()
{
}
