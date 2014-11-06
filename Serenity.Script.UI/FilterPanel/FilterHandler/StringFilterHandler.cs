using jQueryApi;
using Serenity.Data;
using System;
using System.Collections.Generic;

namespace Serenity
{
    public class StringFilterHandler : BaseFilterHandler
    {
        public override List<FilterOperator> GetOperators()
        {
            return AppendNullableOperators(new List<FilterOperator>
            {
                OperatorKeys.Contains,
                OperatorKeys.StartsWith,
                OperatorKeys.EQ,
                OperatorKeys.NE,
                OperatorKeys.BW
            });
        }
    }
}