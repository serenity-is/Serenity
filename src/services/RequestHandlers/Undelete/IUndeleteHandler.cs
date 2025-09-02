namespace Serenity.Services;

/// <summary>
/// Interface for undelete request handlers with custom undelete request / response types.
/// </summary>
/// <typeparam name="TRow">Row type</typeparam>
/// <typeparam name="TUndeleteRequest">Undelete request type</typeparam>
/// <typeparam name="TUndeleteResponse">Undelete response type</typeparam>
public interface IUndeleteHandler<TRow, TUndeleteRequest, TUndeleteResponse>
    : IRequestHandler<TRow, TUndeleteRequest, TUndeleteResponse>
    where TRow : class, IRow, new()
    where TUndeleteRequest : UndeleteRequest
    where TUndeleteResponse : UndeleteResponse, new()
{
    /// <summary>
    /// Processes a undelete request
    /// </summary>
    /// <param name="uow">Unit of work</param>
    /// <param name="request">The undelete request</param>
    TUndeleteResponse Undelete(IUnitOfWork uow, TUndeleteRequest request);
}

/// <summary>
/// Interface for undelete request handlers that use <see cref="UndeleteRequest"/> as request,
/// and <see cref="UndeleteResponse"/> as response types.
/// </summary>
/// <typeparam name="TRow">Row type</typeparam>
public interface IUndeleteHandler<TRow>
    : IUndeleteHandler<TRow, UndeleteRequest, UndeleteResponse>
    where TRow : class, IRow, new()
{
}