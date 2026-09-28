using System.Collections;

namespace Serenity.Data;

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
    OptionalValue<IRow> FindRow(InterceptFindRowArgs args);

    /// <summary>
    /// Intercepts EntityConnectionExtensions.List and Count methods.
    /// </summary>
    /// <param name="args">The list operation arguments.</param>
    OptionalValue<IList> ListRows(InterceptListRowsArgs args);

    /// <summary>
    /// Intercepts EntityConnectionExtensions.DeleteById method.
    /// </summary>
    /// <param name="args">The row manipulation arguments.</param>
    /// <returns>The generated identity value, or null if none was generated.</returns>
    OptionalValue<long?> ManipulateRow(InterceptManipulateRowArgs args);

    /// <summary>
    /// Intercepts the async EntityConnectionExtensions ById/TryById/First/TryFirst/Single/TrySingle methods.
    /// The default implementation forwards to <see cref="FindRow"/>.
    /// </summary>
    /// <param name="args">The find operation arguments.</param>
    /// <returns>Entity with the given ID, or null if not found.</returns>
    Task<OptionalValue<IRow>> FindRowAsync(InterceptFindRowArgs args)
        => Task.FromResult(FindRow(args with { IsAsync = true }));

    /// <summary>
    /// Intercepts the async EntityConnectionExtensions List and Count methods.
    /// The default implementation forwards to <see cref="ListRows"/>.
    /// </summary>
    /// <param name="args">The list operation arguments.</param>
    Task<OptionalValue<IList>> ListRowsAsync(InterceptListRowsArgs args)
        => Task.FromResult(ListRows(args with { IsAsync = true }));

    /// <summary>
    /// Intercepts the async EntityConnectionExtensions DeleteById/Insert/Update methods.
    /// The default implementation forwards to <see cref="ManipulateRow"/>.
    /// </summary>
    /// <param name="args">The row manipulation arguments.</param>
    /// <returns>The generated identity value, or null if none was generated.</returns>
    Task<OptionalValue<long?>> ManipulateRowAsync(InterceptManipulateRowArgs args)
        => Task.FromResult(ManipulateRow(args with { IsAsync = true }));
}
