using Dictionary = System.Collections.Generic.Dictionary<string, object>;

namespace Serenity.Data;

/// <summary>
///   Generates queries of the form <c>UPDATE tablename SET field1 = value1, field2 = value2 ... fieldN = valueN WHERE [filter]</c>.</summary>
/// <remarks>
///   <p>To determine updated field values, Set(field, value) should be called several times.</p>
///   <p>Where expressions determines the record(s) to update.</p></remarks>   
public class SqlUpdate : QueryWithParams, ISetFieldByStatement, IFilterableQuery
{
    private string _tableName;
    private List<string> _nameValuePairs;
    private StringBuilder _where;
    private Dictionary _params;

    private void Initialize(string tableName)
    {
        if (string.IsNullOrEmpty(tableName))
            throw new ArgumentNullException("tableName");

        _tableName = tableName;
        _nameValuePairs = new List<string>();
        _where = new StringBuilder();
    }

    /// <summary>
    ///   Creates a new SqlUpdate query.</summary>
    /// <param name="tableName">
    ///   Table name (required).</param>
    public SqlUpdate(string tableName)
    {
        Initialize(tableName);
    }



    /// <summary>
    ///   Sets field value to the expression.</summary>
    /// <param name="field">
    ///   Field name (required).</param>
    /// <param name="value">
    ///   Field value (expression, required).</param>
    /// <returns>
    ///   SqlUpdate object itself.</returns>
    public SqlUpdate SetTo(string field, string value)
    {
        if (field == null || field.Length == 0)
            throw new ArgumentNullException(field);
        if (value == null || value.Length == 0)
            throw new ArgumentNullException(value);

        _nameValuePairs.Add(field);
        _nameValuePairs.Add(value);
        return this;
    }

    /// <summary>
    ///   Sets field value to the expression.</summary>
    /// <param name="field">
    ///   Field name (required).</param>
    /// <param name="value">
    ///   Field value (expression, required).</param>
    /// <returns>
    ///   SqlUpdate object itself.</returns>
    void ISetFieldByStatement.SetTo(string field, string value)
    {
        if (field == null || field.Length == 0)
            throw new ArgumentNullException(field);
        if (value == null || value.Length == 0)
            throw new ArgumentNullException(value);

        _nameValuePairs.Add(field);
        _nameValuePairs.Add(value);
    }

    /// <summary>
    ///   Sets field value to the expression.</summary>
    /// <param name="field">
    ///   Field name (required).</param>
    /// <param name="value">
    ///   Field expression (required).</param>
    /// <returns>
    ///   SqlUpdate object itself.</returns>
    public SqlUpdate SetTo(IField field, string value)
    {
        if (field == null)
            throw new ArgumentNullException("field");
        return SetTo(field.Name, value);
    }

    /// <summary>
    ///   Sets field value to NULL.</summary>
    /// <param name="field">
    ///   Field (required).</param>
    /// <returns>
    ///   SqlUpdate object itself.</returns>
    public SqlUpdate SetNull(string field)
    {
        if (field == null || field.Length == 0)
            throw new ArgumentNullException(field);

        _nameValuePairs.Add(field);
        _nameValuePairs.Add(SqlKeywords.Null);
        return this;
    }

