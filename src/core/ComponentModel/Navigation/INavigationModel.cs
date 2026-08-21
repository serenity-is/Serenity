namespace Serenity.Navigation;

/// <summary>
/// Navigation model abstraction.
/// </summary>
public interface INavigationModel
{
    /// <summary>
    /// The active navigation item.
    /// </summary>
    NavigationItem? ActiveItem { get; }

    /// <summary>
    /// The navigation items in the active path.
    /// </summary>
    IEnumerable<NavigationItem> ActivePath { get; }

    /// <summary>
    /// The navigation items tree.
    /// </summary>
    IEnumerable<NavigationItem> Items { get; }
}