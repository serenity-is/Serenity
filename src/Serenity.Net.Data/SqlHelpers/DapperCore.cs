using Dapper;

namespace Serenity.Data;

/// <summary>
/// Dapper wrapper
/// </summary>
public static partial class SqlMapper
{
    /// <summary>
    /// Execute parameterized SQL
    /// </summary>
    /// <param name="cnn">Connection.</param>
    /// <param name="sql">SQL query.</param>
    /// <param name="param">The parameters.</param>
    /// <param name="transaction">The transaction.</param>
    /// <param name="commandTimeout">The command timeout.</param>
    /// <param name="commandType">Type of the command.</param>
    /// <returns>
    /// Number of rows affected
    /// </returns>
    public static int Execute(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        cnn.EnsureOpen();
        return Dapper.SqlMapper.Execute(cnn, SqlHelper.FixCommandText(sql, cnn.GetDialect()), param, transaction, commandTimeout, commandType);
    }

    /// <summary>
    /// Return a list of dynamic objects, reader is closed after the call
    /// </summary>
    /// <param name="cnn">Connection.</param>
    /// <param name="sql">SQL query.</param>
    /// <param name="param">The parameters.</param>
    /// <param name="transaction">The transaction.</param>
    /// <param name="buffered">if set to <c>true</c> results are buffered.</param>
    /// <param name="commandTimeout">The command timeout.</param>
    /// <param name="commandType">Type of the command.</param>
    /// <returns>List of dynamic objects</returns>
    public static IEnumerable<dynamic> Query(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
    {
        cnn.EnsureOpen();
        return Dapper.SqlMapper.Query(cnn, SqlHelper.FixCommandText(sql, cnn.GetDialect()), param, transaction, buffered, commandTimeout, commandType);
    }

    /// <summary>
    /// Return a list of dynamic objects, reader is closed after the call
    /// </summary>
    /// <param name="cnn">Connection.</param>
    /// <param name="sql">SQL query.</param>
    /// <param name="transaction">The transaction.</param>
    /// <param name="buffered">if set to <c>true</c> results are buffered.</param>
    /// <param name="commandTimeout">The command timeout.</param>
    /// <param name="commandType">Type of the command.</param>
    /// <returns>List of dynamic objects</returns>
    public static IEnumerable<dynamic> Query(this IDbConnection cnn, ISqlQuery sql, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
    {
        cnn.EnsureOpen();
        return Dapper.SqlMapper.Query(cnn, SqlHelper.FixCommandText(sql.ToString(), cnn.GetDialect()), sql.Params == null ? null : new DynamicParameters(sql.Params), transaction, buffered, commandTimeout, commandType);
    }

    /// <summary>
    /// Return a list of values, reader is closed after the call
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="cnn">Connection.</param>
    /// <param name="sql">SQL query.</param>
    /// <param name="transaction">The transaction.</param>
    /// <param name="buffered">if set to <c>true</c> results are buffered.</param>
    /// <param name="commandTimeout">The command timeout.</param>
    /// <param name="commandType">Type of the command.</param>
    /// <returns>List of values</returns>
    public static IEnumerable<TValue> Query<TValue>(this IDbConnection cnn, ISqlQuery sql, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
    {
        cnn.EnsureOpen();
        return Dapper.SqlMapper.Query<TValue>(cnn, SqlHelper.FixCommandText(sql.ToString(), cnn.GetDialect()), sql.Params == null ? null : new DynamicParameters(sql.Params), transaction, buffered, commandTimeout, commandType);
    }

    /// <summary>
    /// Return a list of dynamic objects, reader is closed after the call
    /// </summary>
    /// <param name="cnn">Connection.</param>
    /// <param name="sql">SQL query.</param>
    /// <param name="param">The parameters.</param>
    /// <param name="transaction">The transaction.</param>
    /// <returns>
    /// List of values
    /// </returns>
    public static IEnumerable<dynamic> Query(this IDbConnection cnn, string sql, object param, IDbTransaction transaction)
    {
        cnn.EnsureOpen();
        return Dapper.SqlMapper.Query(cnn, SqlHelper.FixCommandText(sql.ToString(), cnn.GetDialect()), param, transaction);
    }

    /// <summary>
    /// Return a list of dynamic objects, reader is closed after the call
    /// </summary>
    /// <param name="cnn">Connection.</param>
    /// <param name="sql">SQL query.</param>
    /// <param name="param">The parameters.</param>
    /// <param name="commandType">Type of the command.</param>
    /// <returns>
    /// List of dynamic objects
    /// </returns>
    public static IEnumerable<dynamic> Query(this IDbConnection cnn, string sql, object param, CommandType? commandType)
    {
        cnn.EnsureOpen();
        return Dapper.SqlMapper.Query(cnn, SqlHelper.FixCommandText(sql.ToString(), cnn.GetDialect()), param, null, true, null, commandType);
    }

    /// <summary>
    /// Return a list of dynamic objects, reader is closed after the call
    /// </summary>
    /// <param name="cnn">Connection.</param>
    /// <param name="sql">SQL query.</param>
    /// <param name="param">The parameters.</param>
    /// <param name="transaction">The transaction.</param>
    /// <param name="commandType">Type of the command.</param>
    /// <returns>
    /// List of dynamic objects
    /// </returns>
    public static IEnumerable<dynamic> Query(this IDbConnection cnn, string sql, object param, IDbTransaction transaction, CommandType? commandType)
    {
        cnn.EnsureOpen();
        return Dapper.SqlMapper.Query(cnn, SqlHelper.FixCommandText(sql.ToString(), cnn.GetDialect()), param, transaction, true, null, commandType);
    }

    /// <summary>
    /// Return a list of objects, reader is closed after the call
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cnn">Connection.</param>
    /// <param name="sql">SQL query.</param>
    /// <param name="param">The parameters.</param>
    /// <param name="transaction">The transaction.</param>
    /// <param name="buffered">if set to <c>true</c> results are buffered.</param>
    /// <param name="commandTimeout">The command timeout.</param>
    /// <param name="commandType">Type of the command.</param>
    /// <returns>
    /// List of objects
    /// </returns>
    public static IEnumerable<T> Query<T>(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
    {
        cnn.EnsureOpen();
        return Dapper.SqlMapper.Query<T>(cnn, SqlHelper.FixCommandText(sql.ToString(), cnn.GetDialect()), param, transaction, buffered, commandTimeout, commandType);
    }
}