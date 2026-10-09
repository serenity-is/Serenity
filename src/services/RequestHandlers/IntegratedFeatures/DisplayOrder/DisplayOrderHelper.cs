namespace Serenity.Data;

/// <summary>
///   A static class with helper functions to update display orders of all records or
///   groups of records in a table.</summary>
/// <remarks>
/// Table and column names passed to these methods are assumed to be trusted row metadata
/// (e.g. <see cref="IEntity.Table"/> and <see cref="IDisplayOrderRow"/> fields), not external
/// user input. They are auto bracketed for correct quoting, but are NOT validated or
/// sanitized against SQL injection, so callers must never pass user supplied strings as
/// table or column names (the same way they could not pass such values to
/// <see cref="SqlQuery.From(string)"/> or a field <c>Expression</c>).
/// <para>
/// These helpers do NOT handle race conditions. In particular, <c>GetNextValue</c> reads
/// MAX(display order) + 1 and the caller inserts that value later in the same transaction, so
/// under the default (read committed) isolation two concurrent inserts in the same group can
/// compute the same next value. A unique index on the display order column (grouped with the
/// same keys used by the display order filter) is therefore recommended so the database rejects
/// the duplicate. When such an index exists, pass <c>hasUniqueConstraint: true</c> to the reorder
/// methods so they avoid transient collisions.
/// </para>
/// <para>
/// Automatic retry of conflicting inserts is not implemented. It is feasible (a unique violation
/// can be detected via <c>ISqlErrorExtractor</c> and the value recomputed), but a failed insert
/// aborts the surrounding transaction, so retrying requires re-running the whole save in a fresh
/// transaction and reliably attributing the violation to the display-order constraint (providers
/// do not always report the offending column). Until that is warranted, handling the conflict
/// (e.g. retrying the request or surfacing the error) is left to the application.
/// </para>
/// </remarks>
public static class DisplayOrderHelper
{
    /// <summary>
    ///   Gets the next display order value for a table or a group of records.</summary>
    /// <param name="connection">
    ///   Connection (required).</param>
    /// <param name="tableName">
    ///   Table name (required).</param>
    /// <param name="orderField">
    ///   Display order field meta (required).</param>
    /// <param name="filter">
    ///   Filter for records (can be null).</param>
    /// <returns>
    ///   One more of maximum display order values of records in the group. 
    ///   If none, 1.</returns>
    /// <remarks>
    /// Not race safe: the value is computed here but inserted later, so concurrent inserts in
    /// the same group may compute the same value. A unique index on the display order column is
    /// recommended so the database rejects duplicates. See the class remarks.
    /// </remarks>
    public static int GetNextValue(IDbConnection connection, string tableName,
        Field orderField, ICriteria? filter)
    {
        ArgumentNullException.ThrowIfNull(connection);
        if (tableName == null || tableName.Length == 0)
            throw new ArgumentNullException(nameof(tableName));
        ArgumentNullException.ThrowIfNull(orderField);

        using IDataReader reader = new SqlQuery()
            .Select(
                Sql.Max(SqlSyntax.AutoBracket(orderField.Name, connection.GetDialect())))
            .From(
                tableName, Alias.T0)
            .Where(
                filter)
            .ExecuteReader(connection);
        if (reader.Read() && !reader.IsDBNull(0))
            return Convert.ToInt32(reader.GetValue(0)) + 1;
        else
            return 1;
    }

    /// <summary>
    ///   Gets the next display order value for a table or a group of records.</summary>
    /// <param name="connection">
    ///   Connection (required).</param>
    /// <param name="row">
    ///   Row with a display order field (required).</param>
    /// <param name="filter">
    ///   Filter for records (can be null).</param>
    /// <returns>
    ///   One more of maximum display order values of records in the group. 
    ///   If none, 1.</returns>
    /// <remarks>
    /// Not race safe: the value is computed here but inserted later, so concurrent inserts in
    /// the same group may compute the same value. A unique index on the display order column is
    /// recommended so the database rejects duplicates. See the class remarks.
    /// </remarks>
    public static int GetNextValue(IDbConnection connection, IDisplayOrderRow row, ICriteria? filter = null)
    {
        return GetNextValue(connection, row.Table, row.DisplayOrderField, filter);
    }

