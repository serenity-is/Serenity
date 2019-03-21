using Mono.Cecil;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Serenity.Reflection
{
    public static class CecilUtils
    {
        public static bool IsOrSubClassOf(TypeDefinition childTypeDef, string ns, string name)
        {
            return FindIsOrSubClassOf(childTypeDef, ns, name) != null;
        }

        public static TypeDefinition FindIsOrSubClassOf(TypeDefinition typeDef, string ns, string name)
        {
            if (typeDef.Namespace == ns &&
                typeDef.Name == name)
                return typeDef;

            return EnumerateBaseClasses(typeDef)
                .FirstOrDefault(b => b.Namespace == ns && b.Name == name);
        }

        public static bool IsVoid(TypeReference type)
        {
            while (type is OptionalModifierType || type is RequiredModifierType)
                type = ((TypeSpecification)type).ElementType;
            return type.MetadataType == MetadataType.Void;
        }

        public static TypeDefinition FindIsOrSubClassOf(TypeDefinition typeDef, TypeDefinition[] baseClasses, string ns, string name)
        {
            if (typeDef.Namespace == ns &&
                typeDef.Name == name)
                return typeDef;

            return baseClasses.FirstOrDefault(b => b.Namespace == ns && b.Name == name);
        }

        private static GenericInstanceType FindGenericInstanceType(TypeDefinition t, TypeDefinition[] baseClasses, 
            string ns, string name)
        {
            if (t.BaseType != null &&
                t.BaseType.IsGenericInstance &&
                (t.BaseType as GenericInstanceType).ElementType.Name == name &&
                (t.BaseType as GenericInstanceType).ElementType.Namespace == ns)
                return t.BaseType as GenericInstanceType;

            foreach (var b in baseClasses)
            {
                if (b.BaseType != null &&
                    b.BaseType.IsGenericInstance &&
                    (b.BaseType as GenericInstanceType).ElementType.Name == name &&
                    (b.BaseType as GenericInstanceType).ElementType.Namespace == ns)
                    return b.BaseType as GenericInstanceType;
            }

            return null;
        }

        public static bool Contains(TypeDefinition[] classes, string ns, string name)
        {
            return FindByName(classes, ns, name) != null;
        }

        private static TypeDefinition FindByName(TypeDefinition[] classes, string ns, string name)
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

        public static IEnumerable<TypeDefinition> EnumerateBaseClasses(TypeDefinition klassType)
        {
            for (var td = klassType.BaseType; td != null; td = td.Resolve().BaseType)
                yield return td.Resolve();
        }

        public static bool IsSubclassOf(TypeDefinition type, string ns, string name)
        {
            if (type.Namespace == ns &&
                type.Name == name)
                return false;

            return EnumerateBaseClasses(type).Any(b => b.Namespace == ns && b.Name == name);
        }

        public static AssemblyDefinition[] ToDefinitions(IEnumerable<string> assemblyLocations)
        {
            if (assemblyLocations == null || !assemblyLocations.Any())
                return new AssemblyDefinition[0];

            assemblyLocations = assemblyLocations.Select(x =>
            {
                if (!File.Exists(x))
                    return x;

                var path = Path.GetDirectoryName(x);
                var asmInfo = Path.Combine(path, "__AssemblyInfo__.ini");
                if (!File.Exists(asmInfo))
                    return x;

                var content = File.ReadAllText(asmInfo, System.Text.Encoding.Unicode);
                var idx = content.LastIndexOf("\0file:///");
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

#if COREFX
            var resolver = ICSharpCode.Decompiler.UniversalAssemblyResolver
                .LoadMainModule(assemblyLocations.First(), inMemory: true).AssemblyResolver
                    as ICSharpCode.Decompiler.UniversalAssemblyResolver;
#else
            var resolver = new Mono.Cecil.DefaultAssemblyResolver();
#endif

            foreach (var assembly in assemblyLocations)
                resolver.AddSearchDirectory(Path.GetDirectoryName(assembly));

            var assemblyDefinitions = new List<AssemblyDefinition>();
            foreach (var assembly in assemblyLocations)
                assemblyDefinitions.Add(Mono.Cecil.AssemblyDefinition.ReadAssembly(
                    assembly, new Mono.Cecil.ReaderParameters
                    {
                        AssemblyResolver = resolver,
                        InMemory = true
                    }));

            return assemblyDefinitions.ToArray();
        }

        public static CustomAttribute GetAttr(TypeDefinition klass, string ns, string name, TypeDefinition[] baseClasses = null)
        {
            CustomAttribute attr;

            attr = FindAttr(klass.CustomAttributes, ns, name);
            if (attr != null)
                return attr;

            foreach (var b in baseClasses ?? EnumerateBaseClasses(klass))
            {
                attr = FindAttr(b.CustomAttributes, ns, name);
                if (attr != null)
                    return attr;
            }

            return null;
        }

        public static CustomAttribute FindAttr(IEnumerable<CustomAttribute> attrList, string ns, string name)
        {
            if (attrList == null)
                return null;

            foreach (var x in attrList)
                if (x.AttributeType != null && IsOrSubClassOf(x.AttributeType.Resolve(), ns, name))
                    return x;

            return null;
        }

        public static bool IsAssignableFrom(TypeReference baseType, TypeReference type)
        {
            return IsAssignableFrom(baseType.Resolve(), type.Resolve());
        }

        public static bool IsAssignableFrom(TypeDefinition baseType, TypeDefinition type)
        {
            Queue<TypeDefinition> queue = new Queue<TypeDefinition>();
            queue.Enqueue(type);

            while (queue.Any())
            {
                var current = queue.Dequeue();

                if (baseType.FullName == current.FullName)
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