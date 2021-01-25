using System;
using System.Collections.Generic;
using System.Data;
using Dictionary = System.Collections.Generic.Dictionary<string, object>;

namespace Serenity.Data
{
    /// <summary>
    /// EntitySqlHelper
    /// </summary>
    public static class EntitySqlHelper
    {
        /// <summary>
        /// Gets the first.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="connection">The connection.</param>
        /// <returns></returns>
        public static bool GetFirst(this SqlQuery query, IDbConnection connection)
        {
            using IDataReader reader = SqlHelper.ExecuteReader(connection, query);
            if (reader.Read())
            {
                query.GetFromReader(reader);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Gets the first.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="row">The row.</param>
        /// <param name="param">The parameter.</param>
        /// <returns></returns>
        public static bool GetFirst(this SqlQuery query, IDbConnection connection, IEntity row, Dictionary param)
        {
            using IDataReader reader = SqlHelper.ExecuteReader(connection, query, param);
            if (reader.Read())
            {
                query.GetFromReader(reader, new IEntity[] { row });
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Gets the single.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="connection">The connection.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Query returned more than one result!</exception>
        public static bool GetSingle(this SqlQuery query, IDbConnection connection)
        {
            using IDataReader reader = SqlHelper.ExecuteReader(connection, query);
            if (reader.Read())
            {
                query.GetFromReader(reader);

                if (reader.Read())
                    throw new InvalidOperationException("Query returned more than one result!");

                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Gets the single.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="row">The row.</param>
        /// <param name="param">The parameter.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Query returned more than one result!</exception>
        public static bool GetSingle(this SqlQuery query, IDbConnection connection, IRow row, Dictionary param)
        {
            using IDataReader reader = SqlHelper.ExecuteReader(connection, query, param);
            if (reader.Read())
            {
                query.GetFromReader(reader, new IRow[] { row });

                if (reader.Read())
                    throw new InvalidOperationException("Query returned more than one result!");

                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Fors the first.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="callBack">The call back.</param>
        /// <returns></returns>
        public static bool ForFirst(this SqlQuery query, IDbConnection connection,
            Action callBack)
        {
            using IDataReader reader = SqlHelper.ExecuteReader(connection, query);
            if (reader.Read())
            {
                query.GetFromReader(reader);
                callBack();
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Fors the first.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="callBack">The call back.</param>
        /// <returns></returns>
        public static bool ForFirst(this SqlQuery query, IDbConnection connection,
            ReaderCallBack callBack)
        {
            using IDataReader reader = SqlHelper.ExecuteReader(connection, query);
            if (reader.Read())
            {
                query.GetFromReader(reader);
                callBack(reader);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Fors the each.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="callBack">The call back.</param>
        /// <returns></returns>
        public static int ForEach(this SqlQuery query, IDbConnection connection,
            Action callBack)
        {
            int count = 0;

            if (connection.GetDialect().MultipleResultsets)
            {
                using IDataReader reader = SqlHelper.ExecuteReader(connection, query);
                while (reader.Read())
                {
                    query.GetFromReader(reader);
                    callBack();
                }

                if (query.CountRecords && reader.NextResult() && reader.Read())
                    return Convert.ToInt32(reader.GetValue(0));
            }
            else
            {
                string[] queries = query.ToString().Split(new string[] { "\n---\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (queries.Length > 1)
                    count = Convert.ToInt32(SqlHelper.ExecuteScalar(connection, queries[1], query.Params));

                using IDataReader reader = SqlHelper.ExecuteReader(connection, queries[0], query.Params);
                while (reader.Read())
                {
                    query.GetFromReader(reader);
                    callBack();
                }
            }

            return count;
        }

        /// <summary>
        /// Fors the each.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="callBack">The call back.</param>
        /// <returns></returns>
        public static int ForEach(this SqlQuery query, IDbConnection connection,
            ReaderCallBack callBack)
        {
            int count = 0;

            if (connection.GetDialect().MultipleResultsets)
            {
                using IDataReader reader = SqlHelper.ExecuteReader(connection, query);
                while (reader.Read())
                {
                    query.GetFromReader(reader);
                    callBack(reader);
                }

                if (query.CountRecords && reader.NextResult() && reader.Read())
                    return Convert.ToInt32(reader.GetValue(0));
            }
            else
            {
                string[] queries = query.ToString().Split(new string[] { "\n---\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (queries.Length > 1)
                    count = Convert.ToInt32(SqlHelper.ExecuteScalar(connection, queries[1], query.Params));

                using IDataReader reader = SqlHelper.ExecuteReader(connection, queries[0], query.Params);
                while (reader.Read())
                {
                    query.GetFromReader(reader);
                    callBack(reader);
                }
            }

            return count;
        }

        /// <summary>
        /// Lists the specified connection.
        /// </summary>
        /// <typeparam name="TRow">The type of the row.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="loaderRow">The loader row.</param>
        /// <returns></returns>
        public static List<TRow> List<TRow>(this SqlQuery query,
            IDbConnection connection, TRow loaderRow = null) where TRow : class, IRow
        {
            var list = new List<TRow>();
            ForEach(query, connection, delegate ()
            {
                list.Add(loaderRow.Clone());
            });
            return list;
        }

        /// <summary>
        /// Gets from reader.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="reader">The reader.</param>
        public static void GetFromReader(this SqlQuery query, IDataReader reader)
        {
            var ext = (ISqlQueryExtensible)query;

            GetFromReader(query, reader, ext.IntoRows);
        }

        const string FieldReadValueError = "An error occurred while loading value of the field '{0}' of '{1}' from data reader. " +
            "Please make sure the field type matches the actual data type in database.\r\n\r\nThe error message is:\r\n{2}";

        /// <summary>
        /// Gets from reader.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="into">The into.</param>
        /// <exception cref="Exception">
        /// </exception>
        public static void GetFromReader(this SqlQuery query, IDataReader reader, IList<object> into)
        {
            var ext = (ISqlQueryExtensible)query;

            int index = -1;
            foreach (var info in ext.Columns)
            {
                index++;

                if (info.IntoRowIndex < 0 || info.IntoRowIndex >= into.Count)
                    continue;

                if (!(into[info.IntoRowIndex] is IRow row))
                    continue;

                if (info.IntoField is Field field &&
                    (field.Fields == row.Fields ||
                     field.Fields.GetType() == row.Fields.GetType()))
                {
                    try
                    {
                        field.GetFromReader(reader, index, row);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format(FieldReadValueError,
                            field.PropertyName ?? field.Name, row.GetType().FullName, ex.Message), ex);
                    }
                    continue;
                }

                var name = reader.GetName(index);
                field = row.Fields.FindField(name) ?? row.Fields.FindFieldByPropertyName(name);

                if (field is object)
                {
                    try
                    {
                        field.GetFromReader(reader, index, row);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format(FieldReadValueError,
                            field.PropertyName ?? field.Name, row.GetType().FullName, ex.Message), ex);
                    }
                    continue;
                }

                if (reader.IsDBNull(index))
                    row.SetDictionaryData(name, null);
                else
                {
                    var value = reader.GetValue(index);
                    row.SetDictionaryData(name, value);
                }
            }
        }
    }
}
