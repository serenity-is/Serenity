using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported(ObeysTypeSystem = true)]
    public class BooleanFiltering : BaseFiltering
    {
        public override List<FilterOperator> GetOperators()
        {
            return null;
        }
    }
}