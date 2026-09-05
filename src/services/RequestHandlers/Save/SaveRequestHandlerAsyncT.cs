namespace Serenity.Services;

/// <summary>
/// Base generic class for async save request handlers that use <see cref="SaveRequest{TEntity}"/> as
/// the request type, and <see cref="SaveResponse"/> as the response type.
/// </summary>
/// <typeparam name="TRow">The entity type</typeparam>
/// <remarks>
/// Initializes a new instance of the class.
/// </remarks>
/// <param name="context">Request context</param>
public class SaveRequestHandlerAsync<TRow>(IRequestContext context) : SaveRequestHandlerAsync<TRow, SaveRequest<TRow>, SaveResponse>(context),
    ISaveHandlerAsync<TRow>
    where TRow : class, IRow, IIdRow, new()
{
}
