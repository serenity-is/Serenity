#if ISSOURCEGENERATOR
using Microsoft.CodeAnalysis;
using TypeReference = Microsoft.CodeAnalysis.ITypeSymbol;
using TypeDefinition = Microsoft.CodeAnalysis.ITypeSymbol;
using CustomAttribute = Microsoft.CodeAnalysis.AttributeData;
using PropertyDefinition = Microsoft.CodeAnalysis.IPropertySymbol;
#else
using Mono.Cecil;
#endif
using System.IO;

namespace Serenity.Reflection
{
    public static class TypingsUtils
    {
#if ISSOURCEGENERATOR
        public static string Namespace(this ISymbol symbol)
        {
            if (symbol.ContainingNamespace == null ||
                string.IsNullOrEmpty(symbol.ContainingNamespace.Name))
                return null;

            string restOfResult = symbol.ContainingNamespace.Namespace();
            string result = symbol.ContainingNamespace.Name;

            if (restOfResult != null)
                result = restOfResult + '.' + result;

            return result;
        }

        public static string FullName(this ISymbol symbol)
        {
            var ns = Namespace(symbol);
            if (string.IsNullOrEmpty(ns))
                return symbol.Name;

            return ns + "." + symbol.Name;
        }
#else
        public static string Namespace(this TypeReference symbol)
        {
            return symbol.Namespace;
        }

        public static string FullName(this TypeReference symbol)
        {
            return symbol.FullName;
        }
#endif

        public static bool IsOrSubClassOf(TypeReference childTypeDef, string ns, string name)
        {
            return FindIsOrSubClassOf(childTypeDef, ns, name) != null;
        }

        private static TypeReference FindIsOrSubClassOf(TypeReference typeRef, string ns, string name)
        {
            if (typeRef.Name == name &&
                typeRef.Namespace() == ns)
                return typeRef;

            return EnumerateBaseClasses(typeRef)
                .FirstOrDefault(b => b.Name == name &&
                    b.Namespace() == ns);
        }

        public static bool IsVoid(TypeReference type)
        {
#if ISSOURCEGENERATOR
            return type.SpecialType == SpecialType.System_Void;
#else
            while (type is OptionalModifierType || type is RequiredModifierType)
                type = ((TypeSpecification)type).ElementType;
            return type.MetadataType == MetadataType.Void;
#endif
        }

        public static TypeReference FindIsOrSubClassOf(TypeReference typeRef, TypeReference[] baseClasses, string ns, string name)
        {
            if (typeRef.Name == name &&
                typeRef.Namespace() == ns)
                return typeRef;

            return baseClasses.FirstOrDefault(b => b.Name == name &&
                b.Namespace() == ns);
        }

        public static bool Contains(TypeReference[] classes, string ns, string name)
        {
            return FindByName(classes, ns, name) != null;
        }

        private static TypeReference FindByName(TypeReference[] classes, string ns, string name)
        {
            foreach (var x in classes)
                if (x.Name == name &&
                    x.Namespace() == ns)
                    return x;

            return null;
        }

        public static IEnumerable<TypeDefinition> SelfAndBaseClasses(TypeDefinition klassType)
        {
            for (var td = klassType; td != null;
#if ISSOURCEGENERATOR
                td = td.BaseType)
#else
                td = td.BaseType?.Resolve())
#endif
                yield return td;
        }

#if !ISSOURCEGENERATOR
        public static (string, string) GetCacheKey(TypeReference type)
        {
            if (type.Scope is ModuleDefinition md)
                return (type.FullName, md.Assembly.Name.FullName);
            else if (type.Scope is AssemblyNameReference asm)
                return (type.FullName, asm.FullName);
            else
                return (type.FullName, type.Scope.Name);
        }

        private static readonly Dictionary<(string, string), List<TypeReference>> BaseClassCache = new();
#endif


