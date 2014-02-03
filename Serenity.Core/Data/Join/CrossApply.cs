
using System.Collections.Generic;
namespace Serenity.Data
{
    public class CrossApply : Join
    {
        public CrossApply(string toTable, string alias)
            : base(null, toTable.IsEmptyOrNull() ? toTable : "(" + toTable + ")", alias, null)
        {
        }

        public CrossApply(IDictionary<string, Join> joins, string toTable, string alias)
            : base(joins, toTable.IsEmptyOrNull() ? toTable : "(" + toTable + ")", alias, null)
        {
        }

        public override string GetKeyword()
        {
            return "CROSS APPLY";
        }
     }
}