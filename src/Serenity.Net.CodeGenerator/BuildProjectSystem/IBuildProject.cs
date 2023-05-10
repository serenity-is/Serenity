namespace Serenity.CodeGenerator;

public interface IBuildProject
{
    IEnumerable<IBuildProjectItem> AllEvaluatedItems { get; }
}