        public static IEnumerable<TypeReference> EnumerateBaseClasses(TypeReference typeRef)
        {
#if !ISSOURCEGENERATOR
            var key = GetCacheKey(typeRef);
            if (BaseClassCache.TryGetValue(key, out var cached))
                return cached;
#endif

            var list = new List<TypeReference>();

#if ISSOURCEGENERATOR
            var typeDef = typeRef;
#else
            if (typeRef is not TypeDefinition typeDef)
                typeDef = typeRef.Resolve();
#endif

            var baseType = typeDef.BaseType;
            if (baseType != null)
            {
                list.Add(typeDef.BaseType);
                list.AddRange(EnumerateBaseClasses(typeDef.BaseType));
            }

#if !ISSOURCEGENERATOR
            BaseClassCache[key] = list;
#endif
            return list;
        }

        public static bool IsSubclassOf(TypeReference type, string ns, string name)
        {
            if (type.Name == name &&
                type.Namespace() == ns)
                return false;

            return EnumerateBaseClasses(type).Any(b => 
                b.Name == name &&
                b.Namespace() == ns);
        }

#if !ISSOURCEGENERATOR
        public static AssemblyDefinition[] ToDefinitions(IEnumerable<string> assemblyLocations)
        {
            if (assemblyLocations == null || !assemblyLocations.Any())
                return System.Array.Empty<AssemblyDefinition>();

            assemblyLocations = assemblyLocations.Select(x =>
            {
                if (!File.Exists(x))
                    return x;

                var path = Path.GetDirectoryName(x);
                var asmInfo = Path.Combine(path, "__AssemblyInfo__.ini");
                if (!File.Exists(asmInfo))
                    return x;

                var content = File.ReadAllText(asmInfo, System.Text.Encoding.Unicode);
                var idx = content.LastIndexOf("\0file:///", StringComparison.Ordinal);
                if (idx < 0)
                    return x;

                var end = content.IndexOf('\0', idx + 9);
                if (end < 0)
                    return x;

                var location = content.Substring(idx + 9, end - idx - 9).Replace('/', '\\');
                if (File.Exists(location))
                    return location;

                return x;
            }).ToList();

            var module = ICSharpCode.Decompiler.UniversalAssemblyResolver
                .LoadMainModule(assemblyLocations.First(), inMemory: true);

            if (assemblyLocations.Count() == 1)
                return new[] { module.Assembly };

            var resolver = module.AssemblyResolver as ICSharpCode.Decompiler.UniversalAssemblyResolver;

            foreach (var assembly in assemblyLocations)
                resolver.AddSearchDirectory(Path.GetDirectoryName(assembly));

            var assemblyDefinitions = new List<AssemblyDefinition>();
            foreach (var assembly in assemblyLocations)
                assemblyDefinitions.Add(AssemblyDefinition.ReadAssembly(
                    assembly, new ReaderParameters
                    {
                        AssemblyResolver = resolver,
                        InMemory = true,
                        MetadataResolver = module.MetadataResolver
                    }));

            return assemblyDefinitions.ToArray();
        }
#endif

        public static CustomAttribute GetAttr(TypeDefinition klass, string ns, string name, TypeReference[] baseClasses = null)
        {
            CustomAttribute attr;

#if ISSOURCEGENERATOR
            attr = FindAttr(klass.GetAttributes(), ns, name);
#else
            attr = FindAttr(klass.CustomAttributes, ns, name);
#endif
            if (attr != null)
                return attr;

            foreach (var b in baseClasses ?? EnumerateBaseClasses(klass))
            {
#if ISSOURCEGENERATOR
                attr = FindAttr(b.GetAttributes(), ns, name);
#else
                var typeDef = (b as TypeDefinition) ?? b.Resolve();
                attr = FindAttr(typeDef.CustomAttributes, ns, name);
#endif
                if (attr != null)
                    return attr;
            }

            return null;
        }

