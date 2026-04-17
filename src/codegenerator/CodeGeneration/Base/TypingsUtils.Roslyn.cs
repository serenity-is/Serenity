#if ISSOURCEGENERATOR
using Microsoft.CodeAnalysis;

namespace Serenity.Reflection;

public static partial class TypingsUtils
{
    public static object ArgumentValue(this KeyValuePair<string, TypedConstant> keyValuePair)
    {
        return keyValuePair.Value.Kind == TypedConstantKind.Array ?
            keyValuePair.Value.Values : keyValuePair.Value.Value;
    }

    public static TypeDefinition AttributeType(this CustomAttribute attributeData)
    {
        return attributeData.AttributeClass;
    }

    public static object Constant(this FieldDefinition fieldSymbol)
    {
        return fieldSymbol.ConstantValue;
    }

    public static MethodDefinition Constructor(this CustomAttribute attributeData)
    {
        return attributeData.AttributeConstructor;
    }

    public static IList<TypedConstant> ConstructorArguments(this CustomAttribute attributeData)
    {
        return attributeData.ConstructorArguments;
    }

    public static GenericInstanceType DeclaringType(this FieldDefinition fieldSymbol)
    {
        return fieldSymbol.ContainingType;
    }

    public static GenericInstanceType DeclaringType(this TypeDefinition typeSymbol)
    {
        return typeSymbol.ContainingType;
    }

    public static TypeDefinition ElementType(this TypeDefinition type)
    {
        if (type is IArrayTypeSymbol ats)
            return ats.ElementType;

        if (type is GenericInstanceType git)
            return git.OriginalDefinition;

        return null;
    }

    public static IEnumerable<TypeReference> EnumerateBaseClasses(this TypeReference typeRef)
    {
        if (typeRef == null)
            yield break;
        while ((typeRef = typeRef.BaseType) != null)
            yield return typeRef;
    }

    public static TypeDefinition FieldType(this FieldDefinition field)
    {
        return field.Type;
    }

    public static IEnumerable<FieldDefinition> FieldsOf(this TypeDefinition type)
    {
        return type.GetMembers().OfType<FieldDefinition>();
    }

    public static string FullNameOf(this ISymbol symbol)
    {
        var ns = NamespaceOf(symbol);
        if (string.IsNullOrEmpty(ns))
            return symbol.MetadataName;

        return ns + "." + symbol.MetadataName;
    }

    public static IList<TypeReference> GenericArguments(this GenericInstanceType type)
    {
        return type.TypeArguments;
    }

    public static IEnumerable<CustomAttribute> GetAttributesWithIntrinsic(this PropertyDefinition prop)
    {
        var attributes = prop.GetAttributes();
        foreach (var attr in attributes)
            yield return attr;

        foreach (var attr in attributes)
        {
            if (attr.AttributeClass != null &&
                attr.AttributeClass.Interfaces.Any(i => i.Name == "IIntrinsicPropertyAttributeProvider" && i.NamespaceOf() == "Serenity.Reflection") &&
                attr.AttributeClass.GetMembers("PropertyAttributes").OfType<PropertyDefinition>().FirstOrDefault() is PropertyDefinition property)
            {
                foreach (var intrinsicAttr in property.GetAttributes())
                    yield return intrinsicAttr;
            }
        }
    }

    public static TypeDefinition GetEnumTypeFrom(TypeReference type)
    {
        type = GetNullableUnderlyingType(type) ?? type;
        if (type.TypeKind == TypeKind.Enum)
            return type;

        return null;
    }

    public static TypeReference GetNullableUnderlyingType(TypeReference type)
    {
        if (type is GenericInstanceType namedType &&
            namedType.IsGenericType &&
            namedType.OriginalDefinition.SpecialType == Microsoft.CodeAnalysis.SpecialType.System_Nullable_T)
            return namedType.TypeArguments[0];

        return null;
    }

    public static IList<KeyValuePair<string, TypedConstant>> GetProperties(this CustomAttribute customAttribute)
    {
        return customAttribute.NamedArguments;
    }

    public static bool HasConstant(this FieldDefinition fieldSymbol)
    {
        return fieldSymbol.HasConstantValue;
    }

    public static bool HasConstructorArguments(this CustomAttribute attr)
    {
        return attr.ConstructorArguments.Any();
    }

    public static bool HasCustomAttributes(this FieldDefinition field)
    {
        return field.GetAttributes().Any();
    }

    public static bool HasCustomAttributes(this PropertyDefinition prop)
    {
        return prop.GetAttributes().Any();
    }

    public static bool HasNestedTypes(this TypeReference type)
    {
        return type.GetTypeMembers().Any();
    }

