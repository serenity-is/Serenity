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
    /// <param name="rowType">Type of the row</param>
    /// <param name="id">The identifier if one of ById methods are used.</param>
    /// <param name="where">The where criteria for First/TryFirst/Single/TrySingle methods.</param>
    /// <param name="editQuery">Callback to edit the query.</param>
    /// <param name="byIdOrSingle">True if one of ById/TryById/Single/TrySingle methods is used</param>
    /// <returns>Entity with given ID, or null if not found</returns>
    OptionalValue<IRow> FindRow(Type rowType, OptionalValue<object> id, ICriteria where, Action<SqlQuery> editQuery, bool byIdOrSingle);

    /// <summary>
    /// Intercepts EntityConnectionExtensions.List and Count methods.
    /// </summary>
    /// <param name="rowType">Type of the row</param>
    /// <param name="where">The where criteria.</param>
    /// <param name="editQuery">The edit query callback.</param>
    /// <param name="countOnly">True if intercepting Count method.</param>
    OptionalValue<IList<IRow>> ListRows(Type rowType, ICriteria where, Action<SqlQuery> editQuery, bool countOnly);

    /// <summary>
    /// Intercepts EntityConnectionExtensions.DeleteById method.
    /// </summary>
    /// <param name="rowType">Type of the row</param>
    /// <param name="id">The identifier if Update/Delete is used.</param>
    /// <param name="row">Row being manipulated. Is null for delete.</param>
    /// <param name="expectedRows">The expected number of rows to be manipulated. Default is 1.</param>
    /// <param name="getNewId">True if InsertAndGetID is called</param>
    /// <returns></returns>
    OptionalValue<long?> ManipulateRow(Type rowType, OptionalValue<object> id, IRow row, ExpectedRows expectedRows, bool getNewId);
}