using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Serenity.CodeGenerator
{
    public class EntityCodeGenerationModel
    {
        public string Module { get; set; }
        public string ConnectionKey { get; set; }
        public string Permission { get; set; }
        public string RootNamespace { get; set; }
        public string ClassName { get; set; }
        public string RowClassName { get; set; }
        public string Schema { get; set; }
        public string Tablename { get; set; }
        public string Identity { get; set; }
        public string IsActiveField { get; set; }
        public string RowBaseClass { get; set; }
        public List<EntityCodeField> RowBaseFields { get; set; }
        public string FieldsBaseClass { get; set; }
        public bool IsLookup { get; set; }
        public List<EntityCodeField> Fields;
        public List<EntityCodeJoin> Joins;
        public bool Instance { get; set; }
        public string NameField { get; set; }
        public string FieldPrefix { get; set; }
    }

    public class EntityCodeField
    {
        public string Type { get; set; }
        public string Ident { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Flags { get; set; }
        public string PKSchema { get; set; }
        public string PKTable { get; set; }
        public string PKColumn { get; set; }
        public string ForeignJoinAlias { get; set; }
        public bool IsValueType { get; set; }
        public int? Size { get; set; }
        public int Scale { get; set; }
        public string TextualField { get; set; }
    }

    public class EntityCodeJoin
    {
        public string Name { get; set; }
        public string SourceField { get; set; }
        public List<EntityCodeField> Fields { get; set; }
    }
}
