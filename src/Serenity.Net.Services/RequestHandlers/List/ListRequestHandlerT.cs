namespace Serenity.Services;

/// <summary>
/// Base generic class for list request handlers that use <see cref="ListRequest"/> as
/// the request type, and <see cref="ListResponse{T}"/> as the response type.
/// </summary>
/// <typeparam name="TRow">The entity type</typeparam>
/// <remarks>
/// Creates an instance of the class
/// </remarks>
/// <param name="context">The request context</param>
public class ListRequestHandler<TRow>(IRequestContext context) : ListRequestHandler<TRow, ListRequest, ListResponse<TRow>>(context),
    IListHandler<TRow>, IListHandler<TRow, ListRequest>
    where TRow : class, IRow, new()
{
}