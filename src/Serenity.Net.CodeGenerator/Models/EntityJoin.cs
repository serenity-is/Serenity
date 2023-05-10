namespace Serenity.CodeGenerator;

public class EntityJoin
{
    public string Name { get; set; }
    public string SourceField { get; set; }
    public List<EntityField> Fields { get; } = new();

    public string Alias => "j" + Name;
}