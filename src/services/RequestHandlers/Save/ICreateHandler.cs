namespace Serenity.Services;

/// <summary>
/// Interface for create request handlers with custom request / response types.
/// </summary>
/// <typeparam name="TRow">Row type</typeparam>
/// <typeparam name="TSaveRequest">Save request type</typeparam>
/// <typeparam name="TSaveResponse">Save response type</typeparam>
public interface ICreateHandler<TRow, TSaveRequest, TSaveResponse>
    : IRequestHandler<TRow, TSaveRequest, TSaveResponse>
    where TRow : class, IRow, IIdRow, new()
    where TSaveRequest : SaveRequest<TRow>, new()
    where TSaveResponse : SaveResponse, new()
{
    /// <summary>
    /// Processes a Create request
    /// </summary>
    /// <param name="uow">Unit of work</param>
    /// <param name="request">Save request</param>
    TSaveResponse Create(IUnitOfWork uow, TSaveRequest request);
}