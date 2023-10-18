namespace Serenity.CodeGenerator;

public interface IRowPropertyMetadata : IPropertyMetadata
{
    bool IsIdProperty { get; }
}