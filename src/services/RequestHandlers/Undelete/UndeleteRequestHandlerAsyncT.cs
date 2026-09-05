namespace Serenity.Services;

/// <summary>
/// Base generic class for async undelete request handlers that use <see cref="UndeleteRequest"/> as
/// the request type, and <see cref="UndeleteResponse"/> as the response type.
/// </summary>
/// <typeparam name="TRow">The entity type</typeparam>
/// <remarks>
/// Initializes a new instance of the class.
/// </remarks>
/// <param name="context">The request context</param>
public class UndeleteRequestHandlerAsync<TRow>(IRequestContext context) : UndeleteRequestHandlerAsync<TRow, UndeleteRequest, UndeleteResponse>(context),
    IUndeleteHandlerAsync<TRow>
    where TRow : class, IRow, IIdRow, new()
{
}
