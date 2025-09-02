namespace Serenity.Navigation;

/// <summary>
/// Navigation item source abstraction
/// </summary>
public interface INavigationItemSource
{
    /// <summary>
    /// Gets list of navigation item attributes (<see cref="NavigationItemAttribute"/>)
    /// </summary>
    List<NavigationItemAttribute> GetItems();
}