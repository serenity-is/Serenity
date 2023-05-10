namespace Serenity.Navigation;

/// <summary>
/// Navigation model abstraction
/// </summary>
public interface INavigationModel
{
    /// <summary>
    /// Active navigation item
    /// </summary>
    NavigationItem? ActiveItem { get; }

    /// <summary>
    /// Navigation items in the active path
    /// </summary>
    IEnumerable<NavigationItem> ActivePath { get; }

    /// <summary>
    /// Navigation items tree
    /// </summary>
    IEnumerable<NavigationItem> Items { get; }
}