namespace Serenity.CodeGenerator;

public class EntityCodeGenerator
{
    private readonly GeneratorConfig config;
    private readonly IGeneratorFileSystem fileSystem;
    private readonly ICodeFileHelper codeFileHelper;
    private readonly EntityModel model;
    private readonly string rootDir;
    private readonly Encoding utf8 = new UTF8Encoding(true);
    private readonly string modulePath;
    private readonly string moduleClass;
    private readonly string typingClass;
    private readonly bool modularTS;
    private readonly string modulesPath;

    public EntityCodeGenerator(IGeneratorFileSystem fileSystem, ICodeFileHelper codeFileHelper, EntityModel model, GeneratorConfig config, string csproj)
    {
        this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        this.codeFileHelper = codeFileHelper ?? throw new ArgumentNullException(nameof(codeFileHelper));
        this.model = model;

        rootDir = fileSystem.GetDirectoryName(fileSystem.GetFullPath(csproj));
        this.config = config;
        this.model.CustomSettings = config.CustomSettings;

        var serverTypings = PathHelper.ToPath(fileSystem.Combine(rootDir, PathHelper.ToPath("Modules/Common/Imports/ServerTypings/")));
        if (!fileSystem.DirectoryExists(serverTypings))
            serverTypings = fileSystem.Combine(rootDir, PathHelper.ToPath("Imports/ServerTypings/"));

        TSConfigHelper.LocateTSConfigFiles(fileSystem, rootDir, out modulesPath, out var _);

        modularTS = modulesPath != null && config?.ServerTypings?.ModuleTypings != false;

        if (modularTS)
            serverTypings = fileSystem.Combine(rootDir, PathHelper.ToPath("Modules/ServerTypes/" + model.Module + "/"));

        typingClass = fileSystem.Combine(serverTypings, model.ModuleDot + model.ClassName);

        if (modularTS)
            typingClass = fileSystem.Combine(serverTypings, model.ClassName);

        modulePath = fileSystem.Combine(rootDir, "Modules"); 
        if (!string.IsNullOrEmpty(model.Module))
            modulePath = fileSystem.Combine(modulePath, model.Module);

        moduleClass = fileSystem.Combine(modulePath, fileSystem.Combine(model.ClassName, model.ClassName));
    }

