
using System.Data.Common;

namespace Serenity.Data
{
    /// <summary>
    /// Helper class for replacing database caret references in format [^ConnectionKey] in SQL expressions.
    /// </summary>
    public class DatabaseCaretReferences
    {
        /// <summary>
        /// Replaces caret references like [^ConnectionKey] in the specified expression with actual database names.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>Replaced expression.</returns>
        public static string Replace(string expression)
        {
            if (expression == null || expression.IndexOf('^') < 0)
                return expression;

            return BracketLocator.ReplaceBracketContents(expression, '^', contents =>
            {
                var idx = contents.IndexOf('^');
                if (idx < 0)
                    return contents;

                string connectionKey = null;

                if (idx != 0)
                    connectionKey = contents.Substring(0, idx);

                string databaseName;

                if (!connectionKey.IsEmptyOrNull())
                {
                    databaseName = GetDatabaseName(connectionKey);
                    if (!string.IsNullOrEmpty(databaseName))
                        return databaseName;
                }

                if (idx < contents.Length - 1)
                    return contents[(idx + 1)..];

                return contents;
            });
        }


        private static readonly string[] databaseNameKeys = new string[]
                {
            "Initial Catalog",
            "Database"
                };

        /// <summary>
        /// Exracts database name from connection string
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <returns></returns>
        public static string GetDatabaseName(string connectionString)
        {
            var csb = new DbConnectionStringBuilder
            {
                ConnectionString = connectionString
            };

            foreach (var s in databaseNameKeys)
                if (csb.ContainsKey(s))
                    return csb[s] as string;

            return null;
        }
    }
}