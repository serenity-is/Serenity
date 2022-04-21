namespace Serenity.CodeGenerator
{
    public class EntityJoin
    {
        public string Name { get; set; }
        public string SourceField { get; set; }
        public List<EntityField> Fields { get; set; }
    }
}