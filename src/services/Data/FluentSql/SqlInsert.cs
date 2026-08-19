namespace Serenity.Data;

/// <summary>
///   Class to generate queries of the form <c>INSERT INTO tablename (field1, field2..fieldN) 
///   VALUES (value1, value2..valueN)</c></summary>
public class SqlInsert : QueryWithParams, ISetFieldByStatement
{
    private readonly string tableName;
    private readonly List<FieldExpressionPair> fieldExpressions = [];
    private string identityColumn;
    private string cachedQuery;

    /// <summary>
    ///   Creates a new SqlInsert query.</summary>
    /// <param name="tableName">
    ///   Table to insert record (required).</param>
    public SqlInsert(string tableName)
    {
        if (string.IsNullOrEmpty(tableName))
            throw new ArgumentNullException(nameof(tableName));

        this.tableName = tableName;
    }

    /// <summary>
    /// Gets the identity column.
    /// </summary>
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
    /// Returns field and value expression pairs
    /// </summary>
    public IReadOnlyList<FieldExpressionPair> GetFieldExpressions()
    {
        return fieldExpressions;
    }

    /// <summary>
    /// Gets the table name.
    /// </summary>
    public string TableName()
    {
        return tableName;
    }

    /// <summary>
    ///   Sets field value.</summary>
    /// <param name="field">
    ///   Field name (required).</param>
    /// <param name="expression">
    ///   Field expression, required.</param>
    /// <returns>
    ///   SqlInsert object itself.</returns>
    public SqlInsert SetTo(string field, string expression)
    {
        if (field == null || field.Length == 0)
            throw new ArgumentNullException(field);
        if (expression == null || expression.Length == 0)
            throw new ArgumentNullException(expression);

        fieldExpressions.Add(new FieldExpressionPair(field, expression));
        cachedQuery = null;
        return this;
    }

    /// <summary>
    ///   Sets field value.</summary>
    /// <param name="field">
    ///   Field name (required).</param>
    /// <param name="expression">
    ///   Field expression, required.</param>
    /// <returns>
    ///   SqlInsert object itself.</returns>
    void ISetFieldByStatement.SetTo(string field, string expression)
    {
        if (field == null || field.Length == 0)
            throw new ArgumentNullException(field);
        if (expression == null || expression.Length == 0)
            throw new ArgumentNullException(expression);

        fieldExpressions.Add(new FieldExpressionPair(field, expression));
        cachedQuery = null;
    }

    /// <summary>
    ///   Sets field value.</summary>
    /// <param name="field">
    ///   Field (required).</param>
    /// <param name="expression">
    ///   Field expression, required.</param>
    /// <returns>
    ///   SqlInsert object itself.</returns>
    public SqlInsert SetTo(IField field, string expression)
    {
        ArgumentNullException.ThrowIfNull(field);

        cachedQuery = null;
        return SetTo(field.Name, expression);
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

        fieldExpressions.Add(new FieldExpressionPair(field, SqlKeywords.Null));
        cachedQuery = null;
        return this;
    }

    /// <summary>Clones the query.</summary>
    /// <returns>Clone.</returns>
    public SqlInsert Clone()
    {
        SqlInsert clone = new(tableName);
        clone.fieldExpressions.AddRange(fieldExpressions);
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

        cachedQuery = Format(tableName, fieldExpressions, dialect);

        return cachedQuery;
    }

    /// <summary>
    ///   Formats an INSERT query.</summary>
    /// <param name="tableName">
    ///   Table name (required).</param>
    /// <param name="fieldExpressions">
    ///   Field names and values in the form of [field1, value1, field2, value2, ..., fieldN, valueN].</param>
    /// <param name="dialect">Target dialect</param>
    /// <returns>
    ///   Formatted query.</returns>
    [Obsolete("Use overload with IEnumerable<FieldExpressionPair>")]
    public static string Format(string tableName, List<string> fieldExpressions, ISqlDialect dialect = null)
    {
        ArgumentNullException.ThrowIfNull(fieldExpressions);

        if (fieldExpressions.Count % 2 != 0)
            throw new ArgumentOutOfRangeException(nameof(fieldExpressions));

        var list = new List<FieldExpressionPair>(fieldExpressions.Count / 2);
        for (var i = 0; i < fieldExpressions.Count; i += 2)
            list.Add(new FieldExpressionPair(fieldExpressions[i], fieldExpressions[i + 1]));
        return Format(tableName, list, dialect);
    }

