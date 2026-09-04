namespace Serenity.Data;

/// <summary>
///   Generates queries of the form <c>UPDATE tablename SET field1 = value1, field2 = value2 ... fieldN = valueN WHERE [filter]</c>.</summary>
/// <remarks>
///   <p>To determine updated field values, Set(field, value) should be called several times.</p>
///   <p>Where expressions determines the record(s) to update.</p></remarks>   
public class SqlUpdate : QueryWithParams, ISetFieldByStatement, IFilterableQuery
{
    private readonly string tableName;
    private readonly List<FieldExpressionPair> fieldExpressions = [];
    private readonly List<string> where = [];

    /// <summary>
    ///   Creates a new SqlUpdate query.</summary>
    /// <param name="tableName">
    ///   Table name (required).</param>
    public SqlUpdate(string tableName)
    {
        if (string.IsNullOrEmpty(tableName))
            throw new ArgumentNullException(nameof(tableName));

        this.tableName = tableName;
    }

    /// <summary>
    ///   Gets the table name.</summary>
    /// <returns>The table name.</returns>
    public string TableName()
    {
        return tableName;
    }

    /// <summary>
    ///   Returns field and value expression pairs.</summary>
    /// <returns>The list of field and value expression pairs.</returns>
    public IReadOnlyList<FieldExpressionPair> GetFieldExpressions()
    {
        return fieldExpressions;
    }

    /// <summary>
    ///   Returns the WHERE conditions.</summary>
    /// <returns>The list of WHERE conditions.</returns>
    public IReadOnlyList<string> GetWhereConditions()
    {
        return where;
    }

    /// <summary>
    ///   Returns the WHERE clause (excluding WHERE keyword).</summary>
    /// <returns>The WHERE clause, or an empty string if there are no conditions.</returns>
    public string GetWhereClause()
    {
        return string.Join(SqlKeywords.And, where);
    }

    /// <summary>
    ///   Sets field value to the expression.</summary>
    /// <param name="field">
    ///   Field name (required).</param>
    /// <param name="expression">
    ///   Field expression, required.</param>
    /// <returns>
    ///   SqlUpdate object itself.</returns>
    /// <exception cref="ArgumentNullException">field or expression is null or empty.</exception>
    public SqlUpdate SetTo(string field, string expression)
    {
        if (field == null || field.Length == 0)
            throw new ArgumentNullException(field);
        if (expression == null || expression.Length == 0)
            throw new ArgumentNullException(expression);

        fieldExpressions.Add(new FieldExpressionPair(field, expression));
        return this;
    }

    /// <summary>
    ///   Sets field value to the expression.</summary>
    /// <param name="field">
    ///   Field name (required).</param>
    /// <param name="expression">
    ///   Field expression, required.</param>
    /// <returns>
    ///   SqlUpdate object itself.</returns>
    void ISetFieldByStatement.SetTo(string field, string expression)
    {
        if (field == null || field.Length == 0)
            throw new ArgumentNullException(field);
        if (expression == null || expression.Length == 0)
            throw new ArgumentNullException(expression);

        fieldExpressions.Add(new FieldExpressionPair(field, expression));
    }

    /// <summary>
    ///   Sets field value to the expression.</summary>
    /// <param name="field">
    ///   Field name (required).</param>
    /// <param name="expression">
    ///   Field expression (required).</param>
    /// <returns>
    ///   SqlUpdate object itself.</returns>
    public SqlUpdate SetTo(IField field, string expression)
    {
        ArgumentNullException.ThrowIfNull(field);
        return SetTo(field.Name, expression);
    }

