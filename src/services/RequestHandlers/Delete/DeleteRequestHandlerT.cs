namespace Serenity.Services;

/// <summary>
/// Base generic class for delete request handlers that use <see cref="DeleteRequest"/> as
/// the request type, and <see cref="DeleteResponse"/> as the response type.
/// </summary>
/// <typeparam name="TRow">The entity type</typeparam>
/// <remarks>
/// Creates an instance of the class
/// </remarks>
/// <param name="context">The request context</param>
public class DeleteRequestHandler<TRow>(IRequestContext context) : DeleteRequestHandler<TRow, DeleteRequest, DeleteResponse>(context),
    IDeleteHandler<TRow>
    where TRow : class, IRow, IIdRow, new()
{
}