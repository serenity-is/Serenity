using System;

namespace Serenity.Data
{
    public static class SqlSettings
    {
        public static bool AutoBracket { get; set; }

        public static ISqlDialect DefaultDialect = new SqlServer2012Dialect();
    }
}