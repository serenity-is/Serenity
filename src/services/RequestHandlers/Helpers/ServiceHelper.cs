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
        var query = BuildParentNotDeletedQuery(connection, tableName, filter);
        if (query.Exists(connection))
            throw DataValidation.ParentRecordDeleted(tableName, localizer);
    }

    /// <summary>
    /// Asynchronously checks that parent record is not soft deleted
    /// </summary>
    /// <param name="connection">Connection</param>
    /// <param name="tableName">Table name</param>
    /// <param name="filter">Filter callback</param>
    /// <param name="localizer">Text localizer</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public static async Task CheckParentNotDeletedAsync(IDbConnection connection, string tableName,
        Action<SqlQuery> filter, ITextLocalizer localizer, CancellationToken cancellationToken = default)
    {
        var query = BuildParentNotDeletedQuery(connection, tableName, filter);
        if (await query.ExistsAsync(connection, cancellationToken: cancellationToken).ConfigureAwait(false))
            throw DataValidation.ParentRecordDeleted(tableName, localizer);
    }

    private static SqlQuery BuildParentNotDeletedQuery(IDbConnection connection, string tableName,
        Action<SqlQuery> filter)
    {
        var query = new SqlQuery().Dialect(connection.GetDialect()).Select("1").From(tableName, Alias.T0);
        filter(query);
        return query.Take(1);
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
    /// Sets paging metadata using the state returned by <see cref="ServiceQueryHelper.ApplyPagingParams(SqlQuery, ListRequestPagingParams, out ListRequestPagingState)"/>.
    /// </summary>
    /// <typeparam name="T">Type of the response entities</typeparam>
    /// <param name="response">Response object</param>
    /// <param name="query">Query after execution</param>
    /// <param name="pagingState">Paging strategy selected before query execution</param>
    /// <param name="distinctFieldCount">Number of fields in each flattened distinct value tuple, or null for entities.</param>
    /// <param name="rowsRead">Number of rows returned by the query before post-processing (e.g.
    /// <c>ProcessEntity</c> dropping rows). When provided, the <see cref="ListResponse{T}.More"/> sentinel
    /// check uses it, so a dropped row does not hide the existence of further results.</param>
    public static void SetSkipTakeTotal<T>(this ListResponse<T> response, SqlQuery query,
        ListRequestPagingState pagingState, int? distinctFieldCount = null, int? rowsRead = null)
    {
        var requestedTake = pagingState.Params.Take;
        var includeMore = pagingState.Params.IncludeMore;
        var queryTake = query.Take();
        var hasSentinel = pagingState.UsesSentinel && queryTake == pagingState.AppliedTake;
        response.Skip = query.Skip();
        response.Take = hasSentinel ? requestedTake : queryTake;

        int? resultCount;
        if (distinctFieldCount is int fieldCount)
        {
            resultCount = fieldCount > 0 && response.Values is not null ?
                response.Values.Count / fieldCount : null;
        }
        else
            resultCount = response.Entities.Count;

        if (hasSentinel && resultCount is int sentinelResultCount &&
            sentinelResultCount > requestedTake)
        {
            var extraCount = sentinelResultCount - requestedTake;
            if (distinctFieldCount is int tupleWidth)
                response.Values!.RemoveRange(requestedTake * tupleWidth,
                    extraCount * tupleWidth);
            else
                response.Entities.RemoveRange(requestedTake, extraCount);
        }

        if (includeMore)
        {
            if (queryTake == 0)
                response.More = resultCount is not null ? false : null;
            else if (query.CountRecords)
                response.More = response.TotalCount > (long)response.Skip + response.Take;
            else if (hasSentinel && (rowsRead ?? resultCount) is int count)
                response.More = count > requestedTake;
            else
                response.More = null;
        }

        if (response.Take == 0 && resultCount is int totalCount)
            response.TotalCount = totalCount + response.Skip;
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
    /// <exception cref="ArgumentNullException"><paramref name="connection"/> or <paramref name="exception"/> is <c>null</c>.</exception>
    public static bool IsUniqueIndexException(IDbConnection connection,
        Exception exception, string indexName,
        IRow oldRow, IRow newRow, params Field[] indexFields)
    {
        ArgumentNullException.ThrowIfNull(connection);

        ArgumentNullException.ThrowIfNull(exception);

        if (indexFields == null ||
            indexFields.Length == 0)
            throw new ArgumentNullException(nameof(indexFields));

        if (indexName != null &&
            !exception.Message.Contains(indexName))
            return false;

        if (oldRow != null)
        {
            bool anyDifferent = false;
            foreach (var field in indexFields)
                if (field.IndexCompare(oldRow, newRow, StringComparer.Ordinal) != 0)
                {
                    anyDifferent = true;
                    break;
                }

            if (!anyDifferent)
                return false;
        }

        var row = newRow.CreateNew();
        var idField = newRow.GetIdField();

        var query = new SqlQuery()
            .Dialect(connection.GetDialect())
            .From(row).Select(idField);
        foreach (var field in indexFields)
            query.WhereEqual(field, field.AsSqlValue(newRow));

        if (!query.GetFirst(connection))
            return false;

        return idField.IndexCompare(row, newRow, StringComparer.Ordinal) != 0;
    }
}
