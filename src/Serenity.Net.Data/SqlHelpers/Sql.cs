namespace Serenity.Data;

/// <summary>
/// Contains SQL expression generation helpers
/// </summary>
public static partial class Sql
{
    /// <summary>
    /// Creates a SUM() expression
    /// </summary>
    /// <param name="field">The field.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">field is null or empty</exception>
    public static string Sum(string field)
    {
        if (field == null || field.Length == 0)
            throw new ArgumentNullException("field");

        return "SUM(" + field + ")";
    }

    /// <summary>
    /// Creates a SUM() expression
    /// </summary>
    /// <param name="field">The field.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">field is null or empty</exception>
    public static string Sum(IField field)
    {
        if (field == null)
            throw new ArgumentNullException("field");

        return "SUM(" + field.Expression + ")";
    }

    /// <summary>
    /// Creates a COUNT() expression
    /// </summary>
    /// <param name="field">The field.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">field is null or empty</exception>
    public static string Count(string field)
    {
        if (field == null || field.Length == 0)
            throw new ArgumentNullException("field");
        return "COUNT(" + field + ")";
    }

    /// <summary>
    /// Creates a COUNT() expression
    /// </summary>
    /// <param name="field">The field.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">field is null or empty</exception>
    public static string Count(IField field)
    {
        if (field == null)
            throw new ArgumentNullException("field");
        return "COUNT(" + field.Expression + ")";
    }

    /// <summary>
    /// Creates a COUNT() expression
    /// </summary>
    /// <param name="joinNumber">The join number.</param>
    /// <param name="field">The field.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">field is null or empty</exception>
    public static string Count(int joinNumber, string field)
    {
        if (field == null || field.Length == 0)
            throw new ArgumentNullException("field");

        return string.Format("COUNT(T{0}.{1})", joinNumber.ToString(CultureInfo.InvariantCulture), field);
    }

    /// <summary>
    /// Returns COUNT(*)
    /// </summary>
    /// <returns>COUNT(*)</returns>
    public static string Count()
    {
        return "COUNT(*)";
    }

    /// <summary>
    /// Creates a COALESCE() expression.
    /// </summary>
    /// <param name="statements">The statements.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">fields is null or empty</exception>
    public static string Coalesce(params string[] statements)
    {
        if (statements == null || statements.Length == 0)
            throw new ArgumentNullException("fields");

        StringBuilder sb = new StringBuilder("COALESCE(");
        sb.Append(statements[0]);
        for (int i = 1; i < statements.Length; i++)
        {
            sb.Append(", ");
            sb.Append(statements[i]);
        }
        sb.Append(')');
        return sb.ToString();
    }

    /// <summary>
    /// Creates a COALESCE() expression while adding values to specified query as params.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="values">The values.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">values is null or empty</exception>
    public static string Coalesce(this IQueryWithParams query, params object[] values)
    {
        if (values == null || values.Length == 0)
            throw new ArgumentNullException("values");

        StringBuilder sb = new StringBuilder("COALESCE(");

        for (var i = 0; i < values.Length; i++)
        {
            if (i > 0)
                sb.Append(", ");

            var value = values[i];
            if (value is ICriteria crit)
                crit.ToString(sb, query);
            else if (value is IQueryWithParams qprm)
                sb.Append(qprm.ToString());
            else if (value is IField fld)
                sb.Append(fld.Expression);
            else
            {
                var param = query.AutoParam();
                query.AddParam(param.Name, value);
                sb.Append(param.Name);
            }
        }

        sb.Append(")");

        return sb.ToString();
    }

    /// <summary>
    /// Creates a MIN() expression
    /// </summary>
    /// <param name="field">The field.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">field is null or empty</exception>
    public static string Min(string field)
    {
        if (field == null || field.Length == 0)
            throw new ArgumentNullException("field");

        return "MIN(" + field + ")";
    }

    /// <summary>
    /// Creates a MIN() expression
    /// </summary>
    /// <param name="field">The field.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">field is null or empty</exception>
    public static string Min(IField field)
    {
        if (field == null)
            throw new ArgumentNullException("field");

        return "MIN(" + field.Expression + ")";
    }


