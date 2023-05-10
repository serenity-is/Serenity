namespace Serenity.Services;

/// <summary>
/// Interface for create request handlers
/// </summary>
/// <typeparam name="TRow">Row type</typeparam>
public interface ICreateHandler<TRow>
    : ICreateHandler<TRow, SaveRequest<TRow>, SaveResponse>
    where TRow : class, IRow, IIdRow, new()
{
}