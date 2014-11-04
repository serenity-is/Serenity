using jQueryApi;
using System;
using System.Collections.Generic;
using System.Html;

namespace Serenity
{
    public class FilterField
    {
        public string Name;
        public string Title;
        public string Handler;
    }

    public interface IFilterHandler
    {
        List<string> GetOperators();
        string OperatorTitle(string op);
        string OperatorFormat(string op);
        void CreateEditor(string op);
        void ToFilterLine(FilterLine line);
    }

    public class FilterLine
    {
        public string Field;
        public string Title;
        public string Operator;
        public bool IsOr;
        public bool LeftParen;
        public bool RightParen;
        public string ValidationError;
        public string Value;
        public string Value2;
        public List<string> Values;
        public string DisplayText;
    }

    public class FilterPanelOptions
    {
        public List<FilterField> Fields;
        public Action<List<FilterLine>> FilterChange;
    }

    public class FilterPanel : TemplatedWidget<FilterPanelOptions>
    {
        private jQueryObject rowsDiv;
        private JsDictionary<string, FilterField> fieldByName;
        private List<FilterLine> currentFilter;

        public FilterPanel(jQueryObject div, FilterPanelOptions opt)
            : base(div, opt)
        {
            fieldByName = new JsDictionary<string, FilterField>();

            this.Element.AddClass("s-FilterPanel");
            rowsDiv = this.ById("Rows");

            InitButtons();
            InitFieldByName();
            BindSearchToEnterKey();

            UpdateButtons();
        }

        private void InitFieldByName()
        {
            if (options.Fields != null)
                foreach (FilterField field in options.Fields)
                    fieldByName[field.Name] = field;
        }

        private void InitButtons()
        {
            this.ById("AddButton").Text(Q.Text("Controls.FilterPanel.AddFilter")).Click(AddButtonClick);
            this.ById("SearchButton").Text(Q.Text("Controls.FilterPanel.SearchButton")).Click(SearchButtonClick);
            this.ById("ResetButton").Text(Q.Text("Controls.FilterPanel.ResetButton")).Click(ResetButtonClick);
        }

        private void BindSearchToEnterKey()
        {
            this.Element.Bind("keypress", delegate(jQueryEvent e)
            {
                if (e.Which != 13)
                    return;

                if (rowsDiv.Children().Length == 0)
                    return;

                Search();
            });
        }

        private void AddButtonClick(jQueryEvent e)
        {
            e.PreventDefault();

            AddEmptyRow();
        }

