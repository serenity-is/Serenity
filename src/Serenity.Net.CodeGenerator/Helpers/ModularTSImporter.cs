using Scriban.Runtime;
using Serenity.CodeGeneration;

namespace Serenity.CodeGenerator;

public class ModularTSImporter(string currentModule)
{
    private readonly List<ModuleImport> moduleImports = [];
    private readonly HashSet<string> moduleImportAliases = [];
    private readonly string currentModule = currentModule;

    protected string ImportFromTypes(string name)
    {
        return AddExternalImport("@/ServerTypes/" + currentModule, name);
    }

    protected string ImportFromQ(string name)
    {
        return AddExternalImport("@serenity-is/corelib", name);
    }

    protected string ImportFromCorelib(string name)
    {
        return AddExternalImport("@serenity-is/corelib", name);
    }

    protected string AddExternalImport(string from, string name)
    {
        return AddModuleImport(from, name, external: true);
    }

    protected string AddModuleImport(string from, string name, bool external = false)
    {
        ArgumentNullException.ThrowIfNull(name);

        ArgumentNullException.ThrowIfNull(from);

        var existing = moduleImports.FirstOrDefault(x => x.From == from && x.Name == name && x.External == external);
        if (existing != null)
            return existing.Alias;

        var i = 0; string alias;
        while (moduleImportAliases.Contains(alias = i == 0 ? name : name + "_" + i))
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

        return string.Join(Environment.NewLine, moduleImports
            .OrderBy(x => FromOrderKey(x.From), StringComparer.OrdinalIgnoreCase)
            .GroupBy(x => x.From, StringComparer.Ordinal)
            .Select(x => "import { " + string.Join(", ", x.Select(y => y.Name)) + " } from '" + x.Key + "';")) + Environment.NewLine + Environment.NewLine;
    }

    private static string FromOrderKey(string from)
    {
        if (from == null)
            return null;

        /// local imports ordered last
        return (from.StartsWith('.') ||
            from.StartsWith('/')) ?
            char.MaxValue + from : from;
    }

    delegate string ImportDelegate(string module, string import);
    delegate string ImportFromDelegate(string import);
    delegate string EditorVariableIndexDelegate(string editor, List<EntityModel.EditorVariable> editors);

    /// <summary>
    /// Returns ScriptObject to be used in scriban templates
    /// </summary>
    /// <returns></returns>
    public static ScriptObject GetScriptObject(ModularTSImporter modularTSImporter)
    {
        var scriptObject = new ScriptObject();

        scriptObject.Import("IMPORTFROM", new ImportDelegate((module, import) =>
        {
            if (module.StartsWith('@'))
            {
                return modularTSImporter.AddExternalImport(module, import);
            }
            else
            {
                return modularTSImporter.AddModuleImport(module, import);
            }
        }));

        scriptObject.Import("SERENITYIMPORT", new ImportFromDelegate(modularTSImporter.ImportFromCorelib));

        scriptObject.Import("QIMPORT", new ImportFromDelegate(modularTSImporter.ImportFromQ));

        scriptObject.Import("SERVERTYPEIMPORT", new ImportFromDelegate(modularTSImporter.ImportFromTypes));

        scriptObject.Import("GETEDITORVARIABLEINDEX", new EditorVariableIndexDelegate((editor, editors) =>
        {
            return editors.FirstOrDefault(x => string.Equals(x.Editor, editor, StringComparison.Ordinal)).Index.ToString();
        }));

        return scriptObject;
    }
}
