#if ISSOURCEGENERATOR
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Threading;
#endif

namespace Serenity.CodeGeneration;

public abstract class TypingsGeneratorBase : ImportGeneratorBase
{
    private readonly HashSet<string> visited = new();
    private Queue<TypeDefinition> generateQueue;
    protected List<TypeDefinition> lookupScripts = new();
    protected HashSet<string> localTextKeys = new();
    protected List<GeneratedTypeInfo> generatedTypes = new();
    protected List<AnnotationTypeInfo> annotationTypes = new();
    protected ILookup<string, ExternalType> modularEditorTypeByKey;
    protected ILookup<string, ExternalType> modularFormatterTypeByKey;
    protected ILookup<string, ExternalType> modularDialogTypeByKey;

    public string ModulesPathAlias { get; set; } = "@/";
    public string ModulesPathFolder { get; set; } = "Modules";
    public string RootPathAlias { get; set; } = "@/../";

    protected readonly HashSet<string> assemblyNames = new(StringComparer.Ordinal);

#if ISSOURCEGENERATOR
    private readonly CancellationToken cancellationToken;

    protected TypingsGeneratorBase(Compilation compilation, CancellationToken cancellationToken)
    {
        Compilation = compilation ?? throw new ArgumentNullException(nameof(compilation));
        this.cancellationToken = cancellationToken;
    }

    public Compilation Compilation { get; }

    internal class ExportedTypesCollector : SymbolVisitor
    {
        private readonly CancellationToken _cancellationToken;
        private readonly HashSet<INamedTypeSymbol> _exportedTypes;

        public ExportedTypesCollector(CancellationToken cancellation)
        {
            _cancellationToken = cancellation;
            _exportedTypes = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
        }

        public ImmutableArray<INamedTypeSymbol> GetPublicTypes() => _exportedTypes.ToImmutableArray();

        public override void VisitAssembly(IAssemblySymbol symbol)
        {
            _cancellationToken.ThrowIfCancellationRequested();
            symbol.GlobalNamespace.Accept(this);
        }

        public override void VisitNamespace(INamespaceSymbol symbol)
        {
            foreach (INamespaceOrTypeSymbol namespaceOrType in symbol.GetMembers())
            {
                _cancellationToken.ThrowIfCancellationRequested();
                namespaceOrType.Accept(this);
            }
        }

        public static bool IsAccessibleOutsideOfAssembly(ISymbol symbol) =>
            symbol.DeclaredAccessibility switch
            {
                Accessibility.Protected => true,
                Accessibility.ProtectedOrInternal => true,
                Accessibility.Public => true,
                _ => false
            };

        public override void VisitNamedType(INamedTypeSymbol type)
        {
            _cancellationToken.ThrowIfCancellationRequested();

            if (!IsAccessibleOutsideOfAssembly(type) || !_exportedTypes.Add(type))
                return;

            var nestedTypes = type.GetTypeMembers();

            if (nestedTypes.IsDefaultOrEmpty)
                return;

            foreach (INamedTypeSymbol nestedType in nestedTypes)
            {
                _cancellationToken.ThrowIfCancellationRequested();
                nestedType.Accept(this);
            }
        }
    }
#else
    protected TypingsGeneratorBase(IGeneratorFileSystem fileSystem, params Assembly[] assemblies)
        : this(TypingsUtils.ToDefinitions(fileSystem, assemblies?.Select(x => x.Location)))
    {
    }

    protected TypingsGeneratorBase(IGeneratorFileSystem fileSystem, params string[] assemblyLocations)
        : this(TypingsUtils.ToDefinitions(fileSystem, assemblyLocations))
    {
    }

    protected TypingsGeneratorBase(params Mono.Cecil.AssemblyDefinition[] assemblies)
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

    protected virtual bool EnqueueType(TypeDefinition type)
    {
        if (visited.Contains(type.FullNameOf()))
            return false;

        visited.Add(type.FullNameOf());
        generateQueue.Enqueue(type);
        return true;
    }

    private void EnqueueMemberType(TypeReference memberType)
    {
        if (memberType == null)
            return;

        var enumType = TypingsUtils.GetEnumTypeFrom(memberType);
        if (enumType != null)
            EnqueueType(enumType);
    }

    protected virtual void EnqueueTypeMembers(TypeDefinition type)
    {
        foreach (var field in type.FieldsOf())
            if (!field.IsStatic && field.IsPublic())
                EnqueueMemberType(field.FieldType());

        foreach (var property in type.PropertiesOf())
        {
            if (!TypingsUtils.IsPublicInstanceProperty(property))
                continue;

            EnqueueMemberType(property.PropertyType());
        }
    }

