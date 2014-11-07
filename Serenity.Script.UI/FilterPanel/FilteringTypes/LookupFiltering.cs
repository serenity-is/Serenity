using jQueryApi;
using Serenity.ComponentModel;
using Serenity.Data;
using System;
using System.Collections.Generic;

namespace Serenity
{
    public class LookupFiltering : BaseEditorFiltering<LookupEditor>
    {
        public override List<FilterOperator> GetOperators()
        {
            return AppendNullableOperators(
                new List<FilterOperator>
                {
                    FilterOperators.EQ,
                    FilterOperators.NE,
                    FilterOperators.Contains,
                    FilterOperators.StartsWith
                });
        }

        protected override bool UseIdField()
        {
            return IsComparison(Operator.Key);
        }
    }
}