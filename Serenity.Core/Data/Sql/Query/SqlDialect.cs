using System;

namespace Serenity.Data
{
    [Flags]
    public enum SqlDialect
    {
        MsSql = 16,
        MsSql2000 = 16,
        MsSql2005 = 17,
        MsSql2012 = 18,
        Firebird = 32,
        Sqlite = 64
    }
}