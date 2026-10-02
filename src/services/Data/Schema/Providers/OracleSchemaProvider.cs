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
    public string? DefaultSchema => null;

    /// <inheritdoc/>
    public IEnumerable<FieldInfo> GetFieldInfos(IDbConnection connection, string? schema, string table)
    {
        return connection.Query<FieldInfo>(/*lang=sql*/ """
            SELECT 
                c.column_name "FieldName",
                c.data_type "DataType",
                COALESCE(NULLIF(c.data_precision, 0), c.char_length, 0) "Size",
                COALESCE(c.data_scale, 0) "Scale",
                CASE WHEN c.nullable = 'N' THEN 0 ELSE 1 END "IsNullable"
            FROM all_tab_columns c
            WHERE
                (:sma IS NULL OR c.owner = :sma)
                AND c.table_name = :tbl
            ORDER BY c.column_id

            """, new
        {
            sma = schema,
            tbl = table
        });
    }

    /// <inheritdoc/>
    public IEnumerable<ForeignKeyInfo> GetForeignKeys(IDbConnection connection, string? schema, string table)
    {
        return connection.Query<ForeignKeyInfo>(/*lang=sql*/ """
            SELECT
                a.constraint_name FKName,
                a.column_name FKColumn,
                c.r_owner PKSchema,
                c_pk.table_name PKTable,
                uc.column_name PKColumn
            FROM all_cons_columns a
            JOIN all_constraints c ON c.owner = a.owner AND c.constraint_name = a.constraint_name
            JOIN all_constraints c_pk ON c_pk.owner = c.r_owner AND c_pk.constraint_name = c.r_constraint_name
            JOIN all_cons_columns uc ON uc.owner = c_pk.owner AND uc.constraint_name = c_pk.constraint_name
                AND uc.position = a.position
            WHERE c.constraint_type = 'R'
                AND (:sma IS NULL OR a.owner = :sma)
                AND a.table_name = :tbl
            ORDER BY a.position
            """, new
        {
            sma = schema,
            tbl = table
        });
    }

    /// <inheritdoc/>
    public IEnumerable<string> GetIdentityFields(IDbConnection connection, string? schema, string table)
    {
        return connection.Query<string>(/*lang=sql*/ """
            SELECT column_name
            FROM all_tab_identity_cols
            WHERE (:sch IS NULL OR owner = :sch)
                AND table_name = :tbl
            ORDER BY column_name
            """, new
        {
            sch = schema,
            tbl = table
        });
    }

    /// <inheritdoc/>
    public IEnumerable<string> GetPrimaryKeyFields(IDbConnection connection, string? schema, string table)
    {
        return connection.Query<string>("""
            SELECT cols.column_name
            FROM all_constraints cons
            JOIN all_cons_columns cols
                ON cols.owner = cons.owner
                AND cols.constraint_name = cons.constraint_name
                AND cols.table_name = cons.table_name
            WHERE cols.table_name = :tbl
            AND cons.constraint_type = 'P'
            AND (:sch IS NULL OR cons.owner = :sch)
            ORDER BY cols.position
            """, new
        {
            sch = schema,
            tbl = table
        });
    }

    /// <inheritdoc/>
    public IEnumerable<TableName> GetTableNames(IDbConnection connection)
    {
        return connection.Query<TableNameSource>(/*lang=sql*/ """
            SELECT owner "Schema", table_name "Table", 0 "IsView"
            FROM all_tables
            WHERE owner != 'SYS'
            UNION ALL
            SELECT owner "Schema", view_name "Table", 1 "IsView"
            FROM all_views
            WHERE owner != 'SYS'
            ORDER BY "Schema", "Table"
            """).Select(x => new TableName
            {
                Schema = x.Schema,
                Table = x.Table,
                IsView = x.IsView != 0
            });
    }

    private sealed class TableNameSource
    {
        public string? Schema { get; set; }
        public required string Table { get; set; }
        public int IsView { get; set; }
    }
}
