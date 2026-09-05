namespace Serenity.Services;

/// <summary>
/// Base generic class for async retrieve request handlers that use <see cref="RetrieveRequest"/> as
/// the request type, and <see cref="RetrieveResponse{T}"/> as the response type.
/// </summary>
/// <typeparam name="TRow">The entity type</typeparam>
/// <remarks>
/// Initializes a new instance of the class.
/// </remarks>
/// <param name="context">The request context</param>
public class RetrieveRequestHandlerAsync<TRow>(IRequestContext context) : RetrieveRequestHandlerAsync<TRow, RetrieveRequest, RetrieveResponse<TRow>>(context),
    IRetrieveHandlerAsync<TRow>
    where TRow : class, IRow, new()
{
}
