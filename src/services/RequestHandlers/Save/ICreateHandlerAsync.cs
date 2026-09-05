namespace Serenity.Services;

/// <summary>
/// Abstraction for create request handlers with an async Create method.
/// </summary>
/// <typeparam name="TRow">Row type</typeparam>
/// <typeparam name="TSaveRequest">Save request type</typeparam>
/// <typeparam name="TSaveResponse">Save response type</typeparam>
public interface ICreateHandlerAsync<TRow, TSaveRequest, TSaveResponse>
    : IRequestHandler<TRow, TSaveRequest, TSaveResponse>
    where TRow : class, IRow, IIdRow, new()
    where TSaveRequest : SaveRequest<TRow>, new()
    where TSaveResponse : SaveResponse, new()
{
    /// <summary>
    /// Processes a Create request asynchronously
    /// </summary>
    /// <param name="uow">Unit of work</param>
    /// <param name="request">Save request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<TSaveResponse> CreateAsync(IUnitOfWork uow, TSaveRequest request, CancellationToken cancellationToken = default);
}
