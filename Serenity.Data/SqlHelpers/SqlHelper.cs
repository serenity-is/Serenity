namespace Serenity.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.IO;
    using System.Text;
    using Dictionary = System.Collections.Generic.Dictionary<string, object>;

    /// <summary>
    /// Contains static SQL related helper functions and extensions.
    /// </summary>
    public static class SqlHelper
    {
        /// <summary>
        /// Determines whether the exception is a database exception.
        /// </summary>
        /// <param name="e">The exception.</param>
        /// <returns>
        ///   <c>true</c> if database exception; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsDatabaseException(Exception e)
        {
            return e != null && e is SqlException;
        }

        /// <summary>
        /// Executes the query and returns the generated identity value.
        /// Only works for auto incremented fields, not GUIDs.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="connection">The connection.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">query.IdentityColumn is null</exception>
        /// <exception cref="NotImplementedException">The connection dialect doesn't support returning inserted identity.</exception>
        public static Int64? ExecuteAndGetID(this SqlInsert query, IDbConnection connection)
        {
            string queryText = query.ToString();
            var dialect = connection.GetDialect();
            if (dialect.UseReturningIdentity || dialect.UseReturningIntoVar)
            {
                string identityColumn = query.IdentityColumn();
                if (identityColumn == null)
                    throw new ArgumentNullException("query.IdentityColumn");

                queryText += " RETURNING " + SqlSyntax.AutoBracket(identityColumn);

                if (dialect.UseReturningIntoVar)
                    queryText += " INTO " + dialect.ParameterPrefix + identityColumn;

                using (var command = NewCommand(connection, queryText, query.Params))
                {
                    var param = command.CreateParameter();
                    param.Direction = dialect.UseReturningIntoVar ? ParameterDirection.ReturnValue : ParameterDirection.Output;
                    param.ParameterName = identityColumn;
                    param.DbType = DbType.Int64;
                    command.Parameters.Add(param);
                    ExecuteNonQuery(command);
                    return Convert.ToInt64(param.Value);
                }
            }

            if (dialect.UseScopeIdentity)
            {
                var scopeIdentityExpression = dialect.ScopeIdentityExpression;

                queryText += ";\nSELECT " + scopeIdentityExpression + " AS IDCOLUMNVALUE";

                using (IDataReader reader = ExecuteReader(connection, queryText, query.Params))
                {
                    if (reader.Read() &&
                        !reader.IsDBNull(0))
                        return Convert.ToInt64(reader.GetValue(0));
                    return null;
                }
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Executes the specified query on connection.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="connection">The connection.</param>
        public static void Execute(this SqlInsert query, IDbConnection connection)
        {
            ExecuteNonQuery(connection, query.ToString(), query.Params);
        }

        /// <summary>
        /// Executes the query on connection with specified params.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="param">The parameters.</param>
        public static void Execute(this SqlInsert query, IDbConnection connection, Dictionary param)
        {
            ExecuteNonQuery(connection, query.ToString(), param);
        }

        /// <summary>
        /// Executes the specified update query on connection and returns number of affected rows.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="expectedRows">The expected rows. Used to validate expected number of affected rows.</param>
        /// <returns>Number of affected rows.</returns>
        public static int Execute(this SqlUpdate query, IDbConnection connection, ExpectedRows expectedRows = ExpectedRows.One)
        {
            return CheckExpectedRows(expectedRows, ExecuteNonQuery(connection, query.ToString(), query.Params));
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
                throw new InvalidOperationException(String.Format(ExpectedRowsError, affectedRows, 1));

            if (expectedRows == ExpectedRows.ZeroOrOne && affectedRows > 1)
                throw new InvalidOperationException(String.Format(ExpectedRowsError, affectedRows, "0 or 1"));

            return affectedRows;
        }

        /// <summary>
        /// Executes the specified delete query on connection and returns number of affected rows.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="expectedRows">The expected rows. Used to validate expected number of affected rows.</param>
        /// <returns>
        /// Number of affected rows.
        /// </returns>
        public static int Execute(this SqlDelete query, IDbConnection connection, ExpectedRows expectedRows = ExpectedRows.One)
        {
            return CheckExpectedRows(expectedRows, ExecuteNonQuery(connection, query.ToString(), query.Params));
        }

        /// <summary>
        /// Executes the specified delete query on connection and returns number of affected rows.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="param">The parameters.</param>
        /// <param name="expectedRows">The expected rows. Used to validate expected number of affected rows.</param>
        /// <returns>
        /// Number of affected rows.
        /// </returns>
        public static int Execute(this SqlDelete query, IDbConnection connection, Dictionary param, ExpectedRows expectedRows = ExpectedRows.One)
        {
            return CheckExpectedRows(expectedRows, ExecuteNonQuery(connection, query.ToString(), param));
        }

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="connection">The connection.</param>
        /// <returns>A data reader with results.</returns>
        public static IDataReader ExecuteReader(this SqlQuery query, IDbConnection connection)
        {
            return ExecuteReader(connection, query.ToString(), query.Params);
        }

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="param">The parameters.</param>
        /// <returns>A data reader with results</returns>
        public static IDataReader ExecuteReader(this SqlQuery query, IDbConnection connection, Dictionary param)
        {
            return ExecuteReader(connection, query.ToString(), param);
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

            // TODO: find a workaround in new Dapper
#if !COREFX
            SqlMapper.GetBindByName(command.GetType())?.Invoke(command, true);
#endif
            commandText = FixCommandText(commandText, connection.GetDialect());
            command.CommandText = commandText;
            return command;
        }

        /// <summary>
        /// Fixes the command text for target dialect by replacing brackets ([]), and parameter prefixes (@).
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="dialect">The dialect.</param>
        /// <returns>Fixed query.</returns>
        public static string FixCommandText(string commandText, ISqlDialect dialect)
        {
            commandText = DatabaseCaretReferences.Replace(commandText);

            var openBracket = dialect.OpenQuote;
            if (openBracket != '[')
                commandText = BracketLocator.ReplaceBrackets(commandText, dialect);

            var paramPrefix = dialect.ParameterPrefix;
            if (paramPrefix != '@')
                commandText = ParamPrefixReplacer.Replace(commandText, paramPrefix);

            return commandText;
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
            var command = (DbCommand)(NewCommand(connection, commandText));

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
        /// Fixes the type of the parameter to something suitable as SQL parameter.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static object FixParamType(object value)
        {
            if (value == null)
                return DBNull.Value;

            if (value is Stream)
            {
                if (value is MemoryStream)
                    return ((MemoryStream)value).ToArray();
                else
                {
                    using (var ms = new MemoryStream())
                    {
                        ((Stream)value).CopyTo(ms);
                        return value = ms.ToArray();
                    }
                }
            }

            if (value is Enum)
            {
                var underlyingType = Enum.GetUnderlyingType(value.GetType());
                if (underlyingType == typeof(Int32))
                    return (int)value;
                else if (underlyingType == typeof(Int16))
                    return (Int16)value;
                else
                    return Convert.ChangeType(value, underlyingType);
            }

            return value;
        }

        /// <summary>
        /// Adds the parameter with value to the target command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="dialect">The dialect.</param>
        /// <returns>New parameter</returns>
        public static DbParameter AddParamWithValue(this DbCommand command, string name, object value, ISqlDialect dialect)
        {
            DbParameter param = command.CreateParameter();

            name = dialect.ParameterPrefix != '@' &&
                name.StartsWith("@") ? dialect.ParameterPrefix + name.Substring(1) :
                    name;

            param.ParameterName = name;

            value = FixParamType(value) ?? DBNull.Value;

            if (value is Boolean && dialect.NeedsBoolWorkaround)
            {
                // otherwise argument out of range exception!
                param.Value = (Boolean)value ? 1 : 0;
            }
            else
            {
                param.Value = value;

                if (value != null && value != DBNull.Value)
                {
#if COREFX
#pragma warning disable CS0618
                    var mappedType = Dapper.SqlMapper.GetDbType(value);
#pragma warning restore CS0618
#else
                    var mappedType = SqlMapper.LookupDbType(value.GetType(), name);
#endif

                    if (mappedType != param.DbType)
                        param.DbType = mappedType;

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
        /// Checks for the connection pool exception.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="exception">The exception.</param>
        /// <returns>True if exception is 10054, e.g. connection pool.</returns>
        private static bool CheckConnectionPoolException(IDbConnection connection, Exception exception)
        {
            if (exception is SqlException ex && ex.Number == 10054)
            {
                if (connection is WrappedConnection wrapped && 
                    (wrapped.OpenedOnce || wrapped.CurrentTransaction != null))
                    return false;

                System.Data.SqlClient.SqlConnection.ClearAllPools();
                connection.Close();
                connection.Open();
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Executes the SQL statement, and returns affected rows.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// command is null or command.Connection is null.
        /// </exception>
        public static int ExecuteNonQuery(IDbCommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            if (command.Connection == null)
                throw new ArgumentNullException("command");

            try
            {
                command.Connection.EnsureOpen();
                try
                {
                    return command.ExecuteNonQuery();
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    if (CheckConnectionPoolException(command.Connection, ex))
                        return command.ExecuteNonQuery();
                    else
                        throw;
                }
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
        /// <returns>Number of affected rows</returns>
        public static int ExecuteNonQuery(IDbConnection connection, string commandText,
            IDictionary<string, object> param)
        {
            using (IDbCommand command = NewCommand(connection, commandText, param))
            {
                if (Log.IsDebugEnabled)
                    LogCommand("ExecuteNonQuery", command);

                var result = ExecuteNonQuery(command);

                if (Log.IsDebugEnabled)
                    Log.Debug("END - ExecuteNonQuery");

                return result;
            }
        }

        /// <summary>
        /// Executes the statement.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="commandText">The command text.</param>
        /// <returns>Number of affected rows</returns>
        public static int ExecuteNonQuery(IDbConnection connection, string commandText)
        {
            using (IDbCommand command = NewCommand(connection, commandText))
            {
                if (Log.IsDebugEnabled)
                    LogCommand("ExecuteNonQuery", command);

                var result = ExecuteNonQuery(command);

                if (Log.IsDebugEnabled)
                    Log.Debug("END - ExecuteNonQuery");

                return result;
            }
        }

        /// <summary>
        /// Executes the statement returning a scalar value.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="param">The parameters.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">connection</exception>
        public static object ExecuteScalar(IDbConnection connection, string commandText, IDictionary<string, object> param)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            connection.EnsureOpen();

            using (IDbCommand command = NewCommand(connection, commandText, param))
            {
                try
                {
                    try
                    {
                        if (Log.IsDebugEnabled)
                            LogCommand("ExecuteScalar", command);

                        var result = command.ExecuteScalar();

                        if (Log.IsDebugEnabled)
                            Log.Debug("END - ExecuteScalar");

                        return result;
                    }
                    catch (System.Data.SqlClient.SqlException ex)
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
        }

        /// <summary>
        /// Executes the statement returning a scalar value.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="commandText">The command text.</param>
        /// <returns>Scalar value</returns>
        public static object ExecuteScalar(IDbConnection connection, string commandText)
        {
            return ExecuteScalar(connection, commandText, null);
        }

        /// <summary>
        /// Executes the statement returning a scalar value.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="selectQuery">The select query.</param>
        /// <returns>Scalar value</returns>
        /// <exception cref="ArgumentNullException">selectQuery is null</exception>
        public static object ExecuteScalar(IDbConnection connection, SqlQuery selectQuery)
        {
            if (selectQuery == null)
                throw new ArgumentNullException("selectQuery");

            return ExecuteScalar(connection, selectQuery.ToString(), selectQuery.Params);
        }

        /// <summary>
        /// Executes the statement returning a scalar value.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="selectQuery">The select query.</param>
        /// <param name="param">The parameters.</param>
        /// <returns>
        /// Scalar value
        /// </returns>
        /// <exception cref="ArgumentNullException">selectQuery is null</exception>
        public static object ExecuteScalar(IDbConnection connection, SqlQuery selectQuery, Dictionary param)
        {
            if (selectQuery == null)
                throw new ArgumentNullException("selectQuery");

            return ExecuteScalar(connection, selectQuery.ToString(), param);
        }

        /// <summary>
        /// Logs the command.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="command">The command.</param>
        public static void LogCommand(string type, IDbCommand command)
        {
            try
            {
                if (command is SqlCommand sqlCmd)
                {
                    Log.Debug(type + "\r\n" + SqlCommandDumper.GetCommandText(sqlCmd));
                    return;
                }

                StringBuilder sb = new StringBuilder((command.CommandText ?? "").Length + 1000);
                sb.Append(type);
                sb.Append("\r\n");
                sb.Append(command.CommandText);
                if (command.Parameters != null && command.Parameters.Count > 0)
                {
                    sb.Append(" --- PARAMS --- ");
                    foreach (DbParameter p in command.Parameters)
                    {
                        sb.Append(p.ParameterName);
                        sb.Append("=");
                        if (p.Value == null || p.Value == DBNull.Value)
                            sb.Append("<NULL>");
                        else
                            sb.Append(p.Value.ToString());
                        sb.Append(" ");
                    }
                }

                Log.Debug(sb.ToString());
            }
            catch (Exception ex)
            {
                Log.Debug("Error logging command: " + ex.ToString());
            }
        }

        /// <summary>
        /// Executes the command returning a data reader.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="param">The parameters.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">connection is null</exception>
        public static IDataReader ExecuteReader(IDbConnection connection, string commandText,
            IDictionary<string, object> param)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            connection.EnsureOpen();

            try
            {
                //using (new Tracer(commandText))
                {
                    IDbCommand command = NewCommand(connection, commandText, param);
                    try
                    {
                        if (Log.IsDebugEnabled)
                            LogCommand("ExecuteReader", command);

                        var result = command.ExecuteReader();

                        if (Log.IsDebugEnabled)
                            Log.Debug("END - ExecuteReader");

                        return result;
                    }
                    catch (System.Data.SqlClient.SqlException ex)
                    {
                        if (CheckConnectionPoolException(connection, ex))
                            return command.ExecuteReader();
                        else
                            throw;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.SetData("sql_command_text", commandText);
                throw;
            }
        }

        /// <summary>
        /// Executes the statement returning a data reader.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="commandText">The command text.</param>
        /// <returns>Data reader with results</returns>
        public static IDataReader ExecuteReader(IDbConnection connection, string commandText)
        {
            return ExecuteReader(connection, commandText, null);
        }

        /// <summary>
        /// Executes the statement returning a data reader.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="query">The query.</param>
        /// <returns>
        /// Data reader with results
        /// </returns>
        public static IDataReader ExecuteReader(IDbConnection connection, SqlQuery query)
        {
            return ExecuteReader(connection, query.ToString(), query.Params);
        }

        /// <summary>
        /// Executes the statement returning a data reader.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="query">The query.</param>
        /// <param name="param">The parameters.</param>
        /// <returns>
        /// Data reader with results
        /// </returns>
        public static IDataReader ExecuteReader(IDbConnection connection, SqlQuery query, Dictionary param)
        {
            return ExecuteReader(connection, query.ToString(), param);
        }


        /// <summary>
        /// Executes the query returning true if it has at least one result.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="connection">The connection.</param>
        /// <returns>True if query returns one result.</returns>
        public static bool Exists(this SqlQuery query, IDbConnection connection)
        {
            using (IDataReader reader = ExecuteReader(connection, query))
                return reader.Read();
        }
    }
}