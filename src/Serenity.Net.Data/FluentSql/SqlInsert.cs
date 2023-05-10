namespace Serenity.Data;

/// <summary>
///   Class to generate queries of the form <c>INSERT INTO tablename (field1, field2..fieldN) 
///   VALUES (value1, value2..valueN)</c></summary>
public class SqlInsert : QueryWithParams, ISetFieldByStatement
{
    private string tableName;
    private List<string> nameValuePairs;
    private string identityColumn;
    private string cachedQuery;

    private void Initialize(string tableName)
    {
        if (string.IsNullOrEmpty(tableName))
            throw new ArgumentNullException("tableName");

        this.tableName = tableName;
        nameValuePairs = new List<string>();
        cachedQuery = null;
    }

    /// <summary>
    /// Gets the identity column.
    /// </summary>
    /// <returns></returns>
    public string IdentityColumn()
    {
        return identityColumn;
    }

    /// <summary>
    /// Sets the identity column.
    /// </summary>
    /// <param name="value">The identity column.</param>
    /// <returns></returns>
    public SqlInsert IdentityColumn(string value)
    {
        identityColumn = value;
        return this;
    }

    /// <summary>
    ///   Creates a new SqlInsert query.</summary>
    /// <param name="tableName">
    ///   Table to insert record (required).</param>
    public SqlInsert(string tableName)
    {
        Initialize(tableName);
    }

    /// <summary>
    ///   Sets field value.</summary>
    /// <param name="field">
    ///   Field name (required).</param>
    /// <param name="value">
    ///   Field value (expression, required).</param>
    /// <returns>
    ///   SqlInsert object itself.</returns>
    public SqlInsert SetTo(string field, string value)
    {
        if (field == null || field.Length == 0)
            throw new ArgumentNullException(field);
        if (value == null || value.Length == 0)
            throw new ArgumentNullException(value);

        nameValuePairs.Add(field);
        nameValuePairs.Add(value);
        cachedQuery = null;
        return this;
    }

    /// <summary>
    ///   Sets field value.</summary>
    /// <param name="field">
    ///   Field name (required).</param>
    /// <param name="value">
    ///   Field value (expression, required).</param>
    /// <returns>
    ///   SqlInsert object itself.</returns>
    void ISetFieldByStatement.SetTo(string field, string value)
    {
        if (field == null || field.Length == 0)
            throw new ArgumentNullException(field);
        if (value == null || value.Length == 0)
            throw new ArgumentNullException(value);

        nameValuePairs.Add(field);
        nameValuePairs.Add(value);
        cachedQuery = null;
    }

    /// <summary>
    ///   Sets field value.</summary>
    /// <param name="field">
    ///   Field (required).</param>
    /// <param name="value">
    ///   Field value (expression, required).</param>
    /// <returns>
    ///   SqlInsert object itself.</returns>
    public SqlInsert SetTo(IField field, string value)
    {
        if (field == null)
            throw new ArgumentNullException("meta");

        cachedQuery = null;
        return SetTo(field.Name, value);
    }

    /// <summary>
    ///   Assigns NULL as the field value.</summary>
    /// <param name="field">
    ///   Field (required).</param>
    /// <returns>
    ///   SqlInsert object itself.</returns>
    public SqlInsert SetNull(string field)
    {
        if (string.IsNullOrEmpty(field))
            throw new ArgumentNullException(field);

        nameValuePairs.Add(field);
        nameValuePairs.Add(SqlKeywords.Null);
        cachedQuery = null;
        return this;
    }

    /// <summary>Clones the query.</summary>
    /// <returns>Clone.</returns>
    public SqlInsert Clone()
    {
        SqlInsert clone = new SqlInsert(tableName);
        clone.nameValuePairs.AddRange(nameValuePairs);
        CloneParams(clone);
        clone.cachedQuery = cachedQuery;
        return clone;
    }

    /// <summary>
    /// Sets the dialect (SQL server type / version) for query.
    /// </summary>
    public SqlInsert Dialect(ISqlDialect dialect)
    {
        this.dialect = dialect ?? throw new ArgumentNullException("dialect");
        dialectOverridden = true;

        return this;
    }

    /// <summary>
    ///   Gets string representation of the query.</summary>
    /// <returns>
    ///   String representation.</returns>
    public override string ToString()
    {
        if (cachedQuery != null)
            return cachedQuery;

        cachedQuery = Format(tableName, nameValuePairs);

        return cachedQuery;
    }

    /// <summary>
    ///   Formats an INSERT query.</summary>
    /// <param name="tableName">
    ///   Table name (required).</param>
    /// <param name="nameValuePairs">
    ///   Field names and values. Must be passed in the order of <c>[field1, value1, field2, 
    ///   value2, ...., fieldN, valueN]</c>. It must have even number of elements.</param>
    /// <returns>
    ///   Formatted query.</returns>
    public static string Format(string tableName, List<string> nameValuePairs)
    {
        if (tableName == null || tableName.Length == 0)
            throw new ArgumentNullException(tableName);

        if (nameValuePairs == null)
            throw new ArgumentNullException("nameValuePairs");

        if (nameValuePairs.Count % 2 != 0)
            throw new ArgumentOutOfRangeException("nameValuePairs");

        StringBuilder sb = new StringBuilder("INSERT INTO ", 64 + nameValuePairs.Count * 16);
        sb.Append(SqlSyntax.AutoBracketValid(tableName));
        sb.Append(" (");
        for (int i = 0; i < nameValuePairs.Count; i += 2)
        {
            if (i > 0)
                sb.Append(", ");
            sb.Append(SqlSyntax.AutoBracket(nameValuePairs[i]));
        }
        sb.Append(") VALUES (");
        for (int i = 1; i < nameValuePairs.Count; i += 2)
        {
            if (i > 1)
                sb.Append(", ");
            sb.Append(nameValuePairs[i]);
        }
        sb.Append(')');

        return sb.ToString();
    }
}