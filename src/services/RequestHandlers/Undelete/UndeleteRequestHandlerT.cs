namespace Serenity.Services;

/// <summary>
/// Base generic class for undelete request handlers that use <see cref="UndeleteRequest"/> as
/// the request type, and <see cref="UndeleteResponse"/> as the response type.
/// </summary>
/// <typeparam name="TRow">The entity type</typeparam>
/// <remarks>
/// Creates an instance of the class
/// </remarks>
/// <param name="context">The request context</param>
public class UndeleteRequestHandler<TRow>(IRequestContext context) : UndeleteRequestHandler<TRow, UndeleteRequest, UndeleteResponse>(context),
    IUndeleteHandler<TRow>
    where TRow : class, IRow, IIdRow, new()
{
}