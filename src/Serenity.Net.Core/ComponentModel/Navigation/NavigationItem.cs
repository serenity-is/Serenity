namespace Serenity.Navigation;

/// <summary>
/// A node in a navigation tree model
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
    public List<NavigationItem> Children { get; private set; } = new();

    /// <summary>
    /// True if this is a navigation section
    /// </summary>
    public bool IsSection { get; set; }
}