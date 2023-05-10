namespace Serenity.CodeGenerator;

public interface IBuildProjectItem
{
    string EvaluatedInclude { get; }
    string GetMetadataValue(string name);
    string ItemType { get; }
}