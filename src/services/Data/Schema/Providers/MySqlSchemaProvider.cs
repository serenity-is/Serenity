namespace Serenity.Data.Schema;

/// <summary>
/// MySql metadata provider.
/// </summary>
/// <seealso cref="ISchemaProvider" />
public class MySqlSchemaProvider : ISchemaProvider
{
    /// <summary>
    /// Gets the default schema.
    /// </summary>
    /// <value>
    /// The default schema.
    /// </value>
    public string? DefaultSchema => null;

    private class FieldInfoSource
    {
        public string? COLUMN_NAME { get; set; }
        public string? IS_NULLABLE { get; set; }
        public string? DATA_TYPE { get; set; }
        public long? CHARACTER_MAXIMUM_LENGTH { get; set; }
        public long? NUMERIC_PRECISION { get; set; }
        public long? NUMERIC_SCALE { get; set; }
        public string? COLUMN_KEY { get; set; }
        public string? EXTRA { get; set; }
    }

    /// <inheritdoc/>
    public IEnumerable<FieldInfo> GetFieldInfos(IDbConnection connection, string? schema, string table)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentException.ThrowIfNullOrEmpty(table);

        return connection.Query<FieldInfoSource>(/*lang=sql*/ """
            SELECT COLUMN_NAME,
                IS_NULLABLE,
                DATA_TYPE,
                CHARACTER_MAXIMUM_LENGTH,
                NUMERIC_PRECISION,
                NUMERIC_SCALE,
                COLUMN_KEY,
                EXTRA
            FROM information_schema.COLUMNS
            WHERE TABLE_SCHEMA = COALESCE(@sma, Database())
            AND TABLE_NAME = @tbl
            ORDER BY ORDINAL_POSITION
            """, new
        {
            sma = schema,
            tbl = table
        })
            .Select(src => new FieldInfo
            {
                FieldName = src.COLUMN_NAME!,
                DataType = src.DATA_TYPE,
                Size = (int)(src.CHARACTER_MAXIMUM_LENGTH ?? src.NUMERIC_PRECISION ?? 0),
                Scale = (int)(src.NUMERIC_SCALE ?? 0),
                IsNullable = src.IS_NULLABLE == "YES",
                IsPrimaryKey = src.COLUMN_KEY == "PRI",
                IsIdentity = src.EXTRA == "auto_increment"
            });
    }

    /// <inheritdoc/>
    public IEnumerable<ForeignKeyInfo> GetForeignKeys(IDbConnection connection, string? schema, string table)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentException.ThrowIfNullOrEmpty(table);

        return connection.Query<ForeignKeyInfo>(/*lang=sql*/ """
            SELECT
                k.CONSTRAINT_NAME FKName,
                k.COLUMN_NAME FKColumn,
                CASE WHEN k.REFERENCED_TABLE_SCHEMA = COALESCE(@sma, Database()) THEN NULL
                    ELSE k.REFERENCED_TABLE_SCHEMA END PKSchema,
                k.REFERENCED_TABLE_NAME PKTable,
                k.REFERENCED_COLUMN_NAME PKColumn
            FROM information_schema.TABLE_CONSTRAINTS i
            INNER JOIN information_schema.KEY_COLUMN_USAGE k
                ON k.CONSTRAINT_SCHEMA = i.CONSTRAINT_SCHEMA
                AND k.CONSTRAINT_NAME = i.CONSTRAINT_NAME
                AND k.TABLE_SCHEMA = i.TABLE_SCHEMA
                AND k.TABLE_NAME = i.TABLE_NAME
            WHERE i.CONSTRAINT_TYPE = 'FOREIGN KEY'
            AND i.TABLE_SCHEMA = COALESCE(@sma, Database())
            AND i.TABLE_NAME = @tbl
            ORDER BY k.ORDINAL_POSITION
            """, new
        {
            sma = schema,
            tbl = table
        });
    }

    /// <inheritdoc/>
    public IEnumerable<string> GetIdentityFields(IDbConnection connection, string? schema, string table)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentException.ThrowIfNullOrEmpty(table);

        return connection.Query<string>(/*lang=sql*/ """
            SELECT COLUMN_NAME FROM information_schema.COLUMNS
            WHERE TABLE_SCHEMA = COALESCE(@sma, Database())
            AND table_name = @tbl
            AND EXTRA = 'auto_increment'
            """,
            new
            {
                sma = schema,
                tbl = table
            });
    }

    /// <inheritdoc/>
    public IEnumerable<string> GetPrimaryKeyFields(IDbConnection connection, string? schema, string table)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentException.ThrowIfNullOrEmpty(table);

        return connection.Query<string>(/*lang=sql*/ """
            SELECT ku.COLUMN_NAME
            FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
            INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE ku
                ON ku.CONSTRAINT_SCHEMA = tc.CONSTRAINT_SCHEMA
                AND ku.CONSTRAINT_NAME = tc.CONSTRAINT_NAME
                AND ku.TABLE_SCHEMA = tc.TABLE_SCHEMA
                AND ku.TABLE_NAME = tc.TABLE_NAME
            WHERE tc.CONSTRAINT_TYPE = 'PRIMARY KEY'
            AND tc.TABLE_SCHEMA = COALESCE(@sma, Database())
            AND tc.TABLE_NAME = @tbl
            ORDER BY ku.ORDINAL_POSITION
            """,
            new
            {
                sma = schema,
                tbl = table
            });
    }

    private class TableNameSource
    {
        public string? TABLE_NAME { get; set; }
        public string? TABLE_TYPE { get; set; }
    }

    /// <inheritdoc/>
    public IEnumerable<TableName> GetTableNames(IDbConnection connection)
    {
        return connection.Query<TableNameSource>(
                "SELECT TABLE_NAME, TABLE_TYPE FROM INFORMATION_SCHEMA.TABLES " +
                "WHERE TABLE_SCHEMA = Database() " +
                "ORDER BY TABLE_SCHEMA, TABLE_NAME")
            .Select(x => new TableName
            {
                Table = x.TABLE_NAME!,
                IsView = x.TABLE_TYPE == "VIEW"
            });
    }
}