namespace Serenity.CodeGenerator;

public class EntityCodeGenerator
{
    private readonly GeneratorConfig config;
    private readonly IFileSystem fileSystem;
    private readonly IGeneratedFileWriter writer;
    private readonly EntityModel model;
    private readonly string rootDir;
    private readonly bool esModules;
    private readonly string mainPrefix;
    private readonly string typingPrefix;
    private static readonly char[] slashNSeparator = ['\n'];

    public EntityCodeGenerator(IProjectFileInfo project, EntityModel model, GeneratorConfig config, IGeneratedFileWriter writer)
    {
        ArgumentNullException.ThrowIfNull(project);
        fileSystem = project.FileSystem;
        this.writer = writer ?? throw new ArgumentNullException(nameof(EntityCodeGenerator.writer));
        this.model = model ?? throw new ArgumentNullException(nameof(model));

        rootDir = fileSystem.GetDirectoryName(fileSystem.GetFullPath(project.ProjectFile));
        this.config = config;
        this.model.CustomSettings = config.CustomSettings;

        TSConfigHelper.LocateTSConfigFiles(fileSystem, rootDir, out var esmConfig, out var _);
        esModules = esmConfig != null && config?.ServerTypings?.ModuleTypings != false;

        var modulesFolder = fileSystem.Combine(rootDir, "Modules");

        var typingFolder = esModules ?
            fileSystem.Combine(modulesFolder, "ServerTypes", model.Module) :
            fileSystem.Combine(rootDir, "Imports", "ServerTypings");

        typingPrefix = esModules ? fileSystem.Combine(typingFolder, model.ClassName) :
            fileSystem.Combine(typingFolder, model.ModuleDot + model.ClassName);

        var mainFolder = modulesFolder;
        if (!string.IsNullOrEmpty(model.Module))
            mainFolder = fileSystem.Combine(mainFolder, model.Module);
        
        mainPrefix = fileSystem.Combine(mainFolder, model.ClassName, model.ClassName);
    }

    void Add(string targetFile, string template)
    {
        var content = Templates.Render(fileSystem, template, model);
        writer.WriteAllText(targetFile, content);
        if (esModules && targetFile.StartsWith(typingPrefix) &&
            !string.IsNullOrEmpty(model.Module) &&
            config.ServerTypings?.ModuleReExports != false)
        {
            var moduleIndex = fileSystem.Combine(fileSystem.GetDirectoryName(
                fileSystem.GetDirectoryName(typingPrefix)), model.Module + ".ts");

            var text = "export * from \"./" + model.Module + "/" +
                fileSystem.GetFileNameWithoutExtension(targetFile) + "\"\n";

            if (fileSystem.FileExists(moduleIndex))
            {
                var current = fileSystem.ReadAllText(moduleIndex).Replace("\r", "",
                    StringComparison.Ordinal) + "\n";
                if (!current.Contains(text))
                    fileSystem.WriteAllText(moduleIndex, (current.Trim() + "\n" +
                        text.Trim()).Trim());
            }
            else
                fileSystem.WriteAllText(moduleIndex, text);
        }
    }

    public void Run()
    {
        if (config.GenerateRow)
        {
            Add(mainPrefix + "Row.cs", "Row");
            Add(typingPrefix + "Row.ts", esModules ? "RowTypingModular" : "RowTyping");
        }

        if (config.GenerateService)
        {
            var handlerPrefix = fileSystem.Combine(fileSystem.GetDirectoryName(mainPrefix),
                fileSystem.Combine(model.ClassName, "RequestHandlers", model.ClassName));
            Add(handlerPrefix + "DeleteHandler.cs", "DeleteHandler");
            Add(handlerPrefix + "ListHandler.cs", "ListHandler");
            Add(handlerPrefix + "RetrieveHandler.cs", "RetrieveHandler");
            Add(handlerPrefix + "SaveHandler.cs", "SaveHandler");
            Add(mainPrefix + "Endpoint.cs", "Endpoint");
            Add(typingPrefix + "Service.ts", esModules ? "ServiceTypingModular" : "ServiceTyping");
        }

        if (config.GenerateUI)
        {
            Add(mainPrefix + "Columns.cs", "Columns");
            Add(mainPrefix + "Form.cs", "Form");
            Add(mainPrefix + "Page.cs", esModules ? "PageModular" : "Page");
            Add(mainPrefix + "Dialog.ts", esModules ? "DialogModular" : "Dialog");
            Add(mainPrefix + "Grid.ts", esModules ? "GridModular" : "Grid");
            Add(mainPrefix + (esModules ? "Page.ts" : "Index.cshtml"), (esModules ? "PageModularTS" : "IndexView"));
            Add(typingPrefix + "Form.ts", esModules ? "FormTypingModular" : "FormTyping");
            Add(typingPrefix + "Columns.ts", esModules ? "ColumnsTypingModular" : "ColumnsTyping");
            GenerateNavigationLink();
        }

        if (config.CustomGenerate != null &&
            config.GenerateCustom)
        {
            foreach (var pair in config.CustomGenerate)
            {
                if (string.IsNullOrEmpty(pair.Value))
                    continue;

                var templateKey = pair.Key;
                if (!PathHelper.IsSecureRelativePath(templateKey))
                    throw new ArgumentOutOfRangeException("templateFile");

                var outputFile = pair.Value;
                if (!PathHelper.IsSecureRelativePath(outputFile))
                    throw new ArgumentOutOfRangeException("outputFile");

                outputFile = string.Format(CultureInfo.InvariantCulture, outputFile, model.ClassName, model.Module, 
                    fileSystem.GetDirectoryName(mainPrefix), fileSystem.GetDirectoryName(typingPrefix), rootDir);

                Add(outputFile, templateKey);
            }
        }
    }

    private void GenerateNavigationLink()
    {
        string file = fileSystem.Combine(rootDir, string.IsNullOrEmpty(model.Module) ?
            "Modules/Common/Navigation/NavigationItems.cs" :
            "Modules/" + model.ModuleSlash + model.Module + "Navigation.cs");
        file = PathHelper.ToPath(file);

        string code = Templates.Render(fileSystem, "NavigationLink", model);

        if (fileSystem.FileExists(file))
        {
            var lines = fileSystem.ReadAllText(file).Replace("\r", "").Split('\n').ToList();
            var toInsert = code.Replace("\r", "", StringComparison.Ordinal).Split(slashNSeparator, StringSplitOptions.RemoveEmptyEntries);
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

            code = string.Join(Environment.NewLine, lines);
        }
        else
            fileSystem.CreateDirectory(fileSystem.GetDirectoryName(file));

        fileSystem.WriteAllBytes(file, GeneratedFileWriter.ToUTF8BOM(code));
    }
}