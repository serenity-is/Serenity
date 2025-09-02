namespace Serenity.Services;

/// <summary>
/// Represents a UndeleteRequestHandler. Is used with UndeleteBehavior objects.
/// </summary>
[GenericHandlerType(typeof(UndeleteRequestHandler<>))]
public interface IUndeleteRequestHandler : IRequestHandler
{
    /// <summary>
    /// New row
    /// </summary>
    IRow Row { get; }

    /// <summary>
    /// Undelete request
    /// </summary>
    UndeleteRequest Request { get; }

    /// <summary>
    /// Delete response
    /// </summary>
    UndeleteResponse Response { get; }

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
