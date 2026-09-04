namespace Serenity.Data;

/// <summary>
/// Contains extension methods to query entities directly.
/// </summary>
public static class EntitySqlHelper
{
    /// <summary>
    /// Gets the first entity returned by executing the query.
    /// The result is loaded into the loader row of the query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="connection">The connection.</param>
    /// <returns>True if any rows were returned.</returns>
    public static bool GetFirst(this SqlQuery query, IDbConnection connection)
    {
        using var reader = query.ExecuteReader(connection);
        if (!reader.Read())
            return false;

        query.GetFromReader(reader);
        return true;
    }

    /// <summary>
    /// Gets the single entity returned by executing the query. 
    /// The values are loaded into the loader row of the query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="connection">The connection.</param>
    /// <returns>True if any results were returned from the data reader.</returns>
    /// <exception cref="InvalidOperationException">Query returned more than one result!</exception>
    public static bool GetSingle(this SqlQuery query, IDbConnection connection)
    {
        using IDataReader reader = query.ExecuteReader(connection);
        if (!reader.Read())
            return false;

        query.GetFromReader(reader);

        if (reader.Read())
            throw new InvalidOperationException("Query returned more than one result!");

        return true;
    }

    /// <summary>
    /// Gets the first entity returned by executing the query asynchronously.
    /// The result is loaded into the loader row of the query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="connection">The connection.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is true if any rows were returned.</returns>
    public static async Task<bool> GetFirstAsync(this SqlQuery query, IDbConnection connection, CancellationToken cancellationToken = default)
    {
        using var reader = await query.ExecuteReaderAsync(connection, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (!await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            return false;

        query.GetFromReader(reader);
        return true;
    }

    /// <summary>
    /// Gets the single entity returned by executing the query asynchronously.
    /// The values are loaded into the loader row of the query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="connection">The connection.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is true if any results were returned from the data reader.</returns>
    /// <exception cref="InvalidOperationException">Query returned more than one result!</exception>
    public static async Task<bool> GetSingleAsync(this SqlQuery query, IDbConnection connection, CancellationToken cancellationToken = default)
    {
        using IDataReader reader = await query.ExecuteReaderAsync(connection, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (!await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            return false;

        query.GetFromReader(reader);

        if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            throw new InvalidOperationException("Query returned more than one result!");

        return true;
    }

    /// <summary>
    /// Executes the specified callback for all rows returned from executing the query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="connection">The connection.</param>
    /// <param name="callBack">The call back.</param>
    /// <returns>Number of returned results.</returns>
    public static int ForEach(this SqlQuery query, IDbConnection connection,
        Action callBack)
    {
        return ForEach(query, connection, _ => callBack());
    }

    /// <summary>
    /// Executes the specified data reader callback for all rows returned from executing the query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="connection">The connection.</param>
    /// <param name="callback">The call back.</param>
    /// <returns>Number of returned results.</returns>
    public static int ForEach(this SqlQuery query, IDbConnection connection,
        Action<IDataReader> callback)
    {
        int count = 0;

        if (connection.GetDialect().MultipleResultsets)
        {
            using IDataReader reader = query.ExecuteReader(connection);
            while (reader.Read())
            {
                query.GetFromReader(reader);
                callback(reader);
            }

            if (query.CountRecords && reader.NextResult() && reader.Read())
                return Convert.ToInt32(reader.GetValue(0));
        }
        else
        {
            string[] queries = query.ToString().Split(["\n---\n"], StringSplitOptions.RemoveEmptyEntries);
            if (queries.Length > 1)
                count = Convert.ToInt32(SqlHelper.ExecuteScalar(connection, queries[1], query.Params));

            using IDataReader reader = SqlHelper.ExecuteReader(connection, queries[0], query.Params);
            while (reader.Read())
            {
                query.GetFromReader(reader);
                callback(reader);
            }
        }

        return count;
    }

    /// <summary>
    /// Lists the rows returned from executing the query.
    /// </summary>
    /// <typeparam name="TRow">The type of the row.</typeparam>
    /// <param name="query">The query.</param>
    /// <param name="connection">The connection.</param>
    /// <param name="loaderRow">The loader row.</param>
    /// <returns>List of rows.</returns>
    public static List<TRow> List<TRow>(this SqlQuery query,
        IDbConnection connection, TRow loaderRow = null) where TRow : class, IRow
    {
        var list = new List<TRow>();
        ForEach(query, connection, delegate ()
        {
            list.Add(loaderRow.Clone());
        });
        return list;
    }

    /// <summary>
    /// Asynchronously lists the rows returned from executing the query.
    /// </summary>
    /// <typeparam name="TRow">The type of the row.</typeparam>
    /// <param name="query">The query.</param>
    /// <param name="connection">The connection.</param>
    /// <param name="loaderRow">The loader row.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation. The task result is the list of rows.</returns>
    public static async Task<List<TRow>> ListAsync<TRow>(this SqlQuery query,
        IDbConnection connection, TRow loaderRow = null, CancellationToken cancellationToken = default) where TRow : class, IRow
    {
        var list = new List<TRow>();
        await ForEachAsync(query, connection, delegate ()
        {
            list.Add(loaderRow.Clone());
        }, cancellationToken).ConfigureAwait(false);
        return list;
    }

    /// <summary>
    /// Asynchronously executes the specified callback for all rows returned from executing the query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="connection">The connection.</param>
    /// <param name="callBack">The call back.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation. The task result is the number of returned results.</returns>
    public static Task<int> ForEachAsync(this SqlQuery query, IDbConnection connection,
        Action callBack, CancellationToken cancellationToken = default)
    {
        return ForEachAsync(query, connection, _ => callBack(), cancellationToken);
    }

    /// <summary>
    /// Asynchronously executes the specified data reader callback for all rows returned from executing the query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="connection">The connection.</param>
    /// <param name="callback">The call back.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation. The task result is the number of returned results.</returns>
    public static async Task<int> ForEachAsync(this SqlQuery query, IDbConnection connection,
        Action<IDataReader> callback, CancellationToken cancellationToken = default)
    {
        int count = 0;

        if (connection.GetDialect().MultipleResultsets)
        {
            using IDataReader reader = await query.ExecuteReaderAsync(connection, cancellationToken: cancellationToken).ConfigureAwait(false);
            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                query.GetFromReader(reader);
                callback(reader);
            }

            if (query.CountRecords && await reader.NextResultAsync(cancellationToken).ConfigureAwait(false) &&
                await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                return Convert.ToInt32(reader.GetValue(0));
        }
        else
        {
            string[] queries = query.ToString().Split(["\n---\n"], StringSplitOptions.RemoveEmptyEntries);
            if (queries.Length > 1)
                count = Convert.ToInt32(await SqlHelper.ExecuteScalarAsync(connection, queries[1], query.Params, cancellationToken: cancellationToken).ConfigureAwait(false));

            using IDataReader reader = await SqlHelper.ExecuteReaderAsync(connection, queries[0], query.Params, cancellationToken: cancellationToken).ConfigureAwait(false);
            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                query.GetFromReader(reader);
                callback(reader);
            }
        }

        return count;
    }

    /// <summary>
    /// Gets field values from data reader into the query loader row.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="reader">The reader.</param>
    public static void GetFromReader(this SqlQuery query, IDataReader reader)
    {
        var ext = (ISqlQueryExtensible)query;

        GetFromReader(query, reader, ext.IntoRows);
    }

    const string FieldReadValueError = "An error occurred while loading value of the field '{0}' of '{1}' from data reader. " +
        "Please make sure the field type matches the actual data type in database.\r\n\r\nThe error message is:\r\n{2}";

    /// <summary>
    /// Gets field values from data reader into the set of specified into rows.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="reader">The reader.</param>
    /// <param name="into">The into rows list.</param>
    /// <exception cref="InvalidOperationException">An exception occurred during conversion</exception>
    public static void GetFromReader(this SqlQuery query, IDataReader reader, IList<object> into)
    {
        var ext = (ISqlQueryExtensible)query;

        int index = -1;
        foreach (var info in ext.Columns)
        {
            index++;

            if (info.IntoRowIndex < 0 || info.IntoRowIndex >= into.Count)
                continue;

            if (into[info.IntoRowIndex] is not IRow row)
                continue;

            if (info.IntoField is Field field &&
                (field.Fields == row.Fields ||
                 field.Fields.GetType() == row.Fields.GetType()))
            {
                try
                {
                    field.GetFromReader(reader, index, row);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(string.Format(FieldReadValueError,
                        field.PropertyName ?? field.Name, row.GetType().FullName, ex.Message), ex);
                }
                continue;
            }

            var name = reader.GetName(index);
            field = row.Fields.FindField(name) ?? row.Fields.FindFieldByPropertyName(name);

            if (field is not null)
            {
                try
                {
                    field.GetFromReader(reader, index, row);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(string.Format(FieldReadValueError,
                        field.PropertyName ?? field.Name, row.GetType().FullName, ex.Message), ex);
                }
                continue;
            }

            if (reader.IsDBNull(index))
                row.SetDictionaryData(name, null);
            else
            {
                var value = reader.GetValue(index);
                row.SetDictionaryData(name, value);
            }
        }
    }
}
