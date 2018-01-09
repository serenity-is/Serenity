using jQueryApi;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity.FilterPanels
{
    [Imported(ObeysTypeSystem = true)]
    public class OperatorSelect : Select2Editor<object, FilterOperator>
    {
        public OperatorSelect(jQueryObject hidden, IEnumerable<FilterOperator> source)
            : base(hidden, null)
        {
        }
    }
}
