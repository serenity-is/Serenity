using System;
using System.Collections;
using System.Collections.Generic;
using jQueryApi;

namespace Serenity
{
    public interface IDataGrid
    {
        jQueryObject GetElement();
        SlickGrid GetGrid();
        SlickRemoteView GetView();
        FilterStore GetFilterStore();
    }
}