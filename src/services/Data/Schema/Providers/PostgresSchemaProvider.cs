namespace Serenity.Data.Schema;

/// <summary>
/// PostgreSQL metadata provider
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
	                column_name ""FieldName"",
                    data_type ""DataType"",
                    CASE WHEN is_nullable = 'NO' THEN 0 ELSE 1 END ""IsNullable"",
                    CASE WHEN column_default LIKE 'nextval(%' THEN 1 ELSE 0 END ""IsIdentity"",
                    COALESCE(character_maximum_length, CASE WHEN data_type = 'numeric' OR 
                        data_type = 'decimal' THEN numeric_precision ELSE 0 END) ""Size"",
                    numeric_scale ""Scale""
                FROM information_schema.COLUMNS
                WHERE table_schema = @sma and table_name = @tbl
                ORDER BY ordinal_position", new
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
                    o.conname AS FKName,
                    (SELECT a.attname FROM pg_attribute a WHERE a.attrelid = m.oid AND a.attnum = o.conkey[1] AND a.attisdropped = false) AS FKColumn,
                    (SELECT nspname FROM pg_namespace WHERE oid=f.relnamespace) AS PKSchema,
                    f.relname AS PKTable,
                    (SELECT a.attname FROM pg_attribute a WHERE a.attrelid = f.oid AND a.attnum = o.confkey[1] AND a.attisdropped = false) AS PKColumn
                FROM
                    pg_constraint o LEFT JOIN pg_class c ON c.oid = o.conrelid
                    LEFT JOIN pg_class f ON f.oid = o.confrelid LEFT JOIN pg_class m ON m.oid = o.conrelid
                WHERE
                    o.contype = 'f' AND o.conrelid IN (SELECT oid FROM pg_class c WHERE c.relkind = 'r')
                    AND (SELECT nspname FROM pg_namespace WHERE oid=m.relnamespace) = @sma
                    AND m.relname = @tbl", new
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
        return connection.Query<string>(@"
                SELECT column_name, column_default 
                FROM information_schema.COLUMNS 
                WHERE TABLE_SCHEMA = @sma AND TABLE_NAME = @tbl 
                AND column_default like 'nextval(%'", new
        {
            sma = schema,
            tbl = table
        });
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
        return connection.Query<string>(
            $@"SELECT pg_attribute.attname 
                FROM pg_index, pg_class, pg_attribute, pg_namespace 
                WHERE pg_class.oid = {("\"" + schema + "\".\"" + table + "\"").ToSql(PostgresDialect.Instance)}::regclass 
                AND indrelid = pg_class.oid 
                AND nspname = {("\"" + schema + "\"").ToSql(PostgresDialect.Instance)}
                AND pg_class.relnamespace = pg_namespace.oid
                AND pg_attribute.attrelid = pg_class.oid
                AND pg_attribute.attnum = any(pg_index.indkey)
                AND indisprimary");
    }

    private class TableNameSource
    {
#pragma warning disable IDE1006 // Naming Styles
        public string table_schema { get; set; }
        public string table_name { get; set; }
        public string table_type { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }

    /// <summary>
    /// Gets the table names.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <returns></returns>
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
                Table = x.table_name,
                IsView = x.table_type == "VIEW"
            });
    }
}