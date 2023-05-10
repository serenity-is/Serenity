namespace Serenity.Plugins;

/// <summary>
/// Abstraction for plugins with navigation entries
/// </summary>
public interface INavigationItems
{
    /// <summary>
    /// Gets the navigation entries.
    /// </summary>
    /// <returns>List of navigation entries.</returns>
    IEnumerable<NavigationEntry> GetNavigationEntries();
}