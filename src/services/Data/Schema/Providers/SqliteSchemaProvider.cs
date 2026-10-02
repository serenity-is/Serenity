namespace Serenity.Data.Schema;

/// <summary>
/// SQLite metadata provider.
/// </summary>
/// <seealso cref="ISchemaProvider" />
public class SqliteSchemaProvider : ISchemaProvider
{
    /// <summary>
    /// Gets the default schema.
    /// </summary>
    /// <value>
    /// The default schema.
    /// </value>
    public string? DefaultSchema => null;

    /// <summary>
    /// Builds a PRAGMA statement with an escaped table reference.
    /// PRAGMAs take no bound parameters, so the name is interpolated
    /// double-quoted with <c>"</c> escaped as <c>""</c> (verified against
    /// SQLite: brackets don't support a <c>]]</c> escape, and single
    /// quotes are parsed as string literals). A non-empty schema qualifies
    /// the PRAGMA for attached databases (<c>PRAGMA "schema".name(...)</c>).
    /// </summary>
    private static string PragmaTableRef(string pragma, string? schema, string table)
    {
        ArgumentException.ThrowIfNullOrEmpty(table);

        var tableRef = "\"" + table.Replace("\"", "\"\"") + "\"";
        if (string.IsNullOrEmpty(schema))
            return "PRAGMA " + pragma + "(" + tableRef + ")";

        return "PRAGMA \"" + schema.Replace("\"", "\"\"") + "\"." + pragma + "(" + tableRef + ")";
    }

    private class FieldInfoSource
    {
#pragma warning disable IDE1006 // Naming Styles
        public required string name { get; set; }
        public string? type { get; set; }
        public int notnull { get; set; }
        public int pk { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }

    /// <inheritdoc/>
    public IEnumerable<FieldInfo> GetFieldInfos(IDbConnection connection, string? schema, string table)
    {
        ArgumentNullException.ThrowIfNull(connection);

        var fields = connection.Query<FieldInfoSource>(PragmaTableRef("table_info", schema, table)).ToList();
        var primaryKeys = fields.Where(x => x.pk > 0).ToList();
        var identityName = primaryKeys.Count == 1 &&
            string.Equals(primaryKeys[0].type, "INTEGER", StringComparison.OrdinalIgnoreCase)
                ? primaryKeys[0].name
                : null;

        return fields.Select(x => new FieldInfo
            {
                FieldName = x.name,
                DataType = x.type,
                IsNullable = x.notnull != 1,
                IsPrimaryKey = x.pk > 0,
                IsIdentity = x.name == identityName
            });
    }

    private class ForeignKeySource
    {
#pragma warning disable IDE1006 // Naming Styles
        public int id { get; set; }
        public required string from { get; set; }
        public required string table { get; set; }
        public required string to { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }

    /// <inheritdoc/>
    public IEnumerable<ForeignKeyInfo> GetForeignKeys(IDbConnection connection, string? schema, string table)
    {
        ArgumentNullException.ThrowIfNull(connection);

        return connection.Query<ForeignKeySource>(PragmaTableRef("foreign_key_list", schema, table))
            .Select(x => new ForeignKeyInfo
            {
                FKName = x.id.ToString(),
                FKColumn = x.from,
                PKTable = x.table,
                PKColumn = x.to
            });
    }

    private class IdentitySource
    {
#pragma warning disable IDE1006 // Naming Styles
        public int pk { get; set; }
        public required string name { get; set; }
        public string? type { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }

    /// <inheritdoc/>
    public IEnumerable<string> GetIdentityFields(IDbConnection connection, string? schema, string table)
    {
        ArgumentNullException.ThrowIfNull(connection);

        var fields = connection.Query<IdentitySource>(PragmaTableRef("table_info", schema, table))
            .Where(x => x.pk > 0)
            .ToList();

        // A single INTEGER PRIMARY KEY is a rowid alias (comparison is
        // case-insensitive as the declared type text keeps its case).
        if (fields.Count == 1 &&
            string.Equals(fields[0].type, "INTEGER", StringComparison.OrdinalIgnoreCase))
        {
            return [fields[0].name];
        }

        // ROWID tables have an implicit rowid, WITHOUT ROWID tables don't.
        var master = string.IsNullOrEmpty(schema) ? "sqlite_master"
            : "\"" + schema.Replace("\"", "\"\"") + "\".sqlite_master";
        var createSql = connection.Query<string>(
            "SELECT sql FROM " + master + " WHERE type = 'table' AND name = @tbl",
            new { tbl = table }).FirstOrDefault();
        if (createSql is not null &&
            createSql.Contains("WITHOUT ROWID", StringComparison.OrdinalIgnoreCase))
            return [];

        return ["ROWID"];
    }

    private class PrimaryKeySource
    {
#pragma warning disable IDE1006 // Naming Styles
        public int pk { get; set; }
        public required string name { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }

    /// <inheritdoc/>
    public IEnumerable<string> GetPrimaryKeyFields(IDbConnection connection, string? schema, string table)
    {
        ArgumentNullException.ThrowIfNull(connection);

        return connection.Query<PrimaryKeySource>(PragmaTableRef("table_info", schema, table))
            .Where(x => x.pk > 0)
            .OrderBy(x => x.pk)
            .Select(x => x.name);
    }

    private class TableNameSource
    {
#pragma warning disable IDE1006 // Naming Styles
        public required string name { get; set; }
        public string? type { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }

    /// <inheritdoc/>
    public IEnumerable<TableName> GetTableNames(IDbConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        return connection.Query<TableNameSource>(
                "SELECT name, type FROM sqlite_master WHERE (type='table' or type='view') " +
                "AND name NOT LIKE 'sqlite_%' " +
                "ORDER BY name")
            .Select(x => new TableName
            {
                Table = x.name,
                IsView = x.type == "view"
            });
    }
}
