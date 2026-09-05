using Microsoft.Extensions.Logging;
using System.IO;

namespace Serenity.Data;

/// <summary>
/// Contains static SQL related helper functions and extensions.
/// </summary>
public static class SqlHelper
{
    /// <summary>
    /// Fixes the type of the parameter to something suitable as a SQL parameter.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The value converted to a suitable SQL parameter type.</returns>
    public static object FixParamType(object value)
    {
        if (value == null)
            return DBNull.Value;

        if (value is Stream stream)
        {
            if (value is MemoryStream memoryStream)
                return memoryStream.ToArray();

            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            return value = ms.ToArray();
        }

        if (value is Enum)
        {
            var underlyingType = Enum.GetUnderlyingType(value.GetType());
            if (underlyingType == typeof(int))
                return (int)value;
            else if (underlyingType == typeof(short))
                return (short)value;
            else
                return Convert.ChangeType(value, underlyingType);
        }

        return value;
    }

    /// <summary>
    /// Fixes the command text for target dialect by replacing brackets ([]), and parameter prefixes (@).
    /// </summary>
    /// <param name="commandText">The command text.</param>
    /// <param name="dialect">The dialect.</param>
    /// <returns>Fixed query.</returns>
    [Obsolete("Use SqlConversions.Translate")]
    public static string FixCommandText(string commandText, ISqlDialect dialect)
    {
        return SqlConversions.Translate(commandText, dialect);
    }

    /// <summary>
    /// Logs the command.
    /// </summary>
    /// <param name="method">The method name.</param>
    /// <param name="command">The command.</param>
    /// <param name="logger">The logger.</param>
    private static void LogCommand(string method, IDbCommand command, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(logger);

        if (logger.IsEnabled(LogLevel.Debug) == true)
        try
        {
            logger.LogDebug("SQL - {method}[{uid}] - START\n{sql}", method, command.GetHashCode(), SqlCommandDumper.GetCommandText(command));
        }
        catch (Exception ex)
        {
            logger.LogDebug(ex, "Error logging command");
        }
    }

    /// <summary>
    /// Adds the parameter with value to the target command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    /// <param name="dialect">The dialect.</param>
    /// <returns>The new parameter.</returns>
    public static IDbDataParameter AddParamWithValue(this IDbCommand command, string name, object value, ISqlDialect dialect)
    {
        name = dialect.ParameterPrefix != '@' &&
            name.StartsWith('@') ? dialect.ParameterPrefix + name[1..] :
                name;

#if !NET45
        if (value is Dapper.SqlMapper.ICustomQueryParameter cqp)
        {
            cqp.AddParameter(command, name);
            return (IDbDataParameter)command.Parameters[^1];
        }
#endif
        IDbDataParameter param = command.CreateParameter();

        param.ParameterName = name;

        value = FixParamType(value) ?? DBNull.Value;

        if (value is bool b && dialect.NeedsBoolWorkaround)
        {
            // otherwise argument out of range exception!
            param.Value = b ? 1 : 0;
        }
        else
        {
            param.Value = value;

            if (value is System.Data.SqlTypes.SqlBinary n && n.IsNull)
            {
                param.Value = DBNull.Value;
                param.DbType = DbType.Binary;
            }
            else if (value != null && value != DBNull.Value)
            {
#pragma warning disable CS0618
                var mappedType = Dapper.SqlMapper.LookupDbType(value.GetType(), "n/a", false, out var _); ;
#pragma warning restore CS0618

                if (!(param.DbType == DbType.Date && value is DateOnly) &&
                      mappedType != param.DbType && mappedType != null)
                    param.DbType = mappedType.Value;

                if (param.DbType == DbType.DateTime &&
                    (dialect ?? SqlSettings.DefaultDialect).UseDateTime2)
                    param.DbType = DbType.DateTime2;
            }

            if (value is string str && str.Length < 4000)
                param.Size = 4000;
        }

        command.Parameters.Add(param);
        return param;
    }

    /// <summary>
    /// Creates a new command.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="commandText">The command text.</param>
    /// <param name="param">The parameters.</param>
    /// <returns>A new command with the specified command text and parameters.</returns>
    public static IDbCommand NewCommand(IDbConnection connection, string commandText, IDictionary<string, object> param)
    {
        var command = NewCommand(connection, commandText);

        if (param == null || param.Count == 0)
            return command;

        try
        {
            var dialect = connection.GetDialect();

            foreach (var p in param)
                AddParamWithValue(command, p.Key, p.Value, dialect);

            return command;
        }
        catch
        {
            command.Dispose();
            throw;
        }
    }

