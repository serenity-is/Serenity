namespace Serenity.Data.Schema;

/// <summary>
/// SQLite metadata provider
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
    public string DefaultSchema => null;

    private class FieldInfoSource
    {
#pragma warning disable IDE1006 // Naming Styles
        public string name { get; set; }
        public string type { get; set; }
        public string notnull { get; set; }
        public string pk { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }

    /// <summary>
    /// Gets the field infos.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="schema">The schema.</param>
    /// <param name="table">The table.</param>
    /// <returns></returns>
    public IEnumerable<FieldInfo> GetFieldInfos(IDbConnection connection, string schema, string table)
    {
        return connection.Query<FieldInfoSource>("PRAGMA table_info([" + table + "])")
            .Select(x => new FieldInfo
            {
                FieldName = x.name,
                DataType = x.type,
                IsNullable = Convert.ToInt32(x.notnull) != 1,
                IsPrimaryKey = Convert.ToInt32(x.pk) == 1
            });
    }

    private class ForeignKeySource
    {
#pragma warning disable IDE1006 // Naming Styles
        public string id { get; set; }
        public string from { get; set; }
        public string table { get; set; }
        public string to { get; set; }
#pragma warning restore IDE1006 // Naming Styles
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
        return connection.Query<ForeignKeySource>("PRAGMA foreign_key_list([" + table + "])")
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
        public string name { get; set; }
        public string type { get; set; }
#pragma warning restore IDE1006 // Naming Styles
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
        var fields = connection.Query<IdentitySource>("PRAGMA table_info([" + table + "])")
            .Where(x => x.pk > 0);

        if (fields.Count() == 1 &&
            fields.First().type == "INTEGER")
        {
            return new List<string> { fields.First().name };
        };

        return new List<string> { "ROWID" };
    }

    private class PrimaryKeySource
    {
#pragma warning disable IDE1006 // Naming Styles
        public int pk { get; set; }
        public string name { get; set; }
#pragma warning restore IDE1006 // Naming Styles
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
        return connection.Query<PrimaryKeySource>("PRAGMA table_info([" + table + "])")
            .Where(x => x.pk > 0)
            .OrderBy(x => x.pk)
            .Select(x => x.name);
    }

    private class TableNameSource
    {
#pragma warning disable IDE1006 // Naming Styles
        public string name { get; set; }
        public string type { get; set; }
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
                "SELECT name, type FROM sqlite_master WHERE type='table' or type='view' " +
                "ORDER BY name")
            .Select(x => new TableName
            {
                Table = x.name,
                IsView = x.type == "view"
            });
    }
}