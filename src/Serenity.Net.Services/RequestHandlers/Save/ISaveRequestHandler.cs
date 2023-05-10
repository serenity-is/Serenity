namespace Serenity.Services;

/// <summary>
/// Represents a SaveRequestHandler. Is used with SaveBehavior objects.
/// </summary>
[GenericHandlerType(typeof(SaveRequestHandler<>))]
public interface ISaveRequestHandler : IRequestHandler
{
    /// <summary>
    /// Old row, if any, otherwise null
    /// </summary>
    IRow Old { get; }

    /// <summary>
    /// New row
    /// </summary>
    IRow Row { get; }

    /// <summary>
    /// Is this an INSERT operation?
    /// </summary>
    bool IsCreate { get; }

    /// <summary>
    /// Is this an UPDATE operation?
    /// </summary>
    bool IsUpdate { get; }

    /// <summary>
    /// Save request
    /// </summary>
    ISaveRequest Request { get; }

    /// <summary>
    /// Save response
    /// </summary>
    SaveResponse Response { get; }

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