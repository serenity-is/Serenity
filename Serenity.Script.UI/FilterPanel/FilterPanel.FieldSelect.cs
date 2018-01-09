using jQueryApi;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity.FilterPanels
{
    [Imported(ObeysTypeSystem = true)]
    public class FieldSelect : Select2Editor<object, PropertyItem>
    {
        public FieldSelect(jQueryObject hidden, IEnumerable<PropertyItem> fields)
            : base(hidden, null)
        {
        }
    }
}