    /// <summary>
    ///   Formats an INSERT query.</summary>
    /// <param name="tableName">
    ///   Table name (required).</param>
    /// <param name="fieldExpressions">
    ///   Field names and their value expressions.</param>
    /// <param name="dialect">Target dialect</param>
    /// <returns>
    ///   Formatted query.</returns>
    public static string Format(string tableName, IEnumerable<FieldExpressionPair> fieldExpressions, ISqlDialect dialect = null)
    {
        if (tableName == null || tableName.Length == 0)
            throw new ArgumentNullException(tableName);

        ArgumentNullException.ThrowIfNull(fieldExpressions);

        var list = fieldExpressions.ToList();
        StringBuilder sb = new("INSERT INTO ", 64 + list.Count * 16);
        sb.Append(SqlSyntax.AutoBracketValid(tableName, dialect));
        sb.Append(" (");
        var i = 0;
        foreach (var pair in list)
        {
            if (i++ > 0)
                sb.Append(", ");
            sb.Append(SqlSyntax.AutoBracket(pair.Field, dialect));
        }
        sb.Append(") VALUES (");
        i = 0;
        foreach (var pair in list)
        {
            if (i++ > 0)
                sb.Append(", ");
            sb.Append(pair.Expression);
        }
        sb.Append(')');

        return sb.ToString();
    }

    /// <summary>
    ///   Formats an UPSERT query, i.e. a query that updates the row matching the key fields
    ///   or inserts a new row if no such row exists.</summary>
    /// <param name="tableName">
    ///   Table name (required).</param>
    /// <param name="fieldExpressions">
    ///   Field names and their value expressions.</param>
    /// <param name="keyFields">
    ///   List of key field names (e.g. primary key columns) that should be used to determine 
    ///   whether an existing row is updated or a new row is inserted. Key fields must exist 
    ///   among the fields in <paramref name="fieldExpressions"/>.</param>
    /// <param name="dialect">Target dialect</param>
    /// <returns>
    ///   Formatted UPSERT query.</returns>
    public static string FormatUpsert(string tableName, IEnumerable<FieldExpressionPair> fieldExpressions,
        IEnumerable<string> keyFields, ISqlDialect dialect = null)
    {
        if (tableName == null || tableName.Length == 0)
            throw new ArgumentNullException(nameof(tableName));

        ArgumentNullException.ThrowIfNull(fieldExpressions);
        ArgumentNullException.ThrowIfNull(keyFields);

        dialect ??= SqlSettings.DefaultDialect;

        var keyFieldList = keyFields.ToList();
        if (keyFieldList.Count == 0)
            throw new ArgumentOutOfRangeException(nameof(keyFields));

        var list = fieldExpressions.ToList();
        int fieldCount = list.Count;
        var fields = new List<string>(fieldCount);
        var values = new List<string>(fieldCount);
        var valueByField = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var pair in list)
        {
            var field = pair.Field;
            if (field == null || field.Length == 0)
                throw new ArgumentException("Field name is null or empty!", nameof(fieldExpressions));

            fields.Add(SqlSyntax.AutoBracket(field, dialect));
            values.Add(pair.Expression);
            valueByField[field] = pair.Expression;
        }

        var keyBracketed = new List<string>(keyFieldList.Count);
        var keyValues = new List<string>(keyFieldList.Count);
        foreach (var keyField in keyFieldList)
        {
            if (!valueByField.TryGetValue(keyField, out var keyValue))
                throw new ArgumentException(string.Format(
                    "Key field '{0}' is not among the fields to insert!", keyField), nameof(keyFields));

            keyBracketed.Add(SqlSyntax.AutoBracket(keyField, dialect));
            keyValues.Add(keyValue);
        }

