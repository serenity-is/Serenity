namespace Serenity.CodeGenerator;

public class EntityJoin
{
    public string Name { get; set; } = null!;
    public string SourceField { get; set; } = null!;
    public List<EntityField> Fields { get; } = [];

    public string Alias => "j" + Name;
}