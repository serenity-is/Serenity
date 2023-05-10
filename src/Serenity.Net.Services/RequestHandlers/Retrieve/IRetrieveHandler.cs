namespace Serenity.Services;

/// <summary>
/// Interface for retrieve request handlers with custom retrieve request / response types.
/// </summary>
/// <typeparam name="TRow">Row type</typeparam>
/// <typeparam name="TRetrieveRequest">Retrieve request type</typeparam>
/// <typeparam name="TRetrieveResponse">Retrieve response type</typeparam>
public interface IRetrieveHandler<TRow, TRetrieveRequest, TRetrieveResponse>
    : IRequestHandler<TRow, TRetrieveRequest, TRetrieveResponse>
    where TRow : class, IRow, new()
    where TRetrieveRequest : RetrieveRequest
    where TRetrieveResponse : RetrieveResponse<TRow>, new()
{
    /// <summary>
    /// Processes a Retrieve request
    /// </summary>
    /// <param name="connection">Connection</param>
    /// <param name="request">Retrieve request</param>
    /// <returns></returns>
    TRetrieveResponse Retrieve(IDbConnection connection, TRetrieveRequest request);
}