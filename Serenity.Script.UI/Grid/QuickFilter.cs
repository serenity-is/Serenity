using System;
using System.Collections;
using System.Collections.Generic;
using jQueryApi;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported, Serializable, IncludeGenericArguments(false)]
    public class QuickFilter<TWidget, TOptions>
        where TWidget: Widget
    {
        public string Field { get; set; }
        public string Title { get; set; }
        public Type Type { get; set; }
        public TOptions Options { get; set; }
        public Action<jQueryObject> Element { get; set; }
        public Action<TWidget> Init { get; set; }
        public Action<QuickFilterArgs<TWidget>> Handler { get; set; }
        public bool Seperator { get; set; }
        public string CssClass { get; set; }
    }
}