using System.Collections;

namespace Serenity.Data;

/// <summary>
/// Arguments for intercepting entity list and count operations.
/// </summary>
/// <param name="RowType">The type of the row.</param>
/// <param name="Query">The fully configured query.</param>
public sealed record ListRowsArgs(Type RowType, SqlQuery Query)
{
    /// <summary>
    /// True when the query is for a count operation.
    /// </summary>
    public bool CountOnly { get; init; }
}

/// <summary>
/// Arguments for intercepting entity find operations.
/// </summary>
/// <param name="RowType">The type of the row.</param>
/// <param name="Id">The identifier, if the operation is by ID.</param>
/// <param name="Query">The fully configured query.</param>
/// <param name="ByIdOrSingle">True when a single row is expected.</param>
public sealed record FindRowArgs(Type RowType, OptionalValue<object?> Id, SqlQuery Query, bool ByIdOrSingle);

/// <summary>
/// An interface that allows you to intercept SQL operations on entities. Note that this does not
/// intercept all SQL operations, only the ones that are done through EntityConnectionExtensions.
/// This interface should be implemented by the mock connection class used in tests.
/// </summary>
public interface IRowOperationInterceptor
{
    /// <summary>
    /// Intercepts EntityConnectionExtensions's ById/TryById/First/TryFirst/Single/TrySingle methods.
    /// </summary>
    /// <param name="args">The find operation arguments.</param>
    /// <returns>Entity with the given ID, or null if not found.</returns>
    OptionalValue<IRow> FindRow(FindRowArgs args);

    /// <summary>
    /// Intercepts EntityConnectionExtensions.List and Count methods.
    /// </summary>
    /// <param name="args">The list operation arguments.</param>
    OptionalValue<IList> ListRows(ListRowsArgs args);

    /// <summary>
    /// Intercepts EntityConnectionExtensions.DeleteById method.
    /// </summary>
    /// <param name="rowType">Type of the row.</param>
    /// <param name="id">The identifier if Update/Delete is used.</param>
    /// <param name="row">The row being manipulated. Is null for delete.</param>
    /// <param name="expectedRows">The expected number of rows to be manipulated. Default is 1.</param>
    /// <param name="getNewId">True if InsertAndGetID is called.</param>
    /// <returns>The generated identity value, or null if none was generated.</returns>
    OptionalValue<long?> ManipulateRow(Type rowType, OptionalValue<object?> id, IRow? row, ExpectedRows expectedRows, bool getNewId);

    /// <summary>
    /// Intercepts the async EntityConnectionExtensions ById/TryById/First/TryFirst/Single/TrySingle methods.
    /// The default implementation forwards to <see cref="FindRow"/>.
    /// </summary>
    /// <param name="args">The find operation arguments.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Entity with the given ID, or null if not found.</returns>
    Task<OptionalValue<IRow>> FindRowAsync(FindRowArgs args, CancellationToken cancellationToken = default)
        => Task.FromResult(FindRow(args));

    /// <summary>
    /// Intercepts the async EntityConnectionExtensions List and Count methods.
    /// The default implementation forwards to <see cref="ListRows"/>.
    /// </summary>
    /// <param name="args">The list operation arguments.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<OptionalValue<IList>> ListRowsAsync(ListRowsArgs args, CancellationToken cancellationToken = default)
        => Task.FromResult(ListRows(args));

    /// <summary>
    /// Intercepts the async EntityConnectionExtensions DeleteById/Insert/Update methods.
    /// The default implementation forwards to <see cref="ManipulateRow"/>.
    /// </summary>
    /// <param name="rowType">Type of the row.</param>
    /// <param name="id">The identifier if Update/Delete is used.</param>
    /// <param name="row">The row being manipulated. Is null for delete.</param>
    /// <param name="expectedRows">The expected number of rows to be manipulated. Default is 1.</param>
    /// <param name="getNewId">True if InsertAndGetID is called.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The generated identity value, or null if none was generated.</returns>
    Task<OptionalValue<long?>> ManipulateRowAsync(Type rowType, OptionalValue<object?> id, IRow? row, ExpectedRows expectedRows, bool getNewId, CancellationToken cancellationToken = default)
        => Task.FromResult(ManipulateRow(rowType, id, row, expectedRows, getNewId));
}
