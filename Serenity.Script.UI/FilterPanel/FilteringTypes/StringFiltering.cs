using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported(ObeysTypeSystem = true)]
    public class StringFiltering : BaseFiltering
    {
        public override List<FilterOperator> GetOperators()
        {
            return AppendNullableOperators(new List<FilterOperator>
            {
                FilterOperators.Contains,
                FilterOperators.StartsWith,
                FilterOperators.EQ,
                FilterOperators.NE,
                FilterOperators.BW
            });
        }

        protected override object ValidateEditorValue(string value)
        {
            if (value.Length == 0)
                return value;

            return base.ValidateEditorValue(value);
        }
    }
}