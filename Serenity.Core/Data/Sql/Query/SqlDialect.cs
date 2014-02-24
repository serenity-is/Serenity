using System;

namespace Serenity.Data
{
    [Flags]
    public enum SqlDialect
    {
        ServerKindMsSql = 1,
        ServerKindFirebird = 2,
        UseSkipKeyword = 512,
        UseRowNumber = 1024,
        UseOffsetFetch = 2048,
        Firebird = ServerKindFirebird | UseSkipKeyword,
        MsSql2000 = ServerKindMsSql,
        MsSql2005 = ServerKindMsSql | UseRowNumber,
        MsSql2012 = ServerKindMsSql | UseOffsetFetch | UseRowNumber
    }
}