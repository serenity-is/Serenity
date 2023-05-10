namespace Serenity.Data.Schema;

/// <summary>
/// SQL server metadata provider
/// </summary>
/// <seealso cref="ISchemaProvider" />
public class SqlServerSchemaProvider : ISchemaProvider
{
    /// <summary>
    /// Gets the default schema.
    /// </summary>
    /// <value>
    /// The default schema.
    /// </value>
    public string DefaultSchema => "dbo";

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
                    COLUMN_NAME [FieldName],
                    CASE WHEN DATA_TYPE = 'timestamp' THEN 'rowversion' ELSE DATA_TYPE END [DataType],
                    CASE WHEN IS_NULLABLE = 'NO' THEN 0 ELSE 1 END [IsNullable],
                    COALESCE(CHARACTER_MAXIMUM_LENGTH, CASE WHEN DATA_TYPE in ('decimal', 'money', 'numeric') THEN NUMERIC_PRECISION ELSE 0 END) [Size],
                    NUMERIC_SCALE [Scale]
                FROM
                    INFORMATION_SCHEMA.COLUMNS
                WHERE
                    TABLE_SCHEMA = @sma 
                    AND TABLE_NAME = @tbl
                ORDER BY
                    ORDINAL_POSITION", new
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
                    fk.CONSTRAINT_NAME FKName,
                    fk.COLUMN_NAME FKColumn,
                    pk.TABLE_SCHEMA PKSchema,
                    pk.TABLE_NAME PKTable,
                    pk.COLUMN_NAME PKColumn
                FROM 
                    INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS c,
                    INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE fk,
                    INFORMATION_SCHEMA.KEY_COLUMN_USAGE pk 
                WHERE
                    fk.TABLE_SCHEMA = @sma
                    AND fk.TABLE_NAME = @tbl
                    AND fk.CONSTRAINT_SCHEMA = c.CONSTRAINT_SCHEMA
                    AND fk.CONSTRAINT_NAME = c.CONSTRAINT_NAME
                    AND pk.CONSTRAINT_SCHEMA = c.UNIQUE_CONSTRAINT_SCHEMA
                    AND pk.CONSTRAINT_NAME = c.UNIQUE_CONSTRAINT_NAME", new
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
                    SELECT COLUMN_NAME
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = @schema AND TABLE_NAME = @table
                    AND COLUMNPROPERTY(object_id(TABLE_SCHEMA + '.' + TABLE_NAME), COLUMN_NAME, 'IsIdentity') = 1",
            new
            {
                schema,
                table
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
                "SELECT COLUMN_NAME " +
                "FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS tc " +
                "INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS ku " +
                "ON tc.CONSTRAINT_TYPE = 'PRIMARY KEY' " +
                "AND tc.CONSTRAINT_NAME = ku.CONSTRAINT_NAME " +
                "AND ku.TABLE_SCHEMA = @schema " +
                "AND ku.TABLE_NAME = @table " +
                "ORDER BY ku.ORDINAL_POSITION",
            new
            {
                schema,
                table
            });
    }

    private class TableNameSource
    {
#pragma warning disable IDE1006 // Naming Styles
        public string TABLE_SCHEMA { get; set; }
        public string TABLE_NAME { get; set; }
        public string TABLE_TYPE { get; set; }
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
                "SELECT TABLE_SCHEMA, TABLE_NAME, TABLE_TYPE FROM INFORMATION_SCHEMA.TABLES " +
                "ORDER BY TABLE_SCHEMA, TABLE_NAME")
            .Select(x => new TableName
            {
                Schema = x.TABLE_SCHEMA,
                Table = x.TABLE_NAME,
                IsView = x.TABLE_TYPE == "VIEW"
            });
    }
}