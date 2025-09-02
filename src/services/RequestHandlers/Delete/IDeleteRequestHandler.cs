namespace Serenity.Services;

/// <summary>
/// Represents a DeleteRequestHandler. Is used with DeleteBehavior objects.
/// </summary>
[GenericHandlerType(typeof(DeleteRequestHandler<>))]
public interface IDeleteRequestHandler : IRequestHandler
{
    /// <summary>
    /// New row
    /// </summary>
    IRow Row { get; }

    /// <summary>
    /// Delete request
    /// </summary>
    DeleteRequest Request { get; }

    /// <summary>
    /// Delete response
    /// </summary>
    DeleteResponse Response { get; }

    /// <summary>
    /// A state bag that can be used as storage within a request handler context
    /// </summary>
    IDictionary<string, object> StateBag { get; }

    /// <summary>
    /// Current connection
    /// </summary>
    IDbConnection Connection { get; }

    /// <summary>
    /// Current transaction
    /// </summary>
    IUnitOfWork UnitOfWork { get; }

    /// <summary>
    /// Current request context
    /// </summary>
    IRequestContext Context { get; }
}
