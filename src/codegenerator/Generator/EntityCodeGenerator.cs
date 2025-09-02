namespace Serenity.CodeGenerator;

public class EntityCodeGenerator
{
    private readonly GeneratorConfig config;
    private readonly IFileSystem fileSystem;
    private readonly IGeneratedFileWriter writer;
    private readonly EntityModel model;
    private readonly string rootDir;
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

        var modulesFolder = fileSystem.Combine(rootDir, "Modules");
        var typingFolder = fileSystem.Combine(modulesFolder, "ServerTypes", model.Module);
        typingPrefix = fileSystem.Combine(typingFolder, model.ClassName);

        var mainFolder = modulesFolder;
        if (!string.IsNullOrEmpty(model.Module))
            mainFolder = fileSystem.Combine(mainFolder, model.Module);
        
        mainPrefix = fileSystem.Combine(mainFolder, model.ClassName, model.ClassName);
    }

    void Add(string targetFile, string template)
    {
        var content = Templates.Render(fileSystem, template, model);
        writer.WriteAllText(targetFile, content);
        if (targetFile.StartsWith(typingPrefix) &&
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
            Add(typingPrefix + "Row.ts", "RowTypingModular");
        }

        if (config.GenerateService)
        {
            var handlerPrefix = fileSystem.Combine(fileSystem.GetDirectoryName(mainPrefix),
                "RequestHandlers", model.ClassName);
            Add(handlerPrefix + "DeleteHandler.cs", "DeleteHandler");
            Add(handlerPrefix + "ListHandler.cs", "ListHandler");
            Add(handlerPrefix + "RetrieveHandler.cs", "RetrieveHandler");
            Add(handlerPrefix + "SaveHandler.cs", "SaveHandler");
            Add(mainPrefix + "Endpoint.cs", "Endpoint");
            Add(typingPrefix + "Service.ts", "ServiceTypingModular");
        }

        if (config.GenerateUI)
        {
            Add(mainPrefix + "Columns.cs", "Columns");
            Add(mainPrefix + "Form.cs", "Form");
            Add(mainPrefix + "Page.cs", "PageModular");
            Add(mainPrefix + "Dialog.tsx", "DialogModular");
            Add(mainPrefix + "Grid.tsx", "GridModular");
            Add(mainPrefix + "Page.tsx", "PageModularTS");
            Add(typingPrefix + "Form.ts", "FormTypingModular");
            Add(typingPrefix + "Columns.ts", "ColumnsTypingModular");
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