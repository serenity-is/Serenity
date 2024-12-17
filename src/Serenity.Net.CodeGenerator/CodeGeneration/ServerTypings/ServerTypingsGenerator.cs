namespace Serenity.CodeGeneration;

#if ISSOURCEGENERATOR
public partial class ServerTypingsGenerator(Microsoft.CodeAnalysis.Compilation compilation,
    System.Threading.CancellationToken cancellationToken)
    : TypingsGeneratorBase(compilation, cancellationToken)
{
#else
public partial class ServerTypingsGenerator : TypingsGeneratorBase
{
    public ServerTypingsGenerator(IFileSystem fileSystem, params Assembly[] assemblies)
        : base(fileSystem, assemblies)
    {
    }

    public ServerTypingsGenerator(IFileSystem fileSystem, params string[] assemblyLocations)
        : base(fileSystem, assemblyLocations)
    {
    }
#endif

    public bool LocalTexts { get; set; }
    public bool ModuleReExports { get; set; } = true;

    public readonly HashSet<string> LocalTextFilters = [];

    protected HashSet<string> localTextKeys = [];
    protected Dictionary<string, string> localTextNestedClasses = [];
    protected Dictionary<string, TypeDefinition> scriptDataKeys = [];

    protected override void GenerateAll()
    {
        base.GenerateAll();

        if (LocalTexts)
            GenerateTexts();

        // Disable lazy type loader until excessive chunking is resolved
        // GenerateLazyTypeLoader();
        GenerateScriptDataKeys();

        if (ModuleReExports)
            GenerateModuleReExports();
    }

    protected override void GenerateCodeFor(TypeDefinition type)
    {
        void add(Action<TypeDefinition> action, string fileIdentifier = null)
        {
            var typeNamespace = ScriptNamespaceFor(type);

            action(type);
            var fileName = GetTypingFileNameFor(typeNamespace, fileIdentifier ?? type.Name) + ".ts";
            AddFile(fileName);
        }

        if (type.IsEnum())
        {
            add(GenerateEnum);
            return;
        }

        if (TypingsUtils.IsSubclassOf(type, "Microsoft.AspNetCore.Mvc", "ControllerBase") ||
            TypingsUtils.IsSubclassOf(type, "System.Web.Mvc", "Controller"))
        {
            var controllerIdentifier = GetControllerIdentifier(type);
            add((t) => GenerateService(t, controllerIdentifier), controllerIdentifier);
            return;
        }

        var formScriptAttr = TypingsUtils.GetAttr(type, "Serenity.ComponentModel", "FormScriptAttribute");
        if (formScriptAttr != null)
        {
            var formIdentifier = type.Name;
            var isServiceRequest = TypingsUtils.IsSubclassOf(type, "Serenity.Services", "ServiceRequest");
            if (formIdentifier.EndsWith(requestSuffix, StringComparison.Ordinal) && isServiceRequest)
                formIdentifier = formIdentifier[..^requestSuffix.Length] + "Form";

            add((t) => GenerateForm(t, formScriptAttr, formIdentifier), formIdentifier);

            EnqueueTypeMembers(type);

            if (isServiceRequest)
                add(GenerateBasicType);

            return;
        }

        var columnsScriptAttr = TypingsUtils.GetAttr(type, "Serenity.ComponentModel", "ColumnsScriptAttribute");
        if (columnsScriptAttr != null)
        {
            add((t) => GenerateColumns(t, columnsScriptAttr));
            EnqueueTypeMembers(type);
            return;
        }

        if (TypingsUtils.GetAttr(type, "Serenity.Extensibility", "NestedPermissionKeysAttribute") != null ||
            TypingsUtils.GetAttr(type, "Serenity.ComponentModel", "NestedPermissionKeysAttribute") != null)
        {
            add(GeneratePermissionKeys);
            return;
        }

        if (TypingsUtils.IsSubclassOf(type, "Serenity.Data", "Row") ||
            TypingsUtils.IsSubclassOf(type, "Serenity.Data", "Row`1"))
        {
            var metadata = ExtractRowMetadata(type);
            
            add((t) =>
            {
                GenerateRowType(t);
                GenerateRowMetadata(t, metadata);
            });
            
            return;
        }

        add(GenerateBasicType);
    }

    public void SetLocalTextFiltersFrom(IFileSystem fileSystem, string appSettingsFile)
    {
        ArgumentExceptionHelper.ThrowIfNull(fileSystem, nameof(fileSystem));
        ArgumentExceptionHelper.ThrowIfNull(appSettingsFile, nameof(appSettingsFile));

        if (!LocalTexts || !fileSystem.FileExists(appSettingsFile))
            return;

        try
        {
            var obj = Newtonsoft.Json.Linq.JObject.Parse(fileSystem.ReadAllText(appSettingsFile));
            if ((obj["LocalTextPackages"] ?? ((obj["AppSettings"] 
                as Newtonsoft.Json.Linq.JObject)?["LocalTextPackages"])) is 
                Newtonsoft.Json.Linq.JObject packages)
            {
                foreach (var p in packages.PropertyValues())
                    foreach (var x in p.Values<string>())
                        LocalTextFilters.Add(x);
            }
        }
        catch
        {
        }
    }

    protected override void PreVisitType(TypeDefinition type)
    {
        base.PreVisitType(type);
        PreVisitTypeForTexts(type);
        PreVisitTypeForScriptData(type);
    }

    protected override void Reset()
    {
        base.Reset();

        localTextKeys.Clear();
        localTextNestedClasses.Clear();
        scriptDataKeys.Clear();

    }

    public string DetermineModulesRoot(IFileSystem fileSystem, string projectFile,
        string rootNamespace)
    {
        ArgumentExceptionHelper.ThrowIfNull(fileSystem, nameof(fileSystem));
        ArgumentExceptionHelper.ThrowIfNull(projectFile, nameof(projectFile));

        var projectDir = fileSystem.GetDirectoryName(projectFile);
        var modulesDir = fileSystem.Combine(projectDir, "Modules");

        if (!fileSystem.DirectoryExists(modulesDir) ||
            fileSystem.GetFiles(modulesDir, "*.*").Length == 0)
        {
            var rootNsDir = fileSystem.Combine(projectDir, 
                fileSystem.GetFileName(projectFile));
            if (fileSystem.DirectoryExists(rootNsDir))
            {
                modulesDir = rootNsDir;
                ModulesPathFolder = fileSystem.GetFileName(projectFile);
            }
            else
            {
                rootNsDir = fileSystem.Combine(projectDir, rootNamespace);
                if (fileSystem.DirectoryExists(rootNsDir))
                {
                    modulesDir = rootNsDir;
                    ModulesPathFolder = rootNamespace;
                }
            }
        }

        return modulesDir;
    }
}