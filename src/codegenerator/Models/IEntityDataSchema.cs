namespace Serenity.CodeGenerator;

public interface IEntityDataSchema
{
    /// <summary>
    /// Gets the default schema.
    /// </summary>
    /// <value>
    /// The default schema.
    /// </value>
    public string DefaultSchema { get; }

    /// <summary>
    /// Gets the foreign keys.
    /// </summary>
    /// <param name="schema">The schema.</param>
    /// <param name="table">The table.</param>
    IEnumerable<Data.Schema.ForeignKeyInfo> GetForeignKeys(string schema, string table);

    /// <summary>
    /// Gets the identity fields.
    /// </summary>
    /// <param name="schema">The schema.</param>
    /// <param name="table">The table.</param>
    IEnumerable<string> GetIdentityFields(string schema, string table);

    /// <summary>
    /// Gets the primary key fields.
    /// </summary>
    /// <param name="schema">The schema.</param>
    /// <param name="table">The table.</param>
    IEnumerable<string> GetPrimaryKeyFields(string schema, string table);

    /// <summary>
    /// Gets the field infos.
    /// </summary>
    /// <param name="schema">The schema.</param>
    /// <param name="table">The table.</param>
    IEnumerable<Data.Schema.FieldInfo> GetFieldInfos(string schema, string table);
}