namespace Serenity.Reflection;

public static partial class TypingsUtils
{
    public static bool Contains(TypeReference[] classes, string ns, string name)
    {
        return FindByName(classes, ns, name) != null;
    }

    public static CustomAttribute? FindAttr(IEnumerable<CustomAttribute>? attrList, string? ns, string name)
    {
        if (attrList == null)
            return null;

        foreach (var x in attrList)
            if (x.AttributeType() != null && IsOrSubClassOf(x.AttributeType(), ns, name))
                return x;

        return null;
    }

    private static TypeReference? FindByName(TypeReference[] classes, string ns, string name)
    {
        foreach (var x in classes)
            if (x.MetadataName() == name &&
                x.NamespaceOf() == ns)
                return x;

        return null;
    }


    public static CustomAttribute? GetAttr(TypeDefinition klass, string? ns, string name, TypeReference[]? baseClasses = null)
    {
        CustomAttribute? attr;

        attr = FindAttr(klass.GetAttributes(), ns, name);

        if (attr != null)
            return attr;

        foreach (var b in baseClasses ?? klass.EnumerateBaseClasses())
        {
            if (b is TypeDefinition typeDef ||
                (typeDef = b.Resolve()) is not null)

                attr = FindAttr(typeDef.GetAttributes(), ns, name);
            if (attr != null)
                return attr;
        }

        return null;
    }

    public static IEnumerable<CustomAttribute> GetAttrs(IEnumerable<CustomAttribute> attrList,
        string ns, string name, TypeDefinition[]? baseClasses = null)
    {
        if (attrList == null)
            yield break;

        foreach (var x in attrList)
            if (x.AttributeType() != null && IsOrSubClassOf(x.AttributeType(), ns, name))
                yield return x;

        if (baseClasses != null)
        {
            foreach (var b in baseClasses)
            {
                foreach (var x in b.GetAttributes())
                    if (x.AttributeType() != null && IsOrSubClassOf(x.AttributeType(), ns, name))
                        yield return x;
            }
        }

        yield break;
    }

    public static bool IsAssignableFrom(string baseTypeFullName, TypeDefinition? type)
    {
        if (type is null)
            return false;

        Queue<TypeDefinition> queue = new();
        queue.Enqueue(type);

        while (queue.Count != 0)
        {
            var current = queue.Dequeue();

            if (baseTypeFullName == current.FullNameOf())
                return true;

            if (current.BaseType != null)
                queue.Enqueue(current.BaseType.Resolve());

            foreach (var intf in current.Interfaces)
            {
                queue.Enqueue(intf.InterfaceType());
            }
        }

        return false;
    }

    public static bool IsOrSubClassOf(TypeReference? typeRef, string? ns, string name)
    {
        if (typeRef is null)
            return false;

        if (typeRef.Name == name &&
            typeRef.NamespaceOf() == ns)
            return true;

        return typeRef.EnumerateBaseClasses()
            .Any(b => b.Name == name &&
                b.NamespaceOf() == ns);
    }

    public static bool IsSubclassOf(TypeReference type, string ns, string name)
    {
        if (type.Name == name &&
            type.NamespaceOf() == ns)
            return false;

        return type.EnumerateBaseClasses().Any(b =>
            b.Name == name &&
            b.NamespaceOf() == ns);
    }

    public static string? GetAttributeKeyViaConstructorArgument(CustomAttribute attr)
    {
        if (attr.ConstructorArguments() is { Count: 1 } args &&
            args[0] is { } arg &&
            arg.Value is string value &&
            arg.Type?.FullNameOf() == "System.String")
            return value;

        return null;
    }

    public static string? GetAttributeKeyViaKeyConstant(TypeReference attributeType)
    {
        return attributeType.Resolve().FieldsOf().FirstOrDefault(x =>
            x.IsStatic &&
            x.IsPublic() &&
            x.Name == "Key" &&
            x.HasConstant() &&
            x.Constant() is string &&
            x.DeclaringType().FullNameOf() == attributeType.FullNameOf())?.Constant() as string;
    }

#if ISSOURCEGENERATOR
    public static string? GetModuleKeyForType(TypeDefinition rowType, Microsoft.CodeAnalysis.Compilation compilation, System.Threading.CancellationToken cancellationToken)
#else
    public static string? GetModuleKeyForType(TypeDefinition rowType)
#endif
    {
        var moduleAttr = GetAttr(rowType, "Serenity.ComponentModel", "ModuleAttribute");
        if (moduleAttr is null || moduleAttr.AttributeType() is not { } attributeType)
            return null;

        if (attributeType.FullNameOf() is "Serenity.ComponentModel.ModuleAttribute" &&
            GetAttributeKeyViaConstructorArgument(moduleAttr) is string ctorArgKey)
            return ctorArgKey;

        if ((GetAttributeKeyViaKeyConstant(attributeType) ??
#if ISSOURCEGENERATOR
            GetAttributeKeyViaBaseCtorCall(attributeType, compilation, cancellationToken)) is string typeKey)
#else
            GetAttributeKeyViaBaseCtorCall(attributeType)) is string typeKey)
#endif
            return typeKey;

        return null;
    }
}