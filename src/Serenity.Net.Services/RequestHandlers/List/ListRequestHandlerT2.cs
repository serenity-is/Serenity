namespace Serenity.Services;

/// <summary>
/// Base generic class for list request handlers that use a custom list request as
/// the request type, and <see cref="ListResponse{T}"/> as the response type.
/// </summary>
/// <typeparam name="TRow">The entity type</typeparam>
/// <typeparam name="TListRequest">List request type</typeparam>
public class ListRequestHandler<TRow, TListRequest> : ListRequestHandler<TRow, TListRequest, ListResponse<TRow>>,
    IListHandler<TRow, TListRequest>
    where TRow : class, IRow, new()
    where TListRequest : ListRequest
{
    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    /// <param name="context">Request context</param>
    public ListRequestHandler(IRequestContext context)
        : base(context)
    {
    }
}