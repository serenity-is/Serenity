using System;
using System.Collections.Generic;
using System.Text;

namespace Serenity.Data
{
    /// <summary>
    /// Abstraction for SQL Query ToString(), e.g. dialect with custom ToString() method.
    /// </summary>
    public interface ISqlQueryToString
    {
        /// <summary>
        ///    The method which will call inside SqlQuery.ToString() method
        /// </summary>
        /// <returns>
        ///   Formatted SELECT statement</returns>
        string SqlQueryToString(SqlQuery sqlQuery, SqlQueryElements queryElements);
    }
}