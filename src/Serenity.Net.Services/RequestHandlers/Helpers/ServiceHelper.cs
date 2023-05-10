namespace Serenity.Services;

/// <summary>
/// Contains some helper methods for service handlers
/// </summary>
public static class ServiceHelper
{
    /// <summary>
    /// Checks that parent record is not soft deleted
    /// </summary>
    /// <param name="connection">Connection</param>
    /// <param name="tableName">Table name</param>
    /// <param name="filter">Filter callback</param>
    /// <param name="localizer">Text localizer</param>
    public static void CheckParentNotDeleted(IDbConnection connection, string tableName,
        Action<SqlQuery> filter, ITextLocalizer localizer)
    {
        var query = new SqlQuery().Dialect(connection.GetDialect()).Select("1").From(tableName, Alias.T0);
        filter(query);
        if (query.Take(1).Exists(connection))
            throw DataValidation.ParentRecordDeleted(tableName, localizer);
    }

    /// <summary>
    /// Sets the Skip, Take and Total parameters in the response
    /// </summary>
    /// <typeparam name="T">Type of the response entities</typeparam>
    /// <param name="response">Response object</param>
    /// <param name="query">Query to get params from</param>
    public static void SetSkipTakeTotal<T>(this ListResponse<T> response, SqlQuery query)
    {
        response.Skip = query.Skip();
        response.Take = query.Take();
        if (response.Take == 0)
            response.TotalCount = response.Entities.Count + response.Skip;
    }

    /// <summary>
    /// Checks if an exception seems to be an unique index exception
    /// </summary>
    /// <param name="connection">Connection</param>
    /// <param name="exception">Exception</param>
    /// <param name="indexName">Optional index name to check</param>
    /// <param name="oldRow">Old row</param>
    /// <param name="newRow">New row</param>
    /// <param name="indexFields">List of index fields</param>
    /// <exception cref="ArgumentNullException">connection or exception is null</exception>
    public static bool IsUniqueIndexException(IDbConnection connection,
        Exception exception, string indexName,
        IRow oldRow, IRow newRow, params Field[] indexFields)
    {
        if (connection == null)
            throw new ArgumentNullException("connection");

        if (exception == null)
            throw new ArgumentNullException("exception");

        if (indexFields == null ||
            indexFields.Length == 0)
            throw new ArgumentNullException("indexField");

        if (indexName != null &&
            !exception.Message.Contains(indexName))
            return false;

        if (oldRow != null)
        {
            bool anyDifferent = false;
            foreach (var field in indexFields)
                if (field.IndexCompare(oldRow, newRow) != 0)
                {
                    anyDifferent = true;
                    break;
                }

            if (!anyDifferent)
                return false;
        }

        var row = newRow.CreateNew();
        var idField = newRow.IdField;

        var query = new SqlQuery()
            .Dialect(connection.GetDialect())
            .From(row).Select(idField);
        foreach (var field in indexFields)
            query.WhereEqual(field, field.AsSqlValue(newRow));

        if (!query.GetFirst(connection))
            return false;

        return idField.IndexCompare(row, newRow) != 0;
    }
}
