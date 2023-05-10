namespace Serenity.Navigation;

/// <summary>
/// Navigation section attribute
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class NavigationSectionAttribute : NavigationGroupAttribute
{
    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    /// <param name="order">Display order</param>
    /// <param name="title">Title</param>
    /// <param name="icon">Icon class</param>
    public NavigationSectionAttribute(int order, string title, string? icon = null)
        : base(order, title, icon)
    {
    }

    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    /// <param name="title">Title</param>
    /// <param name="icon">Icon class</param>
    public NavigationSectionAttribute(string title, string? icon = null)
        : this(int.MaxValue, title, icon)
    {
    }
}