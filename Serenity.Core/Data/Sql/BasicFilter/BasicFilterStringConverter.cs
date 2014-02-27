using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Serenity.Data
{
    public class BasicFilterStringConverter
    {
        private IGetExpressionByName _query;
        private Row _row;
        private Func<BasicFilter, BaseCriteria> _processCriteria;
        private FilterFields filterFields;

        const string AND = " AND ";
        const string OR = " OR ";
        const string LPAREN = "(";
        const string RPAREN = ")";

        public BasicFilterStringConverter(IGetExpressionByName query, 
            Row row = null, Func<BasicFilter, BaseCriteria> processCriteria = null,
            FilterFields filterFields = null)
        {
            _query = query;
            _row = row;
            _processCriteria = processCriteria;
            this.filterFields = filterFields;
            if (filterFields == null && row != null)
                filterFields = row._fields.Filters;
        }

        //protected virtual FilterFieldType GetFilterFieldType(Field field, string fieldName)
        //{
        //    // by default, suppose fields are string typed
        //    FilterFieldType type = FilterFieldType.String;
        //    // if given, determine field type by looking up in the filter field options
        //    if (field != null)
        //        type = FilterField.ToFilterFieldType(field);

        //    return type;
        //}

        protected virtual string GetFieldExpression(Field field, string fieldName)
        {
            if (field != null &&
                ((field.Flags & FieldFlags.DenyFiltering) == FieldFlags.DenyFiltering))
                throw new ArgumentOutOfRangeException("DeniedFilterField", field.Name);

            string fieldExpr = null;

            // look up in query
            if (_query != null)
                fieldExpr = _query.GetExpression(fieldName);

            // if still not found, try the field object
            if (fieldExpr == null && field != null)
            {
                // check if field has a foreign schema determine field expression
                /*if (field._joinAlias != null)
                    fieldExpr = field._joinAlias + "." + (field._expression ?? field.Name);*/
                if (field.Expression != null)
                    fieldExpr = field.Expression;
                else
                    fieldExpr = (0).TableAliasDot() + field.Name;
            }

            return fieldExpr;
        }

        public BaseCriteria Convert(BasicFilterBase node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            var sub = node as BasicFilterGroup;
            if (sub != null)
                return Convert(sub);
            else
                return Convert((BasicFilter)node);
        }

        private Double ParseDoubleValue(string valueText)
        {
            valueText = valueText.TrimToEmpty();

            double d;
            if (!Double.TryParse(valueText, NumberStyles.Float, Invariants.NumberFormat, out d))
                throw new ArgumentOutOfRangeException(String.Format("Invalid double value {0}", valueText));

            return d;
        }

        private long ParseIntegerValue(Field field, string valueText)
        {
            valueText = valueText.TrimToEmpty();

            long l;
            if (Int64.TryParse(valueText, out l))
                return l;
            else if (field != null && !valueText.IsTrimmedEmpty())
            {
                var enumType = FieldDescriptor.GetEnumType(field);
                if (enumType != null)
                    return (System.Convert.ToInt32(Enum.Parse(enumType, valueText)));
                else
                    throw new ArgumentOutOfRangeException(String.Format("Invalid integer value {0}", valueText));
            }
            else
                throw new ArgumentOutOfRangeException(String.Format("Invalid integer value {0}", valueText));
        }

        private DateTime ParseDateTimeValue(string valueText)
        {
            DateTime d;
            if (!DateHelper.TryParseISO8601DateTime(valueText, out d))
                throw new ArgumentOutOfRangeException(String.Format("Invalid date time value {0}", valueText));

            return d;
        }

        private BaseCriteria Convert(BasicFilter filter)
        {
            if (filter == null)
                throw new ArgumentNullException("criteria");

            if (!filter.IsValid)
                throw new ArgumentOutOfRangeException("InvalidFilterCriteria", filter.ToJsonString());

            if (_processCriteria != null)
            {
                var processed = _processCriteria(filter);
                if (!Object.ReferenceEquals(processed, null))
                {
                    if (processed.IsEmpty)
                        throw new ArgumentOutOfRangeException("EmptyFilterLine", filter.ToJsonString());

                    return processed;
                }
            }

            string fieldName = filter.Field;

            Field field = null;
            if (_row != null)
                field = _row.FindField(fieldName);

            // if filter fields list is specified, the fieldname must exist in this list, otherwise
            // it may be an hacking attempt, as user tries to filter a field that he is not
            // represented with
            IFilterField filterField = null;
            if (filterFields != null)
            {
                filterField = filterFields.ByNameOrTextual(fieldName);
                if (filterField == null &&
                    (field == null || field.Flags.HasFlag(FieldFlags.DenyFiltering)))
                {
                    throw new ArgumentOutOfRangeException("UnknownFilterField", fieldName);
                }
            }

            //var type = GetFilterFieldType(field, fieldName);
            var fieldExpr = GetFieldExpression(field, fieldName);
            if (fieldExpr == null)
            {
                // field is not found anywhere, don't allow unknown fields as it may cause a script injection 
                // attack or other types of security threats!
                throw new ArgumentOutOfRangeException("UnknownFilterField", filter.ToJsonString());
            }

            bool isInteger = (filterField != null && (filterField.Handler == "Integer")) ||
                (filterField == null && field != null && (field is Int16Field || field is Int32Field || field is Int64Field));

            bool isDecimal = (filterField != null && (filterField.Handler == "Decimal")) ||
                (filterField == null && field != null && (field is DoubleField || field is DecimalField));

            bool isNumeric = isInteger || isDecimal;

            bool isDateTime =
                (filterField != null && (filterField.Handler == "Date")) ||
                (filterField == null && field != null &&
                    (field is DateTimeField));

            var op = filter.Operator;

            switch (op)
            {
                case FilterOp.True:
                    return new Criteria(fieldExpr) == 1;
                case FilterOp.False:
                    return new Criteria(fieldExpr) == 0;
                case FilterOp.IsNull:
                    return new Criteria(fieldExpr).IsNull();
                case FilterOp.IsNotNull:
                    return new Criteria(fieldExpr).IsNotNull();
                case FilterOp.Like:
                    return new Criteria(fieldExpr).Like(filter.Value);
                case FilterOp.NotLike:
                    return new Criteria(fieldExpr).NotLike(filter.Value);
                case FilterOp.Contains:
                    return new Criteria(fieldExpr).Contains(filter.Value);
                case FilterOp.NotContains:
                    return new Criteria(fieldExpr).NotContains(filter.Value);
                case FilterOp.StartsWith:
                    return new Criteria(fieldExpr).StartsWith(filter.Value);
                case FilterOp.EndsWith:
                    return new Criteria(fieldExpr).EndsWith(filter.Value);
                case FilterOp.IN:
                case FilterOp.NotIN:
                    {
                        var values = new List<object>();
                        foreach (var s in filter.Values)
                        {
                            if (isDecimal)
                                values.Add(ParseDoubleValue(s));
                            else if (isInteger)
                                values.Add(ParseIntegerValue(field, s));
                            else
                                values.Add(s);
                        }

                        if (values.Count == 0)
                            throw new ArgumentOutOfRangeException("InvalidFilterLine", filter.ToJsonString());

                        if (op == FilterOp.IN)
                            return new Criteria(fieldExpr).In(values);
                        else 
                            return new Criteria(fieldExpr).NotIn(values);
                    }
            }

            // parse value1 and value2
            string value1Text = filter.Value.TrimToEmpty();
            string value2Text = filter.Value2.TrimToEmpty();

            if ((op == FilterOp.BW || op == FilterOp.NotBW))
            {
                if (value1Text.IsNullOrEmpty() || value2Text.IsNullOrEmpty())
                    throw new ArgumentOutOfRangeException("InvalidFilterLine", filter.ToJsonString());

                if (isInteger)
                    return new Criteria(fieldExpr) >= ParseIntegerValue(field, value1Text) &
                        new Criteria(fieldExpr) <= ParseIntegerValue(field, value2Text);
                else if (isDecimal)
                    return new Criteria(fieldExpr) >= ParseDoubleValue(value1Text) &
                        new Criteria(fieldExpr) <= ParseDoubleValue(value2Text);
                else if (isDateTime)
                {
                    var d1 = ParseDateTimeValue(value1Text);
                    var d2 = ParseDateTimeValue(value2Text);

                    if (d1.Date == d1 && d2.Date == d2)
                    {
                        if (op == FilterOp.BW)
                            return new Criteria(fieldExpr) >= d1.Date & new Criteria(fieldExpr) < d2.Date.AddDays(1);
                        else
                            return ~(new Criteria(fieldExpr) < d1.Date | new Criteria(fieldExpr) >= d2.Date.AddDays(1));
                    }
                    else
                    {
                        if (op == FilterOp.BW)
                            return new Criteria(fieldExpr) >= d1 & new Criteria(fieldExpr) <= d2;
                        else
                            return ~((new Criteria(fieldExpr) < d1 | new Criteria(fieldExpr) > d2));
                    }
                }
                else
                {
                    if (op == FilterOp.BW)
                        return new Criteria(fieldExpr) >= value1Text & new Criteria(fieldExpr) <= value2Text;
                    else
                        return ~((new Criteria(fieldExpr) < value2Text | new Criteria(fieldExpr) > value2Text));
                }
            }

            var result = new Criteria(fieldExpr);

            if (isInteger)
            {
                var i = ParseIntegerValue(field, value1Text);
                if (op == FilterOp.EQ)
                    return result == i;
                else if (op == FilterOp.NE)
                    return result != i;
                else if (op == FilterOp.GT)
                    return result > i;
                else if (op == FilterOp.GE)
                    return result >= i;
                else if (op == FilterOp.LT)
                    return result < i;
                else if (op == FilterOp.LE)
                    return result <= i;
            }
            else if (isDecimal)
            {
                var o = ParseIntegerValue(field, value1Text);
                if (op == FilterOp.EQ)
                    return result == o;
                else if (op == FilterOp.NE)
                    return result != o;
                else if (op == FilterOp.GT)
                    return result > o;
                else if (op == FilterOp.GE)
                    return result >= o;
                else if (op == FilterOp.LT)
                    return result < o;
                else if (op == FilterOp.LE)
                    return result <= o;
            }
            else if (isDateTime)
            {
                var d = ParseDateTimeValue(value1Text);
                if (d.Date == d)
                {
                    if (op == FilterOp.EQ)
                        return result >= d & result < d.AddDays(1);
                    else if (op == FilterOp.NE)
                        return ~(result < d | result >= d.AddDays(1));
                    else if (op == FilterOp.GT)
                        return result >= d.AddDays(1);
                    else if (op == FilterOp.GE)
                        return result >= d;
                    else if (op == FilterOp.LT)
                        return result < d;
                    else if (op == FilterOp.LE)
                        return result < d.AddDays(1);
                }
                else
                {
                    if (op == FilterOp.EQ)
                        return result == d;
                    else if (op == FilterOp.NE)
                        return result != d;
                    else if (op == FilterOp.GT)
                        return result > d;
                    else if (op == FilterOp.GE)
                        return result >= d;
                    else if (op == FilterOp.LT)
                        return result < d;
                    else if (op == FilterOp.LE)
                        return result <= d;
                }
            }
            else
            {
                if (op == FilterOp.EQ)
                    return result == value1Text;
                else if (op == FilterOp.NE)
                    return result != value1Text;
                else if (op == FilterOp.GT)
                    return result > value1Text;
                else if (op == FilterOp.GE)
                    return result >= value1Text;
                else if (op == FilterOp.LT)
                    return result < value1Text;
                else if (op == FilterOp.LE)
                    return result <= value1Text;
            }

            throw new InvalidOperationException();
        }

        public BaseCriteria Convert(BasicFilterGroup group)
        {
            if (group == null)
                throw new ArgumentNullException("group");

            if (group.Nodes.IsEmptyOrNull())
                throw new ArgumentNullException("group.Nodes");

            var result = Criteria.Empty;

            foreach (var node in group.Nodes)
            {
                var sub = Convert(node);

                if (group.Operator == LogicalOp.And ||
                    group.Operator == LogicalOp.NotAnd)
                    result &= sub;
                else
                    result |= sub;
            }

            result = ~(result);

            if (group.Operator == LogicalOp.NotOr ||
                group.Operator == LogicalOp.NotAnd)
            {
                result = ~(!result);
            }

            return result;
        }
    }
}