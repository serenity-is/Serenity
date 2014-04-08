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

            return null;
        }

        public static TRow TryById<TRow>(this IDbConnection connection, Int64 id)
            where TRow: Row, IIdRow, new()
        {
            var row = new TRow() { TrackWithChecks = true };
            if (new SqlQuery().From(row)
                    .SelectTableFields()
                    .Where(new Criteria((IField)row.IdField) == id)
                    .GetFirst(connection))
                return row;

            return null;
        }

        public static TRow ById<TRow>(this IDbConnection connection, Int64 id, Action<SqlQuery> editQuery)
            where TRow : Row, IIdRow, new()
        {
            var row = TryById<TRow>(connection, id, editQuery);
            if (row == null)
                throw new ValidationError("RecordNotFound", String.Format("Can't locate '{0}' record with ID {1}!", new TRow().Table, id));

            return null;
        }

        public static TRow TryById<TRow>(this IDbConnection connection, Int64 id, Action<SqlQuery> editQuery)
            where TRow: Row, IIdRow, new()
        {
            var row = new TRow() { TrackWithChecks = true };
            var query = new SqlQuery().From(row)
                .Where(new Criteria((IField)row.IdField) == id);

            editQuery(query);

            if (query.GetFirst(connection))
                return row;

            return null;
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
                    .GetFirst(connection))
                return row;

            return null;
        }

        public static TRow Single<TRow>(this IDbConnection connection, Action<SqlQuery> editQuery)
            where TRow : Row, IIdRow, new()
        {
            var row = TrySingle<TRow>(connection, editQuery);

            if (row == null)
                throw new ValidationError("RecordNotFound", "Query returned no results!");

            return null;
        }

        public static TRow TrySingle<TRow>(this IDbConnection connection, Action<SqlQuery> editQuery)
            where TRow : Row, IIdRow, new()
        {
            var row = new TRow() { TrackWithChecks = true };
            var query = new SqlQuery().From(row);

            editQuery(query);

            if (query.GetFirst(connection))
                return row;

            return null;
        }

    }
}