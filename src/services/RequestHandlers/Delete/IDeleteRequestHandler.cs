namespace Serenity.Services;

/// <summary>
/// Represents a DeleteRequestHandler. Is used with DeleteBehavior objects.
/// </summary>
[GenericHandlerType(typeof(DeleteRequestHandler<>))]
public interface IDeleteRequestHandler : IRequestHandler
{
    /// <summary>
    /// Gets the row being deleted.
    /// </summary>
    IRow Row { get; }

    /// <summary>
    /// Gets the delete request.
    /// </summary>
    DeleteRequest Request { get; }

    /// <summary>
    /// Gets the delete response.
    /// </summary>
    DeleteResponse Response { get; }

    /// <summary>
    /// Gets a state bag that can be used as storage within a request handler context.
    /// </summary>
    IDictionary<string, object> StateBag { get; }

    /// <summary>
    /// Gets the current connection.
    /// </summary>
    IDbConnection Connection { get; }

    /// <summary>
    /// Gets the current unit of work.
    /// </summary>
    IUnitOfWork UnitOfWork { get; }

    /// <summary>
    /// Gets the current request context.
    /// </summary>
    IRequestContext Context { get; }
}
