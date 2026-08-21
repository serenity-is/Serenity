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

        foreach (NavigationItemAttribute attr in typeSource.GetAssemblyAttributes<NavigationItemAttribute>())
            registry.Add(languageID, "Navigation." + (string.IsNullOrEmpty(attr.Category) ? "" : attr.Category + "/") +
                attr.Title, attr.Title);
    }
}