using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace Serenity.CodeGenerator
{
    public class EntityCodeGenerator
    {
        private EntityCodeGenerationModel model;
        private string siteWebPath;
        private string siteWebProj;
        private string scriptPath;
        private string scriptProject;
        private string kdiff3Path;
        private Encoding utf8 = new UTF8Encoding(true);

        private void AppendComment(StreamWriter sw)
        {
            sw.WriteLine();
            sw.WriteLine();
            sw.WriteLine("/* ------------------------------------------------------------------------- */");
            sw.WriteLine("/* APPENDED BY CODE GENERATOR, MOVE TO CORRECT PLACE AND REMOVE THIS COMMENT */");
            sw.WriteLine("/* ------------------------------------------------------------------------- */");
        }

        public EntityCodeGenerator(EntityCodeGenerationModel model, GeneratorConfig config)
        {
            var kdiff3Paths = new[]
            {
                config.KDiff3Path, 
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "KDiff3\\kdiff3.exe"), 
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "KDiff3\\kdiff3.exe"), 
            };

            this.model = model;
            kdiff3Path = kdiff3Paths.FirstOrDefault(File.Exists);
            siteWebProj = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, config.WebProjectFile));
            siteWebPath = Path.GetDirectoryName(siteWebProj);
            scriptProject = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, config.ScriptProjectFile));
            scriptPath = Path.GetDirectoryName(scriptProject);
        }

        public void Run()
        {
            Directory.CreateDirectory(scriptPath);
            Directory.CreateDirectory(siteWebPath);
            Directory.CreateDirectory(scriptPath);

            GenerateRow();
            GenerateCss();
            GenerateForm();
            GenerateRepository();
            GenerateEndpoint();
            GeneratePageController();
            GeneratePageIndex();
            GenerateScriptGrid();
            GenerateScriptDialog();
        }

        private string CreateDirectoryOrBackupFile(string file)
        {
            if (File.Exists(file))
                return BackupFile(file);
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(file));
                return null;
            }
        }

        private void MergeChanges(string backup, string file)
        {
            if (backup == null || !File.Exists(backup) || !File.Exists(file))
                return;

            bool isEqual;
            using (var fs1 = new FileStream(backup, FileMode.Open))
            using (var fs2 = new FileStream(file, FileMode.Open))
                isEqual = StreamsContentsAreEqual(fs1, fs2);

            if (isEqual)
            {
                File.Delete(backup);
                return;
            }

            var generated = Path.ChangeExtension(file, Path.GetExtension(file) + ".gen.bak");
            File.Copy(file, generated, true);
            File.Copy(backup, file, true);

            if (kdiff3Path.IsNullOrEmpty() ||
                !File.Exists(kdiff3Path))
            {
                throw new InvalidOperationException(String.Format("KDiff3, verilen '{0}' konumunda bulunamadı!",
                    kdiff3Path ?? ""));
            }

            Process.Start(kdiff3Path, "--auto " + file + " " + generated + " -o " + file);
        }

        private static bool StreamsContentsAreEqual(Stream stream1, Stream stream2)
        {
            const int bufferSize = 2048 * 2;
            var buffer1 = new byte[bufferSize];
            var buffer2 = new byte[bufferSize];

            while (true)
            {
                int count1 = stream1.Read(buffer1, 0, bufferSize);
                int count2 = stream2.Read(buffer2, 0, bufferSize);

                if (count1 != count2)
                {
                    return false;
                }

                if (count1 == 0)
                {
                    return true;
                }

                int iterations = (int)Math.Ceiling((double)count1 / sizeof(Int64));
                for (int i = 0; i < iterations; i++)
                {
                    if (BitConverter.ToInt64(buffer1, i * sizeof(Int64)) != BitConverter.ToInt64(buffer2, i * sizeof(Int64)))
                    {
                        return false;
                    }
                }
            }
        }

        private string BackupFile(string file)
        {
            if (File.Exists(file))
            {
                var backupFile = string.Format("{0}.{1}.bak", file, DateTime.Now.ToString("yyyyMMdd_HHmmss"));
                File.Move(file, backupFile);
                return backupFile;
            }

            return null;
        }

        private void CreateNewSiteWebFile(string code, string relativeFile)
        {
            string file = Path.Combine(siteWebPath, relativeFile);
            var backup = CreateDirectoryOrBackupFile(file);
            using (var sw = new StreamWriter(file, false, utf8))
                sw.Write(code);
            MergeChanges(backup, file);
            AddFileToProject(siteWebProj, relativeFile);
        }

        private void CreateNewSiteScriptFile(string code, string relativeFile)
        {
            string file = Path.Combine(scriptPath, relativeFile);
            var backup = CreateDirectoryOrBackupFile(file);
            using (var sw = new StreamWriter(file, false, utf8))
                sw.Write(code);
            MergeChanges(backup, file);
            AddFileToProject(scriptProject, relativeFile);
        }

        private void GenerateRow()
        {
            CreateNewSiteWebFile(Templates.Render(new Views.EntityRow(), model),
                Path.Combine(@"Modules\", Path.Combine(model.Module ?? model.RootNamespace, Path.Combine(model.ClassName, model.RowClassName + ".cs"))));
        }

        private void GenerateCss()
        {
            string relativeFile = Path.Combine(@"Content\site\", "site.less");
            string file = Path.Combine(siteWebPath, relativeFile);
            Directory.CreateDirectory(Path.GetDirectoryName(file));
            if (!File.Exists(file))
                using (var sw = new StreamWriter(file, false, utf8))
                    sw.Write("\r\n");

            string code = Templates.Render(new Views.EntityCss(), model);
            using (var sw = new StreamWriter(file, true, utf8))
            {
                AppendComment(sw);
                sw.Write(code);
            }

            AddFileToProject(siteWebProj, relativeFile);
        }

        private void GenerateForm()
        {
            CreateNewSiteWebFile(Templates.Render(new Views.EntityForm(), new 
            { 
                ClassName = model.ClassName, 
                RowClassName = model.RowClassName,
                Module = model.Module,
                RootNamespace = model.RootNamespace,
                Fields = model.Fields,
                IdField = model.Identity
            }), Path.Combine(@"Modules\", Path.Combine(model.Module ?? model.RootNamespace, Path.Combine(model.ClassName, model.ClassName + "Form.cs"))));
        }

        private void GenerateRepository()
        {
            CreateNewSiteWebFile(Templates.Render(new Views.EntityRepository(), new
            {
                RootNamespace = model.RootNamespace,
                ClassName = model.ClassName,
                RowClassName = model.RowClassName,
                Module = model.Module,
                Permission = model.Permission
            }), Path.Combine(@"Modules\", Path.Combine(model.Module ?? model.RootNamespace, Path.Combine(model.ClassName, model.ClassName + "Repository.cs"))));
        }

        private void GenerateEndpoint()
        {
            CreateNewSiteWebFile(Templates.Render(new Views.EntityEndpoint(), new
            {
                Schema = model.Schema,
                RootNamespace = model.RootNamespace,
                ClassName = model.ClassName,
                RowClassName = model.RowClassName,
                Module = model.Module,
                Permission = model.Permission
            }), Path.Combine(@"Modules\", Path.Combine(model.Module ?? model.RootNamespace, Path.Combine(model.ClassName, model.ClassName + "Endpoint.cs"))));
        }

        private void GeneratePageController()
        {
            CreateNewSiteWebFile(Templates.Render(new Views.EntityPageController(), new 
            {
                Schema = model.Schema,
                RootNamespace = model.RootNamespace,
                ClassName = model.ClassName,
                RowClassName = model.RowClassName,
                Module = model.Module,
                Permission = model.Permission,
                NavigationCategory = model.Module
            }), Path.Combine(@"Modules\", Path.Combine(model.Module ?? model.RootNamespace, Path.Combine(model.ClassName, model.ClassName + "Page.cs"))));
        }

        private void GeneratePageIndex()
        {
            CreateNewSiteWebFile(Templates.Render(new Views.EntityPageIndex(), new
            {
                Schema = model.Schema,
                RootNamespace = model.RootNamespace,
                ClassName = model.ClassName,
                RowClassName = model.RowClassName,
                Module = model.Module,
                Permission = model.Permission,
                NavigationCategory = model.Module
            }), Path.Combine(@"Modules\", Path.Combine(model.Module ?? model.RootNamespace, Path.Combine(model.ClassName, model.ClassName + "Index.cshtml"))));
        }

        private void GenerateScriptGrid()
        {
            CreateNewSiteScriptFile(Templates.Render(new Views.EntityScriptGrid(), model),
                Path.Combine(model.Module ?? model.RootNamespace, Path.Combine(model.ClassName, model.ClassName + "Grid.cs")));
        }


        private void GenerateScriptDialog()
        {
            CreateNewSiteScriptFile(Templates.Render(new Views.EntityScriptDialog(), model),
                Path.Combine(model.Module ?? model.RootNamespace, Path.Combine(model.ClassName, model.ClassName + "Dialog.cs")));
        }

        private void AddFileToProject(string projectFile, string codeFile)
        {
            if (File.Exists(projectFile))
            {
                XElement doc;
                using (var sr = new StreamReader(projectFile))
                    doc = XElement.Parse(sr.ReadToEnd(), LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
                var ns = doc.GetDefaultNamespace();
                XElement g = null;
                foreach (var group in doc.Elements(ns + "ItemGroup"))
                {
                    foreach (var c in group.Elements(ns + "Compile")
                                .Concat(group.Elements(ns + "Content"))
                                .Concat(group.Elements(ns + "None")))
                        if (c.Attribute("Include").Value.ToLowerInvariant() == codeFile.ToLowerInvariant())
                        {
                            return; // already in project file
                        }
                }

                string contentType;
                if (codeFile.EndsWith(".cs"))
                    contentType = "Compile";
                else
                    contentType = "Content";

                foreach (var group in doc.Elements(ns + "ItemGroup"))
                {
                    var compiles = group.Elements(ns + contentType);
                    if (compiles.Count() > 0)
                    {
                        g = group;
                        break;
                    }
                }

                if (g != null)
                {
                    var newElement = new XElement(ns + contentType, new XAttribute("Include", codeFile));
                    var lastElement = g.Elements().LastOrDefault();
                    XText space = null;
                    if (lastElement != null)
                        space = lastElement.PreviousNode as XText;
                    if (lastElement != null)
                    {
                        if (space != null)
                            lastElement.AddAfterSelf(new XText(space.Value), newElement);
                        else
                            lastElement.AddAfterSelf(newElement);
                    }
                    else
                        g.Add(newElement);
                    
                    using (var sw = new StreamWriter(projectFile, false, new UTF8Encoding(true)))
                    {
                        sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                        sw.Write(doc.ToString());
                    }
                }
            }
        }

        /*
        private void GenerateNavigationLink()
        {
            string relativePath = 
            string path = Path.Combine(siteLibPath, @"Pages\Models\LeftNavigation\");
            Directory.CreateDirectory(path);

            string file = Path.Combine(path, "LeftNavigationModel_" + model.Module + ".cs");
            if (!File.Exists(file))
                using (var sw = new StreamWriter(file, false, utf8))
                    sw.Write("\r\n");

            string code = Templates.Render("EntityNavigationLink", new { 
                ClassSingular = model.ClassName,
                Permission = model.Permission
            });

            using (var sw = new StreamWriter(file, true, utf8))
            {
                AppendComment(sw);
                sw.Write(code);
            }
        }*/

        /*
        private void GenerateViews()
        {
            string relative = Path.Combine(@"Views\", Path.Combine(model.Module, model.ClassName + "Views.cs"));
            string code = Templates.Render("EntityViews", new
            {
                ClassSingular = model.ClassName,
                IdField = model.Identity,
                NameField = model.NameField,
                Fields = model.Fields,
                IsLoggingRow = model.RowBaseClass == "LoggingRow"
            });
            string file = Path.Combine(scriptPath, relative);
            CreateDirectoryOrBackupFile(file);
            using (var sw = new StreamWriter(file, false, utf8))
                sw.Write(code);
            string projFile = Path.Combine(scriptPath, "Script.Sinerji.csproj");
            AddFileToProject(projFile, relative);
        }*/

        private bool InsertDefinition(string file, string type, string key, string code)
        {
            int insertAfter = -1;
            int lastPermission = -1;
            bool alreadyAdded = false;

            List<string> lines = new List<string>();
            var spaceRegex = new Regex("\\s+");
            using (var sr = new StreamReader(file, utf8))
            {
                int lineNum = 0;
                while (true)
                {
                    string line = sr.ReadLine();
                    if (line == null)
                        break;
                    lines.Add(line);
                    line = spaceRegex.Replace(line.TrimToEmpty(), " ");
                    string s = " " + type + " ";
                    var idx = line.IndexOf(s);
                    if (idx >= 0)
                    {
                        var idx2 = line.IndexOf("=");
                        if (idx2 >= 0)
                        {
                            var ar = line.Substring(idx + s.Length, idx2 - idx - s.Length).TrimToNull();
                            if (ar != null)
                            {
                                int comp = String.CompareOrdinal(key, ar);
                                if (comp > 0)
                                    insertAfter = lineNum;
                                else if (comp == 0)
                                    alreadyAdded = true;
                            }
                        }
                    }
                        
                    lineNum++;
                }
            }

            if (alreadyAdded)
                return true;

            if (insertAfter == -1)
                insertAfter = lastPermission;

            if (insertAfter != -1)
            {
                lines.Insert(insertAfter + 1, code.TrimEnd());

                using (var sw = new StreamWriter(file, false, utf8))
                {
                    sw.Write(String.Join("\r\n", lines));
                }
                return true;
            }

            return false;
        }

    }
}