namespace Serenity.Services;

/// <summary>
/// Base generic class for undelete request handlers that use <see cref="UndeleteRequest"/> as
/// the request type, and <see cref="UndeleteResponse"/> as the response type.
/// </summary>
/// <typeparam name="TRow">The entity type</typeparam>
public class UndeleteRequestHandler<TRow> : UndeleteRequestHandler<TRow, UndeleteRequest, UndeleteResponse>,
    IUndeleteHandler<TRow>
    where TRow : class, IRow, IIdRow, new()
{
    /// <summary>
    /// Creates an instance of the class
    /// </summary>
    /// <param name="context">The request context</param>
    public UndeleteRequestHandler(IRequestContext context)
        : base(context)
    {
    }
}