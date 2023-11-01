namespace Serenity.CodeGenerator;

public interface IRowMetadata : IClassMetadata
{
    IRowPropertyMetadata GetTableField(string columnName);
    IRowPropertyMetadata GetProperty(string name);
    bool HasLookupScriptAttribute { get; }
    string ListServiceRoute { get; }
    string IdProperty { get; }
    string NameProperty { get; }
    string Module { get; }
}