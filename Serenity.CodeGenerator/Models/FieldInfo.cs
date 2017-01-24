namespace Serenity.CodeGenerator
{
    public class FieldInfo
    {
        public string FieldName { get; set; }
        public int Size { get; set; }
        public int Scale { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsIdentity { get; set; }
        public bool IsNullable { get; set; }
        public string PKSchema { get; set; }
        public string PKTable { get; set; }
        public string PKColumn { get; set; }
        public string DataType { get; set; }
    }
}