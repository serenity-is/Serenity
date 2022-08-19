using Scriban.Runtime;
using static Serenity.CodeGenerator.EntityModel;

namespace Serenity.CodeGenerator
{
    public class ModularTSImporter
    {
        private readonly List<ModuleImport> moduleImports = new();
        private readonly HashSet<string> moduleImportAliases = new();
        private string currentModule;

        public ModularTSImporter(string currentModule)
        {
            this.currentModule = currentModule;
        }

        protected string ImportFromTypes(string name)
        {
            return AddExternalImport("../../ServerTypes/" + currentModule, name);
        }

        protected string ImportFromQ(string name)
        {
            return AddExternalImport("@serenity-is/corelib/q", name);
        }

        protected string ImportFromCorelib(string name)
        {
            return AddExternalImport("@serenity-is/corelib", name);
        }

        protected string ImportFromSerenity(string name)
        {
            return AddExternalImport("@serenity-is/corelib", name);
        }

        protected string ImportFromSlick(string name)
        {
            return AddExternalImport("@serenity-is/corelib/slick", name);
        }

        protected string AddExternalImport(string from, string name)
        {
            return AddModuleImport(from, name, external: true);
        }

        protected string AddModuleImport(string from, string name, bool external = false)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            if (from is null)
                throw new ArgumentNullException(nameof(from));

            var existing = moduleImports.FirstOrDefault(x => x.From == from && x.Name == name && x.External == external);
            if (existing != null)
                return existing.Alias;

            var i = 0; string alias;
            while (moduleImportAliases.Contains(alias = i == 0 ? name : (name + "_" + i)))
                i++;

            moduleImportAliases.Add(alias);
            moduleImports.Add(new ModuleImport
            {
                From = from,
                Name = name,
                Alias = alias,
                External = external
            });

            return alias;
        }

        public string GetImports()
        {
            if (moduleImports.IsEmptyOrNull())
                return "";

             return string.Join(Environment.NewLine, moduleImports.GroupBy(x => x.From).Select(x => "import { " + string.Join(", ", x.Select(y => y.Name)) + " } from '" + x.Key + "';")) + Environment.NewLine + Environment.NewLine;
        }

        delegate void ImportDelegate(string module, object[] imports);
        delegate void ImportFromDelegate(object[] imports);
        delegate string EditorVariableIndexDelegate(string editor, List<EditorVariable> editors);
        
        /// <summary>
        /// Returns ScriptObject to be used in scriban templates
        /// </summary>
        /// <returns></returns>
        public static ScriptObject GetScriptObject(ModularTSImporter modularTSImporter)
        {
            var scriptObject = new ScriptObject();

            scriptObject.Import("IMPORT", new ImportDelegate((module, imports) =>
            {
                if (module.StartsWith('@'))
                {
                    foreach (var import in imports)
                    {
                        modularTSImporter.AddExternalImport(module, (string)import);
                    }
                }
                else
                {
                    foreach (var import in imports)
                    {
                        modularTSImporter.AddModuleImport(module, (string)import);
                    }
                }
            }));

            scriptObject.Import("IMPORTFROMCORE", new ImportFromDelegate((imports) =>
            {
                foreach (var import in imports)
                {
                    modularTSImporter.ImportFromCorelib((string)import);
                }
            }));
            
            scriptObject.Import("IMPORTFROMQ", new ImportFromDelegate((imports) =>
            {
                foreach (var import in imports)
                {
                    modularTSImporter.ImportFromQ((string)import);
                }
            }));

            scriptObject.Import("IMPORTFROMTYPES", new ImportFromDelegate((imports) =>
            {
                foreach (var import in imports)
                {
                    modularTSImporter.ImportFromTypes((string)import);
                }
            }));

            scriptObject.Import("GETEDITORVARIABLEINDEX", new EditorVariableIndexDelegate((editor, editors) =>
            {
                return editors.FirstOrDefault(x => string.Equals(x.Editor, editor, StringComparison.Ordinal)).Index.ToString();
            }));

            return scriptObject;
        }
    }
}