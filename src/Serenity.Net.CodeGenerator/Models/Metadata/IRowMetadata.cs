namespace Serenity.CodeGenerator;

public interface IRowMetadata : IClassMetadata
{
    IRowPropertyMetadata GetTableField(string columnName);
    IRowPropertyMetadata GetProperty(string name);
    bool HasLookupScriptAttribute { get; }
    string IdProperty { get; }
    string NameProperty { get; }
}