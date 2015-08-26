using System;
using System.Collections.Generic;
using System.Text;

namespace Serenity.Data
{
    public class DatabaseReferenceReplacer
    {
        public static string Replace(string expression)
        {
            if (expression == null || expression.IndexOf('^') < 0)
                return expression;

            return BracketLocator.ReplaceBracketContents(expression, '!', contents =>
            {
                var idx = contents.IndexOf('!');
                if (idx < 0)
                    return contents;

                string connectionKey = null;

                if (idx != 0)
                    connectionKey = contents.Substring(0, idx);

                string databaseName;

                if (!connectionKey.IsEmptyOrNull())
                {
                    databaseName = SqlConnections.GetDatabaseName(connectionKey);
                    if (!string.IsNullOrEmpty(databaseName))
                        return databaseName;
                }

                if (idx < contents.Length - 1)
                    return contents.Substring(idx + 1);

                return contents;
            });
        }
    }
}