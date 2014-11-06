using jQueryApi;
using Serenity.Data;
using System;
using System.Collections.Generic;

namespace Serenity
{
    public abstract class BaseFilterHandler : IFilterHandler
    {
        protected static JsDictionary<string, string> OperatorKeyToOperator;
        
        static BaseFilterHandler()
        {
            OperatorKeyToOperator = new JsDictionary<string, string>();
            OperatorKeyToOperator[OperatorKeys.EQ] = "=";
            OperatorKeyToOperator[OperatorKeys.GT] = ">";
            OperatorKeyToOperator[OperatorKeys.GE] = ">=";
            OperatorKeyToOperator[OperatorKeys.LT] = "<";
            OperatorKeyToOperator[OperatorKeys.LE] = "<=";
        }

        public IFilterField Field { get; set; }
        public jQueryObject Container { get; set; }

        public abstract List<FilterOperator> GetOperators();

        protected List<FilterOperator> AppendNullableOperators(List<FilterOperator> list)
        {
            if (!IsNullable())
                return list;

            list.Add(OperatorKeys.IsNotNull);
            list.Add(OperatorKeys.IsNull);
            return list;
        }

        protected virtual bool IsNullable()
        {
            return !Field.NotNull;
        }

        public virtual void CreateEditor(FilterOperator op)
        {
            switch (op.Key)
            {
                case OperatorKeys.IsTrue:
                case OperatorKeys.IsFalse:
                case OperatorKeys.IsNull:
                case OperatorKeys.IsNotNull:
                    return;

                case OperatorKeys.Contains:
                case OperatorKeys.StartsWith:
                    Container.Html("<input type=\"text\"/>");
                    return;
            }

            throw new Exception(String.Format("FilterHandler '{0}' has no editor for '{1}' operator", this.GetType().Name, op.Key));
        }

        public virtual string OperatorFormat(FilterOperator op)
        {
            return op.Format ?? Q.TryGetText("Controls.FilterPanel.OperatorFormats." + op.Key) ?? op.Key;
        }

        protected string DisplayText(FilterOperator op, params object[] values)
        {
            if (values.Length == 0)
                return String.Format(OperatorFormat(op), Field.Title ?? Field.Name);
            else if (values.Length == 1)
                return String.Format(OperatorFormat(op), Field.Title ?? Field.Name, values[0]);
            else
                return String.Format(OperatorFormat(op), Field.Title ?? Field.Name, values[0], values[1]);
        }

        public virtual BaseCriteria GetCriteria(FilterOperator op, out string displayText)
        {
            string text;

            switch (op.Key)
            {
                case OperatorKeys.IsTrue:
                    displayText = DisplayText(op);
                    return new Criteria(Field.Name) == new ValueCriteria(true);

                case OperatorKeys.IsFalse:
                    displayText = DisplayText(op);
                    return new Criteria(Field.Name) == new ValueCriteria(false);

                case OperatorKeys.IsNull:
                    displayText = DisplayText(op);
                    return new Criteria(Field.Name).IsNull();

                case OperatorKeys.IsNotNull:
                    displayText = DisplayText(op);
                    return new Criteria(Field.Name).IsNotNull();

                case OperatorKeys.Contains:
                    text = GetEditorText();
                    displayText = DisplayText(op, text);
                    return new Criteria(Field.Name).Contains(text);

                case OperatorKeys.StartsWith:
                    text = GetEditorText();
                    displayText = DisplayText(op, text);
                    return new Criteria(Field.Name).StartsWith(text);

                case OperatorKeys.EQ:
                case OperatorKeys.NE:
                case OperatorKeys.LT:
                case OperatorKeys.LE:
                case OperatorKeys.GT:
                case OperatorKeys.GE:
                    text = GetEditorText();
                    displayText = DisplayText(op, text);
                    return new BinaryCriteria(new Criteria(Field.Name), OperatorKeyToOperator[op.Key], 
                        new ValueCriteria(GetEditorValue()));
            }

            throw new Exception(String.Format("FilterHandler '{0}' has no ToFilterLine handler for '{1}' operator",
                this.GetType().Name, op));
        }

        public void LoadState(object state)
        {
            throw new NotImplementedException();
        }

        public object SaveState()
        {
            throw new NotImplementedException();
        }

        protected virtual object GetEditorValue()
        {
            var input = Container.Find(":input:visible").First();
            if (input.Length != 1)
                throw new Exception(String.Format("Couldn't find input in filter container for {0}", Field.Title ?? Field.Name));

            string value = input.GetValue().Trim();
            if (value.Length == 0)
                throw new Exception(Q.Text("Controls.FilterPanel.ValueRequired"));

            return value;
        }

        protected virtual string GetEditorText()
        {
            return GetEditorValue().ToString();
        }

