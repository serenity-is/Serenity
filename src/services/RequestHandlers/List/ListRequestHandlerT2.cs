namespace Serenity.Services;

/// <summary>
/// Base generic class for list request handlers that use a custom list request as
/// the request type, and <see cref="ListResponse{T}"/> as the response type.
/// </summary>
/// <typeparam name="TRow">The entity type</typeparam>
/// <typeparam name="TListRequest">List request type</typeparam>
/// <remarks>
/// Creates a new instance of the class
/// </remarks>
/// <param name="context">Request context</param>
public class ListRequestHandler<TRow, TListRequest>(IRequestContext context) : ListRequestHandler<TRow, TListRequest, ListResponse<TRow>>(context),
    IListHandler<TRow, TListRequest>
    where TRow : class, IRow, new()
    where TListRequest : ListRequest
{
}