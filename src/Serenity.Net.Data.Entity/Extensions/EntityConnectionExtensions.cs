using System;
using System.Collections.Generic;
using System.Data;
using Serenity.Services;

namespace Serenity.Data
{
    public static class EntityConnectionExtensions
    {
        public static TRow ById<TRow>(this IDbConnection connection, object id)
            where TRow: class, IRow, IIdRow, new()
        {
            var row = TryById<TRow>(connection, id);
            if (row == null)
                throw new ValidationError("RecordNotFound", string.Format("Can't locate '{0}' record with ID {1}!", new TRow().Table, id));

            return row;
        }

        public static TRow TryById<TRow>(this IDbConnection connection, object id)
            where TRow: class, IRow, IIdRow, new()
        {
            var row = new TRow() { TrackWithChecks = true };
            if (new SqlQuery().From(row)
                    .SelectTableFields()
                    .Where(new Criteria((IField)row.IdField) == new ValueCriteria(id))
                    .GetSingle(connection))
                return row;

            return null;
        }

        public static TRow ById<TRow>(this IDbConnection connection, object id, Action<SqlQuery> editQuery)
            where TRow : class, IRow, IIdRow, new()
        {
            var row = TryById<TRow>(connection, id, editQuery);
            if (row == null)
                throw new ValidationError("RecordNotFound", string.Format("Can't locate '{0}' record with ID {1}!", new TRow().Table, id));

            return row;
        }

        public static TRow TryById<TRow>(this IDbConnection connection, object id, Action<SqlQuery> editQuery)
            where TRow: class, IRow, IIdRow, new()
        {
            var row = new TRow() { TrackWithChecks = true };
            var query = new SqlQuery().From(row)
                .Where(new Criteria((IField)row.IdField) == new ValueCriteria(id));

            editQuery(query);

            if (query.GetSingle(connection))
                return row;

            return null;
        }

        public static TRow Single<TRow>(this IDbConnection connection, ICriteria where)
            where TRow: class, IRow, new()
        {
            var row = TrySingle<TRow>(connection, where);
            
            if (row == null)
                throw new ValidationError("RecordNotFound", "Query returned no results!");

            return row;
        }

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

        public static TRow Single<TRow>(this IDbConnection connection, Action<SqlQuery> editQuery)
            where TRow : class, IRow, new()
        {
            var row = TrySingle<TRow>(connection, editQuery);

            if (row == null)
                throw new ValidationError("RecordNotFound", "Query returned no results!");

            return row;
        }

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

        public static TRow First<TRow>(this IDbConnection connection, ICriteria where)
            where TRow : class, IRow, new()
        {
            var row = TryFirst<TRow>(connection, where);

            if (row == null)
                throw new ValidationError("RecordNotFound", "Query returned no results!");

            return row;
        }

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

        public static TRow First<TRow>(this IDbConnection connection, Action<SqlQuery> editQuery)
            where TRow : class, IRow, new()
        {
            var row = TryFirst<TRow>(connection, editQuery);

            if (row == null)
                throw new ValidationError("RecordNotFound", "Query returned no results!");

            return row;
        }

        public static TRow TryFirst<TRow>(this IDbConnection connection, Action<SqlQuery> editQuery)
            where TRow: class, IRow, new()
        {
            var row = new TRow() { TrackWithChecks = true };
            var query = new SqlQuery().From(row);

            editQuery(query);

            if (query.GetFirst(connection))
                return row;

            return null;
        }

        public static int Count<TRow>(this IDbConnection connection)
            where TRow: class, IRow, new()
        {
            return connection.Count<TRow>((Criteria)null);
        }

        public static int Count<TRow>(this IDbConnection connection, ICriteria where)
            where TRow: class, IRow, new()
        {
            var row = new TRow() { TrackWithChecks = true };

            return Convert.ToInt32(SqlHelper.ExecuteScalar(connection,
                new SqlQuery().From(row)
                    .Select(Sql.Count())
                    .Where(where)));
        }

        public static bool ExistsById<TRow>(this IDbConnection connection, object id)
            where TRow: class, IRow, IIdRow, new()
        {
            var row = new TRow();
            return new SqlQuery()
                    .From(row)
                    .Select("1")
                    .Where(new Criteria((IField)row.IdField) == new ValueCriteria(id))
                    .Exists(connection);
        }

        public static bool Exists<TRow>(this IDbConnection connection, ICriteria where)
            where TRow: class, IRow, new()
        {
            var row = new TRow() { TrackWithChecks = true };
            return new SqlQuery().From(row)
                    .Select("1")
                    .Where(where)
                    .Exists(connection);
        }

        public static List<TRow> List<TRow>(this IDbConnection connection)
            where TRow: class, IRow, new()
        {
            return connection.List<TRow>((Criteria)null);
        }

        public static List<TRow> List<TRow>(this IDbConnection connection, ICriteria where)
            where TRow: class, IRow, new()
        {
            var row = new TRow() { TrackWithChecks = true };
            return new SqlQuery().From(row)
                    .SelectTableFields()
                    .Where(where)
                    .List(connection, row);
        }

        public static List<TRow> List<TRow>(this IDbConnection connection, Action<SqlQuery> editQuery)
            where TRow : class, IRow, new()
        {
            var row = new TRow() { TrackWithChecks = true };
            var query = new SqlQuery().From(row);

            editQuery(query);

            return query.List(connection, row);
        }

        public static void Insert<TRow>(this IDbConnection connection, TRow row)
            where TRow: class, IRow
        {
            ToSqlInsert(row).Execute(connection);
        }

        public static long? InsertAndGetID<TRow>(this IDbConnection connection, TRow row)
            where TRow : IRow
        {
            return ToSqlInsert(row).ExecuteAndGetID(connection);
        }

        public static void UpdateById<TRow>(this IDbConnection connection, TRow row, ExpectedRows expectedRows = ExpectedRows.One)
            where TRow: IIdRow
        {
            var idField = (Field)row.IdField;
            var r = (IRow)(object)row;

            if (idField.IsNull(r))
                throw new InvalidOperationException("ID field of row has null value!");

            row.ToSqlUpdateById()
                .Execute(connection, expectedRows);
        }

        public static int DeleteById<TRow>(this IDbConnection connection, object id, ExpectedRows expectedRows = ExpectedRows.One)
            where TRow: class, IRow, IIdRow, new()
        {
            var row = new TRow();
            return new SqlDelete(row.Table)
                .Where((Field)row.IdField == new ValueCriteria(id))
                .Execute(connection, expectedRows);
        }

        public static SqlInsert ToSqlInsert(this IRow row)
        {
            if (row == null)
                throw new ArgumentNullException("row");

            var insert = new SqlInsert(row.Table);
            
            insert.Set(row);

            if (row as IIdRow != null)
                insert.IdentityColumn(((Field)(((IIdRow)row).IdField)).Name);

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

            update.Set((IRow)row, (Field)(row.IdField));
            var idField = (Field)row.IdField;
            update.Where(idField == new ValueCriteria(idField.AsSqlValue((IRow)row)));

            return update;
        }
    }
}