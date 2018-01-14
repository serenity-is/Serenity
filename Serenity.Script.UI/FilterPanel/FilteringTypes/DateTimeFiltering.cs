using jQueryApi;
using Serenity.Data;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported(ObeysTypeSystem = true)]
    public class DateTimeFiltering : BaseEditorFiltering
    {
        public DateTimeFiltering()
            : base(typeof(DateEditor))
        {
        }

        public override List<FilterOperator> GetOperators()
        {
            return AppendNullableOperators(
                AppendComparisonOperators(
                    new List<FilterOperator>()));
        }

        public override CriteriaWithText GetCriteria()
        {
            var result = new CriteriaWithText();

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
                        result.DisplayText = DisplayText(Operator, text);
                        var date = Q.ParseISODateTime((string)GetEditorValue());
                        date = new JsDate(date.GetFullYear(), date.GetMonth(), date.GetDate());
                        var next = new JsDate(date.GetFullYear(), date.GetMonth(), date.GetDate() + 1);
                        var criteria = new Criteria(GetCriteriaField());
                        var dateValue = new ValueCriteria(Q.FormatDate(date, "yyyy-MM-dd"));
                        var nextValue = new ValueCriteria(Q.FormatDate(next, "yyyy-MM-dd"));

                        switch (Operator.Key)
                        {
                            case FilterOperators.EQ:
                                result.Criteria = criteria >= dateValue & criteria < nextValue;
                                return result;

                            case FilterOperators.NE:
                                result.Criteria = ~(criteria < dateValue | criteria > nextValue);
                                return result;

                            case FilterOperators.LT:
                                result.Criteria = criteria < dateValue;
                                return result;

                            case FilterOperators.LE:
                                result.Criteria = criteria < nextValue;
                                return result;

                            case FilterOperators.GT:
                                result.Criteria = criteria >= nextValue;
                                return result;

                            case FilterOperators.GE:
                                result.Criteria = criteria >= dateValue;
                                return result;
                        }
                    }
                    break;
            }

            return base.GetCriteria();
        }
    }
}