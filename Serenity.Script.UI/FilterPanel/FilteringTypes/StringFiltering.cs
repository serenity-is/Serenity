using jQueryApi;
using Serenity.Data;
using System;
using System.Collections.Generic;

namespace Serenity
{
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