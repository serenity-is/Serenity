namespace Serenity.CodeGeneration;

#if ISSOURCEGENERATOR
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Threading;
#endif

public partial class ServerTypingsGenerator : CodeGeneratorBase
{
#if ISSOURCEGENERATOR
    private readonly CancellationToken cancellationToken;

    public ServerTypingsGenerator(Compilation compilation, CancellationToken cancellationToken)
    {
        Compilation = compilation ?? throw new ArgumentNullException(nameof(compilation));
        this.cancellationToken = cancellationToken;
    }

    public Compilation Compilation { get; }
#else
    public ServerTypingsGenerator(IFileSystem fileSystem, params Assembly[] assemblies)
        : this(TypingsUtils.ToDefinitions(fileSystem, assemblies?.Select(x => x.Location)))
    {
    }

    public ServerTypingsGenerator(IFileSystem fileSystem, params string[] assemblyLocations)
        : this(TypingsUtils.ToDefinitions(fileSystem, assemblyLocations))
    {
    }

    protected ServerTypingsGenerator(params Mono.Cecil.AssemblyDefinition[] assemblies)
        : base()
    {
        if (assemblies == null || assemblies.Length == 0)
            throw new ArgumentNullException(nameof(assemblies));

        foreach (var assembly in assemblies)
        {
            var assemblyName = assembly.Name?.Name;
            if (!string.IsNullOrEmpty(assemblyName))
                assemblyNames.Add(assemblyName);
        }

        Assemblies = assemblies;
    }

    public Mono.Cecil.AssemblyDefinition[] Assemblies { get; private set; }

    /// <summary>Optional predicate for test purposes</summary>
    public Func<TypeDefinition, bool> TypeFilter { get; set; }
#endif

    private readonly HashSet<string> visited = [];
    private Queue<TypeDefinition> generateQueue;
    protected List<TypeDefinition> lookupScripts = [];
    protected List<GeneratedTypeInfo> generatedTypes = [];
    protected List<AnnotationTypeInfo> annotationTypes = [];
    protected TypeDefinition[] emptyTypes = [];

    protected readonly HashSet<string> assemblyNames = new(StringComparer.Ordinal);

    public bool ModuleReExports { get; set; } = true;

    public readonly HashSet<string> LocalTextFilters = [];

    protected Dictionary<string, TypeDefinition> scriptDataKeys = [];
    protected Dictionary<string, List<string>> namespaceConstants = [];

    protected static string GetAssemblyNameFor(TypeReference type)
    {
        var assemblyName =
#if ISSOURCEGENERATOR
        type.ContainingAssembly?.Name;
#else
        type.Scope?.Name;
        if (assemblyName != null && assemblyName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
            assemblyName = assemblyName[0..^4];
#endif
        return assemblyName;
    }

    protected override void GenerateAll()
    {
        InitModularTypeByKey();
        EnqueueInitialTypes();

        while (generateQueue.Count > 0)
        {
            var typeDef = generateQueue.Dequeue();

#if ISSOURCEGENERATOR
            if (typeDef.ContainingAssembly?.Name != Compilation.AssemblyName)
                continue;
#else
            if (!Assemblies.Any(x => x.FullName == typeDef.Module.Assembly.FullName))
                continue;
#endif

            GenerateCodeFor(typeDef);
        }

        GenerateCommonCode();
    }

    protected virtual void GenerateCommonCode()
    {
        if (LocalTexts)
            GenerateTexts();

        // Disable lazy type loader until excessive chunking is resolved
        // GenerateLazyTypeLoader();
        GenerateScriptDataKeys();

        if (ModuleReExports)
            GenerateModuleReExports();

        GenerateNamespaceConstants();
    }

    protected virtual bool IsUsingNamespace(string ns)
    {
        return false;
    }


    protected virtual void PreVisitType(TypeDefinition type)
    {
        PreVisitTypeForTexts(type);
        PreVisitTypeForScriptData(type);
    }

    protected class GeneratedTypeInfo
    {
        public string Namespace { get; set; }
        public string Name { get; set; }
        public bool Module { get; set; }
        public bool TypeOnly { get; set; }
    }

    protected override void AddFile(string filename)
    {
        HandleModuleImportsOnAddFile(filename);
        base.AddFile(filename);
    }

    protected void RegisterGeneratedType(string ns, string name, bool typeOnly)
    {
        generatedTypes.Add(new GeneratedTypeInfo
        {
            Namespace = ns,
            Name = name,
            TypeOnly = typeOnly
        });
    }

    protected virtual void GenerateCodeFor(TypeDefinition type)
    {
        void add(Action<TypeDefinition> action, string fileIdentifier = null)
        {
            var typeNamespace = ScriptNamespaceFor(type);
            var name = fileIdentifier ?? type.Name;

            action(type);
            var fileName = GetTypingFileNameFor(typeNamespace, name) + ".ts";
            AddFile(fileName);

            if (!string.IsNullOrEmpty(typeNamespace))
            {
                var ns = typeNamespace;
                foreach (var rn in RootNamespaces)
                {
                    if (rn == ns)
                    {
                        ns = null;
                        break;
                    }

                    if (ns.StartsWith(rn + '.', StringComparison.Ordinal))
                    {
                        ns = ns[(rn.Length + 1)..];
                        break;
                    }
                }

                if (ns != null)
                {
                    if (!namespaceConstants.TryGetValue(ns, out var list))
                        namespaceConstants[ns] = list = [];

                    if (!list.Contains(typeNamespace, StringComparer.Ordinal))
                        list.Add(typeNamespace);
                }
            }
        }

        if (type.IsEnum())
        {
            add(GenerateEnum);
            return;
        }

        var baseTypes = TypingsUtils.EnumerateBaseClasses(type).ToArray();

        if (IsServiceEndpoint(baseTypes, type))
        {
            var controllerIdentifier = GetControllerIdentifier(type);
            add((t) => GenerateService(t, controllerIdentifier), controllerIdentifier);
            return;
        }

        var formScriptAttr = GetFormScriptAttribute(type);
        if (formScriptAttr != null)
        {
            var formIdentifier = type.Name;
            var isServiceRequest = IsServiceRequest(baseTypes, type);
            if (formIdentifier.EndsWith(requestSuffix, StringComparison.Ordinal) && isServiceRequest)
                formIdentifier = formIdentifier[..^requestSuffix.Length] + "Form";

            add((t) => GenerateForm(t, formScriptAttr, formIdentifier), formIdentifier);

            EnqueueTypeMembers(type);

            if (isServiceRequest)
                add(GenerateBasicType);

            return;
        }

        var columnsScriptAttr = GetColumnsScriptAttribute(type);
        if (columnsScriptAttr != null)
        {
            add((t) => GenerateColumns(t, columnsScriptAttr));
            EnqueueTypeMembers(type);
            return;
        }

        if (GetNestedPermissionKeysAttribute(type) != null)
        {
            add(GeneratePermissionKeys);
            return;
        }

        if (IsRowType(baseTypes, type))
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

    protected override void Reset()
    {
        base.Reset();

        cw.BraceOnSameLine = true;
        generateQueue = new Queue<TypeDefinition>();
        visited.Clear();
        lookupScripts.Clear();
        generatedTypes.Clear();
        annotationTypes.Clear();
        localTextKeys.Clear();
        localTextNestedClasses.Clear();
        namespaceConstants.Clear();
        scriptDataKeys.Clear();
    }

}