    protected string GetAssemblyNameFor(TypeReference type)
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

    protected virtual string GetNamespace(TypeReference type)
    {
        var ns = type.NamespaceOf() ?? "";
        if (string.IsNullOrEmpty(ns) && type.IsNested())
            ns = type.DeclaringType().NamespaceOf();

        if (ns.EndsWith(".Entities", StringComparison.Ordinal))
            return ns[..^".Entities".Length];

        if (ns.EndsWith(".Endpoints", StringComparison.Ordinal))
            return ns[..^".Endpoints".Length];

        if (ns.EndsWith(".Forms", StringComparison.Ordinal))
            return ns[..^".Forms".Length];

        if (ns.EndsWith(".Columns", StringComparison.Ordinal))
            return ns[..^".Columns".Length];

        return ns;
    }

    protected virtual string GetControllerIdentifier(TypeReference controller)
    {
        string className = controller.Name;

        if (className.EndsWith("Controller", StringComparison.Ordinal))
            className = className[0..^10];
        else if (className.EndsWith("Endpoint", StringComparison.Ordinal))
            className = className[0..^8];
        else if (className.EndsWith("Service", StringComparison.Ordinal))
            className = className[0..^7];

        return className + "Service";
    }

    protected override void Reset()
    {
        base.Reset();

        cw.BraceOnSameLine = true;
        generateQueue = new Queue<TypeDefinition>();
        visited.Clear();
        lookupScripts.Clear();
        localTextKeys.Clear();
        generatedTypes.Clear();
        annotationTypes.Clear();
    }

