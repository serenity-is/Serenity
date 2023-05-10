namespace Serenity.Services;

/// <summary>
/// Interface for update request handlers
/// </summary>
/// <typeparam name="TRow">Row type</typeparam>
public interface IUpdateHandler<TRow>
    : IUpdateHandler<TRow, SaveRequest<TRow>, SaveResponse>
    where TRow : class, IRow, IIdRow, new()
{
}