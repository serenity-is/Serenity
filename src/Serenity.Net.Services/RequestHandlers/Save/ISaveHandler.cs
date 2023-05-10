namespace Serenity.Services;

/// <summary>
/// Interface for save request handlers with custom request / response types.
/// </summary>
/// <typeparam name="TRow">Row type</typeparam>
/// <typeparam name="TSaveRequest">Save request type</typeparam>
/// <typeparam name="TSaveResponse">Save response type</typeparam>
public interface ISaveHandler<TRow, TSaveRequest, TSaveResponse>
    : ICreateHandler<TRow, TSaveRequest, TSaveResponse>, IUpdateHandler<TRow, TSaveRequest, TSaveResponse>
    where TRow : class, IRow, IIdRow, new()
    where TSaveRequest : SaveRequest<TRow>, new()
    where TSaveResponse : SaveResponse, new()
{
}