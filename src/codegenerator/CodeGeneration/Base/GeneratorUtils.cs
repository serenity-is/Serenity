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

    private class PackageJson
    {
#pragma warning disable IDE1006 // Naming Styles
        public Dictionary<string, string> dependencies { get; set; }
        public Dictionary<string, string> devDependencies { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }

    public static IDictionary<string, string> GetAssemblyToPackageMappings(
        IFileSystem fileSystem, string csproj)
    {
        var result = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        var projectDir = fileSystem.GetDirectoryName(csproj);
        var packageJson = fileSystem.Combine(projectDir, "package.json");
        if (!fileSystem.FileExists(packageJson))
            return result;

        var packageData = CodeGenerator.TSConfigHelper.TryParseJsonFile<PackageJson>(fileSystem, packageJson);

        foreach (var pair in (packageData.dependencies ?? [])
            .Concat(packageData.devDependencies ?? []))
        {
            if (pair.Value == null)
                continue;

            var dotnetIdx = pair.Value.IndexOf("/node_modules/.dotnet/", StringComparison.Ordinal);
            if (dotnetIdx >= 0)
            {
                var projectPath = pair.Value[(dotnetIdx + "/node_modules/.dotnet/".Length)..];
                if (projectPath.IndexOf('/') < 0)
                    result[projectPath] = pair.Key;
                continue;
            }

            string value = pair.Value;
            if (value.StartsWith("file://", StringComparison.Ordinal))
                value = value["file://".Length..];

            string path;
            if (value.StartsWith("./", StringComparison.Ordinal) ||
                value.StartsWith("../", StringComparison.Ordinal))
            {
                path = fileSystem.Combine(projectDir, value);
            }
            else if (value.StartsWith("workspace:", StringComparison.Ordinal))
            {
                path = fileSystem.Combine(projectDir, "node_modules", pair.Key);
            }
            else
                continue;

            var csprojFiles = fileSystem.GetFiles(path, "*.csproj", recursive: false);
            if (csprojFiles.Length == 1)
                result[fileSystem.GetFileNameWithoutExtension(csprojFiles[0])] = pair.Key;
        }

        return result;
    }
}