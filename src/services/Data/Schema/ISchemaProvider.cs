namespace Serenity.Data.Schema;

/// <summary>
/// Abstraction for SQL metadata providers.
/// </summary>
public interface ISchemaProvider
{
    /// <summary>
    /// Gets the default schema.
    /// </summary>
    /// <value>
    /// The default schema.
    /// </value>
    string DefaultSchema { get; }

    /// <summary>
    /// Gets the foreign keys.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="schema">The schema.</param>
    /// <param name="table">The table.</param>
    /// <returns>The list of foreign keys for the table.</returns>
    IEnumerable<ForeignKeyInfo> GetForeignKeys(IDbConnection connection, string schema, string table);

    /// <summary>
    /// Gets the identity fields.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="schema">The schema.</param>
    /// <param name="table">The table.</param>
    /// <returns>The list of identity field names for the table.</returns>
    IEnumerable<string> GetIdentityFields(IDbConnection connection, string schema, string table);

    /// <summary>
    /// Gets the primary key fields.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="schema">The schema.</param>
    /// <param name="table">The table.</param>
    /// <returns>The list of primary key field names for the table.</returns>
    IEnumerable<string> GetPrimaryKeyFields(IDbConnection connection, string schema, string table);

    /// <summary>
    /// Gets the table names.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <returns>The list of table names in the database.</returns>
    IEnumerable<TableName> GetTableNames(IDbConnection connection);

    /// <summary>
    /// Gets the field infos.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="schema">The schema.</param>
    /// <param name="table">The table.</param>
    /// <returns>The list of field metadata for the table.</returns>
    IEnumerable<FieldInfo> GetFieldInfos(IDbConnection connection, string schema, string table);
}