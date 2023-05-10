namespace Serenity.Data;

/// <summary>
/// Contains extension methods to perform entity CRUD operations directly on connections.
/// Please note that all these methods operate on a low level, and none of them call 
/// service behaviors or performs service validations.
/// </summary>
public static class EntityConnectionExtensions
{
    /// <summary>
    /// Finds an entity by its ID value. This method selects only the table fields,
    /// and no foreign / calculated fields. Use other overloads if you want to 
    /// select different set of fields.
    /// </summary>
    /// <typeparam name="TRow">The type of the row.</typeparam>
    /// <param name="connection">The connection.</param>
    /// <param name="id">The identifier.</param>
    /// <returns>Entity with given ID</returns>
    /// <exception cref="ValidationError">Record with specified ID is not found</exception>
    /// <exception cref="InvalidOperationException">Multiple records with the ID found</exception>
    public static TRow ById<TRow>(this IDbConnection connection, object id)
        where TRow : class, IRow, IIdRow, new()
    {
        var row = TryById<TRow>(connection, id);
        if (row == null)
            throw new ValidationError("RecordNotFound", string.Format("Can't locate '{0}' record with ID {1}!", new TRow().Table, id));

        return row;
    }

    /// <summary>
    /// Tries to finds an entity by its ID value. This method selects only the table fields,
    /// and no foreign / calculated fields. Use other overloads if you want to 
    /// select different set of fields.
    /// </summary>
    /// <typeparam name="TRow">The type of the row.</typeparam>
    /// <param name="connection">The connection.</param>
    /// <param name="id">The identifier.</param>
    /// <returns>Entity with given ID, or null if not found</returns>
    /// <exception cref="InvalidOperationException">Multiple records with the ID found</exception>
    public static TRow TryById<TRow>(this IDbConnection connection, object id)
        where TRow : class, IRow, IIdRow, new()
    {
        var row = new TRow() { TrackWithChecks = true };
        if (new SqlQuery().From(row)
                .SelectTableFields()
                .Where(new Criteria(row.IdField) == new ValueCriteria(id))
                .GetSingle(connection))
            return row;

        return null;
    }

    /// <summary>
    /// Finds an entity by its ID value. This method does not select any fields
    /// by default and allows you to edit the query to select fields you want.
    /// </summary>
    /// <typeparam name="TRow">The type of the row.</typeparam>
    /// <param name="connection">The connection.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="editQuery">Callback to edit the query.</param>
    /// <returns>Entity with given ID</returns>
    /// <exception cref="ValidationError">Record with specified ID is not found</exception>
    /// <exception cref="InvalidOperationException">Multiple records with the ID found</exception>
    public static TRow ById<TRow>(this IDbConnection connection, object id, Action<SqlQuery> editQuery)
        where TRow : class, IRow, IIdRow, new()
    {
        var row = TryById<TRow>(connection, id, editQuery);
        if (row == null)
            throw new ValidationError("RecordNotFound", string.Format("Can't locate '{0}' record with ID {1}!", new TRow().Table, id));

        return row;
    }

    /// <summary>
    /// Tries to find an entity by its ID value. This method does not select any fields
    /// by default and allows you to edit the query to select fields you want.
    /// </summary>
    /// <typeparam name="TRow">The type of the row.</typeparam>
    /// <param name="connection">The connection.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="editQuery">Callback to edit the query.</param>
    /// <returns>Entity with given ID, or null if not found</returns>
    /// <exception cref="InvalidOperationException">Multiple records with the ID found</exception> 
    public static TRow TryById<TRow>(this IDbConnection connection, object id, Action<SqlQuery> editQuery)
        where TRow : class, IRow, IIdRow, new()
    {
        var row = new TRow() { TrackWithChecks = true };
        var query = new SqlQuery().From(row)
            .Where(new Criteria(row.IdField) == new ValueCriteria(id));

        editQuery(query);

        if (query.GetSingle(connection))
            return row;

        return null;
    }

    /// <summary>
    /// Finds a single entity matching the specified criteria.
    /// This method selects only the table fields,
    /// and no foreign / calculated fields. Use other overloads if you want to 
    /// select different set of fields.
    /// </summary>
    /// <typeparam name="TRow">The type of the row.</typeparam>
    /// <param name="connection">The connection.</param>
    /// <param name="where">The where criteria.</param>
    /// <returns>The single entity matching the specified criteria</returns>
    /// <exception cref="ValidationError">No matching records found</exception>
    /// <exception cref="InvalidOperationException">Multiple records matching the specified criteria.</exception>
    public static TRow Single<TRow>(this IDbConnection connection, ICriteria where)
        where TRow : class, IRow, new()
    {
        var row = TrySingle<TRow>(connection, where);

        if (row == null)
            throw new ValidationError("RecordNotFound", "Query returned no results!");

        return row;
    }

