using Microsoft.Build.Evaluation;

namespace Serenity.CodeGenerator.MSBuild;

public class MSBuildProject(Project project) : IBuildProject
{
    private readonly Project project = project ?? throw new ArgumentNullException(nameof(project));

    public IEnumerable<IBuildProjectItem> AllEvaluatedItems =>
        project.AllEvaluatedItems.Select(x => new MSBuildProjectItem(x));
}