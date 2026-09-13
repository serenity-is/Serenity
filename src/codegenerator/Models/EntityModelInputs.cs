namespace Serenity.CodeGenerator;

public class EntityModelInputs : IEntityModelInputs
{
    public IApplicationMetadata Application { get; set; } = null!;
    public GeneratorConfig Config { get; set; } = null!;
    public string ConnectionKey { get; set; } = null!;
    public IEntityDataSchema DataSchema { get; set; } = null!;
    public string Identifier { get; set; } = null!;
    public HashSet<string> GlobalUsings { get; } = [];
    public string? Module { get; set; }
    public bool Net5Plus { get; set; } = true;
    public bool Net8Plus { get; set; } = true;
    public bool Nullable { get; set; } = false;
    public bool SchemaIsDatabase { get; set; }
    public string? PermissionKey { get; set; }
    public string? Schema { get; set; }
    public string Table { get; set; } = null!;
    public bool SkipForeignKeys { get; set; }
}