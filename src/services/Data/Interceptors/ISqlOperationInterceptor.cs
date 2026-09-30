namespace Serenity.Data;

/// <summary>
/// An interface that makes it possible to intercept basic SQL operations on connections
/// (e.g. SqlHelper extensions) mostly for testing purposes. Note that this does not
/// intercept all SQL operations, only the ones that are done through SqlHelper extensions.
/// It does not intercept Dapper operations, for example.
/// This interface should be implemented by the mock connection class used in tests.
/// </summary>
/// <remarks>
/// Interception happens before command creation, so operation arguments carry the
/// pre-translation SQL text (e.g. [brackets] and @ parameters, not dialect quotes).
/// This is intentional: it keeps test assertions dialect-independent.
/// Only the connection itself is checked for this interface; there is no chaining.
/// </remarks>
public interface ISqlOperationInterceptor
{
    /// <summary>
    /// Intercepts the <see cref="SqlHelper"/> <c>Execute</c> method (SqlDelete/SqlUpdate/SqlInsert).
    /// </summary>
    /// <param name="args">The operation arguments.</param>
    OptionalValue<long?> ExecuteNonQuery(InterceptExecuteNonQueryArgs args);

    /// <summary>
    /// Intercepts the <see cref="SqlHelper"/> <c>ExecuteReader</c> method.
    /// </summary>
    /// <param name="args">The operation arguments.</param>
    OptionalValue<IDataReader> ExecuteReader(InterceptExecuteReaderArgs args);

    /// <summary>
    /// Intercepts the <see cref="SqlHelper"/> <c>ExecuteScalar</c> method.
    /// </summary>
    /// <param name="args">The operation arguments.</param>
    OptionalValue<object> ExecuteScalar(InterceptExecuteScalarArgs args);

    /// <summary>
    /// Intercepts the async <see cref="SqlHelper"/> <c>Execute</c> methods (SqlDelete/SqlUpdate/SqlInsert).
    /// The default implementation forwards to <see cref="ExecuteNonQuery"/>.
    /// </summary>
    /// <param name="args">The operation arguments.</param>
    Task<OptionalValue<long?>> ExecuteNonQueryAsync(InterceptExecuteNonQueryArgs args)
        => Task.FromResult(ExecuteNonQuery(args with { IsAsync = true }));

    /// <summary>
    /// Intercepts the async <see cref="SqlHelper"/> <c>ExecuteReader</c> methods.
    /// The default implementation forwards to <see cref="ExecuteReader"/>.
    /// </summary>
    /// <param name="args">The operation arguments.</param>
    Task<OptionalValue<IDataReader>> ExecuteReaderAsync(InterceptExecuteReaderArgs args)
        => Task.FromResult(ExecuteReader(args with { IsAsync = true }));

    /// <summary>
    /// Intercepts the async <see cref="SqlHelper"/> <c>ExecuteScalar</c> methods.
    /// The default implementation forwards to <see cref="ExecuteScalar"/>.
    /// </summary>
    /// <param name="args">The operation arguments.</param>
    Task<OptionalValue<object>> ExecuteScalarAsync(InterceptExecuteScalarArgs args)
        => Task.FromResult(ExecuteScalar(args with { IsAsync = true }));
}
