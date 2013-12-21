
namespace Serenity.Data
{
    public class CrossApply : Join
    {
        public CrossApply(string toTable, string alias)
            : base(null, toTable.IsEmptyOrNull() ? toTable : "(" + toTable + ")", alias, null)
        {
        }

        public CrossApply(RowFieldsBase fields, string toTable, string alias)
            : base(fields, toTable.IsEmptyOrNull() ? toTable : "(" + toTable + ")", alias, null)
        {
        }

        public override string GetKeyword()
        {
            return "CROSS APPLY";
        }
     }
}