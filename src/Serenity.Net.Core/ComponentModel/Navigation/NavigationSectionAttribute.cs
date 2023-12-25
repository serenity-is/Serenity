namespace Serenity.Navigation;

/// <summary>
/// Navigation section attribute
/// </summary>
/// <remarks>
/// Creates a new instance of the class
/// </remarks>
/// <param name="order">Display order</param>
/// <param name="title">Title</param>
/// <param name="icon">Icon class</param>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class NavigationSectionAttribute(int order, string title, string? icon = null) : NavigationGroupAttribute(order, title, icon)
{

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