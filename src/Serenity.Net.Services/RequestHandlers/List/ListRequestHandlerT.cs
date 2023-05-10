namespace Serenity.Services;

/// <summary>
/// Base generic class for list request handlers that use <see cref="ListRequest"/> as
/// the request type, and <see cref="ListResponse{T}"/> as the response type.
/// </summary>
/// <typeparam name="TRow">The entity type</typeparam>
public class ListRequestHandler<TRow> : ListRequestHandler<TRow, ListRequest, ListResponse<TRow>>,
    IListHandler<TRow>, IListHandler<TRow, ListRequest>
    where TRow : class, IRow, new()
{
    /// <summary>
    /// Creates an instance of the class
    /// </summary>
    /// <param name="context">The request context</param>
    public ListRequestHandler(IRequestContext context)
        : base(context)
    {
    }
}