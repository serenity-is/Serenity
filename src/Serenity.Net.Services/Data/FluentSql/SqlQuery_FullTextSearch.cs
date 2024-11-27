namespace Serenity.Data;

public partial class SqlQuery
{
    /// <summary>
    ///   Creates the required join for full text search in MSSQL.
    /// </summary>
    /// <param name="searchTable">
    ///   Table name containing the fields to be searched (required).
    /// </param>
    /// <param name="searchFields">
    ///   Fields to be searched (required).
    /// </param>
    /// <param name="searchQuery">
    ///   Searched word or phrase (required). Words must be separated by comma.
    /// </param>
    /// <param name="searchTableAlias">
    ///   Alias assigned to the table to be searched in the FROM part of the query (required, e.g. T0).
    /// </param>
    /// <param name="searchTableKey">
    ///   Key (ID) field of the table to be searched (required).
    /// </param>
    /// <param name="containsAlias">
    ///   Alias to be assigned to the joined contains table (required, e.g. CT).
    /// </param>
    /// <returns>
    ///   The SqlSelect object itself.
    /// </returns>
    public SqlQuery FullTextSearchJoin(
        string searchTable, string searchFields, string searchQuery,
        string searchTableAlias, string searchTableKey, string containsAlias)
    {
        if (string.IsNullOrEmpty(searchTable))
            throw new ArgumentNullException("searchTable");
        if (string.IsNullOrEmpty(searchFields))
            throw new ArgumentNullException("searchFields");
        if (string.IsNullOrEmpty(searchQuery))
            throw new ArgumentNullException("searchQuery");
        if (string.IsNullOrEmpty(searchTableAlias))
            throw new ArgumentNullException("searchTableAlias");
        if (string.IsNullOrEmpty(searchTableKey))
            throw new ArgumentNullException("searchTableKey");
        if (string.IsNullOrEmpty(containsAlias))
            throw new ArgumentNullException("containsAlias");

        if (from.Length > 0)
            from.Append(" \n");

        from.Append("INNER JOIN CONTAINSTABLE(");

        from.AppendFormat(
            "{0}, ({1}), '{2}') AS {5} ON ({5}.[key] = {3}.{4})",
            searchTable, searchFields, searchQuery.Replace("'", "''"), searchTableAlias, searchTableKey, containsAlias);

        return this;
    }
}