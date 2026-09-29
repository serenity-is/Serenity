namespace Serenity.Data;

/// <summary>
///   Class to generate queries of form <c>DELETE FROM tablename WHERE [conditions]</c>.</summary>
/// <remarks>
///   Creates a new SqlDelete query.</remarks>
/// <param name="tableName">
///   Table to delete records from (required).</param>
public sealed class SqlDelete(string tableName) : QueryWithParams, IFilterableQuery
{
    private readonly string _tableName = tableName ?? throw new ArgumentNullException(nameof(tableName));
    private readonly List<ICriteria> whereCriteria = [];
    private readonly StringBuilder whereClause = new();

    /// <summary>
    ///   Adds a criteria to the WHERE part of the query with an "AND" between.</summary>
    /// <param name="criteria">Condition criteria.</param>
    /// <returns>
    ///   SqlDelete object itself.</returns>
    public SqlDelete Where(ICriteria? criteria)
    {
        if (criteria is null || criteria.IsEmpty)
            return this;

        BeforeModify();
        var sql = criteria.ToString(this);
        if (whereCriteria.Count > 0)
            whereClause.Append(SqlKeywords.And);
        whereClause.Append(sql);
        whereCriteria.Add(criteria);

        return this;
    }

    /// <summary>
    /// Gets the WHERE criteria through <see cref="IFilterableQuery"/>.
    /// </summary>
    IReadOnlyList<ICriteria> IFilterableQuery.GetWhereCriteria() => whereCriteria.AsReadOnly();

    string IFilterableQuery.GetWhereClause() => SqlUpdate.RemoveT0Reference(whereClause.ToString());

    void IFilterableQuery.Where(ICriteria? criteria) => Where(criteria);

    /// <summary>
    ///   Gets string representation of the query.</summary>
    /// <returns>
    ///   String representation of the query.</returns>
    public override string ToString()
    {
        return cachedToString ??= Format(_tableName, ((IFilterableQuery)this).GetWhereClause(), dialect);
    }

    /// <summary>
    ///   Formats a DELETE query.</summary>
    /// <param name="tableName">
    ///   Table name.</param>
    /// <param name="where">
    ///   Where part of the query.</param>
    /// <param name="dialect">Target dialect</param>
    /// <returns>
    ///   Formatted query.</returns>
    /// <exception cref="ArgumentException">tableName is null or empty.</exception>
    public static string Format(string tableName, string? where, ISqlDialect? dialect = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(tableName);

        StringBuilder sb = new("DELETE FROM ", 24 + (where?.Length ?? 0));
        sb.Append(SqlSyntax.AutoBracketValid(tableName, dialect));

        if (!string.IsNullOrEmpty(where))
        {
            sb.Append(" WHERE ");
            sb.Append(where);
        }
        return sb.ToString();
    }
}