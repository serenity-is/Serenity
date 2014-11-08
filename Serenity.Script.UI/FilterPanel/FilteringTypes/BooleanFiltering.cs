using jQueryApi;
using Serenity.Data;
using System;
using System.Collections.Generic;

namespace Serenity
{
    public class BooleanFiltering : BaseFiltering
    {
        public override List<FilterOperator> GetOperators()
        {
            return AppendNullableOperators(new List<FilterOperator>
            {
                FilterOperators.IsTrue,
                FilterOperators.IsFalse
            });
        }
    }
}