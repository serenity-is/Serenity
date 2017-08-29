using System.Collections.Generic;

namespace Serenity.CodeGenerator
{
    public class EntityModel
    {
        public string Module { get; set; }
        public string ConnectionKey { get; set; }
        public string Permission { get; set; }
        public string RootNamespace { get; set; }
        public string ClassName { get; set; }
        public string RowClassName { get; set; }
        public string Schema { get; set; }
        public string Tablename { get; set; }
        public string Title { get; set; }
        public string Identity { get; set; }
        public string RowBaseClass { get; set; }
        public List<EntityField> RowBaseFields { get; set; }
        public string FieldsBaseClass { get; set; }
        public bool IsLookup { get; set; }
        public List<EntityField> Fields;
        public List<EntityJoin> Joins;
        public bool Instance { get; set; }
        public string NameField { get; set; }
        public string FieldPrefix { get; set; }
        public GeneratorConfig GeneratorConfig { get; set; }
    }
}