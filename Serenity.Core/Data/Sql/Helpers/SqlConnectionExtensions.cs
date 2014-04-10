using Serenity.Services;
using System;
using System.Collections.Generic;
using System.Data;

namespace Serenity.Data
{
    public static class SqlConnectionExtensions
    {
        public static TRow ById<TRow>(this IDbConnection connection, Int64 id)
            where TRow: Row, IIdRow, new()
        {
            var row = TryById<TRow>(connection, id);
            if (row == null)
                throw new ValidationError("RecordNotFound", String.Format("Can't locate '{0}' record with ID {1}!", new TRow().Table, id));

            return row;
        }

        public static TRow TryById<TRow>(this IDbConnection connection, Int64 id)
            where TRow: Row, IIdRow, new()
        {
            var row = new TRow() { TrackWithChecks = true };
            if (new SqlQuery().From(row)
                    .SelectTableFields()
                    .Where(new Criteria((IField)row.IdField) == id)
                    .GetSingle(connection))
                return row;

            return null;
        }

        public static TRow ById<TRow>(this IDbConnection connection, Int64 id, Action<SqlQuery> editQuery)
            where TRow : Row, IIdRow, new()
        {
            var row = TryById<TRow>(connection, id, editQuery);
            if (row == null)
                throw new ValidationError("RecordNotFound", String.Format("Can't locate '{0}' record with ID {1}!", new TRow().Table, id));

            return row;
        }

        public static TRow TryById<TRow>(this IDbConnection connection, Int64 id, Action<SqlQuery> editQuery)
            where TRow: Row, IIdRow, new()
        {
            var row = new TRow() { TrackWithChecks = true };
            var query = new SqlQuery().From(row)
                .Where(new Criteria((IField)row.IdField) == id);

            editQuery(query);

            if (query.GetSingle(connection))
                return row;

            return null;
        }

        public static TRow Single<TRow>(this IDbConnection connection, BaseCriteria where)
            where TRow : Row, new()
        {
            var row = TrySingle<TRow>(connection, where);
            
            if (row == null)
                throw new ValidationError("RecordNotFound", "Query returned no results!");

            return row;
        }

        public static TRow TrySingle<TRow>(this IDbConnection connection, BaseCriteria where)
            where TRow : Row, new()
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
            where TRow : Row, IIdRow, new()
        {
            var row = TrySingle<TRow>(connection, editQuery);

            if (row == null)
                throw new ValidationError("RecordNotFound", "Query returned no results!");

            return row;
        }

        public static TRow TrySingle<TRow>(this IDbConnection connection, Action<SqlQuery> editQuery)
            where TRow : Row, IIdRow, new()
        {
            var row = new TRow() { TrackWithChecks = true };
            var query = new SqlQuery().From(row);

            editQuery(query);

            if (query.GetSingle(connection))
                return row;

            return null;
        }

        public static TRow First<TRow>(this IDbConnection connection, BaseCriteria where)
            where TRow : Row, new()
        {
            var row = TryFirst<TRow>(connection, where);

            if (row == null)
                throw new ValidationError("RecordNotFound", "Query returned no results!");

            return row;
        }

        public static TRow TryFirst<TRow>(this IDbConnection connection, BaseCriteria where)
            where TRow : Row, new()
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
            where TRow : Row, IIdRow, new()
        {
            var row = TryFirst<TRow>(connection, editQuery);

            if (row == null)
                throw new ValidationError("RecordNotFound", "Query returned no results!");

            return row;
        }

        public static TRow TryFirst<TRow>(this IDbConnection connection, Action<SqlQuery> editQuery)
            where TRow : Row, IIdRow, new()
        {
            var row = new TRow() { TrackWithChecks = true };
            var query = new SqlQuery().From(row);

            editQuery(query);

            if (query.GetFirst(connection))
                return row;

            return null;
        }

        public static int Count<TRow>(this IDbConnection connection, BaseCriteria where)
            where TRow : Row, new()
        {
            var row = new TRow() { TrackWithChecks = true };

            return Convert.ToInt32(SqlHelper.ExecuteScalar(connection,
                new SqlQuery().From(row)
                    .Select(Sql.Count())
                    .Where(where)));
        }

        public static bool ExistsById<TRow>(this IDbConnection connection, Int64 id)
            where TRow : Row, IIdRow, new()
        {
            var row = new TRow();
            return new SqlQuery()
                    .From(row)
                    .Select("1")
                    .Where(new Criteria((IField)row.IdField) == id)
                    .Exists(connection);
        }

        public static bool Exists<TRow>(this IDbConnection connection, BaseCriteria where)
            where TRow : Row, new()
        {
            var row = new TRow() { TrackWithChecks = true };
            return new SqlQuery().From(row)
                    .Select("1")
                    .Where(where)
                    .Exists(connection);
        }


        public static List<TRow> List<TRow>(this IDbConnection connection, Criteria where)
            where TRow : Row, new()
        {
            var row = new TRow() { TrackWithChecks = true };
            return new SqlQuery().From(row)
                    .SelectTableFields()
                    .Where(where)
                    .List(connection, row);
        }

        public static List<TRow> List<TRow>(this IDbConnection connection, Int64 id, Action<SqlQuery> editQuery)
            where TRow : Row, new()
        {
            var row = new TRow() { TrackWithChecks = true };
            var query = new SqlQuery().From(row);

            editQuery(query);

            return query.List(connection, row);
        }

        public static void Insert<TRow>(this IDbConnection connection, TRow row)
            where TRow: Row
        {
            new SqlInsert(row).Execute(connection);
        }

        public static Int64? InsertAndGetID<TRow>(this IDbConnection connection, TRow row)
            where TRow : Row
        {
            return new SqlInsert(row).ExecuteAndGetID(connection);
        }

        public static void Update<TRow>(this IDbConnection connection, TRow row, ExpectedRows expectedRows = ExpectedRows.One)
            where TRow: Row, IIdRow
        {
            var idField = (Field)row.IdField;

            if (idField.IsNull(row))
                throw new InvalidOperationException("ID field of row has null value!");

            new SqlUpdate(row)
                .Execute(connection, expectedRows);
        }

        public static int DeleteById<TRow>(this IDbConnection connection, Int64 id, ExpectedRows expectedRows = ExpectedRows.One)
            where TRow: Row, IIdRow, new()
        {
            var row = new TRow();
            return new SqlDelete(row.Table)
                .Where(new Criteria((Field)row.IdField) == id)
                .Execute(connection, expectedRows);
        }
    }
}