        public static IEnumerable<CustomAttribute> GetAttrs(IEnumerable<CustomAttribute> attrList, 
            string ns, string name, TypeDefinition[] baseClasses = null)
        {
            if (attrList == null)
                yield break;

            foreach (var x in attrList)
#if ISSOURCEGENERATOR
                if (x.AttributeClass != null && IsOrSubClassOf(x.AttributeClass, ns, name))
#else
                if (x.AttributeType != null && IsOrSubClassOf(x.AttributeType, ns, name))
#endif
                    yield return x;

            if (baseClasses != null)
            {
                foreach (var b in baseClasses)
                {
#if ISSOURCEGENERATOR
                    foreach (var x in b.GetAttributes())
                        if (x.AttributeClass != null && IsOrSubClassOf(x.AttributeClass, ns, name))
#else
                    foreach (var x in b.CustomAttributes)
                        if (x.AttributeType != null && IsOrSubClassOf(x.AttributeType, ns, name))
#endif
                            yield return x;
                }
            }

            yield break;
        }

        public static CustomAttribute FindAttr(IEnumerable<CustomAttribute> attrList, string ns, string name)
        {
            if (attrList == null)
                return null;

            foreach (var x in attrList)
#if ISSOURCEGENERATOR
                if (x.AttributeClass != null && IsOrSubClassOf(x.AttributeClass, ns, name))
#else
                if (x.AttributeType != null && IsOrSubClassOf(x.AttributeType, ns, name))
#endif
                    return x;

            return null;
        }

        public static bool IsAssignableFrom(TypeReference baseType, TypeReference type)
        {
#if ISSOURCEGENERATOR
            return IsAssignableFrom(baseType.FullName(), type);
#else
            return IsAssignableFrom(baseType.FullName, type.Resolve());
#endif
        }

        public static bool IsAssignableFrom(string baseTypeFullName, TypeDefinition type)
        {
            Queue<TypeDefinition> queue = new();
            queue.Enqueue(type);

            while (queue.Any())
            {
                var current = queue.Dequeue();

                if (baseTypeFullName == current.FullName())
                    return true;

                if (current.BaseType != null)
#if ISSOURCEGENERATOR
                    queue.Enqueue(current.BaseType);
#else
                    queue.Enqueue(current.BaseType.Resolve());
#endif

                foreach (var intf in current.Interfaces)
                {
#if ISSOURCEGENERATOR
                    queue.Enqueue(intf);
#else
                    queue.Enqueue(intf.InterfaceType.Resolve());
#endif
                }
            }

            return false;
        }

        public static bool IsPublicInstanceProperty(PropertyDefinition property)
        {
#if ISSOURCEGENERATOR
            if (property.IsStatic)
#else
            if (!property.HasThis)
#endif
                return false;

            if ((property.GetMethod == null ||
#if ISSOURCEGENERATOR
                property.GetMethod.DeclaredAccessibility != Accessibility.Public) &&
#else
                !property.GetMethod.IsPublic) &&
#endif
                (property.SetMethod == null ||
#if ISSOURCEGENERATOR
                property.SetMethod.DeclaredAccessibility != Accessibility.Public))
#else
                !property.SetMethod.IsPublic))
#endif
                return false;

            return true;
        }

        public static TypeReference GetNullableUnderlyingType(TypeReference type)
        {
#if ISSOURCEGENERATOR
            if (type is INamedTypeSymbol namedType &&
                namedType.IsGenericType &&
                namedType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
            {
                return namedType.TypeArguments[0];
            }
#else
            if (type is GenericInstanceType &&
                type.Name == "Nullable`1" &&
                type.Namespace == "System")
            {
                return (type as GenericInstanceType).GenericArguments[0];
            }
#endif

            return null;
        }

        public static TypeDefinition GetEnumTypeFrom(TypeReference type)
        {
            type = GetNullableUnderlyingType(type) ?? type;

#if ISSOURCEGENERATOR
            if (type.SpecialType == SpecialType.System_Enum)
                return type;
#else
            if (!type.IsValueType ||
                type.IsPrimitive)
                return null;

            var definition = type.Resolve();
            if (definition.IsEnum)
                return definition;
#endif

            return null;
        }
    }
}