    /// <summary>
    ///   Sets a records display order to to requested value, and also renumbers other records
    ///   in the group as required.</summary>
    /// <param name="connection">
    ///   Connection (required).</param>
    /// <param name="tableName">
    ///   Table name (required).</param>
    /// <param name="keyField">
    ///   ID field meta that will be used to locate the record (required).</param>
    /// <param name="orderField">
    ///   Display order field meta.</param>
    /// <param name="filter">
    ///   Filter that will determine the record group (can be null).</param>
    /// <param name="recordID">
    ///   ID value of the record.</param>
    /// <param name="newDisplayOrder">
    ///   New display order of the record.</param>
    /// <param name="descendingKeyOrder">
    ///   Will records with same display order values be sorted in ascending or descending ID order?
    ///   For example, if records with ID's 1, 2, 3 has display order value of "0", their actual display
    ///   orders are 1, 2 and 3. If this parameter is set to true (descending), their display orders will
    ///   become 3, 2, 1. This parameter controls if records that are added recently and has no display
    ///   order value assigned (or 0) be shown at start or at the end.</param>
    /// <param name="hasUniqueConstraint">True if the entity has a unique constraint on display order
    /// column.</param>
    /// <returns>
    ///   If any of the display order values is changed true.</returns>
    public static bool ReorderValues(IDbConnection connection, string tableName, Field keyField, Field orderField,
        ICriteria? filter = null, object? recordID = null, int newDisplayOrder = 1,
        bool descendingKeyOrder = false, bool hasUniqueConstraint = false)
    {
        ArgumentNullException.ThrowIfNull(connection);
        if (tableName == null || tableName.Length == 0)
            throw new ArgumentNullException(nameof(tableName));
        ArgumentNullException.ThrowIfNull(keyField);
        ArgumentNullException.ThrowIfNull(orderField);

        // query to fetch id and display order values of the records in the group
        SqlQuery query = new SqlQuery()
            .Select(
                keyField,
                orderField)
            .From(
                tableName, Alias.T0)
            .Where(
                filter)
            .OrderBy(
                orderField);

        // determine display order for records with same display order values 
        // based on ID ordering set
        query.OrderBy(SqlSyntax.AutoBracket(keyField.Name, connection.GetDialect()), desc: descendingKeyOrder);

        var orderRecords = new List<OrderRecord>();
        OrderRecord? changing = null;

        // read all existing records
        using (IDataReader reader = query.ExecuteReader(connection))
        {
            var recordIDStr = recordID == null ? null :
                IdToSql(recordID, connection.GetDialect());

            while (reader.Read())
            {
                OrderRecord r = new()
                {
                    recordID = reader.GetValue(0),
                    oldOrder = Convert.ToInt32(reader.GetValue(1))
                };
                r.newOrder = r.oldOrder;
                orderRecords.Add(r);

                if (recordID != null && recordIDStr ==
                        IdToSql(r.recordID, connection.GetDialect()))
                    changing = r;
            }
        }

        _ = ComputeNewOrders(orderRecords, changing, newDisplayOrder);

        return UpdateOrders(connection, orderRecords, tableName, keyField, orderField, hasUniqueConstraint);
    }

