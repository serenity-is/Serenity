namespace Serenity.Services;

/// <summary>
/// Interface for async update request handlers that use <see cref="SaveRequest{TEntity}"/> as
/// the request type, and <see cref="SaveResponse"/> as the response type.
/// </summary>
/// <typeparam name="TRow">Row type</typeparam>
public interface IUpdateHandlerAsync<TRow>
    : IUpdateHandlerAsync<TRow, SaveRequest<TRow>, SaveResponse>
    where TRow : class, IRow, IIdRow, new()
{
}
