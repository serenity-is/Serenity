namespace Serenity.Data.Schema;

/// <summary>
/// Oracle metadata provider.
/// </summary>
/// <seealso cref="ISchemaProvider" />
public class OracleSchemaProvider : ISchemaProvider
{
    /// <summary>
    /// Gets the default schema.
    /// </summary>
    /// <value>
    /// The default schema.
    /// </value>
    public string DefaultSchema => null;

    /// <summary>
    /// Gets the field infos.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="schema">The schema.</param>
    /// <param name="table">The table.</param>
    /// <returns></returns>
    public IEnumerable<FieldInfo> GetFieldInfos(IDbConnection connection, string schema, string table)
    {
        return connection.Query<FieldInfo>(@"
                SELECT 
                    c.column_name ""FieldName"",
                    c.data_type ""DataType"",
                    COALESCE(NULLIF(c.data_precision, 0), c.char_length) ""Size"",
                    c.data_scale ""Scale"",
                    CASE WHEN c.nullable = 'N' THEN 0 ELSE 1 END ""IsNullable""
                FROM all_tab_columns c
                WHERE 
                    c.owner = :sma
                    AND c.table_name = :tbl
                ORDER BY c.column_id
            ", new
        {
            sma = schema,
            tbl = table
        });
    }

    /// <summary>
    /// Gets the foreign keys.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="schema">The schema.</param>
    /// <param name="table">The table.</param>
    /// <returns></returns>
    public IEnumerable<ForeignKeyInfo> GetForeignKeys(IDbConnection connection, string schema, string table)
    {
        return connection.Query<ForeignKeyInfo>(@"
                SELECT 
                    a.constraint_name FKName,                     
                    a.column_name FKColumn,
                    c.r_owner PKSchema,
                    c_pk.table_name PKTable,
                    uc.column_name PKColumn
                FROM all_cons_columns a
                JOIN all_constraints c ON a.owner = c.owner AND a.constraint_name = c.constraint_name
                JOIN all_constraints c_pk ON c.r_owner = c_pk.owner AND c.r_constraint_name = c_pk.constraint_name
                JOIN user_cons_columns uc ON uc.constraint_name = c.r_constraint_name
                WHERE c.constraint_type = 'R' 
                    AND a.table_name = :tbl
                    AND c.r_owner = :sma", new
        {
            sma = schema,
            tbl = table
        });
    }

    /// <summary>
    /// Gets the identity fields.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="schema">The schema.</param>
    /// <param name="table">The table.</param>
    /// <returns></returns>
    public IEnumerable<string> GetIdentityFields(IDbConnection connection, string schema, string table)
    {
        return new List<string>();
    }

    /// <summary>
    /// Gets the primary key fields.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="schema">The schema.</param>
    /// <param name="table">The table.</param>
    /// <returns></returns>
    public IEnumerable<string> GetPrimaryKeyFields(IDbConnection connection, string schema, string table)
    {
        return connection.Query<string>(@"
                    SELECT cols.column_name
                    FROM all_constraints cons, all_cons_columns cols
                    WHERE cols.table_name = :tbl
                    AND cons.constraint_type = 'P'
                    AND cons.constraint_name = cols.constraint_name
                    AND cons.owner = :sch
                    ORDER BY cols.position;",
            new
            {
                sch = schema,
                tbl = table
            });
    }

    /// <summary>
    /// Gets the table names.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <returns></returns>
    public IEnumerable<TableName> GetTableNames(IDbConnection connection)
    {
        return connection.Query<TableName>(@"
                SELECT owner ""Schema"", table_name ""Table"" 
                FROM all_tables
                WHERE owner != 'SYS' 
                ORDER BY owner, table_name");
    }
}