    /// <summary>
    ///   Sets field value to NULL.</summary>
    /// <param name="field">
    ///   Field (required).</param>
    /// <returns>
    ///   SqlUpdate object itself.</returns>
    /// <exception cref="ArgumentNullException">field is null or empty.</exception>
    public SqlUpdate SetNull(string field)
    {
        if (field == null || field.Length == 0)
            throw new ArgumentNullException(field);

        fieldExpressions.Add(new FieldExpressionPair(field, SqlKeywords.Null));
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
        ArgumentNullException.ThrowIfNull(field);

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
    /// <exception cref="ArgumentNullException">condition is null or empty.</exception>
    public SqlUpdate Where(string condition)
    {
        if (condition == null || condition.Length == 0)
            throw new ArgumentNullException(nameof(condition));

        where.Add(RemoveT0Reference(condition));

        return this;
    }

    /// <summary>
    /// Sets the dialect (SQL server type / version) for query.
    /// </summary>
    /// <param name="dialect">The dialect to use.</param>
    /// <returns>The SqlUpdate object itself.</returns>
    /// <exception cref="ArgumentNullException">dialect is null.</exception>
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
    /// <returns>The expression with the T0 reference removed.</returns>
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
    ///   Clones this SqlUpdate query.</summary>
    /// <returns>
    ///   A new clone.</returns>
    public SqlUpdate Clone()
    {
        SqlUpdate clone = new(tableName);
        clone.fieldExpressions.AddRange(fieldExpressions);
        clone.where.AddRange(where);
        CloneParams(clone);
        return clone;
    }

    /// <summary>
    ///   Gets string representation of SqlUpdate query.</summary>
    /// <returns>
    ///   String representation.</returns>
    public override string ToString()
    {
        return Format(tableName, GetWhereClause(), fieldExpressions, dialect);
    }

    /// <summary>
    /// Formats an SQL UPDATE statement.</summary>
    /// <param name="tableName">Table name (required).</param>
    /// <param name="fieldExpressions">Field names and values in the form of [field1, value1, field2, value2, ..., fieldN, valueN].</param>
    /// <param name="where">WHERE clause (can be null).</param>
    /// <param name="dialect">Target dialect</param>
    /// <returns>Formatted UPDATE query.</returns>
    /// <exception cref="ArgumentNullException">fieldExpressions is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">fieldExpressions has an odd number of elements.</exception>
    [Obsolete("Use overload with IEnumerable<FieldExpressionPair>")]
    public static string Format(string tableName, string where,
        List<string> fieldExpressions, ISqlDialect dialect = null)
    {
        ArgumentNullException.ThrowIfNull(fieldExpressions);

        if (fieldExpressions.Count % 2 != 0)
            throw new ArgumentOutOfRangeException(nameof(fieldExpressions));

        var list = new List<FieldExpressionPair>(fieldExpressions.Count / 2);
        for (var i = 0; i < fieldExpressions.Count; i += 2)
            list.Add(new FieldExpressionPair(fieldExpressions[i], fieldExpressions[i + 1]));
        return Format(tableName, where, list, dialect);
    }

    /// <summary>
    /// Formats an SQL UPDATE statement.</summary>
    /// <param name="tableName">Table name (required).</param>
    /// <param name="fieldExpressions">Field names and their value expressions.</param>
    /// <param name="where">WHERE clause (can be null).</param>
    /// <param name="dialect">Target dialect</param>
    /// <returns>Formatted UPDATE query.</returns>
    /// <exception cref="ArgumentNullException">tableName or fieldExpressions is null.</exception>
    public static string Format(string tableName, string where,
        IEnumerable<FieldExpressionPair> fieldExpressions, ISqlDialect dialect = null)
    {
        if (tableName == null || tableName.Length == 0)
            throw new ArgumentNullException(tableName);

        ArgumentNullException.ThrowIfNull(fieldExpressions);

        var list = fieldExpressions.ToList();
        StringBuilder sb = new("UPDATE ", 64 + where.Length + list.Count * 16);
        sb.Append(SqlSyntax.AutoBracketValid(tableName, dialect));
        sb.Append(" SET ");
        var i = 0;
        foreach (var pair in list)
        {
            if (i++ > 0)
                sb.Append(", ");
            sb.Append(SqlSyntax.AutoBracket(pair.Field, dialect));
            sb.Append(" = ");
            sb.Append(pair.Expression);
        }

        if (where != null && where.Length > 0)
        {
            sb.Append(" WHERE ");
            sb.Append(where);
        }

        return sb.ToString();
    }
}