        var keyFieldsSet = new HashSet<string>(keyFieldList);
        var nonKeyFields = new List<string>();
        var nonKeyValues = new List<string>();
        for (int i = 0; i < fieldCount; i++)
        {
            if (keyFieldsSet.Contains(list[i].Field))
                continue;

            nonKeyFields.Add(fields[i]);
            nonKeyValues.Add(values[i]);
        }

        var table = SqlSyntax.AutoBracketValid(tableName, dialect);
        var insertColumns = string.Join(", ", fields);
        var insertValues = string.Join(", ", values);
        var keyCondition = string.Join(" AND ", keyBracketed.Select((f, i) => f + " = " + keyValues[i]));
        var nonKeyAssignments = string.Join(", ", nonKeyFields.Select((f, i) => f + " = " + nonKeyValues[i]));

        switch (dialect.ServerType)
        {
            case nameof(ServerType.SqlServer):
            {
                var updateBlock = nonKeyFields.Count > 0 ? $"""
                    ;
                    IF @@ROWCOUNT = 0
                    BEGIN
                       UPDATE {table} SET {nonKeyAssignments} WHERE {keyCondition};
                    END
                    """ : "";
                return $"""
                    INSERT INTO {table} ({insertColumns})
                    SELECT {insertValues} WHERE NOT EXISTS (
                        SELECT 1 FROM {table} WITH (UPDLOCK, SERIALIZABLE)
                        WHERE {keyCondition}){updateBlock}
                    """;
            }

            case nameof(ServerType.Sqlite):
            case nameof(ServerType.Postgres):
            {
                var conflictAction = nonKeyFields.Count > 0
                    ? $"DO UPDATE SET {string.Join(", ", nonKeyFields.Select(f => f + " = excluded." + f))}"
                    : "DO NOTHING";
                return $"""
                    INSERT INTO {table} ({insertColumns})
                    VALUES ({insertValues})
                    ON CONFLICT ({string.Join(", ", keyBracketed)})
                    {conflictAction}
                    """;
            }

            case nameof(ServerType.MySql):
            {
                var updateAssignments = nonKeyFields.Count > 0 ? nonKeyAssignments :
                    keyBracketed[0] + " = " + keyBracketed[0];
                return $"""
                    INSERT INTO {table} ({insertColumns})
                    VALUES ({insertValues})
                    ON DUPLICATE KEY UPDATE {updateAssignments}
                    """;
            }

            case nameof(ServerType.Oracle):
            {
                var mergeUpdate = nonKeyFields.Count > 0 ? $"""
                    WHEN MATCHED THEN
                        UPDATE SET {string.Join(", ", nonKeyFields.Select(f => "t." + f + " = s." + f))}
                    """ : "";
                return $"""
                    MERGE INTO {table} t
                    USING (SELECT {string.Join(", ", fields.Select((f, i) => values[i] + " AS " + f))} FROM dual) s
                    ON ({string.Join(" AND ", keyBracketed.Select(f => "t." + f + " = s." + f))})
                    {mergeUpdate}
                    WHEN NOT MATCHED THEN
                        INSERT ({insertColumns}) VALUES ({string.Join(", ", fields.Select(f => "s." + f))})
                    """;
            }

            case nameof(ServerType.Firebird):
                return $"""
                    UPDATE OR INSERT INTO {table} ({insertColumns})
                    VALUES ({insertValues})
                    MATCHING ({string.Join(", ", keyBracketed)})
                    """;

            default:
                throw new NotSupportedException(string.Format(
                    "UPSERT is not supported for dialect '{0}'!", dialect.ServerType));
        }
    }

    /// <summary>
    ///   Formats an UPSERT query, i.e. a query that updates the row matching the key fields
    ///   or inserts a new row if no such row exists.</summary>
    /// <param name="keyFields">
    ///   List of key field names (e.g. primary key columns) that should be used to determine 
    ///   whether an existing row is updated or a new row is inserted. Key fields must exist 
    ///   among the fields set on this query.</param>
    /// <returns>
    ///   Formatted UPSERT query.</returns>
    public string ToUpsertString(IEnumerable<string> keyFields)
    {
        return FormatUpsert(tableName, fieldExpressions, keyFields, dialect);
    }
}