namespace Serenity.Data
{
    /// <summary>
    /// Abstraction for SQL Query ToString(), e.g. dialect with custom ToString() method.
    /// </summary>
    public interface ISqlQueryToString
    {
        /// <summary>
        /// The method which will call inside SqlQuery.ToString() method
        /// </summary>
        /// <value>
        ///   <c>true</c> if the server supports OFFSET FETCH; otherwise, <c>false</c>.
        /// </value>
        string SqlQueryToString(SqlQuery sqlQuery);

    }
}