using System.Collections.Generic;

namespace Serenity.Data
{
    public class CrossApply : Join
    {
        public CrossApply(string toTable, string alias)
            : base(null, string.IsNullOrEmpty(toTable) ? toTable : "(" + toTable + ")", alias, null)
        {
        }

        public CrossApply(IDictionary<string, Join> joins, string toTable, string alias)
            : base(joins, string.IsNullOrEmpty(toTable) ? toTable : "(" + toTable + ")", alias, null)
        {
        }

        public override string GetKeyword()
        {
            return "CROSS APPLY";
        }
     }
}