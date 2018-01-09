using jQueryApi;
using Serenity.Data;
using System;
using System.Collections.Generic;

namespace Serenity
{
    public class IntegerFiltering : BaseEditorFiltering
    {
        public IntegerFiltering()
            : base(typeof(IntegerEditor))
        {
        }

        public override List<FilterOperator> GetOperators()
        {
            return AppendNullableOperators(
                AppendComparisonOperators(
                    new List<FilterOperator>()));
        }
    }
}