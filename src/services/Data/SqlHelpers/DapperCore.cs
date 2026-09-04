using Dapper;

namespace Serenity.Data;

/// <summary>
/// Provides <see cref="IDbConnection"/> extension methods that wrap the corresponding Dapper
/// <see cref="Dapper.SqlMapper"/> methods, translating Serenity SQL (dialect specific brackets
/// and parameter prefixes) via <see cref="SqlConversions.Translate(string, IDbConnection)"/>
/// and ensuring the connection is open before execution.
/// It mirrors the string based <c>Execute</c> and <c>Query</c> extension methods of Dapper's
/// <c>SqlMapper</c>, not every Dapper overload (CommandDefinition based methods, ExecuteScalar,
/// ExecuteReader, QueryFirst, QueryMultiple, etc.), as well as their async variants. The
/// <see cref="ISqlQuery"/> overloads are Serenity specific and have no Dapper equivalent.
/// Note that unlike <see cref="SqlHelper"/> methods, these extension methods do not go through
/// <see cref="ISqlOperationInterceptor"/>.
/// </summary>
public static partial class SqlMapper
{
    /// <summary>
    /// Executes a parameterized SQL statement.
    /// </summary>
    /// <param name="cnn">The connection.</param>
    /// <param name="sql">The SQL query.</param>
    /// <param name="param">The parameters.</param>
    /// <param name="transaction">The transaction.</param>
    /// <param name="commandTimeout">The command timeout.</param>
    /// <param name="commandType">Type of the command.</param>
    /// <returns>
    /// The number of rows affected.
    /// </returns>
    public static int Execute(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        cnn.EnsureOpen();
        return Dapper.SqlMapper.Execute(cnn, SqlConversions.Translate(sql, cnn), param, transaction, commandTimeout, commandType);
    }

