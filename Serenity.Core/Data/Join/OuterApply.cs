
namespace Serenity.Data
{
    public class OuterApply : Join
    {
        public OuterApply(string innerQuery, string alias)
            : base(null, innerQuery.IsEmptyOrNull() ? innerQuery : "(" + innerQuery + ")", alias, null)
        {
        }

        public OuterApply(RowFieldsBase fields, string innerQuery, string alias)
            : base(fields, innerQuery.IsEmptyOrNull() ? innerQuery : "(" + innerQuery + ")", alias, null)
        {
        }

        public override string GetKeyword()
        {
            return "Outer APPLY";
        }
     }
}