using Microsoft.Build.Definition;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Locator;

namespace Serenity.CodeGenerator.MSBuild;

public class MSBuildProjectSystem : IBuildProjectSystem
{
    static MSBuildProjectSystem()
    {
        try
        {
            MSBuildLocator.RegisterDefaults();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("Error while initializing MSBuildLocator: " + ex.ToString());
        }
    }

    public IBuildProject LoadProject(string path)
    {
        var project = ProjectCollection.GlobalProjectCollection
            .GetLoadedProjects(path).FirstOrDefault();

        if (project == null)
        {
            ProjectCollection.GlobalProjectCollection.IsBuildEnabled = false;
            ProjectCollection.GlobalProjectCollection.OnlyLogCriticalEvents = true;

            project = Project.FromFile(path, new ProjectOptions
            {
                GlobalProperties = new Dictionary<string, string>
                {
                    ["DesignTimeBuild"] = "true"
                },
                LoadSettings = 
                    ProjectLoadSettings.IgnoreEmptyImports |
                    ProjectLoadSettings.IgnoreInvalidImports |
                    ProjectLoadSettings.IgnoreMissingImports |
                    ProjectLoadSettings.DoNotEvaluateElementsWithFalseCondition
            });
        }

        return new MSBuildProject(project);
    }
}