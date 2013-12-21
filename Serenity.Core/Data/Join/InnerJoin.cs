
namespace Serenity.Data
{
    public class InnerJoin : Join
    {
        public InnerJoin(string toTable, string alias, BaseCriteria onCriteria)
            : base(null, toTable, alias, onCriteria)
        {
        }

        public InnerJoin(RowFieldsBase fields, string toTable, string alias, BaseCriteria onCriteria)
            : base(fields, toTable, alias, onCriteria)
        {
        }

        public override string GetKeyword()
        {
            return "INNER JOIN";
        }
     }
}