    protected override void GenerateAll()
    {
        var visitedForAnnotations = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        modularEditorTypeByKey = tsTypes.Values.Select(type =>
        {
            if (type.IsAbstract != false &&
                type.IsInterface != false &&
                !string.IsNullOrEmpty(type.Module))
            {
                if (type.SourceFile?.EndsWith(".d.ts") == true &&
                    (HasBaseType(type, ClientTypesGenerator.EditorBaseClasses) ||
                     (HasBaseType(type, "@serenity-is/corelib:Widget", "Widget") &&
                      type.Name?.EndsWith("Editor", StringComparison.Ordinal) == true)))
                {
                    return (key: type.Name, type);
                }
                
                if (type.Attributes != null)
                {
                    foreach (var attr in type.Attributes)
                    {
                        if (attr.Type is not null &&
                            (attr.Type == "registerEditor" ||
                             attr.Type.EndsWith(".registerEditor", StringComparison.Ordinal)) &&
                            attr.Arguments?.Count > 0 &&
                            attr.Arguments[0]?.Value is string key &&
                            !string.IsNullOrEmpty(key))
                            return (key, type);
                    }
                }
            }

            return (null, type);
        }).Where(x => x.key != null)
            .ToLookup(x => x.key, x => x.type);

        modularFormatterTypeByKey = tsTypes.Values.Select(type =>
        {
            if (type.IsAbstract != false &&
                type.IsInterface != false &&
                !string.IsNullOrEmpty(type.Module))
            {
                if (type.SourceFile?.EndsWith(".d.ts") == true &&
                    type.Interfaces != null &&
                    type.Interfaces.Any(x => x == "ISlickFormatter" || 
                        x?.EndsWith(".ISlickFormatter", StringComparison.Ordinal) == true))
                {
                    return (key: type.Name, type);
                }

                if (type.Attributes != null)
                {
                    foreach (var attr in type.Attributes)
                    {
                        if (attr.Type is not null &&
                            (attr.Type == "registerFormatter" ||
                             attr.Type.EndsWith(".registerFormatter", StringComparison.Ordinal)) &&
                            attr.Arguments?.Count > 0 &&
                            attr.Arguments[0]?.Value is string key &&
                            !string.IsNullOrEmpty(key))
                            return (key, type);
                    }
                }
            }

            return (null, type);
        }).Where(x => x.key != null)
            .ToLookup(x => x.key, x => x.type);

        modularDialogTypeByKey = tsTypes.Values.Select(type =>
        {
            if (type.IsAbstract != false &&
                type.IsInterface != false &&
                !string.IsNullOrEmpty(type.Module))
            {
                if (type.Attributes != null)
                {
                    foreach (var attr in type.Attributes)
                    {
                        if (attr.Type is not null &&
                            (attr.Type == "registerClass" ||
                             attr.Type.EndsWith(".registerClass", StringComparison.Ordinal)) &&
                            attr.Arguments?.Count > 0 &&
                            attr.Arguments[0]?.Value is string key &&
                            !string.IsNullOrEmpty(key))
                            return (key, type);
                    }
                }
            }

            return (null, type);
        }).Where(x => x.key != null)
            .ToLookup(x => x.key, x => x.type);

#if ISSOURCEGENERATOR
        if (!string.IsNullOrEmpty(Compilation.AssemblyName))
            assemblyNames.Add(Compilation.AssemblyName);

        var types = Compilation.GetSymbolsWithName(s => true, SymbolFilter.Type).OfType<ITypeSymbol>();

        var collector = new ExportedTypesCollector(cancellationToken);
        collector.VisitNamespace(Compilation.GlobalNamespace);
        foreach (var expType in collector.GetPublicTypes())
            ScanAnnotationTypeAttributes(expType);
#else
        foreach (var assembly in Assemblies)
        {
            foreach (var module in assembly.Modules)
            {
                TypeDefinition[] types;
                try
                {
                    var typeFilter = TypeFilter ?? (type => true);

                    types = module.Types
                        .Where(typeFilter)
                        .ToArray();
                }
                catch
                {
                    // skip assemblies that doesn't like to list its types (e.g. some SignalR reported in #2340)
                    continue;
                }

                if (module.HasAssemblyReferences)
                {
                    foreach (var refAsm in module.AssemblyReferences)
                    { 
                        if (!visitedForAnnotations.Contains(refAsm.Name))
                        {
                            visitedForAnnotations.Add(refAsm.Name);

                            if (SkipPackages.ForAnnotations(refAsm.Name))
                                continue;

                            if (Assemblies.Any(x => string.Equals(x.Name.Name, 
                                refAsm.Name, StringComparison.OrdinalIgnoreCase)))
                                continue;

                            try
                            {
                                
                                var refDef = module.AssemblyResolver.Resolve(refAsm);
                                if (refDef != null)
                                {
                                    foreach (var refMod in refDef.Modules)
                                    {
                                        foreach (var refType in refMod.GetTypes())
                                        {
                                            ScanAnnotationTypeAttributes(refType);
                                        }
                                    }
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                }
#endif

                TypeDefinition[] emptyTypes = Array.Empty<TypeDefinition>();

                foreach (var fromType in types)
                {
                    var nestedLocalTexts = TypingsUtils.GetAttr(fromType, "Serenity.Extensibility",
                        "NestedLocalTextsAttribute", emptyTypes) ??
                        TypingsUtils.GetAttr(fromType, "Serenity.ComponentModel", 
                            "NestedLocalTextsAttribute", emptyTypes);
                    if (nestedLocalTexts != null)
                    {
                        string prefix = null;
#if ISSOURCEGENERATOR
                        prefix = nestedLocalTexts.NamedArguments.FirstOrDefault(x => x.Key == "Prefix").Value.Value as string;
#else
                        if (nestedLocalTexts.HasProperties)
                            prefix = nestedLocalTexts.Properties.FirstOrDefault(x => x.Name == "Prefix").Argument.Value as string;
#endif

                        AddNestedLocalTexts(fromType, prefix ?? "");
                    }

                    ScanAnnotationTypeAttributes(fromType);

                    if (fromType.IsAbstract ||
#if ISSOURCEGENERATOR
                        (fromType.ContainingType != null && fromType.DeclaredAccessibility != Accessibility.Public) ||
#endif
                        TypingsUtils.GetAttr(fromType, "Serenity.ComponentModel", "ScriptSkipAttribute") != null)
                        continue;

                    var baseClasses = TypingsUtils.EnumerateBaseClasses(fromType).ToArray();

                    if (TypingsUtils.Contains(baseClasses, "Serenity.Services", "ServiceRequest") ||
                        TypingsUtils.Contains(baseClasses, "Serenity.Services", "ServiceResponse") ||
                        TypingsUtils.Contains(baseClasses, "Serenity.Data", "Row") ||
                        TypingsUtils.Contains(baseClasses, "Serenity.Data", "Row`1") ||
                        TypingsUtils.Contains(baseClasses, "Serenity.Services", "ServiceEndpoint") ||
                        TypingsUtils.GetAttr(fromType, "Serenity.ComponentModel", "ScriptIncludeAttribute", baseClasses) != null ||
                        TypingsUtils.GetAttr(fromType, "Serenity.ComponentModel", "FormScriptAttribute", baseClasses) != null ||
                        TypingsUtils.GetAttr(fromType, "Serenity.ComponentModel", "ColumnsScriptAttribute", baseClasses) != null ||
                        TypingsUtils.GetAttr(fromType, "Serenity.Extensibility", "NestedPermissionKeysAttribute", emptyTypes) != null ||
                        TypingsUtils.GetAttr(fromType, "Serenity.ComponentModel", "NestedPermissionKeysAttribute", emptyTypes) != null ||
                        ((TypingsUtils.Contains(baseClasses, "Microsoft.AspNetCore.Mvc", "ControllerBase") ||
                          TypingsUtils.Contains(baseClasses, "System.Web.Mvc", "Controller")) && // backwards compability
                         fromType.NamespaceOf()?.EndsWith(".Endpoints", StringComparison.Ordinal) == true))
                    {
                        EnqueueType(fromType);
                        continue;
                    }

                    if (TypingsUtils.GetAttr(fromType, "Serenity.ComponentModel", "LookupScriptAttribute", baseClasses) != null)
                        lookupScripts.Add(fromType);
                }
#if !ISSOURCEGENERATOR
    }
        }
#endif

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
    }

    private void ScanAnnotationTypeAttributes(TypeDefinition fromType)
    {
        var annotationTypeAttrs = TypingsUtils.GetAttrs(
            fromType.GetAttributes(),
            "Serenity.ComponentModel", "AnnotationTypeAttribute", null);

        if (!annotationTypeAttrs.Any())
            return;

        var typeInfo = new AnnotationTypeInfo(fromType);
        foreach (var attr in annotationTypeAttrs)
        {
            var attrInfo = new AnnotationTypeInfo.AttributeInfo();
            if (attr.ConstructorArguments()?.FirstOrDefault(x =>
                x.Type.FullNameOf() == "System.Type").Value is not TypeReference annotatedType)
                continue;

            attrInfo.AnnotatedType = annotatedType.Resolve();

            if (attr.HasProperties())
            {
                attrInfo.Inherited = attr.GetProperties().FirstOrDefault(x =>
                    x.Name() == "Inherited").ArgumentValue() as bool? ?? true;

                attrInfo.Namespaces = attr.GetProperties().FirstOrDefault(x =>
                    x.Name() == "Namespaces").ArgumentValue() as string[];

                attrInfo.Properties = attr.GetProperties().FirstOrDefault(x =>
                    x.Name() == "Properties").ArgumentValue() as string[];
            }
            else
                attrInfo.Inherited = true;
            typeInfo.Attributes.Add(attrInfo);
        }

        if (typeInfo.Attributes.Count > 0)
            annotationTypes.Add(typeInfo);
    }

    protected List<AnnotationTypeInfo> GetAnnotationTypesFor(TypeDefinition type)
    {
        var list = new List<AnnotationTypeInfo>();
        TypeReference[] baseClasses = null;
        foreach (var annotationType in annotationTypes)
        {
            var annotationMatch = false;

            foreach (var attr in annotationType.Attributes)
            {
                baseClasses ??= TypingsUtils.EnumerateBaseClasses(type).ToArray();

                if (TypingsUtils.IsOrSubClassOf(attr.AnnotatedType, "System", "Attribute"))
                {
                    if (TypingsUtils.GetAttr(type, attr.AnnotatedType.NamespaceOf(), 
                        attr.AnnotatedType.Name, baseClasses) == null) 
                        continue;
                }
                else if (attr.Inherited ||
#if ISSOURCEGENERATOR
                    attr.AnnotatedType.TypeKind == TypeKind.Interface)
#else
                    attr.AnnotatedType.IsInterface)
#endif
                {
                    if (!TypingsUtils.IsAssignableFrom(attr.AnnotatedType, type))
                        continue;
                }
#if ISSOURCEGENERATOR
                else if (!type.Equals(attr.AnnotatedType, SymbolEqualityComparer.Default))
#else
                else if (type != attr.AnnotatedType)
#endif
                    continue;

                if (attr.Namespaces != null && attr.Namespaces.Length > 0)
                {
                    bool namespaceMatch = false;
                    foreach (var ns in attr.Namespaces)
                    {
                        if (type.NamespaceOf() == ns)
                        {
                            namespaceMatch = true;
                            break;
                        }

                        if (ns.Length > 2 &&
                            ns.EndsWith(".*", StringComparison.OrdinalIgnoreCase) &&
                            type.NamespaceOf() != null)
                        {
                            if (type.NamespaceOf() == ns[0..^2] ||
                                type.NamespaceOf().StartsWith(ns[0..^1], StringComparison.OrdinalIgnoreCase))
                            {
                                namespaceMatch = true;
                                break;
                            }
                        }
                    }

                    if (!namespaceMatch)
                        continue;
                }

                if (attr.Properties != null &&
                    attr.Properties.Length > 0 &&
                    attr.Properties.Any(name => !type.PropertiesOf().Any(p =>
                        p.Name == name && TypingsUtils.IsPublicInstanceProperty(p))))
                    continue;

                annotationMatch = true;
                break;
            }

            if (annotationMatch)
                list.Add(annotationType);
        }

        return list;
    }


    protected abstract void HandleMemberType(TypeReference memberType, string codeNamespace, bool module);

    public static bool CanHandleType(TypeDefinition memberType)
    {
#if ISSOURCEGENERATOR
        if (memberType.TypeKind == TypeKind.Interface)
#else
        if (memberType.IsInterface)
#endif
            return false;

        if (memberType.IsAbstract)
            return false;

        if (TypingsUtils.IsOrSubClassOf(memberType, "System", "Delegate"))
            return false;

        return true;
    }

    protected string GetFileNameFor(string ns, string name, bool module)
    {
        var filename = RemoveRootNamespace(ns, name);
        if (module)
        {
            var idx = filename.IndexOf('.');
            if (idx >= 0)
                filename = filename[..idx] + '/' + filename[(idx + 1)..];
        }

        return filename;
    }

    protected ExternalType TryFindModuleType(string fullName, string containingAssembly)
    {
        var nonGeneric = fullName;
        var genericIdx = nonGeneric.IndexOf('`', StringComparison.Ordinal);
        if (genericIdx >= 0)
            nonGeneric = nonGeneric[..genericIdx];

        ExternalType scriptType;
        if (nonGeneric.IndexOf(":", StringComparison.Ordinal) >= 0)
        {
            scriptType = GetScriptType(nonGeneric);
            if (scriptType != null)
                return scriptType;
        }

        string ns = "";
        var dotIdx = nonGeneric.LastIndexOf(".", StringComparison.Ordinal);
        if (dotIdx >= 0)
        {
            ns = nonGeneric[0..dotIdx];
            nonGeneric = nonGeneric[(dotIdx + 1)..];
        }

        bool tryModule(string module)
        {
            scriptType = GetScriptType(module + ":" + nonGeneric);
            return scriptType != null;
        }

        if ((!string.IsNullOrEmpty(containingAssembly) && tryModule(containingAssembly.Replace("Serenity.", "@serenity-is/").ToLowerInvariant())) ||
            (!string.IsNullOrEmpty(ns) && tryModule(ns.Replace("Serenity.", "@serenity-is/").ToLowerInvariant())) ||
            ((ns == "Serenity" || ns?.StartsWith("Serenity.") == true) &&
                (tryModule("@serenity-is/corelib/q") ||
                 tryModule("@serenity-is/corelib") ||
                 tryModule("@serenity-is/extensions") ||
                 tryModule("@serenity-is/pro.extensions"))))
        {
            return scriptType;
        }

        return null;
    }

    public virtual string ShortenFullName(string ns, string name, string codeNamespace, bool module, 
        string containingAssembly)
    {
        if (module)
        {
            var nonGeneric = name;
            var genericIdx = nonGeneric.IndexOf('`', StringComparison.Ordinal);
            if (genericIdx >= 0)
                nonGeneric = nonGeneric[..genericIdx];

            if (string.IsNullOrEmpty(containingAssembly) ||
                !assemblyNames.Contains(containingAssembly))
            {
                ExternalType moduleType = TryFindModuleType(ns?.Length > 0 ? (ns + "." + name) : name, containingAssembly);
                if (moduleType != null)
                    return AddExternalImport(moduleType.Module, nonGeneric);
            }

            var filename = GetFileNameFor(ns, nonGeneric, module);
            return AddModuleImport(filename, nonGeneric, external: false);
        }

        if (ns == "Serenity.Services" ||
            ns == "Serenity.ComponentModel")
        {
            if (IsUsingNamespace("Serenity"))
                ns = "";
            else
                ns = "Serenity";
        }
        
        if ((codeNamespace != null && (ns == codeNamespace)) ||
            (codeNamespace != null && codeNamespace.StartsWith(ns + ".", StringComparison.Ordinal)) ||
            IsUsingNamespace(ns))
        {
            ns = "";
        }
        else if (codeNamespace != null)
        {
            var idx = codeNamespace.IndexOf('.', StringComparison.Ordinal);
            if (idx >= 0 && ns.StartsWith(codeNamespace[..(idx + 1)], StringComparison.Ordinal))
                ns = ns[(idx + 1)..];
        }

        return !string.IsNullOrEmpty(ns) ? (ns + "." + name) : name;
    }

    protected virtual bool IsUsingNamespace(string ns)
    {
        return false;
    }

    protected virtual string MakeFriendlyName(TypeReference type, string codeNamespace, bool module)
    {
        if (type.IsGenericInstance())
        {
            var name = type.Name;
            var idx = name.IndexOf('`', StringComparison.Ordinal);
            if (idx >= 0)
                name = name[..idx];

            sb.Append(name);
            sb.Append('<');

            int i = 0;
#if ISSOURCEGENERATOR
            var nt = (INamedTypeSymbol)type;
            foreach (var argument in nt.TypeArguments)
#else
            foreach (var argument in (type as GenericInstanceType).GenericArguments)
#endif
            {
                if (i++ > 0)
                    sb.Append(", ");

                HandleMemberType(argument, codeNamespace, module);
            }

            sb.Append('>');

            return name + "`" +
#if ISSOURCEGENERATOR
                nt.TypeArguments.Length;
#else
                (type as GenericInstanceType).GenericArguments.Count;
#endif
        }
#if ISSOURCEGENERATOR
        else if (type is INamedTypeSymbol nt2 && nt2.TypeParameters.Length > 0)
#else
        else if (type.HasGenericParameters)
#endif
        {
            var name = type.Name;
            var idx = name.IndexOf('`', StringComparison.Ordinal);
            if (idx >= 0)
                name = name[..idx];

            sb.Append(name);
            sb.Append('<');

            int i = 0;
#if ISSOURCEGENERATOR
            foreach (var argument in nt2.TypeParameters)
#else
            foreach (var argument in type.GenericParameters)
#endif
            {
                if (i++ > 0)
                    sb.Append(", ");

                sb.Append(argument.Name);
            }

            sb.Append('>');

            return name + "`" +
#if ISSOURCEGENERATOR
                nt2.TypeParameters.Length;
#else
                type.GenericParameters.Count;
#endif
        }
        else
        {
            sb.Append(type.Name);
            return type.Name;
        }
    }

    protected virtual void MakeFriendlyReference(TypeReference type, string codeNamespace, bool module)
    {
        var fullName = ShortenFullName(GetNamespace(type), type.Name, codeNamespace, module, GetAssemblyNameFor(type));

        if (type.IsGenericInstance())
        {
            var idx = fullName.IndexOf('`', StringComparison.Ordinal);
            if (idx >= 0)
                fullName = fullName[..idx];

            sb.Append(fullName);
            sb.Append('<');

            int i = 0;
#if ISSOURCEGENERATOR
            foreach (var argument in (type as INamedTypeSymbol).TypeArguments)
#else
            foreach (var argument in (type as GenericInstanceType).GenericArguments)
#endif
            {
                if (i++ > 0)
                    sb.Append(", ");

                HandleMemberType(argument, codeNamespace, module);
            }

            sb.Append('>');
            return;
        }

        sb.Append(fullName);
    }

    protected static TypeReference GetBaseClass(TypeDefinition type)
    {
        foreach (var t in TypingsUtils.SelfAndBaseClasses(type))
        {
            if (t.BaseType != null &&
                t.BaseType.IsGenericInstance() &&
#if ISSOURCEGENERATOR
                t.BaseType.OriginalDefinition.NamespaceOf() == "Serenity.Services")
#else
                (t.BaseType as GenericInstanceType).ElementType.Namespace == "Serenity.Services")
#endif
            {
#if ISSOURCEGENERATOR
                var n = t.BaseType.OriginalDefinition.MetadataName();
#else
                var n = (t.BaseType as GenericInstanceType).ElementType.Name;
#endif
                if (n == "ListResponse`1" || n == "RetrieveResponse`1" || n == "SaveRequest`1")
                    return t.BaseType;
            }

            if (t.NamespaceOf() != "Serenity.Services")
                continue;

            if (t.Name == "ListRequest" ||
                t.Name == "RetrieveRequest" ||
                t.Name == "DeleteRequest" ||
                t.Name == "DeleteResponse" ||
                t.Name == "UndeleteRequest" ||
                t.Name == "UndeleteResponse" ||
                t.Name == "SaveResponse" ||
                t.Name == "ServiceRequest" ||
                t.Name == "ServiceResponse")
                return t;
        }

        return null;
    }

    protected abstract void GenerateCodeFor(TypeDefinition type);

    protected static bool IsPublicServiceMethod(MethodDefinition method, out TypeReference requestType, out TypeReference responseType,
        out string requestParam)
    {
        if (method == null)
            throw new ArgumentNullException(nameof(method));

        responseType = null;
        requestType = null;
        requestParam = null;

        if ((TypingsUtils.FindAttr(method.GetAttributes(), "System.Web.Mvc", "NonActionAttribute") ??
             TypingsUtils.FindAttr(method.GetAttributes(), "Microsoft.AspNetCore.Mvc", "NonActionAttribute") ??
             TypingsUtils.FindAttr(method.GetAttributes(), "Serenity.ComponentModel", "ScriptSkipAttribute")) != null)
            return false;

        if (!TypingsUtils.IsSubclassOf(
#if ISSOURCEGENERATOR
                method.ContainingType,
#else
                method.DeclaringType, 
#endif
                "System.Web.Mvc", "Controller") &&
            !TypingsUtils.IsSubclassOf(
#if ISSOURCEGENERATOR
                method.ContainingType,
#else
                method.DeclaringType,
#endif
                "Microsoft.AspNetCore.Mvc", "ControllerBase"))
            return false;

#if ISSOURCEGENERATOR
        if ((method.MethodKind == MethodKind.PropertySet || 
             method.MethodKind == MethodKind.PropertyGet) &&
#else
        if (method.IsSpecialName && 
#endif
            (method.Name.StartsWith("set_", StringComparison.Ordinal) || method.Name.StartsWith("get_", StringComparison.Ordinal)))
            return false;

        var parameters = method.Parameters.Where(x =>
#if ISSOURCEGENERATOR
            x.Type.TypeKind != TypeKind.Interface &&
#else
            !x.ParameterType.Resolve().IsInterface &&
#endif
            TypingsUtils.FindAttr(x.GetAttributes(), "Microsoft.AspNetCore.Mvc", "FromServicesAttribute") == null).ToArray();

        if (parameters.Length > 1)
            return false;

        if (parameters.Length == 1)
        {
#if ISSOURCEGENERATOR
            requestType = parameters[0].Type;
            if (!CanHandleType(requestType))
#else
            requestType = parameters[0].ParameterType;
            if (requestType.IsPrimitive || !CanHandleType(requestType.Resolve()))
#endif
                return false;
        }
        else
            requestType = null;

        requestParam = parameters.Length == 0 ? "request" : parameters[0].Name;

        responseType = method.ReturnType;
        if (responseType != null &&
            responseType.IsGenericInstance() &&
#if ISSOURCEGENERATOR
            (responseType as INamedTypeSymbol).OriginalDefinition
#else
            (responseType as GenericInstanceType).ElementType
#endif
                .FullNameOf().StartsWith("Serenity.Services.Result`1", StringComparison.Ordinal))
        {
#if ISSOURCEGENERATOR
            responseType = (responseType as INamedTypeSymbol).TypeArguments[0];
#else
            responseType = (responseType as GenericInstanceType).GenericArguments[0];
#endif
            return true;
        }
#if ISSOURCEGENERATOR
        else if (responseType.IsGenericInstance() &&
            (responseType as INamedTypeSymbol).OriginalDefinition
#else
        else if (responseType != null &&
            responseType.IsGenericInstance &&
            (responseType as GenericInstanceType).ElementType
#endif
                .FullNameOf().StartsWith("System.Threading.Tasks.Task`1", StringComparison.Ordinal))
        {
#if ISSOURCEGENERATOR
            responseType = (responseType as INamedTypeSymbol).TypeArguments[0];
#else
            responseType = (responseType as GenericInstanceType).GenericArguments[0];
#endif
            return true;
        }
        else if (TypingsUtils.IsOrSubClassOf(responseType, "System.Web.Mvc", "ActionResult") ||
            TypingsUtils.IsAssignableFrom("Microsoft.AspNetCore.Mvc.IActionResult",
#if ISSOURCEGENERATOR
            responseType))
#else
            responseType.Resolve()))
#endif
            return false;
        else if (responseType == null || TypingsUtils.IsVoid(responseType))
            return false;

        return true;
    }

    protected static string GetServiceUrlFromRoute(TypeDefinition controller)
    {
        if (controller == null)
            throw new ArgumentNullException(nameof(controller));

        var route = TypingsUtils.GetAttr(controller, "System.Web.Mvc", "RouteAttribute") ??
            TypingsUtils.GetAttr(controller, "Microsoft.AspNetCore.Mvc", "RouteAttribute");
        string url = route == null ||
            route.ConstructorArguments()?.Count == 0 || route.ConstructorArguments()[0].Value is not string ? 
            ("Services/HasNoRoute/" + controller.Name) : (route.ConstructorArguments()[0].Value as string ?? "");

        url = url.Replace("[controller]", controller.Name[..^"Controller".Length]
#if ISSOURCEGENERATOR
            );
#else
            , StringComparison.Ordinal);
#endif
        url = url.Replace("/[action]", ""
#if ISSOURCEGENERATOR
            );
#else
            , StringComparison.Ordinal);
#endif

        if (!url.StartsWith("~/", StringComparison.Ordinal) && !url.StartsWith("/", StringComparison.Ordinal))
            url = "~/" + url;

        while (true)
        {
            var idx1 = url.IndexOf('{', StringComparison.Ordinal);
            if (idx1 <= 0)
                break;

            var idx2 = url.IndexOf("}", idx1 + 1, StringComparison.Ordinal);
            if (idx2 <= 0)
                break;

            url = url[..idx1] + url[(idx2 + 1)..];
        }

        if (url.StartsWith("~/Services/", StringComparison.OrdinalIgnoreCase))
            url = url["~/Services/".Length..];

        if (url.Length > 1 && url.EndsWith("/", StringComparison.Ordinal))
            url = url[0..^1];

        return url;
    }

