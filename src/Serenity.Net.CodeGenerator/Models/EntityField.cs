namespace Serenity.CodeGenerator
{
    public class EntityField
    {
        public string FieldType { get; set; }
        public string DataType { get; set; }
        public string TSType { get; set; }
        public string Ident { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Flags { get; set; }
        public string PKSchema { get; set; }
        public string PKTable { get; set; }
        public string PKColumn { get; set; }
        public string ForeignJoinAlias { get; set; }
        public bool Insertable { get; set; }
        public bool Updatable { get; set; }
        public bool IsValueType { get; set; }
        public int? Size { get; set; }
        public int Scale { get; set; }
        public string TextualField { get; set; }
        public string Attributes { get; set; }
        public string ColAttributes { get; set; }
        public string Expression { get; set; }

        public string TSEditorType
        {
            get
            {
                switch (FieldType)
                {
                    case "Int32":
                    case "Int16":
                    case "Int64":
                        return "IntegerEditor";

                    case "Single":
                    case "Double":
                    case "Decimal":
                        return "DecimalEditor";

                    case "DateTime":
                        return "DateEditor";

                    case "Boolean":
                        return "BooleanEditor";

                    default:
                        return "StringEditor";
                }
            }
        }

        public string PropertyType
        {
            get { return IsValueType ? DataType + "?" : DataType; }
        }
    }
}