#if COREFX
using Dapper;
using System.Collections.Generic;
using System.Data;

namespace Serenity.Data
{
    public static partial class SqlMapper
    {
        /// <summary>
        /// Execute parameterized SQL  
        /// </summary>
        /// <returns>Number of rows affected</returns>
        public static int Execute(this IDbConnection cnn, string sql, dynamic param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Dapper.SqlMapper.Execute(cnn, SqlHelper.FixCommandText(sql, cnn.GetDialect()), param, transaction, commandTimeout, commandType);
        }

        /// <summary>
        /// Return a list of dynamic objects, reader is closed after the call
        /// </summary>
        public static IEnumerable<dynamic> Query(this IDbConnection cnn, string sql, dynamic param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Dapper.SqlMapper.Query(cnn, SqlHelper.FixCommandText(sql, cnn.GetDialect()), param, transaction, buffered, commandTimeout, commandType);
        }

        public static IEnumerable<dynamic> Query(this IDbConnection cnn, ISqlQuery sql, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Dapper.SqlMapper.Query(cnn, SqlHelper.FixCommandText(sql.ToString(), cnn.GetDialect()), sql.Params == null ? null : new DynamicParameters(sql.Params), null, buffered, commandTimeout, commandType);
        }

        public static IEnumerable<TValue> Query<TValue>(this IDbConnection cnn, ISqlQuery sql, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Dapper.SqlMapper.Query<TValue>(cnn, SqlHelper.FixCommandText(sql.ToString(), cnn.GetDialect()), sql.Params == null ? null : new DynamicParameters(sql.Params), null, buffered, commandTimeout, commandType);
        }

        public static IEnumerable<dynamic> Query(this IDbConnection cnn, string sql, object param, IDbTransaction transaction)
        {
            return Dapper.SqlMapper.Query(cnn, SqlHelper.FixCommandText(sql.ToString(), cnn.GetDialect()), param, transaction);
        }

        /// <summary>
        /// Return a list of dynamic objects, reader is closed after the call
        /// </summary>
        public static IEnumerable<dynamic> Query(this IDbConnection cnn, string sql, object param, CommandType? commandType)
        {
            return Dapper.SqlMapper.Query(cnn, SqlHelper.FixCommandText(sql.ToString(), cnn.GetDialect()), param, null, true, null, commandType);
        }

        public static IEnumerable<dynamic> Query(this IDbConnection cnn, string sql, object param, IDbTransaction transaction, CommandType? commandType)
        {
            return Dapper.SqlMapper.Query(cnn, SqlHelper.FixCommandText(sql.ToString(), cnn.GetDialect()), param, transaction, true, null, commandType);
        }

        public static IEnumerable<T> Query<T>(this IDbConnection cnn, string sql, dynamic param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Dapper.SqlMapper.Query<T>(cnn, SqlHelper.FixCommandText(sql.ToString(), cnn.GetDialect()), param, transaction, buffered, commandTimeout, commandType);
        }
    }
}
#endif