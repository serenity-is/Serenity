using Serenity.Navigation;

namespace Serenity.Localization;

/// <summary>
/// Contains initialization method for adding navigation item attribute
/// related local texts
/// </summary>
public static class NavigationLocalTextRegistration
{
    /// <summary>
    /// Adds navigation item related texts
    /// </summary>
    /// <param name="typeSource">Type source to search for enumeration classes in</param>
    /// <param name="languageID">Language ID texts will be added (default is invariant language)</param>
    /// <param name="registry">Registry</param>
    public static void AddNavigationTexts(this ILocalTextRegistry registry, ITypeSource typeSource,
        string languageID = LocalText.InvariantLanguageID)
    {
        if (typeSource == null)
            throw new ArgumentNullException(nameof(typeSource));

        if (registry is null)
            throw new ArgumentNullException(nameof(registry));

        foreach (NavigationItemAttribute attr in typeSource.GetAssemblyAttributes<NavigationItemAttribute>())
            registry.Add(languageID, "Navigation." + (attr.Category.IsEmptyOrNull() ? "" : attr.Category + "/") +
                attr.Title, attr.Title);
    }
}