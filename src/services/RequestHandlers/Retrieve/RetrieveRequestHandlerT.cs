namespace Serenity.Services;

/// <summary>
/// Base generic class for retrieve request handlers that use <see cref="RetrieveRequest"/> as
/// the request type, and <see cref="RetrieveResponse{T}"/> as the response type.
/// </summary>
/// <typeparam name="TRow">The entity type</typeparam>
/// <remarks>
/// Creates an instance of the class
/// </remarks>
/// <param name="context">The request context</param>
public class RetrieveRequestHandler<TRow>(IRequestContext context) : RetrieveRequestHandler<TRow, RetrieveRequest, RetrieveResponse<TRow>>(context),
    IRetrieveHandler<TRow>
    where TRow : class, IRow, new()
{
}