namespace Serenity.Services;

/// <summary>
/// Represents a RetrieveRequestHandler. Is used with RetrieveBehavior objects.
/// </summary>
[GenericHandlerType(typeof(RetrieveRequestHandler<>))]
public interface IRetrieveRequestHandler : IRequestHandler
{
    /// <summary>
    /// Loader row
    /// </summary>
    IRow Row { get; }

    /// <summary>
    /// Retrieve request
    /// </summary>
    RetrieveRequest Request { get; }

    /// <summary>
    /// Retrieve response
    /// </summary>
    IRetrieveResponse Response { get; }

    /// <summary>
    /// A state bag that can be used as storage within a request handler context
    /// </summary>
    IDictionary<string, object> StateBag { get; }

    /// <summary>
    /// Current connection
    /// </summary>
    IDbConnection Connection { get; }

    /// <summary>
    /// Returns true if field is allowed to be selected based on permissions and SelectLevel.Never flag.
    /// </summary>
    /// <param name="field">Field</param>
    /// <returns>True if field is allowed to be selected</returns>
    bool AllowSelectField(Field field);

    /// <summary>
    /// Returns true if field should be selected based on ColumnSelection flags.
    /// </summary>
    /// <param name="field">Field</param>
    /// <returns>True if field should be selected</returns>
    bool ShouldSelectField(Field field);

    /// <summary>
    /// Current request context
    /// </summary>
    IRequestContext Context { get; }
}