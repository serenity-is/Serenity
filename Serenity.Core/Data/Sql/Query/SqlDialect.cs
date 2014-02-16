using System;

namespace Serenity.Data
{
    [Flags]
    public enum SqlDialect
    {
        MsSql = 1,
        Firebird = 2,
        UseSkipKeyword = 512,
        UseRowNumber = 1024,
        UseOffsetFetch = 2048,
        MsSql2005 = MsSql | UseRowNumber,
        MsSql2012 = MsSql | UseOffsetFetch | UseRowNumber
    }
}