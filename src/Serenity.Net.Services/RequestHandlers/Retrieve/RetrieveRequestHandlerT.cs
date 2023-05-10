namespace Serenity.Services;

/// <summary>
/// Base generic class for retrieve request handlers that use <see cref="RetrieveRequest"/> as
/// the request type, and <see cref="RetrieveResponse{T}"/> as the response type.
/// </summary>
/// <typeparam name="TRow">The entity type</typeparam>
public class RetrieveRequestHandler<TRow> : RetrieveRequestHandler<TRow, RetrieveRequest, RetrieveResponse<TRow>>,
    IRetrieveHandler<TRow>
    where TRow : class, IRow, new()
{
    /// <summary>
    /// Creates an instance of the class
    /// </summary>
    /// <param name="context">The request context</param>
    public RetrieveRequestHandler(IRequestContext context)
        : base(context)
    {
    }
}