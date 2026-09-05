namespace Serenity.Services;

/// <summary>
/// Abstraction for list request handlers with a Process method.
/// </summary>
[GenericHandlerType(typeof(ListRequestHandler<>))]
[CompanionHandlerType(typeof(IListRequestProcessorAsync), typeof(AsyncToSyncListRequestProcessorWrapper<>))]
public interface IListRequestProcessor : IListRequestHandler
{
    /// <summary>
    /// Processes the <see cref="ListRequest"/> and returns a <see cref="ListResponse{T}"/>
    /// </summary>
    /// <param name="connection">Connection</param>
    /// <param name="request">List request</param>
    IListResponse Process(IDbConnection connection, ListRequest request);
}