    /// <summary>
    /// Creates a new command.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="commandText">The command text.</param>
    /// <returns>A new command with the specified command text.</returns>
    /// <exception cref="ArgumentNullException">connection is null.</exception>
    public static IDbCommand NewCommand(IDbConnection connection, string commandText)
    {
        ArgumentNullException.ThrowIfNull(connection);

        IDbCommand command = connection.CreateCommand();

        commandText = SqlConversions.Translate(commandText, connection);
        command.CommandText = commandText;
        return command;
    }

    /// <summary>
    /// Checks for the connection pool exception.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="exception">The exception.</param>
    /// <returns>True if exception is 10054, e.g. connection pool.</returns>
    private static bool CheckConnectionPoolException(IDbConnection connection, Exception exception)
    {
        if (!CheckConnectionPoolExceptionCore(connection, exception))
            return false;

        connection.Close();
        connection.Open();
        return true;
    }

    /// <summary>
    /// Checks for the connection pool exception asynchronously.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="exception">The exception.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is true if exception is 10054, e.g. connection pool.</returns>
    private static async Task<bool> CheckConnectionPoolExceptionAsync(IDbConnection connection, Exception exception, CancellationToken cancellationToken = default)
    {
        if (!CheckConnectionPoolExceptionCore(connection, exception))
            return false;

        if (connection is System.Data.Common.DbConnection dbConnection)
        {
            await dbConnection.CloseAsync().ConfigureAwait(false);
            await dbConnection.OpenAsync(cancellationToken).ConfigureAwait(false);
        }
        else
        {
            connection.Close();
            connection.Open();
        }
        return true;
    }

