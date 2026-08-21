namespace Serenity.Services;

/// <summary>
/// Represents a UndeleteRequestHandler. Is used with UndeleteBehavior objects.
/// </summary>
[GenericHandlerType(typeof(UndeleteRequestHandler<>))]
public interface IUndeleteRequestHandler : IRequestHandler
{
    /// <summary>
    /// Gets the row being undeleted.
    /// </summary>
    IRow Row { get; }

    /// <summary>
    /// Gets the undelete request.
    /// </summary>
    UndeleteRequest Request { get; }

    /// <summary>
    /// Gets the undelete response.
    /// </summary>
    UndeleteResponse Response { get; }

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
