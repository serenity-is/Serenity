using jQueryApi;
using Serenity.Data;
using System;
using System.Collections.Generic;

namespace Serenity
{
    public class DateFiltering : BaseEditorFiltering<DateEditor>
    {
        public override List<FilterOperator> GetOperators()
        {
            return AppendNullableOperators(
                AppendComparisonOperators(
                    new List<FilterOperator>()));
        }
    }
}