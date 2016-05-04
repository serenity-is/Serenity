using System;
using System.Collections;
using System.Collections.Generic;
using jQueryApi;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported, Serializable, IncludeGenericArguments(false)]
    public class QuickFilterOptions<TWidget>
    {
        public Type Type { get; set; }
        public string Field { get; set; }
        public string Title { get; set; }
        public object Options { get; set; }
        public Action<jQueryObject> Element { get; set; }
        public Action<TWidget> Init { get; set; }
        public Action<QuickFilterArgs<TWidget>> Handler { get; set; }
    }
}