        //protected void BaseBetweenToFilterLine(FilterLineEx line)
        //{
        //    Query input = container.children(":input");
        //    if (input.length != 2)
        //        throw new Exception(String.Format("Expected two inputs in filter container for {0}",
        //            line.Field));

        //    string value1 = input.eq(0).Val().Trim();
        //    string value2 = input.eq(1).Val().Trim();

        //    if (value1.Length == 0 &&
        //        value2.Length == 0)
        //    {
        //        line.ValidationError = FilterPanelTexts.Get("ValueRequired");
        //        return;
        //    }

        //    if (value1.Length != 0 &&
        //        value2.Length != 0)
        //    {
        //        line.Value = value1;
        //        line.Value2 = value2;

        //        if (input.eq(0).Is("select"))
        //        {
        //            value1 = input.eq(0).children("option:selected").Text().Trim();
        //            value2 = input.eq(1).children("option:selected").Text().Trim();
        //        }

        //        line.DisplayText = DisplayTextTwoValues(line, value1, value2);
        //    }
        //    else if (value2.Length == 0)
        //    {
        //        line.Operator = FilterOp.GE;
        //        line.Value = value1;

        //        if (input.eq(0).Is("select"))
        //            value1 = input.eq(0).children("option:selected").Text().Trim();

        //        line.DisplayText = DisplayTextOneValue(line, value1);
        //    }
        //    else
        //    {
        //        line.Operator = FilterOp.LE;
        //        line.Value = value2;

        //        if (input.eq(1).Is("select"))
        //            value2 = input.eq(1).children("option:selected").Text().Trim();

        //        line.DisplayText = DisplayTextOneValue(line, value2);
        //    }
        //}

        //protected void BaseInToFilterLine(FilterLineEx line)
        //{
        //    Query select = container.children("select").first();
        //    if (select.length != 1 || select.Attr("multiple") == null)
        //        throw new Exception(String.Format("Expected one multiple SELECT in filter container for {0}",
        //            line.Field));

        //    List<string> values = (List<string>)(object)select.Val();
        //    if (values == null || values.Count == 0)
        //    {
        //        line.ValidationError = FilterPanelTexts.Get("ValueRequired");
        //        return;
        //    }

        //    line.Values = values;

        //    string selectedText = "";
        //    select.children("option:selected").each(delegate(int index, System.Html.Element el)
        //    {
        //        if (selectedText.Length > 0)
        //            selectedText += ", ";

        //        selectedText += J.Query(el).Text();
        //    });

        //    line.DisplayText = DisplayTextOneValue(line, selectedText);
        //}

        //protected void BaseAutoCompleteToFilterLine(FilterLineEx line,
        //    Func<Query, object> getKeyValue, Func<Query, string> getText)
        //{
        //    Query input = container.children("input");
        //    if (input.length != 1)
        //        throw new Exception(String.Format("Unexpected more than one input in filter container for {0}",
        //            line.Field));

        //    input.ntAutoComplete("applyChanges");

        //    object value;
        //    if (getKeyValue != null)
        //        value = getKeyValue(input);
        //    else
        //        value = input.Data("key");

        //    if (value == null)
        //    {
        //        line.ValidationError = FilterPanelTexts.Get("ValueRequired");
        //        return;
        //    }

        //    line.Value = value.ToString();

        //    string text;
        //    if (getText != null)
        //        text = getText(input);
        //    else
        //        text = input.Val();

        //    line.DisplayText = DisplayTextOneValue(line, text);
        //}

        //protected void BaseLinkDropDownToFilterLine(FilterLineEx line,
        //    Func<Query, object> getKeyValue, Func<Query, string> getText)
        //{
        //    Query anchor = container.children("a");
        //    if (anchor.length != 1)
        //        throw new Exception(String.Format("Unexpected more than one anchor in filter container for {0}",
        //            line.Field));

        //    object value;
        //    if (getKeyValue != null)
        //        value = getKeyValue(anchor);
        //    else
        //        value = Nt.toId(anchor.NtLinkDropDown("val"));

        //    if (value == null)
        //    {
        //        line.ValidationError = FilterPanelTexts.Get("ValueRequired");
        //        return;
        //    }

        //    line.Value = value.ToString();

        //    string text;
        //    if (getText != null)
        //        text = getText(anchor);
        //    else
        //    {
        //        text = anchor.Text();

        //        string pathText = anchor.children(".link-text").children(".link-path").Text();
        //        if (pathText != null && pathText.Length > 0)
        //        {
        //            text = text.Substr(0, text.Length - pathText.Length);
        //            text += " " + pathText;
        //        }
        //    }

        //    line.DisplayText = DisplayTextOneValue(line, text);
        //}
    }
}