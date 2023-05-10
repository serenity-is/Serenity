global using Serenity.Reflection;
#if ISSOURCEGENERATOR
global using CustomAttribute = Microsoft.CodeAnalysis.AttributeData;
global using FieldDefinition = Microsoft.CodeAnalysis.IFieldSymbol;
global using MethodDefinition = Microsoft.CodeAnalysis.IMethodSymbol;
global using PropertyDefinition = Microsoft.CodeAnalysis.IPropertySymbol;
global using TypeDefinition = Microsoft.CodeAnalysis.ITypeSymbol;
global using TypeReference = Microsoft.CodeAnalysis.ITypeSymbol;
global using GenericInstanceType = Microsoft.CodeAnalysis.INamedTypeSymbol;
using Microsoft.CodeAnalysis;
#else
global using CustomAttribute = Mono.Cecil.CustomAttribute;
global using FieldDefinition = Mono.Cecil.FieldDefinition;
global using MethodDefinition = Mono.Cecil.MethodDefinition;
global using TypeReference = Mono.Cecil.TypeReference;
global using TypeDefinition = Mono.Cecil.TypeDefinition;
global using ParameterDefinition = Mono.Cecil.ParameterDefinition;
global using PropertyDefinition = Mono.Cecil.PropertyDefinition;
global using GenericInstanceType = Mono.Cecil.GenericInstanceType;
#endif

namespace Serenity.Reflection;

public static class TypingsUtils
{
#if ISSOURCEGENERATOR
    public static string NamespaceOf(this ISymbol symbol)
    {
        if (symbol.ContainingNamespace == null ||
            string.IsNullOrEmpty(symbol.ContainingNamespace.Name))
            return null;

        string restOfResult = symbol.ContainingNamespace.NamespaceOf();
        string result = symbol.ContainingNamespace.Name;

        if (restOfResult != null)
            result = restOfResult + '.' + result;

        return result;
    }

    public static string FullNameOf(this ISymbol symbol)
    {
        var ns = NamespaceOf(symbol);
        if (string.IsNullOrEmpty(ns))
            return symbol.MetadataName;

        return ns + "." + symbol.MetadataName;
    }

    public static IEnumerable<FieldDefinition> FieldsOf(this TypeDefinition type)
    {
        return type.GetMembers().OfType<FieldDefinition>();
    }

    public static IEnumerable<PropertyDefinition> PropertiesOf(this TypeDefinition type)
    {
        return type.GetMembers().OfType<PropertyDefinition>();
    }

    public static IEnumerable<IMethodSymbol> MethodsOf(this TypeDefinition type)
    {
        return type.GetMembers().OfType<IMethodSymbol>();
    }

    public static bool IsGenericInstance(this TypeDefinition typeSymbol)
    {
        return typeSymbol is GenericInstanceType nt &&
            nt.IsGenericType && !nt.TypeArguments.Any(x => x.TypeKind == TypeKind.TypeParameter);
    }

    public static TypeDefinition Resolve(this TypeDefinition typeSymbol)
    {
        return typeSymbol;
    }

    public static TypeDefinition PropertyType(this PropertyDefinition prop)
    {
        return prop.Type;
    }

    public static TypeDefinition AttributeType(this CustomAttribute attributeData)
    {
        return attributeData.AttributeClass;
    }

    public static IList<TypedConstant> ConstructorArguments(this CustomAttribute attributeData)
    {
        return attributeData.ConstructorArguments;
    }

    public static MethodDefinition Constructor(this CustomAttribute attributeData)
    {
        return attributeData.AttributeConstructor;
    }

    public static IList<KeyValuePair<string, TypedConstant>> NamedArguments(this CustomAttribute attributeData)
    {
        return attributeData.NamedArguments;
    }

    public static bool IsPublic(this MethodDefinition fieldSymbol)
    {
        return fieldSymbol.DeclaredAccessibility == Accessibility.Public;
    }

    public static bool IsPublic(this FieldDefinition fieldSymbol)
    {
        return fieldSymbol.DeclaredAccessibility == Accessibility.Public;
    }

    public static bool HasConstant(this FieldDefinition fieldSymbol)
    {
        return fieldSymbol.HasConstantValue;
    }

    public static object Constant(this FieldDefinition fieldSymbol)
    {
        return fieldSymbol.ConstantValue;
    }

    public static GenericInstanceType DeclaringType(this FieldDefinition fieldSymbol)
    {
        return fieldSymbol.ContainingType;
    }

