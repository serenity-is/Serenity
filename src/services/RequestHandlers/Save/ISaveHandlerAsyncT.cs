namespace Serenity.Services;

/// <summary>
/// Interface for async save request handlers that use <see cref="SaveRequest{TEntity}"/> as
/// the request type, and <see cref="SaveResponse"/> as the response type.
/// </summary>
/// <typeparam name="TRow">Row type</typeparam>
public interface ISaveHandlerAsync<TRow> : ISaveHandlerAsync<TRow, SaveRequest<TRow>, SaveResponse>,
    ICreateHandlerAsync<TRow>, IUpdateHandlerAsync<TRow>
    where TRow : class, IRow, IIdRow, new()
{
}
