using jQueryApi;
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
            Document.Body.InnerHTML = "";
            var div = Q.NewBodyDiv().AppendTo(Document.Body);
            Type t = Window.Instance.As<dynamic>().Marmara.Personel.PersonelIletisimGrid;
            dynamic grid = Activator.CreateInstance(t, Q.NewBodyDiv().Hide());
            var filterable = new FilterableColumns(grid.slickGrid.getColumns());
            var panel = new FilterPanel(div);
            panel.Source = filterable;
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

    public partial class FilterPanel : FilterWidgetBase<object>
    {
        private jQueryObject rowsDiv;

        public FilterPanel(jQueryObject div)
            : base(div, null)
        {
            this.Element.AddClass("s-FilterPanel");
            rowsDiv = this.ById("Rows");

            InitButtons();
            UpdateButtons();
        }

        protected override string GetTemplate()
        {
            return PanelTemplate;
        }

        private void InitButtons()
        {
            this.ById("AddButton").Text(Q.Text("Controls.FilterPanel.AddFilter")).Click(AddButtonClick);
            this.ById("SearchButton").Text(Q.Text("Controls.FilterPanel.SearchButton")).Click(SearchButtonClick);
            this.ById("ResetButton").Text(Q.Text("Controls.FilterPanel.ResetButton")).Click(ResetButtonClick);
        }

        private void SearchButtonClick(jQueryEvent e)
        {
            e.PreventDefault();
        }

        private void AddButtonClick(jQueryEvent e)
        {
            AddEmptyRow();
            e.PreventDefault();
        }

        private void ResetButtonClick(jQueryEvent e)
        {
            e.PreventDefault();

            rowsDiv.Empty();
            Store.Items.Clear();
            Store.RaiseChanged();
            UpdateButtons();
            //OnHeightChange();
        }

        private jQueryObject FindEmptyRow()
        {
            jQueryObject result = null;

            rowsDiv.Children().Each((index, row) =>
            {
                jQueryObject fieldInput = J(row).Children("div.f").Children("input.field-select").First();
                if (fieldInput.Length == 0)
                    return true;

                string val = fieldInput.GetValue();
                if (val == null || val.Length == 0)
                {
                    result = J(row);
                    return false;
                }

                return true;
            });

            return result;
        }

        private jQueryObject AddEmptyRow()
        {
            jQueryObject emptyRow = FindEmptyRow();

            if (emptyRow != null)
                return emptyRow;

            bool isLastRowOr = this.rowsDiv.Children().Last().Children("a.andor").HasClass("or");

            jQueryObject row = J(RowTemplate).AppendTo(this.rowsDiv);

            jQueryObject parenDiv = row.Children("div.l").Hide();

            parenDiv.Children("a.leftparen, a.rightparen").Click(LeftRightParenClick);

            jQueryObject andor = parenDiv.Children("a.andor").Attribute("title", Q.Text("Controls.FilterPanel.ChangeAndOr"));

            if (isLastRowOr)
                andor.AddClass("or").Text(Q.Text("Controls.FilterPanel.Or"));
            else
                andor.Text(Q.Text("Controls.FilterPanel.And"));

            andor.Click(AndOrClick);

            row.Children("a.delete").Attribute("title", Q.Text("Controls.FilterPanel.RemoveField")).Click(DeleteRowClick);

            var fieldSel = new FieldSelect(row.Children("div.f").Children("input"), this.Source);
            fieldSel.ChangeSelect2(OnRowFieldChange);

            UpdateParens();
            UpdateButtons();

            fieldSel.Element.Focus();

            return row;
        }

        private void OnRowFieldChange(jQueryEvent e)
        {
            jQueryObject row = J(e.Target).Closest("div.row");
            RowFieldChange(row);
            jQueryObject opSelect = row.Children("div.o").Children("select").Focus();
            try { opSelect.Focus(); }
            catch { }
        }

        private void RowFieldChange(jQueryObject row)
        {
            var select = row.Children("div.f").Children("input.field-select").GetWidget<FieldSelect>();
            string fieldName = select.Value;

            bool isEmpty = (fieldName == null || fieldName == "");

            jQueryObject opDiv = row.Children("div.o");
            jQueryObject opSelect = opDiv.Children("select");

            if (opSelect.Length == 0)
                opSelect = J("<select/>").AppendTo(opDiv).Change(OnRowOperatorChange);
            else
                Q.ClearOptions(opSelect);

            //RemoveFilterHandler(row);

            PopulateOperatorList(opSelect);

            RowOperatorChange(row);
            UpdateParens();
            UpdateButtons();
        }

        private void PopulateOperatorList(jQueryObject select)
        {
            jQueryObject row = select.Closest("div.row");

            /*IFilterHandler handler = GetFilterHandlerFor(row);
            if (handler == null)
                return;

            List<string> operators = handler.GetOperators();

            SelectElement sel = (SelectElement)select[0];

            if (operators != null)
                foreach (string op in operators)
                    Q.AddOption(select, op, handler.OperatorTitle(op));*/
        }


        private void OnRowOperatorChange(jQueryEvent e)
        {
            jQueryObject row = J(e.Target).Closest("div.row");
            RowOperatorChange(row);
            jQueryObject firstInput = row.Children("div.v").Find(":input:visible").First();
            try { firstInput.Focus(); }
            catch { };
        }

        private void RowOperatorChange(jQueryObject row)
        {
            if (row.Length == 0)
                return;

            jQueryObject editorDiv = row.Children("div.v");

            editorDiv.Html("");

            //IFilterHandler handler = null;//GetFilterHandlerFor(row);

            //if (handler == null)
            //    return;

            //string op = row.Children("div.o").Children("select").GetValue();
            //if (op == null || op == "")
            //    return;

            //handler.CreateEditor(op);
        }

        private void DeleteRowClick(jQueryEvent e)
        {
            e.PreventDefault();
            jQueryObject row = J(e.Target).Closest("div.row");
            row.Remove();
            if (this.rowsDiv.Children().Length == 0)
                Search();

            UpdateParens();
            UpdateButtons();
            //OnHeightChange();
        }

        private void Search()
        {
        }

        private void UpdateButtons()
        {
            this.ById("SearchButton").Toggle(rowsDiv.Children().Length >= 1);
            this.ById("ResetButton").Toggle(rowsDiv.Children().Length >= 1 ||
                this.Store.Items.Count > 0);
        }

        private void AndOrClick(jQueryEvent e)
        {
            e.PreventDefault();
            jQueryObject andor = J(e.Target).ToggleClass("or");
            andor.Text(Q.Text("Controls.FilterPanel." + (andor.HasClass("or") ? "Or" : "And")));
        }

        private void LeftRightParenClick(jQueryEvent e)
        {
            e.PreventDefault();
            J(e.Target).ToggleClass("active");
            UpdateParens();
        }

        private void UpdateParens()
        {
            jQueryObject rows = rowsDiv.Children();
            if (rows.Length == 0)
                return;

            rows.RemoveClass("paren-start");
            rows.RemoveClass("paren-end");

            rows.Children("div.l").CSS("display", rows.Length == 1 ? "none" : "block");

            rows.First().Children("div.l").Children("a.rightparen, a.andor").CSS("visibility", "hidden");

            for (int i = 1; i < rows.Length; i++)
            {
                jQueryObject row = rows.Eq(i);
                row.Children("div.l").CSS("display", "block")
                    .Children("a.lefparen, a.andor").CSS("visibility", "visible");
            }

            bool inParen = false;
            for (int i = 0; i < rows.Length; i++)
            {
                jQueryObject row = rows.Eq(i);
                jQueryObject divParen = row.Children("div.l");
                jQueryObject lp = divParen.Children("a.leftparen");
                jQueryObject rp = divParen.Children("a.rightparen");

                if (rp.HasClass("active") && inParen)
                {
                    inParen = false;
                    if (i > 0)
                        rows.Eq(i - 1).AddClass("paren-end");
                }

                if (lp.HasClass("active"))
                {
                    inParen = true;
                    if (i > 0)
                        row.AddClass("paren-start");
                }
            }
        }

        public const string PanelTemplate =
            "<div id='~_Rows' class='filter-lines'>" +
            "</div>" +
            "<div id='~_Buttons' class='buttons'>" +
                "<button id='~_AddButton' class='add'></button>" +
                "<button id='~_SearchButton' class='search'></button>" +
                "<button id='~_ResetButton' class='reset'></button>" +
            "</div>" +
            "<div style='clear: both'>" +
            "</div>";

        public const string RowTemplate =
            "<div class='filter-line'>" +
                "<a class='delete'><span></span></a>" +
                "<div class='l'>" +
                    "<a class='rightparen' href='#'>)</a>" +
                    "<a class='andor' href='#'></a>" +
                    "<a class='leftparen' href='#'>(</a>" +
                "</div>" +
                "<div class='f'>" +
                    "<input type='hidden' class='field-select'>" +
                "</div>" +
                "<div class='o'></div>" +
                "<div class='v'></div>" +
                "<div style='clear: both'></div>" +
            "</div>";
    }
}