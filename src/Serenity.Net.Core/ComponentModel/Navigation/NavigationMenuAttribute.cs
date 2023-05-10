namespace Serenity.Navigation;

/// <summary>
/// Navigation menu without a link
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class NavigationMenuAttribute : NavigationItemAttribute
{
    /// <summary>
    /// Creates a new instance of the attribute
    /// </summary>
    /// <param name="order">Order</param>
    /// <param name="icon">Icon</param>
    /// <param name="title">Title</param>
    public NavigationMenuAttribute(int order, string title, string? icon = null)
        : base(order, title, null, null, icon)
    {
    }

    /// <summary>
    /// Creates a new instance of the attribute
    /// </summary>
    /// <param name="icon">Icon</param>
    /// <param name="title">Title</param>
    public NavigationMenuAttribute(string title, string? icon = null)
        : base(int.MaxValue, title, null, null, icon)
    {
    }
}