    public static GenericInstanceType DeclaringType(this TypeDefinition typeSymbol)
    {
        return typeSymbol.ContainingType;
    }

    public static bool IsConstructor(this IMethodSymbol methodSymbol)
    {
        return methodSymbol.MethodKind == MethodKind.Constructor;
    }

    public static bool IsNested(this TypeDefinition typeSymbol)
    {
        return typeSymbol.ContainingType != null;
    }

    public static TypeDefinition FieldType(this FieldDefinition field)
    {
        return field.Type;
    }

    public static bool HasProperties(this CustomAttribute customAttribute)
    {
        return customAttribute.NamedArguments.Any();
    }

    public static IList<KeyValuePair<string, TypedConstant>> GetProperties(this CustomAttribute customAttribute)
    {
        return customAttribute.NamedArguments;
    }

    public static string Name(this KeyValuePair<string, TypedConstant> keyValuePair)
    {
        return keyValuePair.Key;
    }

    public static string Name(this IParameterSymbol parameterSymbol)
    {
        return parameterSymbol.Name;
    }

    public static object ArgumentValue(this KeyValuePair<string, TypedConstant> keyValuePair)
    {
        return keyValuePair.Value.Kind == TypedConstantKind.Array ?
            keyValuePair.Value.Values : keyValuePair.Value.Value;
    }

    public static object Value(this TypedConstant constant)
    {
        return constant.Kind == TypedConstantKind.Array ?
            constant.Values : constant.Value;
    }

    public static bool HasCustomAttributes(this IFieldSymbol field)
    {
        return field.GetAttributes().Any();
    }

    public static bool HasCustomAttributes(this IPropertySymbol prop)
    {
        return prop.GetAttributes().Any();
    }

    public static bool IsSpecialName(this IFieldSymbol field)
    {
        return field.IsImplicitlyDeclared;
    }

    public static bool HasNestedTypes(this TypeReference type)
    {
        return type.GetTypeMembers().Any();
    }

    public static IEnumerable<TypeReference> NestedTypes(this TypeReference type)
    {
        return type.GetTypeMembers();
    }

    public static bool IsEnum(this TypeReference type)
    {
        return type.TypeKind == TypeKind.Enum;
    }

    public static bool HasConstructorArguments(this CustomAttribute attr)
    {
        return attr.ConstructorArguments.Any();
    }

    public static IList<TypeReference> GenericArguments(this GenericInstanceType type)
    {
        return type.TypeArguments;
    }

    public static bool IsPrimitive(this TypeReference type)
    {
        return type.SpecialType >= SpecialType.System_Boolean &&
            type.SpecialType <= SpecialType.System_UIntPtr;
    }

    public static bool IsArray(this TypeDefinition type)
    {
        return type.TypeKind == TypeKind.Array;
    }

    public static TypeDefinition ElementType(this TypeDefinition type)
    {
        if (type is IArrayTypeSymbol ats)
            return ats.ElementType;
        
        if (type is GenericInstanceType git)
            return git.OriginalDefinition;

        return null;
    }

    public static string MetadataName(this TypeReference type)
    {
        return type.MetadataName;
    }
#else
    public static string NamespaceOf(this TypeReference symbol)
    {
        return symbol.Namespace;
    }

    public static string FullNameOf(this TypeReference symbol)
    {
        return symbol.FullName;
    }

    public static IEnumerable<FieldDefinition> FieldsOf(this TypeDefinition type)
    {
        return type.Fields;
    }

    public static IEnumerable<PropertyDefinition> PropertiesOf(this TypeDefinition type)
    {
        return type.Properties;
    }

    public static bool IsGenericInstance(this TypeReference type)
    {
        return type.IsGenericInstance;
    }

    public static IEnumerable<CustomAttribute> GetAttributes(this FieldDefinition field)
    {
        return field.CustomAttributes;
    }

    public static IEnumerable<CustomAttribute> GetAttributes(this PropertyDefinition prop)
    {
        return prop.CustomAttributes;
    }

    public static IEnumerable<CustomAttribute> GetAttributes(this MethodDefinition method)
    {
        return method.CustomAttributes;
    }

    public static IEnumerable<CustomAttribute> GetAttributes(this TypeDefinition type)
    {
        return type.CustomAttributes;
    }

    public static IEnumerable<CustomAttribute> GetAttributes(this ParameterDefinition pd)
    {
        return pd.CustomAttributes;
    }

    public static bool HasCustomAttributes(this FieldDefinition field)
    {
        return field.HasCustomAttributes;
    }

