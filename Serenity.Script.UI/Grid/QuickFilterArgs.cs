using System;
using System.Collections;
using System.Collections.Generic;
using jQueryApi;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported, Serializable, IncludeGenericArguments(false)]
    public class QuickFilterArgs<TWidget>
    {
        public string Field { get; set; }
        public TWidget Widget { get; set; }
        public ListRequest Request { get; set; }
        public JsDictionary<string, object> EqualityFilter { get; set; }
        public object Value { get; set; }
        public bool Active { get; set; }
        public bool Handled { get; set; }
    }
}