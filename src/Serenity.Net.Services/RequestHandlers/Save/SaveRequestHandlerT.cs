namespace Serenity.Services;

/// <summary>
/// Base generic class for save request handlers that use <see cref="SaveRequest{TEntity}"/> as
/// the request type, and <see cref="SaveResponse"/> as the response type.
/// </summary>
/// <typeparam name="TRow">The entity type</typeparam>
public class SaveRequestHandler<TRow> : SaveRequestHandler<TRow, SaveRequest<TRow>, SaveResponse>,
    ISaveHandler<TRow>
    where TRow : class, IRow, IIdRow, new()
{
    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    /// <param name="context">Request context</param>
    public SaveRequestHandler(IRequestContext context)
        : base(context)
    {
    }
}