    /// <summary>
    /// Returns a list of dynamic objects; the reader is closed after the call.
    /// </summary>
    /// <param name="cnn">The connection.</param>
    /// <param name="sql">The SQL query.</param>
    /// <param name="param">The parameters.</param>
    /// <param name="transaction">The transaction.</param>
    /// <param name="buffered">If set to <c>true</c>, results are buffered.</param>
    /// <param name="commandTimeout">The command timeout.</param>
    /// <param name="commandType">Type of the command.</param>
    /// <returns>List of dynamic objects.</returns>
    public static IEnumerable<dynamic> Query(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
    {
        cnn.EnsureOpen();
        return Dapper.SqlMapper.Query(cnn, SqlConversions.Translate(sql, cnn), param, transaction, buffered, commandTimeout, commandType);
    }

    /// <summary>
    /// Returns a list of dynamic objects; the reader is closed after the call.
    /// Serenity specific overload for <see cref="ISqlQuery"/>, not present in Dapper.
    /// </summary>
    /// <param name="cnn">The connection.</param>
    /// <param name="sql">The SQL query.</param>
    /// <param name="transaction">The transaction.</param>
    /// <param name="buffered">If set to <c>true</c>, results are buffered.</param>
    /// <param name="commandTimeout">The command timeout.</param>
    /// <param name="commandType">Type of the command.</param>
    /// <returns>List of dynamic objects.</returns>
    public static IEnumerable<dynamic> Query(this IDbConnection cnn, ISqlQuery sql, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
    {
        cnn.EnsureOpen();
        return Dapper.SqlMapper.Query(cnn, SqlConversions.Translate(sql, cnn), sql.Params == null ? null : new DynamicParameters(sql.Params), transaction, buffered, commandTimeout, commandType);
    }

    /// <summary>
    /// Returns a list of objects; the reader is closed after the call.
    /// </summary>
    /// <typeparam name="T">The type of the objects to return.</typeparam>
    /// <param name="cnn">The connection.</param>
    /// <param name="sql">The SQL query.</param>
    /// <param name="param">The parameters.</param>
    /// <param name="transaction">The transaction.</param>
    /// <param name="buffered">If set to <c>true</c>, results are buffered.</param>
    /// <param name="commandTimeout">The command timeout.</param>
    /// <param name="commandType">Type of the command.</param>
    /// <returns>
    /// List of objects.
    /// </returns>
    public static IEnumerable<T> Query<T>(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
    {
        cnn.EnsureOpen();
        return Dapper.SqlMapper.Query<T>(cnn, SqlConversions.Translate(sql, cnn), param, transaction, buffered, commandTimeout, commandType);
    }

    /// <summary>
    /// Returns a list of values; the reader is closed after the call.
    /// Serenity specific overload for <see cref="ISqlQuery"/>, not present in Dapper.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="cnn">The connection.</param>
    /// <param name="sql">The SQL query.</param>
    /// <param name="transaction">The transaction.</param>
    /// <param name="buffered">If set to <c>true</c>, results are buffered.</param>
    /// <param name="commandTimeout">The command timeout.</param>
    /// <param name="commandType">Type of the command.</param>
    /// <returns>List of values.</returns>
    public static IEnumerable<T> Query<T>(this IDbConnection cnn, ISqlQuery sql, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
    {
        cnn.EnsureOpen();
        return Dapper.SqlMapper.Query<T>(cnn, SqlConversions.Translate(sql, cnn), sql.Params == null ? null : new DynamicParameters(sql.Params), transaction, buffered, commandTimeout, commandType);
    }

    /// <summary>
    /// Executes a parameterized SQL statement asynchronously.
    /// </summary>
    /// <param name="cnn">The connection.</param>
    /// <param name="sql">The SQL query.</param>
    /// <param name="param">The parameters.</param>
    /// <param name="transaction">The transaction.</param>
    /// <param name="commandTimeout">The command timeout.</param>
    /// <param name="commandType">Type of the command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the number of rows affected.
    /// </returns>
    public static async Task<int> ExecuteAsync(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
    {
        await cnn.EnsureOpenAsync(cancellationToken).ConfigureAwait(false);
        return await Dapper.SqlMapper.ExecuteAsync(cnn, new CommandDefinition(SqlConversions.Translate(sql, cnn), param, transaction, commandTimeout, commandType, CommandFlags.Buffered, cancellationToken)).ConfigureAwait(false);
    }

    /// <summary>
    /// Returns a list of dynamic objects asynchronously; the reader is closed after the call.
    /// </summary>
    /// <param name="cnn">The connection.</param>
    /// <param name="sql">The SQL query.</param>
    /// <param name="param">The parameters.</param>
    /// <param name="transaction">The transaction.</param>
    /// <param name="commandTimeout">The command timeout.</param>
    /// <param name="commandType">Type of the command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of dynamic objects.</returns>
    public static async Task<IEnumerable<dynamic>> QueryAsync(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
    {
        await cnn.EnsureOpenAsync(cancellationToken).ConfigureAwait(false);
        return await Dapper.SqlMapper.QueryAsync(cnn, new CommandDefinition(SqlConversions.Translate(sql, cnn), param, transaction, commandTimeout, commandType, CommandFlags.Buffered, cancellationToken)).ConfigureAwait(false);
    }

    /// <summary>
    /// Returns a list of dynamic objects asynchronously; the reader is closed after the call.
    /// Serenity specific overload for <see cref="ISqlQuery"/>, not present in Dapper.
    /// </summary>
    /// <param name="cnn">The connection.</param>
    /// <param name="sql">The SQL query.</param>
    /// <param name="transaction">The transaction.</param>
    /// <param name="commandTimeout">The command timeout.</param>
    /// <param name="commandType">Type of the command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of dynamic objects.</returns>
    public static async Task<IEnumerable<dynamic>> QueryAsync(this IDbConnection cnn, ISqlQuery sql, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
    {
        await cnn.EnsureOpenAsync(cancellationToken).ConfigureAwait(false);
        return await Dapper.SqlMapper.QueryAsync(cnn, new CommandDefinition(SqlConversions.Translate(sql, cnn), sql.Params == null ? null : new DynamicParameters(sql.Params), transaction, commandTimeout, commandType, CommandFlags.Buffered, cancellationToken)).ConfigureAwait(false);
    }

    /// <summary>
    /// Returns a list of objects asynchronously; the reader is closed after the call.
    /// </summary>
    /// <typeparam name="T">The type of the objects to return.</typeparam>
    /// <param name="cnn">The connection.</param>
    /// <param name="sql">The SQL query.</param>
    /// <param name="param">The parameters.</param>
    /// <param name="transaction">The transaction.</param>
    /// <param name="commandTimeout">The command timeout.</param>
    /// <param name="commandType">Type of the command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a list of objects.
    /// </returns>
    public static async Task<IEnumerable<T>> QueryAsync<T>(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
    {
        await cnn.EnsureOpenAsync(cancellationToken).ConfigureAwait(false);
        return await Dapper.SqlMapper.QueryAsync<T>(cnn, new CommandDefinition(SqlConversions.Translate(sql, cnn), param, transaction, commandTimeout, commandType, CommandFlags.Buffered, cancellationToken)).ConfigureAwait(false);
    }

    /// <summary>
    /// Returns a list of values asynchronously; the reader is closed after the call.
    /// Serenity specific overload for <see cref="ISqlQuery"/>, not present in Dapper.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="cnn">The connection.</param>
    /// <param name="sql">The SQL query.</param>
    /// <param name="transaction">The transaction.</param>
    /// <param name="commandTimeout">The command timeout.</param>
    /// <param name="commandType">Type of the command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of values.</returns>
    public static async Task<IEnumerable<T>> QueryAsync<T>(this IDbConnection cnn, ISqlQuery sql, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
    {
        await cnn.EnsureOpenAsync(cancellationToken).ConfigureAwait(false);
        return await Dapper.SqlMapper.QueryAsync<T>(cnn, new CommandDefinition(SqlConversions.Translate(sql, cnn), sql.Params == null ? null : new DynamicParameters(sql.Params), transaction, commandTimeout, commandType, CommandFlags.Buffered, cancellationToken)).ConfigureAwait(false);
    }

}