    protected virtual void AddNestedLocalTexts(TypeDefinition type, string prefix)
    {
    }

    protected class AnnotationTypeInfo
    {
        public TypeDefinition AnnotationType { get; private set; }
        public List<AttributeInfo> Attributes { get; private set; }
        public Dictionary<string, PropertyDefinition> PropertyByName { get; private set; }

        public AnnotationTypeInfo(TypeDefinition annotationType)
        {
            AnnotationType = annotationType;
            PropertyByName = new Dictionary<string, PropertyDefinition>();
            Attributes = new List<AttributeInfo>();

            foreach (var property in annotationType.PropertiesOf())
                if (TypingsUtils.IsPublicInstanceProperty(property))
                    PropertyByName[property.Name] = property;
        }

        public class AttributeInfo
        {
            public TypeDefinition AnnotatedType { get; set; }
            public bool Inherited { get; set; }
            public string[] Namespaces { get; set; }
            public string[] Properties { get; set; }
        }
    }

    protected class GeneratedTypeInfo
    {
        public string Namespace { get; set; }
        public string Name { get; set; }
        public bool Module { get; set; }
        public bool TypeOnly { get; set; }
    }

    private readonly List<ModuleImport> moduleImports = new();
    private readonly HashSet<string> moduleImportAliases = new();