    public void Run()
    {
        if (config.GenerateRow)
        {
            CreateFile(Templates.Render(fileSystem, "Row", model), moduleClass + "Row.cs");
            if (modularTS)
            {
                CreateModularTypingFile(Templates.Render(fileSystem, "RowTypingModular", model), typingClass + "Row.ts");
            }
            else
            {
                CreateFile(Templates.Render(fileSystem, "RowTyping", model), typingClass + "Row.ts");
            }
        }

        if (config.GenerateService)
        {
            var handlerClass = fileSystem.Combine(modulePath, 
                fileSystem.Combine(model.ClassName, "RequestHandlers", model.ClassName));
            CreateFile(Templates.Render(fileSystem, "DeleteHandler", model), handlerClass + "DeleteHandler.cs");
            CreateFile(Templates.Render(fileSystem, "ListHandler", model), handlerClass + "ListHandler.cs");
            CreateFile(Templates.Render(fileSystem, "RetrieveHandler", model), handlerClass + "RetrieveHandler.cs");
            CreateFile(Templates.Render(fileSystem, "SaveHandler", model), handlerClass + "SaveHandler.cs");

            CreateFile(Templates.Render(fileSystem, "Endpoint", model), moduleClass + "Endpoint.cs");
            if (modularTS)
            {
                CreateModularTypingFile(Templates.Render(fileSystem, "ServiceTypingModular", model), typingClass + "Service.ts");
            }
            else
            {
                CreateFile(Templates.Render(fileSystem, "ServiceTyping", model), typingClass + "Service.ts");
            }
            
        }

        if (config.GenerateUI)
        {
            CreateFile(Templates.Render(fileSystem, "Columns", model), moduleClass + "Columns.cs");
            CreateFile(Templates.Render(fileSystem, "Form", model), moduleClass + "Form.cs");
            if (modularTS)
            {
                CreateFile(Templates.Render(fileSystem, "PageModular", model), moduleClass + "Page.cs");
                CreateFile(Templates.Render(fileSystem, "PageModularTS", model), moduleClass + "Page.ts");
                CreateFile(Templates.Render(fileSystem, "DialogModular", model), moduleClass + "Dialog.ts");
                CreateFile(Templates.Render(fileSystem, "GridModular", model), moduleClass + "Grid.ts");
                CreateModularTypingFile(Templates.Render(fileSystem, "FormTypingModular", model), typingClass + "Form.ts");
                CreateModularTypingFile(Templates.Render(fileSystem, "ColumnsTypingModular", model), typingClass + "Columns.ts");
            }
            else
            {
                CreateFile(Templates.Render(fileSystem, "IndexView", model), moduleClass + "Index.cshtml");
                CreateFile(Templates.Render(fileSystem, "Page", model), moduleClass + "Page.cs");
                CreateFile(Templates.Render(fileSystem, "Dialog", model), moduleClass + "Dialog.ts");
                CreateFile(Templates.Render(fileSystem, "Grid", model), moduleClass + "Grid.ts");
                CreateFile(Templates.Render(fileSystem, "FormTyping", model), typingClass + "Form.ts");
                CreateFile(Templates.Render(fileSystem, "ColumnsTyping", model), typingClass + "Columns.ts");
            }

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
                    fileSystem.GetDirectoryName(moduleClass), fileSystem.GetDirectoryName(typingClass), rootDir);

                var content = Templates.Render(fileSystem, templateKey, model);
                if (!string.IsNullOrWhiteSpace(content))
                    CreateFile(content, outputFile);
            }
        }
    }

    private string CreateDirectoryOrBackupFile(string file)
    {
        if (fileSystem.FileExists(file))
        {
            var backupFile = string.Format(CultureInfo.InvariantCulture, "{0}.{1}.bak", file, 
                DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture));
            codeFileHelper.CheckoutAndWrite(backupFile, fileSystem.ReadAllBytes(file));
            return backupFile;
        }
        else
        {
            fileSystem.CreateDirectory(fileSystem.GetDirectoryName(file));
            return null;
        }
    }

    private void CreateFile(string code, string file)
    {
        var backup = CreateDirectoryOrBackupFile(file);
        codeFileHelper.CheckoutAndWrite(file, code);
        codeFileHelper.MergeChanges(backup, file);
    }

    private void CreateModularTypingFile(string code, string file)
    {
        CreateFile(code, file);

        var path = fileSystem.Combine(rootDir, PathHelper.ToPath("Modules/ServerTypes/" + model.Module + ".ts"));

        var text = "export * from \"./" + model.Module + "/" + fileSystem.GetFileNameWithoutExtension(file) + "\"\n";

        if (fileSystem.FileExists(path))
        {
            var current = fileSystem.ReadAllText(path);
            
            if(!current.Contains(text))
            {
                fileSystem.WriteAllText(path, current + text);
            }
        }
        else
        {
            fileSystem.WriteAllText(path, text);
        }
    }

    private void GenerateStyle()
    {
        string contentSite = PathHelper.ToPath("wwwroot/Content/site");
        string file = fileSystem.Combine(rootDir, fileSystem.Combine(contentSite,
                "site" + model.DotModule.ToLowerInvariant() + ".less"));

        var siteLess = fileSystem.Combine(rootDir, fileSystem.Combine(contentSite, "site.less"));

        if (!fileSystem.FileExists(siteLess) &&
            !fileSystem.FileExists(file))
        {
            // probably newer template where we don't use less
            return;
        }

        if (!string.IsNullOrEmpty(model.Module) &&
            fileSystem.FileExists(siteLess))
        {
            var importLine = "@import \"site." + model.Module.ToLowerInvariant() + ".less\";";
            var lines = fileSystem.ReadAllText(siteLess)
                .Replace("\r", "", StringComparison.Ordinal).Split('\n').ToList();
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
                codeFileHelper.CheckoutAndWrite(siteLess, string.Join(Environment.NewLine, lines));
            }
        }

        if (!fileSystem.FileExists(file))
        {
            fileSystem.CreateDirectory(fileSystem.GetDirectoryName(file));
            CreateFile("@import \"site.mixins.less\";" + Environment.NewLine, file);
        }

        string code = Templates.Render(fileSystem, "Style", model);
        using var ms = new System.IO.MemoryStream();
        var firstLine = code.Replace("\r", "", StringComparison.Ordinal)
            .Split('\n').FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
        if (!string.IsNullOrWhiteSpace(firstLine))
        {
            var lines = fileSystem.ReadAllText(file).Replace("\r", "").Split('\n');
            // don't generate less for dialog multiple times
            if (lines.Any(x => x.IsTrimmedSame(firstLine)))
                return;
        }

        var old = fileSystem.ReadAllBytes(file);
        if (old.Length > 0)
            ms.Write(old, 0, old.Length);
        using var sw = new System.IO.StreamWriter(ms, utf8);
        sw.Write(code);
        sw.Flush();

        codeFileHelper.CheckoutAndWrite(file, ms.ToArray());
    }

    private void GenerateNavigationLink()
    {
        string file = fileSystem.Combine(rootDir, string.IsNullOrEmpty(model.Module) ?
            "Modules/Common/Navigation/NavigationItems.cs" :
            "Modules/" + model.ModuleSlash + model.Module + "Navigation.cs");
        file = PathHelper.ToPath(file);

        string code = Templates.Render(fileSystem, "NavigationLink", model);

        if (!fileSystem.FileExists(file))
        {
            fileSystem.CreateDirectory(fileSystem.GetDirectoryName(file));
            CreateFile(code, file);
        }
        else
        {
            var lines = fileSystem.ReadAllText(file).Replace("\r", "").Split('\n').ToList();
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

            codeFileHelper.CheckoutAndWrite(file, string.Join(Environment.NewLine, lines));
        }
    }
}