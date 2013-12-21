
namespace Serenity.Data
{
    public class RightJoin : Join
    {
        public RightJoin(string toTable, string alias, BaseCriteria onCriteria)
            : base(null, toTable, alias, onCriteria)
        {
        }

        public RightJoin(RowFieldsBase fields, string toTable, string alias, BaseCriteria onCriteria)
            : base(fields, toTable, alias, onCriteria)
        {
        }

        public override string GetKeyword()
        {
            return "RIGHT JOIN";
        }
     }
}