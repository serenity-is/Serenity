using Microsoft.Build.Evaluation;

namespace Serenity.CodeGenerator.MSBuild;

public class MSBuildProjectItem(ProjectItem projectItem) : IBuildProjectItem
{
    private readonly ProjectItem projectItem = projectItem ?? throw new ArgumentNullException(nameof(projectItem));

    public string EvaluatedInclude => projectItem.EvaluatedInclude;
    public string GetMetadataValue(string name) => projectItem.GetMetadataValue(name);
    public string ItemType => projectItem.ItemType;

}