        private jQueryObject FindEmptyRow()
        {
            jQueryObject result = null;

            rowsDiv.Children().Each(delegate(int index, System.Html.Element row)
            {
                jQueryObject fieldSelect = J(row).Children("div.f").Children("select");
                if (fieldSelect.Length == 0)
                    return true;

                string val = fieldSelect.GetValue();
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

            jQueryObject fieldSel = row.Children("div.f").Children("select");

            fieldSel.Change(OnRowFieldChange);

            PopulateFieldList(fieldSel);

            UpdateParens();
            UpdateButtons();
            OnHeightChange();

            fieldSel.Focus();

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

        private void PopulateFieldList(jQueryObject select)
        {
            if (select.Length == 0)
                return;

            SelectElement sel = (SelectElement)select[0];

            Q.AddOption(select, "", Q.Text("Controls.FilterPanel.SelectField"));
            List<FilterField> fields = options.Fields;
            if (fields != null)
            {
                foreach (FilterField field in fields)
                    Q.AddOption(select, field.Name, field.Title ?? field.Name);
            }
        }

        private void RemoveFilterHandler(jQueryObject row)
        {
            row.Data("FilterHandler", null);
            row.Data("FilterHandlerField", null);
        }

        private FilterField GetFieldFor(jQueryObject row)
        {
            if (row.Length == 0)
                return null;

            string fieldName = row.Children("div.f").Children("select").GetValue();
            if (fieldName == null || fieldName == "")
                return null;

            FilterField field = fieldByName[fieldName];
            return field;
        }

        private IFilterHandler GetFilterHandlerFor(jQueryObject row)
        {
            FilterField field = GetFieldFor(row);
            if (field == null)
                return null;

            IFilterHandler handler = (IFilterHandler)row.GetDataValue("FilterHandler");
            string handlerField = row.GetDataValue("FilterHandlerField").As<string>();

            if (handler != null)
            {
                if (handlerField != field.Name)
                {
                    row.Data("FilterHandler", null);
                    handler = null;
                }
                else
                {
                    return handler;
                }
            }

            Type handlerType = Type.GetType("Sinerji." + (field.Handler ?? "??") + "FilterHandler");
            if (handlerType == null)
                throw new Exception(String.Format("FilterHandler type Sinerji.{0}FilterHandler is not defined!", field.Handler));

            jQueryObject editorDiv = row.Children("div.v");

            handler = (IFilterHandler)Activator.CreateInstance(handlerType, editorDiv, field);

            return handler;
        }

        private void PopulateOperatorList(jQueryObject select)
        {
            jQueryObject row = select.Closest("div.row");

            IFilterHandler handler = GetFilterHandlerFor(row);
            if (handler == null)
                return;

            List<string> operators = handler.GetOperators();

            SelectElement sel = (SelectElement)select[0];

            if (operators != null)
                foreach (string op in operators)
                    Q.AddOption(select, op, handler.OperatorTitle(op));
        }

        private void OnRowOperatorChange(jQueryEvent e)
        {
            jQueryObject row = J(e.Target).Closest("div.row");
            RowOperatorChange(row);
            jQueryObject firstInput = row.Children("div.v").Find(":input:visible").First();
            try { firstInput.Focus(); }
            catch { };
        }

        private void RowFieldChange(jQueryObject row)
        {
            jQueryObject select = row.Children("div.f").Children("select");
            string fieldName = select.GetValue();

            bool isEmpty = (fieldName == null || fieldName == "");

            // if a field is selected and first option is "---please select---", remove it
            if (!isEmpty)
            {
                OptionElement firstOption = (OptionElement)select.Children("option").First()[0];
                if (firstOption.Value == null || firstOption.Value == "")
                    J(firstOption).Remove();
            }

            jQueryObject opDiv = row.Children("div.o");
            jQueryObject opSelect = opDiv.Children("select");

            if (opSelect.Length == 0)
                opSelect = J("<select/>").AppendTo(opDiv).Change(OnRowOperatorChange);
            else
                Q.ClearOptions(opSelect);

            RemoveFilterHandler(row);

            PopulateOperatorList(opSelect);

            RowOperatorChange(row);
            UpdateParens();
            UpdateButtons();
        }

        private void RowOperatorChange(jQueryObject row)
        {
            if (row.Length == 0)
                return;

            jQueryObject editorDiv = row.Children("div.v");

            editorDiv.Html("");

            IFilterHandler handler = GetFilterHandlerFor(row);

            if (handler == null)
                return;

            string op = row.Children("div.o").Children("select").GetValue();
            if (op == null || op == "")
                return;

            handler.CreateEditor(op);
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
            OnHeightChange();
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

        private void UpdateButtons()
        {
            this.ById("SearchButton").Toggle(rowsDiv.Children().Length >= 1);
            this.ById("ResetButton").Toggle(rowsDiv.Children().Length >= 1 ||
                this.currentFilter != null);
        }

        private void OnHeightChange()
        {
            //if (Options.HeightChange != null)
            //    Options.HeightChange();
        }

        private void SearchButtonClick(jQueryEvent e)
        {
            e.PreventDefault();

            Search();
        }

        private void Search()
        {
            List<FilterLine> filterLines = new List<FilterLine>();
            string filterText = "";
            string errorText = null;
            jQueryObject row = null;

            this.rowsDiv.Children().Children("div.v").Children("span.error").Remove();

            bool inParens = false;

            for (int i = 0; i < rowsDiv.Children().Length; i++)
            {
                row = rowsDiv.Children().Eq(i);

                IFilterHandler handler = GetFilterHandlerFor(row);
                if (handler == null)
                    continue;

                FilterField field = GetFieldFor(row);

                string op = row.Children("div.o").Children("select").GetValue();
                if (op == null || op.Length == 0)
                {
                    errorText = Q.Text("Controls.FilterPanel.InvalidOperator");
                    break;
                }

                FilterLine lineEx = new FilterLine();
                lineEx.Field = field.Name;
                lineEx.Title = field.Title ?? field.Name;
                lineEx.Operator = op;
                lineEx.IsOr = row.Children("div.l").Children("a.andor").HasClass("or");
                lineEx.LeftParen = row.Children("div.l").Children("a.leftparen").HasClass("active");
                lineEx.RightParen = row.Children("div.l").Children("a.rightparen").HasClass("active");

                handler.ToFilterLine(lineEx);

                if (lineEx.ValidationError != null)
                {
                    errorText = lineEx.ValidationError;
                    break;
                }

                FilterLine line = new FilterLine();
                line.Field = lineEx.Field;
                line.Operator = lineEx.Operator;

                if (Script.IsValue(lineEx.Value))
                    line.Value = lineEx.Value;

                if (Script.IsValue(lineEx.Value2))
                    line.Value2 = lineEx.Value2;

                if (Script.IsValue(lineEx.Values) &&
                    lineEx.Values.Count > 0)
                    line.Values = lineEx.Values;

                if (lineEx.LeftParen)
                    line.LeftParen = 1.As<bool>();

                if (lineEx.RightParen)
                    line.RightParen = 1.As<bool>();

                if (lineEx.IsOr)
                    line.IsOr = 1.As<bool>();

                filterLines.Add(line);

                if (inParens && (lineEx.RightParen || lineEx.LeftParen))
                {
                    filterText += ")";
                    inParens = false;
                }

                if (filterText.Length > 0)
                    filterText += " " + Q.Text("Controls.FilterPanel." + (lineEx.IsOr ? "Or" : "And")) + " ";

                if (lineEx.LeftParen)
                {
                    filterText += "(";
                    inParens = true;
                }

                filterText += lineEx.DisplayText;
            }

            // if an error occured, display it, otherwise set current filters
            if (errorText != null)
            {
                J("<span/>").AddClass("error").Text(errorText).AppendTo(row.Children("div.v"));
                row.Children("div.v").Find("input:first").Focus();
                return;
            }

            if (filterLines.Count == 0)
                this.SetCurrentFilter(null, null);
            else
                this.SetCurrentFilter(filterLines, filterText);
        }

        private void ResetButtonClick(jQueryEvent e)
        {
            e.PreventDefault();

            rowsDiv.Empty();
            SetCurrentFilter(null, null);
            UpdateParens();
            UpdateButtons();
            OnHeightChange();
        }

        private void SetCurrentFilter(List<FilterLine> value, string text)
        {
            if (Q.ToJson(value) != Q.ToJson(currentFilter))
            {
                if (value != null)
                {
                    this.currentFilter = value;
                    text = String.Format(Q.Text("Controls.FilterPanel.CurrentFilter"), text);
                    this.ById("DisplayText").Text(text).Show();
                }
                else
                {
                    currentFilter = null;
                    this.ById("DisplayText").Text("").Hide();
                }

                UpdateParens();
                UpdateButtons();
                OnFilterChange();
                OnHeightChange();
            }
        }

        public List<FilterLine> CurrentFilter
        {
            get { return currentFilter; }
        }

        public const string PanelTemplate =
            "<div id=\"~_Rows\" class=\"rows\">" +
            "</div>" +
            "<div id=\"~_Buttons\" class=\"buttons\">" +
                "<button id=\"~_AddButton\" class=\"add\"></button>" +
                "<button id=\"~_SearchButton\" class=\"search\"></button>" +
                "<button id=\"~_ResetButton\" class=\"reset\"></button>" +
            "</div>" +
            "<div style=\"clear: both\">" +
            "</div>" +
            "<div id=\"~_DisplayText\" class=\"display\" style=\"display: none;\">" +
            "</div>";

        public const string RowTemplate =
            "<div class=\"row\">" +
                "<a class=\"delete\"><span></span></a>" +
                "<div class=\"l\">" +
                    "<a class=\"rightparen\" href=\"#\">)</a>" +
                    "<a class=\"andor\" href=\"#\"></a>" +
                    "<a class=\"leftparen\" href=\"#\">(</a>" +
                "</div>" +
                "<div class=\"f\">" +
                    "<select></select>" +
                "</div>" +
                "<div class=\"o\"></div>" +
                "<div class=\"v\"></div>" +
                "<div style=\"clear: both\"></div>" +
            "</div>";

        protected override string GetTemplate()
        {
            return PanelTemplate;
        }

        public void OnFilterChange()
        {
            if (options.FilterChange != null)
                options.FilterChange(this.currentFilter);
        }
    }
}