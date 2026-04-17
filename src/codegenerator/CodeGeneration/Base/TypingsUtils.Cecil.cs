#if !ISSOURCEGENERATOR
namespace Serenity.Reflection;

public static partial class TypingsUtils
{
    public static object ArgumentValue(this Mono.Cecil.CustomAttributeNamedArgument n)
    {
        return n.Argument.Value;
    }

    public static TypeReference AttributeType(this CustomAttribute attribute)
    {
        return attribute.AttributeType;
    }

    public static object Constant(this FieldDefinition fieldSymbol)
    {
        return fieldSymbol.Constant;
    }

    public static Mono.Cecil.MethodReference Constructor(this CustomAttribute attributeData)
    {
        return attributeData.Constructor;
    }

    public static IList<Mono.Cecil.CustomAttributeArgument> ConstructorArguments(this CustomAttribute attributeData)
    {
        return attributeData.ConstructorArguments;
    }

    public static TypeReference DeclaringType(this TypeReference typeSymbol)
    {
        return typeSymbol.DeclaringType;
    }

    public static TypeReference DeclaringType(this FieldDefinition field)
    {
        return field.DeclaringType;
    }

    public static TypeReference ElementType(this TypeReference type)
    {
        return type.GetElementType();
    }

    private static readonly Dictionary<(string, string), List<TypeReference>> BaseClassCache = [];

    public static IEnumerable<TypeReference> EnumerateBaseClasses(this TypeReference typeRef)
    {
        if (typeRef is null)
            return [];

        var key = GetCacheKey(typeRef);
        if (BaseClassCache.TryGetValue(key, out var cached))
            return cached;

        var list = new List<TypeReference>();

        if (typeRef is not TypeDefinition typeDef)
            typeDef = typeRef.Resolve();

        var baseType = typeDef?.BaseType;
        if (baseType != null)
        {
            list.Add(baseType);
            list.AddRange(EnumerateBaseClasses(baseType));
        }

        BaseClassCache[key] = list;
        return list;
    }

    public static TypeReference FieldType(this FieldDefinition field)
    {
        return field.FieldType;
    }

    public static IEnumerable<FieldDefinition> FieldsOf(this TypeDefinition type)
    {
        return type.Fields;
    }

    public static string FullNameOf(this TypeReference symbol)
    {
        return symbol.FullName;
    }

    public static IList<TypeReference> GenericArguments(this GenericInstanceType type)
    {
        return type.GenericArguments;
    }