    public static bool HasCustomAttributes(this PropertyDefinition prop)
    {
        return prop.HasCustomAttributes;
    }

    public static bool IsSpecialName(this FieldDefinition field)
    {
        return field.IsSpecialName;
    }

    public static TypeReference AttributeType(this CustomAttribute attribute)
    {
        return attribute.AttributeType;
    }

    public static IEnumerable<MethodDefinition> MethodsOf(this TypeDefinition type)
    {
        return type.Methods;
    }

    public static bool IsNested(this TypeDefinition type)
    {
        return type.IsNested;
    }

    public static bool IsConstructor(this MethodDefinition methodDefinition)
    {
        return methodDefinition.IsConstructor;
    }

    public static bool IsNested(this TypeReference typeDefinition)
    {
        return typeDefinition.IsNested;
    }

    public static TypeReference DeclaringType(this TypeReference typeSymbol)
    {
        return typeSymbol.DeclaringType;
    }

    public static TypeReference DeclaringType(this FieldDefinition field)
    {
        return field.DeclaringType;
    }

    public static TypeReference FieldType(this FieldDefinition field)
    {
        return field.FieldType;
    }

    public static bool HasProperties(this CustomAttribute customAttribute)
    {
        return customAttribute.HasProperties;
    }

    public static string Name(this Mono.Cecil.CustomAttributeNamedArgument n)
    {
        return n.Name;
    }

    public static string Name(this Mono.Cecil.ParameterDefinition n)
    {
        return n.Name;
    }

    public static object ArgumentValue(this Mono.Cecil.CustomAttributeNamedArgument n)
    {
        return n.Argument.Value;
    }

    public static object Value(this Mono.Cecil.CustomAttributeArgument n)
    {
        return n.Value;
    }

    public static TypeReference PropertyType(this Mono.Cecil.PropertyDefinition n)
    {
        return n.PropertyType;
    }

    public static IList<Mono.Cecil.CustomAttributeNamedArgument> GetProperties(this CustomAttribute customAttribute)
    {
        return customAttribute.Properties;
    }

    public static IList<Mono.Cecil.CustomAttributeArgument> ConstructorArguments(this CustomAttribute attributeData)
    {
        return attributeData.ConstructorArguments;
    }

    public static Mono.Cecil.MethodReference Constructor(this CustomAttribute attributeData)
    {
        return attributeData.Constructor;
    }

    public static IList<Mono.Cecil.CustomAttributeNamedArgument> NamedArguments(this CustomAttribute attributeData)
    {
        return attributeData.Properties;
    }

    public static bool IsPublic(this MethodDefinition method)
    {
        return method.IsPublic;
    }

    public static bool IsPublic(this FieldDefinition fieldSymbol)
    {
        return fieldSymbol.IsPublic;
    }

    public static bool HasConstant(this FieldDefinition fieldSymbol)
    {
        return fieldSymbol.HasConstant;
    }

    public static object Constant(this FieldDefinition fieldSymbol)
    {
        return fieldSymbol.Constant;
    }

    public static bool HasNestedTypes(this TypeDefinition type)
    {
        return type.HasNestedTypes;
    }

    public static IEnumerable<TypeDefinition> NestedTypes(this TypeDefinition type)
    {
        return type.NestedTypes;
    }

    public static bool IsEnum(this TypeDefinition type)
    {
        return type.IsEnum;
    }

    public static bool HasConstructorArguments(this CustomAttribute attr)
    {
        return attr.HasConstructorArguments;
    }

    public static IList<TypeReference> GenericArguments(this GenericInstanceType type)
    {
        return type.GenericArguments;
    }

    public static bool IsPrimitive(this TypeReference type)
    {
        return type.IsPrimitive;
    }

    public static bool IsArray(this TypeReference type)
    {
        return type.IsArray;
    }

    public static TypeReference ElementType(this TypeReference type)
    {
        return type.GetElementType();
    }

    public static string MetadataName(this TypeReference type)
    {
        return type.Name;
    }
#endif

    public static bool IsOrSubClassOf(TypeReference childTypeDef, string ns, string name)
    {
        return FindIsOrSubClassOf(childTypeDef, ns, name) != null;
    }

    private static TypeReference FindIsOrSubClassOf(TypeReference typeRef, string ns, string name)
    {
        if (typeRef.Name == name &&
            typeRef.NamespaceOf() == ns)
            return typeRef;

        return EnumerateBaseClasses(typeRef)
            .FirstOrDefault(b => b.Name == name &&
                b.NamespaceOf() == ns);
    }

