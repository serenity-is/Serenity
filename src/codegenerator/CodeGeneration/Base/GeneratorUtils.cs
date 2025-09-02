namespace Serenity.Reflection;

public static class GeneratorUtils
{
    public static bool IsEqualOrSubclassOf(Type type, string fullName)
    {
        return (type != null && type.FullName == fullName) ||
            IsSubclassOf(type, fullName);
    }

    public static bool IsSubclassOf(Type type, string fullName)
    {
        if (type == null)
            return false;

        type = type.BaseType;
        while (type != null)
        {
            if (type.FullName == fullName)
                return true;

            type = type.BaseType;
        }

        return false;
    }

    public static Attribute GetAttribute(MemberInfo member, string attributeType)
    {
        return member.GetCustomAttributes().FirstOrDefault(x => IsEqualOrSubclassOf(x.GetType(), attributeType));
    }

    public static bool IsSimpleType(Type type)
    {
        if (type == typeof(string) ||
            type == typeof(int) ||
            type == typeof(long) ||
            type == typeof(short) ||
            type == typeof(double) ||
            type == typeof(decimal) ||
            type == typeof(DateTime) ||
            type == typeof(bool) ||
            type == typeof(TimeSpan))
            return true;

        return false;
    }

    public static bool GetFirstDerivedOfGenericType(Type type, Type genericType, out Type derivedType)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == genericType)
        {
            derivedType = type;
            return true;
        }

        if (type.BaseType != null)
            return GetFirstDerivedOfGenericType(type.BaseType, genericType, out derivedType);

        derivedType = null;
        return false;
    }
}