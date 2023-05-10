namespace Serenity.Navigation;

/// <summary>
/// Defines a navigation group, which are usually the icons on the sidebar band
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class NavigationGroupAttribute : NavigationItemAttribute
{
    /// <summary>
    /// Creates a new instance of the attribute
    /// </summary>
    /// <param name="order">Order</param>
    /// <param name="title">Title</param>
    /// <param name="icon">Icon class</param>
    public NavigationGroupAttribute(int order, string title, string? icon = null)
        : base(order, title, null, null, icon)
    {
    }

    /// <summary>
    /// Creates a new instance of the attribute
    /// </summary>
    /// <param name="title">Title</param>
    /// <param name="icon">Icon</param>
    public NavigationGroupAttribute(string title, string? icon = null)
        : this(int.MaxValue, title, icon)
    {
    }

    /// <summary>
    /// This is a list used to move items that are not normally
    /// under this item based on path (to create groups).
    /// For example, if this item is named A,
    /// and want to move all menus under B/.. or C/.. to A,
    /// the list should be ["B/", "C/"]. To move B and C themselves under A,
    /// list should be ["B", "C"].
    /// </summary>
    public string[]? Include { get; set; }

    /// <summary>
    /// This group automatically includes siblings that does not match any other groups
    /// </summary>
    public bool Default { get; set; }
}