    /// <summary>
    ///   Asynchronously gets the next display order value for a table or a group of records.</summary>
    /// <param name="connection">
    ///   Connection (required).</param>
    /// <param name="tableName">
    ///   Table name (required).</param>
    /// <param name="orderField">
    ///   Display order field meta (required).</param>
    /// <param name="filter">
    ///   Filter for records (can be null).</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>
    ///   A task whose result is one more of maximum display order values of records in the group.
    ///   If none, 1.</returns>
    public static async Task<int> GetNextValueAsync(IDbConnection connection, string tableName,
        Field orderField, ICriteria? filter, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(connection);
        if (tableName == null || tableName.Length == 0)
            throw new ArgumentNullException(nameof(tableName));
        ArgumentNullException.ThrowIfNull(orderField);

        using IDataReader reader = await new SqlQuery()
            .Select(
                Sql.Max(SqlSyntax.AutoBracket(orderField.Name, connection.GetDialect())))
            .From(
                tableName, Alias.T0)
            .Where(
                filter)
            .ExecuteReaderAsync(connection, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false) && !reader.IsDBNull(0))
            return Convert.ToInt32(reader.GetValue(0)) + 1;
        else
            return 1;
    }

    /// <summary>
    ///   Asynchronously gets the next display order value for a table or a group of records.</summary>
    /// <param name="connection">
    ///   Connection (required).</param>
    /// <param name="row">
    ///   Row with a display order field (required).</param>
    /// <param name="filter">
    ///   Filter for records (can be null).</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>
    ///   A task whose result is one more of maximum display order values of records in the group.
    ///   If none, 1.</returns>
    public static Task<int> GetNextValueAsync(IDbConnection connection, IDisplayOrderRow row, ICriteria? filter = null,
        CancellationToken cancellationToken = default)
    {
        return GetNextValueAsync(connection, row.Table, row.DisplayOrderField, filter, cancellationToken);
    }

    /// <summary>
    ///   Asynchronously sets a records display order to to requested value, and also renumbers other records
    ///   in the group as required.</summary>
    /// <param name="connection">
    ///   Connection (required).</param>
    /// <param name="tableName">
    ///   Table name (required).</param>
    /// <param name="keyField">
    ///   ID field meta that will be used to locate the record (required).</param>
    /// <param name="orderField">
    ///   Display order field meta.</param>
    /// <param name="filter">
    ///   Filter that will determine the record group (can be null).</param>
    /// <param name="recordID">
    ///   ID value of the record.</param>
    /// <param name="newDisplayOrder">
    ///   New display order of the record.</param>
    /// <param name="descendingKeyOrder">
    ///   Will records with same display order values be sorted in ascending or descending ID order?</param>
    /// <param name="hasUniqueConstraint">True if the entity has a unique constraint on display order
    /// column.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>
    ///   A task whose result is true if any of the display order values is changed.</returns>
    public static Task<bool> ReorderValuesAsync(IDbConnection connection, string tableName, Field keyField, Field orderField,
        ICriteria? filter = null, object? recordID = null, int newDisplayOrder = 1,
        bool descendingKeyOrder = false, bool hasUniqueConstraint = false, CancellationToken cancellationToken = default)
    {
        return ReorderValuesCoreAsync(connection, tableName, keyField, orderField, filter, recordID,
            newDisplayOrder, descendingKeyOrder, hasUniqueConstraint, cancellationToken);
    }

    /// <summary>
    ///   Sets a records display order to to requested value, and also renumbers other records
    ///   in the group as required.</summary>
    /// <param name="connection">
    ///   Connection (required).</param>
    /// <param name="row">
    ///   Row with a display order and ID field (should implement IDbIdRow interface).</param>
    /// <param name="filter">
    ///   Filter that will determine the record group (can be null).</param>
    /// <param name="recordID">
    ///   ID value of the record.</param>
    /// <param name="newDisplayOrder">
    ///   New display order of the record.</param>
    /// <param name="descendingKeyOrder">
    ///   Will records with same display order values be sorted in ascending or descending ID order?</param>
    /// <param name="hasUniqueConstraint">True if the display order field has a unique index</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>
    ///   A task whose result is true if any of the display order values is changed.</returns>
    public static Task<bool> ReorderValuesAsync(IDbConnection connection, IDisplayOrderRow row, ICriteria? filter = null,
        object? recordID = null, int newDisplayOrder = 1, bool descendingKeyOrder = false,
        bool hasUniqueConstraint = false, CancellationToken cancellationToken = default)
    {
        return ReorderValuesCoreAsync(connection, row.Table, row.GetIdField(), row.DisplayOrderField, filter, recordID,
            newDisplayOrder, descendingKeyOrder, hasUniqueConstraint, cancellationToken);
    }

    private static async Task<bool> ReorderValuesCoreAsync(IDbConnection connection, string tableName, Field keyField, Field orderField,
        ICriteria? filter, object? recordID, int newDisplayOrder, bool descendingKeyOrder, bool hasUniqueConstraint,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(connection);
        if (tableName == null || tableName.Length == 0)
            throw new ArgumentNullException(nameof(tableName));
        ArgumentNullException.ThrowIfNull(keyField);
        ArgumentNullException.ThrowIfNull(orderField);

        // query to fetch id and display order values of the records in the group
        SqlQuery query = new SqlQuery()
            .Select(
                keyField,
                orderField)
            .From(
                tableName, Alias.T0)
            .Where(
                filter)
            .OrderBy(
                orderField);

        // determine display order for records with same display order values 
        // based on ID ordering set
        query.OrderBy(SqlSyntax.AutoBracket(keyField.Name, connection.GetDialect()), desc: descendingKeyOrder);

        var orderRecords = new List<OrderRecord>();
        OrderRecord? changing = null;

        // read all existing records
        using (IDataReader reader = await query.ExecuteReaderAsync(connection, cancellationToken: cancellationToken).ConfigureAwait(false))
        {
            var recordIDStr = recordID == null ? null :
                IdToSql(recordID, connection.GetDialect());

            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                OrderRecord r = new()
                {
                    recordID = reader.GetValue(0),
                    oldOrder = Convert.ToInt32(reader.GetValue(1))
                };
                r.newOrder = r.oldOrder;
                orderRecords.Add(r);

                if (recordID != null && recordIDStr ==
                        IdToSql(r.recordID, connection.GetDialect()))
                    changing = r;
            }
        }

        _ = ComputeNewOrders(orderRecords, changing, newDisplayOrder);

        return await UpdateOrdersAsync(connection, orderRecords, tableName, keyField, orderField,
            hasUniqueConstraint, cancellationToken).ConfigureAwait(false);
    }

    private static int ComputeNewOrders(List<OrderRecord> orderRecords, OrderRecord? changing, int newDisplayOrder)
    {
        int order = orderRecords.Count;

        // ensure that the new display order is within limits
        // if its lower than 1 or bigger than record count, fix it
        if (newDisplayOrder <= 0)
            newDisplayOrder = 1;
        else if (newDisplayOrder > order)
            newDisplayOrder = order;

        var changingIndex = changing is null ? -1 : orderRecords.IndexOf(changing);

        // nothing to reorder if the record can't be found, or it is already at the target position
        if (changingIndex < 0 || changingIndex == newDisplayOrder - 1)
            return newDisplayOrder;

        int from = changingIndex;
        int to = newDisplayOrder - 1;

        // only the records between the old and the new positions need to change. When the current
        // display order values are all distinct we shift only those records and keep the values of
        // the rest (including any gaps) as they are, so a single move does not rewrite the whole
        // group. Otherwise (e.g. several records still have the default 0 value) the group has to
        // be renumbered so the record can be placed at the requested position.
        bool distinct = true;
        for (int i = 1; i < order; i++)
        {
            if (orderRecords[i].oldOrder <= orderRecords[i - 1].oldOrder)
            {
                distinct = false;
                break;
            }
        }

        if (distinct)
        {
            if (from < to)
            {
                // record moves down: records after it up to the target shift up
                for (int i = from + 1; i <= to; i++)
                    orderRecords[i].newOrder = orderRecords[i - 1].oldOrder;
                orderRecords[from].newOrder = orderRecords[to].oldOrder;
            }
            else
            {
                // record moves up: records before it down to the target shift down
                for (int i = to; i <= from - 1; i++)
                    orderRecords[i].newOrder = orderRecords[i + 1].oldOrder;
                orderRecords[from].newOrder = orderRecords[to].oldOrder;
            }
        }
        else
        {
            for (int i = 0; i < order; i++)
                orderRecords[i].newOrder = i + 1;

            if (from < to)
            {
                for (int i = from + 1; i <= to; i++)
                    orderRecords[i].newOrder = i;
                orderRecords[from].newOrder = newDisplayOrder;
            }
            else
            {
                for (int i = to; i <= from - 1; i++)
                    orderRecords[i].newOrder = i + 2;
                orderRecords[from].newOrder = newDisplayOrder;
            }
        }

        return newDisplayOrder;
    }

    private static string IdToSql(object id, ISqlDialect dialect)
    {
        if (id == null || id == DBNull.Value)
            throw new ArgumentNullException("displayOrderID");

        if (id is string str)
            return str.ToSql(dialect);

        if (id is Guid guid)
            return ((Guid?)guid).ToSql(dialect);

        if (long.TryParse(id.ToString(), out long l))
            return l.ToString(CultureInfo.InvariantCulture);

        throw new ArgumentOutOfRangeException("displayOrderIDType");
    }

    /// <summary>
    /// Updates display order values in a table
    /// </summary>
    /// <param name="connection">Connection</param>
    /// <param name="orderRecords">List of records with new orders</param>
    /// <param name="tableName">Tablename</param>
    /// <param name="keyField">Key field</param>
    /// <param name="orderField">Order field</param>
    /// <param name="hasUniqueConstraint">True if order field has a unique constraint</param>
    /// <exception cref="ArgumentNullException">connection, tableName, keyField or orderField is null</exception>
    public static bool UpdateOrders(IDbConnection connection, List<OrderRecord> orderRecords,
        string tableName, Field keyField, Field orderField, bool hasUniqueConstraint = false)
    {
        ArgumentNullException.ThrowIfNull(connection);

        if (string.IsNullOrEmpty(tableName))
            throw new ArgumentNullException(nameof(tableName));

        ArgumentNullException.ThrowIfNull(keyField);

        ArgumentNullException.ThrowIfNull(orderField);

        var queries = BuildUpdateQueries(connection, orderRecords, tableName, keyField, orderField, hasUniqueConstraint);
        if (queries.Length == 0)
            return false;

        SqlHelper.ExecuteNonQuery(connection, queries);
        // one or more records has changed display order values
        return true;
    }

    /// <summary>
    /// Asynchronously updates display order values in a table
    /// </summary>
    /// <param name="connection">Connection</param>
    /// <param name="orderRecords">List of records with new orders</param>
    /// <param name="tableName">Tablename</param>
    /// <param name="keyField">Key field</param>
    /// <param name="orderField">Order field</param>
    /// <param name="hasUniqueConstraint">True if order field has a unique constraint</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <exception cref="ArgumentNullException">connection, tableName, keyField or orderField is null</exception>
    /// <returns>A task whose result is true if any display order values were updated</returns>
    public static async Task<bool> UpdateOrdersAsync(IDbConnection connection, List<OrderRecord> orderRecords,
        string tableName, Field keyField, Field orderField, bool hasUniqueConstraint = false,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(connection);

        if (string.IsNullOrEmpty(tableName))
            throw new ArgumentNullException(nameof(tableName));

        ArgumentNullException.ThrowIfNull(keyField);

        ArgumentNullException.ThrowIfNull(orderField);

        var queries = BuildUpdateQueries(connection, orderRecords, tableName, keyField, orderField, hasUniqueConstraint);
        if (queries.Length == 0)
            return false;

        await SqlHelper.ExecuteNonQueryAsync(connection, queries, cancellationToken: cancellationToken).ConfigureAwait(false);
        // one or more records has changed display order values
        return true;
    }

    private static string BuildUpdateQueries(IDbConnection connection, List<OrderRecord> orderRecords,
        string tableName, Field keyField, Field orderField, bool hasUniqueConstraint)
    {
        // Table and column names are assumed to come from trusted row metadata (see class remarks),
        // never from external users. They are auto bracketed for correct quoting only.
        var dialect = connection.GetDialect();

        // StringBuilder that will contain query(s)
        StringBuilder queries = new();

        if (dialect.NeedsExecuteBlockStatement)
        {
            queries.AppendLine("EXECUTE BLOCK AS");
            queries.AppendLine("BEGIN");
        }

        int updateCount = 0;

        void appendSingleUpdate(object id, long newOrder)
        {
            queries.AppendLine(string.Format(
                "UPDATE {0} SET {1} = {2} WHERE {3} = {4};",
                SqlSyntax.AutoBracket(tableName, dialect),
                SqlSyntax.AutoBracket(orderField.Name, dialect),
                newOrder,
                SqlSyntax.AutoBracket(keyField.Name, dialect),
                IdToSql(id, dialect)));
            updateCount++;
        }

        if (hasUniqueConstraint)
        {
            var byCurrentOrder = new Dictionary<long, OrderRecord>();
            foreach (var rec in orderRecords)
                byCurrentOrder[rec.oldOrder] = rec;

            var list = new List<OrderRecord>();
            list.AddRange(orderRecords);
            list.Sort((x, y) => x.newOrder - y.newOrder);

            foreach (var rec in list)
            {
                if (rec.oldOrder != rec.newOrder)
                {
                    byCurrentOrder.Remove(rec.oldOrder);

                    if (byCurrentOrder.TryGetValue(rec.newOrder, out OrderRecord? congestion))
                    {
                        var empty = list.Count * 2;
                        while (byCurrentOrder.ContainsKey(empty))
                            empty++;

                        congestion.oldOrder = empty;
                        appendSingleUpdate(congestion.recordID, empty);
                        byCurrentOrder[empty] = congestion;
                    }

                    appendSingleUpdate(rec.recordID, rec.newOrder);
                    byCurrentOrder[rec.newOrder] = rec;
                }
            }
        }
        else
        {
            // StringBuilder that will contain IN(...) part of the latest query
            StringBuilder sb = new();

            // scan all display order changing records
            int start = 0;
            while (start < orderRecords.Count)
            {
                OrderRecord rs = orderRecords[start];

                // if this records display order is not changed, skip it
                if (rs.oldOrder == rs.newOrder)
                {
                    start++;
                    continue;
                }

                // find the difference between old and new display orders
                int difference = rs.oldOrder - rs.newOrder;

                // clear the IN(...) list
                sb.Length = 0;

                // add this records ID to the IN (...) part
                sb.Append(IdToSql(rs.recordID, dialect));

                // now we'll find all following records whose display orders are changed same amount 
                // (difference between old and new is same), so we will update them with just one query
                // like UPDATE ORDER = ORDER + 1 WHERE ID IN (X, Y, Z....).
                int finish = start;

                while (finish + 1 < orderRecords.Count)
                {
                    // if we found more than 100 records whose display orders changed same amount, to 
                    // limit IN(...) part to overgrow, break searching and run the query. Collect the
                    // rest in another query. If query is too complex, might result in performance
                    // degradation in SQL server
                    if (finish - start >= 100)
                        break;

                    OrderRecord rf = orderRecords[finish + 1];

                    // is this records display order value changed same amount
                    if (rf.oldOrder - rf.newOrder != difference)
                        break;

                    sb.Append(',');
                    sb.Append(IdToSql(rf.recordID, dialect));

                    finish++;
                }

                // if only one record in batch, no need to use IN clause
                if (start == finish)
                {
                    queries.AppendLine(string.Format(
                        "UPDATE {0} SET {1} = {2} WHERE {3} = {4};",
                        SqlSyntax.AutoBracket(tableName, dialect),
                        SqlSyntax.AutoBracket(orderField.Name, dialect),
                        rs.newOrder,
                        SqlSyntax.AutoBracket(keyField.Name, dialect),
                        IdToSql(rs.recordID, dialect)));
                    updateCount++;
                }
                else
                {
                    // batch update, use IN (...)
                    OrderRecord rf = orderRecords[finish];

                    queries.AppendLine(string.Format(
                        "UPDATE {0} SET {1} = {1} - ({2}) WHERE ({3} IN ({4}));",
                        SqlSyntax.AutoBracket(tableName, dialect),
                        SqlSyntax.AutoBracket(orderField.Name, dialect),
                        rs.oldOrder - rs.newOrder,
                        SqlSyntax.AutoBracket(keyField.Name, dialect),
                        sb.ToString()));
                    updateCount++;
                }

                start = finish + 1;
            }
        }

        if (dialect.NeedsExecuteBlockStatement && updateCount > 0)
            queries.AppendLine("END;");

        return updateCount > 0 ? queries.ToString() : string.Empty;
    }

    /// <summary>
    ///   Sets a records display order to to requested value, and also renumbers other records
    ///   in the group as required.</summary>
    /// <param name="connection">
    ///   Connection (required).</param>
    /// <param name="row">
    ///   Row with a display order and ID field (should implement IDbIdRow interface).</param>
    /// <param name="filter">
    ///   Filter that will determine the record group (can be null).</param>
    /// <param name="recordID">
    ///   ID value of the record.</param>
    /// <param name="newDisplayOrder">
    ///   New display order of the record.</param>
    /// <param name="descendingKeyOrder">
    ///   Will records with same display order values be sorted in ascending or descending ID order?
    ///   For example, if records with ID's 1, 2, 3 has display order value of "0", their actual display
    ///   orders are 1, 2 and 3. If this parameter is set to true (descending), their display orders will
    ///   become 3, 2, 1. This parameter controls if records that are added recently and has no display
    ///   order value assigned (or 0) be shown at start or at the end.</param>
    /// <param name="hasUniqueConstraint">True if the display order field has a unique index</param>
    /// <returns>
    ///   If any of the display order values is changed true.</returns>
    public static bool ReorderValues(IDbConnection connection, IDisplayOrderRow row, ICriteria? filter = null,
        object? recordID = null, int newDisplayOrder = 1, bool descendingKeyOrder = false, bool hasUniqueConstraint = false)
    {
        return ReorderValues(connection, row.Table, row.GetIdField(), row.DisplayOrderField, filter, recordID,
            newDisplayOrder, descendingKeyOrder, hasUniqueConstraint);
    }

    /// <summary>
    ///   An internal class that is used FixRecordOrdering to store old and new display orders
    ///   for records to be sorted.</summary>
    public class OrderRecord
    {
        /// <summary>
        /// Record ID
        /// </summary>
        public required object recordID;
        /// <summary>
        /// Old order
        /// </summary>
        public int oldOrder;
        /// <summary>
        /// New order
        /// </summary>
        public int newOrder;
    }
}