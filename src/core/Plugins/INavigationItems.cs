namespace Serenity.Plugins;

/// <summary>
/// Abstraction for plugins with navigation entries.
/// </summary>
/// <remarks>
/// This type is part of a legacy plugin system design and is no longer used by
/// the framework. Use the <c>NavigationLink</c> / <c>NavigationMenu</c> assembly
/// attributes instead to declare navigation items.
/// </remarks>
public interface INavigationItems
{
    /// <summary>
    /// Gets the navigation entries.
    /// </summary>
    /// <returns>The list of navigation entries.</returns>
    IEnumerable<NavigationEntry> GetNavigationEntries();
}