using Microsoft.Build.Evaluation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Serenity.CodeGenerator.MSBuild
{
    public class MSBuildProject : IBuildProject
    {
        private readonly Project project;

        public MSBuildProject(Project project)
        {
            this.project = project ?? throw new ArgumentNullException(nameof(project));
            
        }

        public IEnumerable<IBuildProjectItem> AllEvaluatedItems =>
            project.AllEvaluatedItems.Select(x => new MSBuildProjectItem(x));
    }
}