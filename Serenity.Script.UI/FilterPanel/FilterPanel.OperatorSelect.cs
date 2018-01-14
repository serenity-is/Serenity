using jQueryApi;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported(ObeysTypeSystem = true)]
    public class FilterOperatorSelect : Select2Editor<object, FilterOperator>
    {
        public FilterOperatorSelect(jQueryObject hidden, IEnumerable<FilterOperator> source)
            : base(hidden, null)
        {
        }
    }
}
