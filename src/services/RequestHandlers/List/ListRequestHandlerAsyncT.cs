namespace Serenity.Services;

/// <summary>
/// Base generic class for async list request handlers that use <see cref="ListRequest"/> as
/// the request type, and <see cref="ListResponse{T}"/> as the response type.
/// </summary>
/// <typeparam name="TRow">The entity type</typeparam>
/// <remarks>
/// Initializes a new instance of the class.
/// </remarks>
/// <param name="context">The request context</param>
public class ListRequestHandlerAsync<TRow>(IRequestContext context) : ListRequestHandlerAsync<TRow, ListRequest, ListResponse<TRow>>(context),
    IListHandlerAsync<TRow>, IListHandlerAsync<TRow, ListRequest>
    where TRow : class, IRow, new()
{
}