    /// <summary>
    /// Tries to find a single entity matching the specified criteria. 
    /// This method selects only the table fields,
    /// and no foreign / calculated fields. Use other overloads if you want to 
    /// select different set of fields.
    /// </summary>
    /// <typeparam name="TRow">The type of the row.</typeparam>
    /// <param name="connection">The connection.</param>
    /// <param name="where">The where criteria.</param>
    /// <returns>The single entity matching the specified criteria, or null if no matching record found</returns>
    /// <exception cref="InvalidOperationException">Multiple records matching the criteria found</exception>
    public static TRow TrySingle<TRow>(this IDbConnection connection, ICriteria where)
        where TRow : class, IRow, new()
    {
        var row = new TRow() { TrackWithChecks = true };
        if (new SqlQuery().From(row)
                .SelectTableFields()
                .Where(where)
                .GetSingle(connection))
            return row;

        return null;
    }

    /// <summary>
    /// Finds a single entity, allowing caller to edit the criteria
    /// and set of fields to load through a editQuery callback.
    /// </summary>
    /// <typeparam name="TRow">The type of the row.</typeparam>
    /// <param name="connection">The connection.</param>
    /// <param name="editQuery">The callback to edit query.</param>
    /// <returns>Single entity matching the criteria set by editQuery.</returns>
    /// <exception cref="ValidationError">No records matching the specified criteria.</exception>
    /// <exception cref="InvalidOperationException">Multiple records matching the specified criteria.</exception>
    public static TRow Single<TRow>(this IDbConnection connection, Action<SqlQuery> editQuery)
        where TRow : class, IRow, new()
    {
        var row = TrySingle<TRow>(connection, editQuery);

        if (row == null)
            throw new ValidationError("RecordNotFound", "Query returned no results!");

        return row;
    }

    /// <summary>
    /// Tries to find a single entity, allowing caller to edit the criteria
    /// and set of fields to load through a editQuery callback.
    /// </summary>
    /// <typeparam name="TRow">The type of the row.</typeparam>
    /// <param name="connection">The connection.</param>
    /// <param name="editQuery">The edit query.</param>
    /// <returns>Single entity matching the criteria set by editQuery, or null if not found.</returns>
    /// <exception cref="InvalidOperationException">Multiple records matching the specified criteria.</exception>
    public static TRow TrySingle<TRow>(this IDbConnection connection, Action<SqlQuery> editQuery)
        where TRow : class, IRow, new()
    {
        var row = new TRow() { TrackWithChecks = true };
        var query = new SqlQuery().From(row);

        editQuery(query);

        if (query.GetSingle(connection))
            return row;

        return null;
    }

    /// <summary>
    /// Finds first entity matching a where criteria.
    /// </summary>
    /// <typeparam name="TRow">The type of the row.</typeparam>
    /// <param name="connection">The connection.</param>
    /// <param name="where">The where criteria.</param>
    /// <returns>First entity matching the where criteria.</returns>
    /// <exception cref="ValidationError">No records matching the specified criteria.</exception>
    public static TRow First<TRow>(this IDbConnection connection, ICriteria where)
        where TRow : class, IRow, new()
    {
        var row = TryFirst<TRow>(connection, where);

        if (row == null)
            throw new ValidationError("RecordNotFound", "Query returned no results!");

        return row;
    }

    /// <summary>
    /// Tries to find first entity matching a where criteria.
    /// </summary>
    /// <typeparam name="TRow">The type of the row.</typeparam>
    /// <param name="connection">The connection.</param>
    /// <param name="where">The where criteria.</param>
    /// <returns>First entity matching the where criteria or null if not found.</returns>
    public static TRow TryFirst<TRow>(this IDbConnection connection, ICriteria where)
        where TRow : class, IRow, new()
    {
        var row = new TRow() { TrackWithChecks = true };
        if (new SqlQuery().From(row)
                .SelectTableFields()
                .Where(where)
                .GetFirst(connection))
            return row;

        return null;
    }

    /// <summary>
    /// Finds first entity, allowing the caller to set criteria and fields to select 
    /// through an editQuery callback.
    /// </summary>
    /// <typeparam name="TRow">The type of the row.</typeparam>
    /// <param name="connection">The connection.</param>
    /// <param name="editQuery">The edit query callback.</param>
    /// <returns>First entity matching the criteria.</returns>
    /// <exception cref="ValidationError">No records matching the specified criteria.</exception>
    public static TRow First<TRow>(this IDbConnection connection, Action<SqlQuery> editQuery)
        where TRow : class, IRow, new()
    {
        var row = TryFirst<TRow>(connection, editQuery);

        if (row == null)
            throw new ValidationError("RecordNotFound", "Query returned no results!");

        return row;
    }

