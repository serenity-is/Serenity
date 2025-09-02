namespace Serenity.Services;

/// <summary>
/// Abstraction for retrieve request handlers with a Process method.
/// </summary>
[GenericHandlerType(typeof(RetrieveRequestHandler<>))]
public interface IRetrieveRequestProcessor : IRetrieveRequestHandler
{
    /// <summary>
    /// Processes the <see cref="RetrieveRequest"/> and returns a <see cref="RetrieveResponse{T}"/>
    /// </summary>
    /// <param name="connection">Connection</param>
    /// <param name="request">Retrieve request</param>
    IRetrieveResponse Process(IDbConnection connection, RetrieveRequest request);
}