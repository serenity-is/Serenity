using Serenity.Services;

namespace Serenity.Data
{
    public interface ICustomFieldDefinition
    {
        int Id { get; }
        string Table { get; }
        CustomFieldType FieldType { get; }
        int FieldGroup { get; }
        int DisplayOrder { get; }
        bool IsRequired { get; }
        int Size { get; }
        int Precision { get; }
        string Name { get; }
        string Title { get; }
        string DefaultValue { get; }
        string Category { get;  }
        bool IsLocalizable { get; }
        string EditorType { get; }
        string EditorOptions { get; }
        string ValueList { get; }
        string OtherProperties { get; }
        int IsActive { get; }
    }

    public class CustomFieldDefinition : ICustomFieldDefinition
    {
        public int Id { get; set; }
        public string Table { get; set; }
        public CustomFieldType FieldType { get; set; }
        public int FieldGroup { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsRequired { get; set; }
        public int Size { get; set; }
        public int Precision { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string DefaultValue { get; set; }
        public string Category { get; set; }
        public bool IsLocalizable { get; set; }
        public string EditorType { get; set; }
        public string EditorOptions { get; set; }
        public string ValueList { get; set; }
        public string OtherProperties { get; set; }
        public int IsActive { get; set; }
    }
}