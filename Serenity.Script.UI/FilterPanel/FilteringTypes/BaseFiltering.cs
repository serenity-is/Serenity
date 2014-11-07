using jQueryApi;
using Serenity.Data;
using System;
using System.Collections.Generic;

namespace Serenity
{
    public abstract class BaseFiltering : IFiltering
    {
        public IFilterField Field { get; set; }
        public jQueryObject Container { get; set; }

        public abstract List<FilterOperator> GetOperators();

        protected List<FilterOperator> AppendNullableOperators(List<FilterOperator> list)
        {
            if (!IsNullable())
                return list;

            list.Add(FilterOperators.IsNotNull);
            list.Add(FilterOperators.IsNull);
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
                case FilterOperators.IsTrue:
                case FilterOperators.IsFalse:
                case FilterOperators.IsNull:
                case FilterOperators.IsNotNull:
                    return;

                case FilterOperators.Contains:
                case FilterOperators.StartsWith:
                case FilterOperators.EQ:
                case FilterOperators.NE:
                case FilterOperators.LT:
                case FilterOperators.LE:
                case FilterOperators.GT:
                case FilterOperators.GE:
                    Container.Html("<input type=\"text\"/>");
                    return;
            }

            throw new Exception(String.Format("Filtering '{0}' has no editor for '{1}' operator", this.GetType().Name, op.Key));
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
                case FilterOperators.IsTrue:
                    displayText = DisplayText(op);
                    return new Criteria(Field.Name) == new ValueCriteria(true);

                case FilterOperators.IsFalse:
                    displayText = DisplayText(op);
                    return new Criteria(Field.Name) == new ValueCriteria(false);

                case FilterOperators.IsNull:
                    displayText = DisplayText(op);
                    return new Criteria(Field.Name).IsNull();

                case FilterOperators.IsNotNull:
                    displayText = DisplayText(op);
                    return new Criteria(Field.Name).IsNotNull();

                case FilterOperators.Contains:
                    text = GetEditorText();
                    displayText = DisplayText(op, text);
                    return new Criteria(Field.Name).Contains(text);

                case FilterOperators.StartsWith:
                    text = GetEditorText();
                    displayText = DisplayText(op, text);
                    return new Criteria(Field.Name).StartsWith(text);

                case FilterOperators.EQ:
                case FilterOperators.NE:
                case FilterOperators.LT:
                case FilterOperators.LE:
                case FilterOperators.GT:
                case FilterOperators.GE:
                    text = GetEditorText();
                    displayText = DisplayText(op, text);
                    return new BinaryCriteria(new Criteria(Field.Name), FilterOperators.ToCriteriaOperator[op.Key], 
                        new ValueCriteria(GetEditorValue()));
            }

            Q.Log(op);

            throw new Exception(String.Format("Filtering '{0}' has no handler for '{1}' operator",
                this.GetType().Name, op));
        }

        public void LoadState(object state)
        {
            var input = Container.Find(":input").First();
            input.Value(state as string);
        }

        public object SaveState()
        {
            var input = Container.Find(":input").First();
            return input.GetValue();
        }

        protected virtual object GetEditorValue()
        {
            var input = Container.Find(":input").First();
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
    }
}