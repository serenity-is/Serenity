using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace RootProjectWizard
{
    public class ChildWizard : IWizard 
    {
        private string wizardData;

        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            replacementsDictionary["$ext_safeprojectname$"] = RootWizard.GlobalDictionary["$ext_safeprojectname$"];
            replacementsDictionary["$ext_projectname$"] = RootWizard.GlobalDictionary["$ext_projectname$"];

            try
            {
                string localDBInstance = "MSSqlLocalDB";
                replacementsDictionary["connectionString=\"Data Source=(LocalDb)\\v11.0;"] =
                    "connectionString=\"Data Source=(LocalDb)\\" + localDBInstance + ";";
                replacementsDictionary["connectionString=\"Data Source=(LocalDb)\\MsSqlLocalDB;"] =
                    "connectionString=\"Data Source=(LocalDb)\\" + localDBInstance + ";";
            }
            catch 
            {
            }

            if (!replacementsDictionary.TryGetValue("$wizarddata$", out wizardData))
                wizardData = null;
        }

        public void BeforeOpeningFile(EnvDTE.ProjectItem projectItem)
        {
        }

        public IEnumerable<Tuple<string, ProjectItem>> Recurse(string path, ProjectItems i)
        {
            if (i == null)
                yield break;

            path = string.IsNullOrEmpty(path) ? "" : path + @"\";
            foreach (ProjectItem j in i)
            {
                var path2 = path + j.Name;
                yield return new Tuple<string, ProjectItem>(path2, j);
                foreach (var k in Recurse(path2, j.ProjectItems))
                    yield return k;
            }
        }

        XNamespace ns = "http://schemas.microsoft.com/developer/vstemplate/2005";

        public void ProjectFinishedGenerating(EnvDTE.Project project)
        {
            project.DTE.StatusBar.Text = "ASP.NET Core Project";

            if (!string.IsNullOrEmpty(wizardData))
            {
                var data = XElement.Parse("<data>" + wizardData + "</data>");

                try
                {
                    project.DTE.StatusBar.Text = "Removing excluded feature files";
                    RemoveExcludedFiles(project, data);
                    project.DTE.StatusBar.Text = "Processing feature conditionals";
                    PreprocessConditionals(project, data);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occured while configuring features\r\n\r\n" +
                        ex.ToString());
                }
            }

            try
            {
                project.DTE.StatusBar.Text = "Running DotNet Restore...";
                System.Diagnostics.Process.Start(new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = "restore",
                    WorkingDirectory = System.IO.Path.GetDirectoryName(project.FullName)
                }).WaitForExit(120000);

                project.DTE.StatusBar.Text = "Running DotNet Tool Restore...";
                System.Diagnostics.Process.Start(new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = "tool restore",
                    WorkingDirectory = System.IO.Path.GetDirectoryName(project.FullName)
                }).WaitForExit(120000);

                project.DTE.StatusBar.Text = "Running DotNet Sergen Restore...";
                System.Diagnostics.Process.Start(new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = "sergen restore",
                    WorkingDirectory = System.IO.Path.GetDirectoryName(project.FullName)
                }).WaitForExit(120000);

                project.DTE.StatusBar.Text = "Running DotNet Build...";
                System.Diagnostics.Process.Start(new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = "build",
                    WorkingDirectory = System.IO.Path.GetDirectoryName(project.FullName)
                }).WaitForExit(120000);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void PreprocessConditionals(EnvDTE.Project project, XElement data)
        {
            var elConditionals = data.Element(ns + "conditionals");
            var conditionals = GetPathMatcher(elConditionals);

            foreach (var item in Recurse("", project.ProjectItems).ToList())
            {
                var path = item.Item1;
                if (string.IsNullOrEmpty(path))
                    continue;

                if (conditionals.IsMatch(path))
                {
                    PreprocessConditional(project, (string)item.Item2.Properties.Item("FullPath").Value);
                }
            }

            PreprocessConditional(project, project.FullName);
        }

        private void PreprocessConditional(Project project, string fullPath)
        {
            if (!File.Exists(fullPath))
                return;

            project.DTE.StatusBar.Text = "Processing Conditionals in File: " + fullPath;
            var content = File.ReadAllText(fullPath);
            content = Build.Shared.PreprocessConditionals(content, RootWizard.SelectedFeatures);
            File.WriteAllText(fullPath, content, Build.Shared.UTF8Bom);
        }

        private PathMatcher GetPathMatcher(XElement node)
        {
            var includes = node.Elements(ns + "files")
                .Where(x => x.Attribute("include") != null)
                .Select(x => x.Attribute("include").Value)
                .Where(x => x != null);

            var excludes = node.Elements(ns + "files")
                .Where(x => x.Attribute("exclude") != null)
                .Select(x => x.Attribute("exclude").Value)
                .Where(x => x != null);

            return new PathMatcher(includes, excludes);
        }

        private void RemoveExcludedFiles(EnvDTE.Project project, XElement data)
        {
            var selectedMatchers = new List<PathMatcher>();
            var unselectedMatchers = new List<PathMatcher>();
            var elFeatures = data.Elements(ns + "features");
            var elFeatureList = elFeatures.Elements(ns + "feature");
            foreach (var elFeature in elFeatureList)
            {
                var matcher = GetPathMatcher(elFeature);
                if (RootWizard.SelectedFeatures.Contains(elFeature.Attribute("key").Value))
                    selectedMatchers.Add(matcher);
                else
                    unselectedMatchers.Add(matcher);
            }

            var deleteList = new List<string>();
            foreach (var item in Recurse("", project.ProjectItems).ToList())
            {
                var path = item.Item1;
                if (string.IsNullOrEmpty(path))
                    continue;

                if (unselectedMatchers.Any(x => x.IsMatch(path)))
                {
                    if (!selectedMatchers.Any(x => x.IsMatch(path)))
                    {
                        project.DTE.StatusBar.Text = "Deleting Excluded Feature File: " + path;
                        deleteList.Add(item.Item2.Properties.Item("FullPath").Value.ToString());
                    }
                }
            }

            foreach (var name in deleteList)
            {
                try
                {
                    if (File.Exists(name))
                        File.Delete(name);
                    else if (Directory.Exists(name))
                        Directory.Delete(name, true);
                }
                catch
                {
                    project.DTE.StatusBar.Text = "Error deleting: " + name;
                }
            }
        }

        public void ProjectItemFinishedGenerating(EnvDTE.ProjectItem projectItem)
        {
        }

        public void RunFinished()
        {
        }

        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }
    }
}