    /// <summary>
    /// Tries to finds first entity, allowing the caller to set criteria and fields to select 
    /// through an editQuery callback.
    /// </summary>
    /// <typeparam name="TRow">The type of the row.</typeparam>
    /// <param name="connection">The connection.</param>
    /// <param name="editQuery">The edit query callback.</param>
    /// <returns>First entity matching the criteria, or null if not found.</returns>
    public static TRow TryFirst<TRow>(this IDbConnection connection, Action<SqlQuery> editQuery)
        where TRow : class, IRow, new()
    {
        var row = new TRow() { TrackWithChecks = true };
        var query = new SqlQuery().From(row);

        editQuery(query);

        if (query.GetFirst(connection))
            return row;

        return null;
    }

    /// <summary>
    /// Gets count of all records.
    /// </summary>
    /// <typeparam name="TRow">The type of the row.</typeparam>
    /// <param name="connection">The connection.</param>
    /// <returns>Number of records in the table</returns>
    public static int Count<TRow>(this IDbConnection connection)
        where TRow : class, IRow, new()
    {
        return connection.Count<TRow>(null);
    }

    /// <summary>
    /// Gets count of records matching a specified criteria.
    /// </summary>
    /// <typeparam name="TRow">The type of the row.</typeparam>
    /// <param name="connection">The connection.</param>
    /// <param name="where">The where criteria.</param>
    /// <returns>Number of records matching the specified criteria</returns>
    public static int Count<TRow>(this IDbConnection connection, ICriteria where)
        where TRow : class, IRow, new()
    {
        var row = new TRow() { TrackWithChecks = true };

        return Convert.ToInt32(SqlHelper.ExecuteScalar(connection,
            new SqlQuery().From(row)
                .Select(Sql.Count())
                .Where(where)));
    }

    /// <summary>
    /// Checks if the record with specified ID exists.
    /// </summary>
    /// <typeparam name="TRow">The type of the row.</typeparam>
    /// <param name="connection">The connection.</param>
    /// <param name="id">The identifier.</param>
    /// <returns>True if record exists</returns>
    public static bool ExistsById<TRow>(this IDbConnection connection, object id)
        where TRow : class, IRow, IIdRow, new()
    {
        var row = new TRow();
        return new SqlQuery()
                .From(row)
                .Select("1")
                .Where(new Criteria(row.IdField) == new ValueCriteria(id))
                .Exists(connection);
    }

    /// <summary>
    /// Checks if record matching specified criteria exists.
    /// </summary>
    /// <typeparam name="TRow">The type of the row.</typeparam>
    /// <param name="connection">The connection.</param>
    /// <param name="where">The where criteria.</param>
    /// <returns>True if record matching criteria exists.</returns>
    public static bool Exists<TRow>(this IDbConnection connection, ICriteria where)
        where TRow : class, IRow, new()
    {
        var row = new TRow() { TrackWithChecks = true };
        return new SqlQuery().From(row)
                .Select("1")
                .Where(where)
                .Exists(connection);
    }

    /// <summary>
    /// Lists all records. This method selects only the table fields,
    /// and no foreign / calculated fields. Use other overloads if you want to 
    /// select different set of fields.
    /// </summary>
    /// <typeparam name="TRow">The type of the row.</typeparam>
    /// <param name="connection">The connection.</param>
    /// <returns>All records</returns>
    public static List<TRow> List<TRow>(this IDbConnection connection)
        where TRow : class, IRow, new()
    {
        return connection.List<TRow>((Criteria)null);
    }

    /// <summary>
    /// Lists the records matching specified where criteria.
    /// This method selects only the table fields, and no foreign / calculated fields. 
    /// Use other overloads if you want to select different set of fields.
    /// </summary>
    /// <typeparam name="TRow">The type of the row.</typeparam>
    /// <param name="connection">The connection.</param>
    /// <param name="where">The where criteria.</param>
    /// <returns>Records matching the specified criteria</returns>
    public static List<TRow> List<TRow>(this IDbConnection connection, ICriteria where)
        where TRow : class, IRow, new()
    {
        var row = new TRow() { TrackWithChecks = true };
        return new SqlQuery().From(row)
                .SelectTableFields()
                .Where(where)
                .List(connection, row);
    }

    /// <summary>
    /// Lists the records, allowing the caller to specify criteria and 
    /// set of fields to select through an editQuery callback.
    /// </summary>
    /// <typeparam name="TRow">The type of the row.</typeparam>
    /// <param name="connection">The connection.</param>
    /// <param name="editQuery">The edit query callback.</param>
    /// <returns>List of records matching the edited query.</returns>
    public static List<TRow> List<TRow>(this IDbConnection connection, Action<SqlQuery> editQuery)
        where TRow : class, IRow, new()
    {
        var row = new TRow() { TrackWithChecks = true };
        var query = new SqlQuery().From(row);

        editQuery(query);

        return query.List(connection, row);
    }

