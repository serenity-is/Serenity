using System;

namespace Serenity.Data
{
    public class SqlServer2008Dialect : SqlServer2005Dialect
    {
        public static new readonly ISqlDialect Instance = new SqlServer2008Dialect();

        public override bool UseDateTime2
        {
            get
            {
                return true;
            }
        }
    }
}