using Microsoft.Build.Definition;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Locator;
using System;
using System.Linq;

namespace Serenity.CodeGenerator.MSBuild
{
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
                project = Project.FromFile(path, new ProjectOptions
                {
                    LoadSettings = ProjectLoadSettings.IgnoreEmptyImports |
                    ProjectLoadSettings.IgnoreEmptyImports |
                    ProjectLoadSettings.IgnoreInvalidImports |
                    ProjectLoadSettings.DoNotEvaluateElementsWithFalseCondition
                });
            }

            return new MSBuildProject(project);
        }
    }
}