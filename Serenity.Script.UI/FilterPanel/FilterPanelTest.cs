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
            Type t = Window.Instance.As<dynamic>().Serene.Northwind.ProductGrid;
            dynamic grid = Activator.CreateInstance(t, Q.NewBodyDiv().Hide());
            var columns = grid.slickGrid.getColumns();
            var filterable = new FilterableColumns(columns);
            var dialog = new FilterDialog();
            var panel = dialog.FilterPanel;
            panel.ShowInitialLine = true;
            var store = new FilterStore(filterable);
            panel.ShowSearchButton = true;
            panel.Store = store;
            dialog.DialogOpen();
        }

        private class FilterableColumns : IFilterableSource
        {
            private SlickColumn[] columns;

            public FilterableColumns(SlickColumn[] columns)
            {
                this.columns = columns;
            }

            private class MyFilterField : IFilterField
            {
                private SlickColumn column;

                public MyFilterField(SlickColumn column)
                {
                    this.column = column;
                }

                public string Name
                {
                    get 
                    { 
                        return column.Field; 
                    }
                }

                public string Title
                {
                    get 
                    {
                        return column.Title; 
                    }
                }

                public bool NotNull
                {
                    get { return false; }
                }

                public string FilteringType
                {
                    get { return "String"; }
                }
            }

            public IFilterField FindField(string fieldName)
            {
                var column = columns.FirstOrDefault(x => x.Field == fieldName);
                return column == null ? null : new MyFilterField(column);
            }

            public IEnumerable<IFilterField> GetFields()
            {
                return columns.Select(x => (IFilterField)new MyFilterField(x));
            }
        }
    }
}