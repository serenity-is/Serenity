using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Serenity.CodeGenerator
{
    public class EntityCodeGenerator
    {
        private readonly GeneratorConfig config;
        private readonly EntityModel model;
        private readonly string rootDir;
        private readonly Encoding utf8 = new UTF8Encoding(true);
        private readonly string modulePath;
        private readonly string moduleClass;
        private readonly string typingClass;

        public EntityCodeGenerator(EntityModel model, GeneratorConfig config, string csproj)
        {
            this.model = model;

            rootDir = Path.GetDirectoryName(Path.GetFullPath(csproj));
            this.config = config;
            this.model.CustomSettings = config.CustomSettings;

            var serverTypings = PathHelper.ToPath(Path.Combine(rootDir, PathHelper.ToPath("Modules/Common/Imports/ServerTypings/")));
            if (!Directory.Exists(serverTypings))
                serverTypings = Path.Combine(rootDir, PathHelper.ToPath("Imports/ServerTypings/"));

            typingClass = Path.Combine(serverTypings, model.ModuleDot + model.ClassName);

            modulePath = Path.Combine(rootDir, "Modules"); 
            if (!string.IsNullOrEmpty(model.Module))
                modulePath = Path.Combine(modulePath, model.Module);

            moduleClass = Path.Combine(modulePath, Path.Combine(model.ClassName, model.ClassName));
        }

        public void Run()
        {
            if (config.GenerateRow)
            {
                CreateFile(Templates.Render("Row", model), moduleClass + "Row.cs");
                CreateFile(Templates.Render("RowTyping", model), typingClass + "Row.ts");
            }

            if (config.GenerateService)
            {
                var handlerClass = Path.Combine(modulePath, 
                    Path.Combine(model.ClassName, "RequestHandlers", model.ClassName));
                CreateFile(Templates.Render("DeleteHandler", model), handlerClass + "DeleteHandler.cs");
                CreateFile(Templates.Render("ListHandler", model), handlerClass + "ListHandler.cs");
                CreateFile(Templates.Render("RetrieveHandler", model), handlerClass + "RetrieveHandler.cs");
                CreateFile(Templates.Render("SaveHandler", model), handlerClass + "SaveHandler.cs");

                CreateFile(Templates.Render("Endpoint", model), moduleClass + "Endpoint.cs");
                CreateFile(Templates.Render("ServiceTyping", model), typingClass + "Service.ts");
            }

            if (config.GenerateUI)
            {
                CreateFile(Templates.Render("Page", model), moduleClass + "Page.cs");
                CreateFile(Templates.Render("IndexView", model), moduleClass + "Index.cshtml");
                CreateFile(Templates.Render("Columns", model), moduleClass + "Columns.cs");
                CreateFile(Templates.Render("Form", model), moduleClass + "Form.cs");
                CreateFile(Templates.Render("Dialog", model), moduleClass + "Dialog.ts");
                CreateFile(Templates.Render("Grid", model), moduleClass + "Grid.ts");
                CreateFile(Templates.Render("FormTyping", model), typingClass + "Form.ts");
                CreateFile(Templates.Render("ColumnsTyping", model), typingClass + "Columns.ts");

                GenerateNavigationLink();
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
                    if (templateKey.Contains("..", StringComparison.Ordinal) ||
                        templateKey.StartsWith("\\", StringComparison.Ordinal) ||
                        templateKey.StartsWith("//", StringComparison.Ordinal))
                        throw new ArgumentOutOfRangeException("templateFile");

                    var outputFile = pair.Value;
                    if (outputFile.Contains("..", StringComparison.Ordinal) || 
                        outputFile.StartsWith("\\", StringComparison.Ordinal) ||
                        outputFile.StartsWith("//", StringComparison.Ordinal))
                        throw new ArgumentOutOfRangeException("outputFile");

                    outputFile = string.Format(CultureInfo.InvariantCulture, outputFile, model.ClassName, model.Module, 
                        Path.GetDirectoryName(moduleClass), Path.GetDirectoryName(typingClass), rootDir);

                    var content = Templates.Render(templateKey, model);
                    if (!string.IsNullOrWhiteSpace(content))
                        CreateFile(content, outputFile);
                }
            }
        }

        private static string CreateDirectoryOrBackupFile(string file)
        {
            if (File.Exists(file))
            {
                var backupFile = string.Format(CultureInfo.InvariantCulture, "{0}.{1}.bak", file, 
                    DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture));
                CodeFileHelper.CheckoutAndWrite(backupFile, File.ReadAllBytes(file), false);
                return backupFile;
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(file));
                return null;
            }
        }

        private static void CreateFile(string code, string file)
        {
            var backup = CreateDirectoryOrBackupFile(file);
            CodeFileHelper.CheckoutAndWrite(file, code, true);
            CodeFileHelper.MergeChanges(backup, file);
        }

        private void GenerateStyle()
        {
            string contentSite = PathHelper.ToPath("wwwroot/Content/site");
            string file = Path.Combine(rootDir, Path.Combine(contentSite,
                    "site" + model.DotModule.ToLowerInvariant() + ".less"));

            var siteLess = Path.Combine(rootDir, Path.Combine(contentSite, "site.less"));

            if (!File.Exists(siteLess) &&
                !File.Exists(file))
            {
                // probably newer template where we don't use less
                return;
            }

            if (!string.IsNullOrEmpty(model.Module) &&
                File.Exists(siteLess))
            {
                var importLine = "@import \"site." + model.Module.ToLowerInvariant() + ".less\";";
                var lines = File.ReadAllLines(siteLess).ToList();
                if (!lines.Any(x => string.Compare(x ?? "", importLine, StringComparison.OrdinalIgnoreCase) == 0))
                {
                    var index = lines.FindLastIndex(x =>
                    {
                        return x.StartsWith("@import", StringComparison.Ordinal) ||
                            (x.StartsWith("//", StringComparison.Ordinal) && x.Contains("if:", StringComparison.Ordinal));
                    });

                    if (index < 0)
                        index = lines.Count;
                    else
                        index++;

                    lines.Insert(index, importLine);
                    CodeFileHelper.CheckoutAndWrite(siteLess, string.Join(Environment.NewLine, lines), false);
                }
            }

            if (!File.Exists(file))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(file));
                CreateFile("@import \"site.mixins.less\";" + Environment.NewLine, file);
            }

            string code = Templates.Render("Style", model);
            using var ms = new MemoryStream();
            var firstLine = code.Replace("\r", "", StringComparison.Ordinal)
                .Split('\n').FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
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
            using var sw = new StreamWriter(ms, utf8);
            sw.Write(code);
            sw.Flush();

            CodeFileHelper.CheckoutAndWrite(file, ms.ToArray(), false);
        }

        private void GenerateNavigationLink()
        {
            string file = Path.Combine(rootDir, string.IsNullOrEmpty(model.Module) ?
                "Modules/Common/Navigation/NavigationItems.cs" :
                "Modules/" + model.ModuleSlash + model.Module + "Navigation.cs");
            file = PathHelper.ToPath(file);

            string code = Templates.Render("NavigationLink", model);

            if (!File.Exists(file))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(file));
                CreateFile(code, file);
            }
            else
            {
                var lines = File.ReadAllLines(file).ToList();
                var toInsert = code.Replace("\r", "", StringComparison.Ordinal).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                var usingIndex = lines.FindLastIndex(x => x.TrimToEmpty().StartsWith("using ", StringComparison.Ordinal));
                if (usingIndex < 0)
                    usingIndex = 0;

                foreach (var usng in toInsert.Where(x => x.TrimToEmpty().StartsWith("using ", StringComparison.Ordinal)))
                {
                    if (lines.Find(x => x.TrimToEmpty().Replace(" ", "", StringComparison.Ordinal)
                        .IsTrimmedSame(usng.TrimToEmpty().Replace(" ", "", StringComparison.Ordinal))) == null)
                    {
                        lines.Insert(usingIndex, usng);
                        usingIndex++;
                    }
                }

                if (!lines.Any(x => x.Contains("MyPages." + model.ClassName + "Controller", StringComparison.Ordinal)))
                {
                    var insertIndex = lines.FindLastIndex(x => !string.IsNullOrWhiteSpace(x)) + 1;
                    foreach (var z in toInsert.Where(x => !string.IsNullOrWhiteSpace(x) &&
                        !x.TrimToEmpty().StartsWith("using ", StringComparison.Ordinal)))
                        lines.Insert(insertIndex, z);
                }

                CodeFileHelper.CheckoutAndWrite(file, string.Join(Environment.NewLine, lines), false);
            }
        }
    }
}