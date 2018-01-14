using jQueryApi;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported(ObeysTypeSystem = true)]
    public class FilterFieldSelect : Select2Editor<object, PropertyItem>
    {
        public FilterFieldSelect(jQueryObject hidden, IEnumerable<PropertyItem> fields)
            : base(hidden, null)
        {
        }
    }
}