    /// <summary>
    /// Creates a MIN() expression.
    /// </summary>
    /// <param name="joinNumber">The join number.</param>
    /// <param name="field">The field.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">field is null or empty</exception>
    public static string Min(int joinNumber, string field)
    {
        if (field == null || field.Length == 0)
            throw new ArgumentNullException("field");

        return string.Format("MIN(T{0}.{1})", joinNumber.ToString(CultureInfo.InvariantCulture), field);
    }

    /// <summary>
    /// Creates a MAX() expression.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">field is null or empty</exception>
    public static string Max(string field)
    {
        if (field == null || field.Length == 0)
            throw new ArgumentNullException("field");

        return "MAX(" + field + ")";
    }

    /// <summary>
    /// Creates a MAX() expression.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">field is null or empty</exception>
    public static string Max(IField field)
    {
        if (field == null)
            throw new ArgumentNullException("field");

        return "MAX(" + field.Expression + ")";
    }

    /// <summary>
    /// Creates a MAX() expression.
    /// </summary>
    /// <param name="joinNumber">The join number.</param>
    /// <param name="field">The field.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">field is null or empty</exception>
    public static string Max(int joinNumber, string field)
    {
        if (field == null || field.Length == 0)
            throw new ArgumentNullException("field");

        return string.Format("MAX(T{0}.{1})", joinNumber.ToString(CultureInfo.InvariantCulture), field);
    }

    /// <summary>
    /// Creates a SUM() expression.
    /// </summary>
    /// <param name="joinNumber">The join number.</param>
    /// <param name="field">The field.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">field is null or empty.</exception>
    public static string Sum(int joinNumber, string field)
    {
        if (field == null || field.Length == 0)
            throw new ArgumentNullException("field");

        return string.Format("SUM(T{0}.{1})", joinNumber.ToString(CultureInfo.InvariantCulture), field);
    }

    /// <summary>
    /// Creates a AVG expression.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">field is null or empty</exception>
    public static string Avg(string field)
    {
        if (field == null || field.Length == 0)
            throw new ArgumentNullException("field");

        return "AVG(" + field + ")";
    }

    /// <summary>
    /// Creates a AVG expression.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">field is null or empty</exception>
    public static string Avg(IField field)
    {
        if (field == null)
            throw new ArgumentNullException("field");

        return "AVG(" + field.Expression + ")";
    }

    /// <summary>
    /// Creates a AVG() expression.
    /// </summary>
    /// <param name="joinNumber">The join number.</param>
    /// <param name="field">The field.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">field is null or empty</exception>
    public static string Avg(int joinNumber, string field)
    {
        if (field == null || field.Length == 0)
            throw new ArgumentNullException("field");

        return string.Format("AVG(T{0}.{1})", joinNumber.ToString(CultureInfo.InvariantCulture), field);
    }

    /// <summary>
    /// Creates a Convert() expression.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="field">The field.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">
    /// type or field is null or empty.
    /// </exception>
    public static string Convert(string type, string field)
    {
        if (string.IsNullOrEmpty(type))
            throw new ArgumentNullException("type");

        if (string.IsNullOrEmpty(field))
            throw new ArgumentNullException("field");

        return string.Format(" Convert({0},{1}) ", type, field);
    }

    /// <summary>
    /// Creates a SUBSTRING() expression.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <param name="startIndex">The start index.</param>
    /// <param name="endIndex">The end index.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">expression</exception>
    public static string SubString(string expression, int startIndex, int endIndex)
    {
        if (string.IsNullOrEmpty(expression))
            throw new ArgumentNullException("expression");

        return string.Format(" substring({0},{1},{2}) ", expression, startIndex, endIndex);
    }

    /// <summary>
    /// Creates a CASE() expression.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="builder">The action which will receive CASE statement builder.</param>
    /// <returns></returns>
    public static string Case(this IQueryWithParams query, Action<CaseBuilder> builder)
    {
        var cb = new CaseBuilder();
        builder(cb);
        return cb.ToString(query);
    }