    /// <summary>
    /// Clears the connection pool and returns true if the exception is a connection pool
    /// exception (e.g. error 10054). The caller should close and reopen the connection
    /// when this returns true.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="exception">The exception.</param>
    /// <returns>True if exception is 10054, e.g. connection pool.</returns>
    private static bool CheckConnectionPoolExceptionCore(IDbConnection connection, Exception exception)
    {
        var exceptionType = exception.GetType();

        if ((connection is IHasOpenedOnce hoo && hoo.OpenedOnce) ||
            (connection is IHasCurrentTransaction hct && hct.CurrentTransaction != null))
            return false;

        if (exceptionType.FullName == "Microsoft.Data.SqlException" ||
            exceptionType.FullName == "System.Data.SqlException" &&
            exceptionType.GetProperty("Number")?.GetValue(exception) is 10054)
        {
            var sqlConnectionType = exceptionType.Assembly.GetType(exceptionType.FullName.Replace("Exception", "Connection"));
            var clearAllPools = sqlConnectionType?.GetMethod("ClearAllPools", BindingFlags.Static | BindingFlags.Public);
            clearAllPools?.Invoke(null, null);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Executes the SQL statement and returns the number of affected rows.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="logger">The logger.</param>
    /// <returns>The number of affected rows.</returns>
    /// <exception cref="ArgumentNullException">
    /// command is null or command.Connection is null.
    /// </exception>
    private static int InternalExecuteNonQuery(IDbCommand command, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.Connection == null)
            throw new ArgumentNullException("command.Connection");

        try
        {
            int result;
            command.Connection.EnsureOpen();
            var stopwatch = ValueStopwatch.StartNew();
            try
            {
                logger ??= command.Connection.GetLogger();

                if (logger?.IsEnabled(LogLevel.Debug) == true)
                    LogCommand("ExecuteNonQuery", command, logger);

                result = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                if (CheckConnectionPoolException(command.Connection, ex))
                    return command.ExecuteNonQuery();
                else
                    throw;
            }

            if (logger?.IsEnabled(LogLevel.Debug) == true)
                logger.LogDebug("SQL - {method}[{uid}] - END - {ElapsedMilliseconds} ms",
                    "ExecuteNonQuery", command.GetHashCode(), stopwatch.ElapsedMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            ex.SetData("sql_command_text", command.CommandText);
            throw;
        }
    }

    /// <summary>
    /// Executes the statement.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="commandText">The command text.</param>
    /// <param name="param">The parameters.</param>
    /// <param name="logger">The logger.</param>
    /// <returns>The number of affected rows.</returns>
    public static int ExecuteNonQuery(IDbConnection connection, string commandText, IDictionary<string, object> param = null, ILogger logger = null)
    {
        if (connection is ISqlOperationInterceptor interceptor &&
            interceptor.ExecuteNonQuery(commandText, param, ExpectedRows.Ignore, query: null, getNewId: false) is { HasValue: true } intres)
            return (int)intres.Value;
        using IDbCommand command = NewCommand(connection, commandText, param);
        return InternalExecuteNonQuery(command, logger);
    }

    private static Task<int> ExecuteNonQueryAsync(IDbCommand command, CancellationToken cancellationToken)
    {
        if (command is System.Data.Common.DbCommand dbCommand)
            return dbCommand.ExecuteNonQueryAsync(cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(command.ExecuteNonQuery());
    }

    /// <summary>
    /// Executes the SQL statement asynchronously and returns the number of affected rows.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of affected rows.</returns>
    /// <exception cref="ArgumentNullException">
    /// command is null or command.Connection is null.
    /// </exception>
    private static async Task<int> InternalExecuteNonQueryAsync(IDbCommand command, ILogger logger, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.Connection == null)
            throw new ArgumentNullException("command.Connection");

        try
        {
            int result;
            await command.Connection.EnsureOpenAsync(cancellationToken).ConfigureAwait(false);
            var stopwatch = ValueStopwatch.StartNew();
            try
            {
                logger ??= command.Connection.GetLogger();

                if (logger?.IsEnabled(LogLevel.Debug) == true)
                    LogCommand("ExecuteNonQuery", command, logger);

                result = await ExecuteNonQueryAsync(command, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (await CheckConnectionPoolExceptionAsync(command.Connection, ex, cancellationToken).ConfigureAwait(false))
                    return await ExecuteNonQueryAsync(command, cancellationToken).ConfigureAwait(false);
                else
                    throw;
            }

            if (logger?.IsEnabled(LogLevel.Debug) == true)
                logger.LogDebug("SQL - {method}[{uid}] - END - {ElapsedMilliseconds} ms",
                    "ExecuteNonQuery", command.GetHashCode(), stopwatch.ElapsedMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            ex.SetData("sql_command_text", command.CommandText);
            throw;
        }
    }

    /// <summary>
    /// Executes the statement asynchronously.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="commandText">The command text.</param>
    /// <param name="param">The parameters.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of affected rows.</returns>
    public static async Task<int> ExecuteNonQueryAsync(IDbConnection connection, string commandText, IDictionary<string, object> param = null, ILogger logger = null, CancellationToken cancellationToken = default)
    {
        if (connection is ISqlOperationInterceptor interceptor &&
            await interceptor.ExecuteNonQueryAsync(commandText, param, ExpectedRows.Ignore, query: null, getNewId: false, cancellationToken).ConfigureAwait(false) is { HasValue: true } intres)
            return (int)intres.Value;
        using IDbCommand command = NewCommand(connection, commandText, param);
        return await InternalExecuteNonQueryAsync(command, logger, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Executes the query and returns the generated identity value.
    /// Only works for auto incremented fields, not GUIDs.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="connection">The connection.</param>
    /// <param name="logger">The logger.</param>
    /// <returns>The generated identity value, or null if none was generated.</returns>
    /// <exception cref="ArgumentNullException">query.IdentityColumn is null.</exception>
    /// <exception cref="NotImplementedException">The connection dialect doesn't support returning the inserted identity.</exception>
    public static long? ExecuteAndGetID(this SqlInsert query, IDbConnection connection, ILogger logger = null)
    {
        string queryText = query.ToString();

        if (connection is ISqlOperationInterceptor interceptor &&
            interceptor.ExecuteNonQuery(queryText, query.Params, ExpectedRows.One, query, getNewId: true) is { HasValue: true } intres)
            return intres.Value;

        var dialect = connection.GetDialect();
        if (dialect.UseReturningIdentity || dialect.UseReturningIntoVar)
        {
            using var command = CreateReturningIdentityCommand(query, connection, queryText, dialect, out var param);
            InternalExecuteNonQuery(command, logger);
            return Convert.ToInt64(param.Value);
        }

        if (dialect.UseScopeIdentity)
        {
            queryText += ";\nSELECT " + dialect.ScopeIdentityExpression + " AS IDCOLUMNVALUE";

            using IDataReader reader = InternalExecuteReader(connection, queryText, query.Params, logger);
            return reader.Read() ? ReadIdentityValue(reader) : null;
        }

        throw new NotImplementedException();
    }

    /// <summary>
    /// Executes the query asynchronously and returns the generated identity value.
    /// Only works for auto incremented fields, not GUIDs.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="connection">The connection.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the generated identity value, or null if none was generated.</returns>
    /// <exception cref="ArgumentNullException">query.IdentityColumn is null.</exception>
    /// <exception cref="NotImplementedException">The connection dialect doesn't support returning the inserted identity.</exception>
    public static async Task<long?> ExecuteAndGetIDAsync(this SqlInsert query, IDbConnection connection, ILogger logger = null, CancellationToken cancellationToken = default)
    {
        string queryText = query.ToString();

        if (connection is ISqlOperationInterceptor interceptor &&
            await interceptor.ExecuteNonQueryAsync(queryText, query.Params, ExpectedRows.One, query, getNewId: true, cancellationToken).ConfigureAwait(false) is { HasValue: true } intres)
            return intres.Value;

        var dialect = connection.GetDialect();
        if (dialect.UseReturningIdentity || dialect.UseReturningIntoVar)
        {
            using var command = CreateReturningIdentityCommand(query, connection, queryText, dialect, out var param);
            await InternalExecuteNonQueryAsync(command, logger, cancellationToken).ConfigureAwait(false);
            return Convert.ToInt64(param.Value);
        }

        if (dialect.UseScopeIdentity)
        {
            queryText += ";\nSELECT " + dialect.ScopeIdentityExpression + " AS IDCOLUMNVALUE";

            using IDataReader reader = await InternalExecuteReaderAsync(connection, queryText, query.Params, logger, cancellationToken).ConfigureAwait(false);
            return await reader.ReadAsync(cancellationToken).ConfigureAwait(false) ? ReadIdentityValue(reader) : null;
        }

        throw new NotImplementedException();
    }


    const string ExpectedRowsError = "Query affected {0} rows while {1} expected! " +
        "This might mean that your query lacks a proper WHERE statement " +
        "or a TRIGGER changes number of affected rows. In the latter case, " +
        "you may try adding \"SET NOCOUNT ON\" to your trigger code.";

    private static int CheckExpectedRows(ExpectedRows expectedRows, int affectedRows)
    {
        if (expectedRows == ExpectedRows.Ignore)
            return affectedRows;

        if (expectedRows == ExpectedRows.One && affectedRows != 1)
            throw new InvalidOperationException(string.Format(ExpectedRowsError, affectedRows, 1));

        if (expectedRows == ExpectedRows.ZeroOrOne && affectedRows > 1)
            throw new InvalidOperationException(string.Format(ExpectedRowsError, affectedRows, "0 or 1"));

        return affectedRows;
    }

    private static IDbCommand CreateReturningIdentityCommand(SqlInsert query, IDbConnection connection, string queryText, ISqlDialect dialect, out IDbDataParameter param)
    {
        string identityColumn = query.IdentityColumn() ?? throw new ArgumentNullException("query.IdentityColumn");
        queryText += " RETURNING " + SqlSyntax.AutoBracket(identityColumn, dialect);

        if (dialect.UseReturningIntoVar)
            queryText += " INTO " + dialect.ParameterPrefix + identityColumn;

        var command = NewCommand(connection, queryText, query.Params);
        param = command.CreateParameter();
        param.Direction = dialect.UseReturningIntoVar ? ParameterDirection.ReturnValue : ParameterDirection.Output;
        param.ParameterName = identityColumn;
        param.DbType = DbType.Int64;
        command.Parameters.Add(param);
        return command;
    }

    private static long? ReadIdentityValue(IDataReader reader)
    {
        if (!reader.IsDBNull(0))
            return Convert.ToInt64(reader.GetValue(0));
        return null;
    }

    private static SqlUpdate CreateUpsertFallbackUpdate(SqlInsert query, IEnumerable<string> keyFields)
    {
        var tableName = query.TableName();
        var keySet = new HashSet<string>(keyFields, StringComparer.OrdinalIgnoreCase);

        var update = new SqlUpdate(tableName).Dialect(query.Dialect());
        foreach (var pair in query.GetFieldExpressions())
        {
            if (keySet.Contains(pair.Field))
                update.Where((new Criteria(pair.Field) == new Criteria(pair.Expression)).ToString());
            else
                update.SetTo(pair.Field, pair.Expression);
        }
        if (query.Params is { } prms)
            foreach (var p in prms)
                update.AddParam(p.Key, p.Value);

        return update;
    }

    /// <summary>
    /// Executes the specified query on the connection.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="connection">The connection.</param>
    /// <param name="logger">The logger.</param>
    public static void Execute(this SqlInsert query, IDbConnection connection, ILogger logger = null)
    {
        string commandText = query.ToString();
        if (connection is ISqlOperationInterceptor interceptor &&
            interceptor.ExecuteNonQuery(commandText, query.Params, ExpectedRows.One, query, true) is { HasValue: true })
            return;

        using var command = NewCommand(connection, commandText, query.Params);
        InternalExecuteNonQuery(command, logger);
    }

    /// <summary>
    /// Executes the specified query on the connection asynchronously.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="connection">The connection.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task ExecuteAsync(this SqlInsert query, IDbConnection connection, ILogger logger = null, CancellationToken cancellationToken = default)
    {
        string commandText = query.ToString();
        if (connection is ISqlOperationInterceptor interceptor &&
            await interceptor.ExecuteNonQueryAsync(commandText, query.Params, ExpectedRows.One, query, true, cancellationToken).ConfigureAwait(false) is { HasValue: true })
            return;

        using var command = NewCommand(connection, commandText, query.Params);
        await InternalExecuteNonQueryAsync(command, logger, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Executes an UPSERT (insert or update) query on the connection and returns the number of affected rows.
    /// The key fields are used to determine whether an existing record is updated or a new record is inserted.
    /// </summary>
    /// <param name="query">The insert query.</param>
    /// <param name="connection">The connection.</param>
    /// <param name="keyFields">List of key fields (e.g. primary key columns) used to match an existing record.</param>
    /// <param name="expectedRows">The expected rows. Used to validate the expected number of affected rows.</param>
    /// <param name="logger">The logger.</param>
    /// <returns>The number of affected rows.</returns>
    public static int ExecuteUpsert(this SqlInsert query, IDbConnection connection,
        IEnumerable<string> keyFields, ExpectedRows expectedRows = ExpectedRows.Ignore, ILogger logger = null)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(connection);

        if (!query.IsDialectOverridden)
            query.Dialect(connection.GetDialect());

        string commandText;
        try
        {
            commandText = query.ToUpsertString(keyFields);
        }
        catch (NotSupportedException)
        {
            // Unknown dialect: fall back to a non-atomic update-then-insert.
            var update = CreateUpsertFallbackUpdate(query, keyFields);
            if (update.Execute(connection, ExpectedRows.ZeroOrOne, logger) != 1)
                query.Execute(connection, logger);

            return CheckExpectedRows(expectedRows, 1);
        }

        if (connection is ISqlOperationInterceptor interceptor &&
            interceptor.ExecuteNonQuery(commandText, query.Params, expectedRows, query, getNewId: false) is { HasValue: true } intres)
            return (int)intres.Value;

        using var command = NewCommand(connection, commandText, query.Params);
        return CheckExpectedRows(expectedRows, InternalExecuteNonQuery(command, logger));
    }

    /// <summary>
    /// Executes an UPSERT (insert or update) query on the connection asynchronously and returns the number of affected rows.
    /// The key fields are used to determine whether an existing record is updated or a new record is inserted.
    /// </summary>
    /// <param name="query">The insert query.</param>
    /// <param name="connection">The connection.</param>
    /// <param name="keyFields">List of key fields (e.g. primary key columns) used to match an existing record.</param>
    /// <param name="expectedRows">The expected rows. Used to validate the expected number of affected rows.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of affected rows.</returns>
    public static async Task<int> ExecuteUpsertAsync(this SqlInsert query, IDbConnection connection,
        IEnumerable<string> keyFields, ExpectedRows expectedRows = ExpectedRows.Ignore, ILogger logger = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(connection);

        if (!query.IsDialectOverridden)
            query.Dialect(connection.GetDialect());

        string commandText;
        try
        {
            commandText = query.ToUpsertString(keyFields);
        }
        catch (NotSupportedException)
        {
            // Unknown dialect: fall back to a non-atomic update-then-insert.
            var update = CreateUpsertFallbackUpdate(query, keyFields);
            if (await update.ExecuteAsync(connection, ExpectedRows.ZeroOrOne, logger, cancellationToken).ConfigureAwait(false) != 1)
                await query.ExecuteAsync(connection, logger, cancellationToken).ConfigureAwait(false);

            return CheckExpectedRows(expectedRows, 1);
        }

        if (connection is ISqlOperationInterceptor interceptor &&
            await interceptor.ExecuteNonQueryAsync(commandText, query.Params, expectedRows, query, getNewId: false, cancellationToken).ConfigureAwait(false) is { HasValue: true } intres)
            return (int)intres.Value;

        using var command = NewCommand(connection, commandText, query.Params);
        return CheckExpectedRows(expectedRows, await InternalExecuteNonQueryAsync(command, logger, cancellationToken).ConfigureAwait(false));
    }

    /// <summary>
    /// Executes the specified update query on the connection and returns the number of affected rows.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="connection">The connection.</param>
    /// <param name="expectedRows">The expected rows. Used to validate the expected number of affected rows.</param>
    /// <param name="logger">The logger.</param>
    /// <returns>The number of affected rows.</returns>
    public static int Execute(this SqlUpdate query, IDbConnection connection, ExpectedRows expectedRows = ExpectedRows.One, ILogger logger = null)
    {
        string commandText = query.ToString();
        if (connection is ISqlOperationInterceptor interceptor &&
            interceptor.ExecuteNonQuery(commandText, query.Params, ExpectedRows.One, query, true) is { HasValue: true } intres)
            return (int)intres.Value;

        using var command = NewCommand(connection, commandText, query.Params);
        return CheckExpectedRows(expectedRows, InternalExecuteNonQuery(command, logger));
    }

    /// <summary>
    /// Executes the specified update query on the connection asynchronously and returns the number of affected rows.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="connection">The connection.</param>
    /// <param name="expectedRows">The expected rows. Used to validate the expected number of affected rows.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of affected rows.</returns>
    public static async Task<int> ExecuteAsync(this SqlUpdate query, IDbConnection connection, ExpectedRows expectedRows = ExpectedRows.One, ILogger logger = null, CancellationToken cancellationToken = default)
    {
        string commandText = query.ToString();
        if (connection is ISqlOperationInterceptor interceptor &&
            await interceptor.ExecuteNonQueryAsync(commandText, query.Params, ExpectedRows.One, query, true, cancellationToken).ConfigureAwait(false) is { HasValue: true } intres)
            return (int)intres.Value;

        using var command = NewCommand(connection, commandText, query.Params);
        return CheckExpectedRows(expectedRows, await InternalExecuteNonQueryAsync(command, logger, cancellationToken).ConfigureAwait(false));
    }

    /// <summary>
    /// Executes the specified delete query on the connection and returns the number of affected rows.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="connection">The connection.</param>
    /// <param name="expectedRows">The expected rows. Used to validate the expected number of affected rows.</param>
    /// <param name="logger">The logger.</param>
    /// <returns>
    /// The number of affected rows.
    /// </returns>
    public static int Execute(this SqlDelete query, IDbConnection connection, ExpectedRows expectedRows = ExpectedRows.One, ILogger logger = null)
    {
        ArgumentNullException.ThrowIfNull(query);

        var commandText = query.ToString();

        if (connection is ISqlOperationInterceptor interceptor &&
            interceptor.ExecuteNonQuery(commandText, query.Params, expectedRows, query, getNewId: false) is { HasValue: true } intres)
            return (int)intres.Value;

        using var command = NewCommand(connection, commandText, query.Params);
        return CheckExpectedRows(expectedRows, InternalExecuteNonQuery(command, logger));
    }

    /// <summary>
    /// Executes the specified delete query on the connection asynchronously and returns the number of affected rows.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="connection">The connection.</param>
    /// <param name="expectedRows">The expected rows. Used to validate the expected number of affected rows.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of affected rows.</returns>
    public static async Task<int> ExecuteAsync(this SqlDelete query, IDbConnection connection, ExpectedRows expectedRows = ExpectedRows.One, ILogger logger = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var commandText = query.ToString();

        if (connection is ISqlOperationInterceptor interceptor &&
            await interceptor.ExecuteNonQueryAsync(commandText, query.Params, expectedRows, query, getNewId: false, cancellationToken).ConfigureAwait(false) is { HasValue: true } intres)
            return (int)intres.Value;

        using var command = NewCommand(connection, commandText, query.Params);
        return CheckExpectedRows(expectedRows, await InternalExecuteNonQueryAsync(command, logger, cancellationToken).ConfigureAwait(false));
    }

    private static IDataReader InternalExecuteReader(IDbConnection connection, string commandText, IDictionary<string, object> param, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(connection);

        connection.EnsureOpen();

        try
        {
            IDbCommand command = NewCommand(connection, commandText, param);
            var stopwatch = ValueStopwatch.StartNew();
            try
            {
                logger ??= connection.GetLogger();
                 
                if (logger?.IsEnabled(LogLevel.Debug) == true)
                    LogCommand("ExecuteReader", command, logger);

                var result = command.ExecuteReader();

                if (logger?.IsEnabled(LogLevel.Debug) == true)
                    logger.LogDebug("SQL - {method}[{uid}] - END - {ElapsedMilliseconds} ms",
                        "ExecuteReader", command.GetHashCode(), stopwatch.ElapsedMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                if (CheckConnectionPoolException(connection, ex))
                    return command.ExecuteReader();
                else
                    throw;
            }
        }
        catch (Exception ex)
        {
            ex.SetData("sql_command_text", commandText);
            throw;
        }
    }

    /// <summary>
    /// Executes the command returning a data reader.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="commandText">The command text.</param>
    /// <param name="param">The parameters.</param>
    /// <param name="logger">The logger.</param>
    /// <returns>A data reader with the results.</returns>
    /// <exception cref="ArgumentNullException">connection is null.</exception>
    public static IDataReader ExecuteReader(IDbConnection connection, string commandText,
        IDictionary<string, object> param, ILogger logger = null)
    {
        if (connection is ISqlOperationInterceptor interceptor &&
            interceptor.ExecuteReader(commandText, param, query: null) is { HasValue: true } intres)
            return intres.Value;

        return InternalExecuteReader(connection, commandText, param, logger);
    }

    private static async Task<IDataReader> ExecuteReaderAsync(IDbCommand command, CancellationToken cancellationToken)
    {
        if (command is System.Data.Common.DbCommand dbCommand)
            return await dbCommand.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

        cancellationToken.ThrowIfCancellationRequested();
        return command.ExecuteReader();
    }

    private static async Task<IDataReader> InternalExecuteReaderAsync(IDbConnection connection, string commandText, IDictionary<string, object> param, ILogger logger, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(connection);

        await connection.EnsureOpenAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            IDbCommand command = NewCommand(connection, commandText, param);
            var stopwatch = ValueStopwatch.StartNew();
            try
            {
                logger ??= connection.GetLogger();

                if (logger?.IsEnabled(LogLevel.Debug) == true)
                    LogCommand("ExecuteReader", command, logger);

                var result = await ExecuteReaderAsync(command, cancellationToken).ConfigureAwait(false);

                if (logger?.IsEnabled(LogLevel.Debug) == true)
                    logger.LogDebug("SQL - {method}[{uid}] - END - {ElapsedMilliseconds} ms",
                        "ExecuteReader", command.GetHashCode(), stopwatch.ElapsedMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                if (await CheckConnectionPoolExceptionAsync(connection, ex, cancellationToken).ConfigureAwait(false))
                    return await ExecuteReaderAsync(command, cancellationToken).ConfigureAwait(false);
                else
                    throw;
            }
        }
        catch (Exception ex)
        {
            ex.SetData("sql_command_text", commandText);
            throw;
        }
    }

    /// <summary>
    /// Executes the command asynchronously returning a data reader.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="commandText">The command text.</param>
    /// <param name="param">The parameters.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a data reader with the results.</returns>
    /// <exception cref="ArgumentNullException">connection is null.</exception>
    public static async Task<IDataReader> ExecuteReaderAsync(IDbConnection connection, string commandText,
        IDictionary<string, object> param, ILogger logger = null, CancellationToken cancellationToken = default)
    {
        if (connection is ISqlOperationInterceptor interceptor &&
            await interceptor.ExecuteReaderAsync(commandText, param, query: null, cancellationToken).ConfigureAwait(false) is { HasValue: true } intres)
            return intres.Value;

        return await InternalExecuteReaderAsync(connection, commandText, param, logger, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Executes the query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="connection">The connection.</param>
    /// <param name="logger">The logger.</param>
    /// <returns>A data reader with the results.</returns>
    public static IDataReader ExecuteReader(this SqlQuery query, IDbConnection connection, ILogger logger = null)
    {
        ArgumentNullException.ThrowIfNull(query);

        var commandText = query.ToString();
        if (connection is ISqlOperationInterceptor interceptor &&
            interceptor.ExecuteReader(commandText, query.Params, query) is { HasValue: true } intres)
            return intres.Value;

        return InternalExecuteReader(connection, commandText, query.Params, logger);
    }

    /// <summary>
    /// Executes the query asynchronously.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="connection">The connection.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a data reader with the results.</returns>
    public static async Task<IDataReader> ExecuteReaderAsync(this SqlQuery query, IDbConnection connection, ILogger logger = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var commandText = query.ToString();
        if (connection is ISqlOperationInterceptor interceptor &&
            await interceptor.ExecuteReaderAsync(commandText, query.Params, query, cancellationToken).ConfigureAwait(false) is { HasValue: true } intres)
            return intres.Value;

        return await InternalExecuteReaderAsync(connection, commandText, query.Params, logger, cancellationToken).ConfigureAwait(false);
    }

    private static object InternalExecuteScalar(IDbConnection connection, string commandText, IDictionary<string, object> param, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(connection);

        connection.EnsureOpen();

        using IDbCommand command = NewCommand(connection, commandText, param);
        try
        {
            var stopwatch = ValueStopwatch.StartNew();
            try
            {
                logger ??= connection.GetLogger();

                if (logger?.IsEnabled(LogLevel.Debug) == true)
                    LogCommand("ExecuteScalar", command, logger);

                var result = command.ExecuteScalar();

                if (logger?.IsEnabled(LogLevel.Debug) == true)
                    logger.LogDebug("SQL - {method}[{uid}] - END - {ElapsedMilliseconds} ms", 
                        "ExecuteScalar", command.GetHashCode(), stopwatch.ElapsedMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                if (CheckConnectionPoolException(connection, ex))
                    return command.ExecuteScalar();
                else
                    throw;
            }
        }
        catch (Exception ex)
        {
            ex.SetData("sql_command_text", commandText);
            throw;
        }
    }

    /// <summary>
    /// Executes the statement returning a scalar value.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="commandText">The command text.</param>
    /// <param name="param">The parameters.</param>
    /// <param name="logger">The logger.</param>
    /// <returns>The scalar value.</returns>
    /// <exception cref="ArgumentNullException">connection is null.</exception>
    public static object ExecuteScalar(IDbConnection connection, string commandText, IDictionary<string, object> param = null, ILogger logger = null)
    {
        if (connection is ISqlOperationInterceptor interceptor &&
            interceptor.ExecuteScalar(commandText, param, query: null) is { HasValue: true } intres)
            return intres.Value;

        return InternalExecuteScalar(connection, commandText, param, logger);
    }

    private static Task<object> ExecuteScalarAsync(IDbCommand command, CancellationToken cancellationToken)
    {
        if (command is System.Data.Common.DbCommand dbCommand)
            return dbCommand.ExecuteScalarAsync(cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(command.ExecuteScalar());
    }

    private static async Task<object> InternalExecuteScalarAsync(IDbConnection connection, string commandText, IDictionary<string, object> param, ILogger logger, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(connection);

        await connection.EnsureOpenAsync(cancellationToken).ConfigureAwait(false);

        using IDbCommand command = NewCommand(connection, commandText, param);
        try
        {
            var stopwatch = ValueStopwatch.StartNew();
            try
            {
                logger ??= connection.GetLogger();

                if (logger?.IsEnabled(LogLevel.Debug) == true)
                    LogCommand("ExecuteScalar", command, logger);

                var result = await ExecuteScalarAsync(command, cancellationToken).ConfigureAwait(false);

                if (logger?.IsEnabled(LogLevel.Debug) == true)
                    logger.LogDebug("SQL - {method}[{uid}] - END - {ElapsedMilliseconds} ms",
                        "ExecuteScalar", command.GetHashCode(), stopwatch.ElapsedMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                if (await CheckConnectionPoolExceptionAsync(connection, ex, cancellationToken).ConfigureAwait(false))
                    return await ExecuteScalarAsync(command, cancellationToken).ConfigureAwait(false);
                else
                    throw;
            }
        }
        catch (Exception ex)
        {
            ex.SetData("sql_command_text", commandText);
            throw;
        }
    }

    /// <summary>
    /// Executes the statement asynchronously returning a scalar value.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="commandText">The command text.</param>
    /// <param name="param">The parameters.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the scalar value.</returns>
    /// <exception cref="ArgumentNullException">connection is null.</exception>
    public static async Task<object> ExecuteScalarAsync(IDbConnection connection, string commandText, IDictionary<string, object> param = null, ILogger logger = null, CancellationToken cancellationToken = default)
    {
        if (connection is ISqlOperationInterceptor interceptor &&
            await interceptor.ExecuteScalarAsync(commandText, param, query: null, cancellationToken).ConfigureAwait(false) is { HasValue: true } intres)
            return intres.Value;

        return await InternalExecuteScalarAsync(connection, commandText, param, logger, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Executes the statement returning a scalar value.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="query">The select query.</param>
    /// <param name="logger">The logger.</param>
    /// <returns>The scalar value.</returns>
    /// <exception cref="ArgumentNullException">selectQuery is null.</exception>
    public static object ExecuteScalar(IDbConnection connection, SqlQuery query, ILogger logger = null)
    {
        ArgumentNullException.ThrowIfNull(query);

        string commandText = query.ToString();
        if (connection is ISqlOperationInterceptor interceptor &&
            interceptor.ExecuteScalar(commandText, query.Params, query) is { HasValue: true } intres)
            return intres.Value;

        return InternalExecuteScalar(connection, commandText, query.Params, logger);
    }

    /// <summary>
    /// Executes the statement asynchronously returning a scalar value.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="query">The select query.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the scalar value.</returns>
    /// <exception cref="ArgumentNullException">selectQuery is null.</exception>
    public static async Task<object> ExecuteScalarAsync(IDbConnection connection, SqlQuery query, ILogger logger = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        string commandText = query.ToString();
        if (connection is ISqlOperationInterceptor interceptor &&
            await interceptor.ExecuteScalarAsync(commandText, query.Params, query, cancellationToken).ConfigureAwait(false) is { HasValue: true } intres)
            return intres.Value;

        return await InternalExecuteScalarAsync(connection, commandText, query.Params, logger, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Executes the query returning true if it has at least one result.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="connection">The connection.</param>
    /// <param name="logger">The logger.</param>
    /// <returns>True if the query returns at least one result.</returns>
    public static bool Exists(this SqlQuery query, IDbConnection connection, ILogger logger = null)
    {
        using var reader = ExecuteReader(query, connection, logger);
        return reader.Read();
    }

    /// <summary>
    /// Executes the query asynchronously returning true if it has at least one result.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="connection">The connection.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is true if the query returns at least one result.</returns>
    public static async Task<bool> ExistsAsync(this SqlQuery query, IDbConnection connection, ILogger logger = null, CancellationToken cancellationToken = default)
    {
        using var reader = await ExecuteReaderAsync(query, connection, logger, cancellationToken).ConfigureAwait(false);
        return await reader.ReadAsync(cancellationToken).ConfigureAwait(false);
    }
}