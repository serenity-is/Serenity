namespace Serenity.Services;

/// <summary>
/// Base generic class for async delete request handlers that use <see cref="DeleteRequest"/> as
/// the request type, and <see cref="DeleteResponse"/> as the response type.
/// </summary>
/// <typeparam name="TRow">The entity type</typeparam>
/// <remarks>
/// Initializes a new instance of the class.
/// </remarks>
/// <param name="context">The request context</param>
public class DeleteRequestHandlerAsync<TRow>(IRequestContext context) : DeleteRequestHandlerAsync<TRow, DeleteRequest, DeleteResponse>(context),
    IDeleteHandlerAsync<TRow>
    where TRow : class, IRow, IIdRow, new()
{
}
