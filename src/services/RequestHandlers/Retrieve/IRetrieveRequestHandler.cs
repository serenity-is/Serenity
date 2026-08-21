namespace Serenity.Services;

/// <summary>
/// Represents a RetrieveRequestHandler. Is used with RetrieveBehavior objects.
/// </summary>
[GenericHandlerType(typeof(RetrieveRequestHandler<>))]
public interface IRetrieveRequestHandler : IRequestHandler
{
    /// <summary>
    /// Gets the row used for querying / metadata lookup.
    /// </summary>
    IRow Row { get; }

    /// <summary>
    /// Gets the retrieve request.
    /// </summary>
    RetrieveRequest Request { get; }

    /// <summary>
    /// Gets the retrieve response.
    /// </summary>
    IRetrieveResponse Response { get; }

    /// <summary>
    /// Gets a state bag that can be used as storage within a request handler context.
    /// </summary>
    IDictionary<string, object> StateBag { get; }

    /// <summary>
    /// Gets the current connection.
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
    /// Gets the current request context.
    /// </summary>
    IRequestContext Context { get; }
}