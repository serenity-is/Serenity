using System;
using System.Collections.Generic;
using System.Html;
using System.Linq;

namespace Serenity
{
    public class FilterPanelTest : ScriptContext
    {
        public void Run()
        {
            var fields = Q.GetColumns("Northwind.Product");
            var dialog = new FilterDialog();
            var panel = dialog.FilterPanel;
            panel.ShowInitialLine = true;
            var store = new FilterStore(fields);
            panel.ShowSearchButton = true;
            panel.Store = store;
            dialog.DialogOpen();
        }
    }
}