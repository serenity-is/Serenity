namespace Serenity.Localization;

/// <summary>
/// Contains helper methods for registration of permission keys in nested static classes.
/// These classes contain string constants containing permission keys.
/// The display name for permission keys can be set by a <see cref="DisplayNameAttribute"/>.
/// The display name for groups can be set by a <see cref="DescriptionAttribute"/> on the classes themselves.
/// For the group display name to work, all constants in a class must start with the same group prefix.
/// </summary>
public static class NestedPermissionKeyRegistration
{
    /// <summary>
    /// Gets permission keys and adds texts, if any, from static nested permission key
    /// classes marked with <see cref="NestedPermissionKeysAttribute"/>.
    /// </summary>
    /// <param name="registry">The registry to add texts to, or <c>null</c>.</param>
    /// <param name="typeSource">The type source to search for nested permission key classes in.</param>
    /// <returns>A set of the permission keys found.</returns>
    public static HashSet<string> AddNestedPermissions(this ILocalTextRegistry? registry, 
        ITypeSource typeSource)
    {
        if (typeSource == null)
            throw new ArgumentNullException(nameof(typeSource));

        var permissions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var type in typeSource.GetTypesWithAttribute(typeof(NestedPermissionKeysAttribute)))
        {
            var attr = type.GetCustomAttribute<NestedPermissionKeysAttribute>();
            if (attr != null)
                AddKeysFrom(permissions, registry, type, 
                    attr.LanguageID ?? LocalText.InvariantLanguageID);
        }

        return permissions;
    }

    private static readonly char[] splitChar = ['|', '&'];

    private static void AddKeysFrom(HashSet<string> permissions, ILocalTextRegistry? registry, 
        Type type, string languageID)
    {
        var thisKeys = new List<string>();

        foreach (var member in type.GetFields(BindingFlags.Static | BindingFlags.DeclaredOnly |
            BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (member.FieldType != typeof(string))
                continue;

            if (member.GetValue(null) is not string key ||
                string.IsNullOrEmpty(key) ||
                key.IndexOfAny(splitChar) >= 0) // skip permissions with logical operators
                continue;

            if (key.EndsWith(":"))
            {
                if (registry != null)
                    AddDescriptionText(registry, languageID, key, member.Name,
                        member.GetCustomAttribute<DescriptionAttribute>());

                continue;
            }

            thisKeys.Add(key);

            permissions?.Add(key);

            if (registry != null)
                AddDescriptionText(registry, languageID, key, member.Name,
                    member.GetCustomAttribute<DescriptionAttribute>());
        }

        if (registry != null && thisKeys.Count > 0)
        {
            var displayName = type.GetCustomAttribute<DisplayNameAttribute>();

            var lastColonIndex = thisKeys[0].LastIndexOf(":");
            if (displayName != null &&
                lastColonIndex > 0 &&
                lastColonIndex < thisKeys[0].Length - 1 &&
                thisKeys.TrueForAll(x => x.LastIndexOf(":") == lastColonIndex))
            {
                var defaultKey = "Permission." + thisKeys[0][..(lastColonIndex + 1)];
                var resolved = LocalText.ResolveFromAttribute(displayName, displayName.DisplayName, defaultKey);
                if (resolved.Key is string key)
                    registry.Add(languageID, key, resolved.Text);
            }
        }

        foreach (var nested in type.GetNestedTypes(BindingFlags.Public | BindingFlags.DeclaredOnly))
            AddKeysFrom(permissions!, registry, nested, languageID);
    }

    private static void AddDescriptionText(ILocalTextRegistry registry, string languageID,
        string permissionKey, string memberName, DescriptionAttribute? attribute)
    {
        var defaultKey = "Permission." + permissionKey;
        if (attribute is null)
        {
            registry.Add(languageID, defaultKey, memberName);
            return;
        }

        var resolved = LocalText.ResolveFromAttribute(attribute, attribute.Description, defaultKey);
        if (resolved.Key is string key)
            registry.Add(languageID, key, resolved.Text);
    }
}