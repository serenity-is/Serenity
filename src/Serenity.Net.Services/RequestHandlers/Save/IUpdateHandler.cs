namespace Serenity.Services;

/// <summary>
/// Interface for update request handlers with custom request / response types.
/// </summary>
/// <typeparam name="TRow">Row type</typeparam>
/// <typeparam name="TSaveRequest">Save request type</typeparam>
/// <typeparam name="TSaveResponse">Save response type</typeparam>
public interface IUpdateHandler<TRow, TSaveRequest, TSaveResponse>
    : IRequestHandler<TRow, TSaveRequest, TSaveResponse>
    where TRow : class, IRow, IIdRow, new()
    where TSaveRequest : SaveRequest<TRow>, new()
    where TSaveResponse : SaveResponse, new()
{
    /// <summary>
    /// Processes an Update request
    /// </summary>
    /// <param name="uow">Unit of work</param>
    /// <param name="request">Save request</param>
    TSaveResponse Update(IUnitOfWork uow, TSaveRequest request);
}