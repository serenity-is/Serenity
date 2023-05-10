namespace Serenity.Services;

/// <summary>
/// Base generic class for delete request handlers that use <see cref="DeleteRequest"/> as
/// the request type, and <see cref="DeleteResponse"/> as the response type.
/// </summary>
/// <typeparam name="TRow">The entity type</typeparam>
public class DeleteRequestHandler<TRow> : DeleteRequestHandler<TRow, DeleteRequest, DeleteResponse>,
    IDeleteHandler<TRow>
    where TRow : class, IRow, IIdRow, new()
{
    /// <summary>
    /// Creates an instance of the class
    /// </summary>
    /// <param name="context">The request context</param>
    public DeleteRequestHandler(IRequestContext context)
        : base(context)
    {
    }
}