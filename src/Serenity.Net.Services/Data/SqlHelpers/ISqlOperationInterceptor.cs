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
    /// Intercepts SqlHelper.Execute(SqlDelete/SqlUpdate/SqlInsert) method.
    /// <param name="commandText">Command text</param>
    /// <param name="parameters">Parameters</param>
    /// <param name="query">The query.</param>
    /// <param name="expectedRows">Expected rows</param>
    /// <param name="getNewId">True if InsertAndGetID is called</param>
    /// </summary>
    OptionalValue<long?> ExecuteNonQuery(string commandText, IDictionary<string, object> parameters, ExpectedRows expectedRows, IQueryWithParams query, bool getNewId);

    /// <summary>
    /// Intercepts SqlHelper.ExecuteReader method.
    /// </summary>
    /// <param name="commandText">Command text</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="query">The query</param>
    OptionalValue<IDataReader> ExecuteReader(string commandText, IDictionary<string, object> parameters, SqlQuery query);

    /// <summary>
    /// Intercepts SqlHelper.ExecuteReader method.
    /// </summary>
    /// <param name="commandText">Command text</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="query">The query</param>
    OptionalValue<object> ExecuteScalar(string commandText, IDictionary<string, object> parameters, SqlQuery query);
}