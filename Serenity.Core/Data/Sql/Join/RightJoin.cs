
using System.Collections.Generic;
namespace Serenity.Data
{
    public class RightJoin : Join
    {
        public RightJoin(string toTable, string alias, BaseCriteria onCriteria)
            : base(null, toTable, alias, onCriteria)
        {
        }

        public RightJoin(IDictionary<string, Join> joins, string toTable, string alias, BaseCriteria onCriteria)
            : base(joins, toTable, alias, onCriteria)
        {
        }

        public override string GetKeyword()
        {
            return "RIGHT JOIN";
        }
     }
}