using jQueryApi;
using Serenity.Data;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported(ObeysTypeSystem = true)]
    public interface IQuickFiltering
    {
        [IncludeGenericArguments(false)]
        void InitQuickFilter(QuickFilter<Widget, object> filter);
    }
}
