namespace Serenity.Navigation;

/// <summary>
/// A node in a navigation tree model.
/// </summary>
public class NavigationItem
{
    /// <summary>
    /// Title
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Full path of the item
    /// </summary>
    public string? FullPath { get; set; }

    /// <summary>
    /// Icon class
    /// </summary>
    public string? IconClass { get; set; }

    /// <summary>
    /// Item class
    /// </summary>
    public string? ItemClass { get; set; }

    /// <summary>
    /// URL
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Target window to open the link, e.g. "_blank"
    /// </summary>
    public string? Target { get; set; }

    /// <summary>
    /// Parent navigation item
    /// </summary>
    public NavigationItem? Parent { get; set; }

    /// <summary>
    /// List of children
    /// </summary>
    public List<NavigationItem> Children { get; private set; } = [];

    /// <summary>
    /// True if this is a navigation section
    /// </summary>
    public bool IsSection { get; set; }

    /// <summary>
    /// Effective order among siblings. Unlike the declared <c>Order</c> on
    /// <see cref="NavigationItemAttribute"/> (which may be assigned from an <see cref="int"/> in
    /// an assembly attribute), this is a
    /// <see cref="decimal"/> so that duplicate declared orders can be resolved to unique
    /// values (see <c>NavigationHelper.HealOrders</c>), and so consumers (e.g. a CMS
    /// inserting dynamic pages into the menu) can compute a midpoint between two siblings.
    /// </summary>
    public decimal Order { get; set; }
}