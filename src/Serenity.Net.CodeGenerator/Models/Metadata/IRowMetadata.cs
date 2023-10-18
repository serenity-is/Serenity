namespace Serenity.CodeGenerator;

public interface IRowMetadata : IClassMetadata
{
    IRowPropertyMetadata GetTableField(string columnName);
    bool HasLookupScriptAttribute { get; }
}