#if ISSOURCEGENERATOR
using Microsoft.CodeAnalysis;
using Serenity.CodeGenerator;
using System.Collections.Immutable;
using System.IO;
using System.Threading;
using INamedTypeSymbol = Microsoft.CodeAnalysis.INamedTypeSymbol;
#endif

namespace Serenity.CodeGeneration;

public abstract class TypingsGeneratorBase : ImportGeneratorBase
{
    private readonly HashSet<string> visited = [];
    private Queue<TypeDefinition> generateQueue;
    protected List<TypeDefinition> lookupScripts = [];
    protected List<GeneratedTypeInfo> generatedTypes = [];
    protected List<AnnotationTypeInfo> annotationTypes = [];
    protected ILookup<string, ExternalType> modularEditorTypeByKey;
    protected ILookup<string, ExternalType> modularFormatterTypeByKey;
    protected ILookup<string, ExternalType> modularDialogTypeByKey;
    protected TypeDefinition[] emptyTypes = [];

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

    internal class ExportedTypesCollector(CancellationToken cancellation) : SymbolVisitor
    {
        private readonly HashSet<INamedTypeSymbol> _exportedTypes = new(SymbolEqualityComparer.Default);
        public ImmutableArray<INamedTypeSymbol> GetPublicTypes() => _exportedTypes.ToImmutableArray();

        public override void VisitAssembly(IAssemblySymbol symbol)
        {
            cancellation.ThrowIfCancellationRequested();
            symbol.GlobalNamespace.Accept(this);
        }

        public override void VisitNamespace(INamespaceSymbol symbol)
        {
            foreach (INamespaceOrTypeSymbol namespaceOrType in symbol.GetMembers())
            {
                cancellation.ThrowIfCancellationRequested();
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
            cancellation.ThrowIfCancellationRequested();

            if (!IsAccessibleOutsideOfAssembly(type) || !_exportedTypes.Add(type))
                return;

            var nestedTypes = type.GetTypeMembers();

            if (nestedTypes.IsDefaultOrEmpty)
                return;

            foreach (INamedTypeSymbol nestedType in nestedTypes)
            {
                cancellation.ThrowIfCancellationRequested();
                nestedType.Accept(this);
            }
        }
    }
#else
    protected TypingsGeneratorBase(IFileSystem fileSystem, params Assembly[] assemblies)
        : this(TypingsUtils.ToDefinitions(fileSystem, assemblies?.Select(x => x.Location)))
    {
    }

    protected TypingsGeneratorBase(IFileSystem fileSystem, params string[] assemblyLocations)
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

    protected virtual string ScriptNamespaceFor(TypeReference type)
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
        generatedTypes.Clear();
        annotationTypes.Clear();
    }

