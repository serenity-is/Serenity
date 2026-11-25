using Serenity.Navigation;

namespace Serenity.Localization;

/// <summary>
/// Contains initialization methods for adding navigation item attribute related local texts.
/// </summary>
public static class NavigationLocalTextRegistration
{
    /// <summary>
    /// Adds navigation item related texts.
    /// </summary>
    /// <param name="registry">The registry to add texts to.</param>
    /// <param name="typeSource">The type source to search for navigation item attributes in.</param>
    /// <param name="languageID">The language ID texts will be added for (default is the invariant language).</param>
    public static void AddNavigationTexts(this ILocalTextRegistry registry, ITypeSource typeSource,
        string languageID = LocalText.InvariantLanguageID)
    {
        if (typeSource == null)
            throw new ArgumentNullException(nameof(typeSource));

        if (registry is null)
            throw new ArgumentNullException(nameof(registry));

        static string keyOf(NavigationItemAttribute attr) =>
            "Navigation." + (string.IsNullOrEmpty(attr.Category) ? "" : attr.Category + "/") + attr.Title;

        var keys = new HashSet<string>(typeSource.GetAssemblyAttributes<NavigationItemAttribute>().Select(keyOf),
            StringComparer.Ordinal);

        // the items of an assembly are registered for the language of its texts, see LocalTextLanguageAttribute; the
        // type source may leave out some assemblies, e.g. of a feature that is off, so only the items it gives count
        if (typeSource is IGetAssemblies getAssemblies)
        {
            foreach (var assembly in getAssemblies.GetAssemblies())
            {
                var assemblyLanguageID = LocalTextLanguages.ForAssembly(registry, assembly, languageID);
                foreach (var attr in assembly.GetCustomAttributes<NavigationItemAttribute>())
                {
                    var key = keyOf(attr);
                    if (keys.Remove(key))
                        registry.Add(assemblyLanguageID, key, attr.Title);
                }
            }
        }

        foreach (NavigationItemAttribute attr in typeSource.GetAssemblyAttributes<NavigationItemAttribute>())
        {
            var key = keyOf(attr);
            if (keys.Remove(key))
                registry.Add(languageID, key, attr.Title);
        }
    }
}