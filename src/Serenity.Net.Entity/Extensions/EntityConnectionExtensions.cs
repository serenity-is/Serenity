using Serenity.Services;
using System;
using System.Collections.Generic;
using System.Data;

namespace Serenity.Data
{
    /// <summary>
    /// EntityConnectionExtensions
    /// </summary>
    public static class EntityConnectionExtensions
    {
        /// <summary>
        /// Bies the identifier.
        /// </summary>
        /// <typeparam name="TRow">The type of the row.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <exception cref="ValidationError">RecordNotFound</exception>
        /// <exception cref="TRow"></exception>
        public static TRow ById<TRow>(this IDbConnection connection, object id)
            where TRow : class, IRow, IIdRow, new()
        {
            var row = TryById<TRow>(connection, id);
            if (row == null)
                throw new ValidationError("RecordNotFound", string.Format("Can't locate '{0}' record with ID {1}!", new TRow().Table, id));

            return row;
        }

        /// <summary>
        /// Tries the by identifier.
        /// </summary>
        /// <typeparam name="TRow">The type of the row.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
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
        /// Bies the identifier.
        /// </summary>
        /// <typeparam name="TRow">The type of the row.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="editQuery">The edit query.</param>
        /// <returns></returns>
        /// <exception cref="ValidationError">RecordNotFound</exception>
        /// <exception cref="TRow"></exception>
        public static TRow ById<TRow>(this IDbConnection connection, object id, Action<SqlQuery> editQuery)
            where TRow : class, IRow, IIdRow, new()
        {
            var row = TryById<TRow>(connection, id, editQuery);
            if (row == null)
                throw new ValidationError("RecordNotFound", string.Format("Can't locate '{0}' record with ID {1}!", new TRow().Table, id));

            return row;
        }

        /// <summary>
        /// Tries the by identifier.
        /// </summary>
        /// <typeparam name="TRow">The type of the row.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="editQuery">The edit query.</param>
        /// <returns></returns>
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
        /// Singles the specified where.
        /// </summary>
        /// <typeparam name="TRow">The type of the row.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="where">The where.</param>
        /// <returns></returns>
        /// <exception cref="ValidationError">RecordNotFound - Query returned no results!</exception>
        public static TRow Single<TRow>(this IDbConnection connection, ICriteria where)
            where TRow : class, IRow, new()
        {
            var row = TrySingle<TRow>(connection, where);

            if (row == null)
                throw new ValidationError("RecordNotFound", "Query returned no results!");

            return row;
        }

        /// <summary>
        /// Tries the single.
        /// </summary>
        /// <typeparam name="TRow">The type of the row.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="where">The where.</param>
        /// <returns></returns>
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
        /// Singles the specified edit query.
        /// </summary>
        /// <typeparam name="TRow">The type of the row.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="editQuery">The edit query.</param>
        /// <returns></returns>
        /// <exception cref="ValidationError">RecordNotFound - Query returned no results!</exception>
        public static TRow Single<TRow>(this IDbConnection connection, Action<SqlQuery> editQuery)
            where TRow : class, IRow, new()
        {
            var row = TrySingle<TRow>(connection, editQuery);

            if (row == null)
                throw new ValidationError("RecordNotFound", "Query returned no results!");

            return row;
        }

        /// <summary>
        /// Tries the single.
        /// </summary>
        /// <typeparam name="TRow">The type of the row.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="editQuery">The edit query.</param>
        /// <returns></returns>
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
        /// Firsts the specified where.
        /// </summary>
        /// <typeparam name="TRow">The type of the row.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="where">The where.</param>
        /// <returns></returns>
        /// <exception cref="ValidationError">RecordNotFound - Query returned no results!</exception>
        public static TRow First<TRow>(this IDbConnection connection, ICriteria where)
            where TRow : class, IRow, new()
        {
            var row = TryFirst<TRow>(connection, where);

            if (row == null)
                throw new ValidationError("RecordNotFound", "Query returned no results!");

            return row;
        }

