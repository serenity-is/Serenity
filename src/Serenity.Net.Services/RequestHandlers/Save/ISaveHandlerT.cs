namespace Serenity.Services;

/// <summary>
/// Interface for save request handlers
/// </summary>
/// <typeparam name="TRow">Row type</typeparam>
public interface ISaveHandler<TRow> : ISaveHandler<TRow, SaveRequest<TRow>, SaveResponse>,
    ICreateHandler<TRow>, IUpdateHandler<TRow>
    where TRow : class, IRow, IIdRow, new()
{
}