
namespace Serenity.Data;

/// <summary>
///   Extensions for objects implementing IDbWhere interface.</summary>
public static class FilterableQueryExtensions
{
    /// <summary>
    ///   Adds a filter to query</summary>
    /// <typeparam name="T">
    ///   Query class</typeparam>
    /// <param name="self">
    ///   Query</param>
    /// <param name="filter">
    ///   Filter</param>
    /// <returns>
    ///   Query itself.</returns>
    public static T Where<T>(this T self, ICriteria filter) where T : IFilterableQuery
    {
        if (filter is object && !filter.IsEmpty)
        {
            var statement = filter.ToString(self);
            self.Where(statement);
        }
        return self;
    }

    /// <summary>
    ///   Adds a where statement with equality filter to a query, and sets the parameter value with a parameter.</summary>
    /// <param field="field">
    ///   Field.</param>
    /// <param field="value">
    ///   Parameter value</param>
    /// <returns>
    ///   The new filter parameter.</returns>
    public static T WhereEqual<T>(this T self, IField field, object value) where T : IFilterableQuery
    {
        self.Where(new Criteria(field) == self.AddParam(value));
        return self;
    }
}