    protected override void GenerateAll()
    {
        var visitedForAnnotations = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        modularEditorTypeByKey = TSTypes.Values.Select(type =>
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

        modularFormatterTypeByKey = TSTypes.Values.Select(type =>
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

        modularDialogTypeByKey = TSTypes.Values.Select(type =>
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


                foreach (var fromType in types)
                {
                    PreVisitType(fromType);
                    ScanAnnotationTypeAttributes(fromType);

                    if ((fromType.IsAbstract &&
                         TypingsUtils.GetAttr(fromType, "Serenity.ComponentModel", "ScriptIncludeAttribute") == null) ||
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
                          TypingsUtils.Contains(baseClasses, "System.Web.Mvc", "Controller")) && // backwards compatibility
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

    protected virtual void ScanAnnotationTypeAttributes(TypeDefinition fromType)
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


    protected abstract void HandleMemberType(TypeReference memberType, string codeNamespace);

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

    protected string GetTypingFileNameFor(string ns, string name)
    {
        var filename = RemoveRootNamespace(ns, name);
        var idx = filename.IndexOf('.');
        if (idx >= 0)
            filename = filename[..idx] + '/' + filename[(idx + 1)..];

        return filename;
    }

    private static readonly HashSet<string> SerenityNetAssemblies = new([
        "Serenity.Net.Core",
        "Serenity.Net.Services",
        "Serenity.Net.Web"
    ], StringComparer.OrdinalIgnoreCase);

    private static readonly string[] CommonSerenityModulesColon = [
        "@serenity-is/corelib:",
        "@serenity-is/extensions:",
        "@serenity-is/pro.extensions:",
    ];

    protected ExternalType TryFindModuleType(string fullName, string containingAssembly)
    {
        var nonGeneric = fullName;
        var genericIdx = nonGeneric.IndexOf('`', StringComparison.Ordinal);
        if (genericIdx >= 0)
            nonGeneric = nonGeneric[..genericIdx];

        ExternalType scriptType;
        if (nonGeneric.Contains(':'))
        {
            scriptType = GetScriptType(nonGeneric);
            if (scriptType != null)
                return scriptType;
        }

        string ns = "";
        var dotIdx = nonGeneric.LastIndexOf('.');
        if (dotIdx >= 0)
        {
            ns = nonGeneric[0..dotIdx];
            nonGeneric = nonGeneric[(dotIdx + 1)..];
        }

        if (!string.IsNullOrEmpty(containingAssembly))
        {
            if (SerenityNetAssemblies.Contains(containingAssembly))
            {
                if ((scriptType = GetScriptType("@serenity-is/corelib:" + nonGeneric)) != null)
                    return scriptType;
            }
            else if (containingAssembly.StartsWith("Serenity.", StringComparison.OrdinalIgnoreCase))
            {
                if ((scriptType = GetScriptType("@serenity-is/" + containingAssembly[9..].ToLowerInvariant() + ":" + nonGeneric)) != null)
                    return scriptType;
            }
            else if ((scriptType = GetScriptType(containingAssembly.ToLowerInvariant() + ":" + nonGeneric)) != null)
                return scriptType;
        }

        if (ns == null)
            return null;

        if (ns.StartsWith("Serenity.", StringComparison.OrdinalIgnoreCase) &&
            (scriptType = GetScriptType("@serenity-is/" + ns[9..].ToLowerInvariant() + ":" + nonGeneric)) != null)
            return scriptType;

        if (ns.Equals("Serenity", StringComparison.OrdinalIgnoreCase))
        {
            foreach (var moduleColon in CommonSerenityModulesColon)
            {
                if ((scriptType = GetScriptType(moduleColon + nonGeneric)) != null)
                    return scriptType;
            }
        }

        if ((scriptType = GetScriptType(ns.ToLowerInvariant() + ":" + nonGeneric)) != null)
            return scriptType;

        return null;
    }

    public virtual string ShortenFullName(string ns, string name, string codeNamespace, 
        string containingAssembly)
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

        var filename = GetTypingFileNameFor(ns, nonGeneric);
        return AddModuleImport(filename, nonGeneric, external: false);
    }

    protected virtual bool IsUsingNamespace(string ns)
    {
        return false;
    }

    protected virtual string MakeFriendlyName(TypeReference type, string codeNamespace)
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

                HandleMemberType(argument, codeNamespace);
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

    protected virtual void MakeFriendlyReference(TypeReference type, string codeNamespace)
    {
        var fullName = ShortenFullName(ScriptNamespaceFor(type), type.Name, codeNamespace, GetAssemblyNameFor(type));

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

                HandleMemberType(argument, codeNamespace);
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
        ArgumentExceptionHelper.ThrowIfNull(method);

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
        ArgumentExceptionHelper.ThrowIfNull(controller);

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

        if (!url.StartsWith("~/", StringComparison.Ordinal) && !url.StartsWith('/'))
            url = "~/" + url;

        while (true)
        {
            var idx1 = url.IndexOf('{', StringComparison.Ordinal);
            if (idx1 <= 0)
                break;

            var idx2 = url.IndexOf('}', idx1 + 1);
            if (idx2 <= 0)
                break;

            url = url[..idx1] + url[(idx2 + 1)..];
        }

        if (url.StartsWith("~/Services/", StringComparison.OrdinalIgnoreCase))
            url = url["~/Services/".Length..];

        if (url.Length > 1 && url.EndsWith('/'))
            url = url[0..^1];

        return url;
    }

    protected virtual void PreVisitType(TypeDefinition type)
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
            PropertyByName = [];
            Attributes = [];

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

    private readonly List<ModuleImport> moduleImports = [];
    private readonly HashSet<string> moduleImportAliases = [];

    protected void ClearImports()
    {
        moduleImports.Clear();
    }

    protected override void AddFile(string filename)
    {
        if (moduleImports.Count != 0)
        {
            var dotTsIndex = filename.IndexOf(".ts", StringComparison.Ordinal);
            var currentFrom = filename;

            if (dotTsIndex >= 0)
                currentFrom = filename[..dotTsIndex];

            var moduleImportsLookup = moduleImports
                .Where(x => x.From != currentFrom)
                .ToLookup(x => (x.From, x.External));

            if (moduleImportsLookup.Count != 0)
            {
                sb.Insert(0, string.Join(Environment.NewLine, moduleImportsLookup
                    .Select(z =>
                {
                    var from = z.Key.From;
                    if (!z.Key.External && !from.StartsWith('.'))
                    {
                        if (from.StartsWith("@/") &&
                            !string.IsNullOrEmpty(ModulesPathFolder))
                            from = "/" + ModulesPathFolder + "/" + from;

                        if (!from.StartsWith('/'))
                        {
                            if (System.IO.Path.GetDirectoryName(filename) ==
                                System.IO.Path.GetDirectoryName(from))
                                from = "./" + System.IO.Path.GetFileName(from);
                            else
                                from = "../" + from;
                        }
                        else if (!string.IsNullOrEmpty(ModulesPathFolder) &&
                            !string.IsNullOrEmpty(ModulesPathAlias) &&
                            from.StartsWith("/" + ModulesPathFolder + "/", StringComparison.Ordinal))
                        {
                            from = ModulesPathAlias + from[(ModulesPathFolder.Length + 2)..];
                        }
                        else if (!string.IsNullOrEmpty(RootPathAlias))
                        {
                            from = RootPathAlias + from[1..];
                        }
                        else
                        {
                            var relativeTo = PathHelper.ToPath("/Modules/ServerTypes/") + 
                                PathHelper.ToPath(System.IO.Path.GetDirectoryName(filename));

#if ISSOURCEGENERATOR
                            from = PathHelper.GetRelativePath(relativeTo, from);
#else
                            from = System.IO.Path.GetRelativePath(relativeTo, from);
#endif
                            from = PathHelper.ToUrl(from);
                        }
                    }

                    var importList = string.Join(", ", z.Select(p =>
                        p.Name + (p.Alias != p.Name ? (" as " + p.Alias) : "")));

                    return (from, importList);
                }).OrderBy(x => FromOrderKey(x.from), StringComparer.OrdinalIgnoreCase).Select(x => $"import {{ {x.importList} }} from \"{x.from}\";")) + 
                    Environment.NewLine + Environment.NewLine);
            }

            moduleImports.Clear();
            moduleImportAliases.Clear();
        }

        base.AddFile(filename);
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
        ArgumentExceptionHelper.ThrowIfNull(name);
        ArgumentExceptionHelper.ThrowIfNull(from);

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

    protected void RegisterGeneratedType(string ns, string name, bool typeOnly)
    {
        generatedTypes.Add(new GeneratedTypeInfo 
        { 
            Namespace = ns, 
            Name = name, 
            TypeOnly = typeOnly 
        });
    }

}