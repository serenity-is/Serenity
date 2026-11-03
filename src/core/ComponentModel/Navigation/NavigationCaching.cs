namespace Serenity.Navigation;

/// <summary>
/// The cache group keys related to the navigation items, so that code changing what a navigation item source
/// returns can expire them without having to know how a navigation model factory caches its items.
/// </summary>
public static class NavigationCaching
{
    /// <summary>
    /// The cache group key that navigation model factories should use for the navigation items, in addition to
    /// any other groups they depend on, like the permissions of the users.
    /// </summary>
    public const string ItemsGroupKey = "Navigation.Items";
}
