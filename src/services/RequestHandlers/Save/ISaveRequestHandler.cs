namespace Serenity.Services;

/// <summary>
/// Represents a SaveRequestHandler. Is used with SaveBehavior objects.
/// </summary>
[GenericHandlerType(typeof(SaveRequestHandler<>))]
public interface ISaveRequestHandler : IRequestHandler
{
    /// <summary>
    /// Gets the old row, if any, otherwise <c>null</c>.
    /// </summary>
    IRow Old { get; }

    /// <summary>
    /// Gets the new row.
    /// </summary>
    IRow Row { get; }

    /// <summary>
    /// Gets a value indicating whether this is an INSERT operation.
    /// </summary>
    bool IsCreate { get; }

    /// <summary>
    /// Gets a value indicating whether this is an UPDATE operation.
    /// </summary>
    bool IsUpdate { get; }

    /// <summary>
    /// Gets the save request.
    /// </summary>
    ISaveRequest Request { get; }

    /// <summary>
    /// Gets the save response.
    /// </summary>
    SaveResponse Response { get; }

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