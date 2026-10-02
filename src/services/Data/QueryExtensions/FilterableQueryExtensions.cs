
namespace Serenity.Data;

/// <summary>
///   Extensions for objects implementing <see cref="IFilterableQuery"/>.
/// </summary>
public static class FilterableQueryExtensions
{
    /// <summary>
    ///   Adds a filter string to query.
    /// </summary>
    /// <typeparam name="T">
    ///   Query class.
    /// </typeparam>
    /// <param name="self">
    ///   Query.
    /// </param>
    /// <param name="filter">
    ///   Filter string.
    /// </param>
    /// <returns>
    ///   Query itself.
    /// </returns>
    public static T Where<T>(this T self, string filter) where T : IFilterableQuery
    {
        ArgumentNullException.ThrowIfNull(self);
        if (string.IsNullOrEmpty(filter))
            throw new ArgumentNullException(nameof(filter));

        self.Where(new Criteria(filter));
        return self;
    }

    /// <summary>
    ///   Adds a where statement with equality filter to a query, and sets the parameter value with a parameter.
    /// </summary>
    /// <param name="self">
    ///   Query.
    /// </param>
    /// <param name="field">
    ///   Field.
    /// </param>
    /// <param name="value">
    ///   Parameter value.
    /// </param>
    /// <param name="emitIsNull">
    ///   When true and <paramref name="value"/> is null, an IS NULL filter is
    ///   emitted instead of an equality check. This is false by default, in which
    ///   case null produces a plain equality against NULL that never matches
    ///   (matching SQL three-valued logic and Criteria equality semantics).
    /// </param>
    /// <returns>
    ///   The new filter parameter.
    /// </returns>
    public static T WhereEqual<T>(this T self, IField field, object? value, bool emitIsNull = false) where T : IFilterableQuery
    {
        ArgumentNullException.ThrowIfNull(self);
        ArgumentNullException.ThrowIfNull(field);

        if (value is null && emitIsNull)
            self.Where(new Criteria(field).IsNull());
        else
            self.Where(new Criteria(field) == self.AddParam(value));
        return self;
    }
}
