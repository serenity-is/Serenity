namespace Serenity.Data;

/// <summary>
///   Interface for query classes (e.g. SqlSelect, SqlUpdate) having a where method to filter
///   records</summary>
public interface IFilterableQuery : IQueryWithParams
{
    /// <summary>
    ///   Filters a query by a filter string.</summary>
    /// <param name="filter">
    ///   Filter string.</param>
    /// <returns>
    ///   Object itself.</returns>
    void Where(string filter);
}
