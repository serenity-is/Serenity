using Mono.Cecil;
using System.IO;

namespace Serenity.Reflection
{
    public static class CecilUtils
    {
        public static bool IsOrSubClassOf(TypeReference childTypeDef, string ns, string name)
        {
            return FindIsOrSubClassOf(childTypeDef, ns, name) != null;
        }

        private static TypeReference FindIsOrSubClassOf(TypeReference typeRef, string ns, string name)
        {
            if (typeRef.Namespace == ns &&
                typeRef.Name == name)
                return typeRef;

            return EnumerateBaseClasses(typeRef)
                .FirstOrDefault(b => b.Namespace == ns && b.Name == name);
        }

        public static bool IsVoid(TypeReference type)
        {
            while (type is OptionalModifierType || type is RequiredModifierType)
                type = ((TypeSpecification)type).ElementType;
            return type.MetadataType == MetadataType.Void;
        }

        public static TypeReference FindIsOrSubClassOf(TypeReference typeRef, TypeReference[] baseClasses, string ns, string name)
        {
            if (typeRef.Namespace == ns &&
                typeRef.Name == name)
                return typeRef;

            return baseClasses.FirstOrDefault(b => b.Namespace == ns && b.Name == name);
        }

        public static bool Contains(TypeReference[] classes, string ns, string name)
        {
            return FindByName(classes, ns, name) != null;
        }

        private static TypeReference FindByName(TypeReference[] classes, string ns, string name)
        {
            foreach (var x in classes)
                if (x.Namespace == ns && x.Name == name)
                    return x;

            return null;
        }

        public static IEnumerable<TypeDefinition> SelfAndBaseClasses(TypeDefinition klassType)
        {
            for (var td = klassType; td != null; td = td.BaseType?.Resolve())
                yield return td;
        }


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

        public static IEnumerable<TypeReference> EnumerateBaseClasses(TypeReference typeRef)
        {
            var key = GetCacheKey(typeRef);
            if (BaseClassCache.TryGetValue(key, out var cached))
                return cached;

            var list = new List<TypeReference>();

            if (typeRef is not TypeDefinition typeDef)
                typeDef = typeRef.Resolve();

            var baseType = typeDef.BaseType;
            if (baseType != null)
            {
                list.Add(typeDef.BaseType);
                list.AddRange(EnumerateBaseClasses(typeDef.BaseType));
            }

            BaseClassCache[key] = list;
            return list;
        }

        public static bool IsSubclassOf(TypeReference type, string ns, string name)
        {
            if (type.Namespace == ns &&
                type.Name == name)
                return false;

            return EnumerateBaseClasses(type).Any(b => b.Namespace == ns && b.Name == name);
        }

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

        public static CustomAttribute GetAttr(TypeDefinition klass, string ns, string name, TypeReference[] baseClasses = null)
        {
            CustomAttribute attr;

            attr = FindAttr(klass.CustomAttributes, ns, name);
            if (attr != null)
                return attr;

            foreach (var b in baseClasses ?? EnumerateBaseClasses(klass))
            {
                var typeDef = (b as TypeDefinition) ?? b.Resolve();
                attr = FindAttr(typeDef.CustomAttributes, ns, name);
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
                if (x.AttributeType != null && IsOrSubClassOf(x.AttributeType, ns, name))
                    yield return x;

            if (baseClasses != null)
            {
                foreach (var b in baseClasses)
                {
                    foreach (var x in b.CustomAttributes)
                        if (x.AttributeType != null && IsOrSubClassOf(x.AttributeType, ns, name))
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
                if (x.AttributeType != null && IsOrSubClassOf(x.AttributeType, ns, name))
                    return x;

            return null;
        }

        public static bool IsAssignableFrom(TypeReference baseType, TypeReference type)
        {
            return IsAssignableFrom(baseType.FullName, type.Resolve());
        }

        public static bool IsAssignableFrom(string baseTypeFullName, TypeDefinition type)
        {
            Queue<TypeDefinition> queue = new();
            queue.Enqueue(type);

            while (queue.Any())
            {
                var current = queue.Dequeue();

                if (baseTypeFullName == current.FullName)
                    return true;

                if (current.BaseType != null)
                    queue.Enqueue(current.BaseType.Resolve());

                foreach (var intf in current.Interfaces)
                {
                    queue.Enqueue(intf.InterfaceType.Resolve());
                }
            }

            return false;
        }

        public static bool IsPublicInstanceProperty(PropertyDefinition property)
        {
            if (!property.HasThis)
                return false;

            if ((property.GetMethod == null ||
                !property.GetMethod.IsPublic) &&
                (property.SetMethod == null ||
                !property.SetMethod.IsPublic))
                return false;

            return true;
        }

        public static TypeReference GetNullableUnderlyingType(TypeReference type)
        {
            if (type is GenericInstanceType &&
                type.Name == "Nullable`1" &&
                type.Namespace == "System")
            {
                return (type as GenericInstanceType).GenericArguments[0];
            }

            return null;
        }

        public static TypeDefinition GetEnumTypeFrom(TypeReference type)
        {
            type = GetNullableUnderlyingType(type) ?? type;

            if (!type.IsValueType ||
                type.IsPrimitive)
                return null;

            var definition = type.Resolve();
            if (definition.IsEnum)
                return definition;

            return null;
        }
    }
}