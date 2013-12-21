
namespace Serenity.Data
{
    public class LeftJoin : Join
    {
        public LeftJoin(string toTable, string alias, BaseCriteria onCriteria)
            : base(null, toTable, alias, onCriteria)
        {
        }
        
        public LeftJoin(RowFieldsBase fields, string toTable, string alias, BaseCriteria onCriteria)
            : base(fields, toTable, alias, onCriteria)
        {
        }

        public override string GetKeyword()
        {
            return "LEFT JOIN";
        }
     }
}