    public static IEnumerable<CustomAttribute> GetAttributes(this PropertyDefinition prop, string ns, string name, bool subAttributes = false)
    {
        return prop.CustomAttributes.Where(x => (x.AttributeType?.Namespace == ns &&
            x.AttributeType.Name == name) ||
            (subAttributes && TypingsUtils.IsSubclassOf(x.AttributeType, ns, name)));
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

    public static IEnumerable<CustomAttribute> GetAttributesWithIntrinsic(this PropertyDefinition prop)
    {
        var attributes = prop.CustomAttributes;
        foreach (var attr in attributes)
            yield return attr;

        foreach (var attr in attributes)
        {
            if (attr.AttributeType != null &&
                attr.AttributeType.Resolve() is TypeDefinition type &&
                type.Interfaces.Any(i => i.InterfaceType?.Name == "IIntrinsicPropertyAttributeProvider" &&
                    i.InterfaceType.Namespace == "Serenity.Reflection") &&
                type.Properties.FirstOrDefault(x => x.Name == "PropertyAttributes") is PropertyDefinition property)
            {
                foreach (var intrinsicAttr in property.CustomAttributes)
                    yield return intrinsicAttr;
            }
        }
    }

    public static (string, string) GetCacheKey(TypeReference type)
    {
        if (type.Scope is Mono.Cecil.ModuleDefinition md)
            return (type.FullName, md.Assembly.Name.FullName);
        else if (type.Scope is Mono.Cecil.AssemblyNameReference asm)
            return (type.FullName, asm.FullName);
        else
            return (type.FullName, type.Scope.Name);
    }

    public static TypeDefinition GetEnumTypeFrom(TypeReference type)
    {
        type = GetNullableUnderlyingType(type) ?? type;

        if (!type.IsValueType || type.IsPrimitive)
            return null;

        var definition = type.Resolve();
        return definition.IsEnum ? definition : null;
    }

    public static TypeDefinition InterfaceType(this Mono.Cecil.InterfaceImplementation intf)
    {
        return intf.InterfaceType.Resolve();
    }

    public static bool IsEnum(this TypeReference type)
    {
        return type != null && type.Resolve()?.IsEnum == true;
    }

    public static TypeReference GetNullableUnderlyingType(TypeReference type)
    {
        if (type is GenericInstanceType &&
            type.Name == "Nullable`1" &&
            type.Namespace == "System")
            return (type as GenericInstanceType).GenericArguments[0];

        return null;
    }

    public static IList<Mono.Cecil.CustomAttributeNamedArgument> GetProperties(this CustomAttribute customAttribute)
    {
        return customAttribute.Properties;
    }

    public static bool HasConstant(this FieldDefinition fieldSymbol)
    {
        return fieldSymbol.HasConstant;
    }

    public static bool HasConstructorArguments(this CustomAttribute attr)
    {
        return attr.HasConstructorArguments;
    }

    public static bool HasCustomAttributes(this FieldDefinition field)
    {
        return field.HasCustomAttributes;
    }

    public static bool HasCustomAttributes(this PropertyDefinition prop)
    {
        return prop.HasCustomAttributes;
    }

    public static bool HasNestedTypes(this TypeDefinition type)
    {
        return type.HasNestedTypes;
    }

    public static bool HasProperties(this CustomAttribute customAttribute)
    {
        return customAttribute.HasProperties;
    }

    public static bool IsArray(this TypeReference type)
    {
        return type.IsArray;
    }

    public static bool IsAssignableFrom(TypeReference baseType, TypeReference type)
    {
        return IsAssignableFrom(baseType.FullName, type.Resolve());
    }


    public static bool IsConstructor(this MethodDefinition methodDefinition)
    {
        return methodDefinition.IsConstructor;
    }

    public static bool IsEnum(this TypeDefinition type)
    {
        return type.IsEnum;
    }

    public static bool IsGenericInstanceType(this TypeReference type, out TypeReference elementType)
    {
        if (type.IsGenericInstance)
        {
            elementType = (type as GenericInstanceType).ElementType;
            return true;
        }

        elementType = null;
        return false;
    }

    public static bool IsNested(this TypeDefinition type)
    {
        return type.IsNested;
    }

    public static bool IsNested(this TypeReference typeDefinition)
    {
        return typeDefinition.IsNested;
    }

    public static bool IsPrimitive(this TypeReference type)
    {
        return type.IsPrimitive;
    }

    public static bool IsPublic(this MethodDefinition method)
    {
        return method.IsPublic;
    }

    public static bool IsPublic(this FieldDefinition fieldSymbol)
    {
        return fieldSymbol.IsPublic;
    }

    public static bool IsPublicInstanceProperty(PropertyDefinition property)
    {
        if (property.IsStatic())
            return false;

        if ((property.GetMethod == null ||
            !property.GetMethod.IsPublic) &&
            (property.SetMethod == null ||
            !property.SetMethod.IsPublic))
            return false;

        return true;
    }

    public static bool IsSpecialName(this FieldDefinition field)
    {
        return field.IsSpecialName;
    }

    public static bool IsStatic(this PropertyDefinition prop)
    {
        return !prop.HasThis;
    }

    public static bool IsVoid(this TypeReference type)
    {
        while (type is Mono.Cecil.OptionalModifierType || 
            type is Mono.Cecil.RequiredModifierType)
            type = ((Mono.Cecil.TypeSpecification)type).ElementType;
        return type.MetadataType == Mono.Cecil.MetadataType.Void;
    }

    public static string MetadataName(this TypeReference type)
    {
        return type.Name;
    }    

    public static IEnumerable<MethodDefinition> MethodsOf(this TypeDefinition type)
    {
        return type.Methods;
    }

    public static string Name(this Mono.Cecil.CustomAttributeNamedArgument n)
    {
        return n.Name;
    }

    public static string Name(this Mono.Cecil.ParameterDefinition n)
    {
        return n.Name;
    }

    public static IList<Mono.Cecil.CustomAttributeNamedArgument> NamedArguments(this CustomAttribute attributeData)
    {
        return attributeData.Properties;
    }

    public static string NamespaceOf(this TypeReference symbol)
    {
        return symbol.Namespace;
    }

    public static IEnumerable<TypeDefinition> NestedTypes(this TypeDefinition type)
    {
        return type.NestedTypes;
    }

    public static IEnumerable<PropertyDefinition> PropertiesOf(this TypeDefinition type)
    {
        return type.Properties;
    }

    public static TypeReference PropertyType(this Mono.Cecil.PropertyDefinition n)
    {
        return n.PropertyType;
    }

    public static IEnumerable<TypeDefinition> SelfAndBaseClasses(this TypeDefinition klassType)
    {
        for (var td = klassType; td != null;
            td = td.BaseType?.Resolve())
            yield return td;
    }

    public static Mono.Cecil.AssemblyDefinition[] ToDefinitions(IFileSystem fileSystem,
            IEnumerable<string> assemblyLocations)
    {
        if (assemblyLocations == null || !assemblyLocations.Any())
            return [];

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
            return [module.Assembly];

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

        return [.. assemblyDefinitions];
    }

    public static object Value(this Mono.Cecil.CustomAttributeArgument n)
    {
        return n.Value;
    }
}
#endif