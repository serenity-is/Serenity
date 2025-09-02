namespace Serenity.CodeGenerator;

public interface IEntityModelInputs
{
    public IApplicationMetadata Application { get; }
    string ConnectionKey { get; }
    string Identifier { get; }
    GeneratorConfig Config { get; }
    IEntityDataSchema DataSchema { get; }
    HashSet<string> GlobalUsings { get; }
    string Module { get; }
    bool Net5Plus { get; }
    bool SchemaIsDatabase { get; }
    string PermissionKey { get; }
    string Schema { get; }
    bool SkipForeignKeys { get; }
    string Table { get; }
    string Tablename => string.IsNullOrEmpty(Schema) ? Table : (Schema + "." + Table);
}