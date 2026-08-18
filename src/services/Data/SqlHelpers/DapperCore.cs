using Dapper;

namespace Serenity.Data;

/// <summary>
/// Provides <see cref="IDbConnection"/> extension methods that wrap the corresponding Dapper
/// <see cref="Dapper.SqlMapper"/> methods, translating Serenity SQL (dialect specific brackets
/// and parameter prefixes) via <see cref="SqlConversions.Translate(string, IDbConnection)"/>
/// and ensuring the connection is open before execution.
/// It mirrors the string based <c>Execute</c> and <c>Query</c> extension methods of Dapper's
/// <c>SqlMapper</c>, not every Dapper overload (CommandDefinition based methods, ExecuteScalar,
/// ExecuteReader, QueryFirst, QueryMultiple, async variants, etc.). The <see cref="ISqlQuery"/>
/// overloads are Serenity specific and have no Dapper equivalent.
/// Note that unlike <see cref="SqlHelper"/> methods, these extension methods do not go through
/// <see cref="ISqlOperationInterceptor"/>.
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
        return Dapper.SqlMapper.Execute(cnn, SqlConversions.Translate(sql, cnn), param, transaction, commandTimeout, commandType);
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
        return Dapper.SqlMapper.Query(cnn, SqlConversions.Translate(sql, cnn), param, transaction, buffered, commandTimeout, commandType);
    }

    /// <summary>
    /// Return a list of dynamic objects, reader is closed after the call.
    /// Serenity specific overload for <see cref="ISqlQuery"/>, not present in Dapper.
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
        return Dapper.SqlMapper.Query(cnn, SqlConversions.Translate(sql, cnn), sql.Params == null ? null : new DynamicParameters(sql.Params), transaction, buffered, commandTimeout, commandType);
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
        return Dapper.SqlMapper.Query<T>(cnn, SqlConversions.Translate(sql, cnn), param, transaction, buffered, commandTimeout, commandType);
    }

    /// <summary>
    /// Return a list of values, reader is closed after the call.
    /// Serenity specific overload for <see cref="ISqlQuery"/>, not present in Dapper.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="cnn">Connection.</param>
    /// <param name="sql">SQL query.</param>
    /// <param name="transaction">The transaction.</param>
    /// <param name="buffered">if set to <c>true</c> results are buffered.</param>
    /// <param name="commandTimeout">The command timeout.</param>
    /// <param name="commandType">Type of the command.</param>
    /// <returns>List of values</returns>
    public static IEnumerable<T> Query<T>(this IDbConnection cnn, ISqlQuery sql, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
    {
        cnn.EnsureOpen();
        return Dapper.SqlMapper.Query<T>(cnn, SqlConversions.Translate(sql, cnn), sql.Params == null ? null : new DynamicParameters(sql.Params), transaction, buffered, commandTimeout, commandType);
    }

}