    /// <summary>
    /// Builds a CASE statement.
    /// </summary>
    /// <param name="condition">Optional condition. For example for statement "CASE Field1 WHEN 1 THEN 2 END", condition would be "Field1"</param>
    /// <param name="whenThenPairs">Pairs of WHEN/THEN statements like ["A = 1", "'Result1'", "A = 2", "'Result2'"] for a case
    /// statement like CASE WHEN A = 1 THEN 'Result1' WHEN A = 2 THEN 'Result2' END.</param>
    /// <param name="elseStatement">Optional ELSE statement</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException">whenThenPairs is empty or contains odd number of elements</exception>
    public static string Case(string condition, string[] whenThenPairs, string elseStatement)
    {
        StringBuilder sb = new StringBuilder("CASE ");
        sb.Append(condition);

        if (whenThenPairs.Length == 0 ||
            whenThenPairs.Length % 2 == 1)
            throw new ArgumentOutOfRangeException("whenThenPairs");

        for (var i = 0; i < whenThenPairs.Length; i += 2)
        {
            sb.Append(" WHEN ");
            sb.Append(whenThenPairs[i]);
            sb.Append(" THEN ");
            sb.Append(whenThenPairs[i + 1]);
        }

        if (elseStatement != null)
        {
            sb.Append(" ELSE ");
            sb.Append(elseStatement);
        }

        sb.Append(" END");

        return sb.ToString();
    }

    /// <summary>
    /// CASE statement builder
    /// </summary>
    public class CaseBuilder
    {
        private readonly List<ICriteria> when;
        private readonly List<object> then;
        private object elseValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="CaseBuilder"/> class.
        /// </summary>
        public CaseBuilder()
        {
            when = new List<ICriteria>();
            then = new List<object>();
        }

        /// <summary>
        /// Adds a WHEN THEN part
        /// </summary>
        /// <param name="when">The when.</param>
        /// <param name="then">The then.</param>
        /// <returns></returns>
        public CaseBuilder WhenThen(ICriteria when, object then)
        {
            this.when.Add(when);
            this.then.Add(then);
            return this;
        }

        /// <summary>
        /// Adds a WHEN part
        /// </summary>
        /// <param name="when">The when.</param>
        /// <returns></returns>
        public CaseBuilder When(ICriteria when)
        {
            this.when.Add(when);
            return this;
        }

        /// <summary>
        /// Adds a THEN PART
        /// </summary>
        /// <param name="then">The then.</param>
        /// <returns></returns>
        public CaseBuilder Then(object then)
        {
            this.then.Add(then);
            return this;
        }

        /// <summary>
        /// Adds a ELSE part
        /// </summary>
        /// <param name="elseValue">The else value.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Internal else value is not null</exception>
        public CaseBuilder Else(object elseValue)
        {
            if (this.elseValue is not null)
                throw new InvalidOperationException();

            this.elseValue = elseValue ?? DBNull.Value;

            return this;
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// There should be at least one WHEN/THEN pair.
        /// or
        /// WHEN/THEN pairs doesn't match.
        /// </exception>
        public string ToString(IQueryWithParams query)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("CASE ");

            if (when.Count == 0)
                throw new InvalidOperationException("There should be at least one WHEN/THEN pair.");

            if (when.Count != then.Count)
                throw new InvalidOperationException("WHEN/THEN pairs doesn't match.");

            for (var i = 0; i < when.Count; i++)
            {
                sb.Append(" WHEN ");
                when[i].ToString(sb, query);
                sb.Append(" THEN ");
                var value = then[i];
                if (value is ICriteria crit)
                    crit.ToString(sb, query);
                else if (value is IQueryWithParams qprm)
                    sb.Append(qprm.ToString());
                else if (value is IField fld)
                    sb.Append(fld.Expression);
                else
                {
                    var param = query.AutoParam();
                    query.AddParam(param.Name, value);
                    sb.Append(param.Name);
                }
            }

            if (elseValue is not null)
            {
                sb.Append(" ELSE ");

                if (elseValue is ICriteria crit)
                    crit.ToString(sb, query);
                else if (elseValue is IQueryWithParams qprm)
                    sb.Append(qprm.ToString());
                else if (elseValue is IField fld)
                    sb.Append(fld.Expression);
                else
                {
                    var param = query.AutoParam();
                    query.AddParam(param.Name, elseValue);
                    sb.Append(param.Name);
                }
            }

            sb.Append(" END");

            return sb.ToString();
        }
    }
}