using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using Newtonsoft.Json;
using Serenity.Data;

namespace Serenity.Data
{
    /// <summary>
    ///   This class corresponds to one line of filter data that is recieved from a FilterPanel.</summary>
    [JsonConverter(typeof(JsonFilterLineConverter))]
    public class FilterLine
    {
        private string _field;
        private FilterOp _op;
        private string _value;
        private string[] _values;
        private string _value2;
        private bool _leftParen;
        private bool _rightParen;
        private bool _or;

        /// <summary>
        ///   Creates a new FilterLine object.</summary>
        /// <param name="field">
        ///   Field name (required).</param>
        /// <param name="op">
        ///   Operator</param>
        public FilterLine()
        {
        }

        [JsonProperty("f")]
        /// <summary>Field name</summary>
        public string Field
        {
            get { return _field; }
            set { _field = value; }
        }

        [JsonProperty("o")]
        /// <summary>Operator</summary>
        public FilterOp Op
        {
            get { return _op; }
            set { _op = value; }
        }

        [JsonProperty("or", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [DefaultValue(false)]
        /// <summary>True if filters join operator is OR (false if it is AND)</summary>
        public bool OR
        {
            get { return _or; }
            set { _or = value; }
        }

        [JsonProperty("v")]
        /// <summary>Value</summary>
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        [JsonProperty("v2")]
        /// <summary>Value2 for between operator</summary>
        public string Value2
        {
            get { return _value2; }
            set { _value2 = value; }
        }

        [JsonProperty("l", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [DefaultValue(false)]
        /// <summary>True if this line has a left parenthesis</summary>
        public bool LeftParen
        {
            get { return _leftParen; }
            set { _leftParen = value; }
        }

        [JsonProperty("r", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [DefaultValue(false)]
        /// <summary>True if this line has a right parenthesis</summary>
        public bool RightParen
        {
            get { return _rightParen; }
            set { _rightParen = value; }
        }

        [JsonProperty("vs")]
        /// <summary>List of values for IN operator</summary>
        public string[] Values
        {
            get { return _values; }
            set { _values = value; }
        }

        [JsonIgnore]
        public bool IsValid
        {
            get
            {
                if (_field.IsEmptyOrNull())
                    return false;
                if (_op < FilterOp.True || _op > FilterOp.IsNotNull)
                    return false;

                if ((_op != FilterOp.IN && _values != null) ||
                    (_op == FilterOp.IN && (_values == null || _values.Length == 0)))
                    return false;

                return true;
            }
        }

#if !SILVERLIGHT
        /// <summary>
        ///   Converts a list of FilterLine objects to a WHERE clause usable in a SQL select query.</summary>
        /// <param name="lines">
        ///   List of FilterLine objects to convert to a WHERE clause.</param>
        /// <param name="filterFields">
        ///   Collection of FilterField objects to determine field types and check filterability of fields.
        ///   This list is usually the filter field options list that is sent to a filter panel object.
        ///   If this list is specified, for a filter to be considered valid, its fieldname must be
        ///   found in the list. If the list is given null, field name is looked up in fieldExpressions, 
        ///   query or row and field type is determined by the field object in the row.</param>
        /// <param name="fieldExpressions">
        ///   An optional dictionary of field expressions. If a field's corresponding SQL expression like
        ///   "join.field_name" is not determinable by reading in the query or the row, this list must be specified.
        ///   When specified, this list takes priority over the query and row objects to determine the field
        ///   expression.</param>
        /// <param name="query">
        ///   An optional SqlSelect query to determine field expressions. If a field's expression is not found
        ///   in "fieldExpressions" dictionary, it is looked up in query object.</param>
        /// <param name="row">
        ///   An optional Row object to determine field types and expressions.</param>
        /// <param name="process">
        ///   An optional delegate to preprocess a filter line and return a filter. It should return an empty
        ///   filter if this line should be ignored. Null if this line should be processed as usual.</param>
        /// <returns>
        ///   WHERE clause.</returns>
        /// <remarks>
        ///   Invalid filter lines are simply skipped, no error occurs.</remarks>
        public static string ToWhereString(SqlQuery query, IEnumerable<FilterLine> lines, FilterFields filterFields, 
            IDictionary<string, string> fieldExpressions = null, Row row = null, Func<FilterLine, Criteria> process = null)
        {
            if (lines == null)
                throw new ArgumentNullException("lines");

            const string AND = " AND ";
            const string OR = " OR ";
            const string LPAREN = "(";
            const string RPAREN = ")";
            //const string TRUE = "1=1";

            // build a dictionary of FilterField objects if the list is specified
            // this list is usually the filter field options list that is sent to FilterPanel object.
            /*Dictionary<string, FilterField> filterFieldDict = null;
            if (filterFields != null)
            {
                filterFieldDict = new Dictionary<string, FilterField>(StringComparer.OrdinalIgnoreCase);
                foreach (FilterField f in filterFields)
                {
                    filterFieldDict[f.GetName()] = f;

                    if (f.GetTextual() != null)
                        filterFieldDict[f.GetTextual()] = f;
                }
            }*/

            bool inParens = false;
            bool hasOr = false;

            StringBuilder sb = new StringBuilder();
            foreach (FilterLine line in lines)
            {
                if (inParens && 
                    (line._rightParen || line._leftParen))
                {
                    sb.Append(RPAREN);
                    inParens = false;
                }

                if (sb.Length > 0)
                {
                    sb.Append(line._or ? OR : AND);
                    if (line._or)
                        hasOr = true;
                }

                if (line._leftParen)
                {
                    sb.Append(LPAREN);
                    inParens = true;
                }

                if (!line.IsValid)
                {
                    throw new ArgumentOutOfRangeException("InvalidFilterLine", line.ToJsonString());
                    //sb.Append(TRUE);
                    //continue;
                }

                if (process != null)
                {
                    var filter = process(line);
                    if (!Object.ReferenceEquals(filter, null))
                    {
                        if (filter.IsEmpty)
                        {
                            //sb.Append(TRUE);
                            throw new ArgumentOutOfRangeException("EmptyFilterLine", line.ToJsonString());
                        }
                        else
                            sb.Append(filter.ToString());

                        continue;
                    }
                }

                string fieldName = line.Field;

                // if filter fields list is specified, the fieldname must exist in this list, otherwise
                // it may be an hacking attempt, as user tries to filter a field that he is not
                // represented with
                IFilterField filterField = null;
                if (filterFields != null)
                {
                    filterField = filterFields.ByNameOrTextual(fieldName);
                    if (filterField == null)
                        throw new ArgumentOutOfRangeException("UnknownFilterField", line.ToJsonString());
                    //sb.Append(TRUE);
                    //continue;
                }

                Field field = null;
                if (row != null)
                    field = row.FindField(fieldName);


                /*// by default, suppose fields are string typed
                FilterFieldType type = FilterFieldType.String;
                // if given, determine field type by looking up in the filter field options
                if (filterField != null)
                    type = filterField.GetType();
                // otherwise, determine field type by the class of field object
                else if (field != null)
                    type = FilterField.ToFilterFieldType(field);*/

                // to determine field expression, first look it up in fieldExpressions dictionary
                string fieldExpr;
                if (fieldExpressions == null || 
                    !fieldExpressions.TryGetValue(fieldName, out fieldExpr))
                    fieldExpr = null;

                if (fieldExpr == null)
                {
                    if (field != null)
                        fieldExpr = field.FilterExpression(query);
                    else if (query != null)
                        fieldExpr = query.GetExpression(fieldName);
                }

                if (fieldExpr == null)
                {
                    // field is not found anywhere, don't allow unknown fields as it may cause a script injection 
                    // attack or other types of security threats!
                    //sb.Append(TRUE);
                    throw new ArgumentOutOfRangeException("UnknownFilterField", line.ToJsonString());
                    //continue;
                }

                bool isNumeric =
                    (filterField != null && (filterField.Handler == "Integer" || filterField.Handler == "Decimal")) ||
                    (filterField == null && field != null &&
                        (field is Int16Field || field is Int32Field || field is Int64Field || field is DoubleField || field is DecimalField));

                // determine expression for this filter by operator type
                FilterOp op = line.Op;
                string sqlOp = SqlConditionOperators[(int)(op)];

                if (op == FilterOp.IN)
                {
                    StringBuilder vs = new StringBuilder();

                    var values = line.Values;
                    if (line.Values != null)
                        foreach (var s in line.Values)
                        {
                            if (isNumeric)
                            {   // parse invariant decimal value for integer and float fields
                                decimal d;
                                if (Decimal.TryParse(s, NumberStyles.Float, Invariants.NumberFormat, out d))
                                {
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
                        //sb.Append(TRUE);
                        throw new ArgumentOutOfRangeException("InvalidFilterLine", line.ToJsonString());
                    else
                        sb.AppendFormat(sqlOp, fieldExpr, vs.ToString());

                    continue;
                }

                // operator needs value if not one of "true", "false", "is null", "is not null"
                if (op.IsNeedsValue())
                {
                    // starts with and contains operators requires special care, as their sql expressions 
                    // already contains single quotes, so .ToSql() cannot be used
                    if (op.IsLike())
                    {
                        if (line.Value != null)
                            sb.AppendFormat(SqlConditionOperators[(int)(op)], 
                                fieldExpr, line.Value.Replace("'", "''"));
                        continue;
                    }

                    // parse value1 and value2
                    string value1 = "";
                    string value2 = "";

                    // simple loop to parse value1 and value2 in one turn
                    for (int phase = 0; phase <= 1; phase++)
                    {
                        string valueText;

                        if (phase == 0)
                            valueText = line.Value;
                        else
                            valueText = line.Value2;

                        valueText = valueText.TrimToNull();

                        // value must be entered
                        if (valueText == null)
                            throw new ArgumentOutOfRangeException("InvalidFilterLine", line.ToJsonString());

                        bool isDateTime =
                            (filterField != null && (filterField.Handler == "Date")) ||
                            (filterField == null && field != null &&
                                (field is DateTimeField));
                        
                        if (isNumeric)
                        {   // parse invariant decimal value for integer and float fields
                            decimal d;
                            if (!Decimal.TryParse(valueText, NumberStyles.Float, Invariants.NumberFormat, out d))
                                throw new ArgumentOutOfRangeException("InvalidFilterLine", line.ToJsonString());

                            valueText = d.ToInvariant();
                        }
                        else if (isDateTime)
                        {   // parse iso date-time string
                            DateTime d;
                            if (!DateHelper.TryParseISO8601DateTime(valueText, out d))
                                throw new ArgumentOutOfRangeException("InvalidFilterLine", line.ToJsonString());

                            DateTimeKind kind = DateTimeKind.Unspecified;
                            object dateKindObj;
                            if (filterField != null && filterField.Params != null && filterField.Params.TryGetValue("DateKind", out dateKindObj))
                            {
                                var dateKind = ((DateFilterKind)Convert.ToInt32(dateKindObj));
                                kind = dateKind == DateFilterKind.DateTimeLocal ? DateTimeKind.Local : (dateKind == DateFilterKind.DateTimeUTC ? DateTimeKind.Utc : DateTimeKind.Unspecified);
                            }
                            else if (field != null && field is DateTimeField)
                            {
                                kind = ((DateTimeField)field).DateTimeKind;
                            }

                            d = DateTimeField.ToDateTimeKind(d, kind);

                            if (op == FilterOp.BW)
                            {
                                if (phase == 1)
                                {
                                    sqlOp = "{0} >= {1} AND {0} < {2}";
                                    d = d.AddDays(1);
                                }
                            }
                            else if (phase == 0)
                            {
                                if (op == FilterOp.NE || op == FilterOp.EQ)
                                {
                                    value1 = d.ToSql();
                                    value2 = d.AddDays(1).ToSql();
                                    if (op == FilterOp.NE)
                                        sqlOp = "NOT ({0} >= {1} AND {0} < {2})";
                                    else
                                        sqlOp = "{0} >= {1} AND {0} < {2}";
                                    op = FilterOp.BW;
                                    break;
                                }
                                else
                                {
                                    if (op == FilterOp.GT)
                                    {
                                        op = FilterOp.GE;
                                        d = d.AddDays(1);
                                    }
                                    else if (op == FilterOp.LE)
                                    {
                                        op = FilterOp.LT;
                                        d = d.AddDays(1);
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
                        if (op != FilterOp.BW)
                            break;
                    }

                    // format sql operator text with values
                    if (op == FilterOp.BW)
                        sb.AppendFormat(sqlOp, fieldExpr, value1, value2);
                    else
                        sb.AppendFormat(sqlOp, fieldExpr, value1);
                }
                else
                    sb.AppendFormat(sqlOp, fieldExpr);
            }

            if (inParens)
                sb.Append(RPAREN);

            if (hasOr)
            {
                sb.Append(RPAREN);
                sb.Insert(0, LPAREN);
            }

            return sb.ToString();
        }

        /// <summary>
        ///   Parses grouping list received from a filter panel</summary>
        /// <param name="groupingJson">
        ///   JSON string with grouping list (an array of strings)</param>
        /// <param name="filterFields">
        ///   FilterFields list to check allowed fields</param>
        /// <returns>
        ///   A list of grouping fields</returns>
        public static List<string> ParseGrouping(string groupingJson, FilterFields filterFields)
        {
            var list = new List<string>();

            groupingJson = groupingJson.TrimToNull();
            if (groupingJson == null)
                return list;

            var e = JsonConvert.DeserializeObject<List<string>>(groupingJson);
            List<string> groupBy = new List<string>();
            if (e != null)
                foreach (var s in e)
                    groupBy.Add(s as string);

            if (groupBy.Count == 0)
                return list;

            /*IDictionary<string, FilterField> filterFieldDict = null;
            if (filterFields != null)
            {
                filterFieldDict = new Dictionary<string, FilterField>(StringComparer.OrdinalIgnoreCase);
                foreach (FilterField f in filterFields)
                    filterFieldDict[f.GetName()] = f;
            }*/

            HashSet<string> added = new HashSet<string>();

            foreach (var fieldName in groupBy)
            {
                if (fieldName.IsEmptyOrNull())
                    continue;

                if (filterFields != null && filterFields.ByNameOrTextual(fieldName) == null)
                    continue;

                added.Add(fieldName);
                list.Add(fieldName);
            }

            return list;
        }
#endif
        /// <summary>
        ///   Constant SQL where clause formats for each operator type</summary>
        private static readonly LocalText[] SqlConditionOperators =
        {
            "{0} = 1",
            "{0} = 0",
            "{0} LIKE '%{1}%'",
            "{0} LIKE '{1}%'",
            "{0} = {1}",
            "{0} <> {1}",
            "{0} > {1}",
            "{0} >= {1}",
            "{0} < {1}",
            "{0} <= {1}",
            "{0} >= {1} AND {0} <= {2}",
            "{0} IN ({1})",
            "{0} IS NULL",
            "{0} IS NOT NULL"
        };
    }
}