    /// <summary>
    ///   Increases a fields value.</summary>
    /// <param name="field">
    ///   Field name (required).</param>
    /// <param name="value">
    ///   Increase amount (can be negative).</param>
    /// <returns>
    ///   SqlUpdate object itself.</returns>
    public SqlUpdate Inc(string field, int value)
    {
        return SetTo(field, field + (value >= 0 ? " + " : "") + value.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    ///   Increases a fields value.</summary>
    /// <param name="field">
    ///   Field (required).</param>
    /// <param name="value">
    ///   Increase amount (can be negative).</param>
    /// <returns>
    ///   SqlUpdate object itself.</returns>
    public SqlUpdate Inc(IField field, int value)
    {
        if (field == null)
            throw new ArgumentNullException("field");

        return Inc(field.Name, value);
    }

    /// <summary>
    ///   Decreases a fields value.</summary>
    /// <param name="field">
    ///   Field name (required).</param>
    /// <param name="value">
    ///   Decrease amount (can be negative).</param>
    /// <returns>
    ///   SqlUpdate object itself.</returns>
    public SqlUpdate Dec(string field, int value)
    {
        return Inc(field, -value);
    }

    /// <summary>
    ///   Decreases a fields value.</summary>
    /// <param name="field">
    ///   Field (required).</param>
    /// <param name="value">
    ///   Decrease amount (can be negative).</param>
    /// <returns>
    ///   SqlUpdate object itself.</returns>
    public SqlUpdate Dec(IField field, int value)
    {
        return Inc(field, -value);
    }

    /// <summary>
    ///   Adds a condition to WHERE clause of the query.</summary>
    /// <param name="condition">
    ///   Condition.</param>
    /// <returns>
    ///   SqlUpdate object itself.</returns>
    public SqlUpdate Where(string condition)
    {
        if (condition == null || condition.Length == 0)
            throw new ArgumentNullException("condition");

        condition = RemoveT0Reference(condition);

        if (_where.Length > 0)
            _where.Append(SqlKeywords.And);

        _where.Append(condition);

        return this;
    }

    /// <summary>
    /// Sets the dialect (SQL server type / version) for query.
    /// </summary>
    public SqlUpdate Dialect(ISqlDialect dialect)
    {
        this.dialect = dialect ?? throw new ArgumentNullException("dialect");
        dialectOverridden = true;

        return this;
    }

    /// <summary>
    /// Removes the t0 reference from an SQL field reference.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <returns></returns>
    public static string RemoveT0Reference(string expression)
    {
        var index = expression.IndexOf("T0.", StringComparison.OrdinalIgnoreCase);
        if (index == 0)
        {
            string rest = expression[3..];
            if (SqlSyntax.IsValidQuotedIdentifier(rest))
                return rest;
        }

        if (index >= 0)
            return T0ReferenceRemover.RemoveT0Aliases(expression);

        return expression;
    }

    /// <summary>
    ///   Adds a condition to WHERE clause of the query.</summary>
    /// <param name="condition">
    ///   Condition.</param>
    /// <returns>
    ///   SqlUpdate object itself.</returns>
    void IFilterableQuery.Where(string condition)
    {
        Where(condition);
    }

    /// <summary>
    ///   Adds conditions to WHERE clause of the query.</summary>
    /// <param name="conditions">
    ///   Condition.</param>
    /// <returns>
    ///   SqlUpdate object itself.</returns>
    public SqlUpdate Where(params string[] conditions)
    {
        if (conditions == null || conditions.Length == 0)
            throw new ArgumentNullException("conditions");

        foreach (var condition in conditions)
            Where(condition);

        return this;
    }

    /// <summary>
    ///   Clones this SqlUpdate query.</summary>
    /// <returns>
    ///   A new clone.</returns>
    public SqlUpdate Clone()
    {
        SqlUpdate clone = new SqlUpdate(_tableName);
        clone._nameValuePairs.AddRange(_nameValuePairs);
        clone._where.Append(_where.ToString());
        if (_params != null)
        {
            clone._params = new Dictionary();
            foreach (var pair in _params)
                clone._params.Add(pair.Key, pair.Value);
        }
        return clone;
    }

    /// <summary>
    ///   Gets string representation of SqlUpdate query.</summary>
    /// <returns>
    ///   String representation.</returns>
    public override string ToString()
    {
        return Format(_tableName, _where.ToString(), _nameValuePairs);
    }

    /// <summary>
    ///   Formats an SQL UPDATE statement.</summary>
    /// <param name="tableName">
    ///   Table name (required).</param>
    /// <param name="nameValuePairs">
    ///   Field name and values. Should have structure of <c>[field1, value1, field2, value2, ...., fieldN, valueN]</c>.
    ///   This array is required and must have even number of elements.</param>
    /// <param name="where">
    ///   WHERE clause (can be null).</param>
    /// <returns>
    ///   Formatted UPDATE query.</returns>
    public static string Format(string tableName, string where,
        List<string> nameValuePairs)
    {
        if (tableName == null || tableName.Length == 0)
            throw new ArgumentNullException(tableName);

        if (nameValuePairs == null)
            throw new ArgumentNullException("nameValuePairs");

        if (nameValuePairs.Count % 2 != 0)
            throw new ArgumentOutOfRangeException("nameValuePairs");

        StringBuilder sb = new StringBuilder("UPDATE ", 64 + where.Length +
            nameValuePairs.Count * 16);
        sb.Append(SqlSyntax.AutoBracketValid(tableName));
        sb.Append(" SET ");
        for (int i = 0; i < nameValuePairs.Count - 1; i += 2)
        {
            if (i > 0)
                sb.Append(", ");
            sb.Append(SqlSyntax.AutoBracket(nameValuePairs[i]));
            sb.Append(" = ");
            sb.Append(nameValuePairs[i + 1]);
        }

        if (where != null && where.Length > 0)
        {
            sb.Append(" WHERE ");
            sb.Append(where);
        }

        return sb.ToString();
    }
}