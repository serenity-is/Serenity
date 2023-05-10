using Serenity.Data.Schema;

namespace Serenity.CodeGenerator;

public class EntityDataSchema : IEntityDataSchema
{
    private readonly IDbConnection connection;
    private readonly ISchemaProvider schemaProvider;

    public EntityDataSchema(IDbConnection connection)
    {
        this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        schemaProvider = SchemaHelper.GetSchemaProvider(connection.GetDialect().ServerType);
    }

    public string DefaultSchema => schemaProvider.DefaultSchema;

    public IEnumerable<Data.Schema.FieldInfo> GetFieldInfos(string schema, string table)
        => schemaProvider.GetFieldInfos(connection, schema, table);

    public IEnumerable<ForeignKeyInfo> GetForeignKeys(string schema, string table)
        => schemaProvider.GetForeignKeys(connection, schema, table);

    public IEnumerable<string> GetIdentityFields(string schema, string table)
        => schemaProvider.GetIdentityFields(connection, schema, table);

    public IEnumerable<string> GetPrimaryKeyFields(string schema, string table)
        => schemaProvider.GetPrimaryKeyFields(connection, schema, table);
}