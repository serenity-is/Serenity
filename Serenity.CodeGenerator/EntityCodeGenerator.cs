using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Serenity.CodeGenerator
{
    public class EntityCodeGenerator
    {
        private GeneratorConfig config;
        private EntityCodeGenerationModel model;
        private string siteWebPath;
        private string siteWebProj;
        private string scriptPath;
        private string scriptProject;
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
            CodeFileHelper.Kdiff3Path = kdiff3Paths.FirstOrDefault(File.Exists);

            if (config.TFSIntegration)
                CodeFileHelper.SetupTFSIntegration(config.TFPath);

            CodeFileHelper.SetupTSCPath(config.TSCPath);

            siteWebProj = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, config.WebProjectFile));
            siteWebPath = Path.GetDirectoryName(siteWebProj);
            if (!string.IsNullOrEmpty(config.ScriptProjectFile))
            {
                scriptProject = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, config.ScriptProjectFile));
                scriptPath = Path.GetDirectoryName(scriptProject);

                if (!File.Exists(scriptProject))
                {
                    scriptProject = null;
                    scriptPath = null;
                }
            }

            this.config = config;
        }

        public void Run()
        {
            if (!scriptPath.IsEmptyOrNull())
                Directory.CreateDirectory(scriptPath);

            Directory.CreateDirectory(siteWebPath);

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

            if (config.GenerateSSImports && scriptProject != null)
            {
                if (config.GenerateRow)
                    GenerateScriptRowSS();

                if (config.GenerateEndpoint)
                    GenerateScriptServiceSS();

                if (config.GenerateForm)
                    GenerateScriptFormSS();
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
            }

            else if (scriptProject != null)
            {
                if (config.GenerateGrid)
                    GenerateScriptGridSS();

                if (config.GenerateDialog)
                    GenerateScriptDialogSS();
            }

            if (config.GenerateTSCode ||
                config.GenerateTSTypings)
            {
                CodeFileHelper.ExecuteTSC(Path.Combine(siteWebPath, @"Scripts\"), "");
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
            string file = Path.Combine(siteWebPath, relativeFile);
            var backup = CreateDirectoryOrBackupFile(file);
            CodeFileHelper.CheckoutAndWrite(file, code, true);
            CodeFileHelper.MergeChanges(backup, file);
            ProjectFileHelper.AddFileToProject(siteWebProj, relativeFile, dependentUpon);
        }

        private bool ScriptFileExists(string relativeFile)
        {
            return File.Exists(scriptProject) &&
                File.Exists(Path.Combine(Path.GetDirectoryName(scriptProject), relativeFile));
        }

        private void CreateNewSiteScriptFile(string code, string relativeFile, string dependentUpon = null)
        {
            string file = Path.Combine(scriptPath, relativeFile);
            var backup = CreateDirectoryOrBackupFile(file);
            CodeFileHelper.CheckoutAndWrite(file, code, true);
            CodeFileHelper.MergeChanges(backup, file);
            ProjectFileHelper.AddFileToProject(scriptProject, relativeFile, dependentUpon);
        }

        private void GenerateRow()
        {
            CreateNewSiteWebFile(Templates.Render(GeneratorConfig.GetEntityRowView(config), model),
                Path.Combine(@"Modules\", Path.Combine(model.Module ?? model.RootNamespace, Path.Combine(model.ClassName, model.RowClassName + ".cs"))));
        }

        private void GenerateCss()
        {
            string relativeFile = Path.Combine(@"Content\site\", "site.less");
            string file = Path.Combine(siteWebPath, relativeFile);
            Directory.CreateDirectory(Path.GetDirectoryName(file));
            if (!File.Exists(file))
            {
                CodeFileHelper.CheckoutAndWrite(file, "\r\n", false);
            }

            string code = Templates.Render(new Views.EntityCss(), model);
            using (var ms = new MemoryStream())
            {
                var old = File.ReadAllBytes(file);
                if (old.Length > 0)
                    ms.Write(old, 0, old.Length);
                using (var sw = new StreamWriter(ms, utf8))
                {
                    AppendComment(sw);
                    sw.Write(code);
                    sw.Flush();

                    CodeFileHelper.CheckoutAndWrite(file, ms.ToArray(), false);
                }
            }

            ProjectFileHelper.AddFileToProject(siteWebProj, relativeFile);
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
            }), Path.Combine(@"Modules\", Path.Combine(model.Module ?? model.RootNamespace, Path.Combine(model.ClassName, model.ClassName + "Columns.cs"))));
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
                ConnectionKey = model.ConnectionKey,
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
                ConnectionKey = model.ConnectionKey,
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
                ConnectionKey = model.ConnectionKey,
                RootNamespace = model.RootNamespace,
                ClassName = model.ClassName,
                RowClassName = model.RowClassName,
                Module = model.Module,
                Permission = model.Permission,
                NavigationCategory = model.Module
            }), Path.Combine(@"Modules\", Path.Combine(model.Module ?? model.RootNamespace, Path.Combine(model.ClassName, model.ClassName + "Index.cshtml"))));
        }

        // old script contracts tt file in script project for backward compability
        const string formContexts = @"Imports\FormContexts\FormContexts.tt";
        // old form context tt file in script project for backward compability
        const string serviceContracts = @"Imports\ServiceContracts\ServiceContracts.tt";
        // newer server imports file in script project
        const string serverImports = @"Imports\ServerImports\ServerImports.tt";
        // newer server imports file in web project
        const string serverTypings = @"Modules\Common\Imports\ServerTypings\ServerTypings.tt";

        private void GenerateScriptRowSS()
        {
            var targetFile = model.RowClassName + ".cs";

            if (model.Module != null)
                targetFile = model.Module + "." + targetFile;

            var dependentUpon =
                (ScriptFileExists(serverImports) ||
                 !ScriptFileExists(serviceContracts)) ? serverImports : serviceContracts;

            targetFile = Path.Combine(Path.GetDirectoryName(dependentUpon), targetFile);

            var content = Templates.Render(new Views.EntityScriptRowSS(), model);

            CreateNewSiteScriptFile(content, targetFile, dependentUpon);
        }

        private void GenerateScriptRowTS()
        {
            var targetFile = model.RowClassName + ".ts";

            if (model.Module != null)
                targetFile = model.Module + "." + targetFile;

            targetFile = Path.Combine(Path.GetDirectoryName(serverTypings), targetFile);

            var content = Templates.Render(new Views.EntityScriptRowTS(), model);

            CreateNewSiteWebFile(content, targetFile, serverTypings);
        }

        private void GenerateScriptServiceSS()
        {
            var targetFile = model.ClassName + "Service.cs";

            if (model.Module != null)
                targetFile = model.Module + "." + targetFile;

            var dependentUpon =
                (ScriptFileExists(serverImports) ||
                 !ScriptFileExists(serviceContracts)) ? serverImports : serviceContracts;

            targetFile = Path.Combine(Path.GetDirectoryName(dependentUpon), targetFile);

            var content = Templates.Render(new Views.EntityScriptServiceSS(), model);

            CreateNewSiteScriptFile(content, targetFile, dependentUpon);
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

        private void GenerateScriptFormSS()
        {
            var targetFile = model.ClassName + "Form.cs";

            if (model.Module != null)
                targetFile = model.Module + "." + targetFile;

            var dependentUpon =
                (ScriptFileExists(serverImports) ||
                 !ScriptFileExists(formContexts)) ? serverImports : formContexts;

            targetFile = Path.Combine(Path.GetDirectoryName(dependentUpon), targetFile);

            var content = Templates.Render(new Views.EntityScriptFormSS(), model);

            CreateNewSiteScriptFile(content, targetFile, dependentUpon);
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

        private void GenerateScriptGridSS()
        {
            CreateNewSiteScriptFile(Templates.Render(new Views.EntityScriptGridSS(), model),
                Path.Combine(model.Module ?? model.RootNamespace, Path.Combine(model.ClassName, model.ClassName + "Grid.cs")));
        }

        private void GenerateScriptGridTS()
        {
            CreateNewSiteWebFile(Templates.Render(new Views.EntityScriptGridTS(), model),
                Path.Combine(@"Modules\", Path.Combine(model.Module ?? model.RootNamespace, Path.Combine(model.ClassName, model.ClassName + "Grid.ts"))));
        }

        private void GenerateScriptDialogSS()
        {
            CreateNewSiteScriptFile(Templates.Render(new Views.EntityScriptDialogSS(), model),
                Path.Combine(model.Module ?? model.RootNamespace, Path.Combine(model.ClassName, model.ClassName + "Dialog.cs")));
        }

        private void GenerateScriptDialogTS()
        {
            CreateNewSiteWebFile(Templates.Render(new Views.EntityScriptDialogTS(), model),
                Path.Combine(@"Modules\", Path.Combine(model.Module ?? model.RootNamespace, Path.Combine(model.ClassName, model.ClassName + "Dialog.ts"))));
        }
    }
}