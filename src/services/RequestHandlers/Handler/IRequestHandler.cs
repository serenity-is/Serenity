
namespace Serenity.Services;

/// <summary>
/// Marker interface for standard and custom request handlers
/// </summary>
public interface IRequestHandler
{
}

/// <summary>
/// Marker interface for request handlers that operate on a specific entity type.
/// </summary>
/// <typeparam name="TRow">The entity type</typeparam>
public interface IRequestHandler<TRow> : IRequestHandler
{
}

/// <summary>
/// Marker interface for request handlers that operate on a specific entity type,
/// request type and a response type.
/// </summary>
/// <typeparam name="TRow">The entity type</typeparam>
/// <typeparam name="TRequest">Request type</typeparam>
/// <typeparam name="TResponse">Response type</typeparam>
public interface IRequestHandler<TRow, TRequest, TResponse> :
    IRequestHandler<TRow>, IRequestType<TRequest>, IResponseType<TResponse>
{
}