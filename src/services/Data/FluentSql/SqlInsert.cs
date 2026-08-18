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
        nameValuePairs = [];
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
        ArgumentNullException.ThrowIfNull(field);

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
        SqlInsert clone = new(tableName);
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

        cachedQuery = Format(tableName, nameValuePairs, dialect);

        return cachedQuery;
    }

    /// <summary>
    ///   Formats an INSERT query.</summary>
    /// <param name="tableName">
    ///   Table name (required).</param>
    /// <param name="nameValuePairs">
    ///   Field names and values. Must be passed in the order of <c>[field1, value1, field2, 
    ///   value2, ...., fieldN, valueN]</c>. It must have even number of elements.</param>
    /// <param name="dialect">Target dialect</param>
    /// <returns>
    ///   Formatted query.</returns>
    public static string Format(string tableName, List<string> nameValuePairs, ISqlDialect dialect = null)
    {
        if (tableName == null || tableName.Length == 0)
            throw new ArgumentNullException(tableName);

        ArgumentNullException.ThrowIfNull(nameValuePairs);

        if (nameValuePairs.Count % 2 != 0)
            throw new ArgumentOutOfRangeException("nameValuePairs");

        StringBuilder sb = new("INSERT INTO ", 64 + nameValuePairs.Count * 16);
        sb.Append(SqlSyntax.AutoBracketValid(tableName, dialect));
        sb.Append(" (");
        for (int i = 0; i < nameValuePairs.Count; i += 2)
        {
            if (i > 0)
                sb.Append(", ");
            sb.Append(SqlSyntax.AutoBracket(nameValuePairs[i], dialect));
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

    /// <summary>
    ///   Formats an UPSERT query, i.e. a query that updates the row matching the key fields
    ///   or inserts a new row if no such row exists.</summary>
    /// <param name="tableName">
    ///   Table name (required).</param>
    /// <param name="nameValuePairs">
    ///   Field names and values. Must be passed in the order of <c>[field1, value1, field2, 
    ///   value2, ...., fieldN, valueN]</c>. It must have even number of elements.</param>
    /// <param name="keyFields">
    ///   List of key field names (e.g. primary key columns) that should be used to determine 
    ///   whether an existing row is updated or a new row is inserted. Key fields must exist 
    ///   among the fields in <paramref name="nameValuePairs"/>.</param>
    /// <param name="dialect">Target dialect</param>
    /// <returns>
    ///   Formatted UPSERT query.</returns>
    public static string FormatUpsert(string tableName, List<string> nameValuePairs,
        IEnumerable<string> keyFields, ISqlDialect dialect = null)
    {
        if (tableName == null || tableName.Length == 0)
            throw new ArgumentNullException(nameof(tableName));

        ArgumentNullException.ThrowIfNull(nameValuePairs);
        ArgumentNullException.ThrowIfNull(keyFields);

        if (nameValuePairs.Count % 2 != 0)
            throw new ArgumentOutOfRangeException(nameof(nameValuePairs));

        dialect ??= SqlSettings.DefaultDialect;

        var keyFieldList = keyFields.ToList();
        if (keyFieldList.Count == 0)
            throw new ArgumentOutOfRangeException(nameof(keyFields));

        int fieldCount = nameValuePairs.Count / 2;
        var fields = new List<string>(fieldCount);
        var values = new List<string>(fieldCount);
        var valueByField = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        for (int i = 0; i < nameValuePairs.Count; i += 2)
        {
            var field = nameValuePairs[i];
            if (field == null || field.Length == 0)
                throw new ArgumentException(string.Format(
                    "Field name at index {0} is null or empty!", i), nameof(nameValuePairs));

            fields.Add(SqlSyntax.AutoBracket(field, dialect));
            values.Add(nameValuePairs[i + 1]);
            valueByField[field] = nameValuePairs[i + 1];
        }

        var keyFieldsSet = new HashSet<string>(keyFieldList, StringComparer.OrdinalIgnoreCase);
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

        var nonKeyFields = new List<string>();
        var nonKeyValues = new List<string>();
        for (int i = 0; i < fieldCount; i++)
        {
            if (keyFieldsSet.Contains(nameValuePairs[i * 2]))
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
        return FormatUpsert(tableName, nameValuePairs, keyFields, dialect);
    }
}