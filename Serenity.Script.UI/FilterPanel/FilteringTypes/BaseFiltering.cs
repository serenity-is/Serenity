using jQueryApi;
using Serenity.Data;
using System;
using System.Collections.Generic;

namespace Serenity
{
    public abstract class BaseFiltering : IFiltering
    {
        public PropertyItem Field { get; set; }
        public jQueryObject Container { get; set; }
        public FilterOperator Operator { get; set; }

        public abstract List<FilterOperator> GetOperators();

        protected List<FilterOperator> AppendNullableOperators(List<FilterOperator> list)
        {
            if (!IsNullable())
                return list;

            list.Add(FilterOperators.IsNotNull);
            list.Add(FilterOperators.IsNull);
            return list;
        }

        protected List<FilterOperator> AppendComparisonOperators(List<FilterOperator> list)
        {
            list.Add(FilterOperators.EQ);
            list.Add(FilterOperators.NE);
            list.Add(FilterOperators.LT);
            list.Add(FilterOperators.LE);
            list.Add(FilterOperators.GT);
            list.Add(FilterOperators.GE);
            return list;
        }

        protected virtual bool IsNullable()
        {
            return !Field.Required;
        }

        public virtual void CreateEditor()
        {
            switch (Operator.Key)
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

            throw new Exception(String.Format("Filtering '{0}' has no editor for '{1}' operator", this.GetType().Name, Operator.Key));
        }

        public virtual string OperatorFormat(FilterOperator op)
        {
            return op.Format ?? Q.TryGetText("Controls.FilterPanel.OperatorFormats." + op.Key) ?? op.Key;
        }

        protected virtual string GetTitle(PropertyItem field)
        {
            return Q.TryGetText(field.Title) ?? field.Title ?? field.Name;
        }

        protected string DisplayText(FilterOperator op, params object[] values)
        {
            if (values.Length == 0)
                return String.Format(OperatorFormat(op), GetTitle(Field));
            else if (values.Length == 1)
                return String.Format(OperatorFormat(op), GetTitle(Field), values[0]);
            else
                return String.Format(OperatorFormat(op), GetTitle(Field), values[0], values[1]);
        }

        protected virtual string GetFieldName()
        {
            return Field.Name;
        }

        public virtual BaseCriteria GetCriteria(out string displayText, ref string errorMessage)
        {
            string text;

            switch (Operator.Key)
            {
                case FilterOperators.IsTrue:
                    displayText = DisplayText(Operator);
                    return new Criteria(GetFieldName()) == new ValueCriteria(true);

                case FilterOperators.IsFalse:
                    displayText = DisplayText(Operator);
                    return new Criteria(GetFieldName()) == new ValueCriteria(false);

                case FilterOperators.IsNull:
                    displayText = DisplayText(Operator);
                    return new Criteria(GetFieldName()).IsNull();

                case FilterOperators.IsNotNull:
                    displayText = DisplayText(Operator);
                    return new Criteria(GetFieldName()).IsNotNull();

                case FilterOperators.Contains:
                    text = GetEditorText();
                    displayText = DisplayText(Operator, text);
                    return new Criteria(GetFieldName()).Contains(text);

                case FilterOperators.StartsWith:
                    text = GetEditorText();
                    displayText = DisplayText(Operator, text);
                    return new Criteria(GetFieldName()).StartsWith(text);

                case FilterOperators.EQ:
                case FilterOperators.NE:
                case FilterOperators.LT:
                case FilterOperators.LE:
                case FilterOperators.GT:
                case FilterOperators.GE:
                    text = GetEditorText();
                    displayText = DisplayText(Operator, text);
                    return new BinaryCriteria(new Criteria(GetFieldName()), FilterOperators.ToCriteriaOperator[Operator.Key], 
                        new ValueCriteria(GetEditorValue()));
            }

            throw new Exception(String.Format("Filtering '{0}' has no handler for '{1}' operator",
                this.GetType().Name, Operator.Key));
        }

        public virtual void LoadState(object state)
        {
            var input = Container.Find(":input").First();
            input.Value(state as string);
        }

        public virtual object SaveState()
        {
            switch (Operator.Key)
            {
                case FilterOperators.Contains:
                case FilterOperators.StartsWith:
                case FilterOperators.EQ:
                case FilterOperators.NE:
                case FilterOperators.LT:
                case FilterOperators.LE:
                case FilterOperators.GT:
                case FilterOperators.GE:
                    var input = Container.Find(":input").First();
                    return input.GetValue();
            }

            return null;
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