using jQueryApi;
using Serenity.Data;
using System;
using System.Collections.Generic;

namespace Serenity
{
    public class EnumFiltering : BaseEditorFiltering
    {
        public EnumFiltering()
            : base(typeof(EnumEditor))
        {
        }

        public override List<FilterOperator> GetOperators()
        {
            return AppendNullableOperators(
                new List<FilterOperator>
                {
                    FilterOperators.EQ,
                    FilterOperators.NE
                });
        }
    }
}