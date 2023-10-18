namespace Serenity.CodeGenerator;

public interface IRowPropertyMetadata : IPropertyMetadata
{
    string ColumnName { get; }
}