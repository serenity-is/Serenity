using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Serenity.CodeGenerator
{
    public class EntityCodeGenerator
    {
        private GeneratorConfig config;
        private EntityModel model;
        private string rootDir;
        private Encoding utf8 = new UTF8Encoding(true);
        private string csproj;
        private string moduleClass;
        private string typingClass;
        private string serverTypingsTT;

        public EntityCodeGenerator(EntityModel model, GeneratorConfig config, string csproj)
        {
            this.model = model;
            this.csproj = csproj;

            this.rootDir = Path.GetDirectoryName(csproj);
            this.config = config;
            this.model.CustomSettings = config.CustomSettings;

            var serverTypings = Path.Combine(rootDir, "Modules/Common/Imports/ServerTypings/"
                .Replace('/', Path.DirectorySeparatorChar));
            if (!Directory.Exists(serverTypings))
                serverTypings = Path.Combine(rootDir, "Imports/ServerTypings/".Replace('/', Path.DirectorySeparatorChar));

            serverTypingsTT = (serverTypings.Substring(rootDir.Length + 1) + "ServerTypings.tt").Replace('/', '\\');

            typingClass = Path.Combine(serverTypings, model.ModuleDot + model.ClassName);

            var modulePath = Path.Combine(rootDir, "Modules");
            if (!string.IsNullOrEmpty(model.Module))
                modulePath = Path.Combine(modulePath, model.Module);

            moduleClass = Path.Combine(modulePath, Path.Combine(model.ClassName, model.ClassName));
        }

        public void Run()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(rootDir));

            if (config.GenerateRow)
            {
                CreateFile(Templates.Render("Row", model), moduleClass + "Row.cs");
                CreateFile(Templates.Render("RowTyping", model), typingClass + "Row.ts", serverTypingsTT);
            }

            if (config.GenerateService)
            {
                CreateFile(Templates.Render("Repository", model), moduleClass + "Repository.cs");
                CreateFile(Templates.Render("Endpoint", model), moduleClass + "Endpoint.cs");
                CreateFile(Templates.Render("ServiceTyping", model), typingClass + "Service.ts", serverTypingsTT);
            }

            if (config.GenerateUI)
            {
                CreateFile(Templates.Render("Page", model), moduleClass + "Page.cs");
                CreateFile(Templates.Render("IndexView", model), moduleClass + "Index.cshtml");
                CreateFile(Templates.Render("Columns", model), moduleClass + "Columns.cs");
                CreateFile(Templates.Render("Form", model), moduleClass + "Form.cs");
                CreateFile(Templates.Render("Dialog", model), moduleClass + "Dialog.ts");
                CreateFile(Templates.Render("Grid", model), moduleClass + "Grid.ts");
                CreateFile(Templates.Render("FormTyping", model), typingClass + "Form.ts", serverTypingsTT);
                GenerateNavigationLink();
                GeneratePermissionKeys();
                GenerateStyle();
            }

            if (config.CustomGenerate != null &&
                config.GenerateCustom)
            {
                foreach (var pair in config.CustomGenerate)
                {
                    if (string.IsNullOrEmpty(pair.Value))
                        continue;

                    var templateKey = pair.Key;
                    if (templateKey.IndexOf("..") >= 0 ||
                        templateKey.StartsWith("\\") ||
                        templateKey.StartsWith("//"))
                        throw new ArgumentOutOfRangeException("templateFile");

                    var outputFile = pair.Value;
                    if (outputFile.IndexOf("..") >= 0 ||
                        outputFile.StartsWith("\\") ||
                        outputFile.StartsWith("//"))
                        throw new ArgumentOutOfRangeException("outputFile");

                    outputFile = String.Format(outputFile, model.ClassName, model.Module,
                        Path.GetDirectoryName(moduleClass), Path.GetDirectoryName(typingClass), rootDir);

                    var content = Templates.Render(templateKey, model);
                    if (!string.IsNullOrWhiteSpace(content))
                        CreateFile(content, outputFile);
                }
            }
        }

        private string CreateDirectoryOrBackupFile(string file)
        {
            if (File.Exists(file))
            {
                var backupFile = string.Format("{0}.{1}.bak", file, DateTime.Now.ToString("yyyyMMdd_HHmmss"));
                CodeFileHelper.CheckoutAndWrite(backupFile, File.ReadAllBytes(file), false);
                return backupFile;
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(file));
                return null;
            }
        }

        private void CreateFile(string code, string file, string dependentUpon = null)
        {
            var backup = CreateDirectoryOrBackupFile(file);
            CodeFileHelper.CheckoutAndWrite(file, code, true);
            CodeFileHelper.MergeChanges(backup, file);
#if !ASPNETCORE
            ProjectFileHelper.AddFileToProject(this.csproj,
                file.Substring(Path.GetDirectoryName(csproj).Length + 1).Replace('/', '\\'), dependentUpon);
#endif
        }

        private void GenerateStyle()
        {
#if ASPNETCORE
            string contentSite = "wwwroot/Content/site".Replace('/', Path.DirectorySeparatorChar);
#else
            string contentSite = "Content/site".Replace('/', Path.DirectorySeparatorChar);
#endif
            string file = Path.Combine(rootDir, Path.Combine(contentSite,
                    "site" + model.DotModule.ToLowerInvariant() + ".less"));

            if (!string.IsNullOrEmpty(model.Module))
            {
                var siteLess = Path.Combine(rootDir, Path.Combine(contentSite, "site.less"));

                if (File.Exists(siteLess))
                {
                    var importLine = "@import \"site." + model.Module.ToLowerInvariant() + ".less\";";
                    var lines = File.ReadAllLines(siteLess).ToList();
                    if (!lines.Any(x => (x ?? "").ToLowerInvariant().IsTrimmedSame(importLine)))
                    {
                        var index = lines.FindLastIndex(x =>
                        {
                            return x.StartsWith("@import") ||
                                (x.StartsWith("//") && x.Contains("if:"));
                        });

                        if (index < 0)
                            index = lines.Count;
                        else
                            index++;

                        lines.Insert(index, importLine);
                        CodeFileHelper.CheckoutAndWrite(siteLess, string.Join(Environment.NewLine, lines), false);
                    }
                }
            }

            if (!File.Exists(file))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(file));
                CreateFile("@import \"site.mixins.less\";" + Environment.NewLine, file);
            }

            string code = Templates.Render("Style", model);
            using (var ms = new MemoryStream())
            {
                var firstLine = code.Replace("\r", "").Split('\n').FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
                if (!string.IsNullOrWhiteSpace(firstLine))
                {
                    var lines = File.ReadAllLines(file);
                    // don't generate less for dialog multiple times
                    if (lines.Any(x => x.IsTrimmedSame(firstLine)))
                        return;
                }

                var old = File.ReadAllBytes(file);
                if (old.Length > 0)
                    ms.Write(old, 0, old.Length);
                using (var sw = new StreamWriter(ms, utf8))
                {
                    sw.Write(code);
                    sw.Flush();

                    CodeFileHelper.CheckoutAndWrite(file, ms.ToArray(), false);
                }
            }
        }

        private void GenerateNavigationLink()
        {
            string file = Path.Combine(rootDir, string.IsNullOrEmpty(model.Module) ?
                "Modules/Common/Navigation/NavigationItems.cs" :
                "Modules/" + model.ModuleSlash + model.Module + "Navigation.cs");
            file = file.Replace('/', Path.DirectorySeparatorChar);

            string code = Templates.Render("NavigationLink", model);

            if (!File.Exists(file))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(file));
                CreateFile(code, file);
            }
            else
            {
                var lines = File.ReadAllLines(file).ToList();
                var toInsert = code.Replace("\r", "").Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                var usingIndex = lines.FindLastIndex(x => x.TrimToEmpty().StartsWith("using "));
                if (usingIndex < 0)
                    usingIndex = 0;

                foreach (var usng in toInsert.Where(x => x.TrimToEmpty().StartsWith("using ")))
                {
                    if (lines.Find(x => x.TrimToEmpty().Replace(" ", "").IsTrimmedSame(usng.TrimToEmpty().Replace(" ", ""))) == null)
                    {
                        lines.Insert(usingIndex, usng);
                        usingIndex++;
                    }
                }

                if (!lines.Any(x => x.Contains("MyPages." + model.ClassName + "Controller")))
                {
                    var insertIndex = lines.FindLastIndex(x => !string.IsNullOrWhiteSpace(x)) + 1;
                    foreach (var z in toInsert.Where(x => !string.IsNullOrWhiteSpace(x) && !x.TrimToEmpty().StartsWith("using ")))
                        lines.Insert(insertIndex, z);
                }

                CodeFileHelper.CheckoutAndWrite(file, string.Join(Environment.NewLine, lines), false);
            }
        }

        private void GeneratePermissionKeys()
        {
            string file = Path.Combine(rootDir, string.IsNullOrEmpty(model.Module) ?
                "Modules/Common/PermissionKeys/PermissionKeys.cs" :
                "Modules/" + model.ModuleSlash + model.Module + "PermissionKeys.cs");
            file = file.Replace('/', Path.DirectorySeparatorChar);

            string code = Templates.Render("PermissionKeys", model);

            if (!File.Exists(file))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(file));
                CreateFile(code, file);
            }
            else
            {
                var lines = File.ReadAllLines(file).ToList();
                var toInsert = code.Replace("\r", "").Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                var usingIndex = lines.FindLastIndex(x => x.TrimToEmpty().StartsWith("using "));
                if (usingIndex < 0)
                    usingIndex = 0;

                foreach (var usng in toInsert.Where(x => x.TrimToEmpty().StartsWith("using ")))
                {
                    if (lines.Find(x => x.TrimToEmpty().Replace(" ", "").IsTrimmedSame(usng.TrimToEmpty().Replace(" ", ""))) == null)
                    {
                        lines.Insert(usingIndex, usng);
                        usingIndex++;
                    }
                }

                if (!lines.Any(x => x.Contains(model.ClassName + "Read")))
                {
                    var insertIndex = lines.FindLastIndex(x => x.TrimToEmpty().StartsWith("{")) + 1;
                    foreach (var z in toInsert.Where(x => !string.IsNullOrWhiteSpace(x)
                                                && !x.TrimToEmpty().StartsWith("{")
                                                && !x.TrimToEmpty().StartsWith("}")
                                                && !x.TrimToEmpty().Contains("using ")
                                                && !x.TrimToEmpty().Contains("partial class ")
                                                && !x.TrimToEmpty().Contains("namespace ")

                                                ))
                    {
                        lines.Insert(insertIndex, z);
                        insertIndex++;
                    }
                }

                CodeFileHelper.CheckoutAndWrite(file, string.Join(Environment.NewLine, lines), false);
            }
        }
    }
}