    protected void ClearImports()
    {
        moduleImports.Clear();
    }

    protected override void AddFile(string filename, bool module = false)
    {
        if (moduleImports.Any())
        {
            if (module)
            {
                var dotTsIndex = filename.IndexOf(".ts", StringComparison.Ordinal);
                var currentFrom = filename;

                if (dotTsIndex >= 0)
                    currentFrom = filename[..dotTsIndex];

                var moduleImportsLookup = moduleImports
                    .Where(x => x.From != currentFrom)
                    .ToLookup(x => (x.From, x.External));

                if (moduleImportsLookup.Any())
                {
                    sb.Insert(0, string.Join(Environment.NewLine, moduleImportsLookup.Select(z =>
                    {
                        var from = z.Key.From;
                        if (!z.Key.External)
                        {
                            if (!from.StartsWith("/", StringComparison.Ordinal) &&
                                !from.StartsWith(".", StringComparison.Ordinal))
                            {
                                if (System.IO.Path.GetDirectoryName(filename) ==
                                    System.IO.Path.GetDirectoryName(from))
                                    from = "./" + System.IO.Path.GetFileName(from);
                                else
                                    from = "../" + from;
                            }
                            else if (!string.IsNullOrEmpty(ModulesPathFolder) && 
                                ModulesPathAlias != null &&
                                from.StartsWith("/" + ModulesPathFolder + "/", StringComparison.Ordinal))
                            {
                                from = ModulesPathAlias + from[(ModulesPathFolder.Length + 2)..];
                            }
                            else if (!string.IsNullOrEmpty(RootPathAlias) &&
                                from.StartsWith("/", StringComparison.Ordinal))
                            {
                                from = RootPathAlias + from[1..];
                            }
                        }

                        var importList = string.Join(", ", z.Select(p =>
                            p.Name + (p.Alias != p.Name ? (" as " + p.Alias) : "")));

                        return $"import {{ {importList} }} from \"{from}\";";
                    })) + Environment.NewLine + Environment.NewLine);
                }
            }

            moduleImports.Clear();
            moduleImportAliases.Clear();
        }

        base.AddFile(filename, module);
    }

    protected string ImportFromQ(string name)
    {
        return AddExternalImport("@serenity-is/corelib/q", name);
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

    protected void RegisterGeneratedType(string ns, string name, bool module, bool typeOnly)
    {
        generatedTypes.Add(new GeneratedTypeInfo 
        { 
            Namespace = ns, 
            Name = name, 
            Module = module, 
            TypeOnly = typeOnly 
        });
    }

}