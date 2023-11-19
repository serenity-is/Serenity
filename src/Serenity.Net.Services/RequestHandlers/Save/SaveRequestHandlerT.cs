namespace Serenity.Services;

/// <summary>
/// Base generic class for save request handlers that use <see cref="SaveRequest{TEntity}"/> as
/// the request type, and <see cref="SaveResponse"/> as the response type.
/// </summary>
/// <typeparam name="TRow">The entity type</typeparam>
/// <remarks>
/// Creates a new instance of the class
/// </remarks>
/// <param name="context">Request context</param>
public class SaveRequestHandler<TRow>(IRequestContext context) : SaveRequestHandler<TRow, SaveRequest<TRow>, SaveResponse>(context),
    ISaveHandler<TRow>
    where TRow : class, IRow, IIdRow, new()
{
}
