namespace Serenity.Reflection;

public static partial class TypingsUtils
{
    public static bool Contains(TypeReference[] classes, string ns, string name)
    {
        return FindByName(classes, ns, name) != null;
    }

    public static CustomAttribute FindAttr(IEnumerable<CustomAttribute> attrList, string ns, string name)
    {
        if (attrList == null)
            return null;

        foreach (var x in attrList)
            if (x.AttributeType() != null && IsOrSubClassOf(x.AttributeType(), ns, name))
                return x;

        return null;
    }

    private static TypeReference FindByName(TypeReference[] classes, string ns, string name)
    {
        foreach (var x in classes)
            if (x.MetadataName() == name &&
                x.NamespaceOf() == ns)
                return x;

        return null;
    }


    public static CustomAttribute GetAttr(TypeDefinition klass, string ns, string name, TypeReference[] baseClasses = null)
    {
        CustomAttribute attr;

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
        string ns, string name, TypeDefinition[] baseClasses = null)
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

    public static bool IsAssignableFrom(string baseTypeFullName, TypeDefinition type)
    {
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

    public static bool IsOrSubClassOf(TypeReference typeRef, string ns, string name)
    {
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
}