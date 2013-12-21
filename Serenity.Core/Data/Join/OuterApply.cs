
namespace Serenity.Data
{
    public class OuterApply : Join
    {
        public OuterApply(string toTable, string alias)
            : base(null, toTable.IsEmptyOrNull() ? toTable : "(" + toTable + ")", alias, null)
        {
        }

        public OuterApply(RowFieldsBase fields, string toTable, string alias)
            : base(fields, toTable.IsEmptyOrNull() ? toTable : "(" + toTable + ")", alias, null)
        {
        }

        public override string GetKeyword()
        {
            return "Outer APPLY";
        }
     }
}