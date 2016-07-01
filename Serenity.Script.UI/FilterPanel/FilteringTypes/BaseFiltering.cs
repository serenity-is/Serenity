using jQueryApi;
using Serenity.Data;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    public abstract class BaseFiltering : IFiltering, IQuickFiltering
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
            return Field.Required != true;
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

        protected virtual string GetCriteriaField()
        {
            return Field.Name;
        }

        public virtual BaseCriteria GetCriteria(out string displayText)
        {
            string text;

            switch (Operator.Key)
            {
                case FilterOperators.IsTrue:
                    displayText = DisplayText(Operator);
                    return new Criteria(GetCriteriaField()) == new ValueCriteria(true);

                case FilterOperators.IsFalse:
                    displayText = DisplayText(Operator);
                    return new Criteria(GetCriteriaField()) == new ValueCriteria(false);

                case FilterOperators.IsNull:
                    displayText = DisplayText(Operator);
                    return new Criteria(GetCriteriaField()).IsNull();

                case FilterOperators.IsNotNull:
                    displayText = DisplayText(Operator);
                    return new Criteria(GetCriteriaField()).IsNotNull();

                case FilterOperators.Contains:
                    text = GetEditorText();
                    displayText = DisplayText(Operator, text);
                    return new Criteria(GetCriteriaField()).Contains(text);

                case FilterOperators.StartsWith:
                    text = GetEditorText();
                    displayText = DisplayText(Operator, text);
                    return new Criteria(GetCriteriaField()).StartsWith(text);

                case FilterOperators.EQ:
                case FilterOperators.NE:
                case FilterOperators.LT:
                case FilterOperators.LE:
                case FilterOperators.GT:
                case FilterOperators.GE:
                    text = GetEditorText();
                    displayText = DisplayText(Operator, text);
                    return new BinaryCriteria(new Criteria(GetCriteriaField()), FilterOperators.ToCriteriaOperator[Operator.Key], 
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

        protected ArgumentNullException ArgumentNull()
        {
            return new ArgumentNullException("value", Q.Text("Controls.FilterPanel.ValueRequired"));
        }

        protected virtual object ValidateEditorValue(string value)
        {
            if (value.Length == 0)
                throw ArgumentNull();

            return value;
        }

        protected virtual object GetEditorValue()
        {
            var input = Container.Find(":input")
                .Not(".select2-focusser")
                .First();

            if (input.Length != 1)
                throw new Exception(String.Format("Couldn't find input in filter container for {0}", Field.Title ?? Field.Name));

            string value;
            if (Script.IsValue(input.GetDataValue("select2")))
                value = input.Select2Get("val") as string;
            else
                value = input.GetValue();

            value = (value ?? "").Trim();

            return ValidateEditorValue(value);
        }

        protected virtual string GetEditorText()
        {
            var input = Container.Find(":input")
                .Not(".select2-focusser")
                .Not(".select2-input")
                .First();

            if (input.Length == 0)
                return Container.GetText().Trim();

            string value;
            if (Script.IsValue(input.GetDataValue("select2")))
                value = ((dynamic)input.Select2Get("data") ?? new object()).text;
            else
                value = input.GetValue();

            return value;
        }

        [IncludeGenericArguments(false)]
        public virtual void InitQuickFilter(QuickFilter<Widget, object> filter)
        {
            filter.Field = GetCriteriaField();
            filter.Type = typeof(StringEditor);
            filter.Title = GetTitle(this.Field);
            filter.Options = Q.DeepExtend<object>(new JsDictionary<string, object>(), Field.QuickFilterParams);
        }
    }
}