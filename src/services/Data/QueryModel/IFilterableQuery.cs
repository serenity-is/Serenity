namespace Serenity.Data;

/// <summary>
///   Interface for query classes (e.g. SqlSelect, SqlUpdate) having a where method to filter
///   records.
/// </summary>
public interface IFilterableQuery : IQueryWithParams
{
    /// <summary>
    ///   Filters a query by a criteria.
    /// </summary>
    /// <param name="criteria">
    ///   Filter criteria.
    /// </param>
    void Where(ICriteria? criteria);

    /// <summary>
    /// Gets the criteria passed to the query's WHERE method.
    /// </summary>
    /// <returns>A read-only list of WHERE criteria.</returns>
    IReadOnlyList<ICriteria> GetWhereCriteria();

    /// <summary>
    /// Gets the WHERE conditions as SQL text, without the WHERE keyword.
    /// </summary>
    /// <remarks>
    /// Criteria parameters are added when <see cref="Where(ICriteria?)"/> is called. Reading
    /// this clause repeatedly does not add duplicate parameters. Query-specific table references
    /// are normalized where needed.
    /// </remarks>
    /// <returns>The WHERE conditions joined with AND, or an empty string.</returns>
    string GetWhereClause();
}
