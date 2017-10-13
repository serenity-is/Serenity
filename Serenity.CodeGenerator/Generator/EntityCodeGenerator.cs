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
        private string modules = "Modules/".Replace('/', Path.DirectorySeparatorChar);
        private string serverTypings;

        public EntityCodeGenerator(EntityModel model, GeneratorConfig config, string csproj)
        {
            var kdiff3Paths = new[]
            {
                config.KDiff3Path
            };

            this.model = model;
            CodeFileHelper.Kdiff3Path = kdiff3Paths.FirstOrDefault(File.Exists);
            CodeFileHelper.TSCPath = config.TSCPath ?? "tsc";

            this.rootDir = Path.GetDirectoryName(csproj);
            this.config = config;
            this.model.CustomSettings = config.CustomSettings;

            this.serverTypings = Path.Combine(rootDir, "Modules/Common/Imports/ServerTypings/".Replace('/', Path.DirectorySeparatorChar));
            if (!Directory.Exists(serverTypings))
                this.serverTypings = Path.Combine(rootDir, "Imports/ServerTypings/".Replace('/', Path.DirectorySeparatorChar));
        }

        public void Run()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(rootDir));

            if (config.GenerateRow)
            {
                GenerateRow();
                GenerateRowTyping();
            }

            if (config.GenerateService)
            {
                GenerateRepository();
                GenerateEndpoint();
                GenerateServiceTyping();
            }

            if (config.GenerateUI)
            {
                GeneratePage();
                GenerateIndexView();
                GenerateNavigationLink();
                GenerateStyle();
                GenerateColumns();
                GenerateForm();
                GenerateDialog();
                GenerateGrid();
                GenerateFormTyping();
            }
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

        private string BackupFile(string file)
        {
            if (File.Exists(file))
            {
                var backupFile = string.Format("{0}.{1}.bak", file, DateTime.Now.ToString("yyyyMMdd_HHmmss"));
                CodeFileHelper.CheckoutAndWrite(backupFile, File.ReadAllBytes(file), false);
                return backupFile;
            }

            return null;
        }

        private void CreateNewSiteWebFile(string code, string relativeFile, string dependentUpon = null)
        {
            string file = Path.Combine(rootDir, relativeFile);
            var backup = CreateDirectoryOrBackupFile(file);
            CodeFileHelper.CheckoutAndWrite(file, code, true);
            CodeFileHelper.MergeChanges(backup, file);
        }

        private void GenerateRow()
        {
            CreateNewSiteWebFile(Templates.Render("Row", model),
                Path.Combine(modules, Path.Combine(model.Module ?? model.RootNamespace, 
                    Path.Combine(model.ClassName, model.RowClassName + ".cs"))));
        }

        private void GenerateStyle()
        {
            string relativeFile = Path.Combine(@"wwwroot/Content/site/".Replace('/', Path.DirectorySeparatorChar), "site" +  
                (!string.IsNullOrEmpty(model.Module) ? ("." + model.Module.ToLowerInvariant()) : "") + ".less");

            string file = Path.Combine(rootDir, relativeFile);

            if (!string.IsNullOrEmpty(model.Module))
            {
                var siteLess = Path.Combine("wwwroot/Content/site/".Replace('/', Path.DirectorySeparatorChar), "site.less");
                var siteLessFile = Path.Combine(rootDir, siteLess);
                if (File.Exists(siteLessFile))
                {
                    var importLine = "@import \"site." + model.Module.ToLowerInvariant() + ".less";
                    var lines = File.ReadAllLines(siteLessFile).ToList();
                    if (!lines.Any(x => (x ?? "").ToLowerInvariant().IsTrimmedSame(importLine)))
                    {
                        var index = lines.FindLastIndex(x =>
                        {
                            return x.StartsWith("@import") ||
                                (x.StartsWith("//") && x.Contains("if:"));
                        });

                        if (index < 0)
                            index = lines.Count;

                        lines.Insert(index, importLine);
                        CreateNewSiteWebFile(string.Join(Environment.NewLine, lines), siteLess);
                    }
                }
            }

            if (!File.Exists(file))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(file));
                CreateNewSiteWebFile("@import \"site.mixins.less\";" + Environment.NewLine, relativeFile);
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
            string relativeFile = string.IsNullOrEmpty(model.Module) ?
                "Modules/Common/Navigation/NavigationItems.cs" :
                "Modules/" + model.ModuleSlash + model.Module + "Navigation.cs";
            relativeFile = relativeFile.Replace('/', Path.DirectorySeparatorChar);

            string file = Path.Combine(rootDir, relativeFile);
            string code = Templates.Render("NavigationLink", model);

            if (!File.Exists(file))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(file));
                CreateNewSiteWebFile(code, relativeFile);
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
                    if (lines.Find(x => x.TrimToEmpty().Replace(" ", "")
                        .IsTrimmedSame(usng.TrimToEmpty().Replace(" ", ""))) == null)
                    {
                        lines.Insert(usingIndex, usng);
                        usingIndex++;
                    }
                }

                if (!lines.Any(x => x.Contains("MyPages." + model.ClassName + "Controller")))
                {
                    var insertIndex = lines.FindLastIndex(x => !string.IsNullOrWhiteSpace(x)) + 1;
                    foreach (var z in toInsert.Where(x => !string.IsNullOrWhiteSpace(x) &&
                        !x.TrimToEmpty().StartsWith("using ")))
                        lines.Insert(insertIndex, z);
                }

                CodeFileHelper.CheckoutAndWrite(file, string.Join(Environment.NewLine, lines), false);
            }
        }

        private void GenerateColumns()
        {
            CreateNewSiteWebFile(Templates.Render("Columns", model), 
                Path.Combine(modules, Path.Combine(model.Module ?? model.RootNamespace, 
                    Path.Combine(model.ClassName, model.ClassName + "Columns.cs"))));
        }

        private void GenerateForm()
        {
            CreateNewSiteWebFile(Templates.Render("Form", model), 
                Path.Combine(modules, Path.Combine(model.Module ?? model.RootNamespace, 
                    Path.Combine(model.ClassName, model.ClassName + "Form.cs"))));
        }

        private void GenerateRepository()
        {
            CreateNewSiteWebFile(Templates.Render("Repository", model), 
                Path.Combine(modules, Path.Combine(model.Module ?? model.RootNamespace, 
                    Path.Combine(model.ClassName, model.ClassName + "Repository.cs"))));
        }

        private void GenerateEndpoint()
        {
            CreateNewSiteWebFile(Templates.Render("Endpoint", model), 
                Path.Combine(modules, Path.Combine(model.Module ?? model.RootNamespace, 
                    Path.Combine(model.ClassName, model.ClassName + "Endpoint.cs"))));
        }

        private void GeneratePage()
        {
            CreateNewSiteWebFile(Templates.Render("Page", model), 
                Path.Combine(modules, Path.Combine(model.Module ?? model.RootNamespace, 
                    Path.Combine(model.ClassName, model.ClassName + "Page.cs"))));
        }

        private void GenerateIndexView()
        {
            CreateNewSiteWebFile(Templates.Render("IndexView", model), 
                Path.Combine(modules, Path.Combine(model.Module ?? model.RootNamespace, 
                    Path.Combine(model.ClassName, model.ClassName + "Index.cshtml"))));
        }

        private void GenerateRowTyping()
        {
            var targetFile = model.RowClassName + ".ts";

            if (model.Module != null)
                targetFile = model.Module + "." + targetFile;

            targetFile = Path.Combine(Path.GetDirectoryName(serverTypings), targetFile);

            var content = Templates.Render("RowTyping", model);

            CreateNewSiteWebFile(content, targetFile, serverTypings);
        }

        private void GenerateServiceTyping()
        {
            var targetFile = model.ClassName + "Service.ts";

            if (model.Module != null)
                targetFile = model.Module + "." + targetFile;

            targetFile = Path.Combine(Path.GetDirectoryName(serverTypings), targetFile);

            var content = Templates.Render("ServiceTyping", model);

            CreateNewSiteWebFile(content, targetFile, serverTypings);
        }

        private void GenerateFormTyping()
        {
            var targetFile = model.ClassName + "Form.ts";

            if (model.Module != null)
                targetFile = model.Module + "." + targetFile;

            targetFile = Path.Combine(Path.GetDirectoryName(serverTypings), targetFile);

            var content = Templates.Render("FormTyping", model);

            CreateNewSiteWebFile(content, targetFile, serverTypings);
        }

        private void GenerateGrid()
        {
            CreateNewSiteWebFile(Templates.Render("Grid", model),
                Path.Combine(modules, Path.Combine(model.Module ?? model.RootNamespace, 
                    Path.Combine(model.ClassName, model.ClassName + "Grid.ts"))));
        }

        private void GenerateDialog()
        {
            CreateNewSiteWebFile(Templates.Render("Dialog", model),
                Path.Combine(modules, Path.Combine(model.Module ?? model.RootNamespace, 
                    Path.Combine(model.ClassName, model.ClassName + "Dialog.ts"))));
        }
    }
}