    public static bool HasProperties(this CustomAttribute customAttribute)
    {
        return customAttribute.NamedArguments.Any();
    }

    public static TypeDefinition InterfaceType(this TypeDefinition intf)
    {
        return intf;
    }

    public static bool IsArray(this TypeDefinition type)
    {
        return type.TypeKind == TypeKind.Array;
    }


    public static bool IsAssignableFrom(TypeReference baseType, TypeReference type)
    {
        if (baseType == null || type == null)
            return false;

        if (baseType.TypeKind == TypeKind.Interface)
            return type.AllInterfaces.Any(t => SymbolEqualityComparer.Default.Equals(t, baseType));

        if (SymbolEqualityComparer.Default.Equals(type, baseType))
            return true;

        return type.EnumerateBaseClasses().Any(t => SymbolEqualityComparer.Default.Equals(t, baseType));
    }

    public static bool IsConstructor(this IMethodSymbol methodSymbol)
    {
        return methodSymbol.MethodKind == MethodKind.Constructor;
    }

    public static bool IsEnum(this TypeReference type)
    {
        return type.TypeKind == TypeKind.Enum;
    }

    public static bool IsGenericInstanceType(this TypeDefinition typeSymbol, out GenericInstanceType originalDefinition)
    {
        if (typeSymbol is GenericInstanceType nt &&
            nt.IsGenericType && !nt.TypeArguments.Any(x => x.TypeKind == TypeKind.TypeParameter))
        {
            originalDefinition = nt.OriginalDefinition;
            return true;
        }
        else
        {
            originalDefinition = null;
            return false;
        }
    }

    public static bool IsNested(this TypeDefinition typeSymbol)
    {
        return typeSymbol.ContainingType != null;
    }

    public static bool IsPrimitive(this TypeReference type)
    {
        return type.SpecialType >= SpecialType.System_Boolean &&
            type.SpecialType <= SpecialType.System_UIntPtr;
    }

    public static bool IsPublic(this MethodDefinition fieldSymbol)
    {
        return fieldSymbol.DeclaredAccessibility == Accessibility.Public;
    }

    public static bool IsPublic(this FieldDefinition fieldSymbol)
    {
        return fieldSymbol.DeclaredAccessibility == Accessibility.Public;
    }

    public static bool IsPublicInstanceProperty(PropertyDefinition property)
    {
        if (property.IsStatic)
            return false;

        if ((property.GetMethod == null ||
            property.GetMethod.DeclaredAccessibility != Accessibility.Public) &&
            (property.SetMethod == null ||
            property.SetMethod.DeclaredAccessibility != Accessibility.Public))
            return false;

        return true;
    }

    public static bool IsSpecialName(this FieldDefinition field)
    {
        return field.IsImplicitlyDeclared;
    }

    public static bool IsStatic(this PropertyDefinition prop)
    {
        return prop.IsStatic;
    }

    public static bool IsVoid(this TypeReference type)
    {
        return type.SpecialType == SpecialType.System_Void;
    }

    public static string MetadataName(this TypeReference type)
    {
        return type.MetadataName;
    }

    public static IEnumerable<IMethodSymbol> MethodsOf(this TypeDefinition type)
    {
        return type.GetMembers().OfType<IMethodSymbol>();
    }

    public static string Name(this KeyValuePair<string, TypedConstant> keyValuePair)
    {
        return keyValuePair.Key;
    }

    public static string Name(this IParameterSymbol parameterSymbol)
    {
        return parameterSymbol.Name;
    }

    public static IList<KeyValuePair<string, TypedConstant>> NamedArguments(this CustomAttribute attributeData)
    {
        return attributeData.NamedArguments;
    }

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

    public static IEnumerable<TypeReference> NestedTypes(this TypeReference type)
    {
        return type.GetTypeMembers();
    }

    public static IEnumerable<PropertyDefinition> PropertiesOf(this TypeDefinition type)
    {
        return type.GetMembers().OfType<PropertyDefinition>();
    }

    public static TypeDefinition PropertyType(this PropertyDefinition prop)
    {
        return prop.Type;
    }

    public static TypeDefinition Resolve(this TypeDefinition typeSymbol)
    {
        return typeSymbol;
    }

    public static IEnumerable<TypeDefinition> SelfAndBaseClasses(this TypeDefinition klassType)
    {
        for (var td = klassType; td != null;
            td = td.BaseType)
            yield return td;
    }

    public static object Value(this TypedConstant constant)
    {
        return constant.Kind == TypedConstantKind.Array ?
            constant.Values : constant.Value;
    }
}
#endif