using System.Collections.Generic;

namespace Serenity.Data
{
    public class CrossApply : Join
    {
        public CrossApply(string toTable, string alias)
            : base(null, toTable.IsNullOrEmpty() ? toTable : "(" + toTable + ")", alias, null)
        {
        }

        public CrossApply(IDictionary<string, Join> joins, string toTable, string alias)
            : base(joins, toTable.IsNullOrEmpty() ? toTable : "(" + toTable + ")", alias, null)
        {
        }

        public override string GetKeyword()
        {
            return "CROSS APPLY";
        }
     }
}