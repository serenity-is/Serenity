using System;

namespace Serenity.Data
{
    public class SqlServer2005Dialect : SqlServer2000Dialect
    {
        public static new readonly ISqlDialect Instance = new SqlServer2005Dialect();

        public override bool CanUseRowNumber
        {
            get
            {
                return true;
            }
        }
    }
}