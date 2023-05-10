namespace Serenity.Services;

/// <summary>
/// Represents a ListRequestHandler. Is used with ListBehavior objects.
/// </summary>
[GenericHandlerType(typeof(ListRequestHandler<>))]
public interface IListRequestHandler : IRequestHandler
{
    /// <summary>
    /// Loader row
    /// </summary>
    IRow Row { get; }

    /// <summary>
    /// List request
    /// </summary>
    ListRequest Request { get; }

    /// <summary>
    /// List response
    /// </summary>
    IListResponse Response { get; }

    /// <summary>
    /// A state bag that can be used as storage within a request handler context
    /// </summary>
    IDictionary<string, object> StateBag { get; }

    /// <summary>
    /// Current connection
    /// </summary>
    IDbConnection Connection { get; }

    /// <summary>
    /// Current request context
    /// </summary>
    IRequestContext Context { get; }

    /// <summary>
    /// Returns true if field is allowed to be selected, based on permissions and SelectLevel.Never.
    /// </summary>
    /// <param name="field">Field</param>
    /// <returns>True if field should be selected</returns>
    bool AllowSelectField(Field field);

    /// <summary>
    /// Returns true if field should be selected based on ColumnSelection flags.
    /// </summary>
    /// <param name="field">Field</param>
    /// <returns>True if field should be selected</returns>
    bool ShouldSelectField(Field field);

    /// <summary>
    /// Use to ignore an equality filter
    /// </summary>
    /// <param name="field">Field name or property name</param>
    void IgnoreEqualityFilter(string field);
}