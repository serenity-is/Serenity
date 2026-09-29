namespace Serenity.Data.Schema;

/// <summary>
/// PostgreSQL metadata provider.
/// </summary>
/// <seealso cref="ISchemaProvider" />
public class PostgresSchemaProvider : ISchemaProvider
{
    /// <summary>
    /// Gets the default schema.
    /// </summary>
    /// <value>
    /// The default schema.
    /// </value>
    public string DefaultSchema => "public";

    /// <inheritdoc/>
    public IEnumerable<FieldInfo> GetFieldInfos(IDbConnection connection, string? schema, string table)
    {
        return connection.Query<FieldInfo>(/*lang=sql*/ """
            SELECT  
             column_name "FieldName",
                data_type "DataType",
                CASE WHEN is_nullable = 'NO' THEN 0 ELSE 1 END "IsNullable",
                CASE WHEN column_default LIKE 'nextval(%' THEN 1 ELSE 0 END "IsIdentity",
                COALESCE(character_maximum_length, CASE WHEN data_type = 'numeric' OR 
                    data_type = 'decimal' THEN numeric_precision ELSE 0 END) "Size",
                numeric_scale "Scale"
            FROM information_schema.COLUMNS
            WHERE table_schema = @sma and table_name = @tbl
            ORDER BY ordinal_position
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
                o.conname AS FKName,
                fa.attname AS FKColumn,
                (SELECT nspname FROM pg_namespace WHERE oid = f.relnamespace) AS PKSchema,
                f.relname AS PKTable,
                pa.attname AS PKColumn
            FROM
                pg_constraint o
                JOIN pg_class m ON m.oid = o.conrelid
                JOIN pg_class f ON f.oid = o.confrelid
                JOIN LATERAL unnest(o.conkey) WITH ORDINALITY AS fk_col(attnum, ord) ON true
                JOIN pg_attribute fa ON fa.attrelid = m.oid AND fa.attnum = fk_col.attnum AND fa.attisdropped = false
                JOIN LATERAL unnest(o.confkey) WITH ORDINALITY AS pk_col(attnum, ord) ON pk_col.ord = fk_col.ord
                JOIN pg_attribute pa ON pa.attrelid = f.oid AND pa.attnum = pk_col.attnum AND pa.attisdropped = false
            WHERE
                o.contype = 'f' AND m.relkind = 'r'
                AND (SELECT nspname FROM pg_namespace WHERE oid = m.relnamespace) = @sma
                AND m.relname = @tbl
            ORDER BY fk_col.ord
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
            SELECT column_name, column_default 
            FROM information_schema.COLUMNS 
            WHERE TABLE_SCHEMA = @sma AND TABLE_NAME = @tbl 
            AND column_default like 'nextval(%'
            """, new
        {
            sma = schema,
            tbl = table
        });
    }

    /// <inheritdoc/>
    public IEnumerable<string> GetPrimaryKeyFields(IDbConnection connection, string? schema, string table)
    {
        return connection.Query<string>(
            /*lang=sql*/ $$"""
            SELECT pg_attribute.attname 
                FROM pg_index, pg_class, pg_attribute, pg_namespace 
                WHERE pg_class.oid = {{("\"" + schema + "\".\"" + table + "\"").ToSql(PostgresDialect.Instance)}}::regclass 
                AND indrelid = pg_class.oid 
                AND nspname = {{("\"" + schema + "\"").ToSql(PostgresDialect.Instance)}}
                AND pg_class.relnamespace = pg_namespace.oid
                AND pg_attribute.attrelid = pg_class.oid
                AND pg_attribute.attnum = any(pg_index.indkey)
                AND indisprimary
            """);
    }

    private class TableNameSource
    {
#pragma warning disable IDE1006 // Naming Styles
        public string? table_schema { get; set; }
        public string? table_name { get; set; }
        public string? table_type { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }

    /// <inheritdoc/>
    public IEnumerable<TableName> GetTableNames(IDbConnection connection)
    {
        return connection.Query<TableNameSource>(
                "SELECT table_schema, table_name, table_type " +
                "FROM information_schema.tables " +
                "WHERE table_schema NOT IN ('pg_catalog', 'information_schema') " +
                "ORDER BY table_schema, table_name")
            .Select(x => new TableName
            {
                Schema = x.table_schema,
                Table = x.table_name!,
                IsView = x.table_type == "VIEW"
            });
    }
}