    public static bool IsVoid(TypeReference type)
    {
#if ISSOURCEGENERATOR
        return type.SpecialType == SpecialType.System_Void;
#else
        while (type is Mono.Cecil.OptionalModifierType || 
            type is Mono.Cecil.RequiredModifierType)
            type = ((Mono.Cecil.TypeSpecification)type).ElementType;
        return type.MetadataType == Mono.Cecil.MetadataType.Void;
#endif
    }

    public static TypeReference FindIsOrSubClassOf(TypeReference typeRef, TypeReference[] baseClasses, string ns, string name)
    {
        if (typeRef.Name == name &&
            typeRef.NamespaceOf() == ns)
            return typeRef;

        return baseClasses.FirstOrDefault(b => b.Name == name &&
            b.NamespaceOf() == ns);
    }

    public static bool Contains(TypeReference[] classes, string ns, string name)
    {
        return FindByName(classes, ns, name) != null;
    }

    private static TypeReference FindByName(TypeReference[] classes, string ns, string name)
    {
        foreach (var x in classes)
            if (x.MetadataName() == name &&
                x.NamespaceOf() == ns)
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
        if (type.Scope is Mono.Cecil.ModuleDefinition md)
            return (type.FullName, md.Assembly.Name.FullName);
        else if (type.Scope is Mono.Cecil.AssemblyNameReference asm)
            return (type.FullName, asm.FullName);
        else
            return (type.FullName, type.Scope.Name);
    }

    private static readonly Dictionary<(string, string), List<TypeReference>> BaseClassCache = new();
#endif


    public static IEnumerable<TypeReference> EnumerateBaseClasses(TypeReference typeRef)
    {
        if (typeRef is null)
            return Array.Empty<TypeReference>();

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

        var baseType = typeDef?.BaseType;
        if (baseType != null)
        {
            list.Add(baseType);
            list.AddRange(EnumerateBaseClasses(baseType));
        }

#if !ISSOURCEGENERATOR
        BaseClassCache[key] = list;
#endif
        return list;
    }

    public static bool IsSubclassOf(TypeReference type, string ns, string name)
    {
        if (type.Name == name &&
            type.NamespaceOf() == ns)
            return false;

        return EnumerateBaseClasses(type).Any(b => 
            b.Name == name &&
            b.NamespaceOf() == ns);
    }

#if !ISSOURCEGENERATOR
    public static Mono.Cecil.AssemblyDefinition[] ToDefinitions(IGeneratorFileSystem fileSystem,
        IEnumerable<string> assemblyLocations)
    {
        if (assemblyLocations == null || !assemblyLocations.Any())
            return System.Array.Empty<Mono.Cecil.AssemblyDefinition>();

        assemblyLocations = assemblyLocations.Select(x =>
        {
            if (!fileSystem.FileExists(x))
                return x;

            var path = fileSystem.GetDirectoryName(x);
            var asmInfo = fileSystem.Combine(path, "__AssemblyInfo__.ini");
            if (!fileSystem.FileExists(asmInfo))
                return x;

            var content = fileSystem.ReadAllText(asmInfo, System.Text.Encoding.Unicode);
            var idx = content.LastIndexOf("\0file:///", StringComparison.Ordinal);
            if (idx < 0)
                return x;

            var end = content.IndexOf('\0', idx + 9);
            if (end < 0)
                return x;

            var location = content.Substring(idx + 9, end - idx - 9).Replace('/', '\\');
            if (fileSystem.FileExists(location))
                return location;

            return x;
        }).ToList();

        var module = ICSharpCode.Decompiler.UniversalAssemblyResolver
            .LoadMainModule(assemblyLocations.First(), inMemory: true);

        if (assemblyLocations.Count() == 1)
            return new[] { module.Assembly };

        var resolver = module.AssemblyResolver as ICSharpCode.Decompiler.UniversalAssemblyResolver;

        foreach (var assembly in assemblyLocations)
            resolver.AddSearchDirectory(fileSystem.GetDirectoryName(assembly));

        var assemblyDefinitions = new List<Mono.Cecil.AssemblyDefinition>();
        foreach (var assembly in assemblyLocations)
            assemblyDefinitions.Add(Mono.Cecil.AssemblyDefinition.ReadAssembly(
                assembly, new Mono.Cecil.ReaderParameters
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
        return IsAssignableFrom(baseType.FullNameOf(), type);
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

            if (baseTypeFullName == current.FullNameOf())
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
        if (type is GenericInstanceType namedType &&
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
        if (type.TypeKind == TypeKind.Enum)
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