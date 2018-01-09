using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using jQueryApi;

namespace Serenity
{
    [Imported(ObeysTypeSystem = true)]
    public interface IDataGrid
    {
        jQueryObject GetElement();
        SlickGrid GetGrid();
        SlickRemoteView GetView();
        FilterStore GetFilterStore();
    }
}