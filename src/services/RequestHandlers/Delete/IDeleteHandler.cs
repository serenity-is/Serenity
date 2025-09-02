namespace Serenity.Services;

/// <summary>
/// Interface for delete request handlers with custom delete request / response types.
/// </summary>
/// <typeparam name="TRow">Row type</typeparam>
/// <typeparam name="TDeleteRequest">Delete request type</typeparam>
/// <typeparam name="TDeleteResponse">Delete response type</typeparam>
public interface IDeleteHandler<TRow, TDeleteRequest, TDeleteResponse>
    : IRequestHandler<TRow, TDeleteRequest, TDeleteResponse>
    where TRow : class, IRow, new()
    where TDeleteRequest : DeleteRequest
    where TDeleteResponse : DeleteResponse, new()
{
    /// <summary>
    /// Processes a delete request
    /// </summary>
    /// <param name="uow">Unit of work</param>
    /// <param name="request">The delete request</param>
    TDeleteResponse Delete(IUnitOfWork uow, TDeleteRequest request);
}

/// <summary>
/// Interface for delete request handlers that use <see cref="DeleteRequest"/> as request,
/// and <see cref="DeleteResponse"/> as response types.
/// </summary>
/// <typeparam name="TRow">Row type</typeparam>
public interface IDeleteHandler<TRow>
    : IDeleteHandler<TRow, DeleteRequest, DeleteResponse>
    where TRow : class, IRow, new()
{
}