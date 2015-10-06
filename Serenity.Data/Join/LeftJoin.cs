using System.Collections.Generic;

namespace Serenity.Data
{
    public class LeftJoin : Join
    {
        public LeftJoin(string toTable, string alias, ICriteria onCriteria)
            : base(null, toTable, alias, onCriteria)
        {
        }
        
        public LeftJoin(IDictionary<string, Join> joins, string toTable, string alias, ICriteria onCriteria)
            : base(joins, toTable, alias, onCriteria)
        {
        }

        public override string GetKeyword()
        {
            return "LEFT JOIN";
        }
     }
}