        /// <summary>
        /// Tries the first.
        /// </summary>
        /// <typeparam name="TRow">The type of the row.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="where">The where.</param>
        /// <returns></returns>
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
        /// Firsts the specified edit query.
        /// </summary>
        /// <typeparam name="TRow">The type of the row.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="editQuery">The edit query.</param>
        /// <returns></returns>
        /// <exception cref="ValidationError">RecordNotFound - Query returned no results!</exception>
        public static TRow First<TRow>(this IDbConnection connection, Action<SqlQuery> editQuery)
            where TRow : class, IRow, new()
        {
            var row = TryFirst<TRow>(connection, editQuery);

            if (row == null)
                throw new ValidationError("RecordNotFound", "Query returned no results!");

            return row;
        }

        /// <summary>
        /// Tries the first.
        /// </summary>
        /// <typeparam name="TRow">The type of the row.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="editQuery">The edit query.</param>
        /// <returns></returns>
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
        /// Counts the specified connection.
        /// </summary>
        /// <typeparam name="TRow">The type of the row.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <returns></returns>
        public static int Count<TRow>(this IDbConnection connection)
            where TRow : class, IRow, new()
        {
            return connection.Count<TRow>(null);
        }

        /// <summary>
        /// Counts the specified where.
        /// </summary>
        /// <typeparam name="TRow">The type of the row.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="where">The where.</param>
        /// <returns></returns>
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
        /// Existses the by identifier.
        /// </summary>
        /// <typeparam name="TRow">The type of the row.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
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
        /// Existses the specified where.
        /// </summary>
        /// <typeparam name="TRow">The type of the row.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="where">The where.</param>
        /// <returns></returns>
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
        /// Lists the specified connection.
        /// </summary>
        /// <typeparam name="TRow">The type of the row.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <returns></returns>
        public static List<TRow> List<TRow>(this IDbConnection connection)
            where TRow : class, IRow, new()
        {
            return connection.List<TRow>((Criteria)null);
        }

        /// <summary>
        /// Lists the specified where.
        /// </summary>
        /// <typeparam name="TRow">The type of the row.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="where">The where.</param>
        /// <returns></returns>
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
        /// Lists the specified edit query.
        /// </summary>
        /// <typeparam name="TRow">The type of the row.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="editQuery">The edit query.</param>
        /// <returns></returns>
        public static List<TRow> List<TRow>(this IDbConnection connection, Action<SqlQuery> editQuery)
            where TRow : class, IRow, new()
        {
            var row = new TRow() { TrackWithChecks = true };
            var query = new SqlQuery().From(row);

            editQuery(query);

            return query.List(connection, row);
        }

        /// <summary>
        /// Inserts the specified row.
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
        /// Inserts the and get identifier.
        /// </summary>
        /// <typeparam name="TRow">The type of the row.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public static long? InsertAndGetID<TRow>(this IDbConnection connection, TRow row)
            where TRow : IRow
        {
            return ToSqlInsert(row).ExecuteAndGetID(connection);
        }

        /// <summary>
        /// Updates the by identifier.
        /// </summary>
        /// <typeparam name="TRow">The type of the row.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="row">The row.</param>
        /// <param name="expectedRows">The expected rows.</param>
        /// <exception cref="InvalidOperationException">ID field of row has null value!</exception>
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
        /// Deletes the by identifier.
        /// </summary>
        /// <typeparam name="TRow">The type of the row.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="expectedRows">The expected rows.</param>
        /// <returns></returns>
        public static int DeleteById<TRow>(this IDbConnection connection, object id, ExpectedRows expectedRows = ExpectedRows.One)
            where TRow : class, IRow, IIdRow, new()
        {
            var row = new TRow();
            return new SqlDelete(row.Table)
                .Where(row.IdField == new ValueCriteria(id))
                .Execute(connection, expectedRows);
        }

        /// <summary>
        /// Converts to sqlinsert.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">row</exception>
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
        ///   Creates a new SqlUpdate query.</summary>
        /// <param name="row">
        ///   Row with field values to set in new record (must be in TrackAssignments mode).</param>
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
}