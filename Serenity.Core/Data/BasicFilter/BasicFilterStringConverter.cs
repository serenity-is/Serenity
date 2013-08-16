using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Serenity.Data
{
    public class BasicFilterStringConverter
    {
        private IFilterableQuery _query;
        private Row _row;
        private Func<BasicCriteria, Filter> _processCriteria;
        private FilterFields filterFields;

        const string AND = " AND ";
        const string OR = " OR ";
        const string LPAREN = "(";
        const string RPAREN = ")";

        public BasicFilterStringConverter(IFilterableQuery query, 
            Row row = null, Func<BasicCriteria, Filter> processCriteria = null,
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
                if (field._expression != null)
                    fieldExpr = field._expression;
                else
                    fieldExpr = (0).TableAliasDot() + field.Name;

                if (_query != null)
                    _query.EnsureForeignJoin(field);
            }

            return fieldExpr;
        }

        public string Convert(BasicFilter node)
        {
            StringBuilder sb = new StringBuilder();
            Convert(sb, node);
            return sb.ToString();
        }

        public void Convert(StringBuilder sb, BasicFilter node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            var sub = node as BasicFilterGroup;
            if (sub != null)
                Convert(sb, sub);
            else
                Convert(sb, (BasicCriteria)node);
        }

        private void Convert(StringBuilder sb, BasicCriteria criteria)
        {
            if (criteria == null)
                throw new ArgumentNullException("criteria");

            if (!criteria.IsValid)
                throw new ArgumentOutOfRangeException("InvalidFilterCriteria", criteria.ToJsonString());

            if (_processCriteria != null)
            {
                var filter = _processCriteria(criteria);
                if (!Object.ReferenceEquals(filter, null))
                {
                    if (filter.IsEmpty)
                        throw new ArgumentOutOfRangeException("EmptyFilterLine", criteria.ToJsonString());
                    sb.Append(filter.ToString());
                }
            }

            string fieldName = criteria.Field;

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
                throw new ArgumentOutOfRangeException("UnknownFilterField", criteria.ToJsonString());
            }

            bool isNumeric =
                (filterField != null && (filterField.Handler == "Integer" || filterField.Handler == "Decimal")) ||
                (filterField == null && field != null &&
                    (field is Int16Field || field is Int32Field || field is Int64Field || field is DoubleField || field is DecimalField));

            bool isDateTime =
                (filterField != null && (filterField.Handler == "Date")) ||
                (filterField == null && field != null &&
                    (field is DateTimeField));

            // determine expression for this filter by operator type
            FilterOp op = criteria.Operator;
            string sqlOp = FilterOpToWhereString[(int)(op)];

            switch (op)
            {
                case FilterOp.True:
                case FilterOp.False:
                case FilterOp.IsNull:
                case FilterOp.IsNotNull:
                    sb.AppendFormat(sqlOp, fieldExpr);
                    return;

                case FilterOp.Like:
                case FilterOp.NotLike:
                case FilterOp.Contains:
                case FilterOp.NotContains:
                case FilterOp.StartsWith:
                case FilterOp.EndsWith:
                    var value = criteria.Value.Replace("'", "''");
                    if (SqlSettings.IsCaseSensitive)
                    {
                        fieldExpr = "UPPER(" + fieldExpr + ")";
                        value = value.ToUpper();
                    }
                    sb.AppendFormat(sqlOp, fieldExpr, value);
                    return;

                case FilterOp.IN:
                case FilterOp.NotIN:
                    {
                        StringBuilder vs = new StringBuilder();
                        foreach (var s in criteria.Values)
                        {
                            if (isNumeric)
                            {   // parse invariant decimal value for integer and float fields
                                double d;
                                if (Double.TryParse(s, NumberStyles.Float, Invariants.NumberFormat, out d))
                                {
                                    if (vs.Length > 0)
                                        vs.Append(",");
                                    vs.Append(d.ToInvariant());
                                }
                                else
                                {
                                    if (field != null && !s.IsTrimmedEmpty())
                                    {
                                        var enumType = FieldDescriptor.GetEnumType(field);
                                        if (enumType != null)
                                            d = System.Convert.ToInt32(Enum.Parse(enumType, s));
                                    }
                                    if (vs.Length > 0)
                                        vs.Append(",");
                                    vs.Append(d.ToInvariant());
                                }
                            }
                            else
                            {
                                if (vs.Length > 0)
                                    vs.Append(",");

                                vs.Append(s.ToSql());
                            }
                        }

                        if (vs.Length == 0)
                            throw new ArgumentOutOfRangeException("InvalidFilterLine", criteria.ToJsonString());
                        else
                            sb.AppendFormat(sqlOp, fieldExpr, vs.ToString());

                        return;
                    }
            }

            // parse value1 and value2
            string value1 = "";
            string value2 = "";

            // simple loop to parse value1 and value2 in one turn
            //bool cancel = false;
            for (int phase = 0; phase <= 1; phase++)
            {
                string valueText;

                if (phase == 0)
                    valueText = criteria.Value;
                else
                    valueText = criteria.Value2;

                valueText = valueText.TrimToEmpty();

                /*
                // value must be entered
                if (valueText == null)
                    throw new ArgumentOutOfRangeException("InvalidFilterLine", criteria.ToJsonString());*/

                if (isNumeric)
                {   // parse invariant decimal value for integer and float fields
                    double d;
                    if (!Double.TryParse(valueText, NumberStyles.Float, Invariants.NumberFormat, out d))
                    {
                        if (field != null)
                        {
                            var enumType = FieldDescriptor.GetEnumType(field);
                            if (enumType == null)
                                throw new ArgumentOutOfRangeException("InvalidFilterLine", criteria.ToJsonString());
                            else
                                d = System.Convert.ToInt32(Enum.Parse(enumType, valueText));
                        }
                        else
                            throw new ArgumentOutOfRangeException("InvalidFilterLine", criteria.ToJsonString());
                    }
                    valueText = d.ToInvariant();
                }
                else if (isDateTime)
                {   // parse iso date-time string
                    DateTime d;
                    if (!DateHelper.TryParseISO8601DateTime(valueText, out d))
                        throw new ArgumentOutOfRangeException("InvalidFilterLine", criteria.ToJsonString());

                    // datetime fields requires special care for date only values

                    if (d.Date == d)
                    {
                        if (op == FilterOp.BW || op == FilterOp.NotBW)
                        {
                            if (phase == 1)
                                d = d.Date.AddDays(1).AddMilliseconds(-1);
                        }
                        else if (phase == 0)
                        {
                            if (op == FilterOp.NE || op == FilterOp.EQ)
                            {
                                value1 = d.Date.ToSql();
                                value2 = d.Date.AddDays(1).AddMilliseconds(-1).ToSql();
                                op = (op == FilterOp.NE ? FilterOp.NotBW : FilterOp.BW);
                                break;
                            }
                            else
                            {
                                if (op == FilterOp.GT)
                                {
                                    op = FilterOp.GE;
                                    d = d.Date.AddDays(1);
                                }
                                else if (op == FilterOp.LE)
                                {
                                    op = FilterOp.LT;
                                    d = d.Date.AddDays(1);
                                }
                            }
                        }
                    }

                    valueText = d.ToSql();
                }
                else // convert simple string value to sql string (duplicate quotes)
                    valueText = valueText.ToSql();

                if (phase == 0)
                    value1 = valueText;
                else
                    value2 = valueText;

                // use second phase to parse value2 if operator is BW
                if (op != FilterOp.BW && op != FilterOp.NotBW)
                    break;
            }

            // operator may have been changed (probably by datetime interval fix)
            sqlOp = FilterOpToWhereString[(int)(op)];

            // format sql operator text with values
            if (op == FilterOp.BW || op == FilterOp.NotBW)
                sb.AppendFormat(sqlOp, fieldExpr, value1, value2);
            else
                sb.AppendFormat(sqlOp, fieldExpr, value1);
        }

        public void Convert(StringBuilder sb, BasicFilterGroup group)
        {
            if (group == null)
                throw new ArgumentNullException("group");

            if (group.Nodes.IsEmptyOrNull())
                throw new ArgumentNullException("group.Nodes");

            if (group.Operator == LogicalOp.NotOr ||
                group.Operator == LogicalOp.NotAnd)
            {
                sb.Append("NOT ");
            }

            sb.Append("(");

            string separator = null;
            foreach (var node in group.Nodes)
            {
                if (separator == null)
                    separator = group.Operator == LogicalOp.And ? " AND " : " OR ";
                else
                    sb.Append(separator);

                Convert(sb, node);
            }

            sb.Append(")");
        }

        /// <summary>
        ///   Constant SQL where clause formats for each operator type</summary>
        private static readonly string[] FilterOpToWhereString =
        {
            "{0} = 1",
            "{0} = 0",
            "{0} = {1}",
            "{0} <> {1}",
            "{0} > {1}",
            "{0} >= {1}",
            "{0} < {1}",
            "{0} <= {1}",
            "({0} >= {1} AND {0} <= {2})",
            "({0} < {1} OR {0} > {2})",
            "{0} LIKE '%{1}%'",
            "{0} NOT LIKE '%{1}%'",
            "{0} LIKE '{1}%'",
            "{0} LIKE '%{1}'",
            "{0} LIKE '{1}'",
            "{0} NOT LIKE '{1}'",
            "{0} IS NULL",
            "{0} IS NOT NULL",
            "{0} IN ({1})",
            "{0} NOT IN ({1})"
        };
    }
}