    /// <summary>
    /// Inserts the specified entity. Note that this operates at a low level,
    /// it does not perform any validation or permission check, and does not call service behaviors / handlers.
    /// </summary>
    /// <typeparam name="TRow">The type of the row.</typeparam>
    /// <param name="connection">The connection.</param>
    /// <param name="row">The row.</param>
    public static void Insert<TRow>(this IDbConnection connection, TRow row)
        where TRow : class, IRow
    {
        ToSqlInsert(row).Execute(connection);
    }

    /// <summary>
    /// Inserts the specified entity and returns the ID of record inserted.
    /// Only works for identity columns of integer type. Note that this operates at a low level,
    /// it does not perform any validation or permission check and does not call service behaviors / handlers.
    /// </summary>
    /// <typeparam name="TRow">The type of the row.</typeparam>
    /// <param name="connection">The connection.</param>
    /// <param name="row">The row.</param>
    /// <returns>The ID of the record inserted.</returns>
    public static long? InsertAndGetID<TRow>(this IDbConnection connection, TRow row)
        where TRow : IRow
    {
        return ToSqlInsert(row).ExecuteAndGetID(connection);
    }

    /// <summary>
    /// Updates the entity by its identifier. Note that this operates at a low level,
    /// it does not perform any validation or permission check and does not call service behaviors / handlers.
    /// </summary>
    /// <typeparam name="TRow">The type of the row.</typeparam>
    /// <param name="connection">The connection.</param>
    /// <param name="row">The row.</param>
    /// <param name="expectedRows">The expected number of rows to be updated, by default 1.</param>
    /// <exception cref="InvalidOperationException">ID field of row has null value!</exception>
    /// <exception cref="InvalidOperationException">Expected rows and number of updated rows does not match!</exception>
    public static void UpdateById<TRow>(this IDbConnection connection, TRow row, ExpectedRows expectedRows = ExpectedRows.One)
        where TRow : IIdRow
    {
        var idField = row.IdField;
        var r = (IRow)(object)row;

        if (idField.IsNull(r))
            throw new InvalidOperationException("ID field of row has null value!");

        row.ToSqlUpdateById()
            .Execute(connection, expectedRows);
    }

    /// <summary>
    /// Deletes the entity by its identifier. Note that this operates at a low level,
    /// it does not perform any validation or permission check and does not call service behaviors / handlers.
    /// </summary>
    /// <typeparam name="TRow">The type of the row.</typeparam>
    /// <param name="connection">The connection.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="expectedRows">The expected number of rows to be deleted, 1 by default.</param>
    /// <exception cref="InvalidOperationException">Expected rows and number of deleted rows does not match!</exception>
    /// <returns>Number of deleted rows</returns>
    public static int DeleteById<TRow>(this IDbConnection connection, object id, ExpectedRows expectedRows = ExpectedRows.One)
        where TRow : class, IRow, IIdRow, new()
    {
        var row = new TRow();
        return new SqlDelete(row.Table)
            .Where(row.IdField == new ValueCriteria(id))
            .Execute(connection, expectedRows);
    }

    /// <summary>
    /// Converts the entity to an SqlInsert object by setting only 
    /// the assigned fields.
    /// </summary>
    /// <param name="row">The row with field values to set in new record (must be in TrackAssignments mode).</param>
    /// <returns>SqlInsert object</returns>
    /// <exception cref="ArgumentNullException">row is null</exception>
    public static SqlInsert ToSqlInsert(this IRow row)
    {
        if (row == null)
            throw new ArgumentNullException("row");

        var insert = new SqlInsert(row.Table);

        insert.Set(row);

        if (row as IIdRow != null)
            insert.IdentityColumn(row.IdField.Name);

        return insert;
    }

    /// <summary>
    /// Converts the entity to an SqlUpdate object by ID setting only 
    /// the assigned fields.
    /// </summary>
    /// <param name="row">The row with field values to set in new record (must be in TrackAssignments mode).</param>
    /// <returns>SqlUpdate object</returns>
    /// <exception cref="ArgumentNullException">row is null</exception>
    public static SqlUpdate ToSqlUpdateById(this IIdRow row)
    {
        if (row == null)
            throw new ArgumentNullException("row");

        var update = new SqlUpdate(row.Table);

        var idField = row.IdField;
        update.Set(row, idField);
        update.Where(idField == new ValueCriteria(idField.AsSqlValue(row)));

        return update;
    }
}