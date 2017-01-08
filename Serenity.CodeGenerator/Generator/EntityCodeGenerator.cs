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

        public EntityCodeGenerator(EntityModel model, GeneratorConfig config, string projectJson)
        {
            var kdiff3Paths = new[]
            {
                config.KDiff3Path
            };

            this.model = model;
            CodeFileHelper.Kdiff3Path = kdiff3Paths.FirstOrDefault(File.Exists);
            CodeFileHelper.TSCPath = config.TSCPath ?? "tsc";

            this.rootDir = Path.GetDirectoryName(projectJson);
            this.config = config;
        }

        public void Run()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(rootDir));

            if (config.GenerateRow)
                GenerateRow();

            if (config.GenerateDialog)
                GenerateCss();

            if (config.GenerateColumn)
                GenerateColumns();

            if (config.GenerateForm)
                GenerateForm();

            if (config.GenerateRepository)
                GenerateRepository();

            if (config.GenerateEndpoint)
                GenerateEndpoint();

            if (config.GeneratePage)
            {
                GeneratePageController();
                GeneratePageIndex();
            }

            if (config.GenerateTSTypings)
            {
                if (config.GenerateRow)
                    GenerateScriptRowTS();

                if (config.GenerateEndpoint)
                    GenerateScriptServiceTS();

                if (config.GenerateForm)
                    GenerateScriptFormTS();
            }

            if (config.GenerateTSCode)
            {
                if (config.GenerateGrid)
                    GenerateScriptGridTS();

                if (config.GenerateDialog)
                    GenerateScriptDialogTS();
                if (config.GenerateGridEditor)
                    GenerateScriptGridEditorTS();
                if (config.GenerateGridEditorDialog)
                    GenerateScriptGridEditorDialogTS();
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
            if (config.RowFieldsSurroundWithRegion)
            {
                CreateNewSiteWebFile(Templates.Render(new Views.EntityRowWithRegion(), model, config),
                    Path.Combine(modules, Path.Combine(model.Module ?? model.RootNamespace, 
                        Path.Combine(model.ClassName, model.RowClassName + ".cs"))));
            }
            else
            {
                CreateNewSiteWebFile(Templates.Render(new Views.EntityRow(), model),
                    Path.Combine(modules, Path.Combine(model.Module ?? model.RootNamespace, 
                        Path.Combine(model.ClassName, model.RowClassName + ".cs"))));
            }
        }

        private void GenerateCss()
        {
            string relativeFile = Path.Combine(@"Content/site/".Replace('/', Path.DirectorySeparatorChar), "site" +  
                (!string.IsNullOrEmpty(model.Module) ? ("." + model.Module.ToLowerInvariant()) : "") + ".less");

            string file = Path.Combine(rootDir, relativeFile);
            Directory.CreateDirectory(Path.GetDirectoryName(file));
            if (!File.Exists(file))
            {
                if (!string.IsNullOrEmpty(model.Module))
                {
                    relativeFile = Path.Combine("Content/site/".Replace('/', Path.DirectorySeparatorChar), "site.less");
                    file = Path.Combine(rootDir, relativeFile);
                }

                if (!File.Exists(file))
                    CodeFileHelper.CheckoutAndWrite(file, Environment.NewLine, false);
            }

            string code = Templates.Render(new Views.EntityCss(), model);
            using (var ms = new MemoryStream())
            {
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

        private void GenerateColumns()
        {
            CreateNewSiteWebFile(Templates.Render(new Views.EntityColumns(), new
            {
                ClassName = model.ClassName,
                RowClassName = model.RowClassName,
                Module = model.Module,
                RootNamespace = model.RootNamespace,
                Fields = model.Fields,
                IdField = model.Identity,
                NameField = model.NameField
            }), Path.Combine(modules, Path.Combine(model.Module ?? model.RootNamespace, Path.Combine(model.ClassName, model.ClassName + "Columns.cs"))));
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
                IdField = model.Identity,
                NameField = model.NameField
            }), Path.Combine(modules, Path.Combine(model.Module ?? model.RootNamespace, Path.Combine(model.ClassName, model.ClassName + "Form.cs"))));
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
            }), Path.Combine(modules, Path.Combine(model.Module ?? model.RootNamespace, Path.Combine(model.ClassName, model.ClassName + "Repository.cs"))));
        }

        private void GenerateEndpoint()
        {
            CreateNewSiteWebFile(Templates.Render(new Views.EntityEndpoint(), new
            {
                ConnectionKey = model.ConnectionKey,
                RootNamespace = model.RootNamespace,
                ClassName = model.ClassName,
                RowClassName = model.RowClassName,
                Module = model.Module,
                Permission = model.Permission
            }), Path.Combine(modules, Path.Combine(model.Module ?? model.RootNamespace, Path.Combine(model.ClassName, model.ClassName + "Endpoint.cs"))));
        }

        private void GeneratePageController()
        {
            CreateNewSiteWebFile(Templates.Render(new Views.EntityPageController(), new
            {
                ConnectionKey = model.ConnectionKey,
                RootNamespace = model.RootNamespace,
                ClassName = model.ClassName,
                RowClassName = model.RowClassName,
                Module = model.Module,
                Permission = model.Permission,
                NavigationCategory = model.Module
            }), Path.Combine(modules, Path.Combine(model.Module ?? model.RootNamespace, Path.Combine(model.ClassName, model.ClassName + "Page.cs"))));
        }

        private void GeneratePageIndex()
        {
            CreateNewSiteWebFile(Templates.Render(new Views.EntityPageIndex(), new
            {
                ConnectionKey = model.ConnectionKey,
                RootNamespace = model.RootNamespace,
                ClassName = model.ClassName,
                RowClassName = model.RowClassName,
                Module = model.Module,
                Permission = model.Permission,
                NavigationCategory = model.Module
            }), Path.Combine(modules, Path.Combine(model.Module ?? model.RootNamespace, Path.Combine(model.ClassName, model.ClassName + "Index.cshtml"))));
        }

        // old script contracts tt file in script project for backward compability
        const string formContexts = @"Imports\FormContexts\FormContexts.tt";
        // old form context tt file in script project for backward compability
        const string serviceContracts = @"Imports\ServiceContracts\ServiceContracts.tt";
        // newer server imports file in script project
        const string serverImports = @"Imports\ServerImports\ServerImports.tt";
        // newer server imports file in web project
        const string serverTypings = @"Imports\ServerTypings\ServerTypings.tt";

        private void GenerateScriptRowTS()
        {
            var targetFile = model.RowClassName + ".ts";

            if (model.Module != null)
                targetFile = model.Module + "." + targetFile;

            targetFile = Path.Combine(Path.GetDirectoryName(serverTypings), targetFile);

            var content = Templates.Render(new Views.EntityScriptRowTS(), model);

            CreateNewSiteWebFile(content, targetFile, serverTypings);
        }

        private void GenerateScriptServiceTS()
        {
            var targetFile = model.ClassName + "Service.ts";

            if (model.Module != null)
                targetFile = model.Module + "." + targetFile;

            targetFile = Path.Combine(Path.GetDirectoryName(serverTypings), targetFile);

            var content = Templates.Render(new Views.EntityScriptServiceTS(), model);

            CreateNewSiteWebFile(content, targetFile, serverTypings);
        }

        private void GenerateScriptFormTS()
        {
            var targetFile = model.ClassName + "Form.ts";

            if (model.Module != null)
                targetFile = model.Module + "." + targetFile;

            targetFile = Path.Combine(Path.GetDirectoryName(serverTypings), targetFile);

            var content = Templates.Render(new Views.EntityScriptFormTS(), model);

            CreateNewSiteWebFile(content, targetFile, serverTypings);
        }

        private void GenerateScriptGridTS()
        {
            CreateNewSiteWebFile(Templates.Render(new Views.EntityScriptGridTS(), model),
                Path.Combine(modules, Path.Combine(model.Module ?? model.RootNamespace, Path.Combine(model.ClassName, model.ClassName + "Grid.ts"))));
        }

        private void GenerateScriptDialogTS()
        {
            CreateNewSiteWebFile(Templates.Render(new Views.EntityScriptDialogTS(), model, config),
                Path.Combine(modules, Path.Combine(model.Module ?? model.RootNamespace, Path.Combine(model.ClassName, model.ClassName + "Dialog.ts"))));
        }

        private void GenerateScriptGridEditorTS()
        {
            CreateNewSiteWebFile(Templates.Render(new Views.EntityScriptGridEditorTS(), model),
                Path.Combine(modules, Path.Combine(model.Module ?? model.RootNamespace, Path.Combine(model.ClassName, model.ClassName + "Editor.ts"))));
        }

        private void GenerateScriptGridEditorDialogTS()
        {
            CreateNewSiteWebFile(Templates.Render(new Views.EntityScriptGridEditorDialogTS(), model),
                Path.Combine(modules, Path.Combine(model.Module ?? model.RootNamespace, Path.Combine(model.ClassName, model.ClassName + "EditorDialog.ts"))));
        }
    }
}