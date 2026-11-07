namespace Serenity.Services;

/// <summary>
/// Specifies how an entity is read when it is used as a source of another query, e.g. by an entity query of a
/// data source, see <see cref="EntityQueryAttribute"/>. Each mode decides which filters of the application apply,
/// e.g. those limiting the rows to a tenant or the user's own records.
/// </summary>
public enum EntityQueryMode
{
    /// <summary>
    /// The entity is read through the query its list handler builds, with the handler's and its behaviors' filters,
    /// see <see cref="IListQueryCapture"/>. This is the default, while a consumer may still refuse a handler that
    /// does something after its query is executed, e.g. drops rows in ProcessEntity or OnReturn, unless the entity
    /// specifies this mode explicitly, which confirms that all its filtering is done in the query.
    /// </summary>
    ListHandler = 0,

    /// <summary>
    /// The entity is read through the query of a generic list handler with the entity's list behaviors, ignoring
    /// its custom list handler, e.g. when that one is only meant for a page, while the behaviors do all the filtering.
    /// </summary>
    Behaviors = 1,

    /// <summary>
    /// The entity is read from its table and joins, with the permissions declared on it and its fields, but
    /// without any filter of its list handler or behaviors. Consumers should not allow it for entities whose rows
    /// must be filtered, e.g. multi tenant ones.
    /// </summary>
    Unfiltered = 2,

    /// <summary>
    /// The entity can't be used as a source of another query.
    /// </summary>
    Disabled = 3
}
