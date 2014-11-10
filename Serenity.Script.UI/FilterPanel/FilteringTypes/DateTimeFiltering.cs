using jQueryApi;
using Serenity.Data;
using System;
using System.Collections.Generic;

namespace Serenity
{
    public class DateTimeFiltering : BaseEditorFiltering<DateEditor>
    {
        public override List<FilterOperator> GetOperators()
        {
            return AppendNullableOperators(
                AppendComparisonOperators(
                    new List<FilterOperator>()));
        }

        public override BaseCriteria GetCriteria(out string displayText)
        {
            switch (Operator.Key)
            {
                case FilterOperators.EQ:
                case FilterOperators.NE:
                case FilterOperators.LT:
                case FilterOperators.LE:
                case FilterOperators.GT:
                case FilterOperators.GE:
                    {
                        var text = GetEditorText();
                        displayText = DisplayText(Operator, text);
                        var date = Q.ParseISODateTime((string)GetEditorValue());
                        date = new JsDate(date.GetFullYear(), date.GetMonth(), date.GetDate());
                        var next = new JsDate(date.GetFullYear(), date.GetMonth(), date.GetDate() + 1);
                        var criteria = new Criteria(GetCriteriaField());
                        var dateValue = new ValueCriteria(Q.FormatDate(date, "yyyy-MM-dd"));
                        var nextValue = new ValueCriteria(Q.FormatDate(next, "yyyy-MM-dd"));

                        switch (Operator.Key)
                        {
                            case FilterOperators.EQ:
                                return criteria >= dateValue & criteria < nextValue;

                            case FilterOperators.NE:
                                return ~(criteria < dateValue | criteria > nextValue);

                            case FilterOperators.LT:
                                return criteria < dateValue;

                            case FilterOperators.LE:
                                return criteria < nextValue;

                            case FilterOperators.GT:
                                return criteria >= nextValue;

                            case FilterOperators.GE:
                                return criteria >= dateValue;
                        }
                    }
                    break;
            }

            return base.GetCriteria(out displayText);
        }
    }
}