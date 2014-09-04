
using System.Collections.Generic;

namespace Serenity.Data
{
    public class InnerJoin : Join
    {
        public InnerJoin(string toTable, string alias, ICriteria onCriteria)
            : base(null, toTable, alias, onCriteria)
        {
        }

        public InnerJoin(IDictionary<string, Join> joins, string toTable, string alias, ICriteria onCriteria)
            : base(joins, toTable, alias, onCriteria)
        {
        }

        public override string GetKeyword()
        {
            return "INNER JOIN";
        }
     }
}