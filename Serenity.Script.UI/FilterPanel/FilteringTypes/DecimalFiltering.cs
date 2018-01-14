using jQueryApi;
using Serenity.Data;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported(ObeysTypeSystem = true)]
    public class DecimalFiltering : BaseEditorFiltering
    {
        public DecimalFiltering()
            : base(typeof(DecimalEditor))
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