#if ISSOURCEGENERATOR
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Serenity.Reflection;
#endif

namespace Serenity.CodeGeneration
{
    public abstract class TypingsGeneratorBase : ImportGeneratorBase
    {
        private HashSet<string> visited;
        private Queue<TypeDefinition> generateQueue;
        protected List<TypeDefinition> lookupScripts;
        protected HashSet<string> localTextKeys;
        protected HashSet<string> generatedTypes;
        protected List<AnnotationTypeInfo> annotationTypes;

#if ISSOURCEGENERATOR
        private readonly CancellationToken cancellationToken;

        protected TypingsGeneratorBase(Compilation compilation, CancellationToken cancellationToken)
        {
            Compilation = compilation ?? throw new ArgumentNullException(nameof(compilation));
            this.cancellationToken = cancellationToken;
            generatedTypes = new HashSet<string>();
            annotationTypes = new List<AnnotationTypeInfo>();
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
            generatedTypes = new HashSet<string>();
            annotationTypes = new List<AnnotationTypeInfo>();

            if (assemblies == null || assemblies.Length == 0)
                throw new ArgumentNullException(nameof(assemblies));

            Assemblies = assemblies;
        }

        public Mono.Cecil.AssemblyDefinition[] Assemblies { get; private set; }
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

            return className + "Service";
        }

        protected override void Reset()
        {
            base.Reset();

            cw.BraceOnSameLine = true;
            generateQueue = new Queue<TypeDefinition>();
            visited = new HashSet<string>();
            lookupScripts = new List<TypeDefinition>();
            localTextKeys = new HashSet<string>();
        }

        protected override void GenerateAll()
        {
            var visitedForAnnotations = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

#if ISSOURCEGENERATOR
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
                        types = module.Types.ToArray();
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
                            ((TypingsUtils.Contains(baseClasses, "Microsoft.AspNetCore.Mvc", "Controller") ||
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

        public virtual string ShortenNamespace(TypeReference type, string codeNamespace)
        {
            string ns = GetNamespace(type);

            if (ns == "Serenity.Services" ||
                ns == "Serenity.ComponentModel")
            {
                if (IsUsingNamespace("Serenity"))
                    return "";
                else
                    return "Serenity";
            }

            if ((codeNamespace != null && (ns == codeNamespace)) ||
                (codeNamespace != null && codeNamespace.StartsWith(ns + ".", StringComparison.Ordinal)))
            {
                return "";
            }

            if (IsUsingNamespace(ns))
                return "";

            if (codeNamespace != null)
            {
                var idx = codeNamespace.IndexOf('.', StringComparison.Ordinal);
                if (idx >= 0 && ns.StartsWith(codeNamespace[..(idx + 1)], StringComparison.Ordinal))
                    return ns[(idx + 1)..];
            }

            return ns;
        }

        protected virtual bool IsUsingNamespace(string ns)
        {
            return false;
        }

        protected virtual string ShortenNamespace(ExternalType type, string codeNamespace)
        {
            string ns = type.Namespace ?? "";

            if ((codeNamespace != null && (ns == codeNamespace)) ||
                (codeNamespace != null && codeNamespace.StartsWith(ns + ".", StringComparison.Ordinal)))
            {
                return "";
            }

            if (IsUsingNamespace(ns))
                return "";

            if (codeNamespace != null)
            {
                var idx = codeNamespace.IndexOf('.', StringComparison.Ordinal);
                if (idx >= 0 && ns.StartsWith(codeNamespace[..(idx + 1)], StringComparison.Ordinal))
                    return ns[(idx + 1)..];
            }

            return ns;
        }

        protected virtual string MakeFriendlyName(TypeReference type, string codeNamespace, StringBuilder sb = null)
        {
            sb ??= this.sb;

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
            string ns;

            if (type.IsGenericInstance())
            {
                ns = ShortenNamespace(type, codeNamespace);

                if (!string.IsNullOrEmpty(ns))
                {
                    sb.Append(ns);
                    sb.Append('.');
                }

                var name = type.Name;
                var idx = name.IndexOf('`', StringComparison.Ordinal);
                if (idx >= 0)
                    name = name[..idx];

                sb.Append(name);
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

            if (codeNamespace != null)
            {
                ns = ShortenNamespace(type, codeNamespace);
                if (!string.IsNullOrEmpty(ns))
                    sb.Append(ns + "." + type.Name);
                else
                    sb.Append(type.Name);
            }
            else
                sb.Append(type.Name);
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
                    "Microsoft.AspNetCore.Mvc", "Controller"))
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

        private readonly List<ModuleImport> moduleImports = new();
        private readonly HashSet<string> moduleImportAliases = new();

        protected class ModuleImport
        {
            public string Name { get; set; }
            public string Alias { get; set; }
            public string From { get; set; }
        }

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
                    sb.Insert(0, string.Join(Environment.NewLine,
                        moduleImports.ToLookup(x => x.From)
                            .Select(from =>
                            {
                                var importList = string.Join(", ", from.Select(p =>
                                    p.Name + (p.Alias != p.Name ? (" as " + p.Alias) : "")));

                                return $"import {{ {importList} }} from \"{from.Key}\";";
                            })) + Environment.NewLine);
                }

                moduleImports.Clear();
                moduleImportAliases.Clear();
            }

            base.AddFile(filename, module);
        }

        protected string AddModuleImport(string from, string name)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            if (from is null)
                throw new ArgumentNullException(nameof(from));

            var existing = moduleImports.FirstOrDefault(x => x.From == from && x.Name == name);
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
                Alias = alias
            });

            return alias;
        }

    }
}