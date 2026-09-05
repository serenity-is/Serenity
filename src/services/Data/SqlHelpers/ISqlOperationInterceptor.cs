namespace Serenity.Data;

/// <summary>
/// An interface that makes it possible to intercept basic SQL operations on connections 
/// (e.g. SqlHelper extensions) mostly for testing purposes. Note that this does not
/// intercept all SQL operations, only the ones that are done through SqlHelper extensions.
/// It does not intercept Dapper operations, for example.
/// This interface should be implemented by the mock connection class used in tests.
/// </summary>
public interface ISqlOperationInterceptor
{
    /// <summary>
    /// Intercepts the <see cref="SqlHelper"/> <c>Execute</c> method (SqlDelete/SqlUpdate/SqlInsert).
    /// </summary>
    /// <param name="commandText">The command text.</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="expectedRows">The expected rows.</param>
    /// <param name="query">The query.</param>
    /// <param name="getNewId">True if <c>InsertAndGetID</c> is called.</param>
    OptionalValue<long?> ExecuteNonQuery(string commandText, IDictionary<string, object> parameters, ExpectedRows expectedRows, IQueryWithParams query, bool getNewId);

    /// <summary>
    /// Intercepts the <see cref="SqlHelper"/> <c>ExecuteReader</c> method.
    /// </summary>
    /// <param name="commandText">The command text.</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="query">The query.</param>
    OptionalValue<IDataReader> ExecuteReader(string commandText, IDictionary<string, object> parameters, SqlQuery query);

    /// <summary>
    /// Intercepts the <see cref="SqlHelper"/> <c>ExecuteScalar</c> method.
    /// </summary>
    /// <param name="commandText">The command text.</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="query">The query.</param>
    OptionalValue<object> ExecuteScalar(string commandText, IDictionary<string, object> parameters, SqlQuery query);

    /// <summary>
    /// Intercepts the async <see cref="SqlHelper"/> <c>Execute</c> methods (SqlDelete/SqlUpdate/SqlInsert).
    /// The default implementation forwards to <see cref="ExecuteNonQuery"/>.
    /// </summary>
    /// <param name="commandText">The command text.</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="expectedRows">The expected rows.</param>
    /// <param name="query">The query.</param>
    /// <param name="getNewId">True if <c>InsertAndGetIDAsync</c> is called.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<OptionalValue<long?>> ExecuteNonQueryAsync(string commandText, IDictionary<string, object> parameters, ExpectedRows expectedRows, IQueryWithParams query, bool getNewId, CancellationToken cancellationToken = default)
        => Task.FromResult(ExecuteNonQuery(commandText, parameters, expectedRows, query, getNewId));

    /// <summary>
    /// Intercepts the async <see cref="SqlHelper"/> <c>ExecuteReader</c> methods.
    /// The default implementation forwards to <see cref="ExecuteReader"/>.
    /// </summary>
    /// <param name="commandText">The command text.</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<OptionalValue<IDataReader>> ExecuteReaderAsync(string commandText, IDictionary<string, object> parameters, SqlQuery query, CancellationToken cancellationToken = default)
        => Task.FromResult(ExecuteReader(commandText, parameters, query));

    /// <summary>
    /// Intercepts the async <see cref="SqlHelper"/> <c>ExecuteScalar</c> methods.
    /// The default implementation forwards to <see cref="ExecuteScalar"/>.
    /// </summary>
    /// <param name="commandText">The command text.</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<OptionalValue<object>> ExecuteScalarAsync(string commandText, IDictionary<string, object> parameters, SqlQuery query, CancellationToken cancellationToken = default)
        => Task.FromResult(ExecuteScalar(commandText, parameters, query));
}
