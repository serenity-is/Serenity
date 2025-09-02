using Microsoft.Extensions.Logging;
using System.IO;

namespace Serenity.Data;

/// <summary>
/// Contains static SQL related helper functions and extensions.
/// </summary>
public static class SqlHelper
{
    /// <summary>
    /// Fixes the type of the parameter to something suitable as SQL parameter.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
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
    /// <param name="method">The type.</param>
    /// <param name="command">The command.</param>
    /// <param name="logger">Logger</param>
    public static void LogCommand(string method, IDbCommand command, ILogger logger)
    {
        if (logger == null)
            throw new ArgumentNullException(nameof(logger));

        try
        {
            logger.LogDebug("SQL - {method}[{uid}] - START\n{sql}", method, command.GetHashCode(), SqlCommandDumper.GetCommandText(command));
        }
        catch (Exception ex)
        {
            logger.LogDebug(ex, "Error logging command: {");
        }
    }

    /// <summary>
    /// Adds the parameter with value to the target command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    /// <param name="dialect">The dialect.</param>
    /// <returns>New parameter</returns>
    public static IDbDataParameter AddParamWithValue(this IDbCommand command, string name, object value, ISqlDialect dialect)
    {
        name = dialect.ParameterPrefix != '@' &&
            name.StartsWith("@") ? dialect.ParameterPrefix + name[1..] :
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

                if (mappedType != param.DbType && mappedType != null)
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
    /// Creates new command.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="commandText">The command text.</param>
    /// <param name="param">The parameters.</param>
    /// <returns>New command with specified command text and parameters</returns>
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
    /// <returns>A new command with specified command text</returns>
    /// <exception cref="ArgumentNullException">connection</exception>
    public static IDbCommand NewCommand(IDbConnection connection, string commandText)
    {
        if (connection == null)
            throw new ArgumentNullException("connection");

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
            connection.Close();
            connection.Open();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Executes the SQL statement, and returns affected rows.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="logger">Logger</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">
    /// command is null or command.Connection is null.
    /// </exception>
    private static int InternalExecuteNonQuery(IDbCommand command, ILogger logger)
    {
        if (command == null)
            throw new ArgumentNullException("command");

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
    /// Executes the statement
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="commandText">The command text.</param>
    /// <param name="param">The parameters.</param>
    /// <param name="logger">Logger</param>
    /// <returns>Number of affected rows</returns>
    public static int ExecuteNonQuery(IDbConnection connection, string commandText, IDictionary<string, object> param = null, ILogger logger = null)
    {
        if (connection is ISqlOperationInterceptor interceptor &&
            interceptor.ExecuteNonQuery(commandText, param, ExpectedRows.Ignore, query: null, getNewId: false) is { HasValue: true } intres)
            return (int)intres.Value;
        using IDbCommand command = NewCommand(connection, commandText, param);
        return InternalExecuteNonQuery(command, logger);
    }

    /// <summary>
    /// Executes the query and returns the generated identity value.
    /// Only works for auto incremented fields, not GUIDs.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="connection">The connection.</param>
    /// <param name="logger">Logger</param>
    /// <exception cref="ArgumentNullException">query.IdentityColumn is null</exception>
    /// <exception cref="NotImplementedException">The connection dialect doesn't support returning inserted identity.</exception>
    public static long? ExecuteAndGetID(this SqlInsert query, IDbConnection connection, ILogger logger = null)
    {
        string queryText = query.ToString();

        if (connection is ISqlOperationInterceptor interceptor &&
            interceptor.ExecuteNonQuery(queryText, query.Params, ExpectedRows.One, query, getNewId: true) is { HasValue: true } intres)
            return intres.Value;

        var dialect = connection.GetDialect();
        if (dialect.UseReturningIdentity || dialect.UseReturningIntoVar)
        {
            string identityColumn = query.IdentityColumn() ?? throw new ArgumentNullException("query.IdentityColumn");
            queryText += " RETURNING " + SqlSyntax.AutoBracket(identityColumn, dialect);

            if (dialect.UseReturningIntoVar)
                queryText += " INTO " + dialect.ParameterPrefix + identityColumn;

            using var command = NewCommand(connection, queryText, query.Params);
            var param = command.CreateParameter();
            param.Direction = dialect.UseReturningIntoVar ? ParameterDirection.ReturnValue : ParameterDirection.Output;
            param.ParameterName = identityColumn;
            param.DbType = DbType.Int64;
            command.Parameters.Add(param);
            InternalExecuteNonQuery(command, logger);
            return Convert.ToInt64(param.Value);
        }

        if (dialect.UseScopeIdentity)
        {
            var scopeIdentityExpression = dialect.ScopeIdentityExpression;

            queryText += ";\nSELECT " + scopeIdentityExpression + " AS IDCOLUMNVALUE";

            using IDataReader reader = InternalExecuteReader(connection, queryText, query.Params, logger);
            if (reader.Read() &&
                !reader.IsDBNull(0))
                return Convert.ToInt64(reader.GetValue(0));
            return null;
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

    /// <summary>
    /// Executes the specified query on connection.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="connection">The connection.</param>
    /// <param name="logger">Logger</param>
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
    /// Executes the specified update query on connection and returns number of affected rows.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="connection">The connection.</param>
    /// <param name="expectedRows">The expected rows. Used to validate expected number of affected rows.</param>
    /// <param name="logger">Logger</param>
    /// <returns>Number of affected rows.</returns>
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
    /// Executes the specified delete query on connection and returns number of affected rows.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="connection">The connection.</param>
    /// <param name="expectedRows">The expected rows. Used to validate expected number of affected rows.</param>
    /// <param name="logger">Logger</param>
    /// <returns>
    /// Number of affected rows.
    /// </returns>
    public static int Execute(this SqlDelete query, IDbConnection connection, ExpectedRows expectedRows = ExpectedRows.One, ILogger logger = null)
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        var commandText = query.ToString();

        if (connection is ISqlOperationInterceptor interceptor &&
            interceptor.ExecuteNonQuery(commandText, query.Params, expectedRows, query, getNewId: false) is { HasValue: true } intres)
            return (int)intres.Value;

        using var command = NewCommand(connection, commandText, query.Params);
        return CheckExpectedRows(expectedRows, InternalExecuteNonQuery(command, logger));
    }

    private static IDataReader InternalExecuteReader(IDbConnection connection, string commandText, IDictionary<string, object> param, ILogger logger)
    {
        if (connection == null)
            throw new ArgumentNullException("connection");

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
    /// <param name="logger">Logger</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">connection is null</exception>
    public static IDataReader ExecuteReader(IDbConnection connection, string commandText,
        IDictionary<string, object> param, ILogger logger = null)
    {
        if (connection is ISqlOperationInterceptor interceptor &&
            interceptor.ExecuteReader(commandText, param, query: null) is { HasValue: true } intres)
            return intres.Value;

        return InternalExecuteReader(connection, commandText, param, logger);
    }

    /// <summary>
    /// Executes the query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="connection">The connection.</param>
    /// <param name="logger">Logger</param>
    /// <returns>A data reader with results.</returns>
    public static IDataReader ExecuteReader(this SqlQuery query, IDbConnection connection, ILogger logger = null)
    {
        if (query == null) 
            throw new ArgumentNullException("query");

        var commandText = query.ToString();
        if (connection is ISqlOperationInterceptor interceptor &&
            interceptor.ExecuteReader(commandText, query.Params, query) is { HasValue: true } intres)
            return intres.Value;

        return InternalExecuteReader(connection, commandText, query.Params, logger);
    }

    private static object InternalExecuteScalar(IDbConnection connection, string commandText, IDictionary<string, object> param, ILogger logger)
    {
        if (connection == null)
            throw new ArgumentNullException("connection");

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
    /// <param name="logger">Logger</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">connection</exception>
    public static object ExecuteScalar(IDbConnection connection, string commandText, IDictionary<string, object> param = null, ILogger logger = null)
    {
        if (connection is ISqlOperationInterceptor interceptor &&
            interceptor.ExecuteScalar(commandText, param, query: null) is { HasValue: true } intres)
            return intres.Value;

        return InternalExecuteScalar(connection, commandText, param, logger);
    }

    /// <summary>
    /// Executes the statement returning a scalar value.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="query">The select query.</param>
    /// <param name="logger">Logger</param>
    /// <returns>Scalar value</returns>
    /// <exception cref="ArgumentNullException">selectQuery is null</exception>
    public static object ExecuteScalar(IDbConnection connection, SqlQuery query, ILogger logger = null)
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        string commandText = query.ToString();
        if (connection is ISqlOperationInterceptor interceptor &&
            interceptor.ExecuteReader(commandText, query.Params, query) is { HasValue: true } intres)
            return intres.Value;

        return InternalExecuteScalar(connection, commandText, query.Params, logger);
    }

    /// <summary>
    /// Executes the query returning true if it has at least one result.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="connection">The connection.</param>
    /// <param name="logger">Logger</param>
    /// <returns>True if query returns one result.</returns>
    public static bool Exists(this SqlQuery query, IDbConnection connection, ILogger logger = null)
    {
        using var reader = ExecuteReader(query, connection, logger);
        return reader.Read();
    }
}