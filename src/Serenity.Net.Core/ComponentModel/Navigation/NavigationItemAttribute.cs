namespace Serenity.Navigation;

/// <summary>
/// Navigation item attribute
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public abstract class NavigationItemAttribute : Attribute
{
    /// <summary>
    /// Creates a new instance of the attribute
    /// </summary>
    /// <param name="order">Order</param>
    /// <param name="path">Path</param>
    /// <param name="url">Url</param>
    /// <param name="permission">Permission</param>
    /// <param name="icon">Icon class</param>
    protected NavigationItemAttribute(int order, string path, string? url, object? permission, string? icon)
    {
        path ??= "";
        FullPath = path;

        var idx = path.LastIndexOf('/');

        if (idx > 0 && path[idx - 1] == '/')
            idx = path.Replace("//", "\x1\x1", StringComparison.Ordinal).LastIndexOf('/');

        if (idx >= 0)
        {
            Category = path[..idx];
            Title = path[(idx + 1)..];
        }
        else
        { 
            Title = path;
        }

        Order = order;
        Permission = permission?.ToString();
        IconClass = icon;
        Url = url;
    }

    /// <summary>
    /// Gets / sets the order (only) among its siblings.
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Url of this navigation item, should be null for menu
    /// </summary>
    public string? Url { get; set; }
    
    /// <summary>
    /// The full path to navigation item like A/B/C
    /// This is used to generate local text key for this item
    /// like Navigation.A/B/C
    /// </summary>
    public string? FullPath { get; set; }

    /// <summary>
    /// This is full path of its parent, e.g. A/B for A/B/C
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Title of the navigation item. It is the part after last slash,
    /// e.g. C for A/B/C
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Icon class
    /// </summary>
    public string? IconClass { get; set; }

    /// <summary>
    /// Extra css class to apply to its navigation element e.g. LI
    /// </summary>
    public string? ItemClass { get; set; }
    
    /// <summary>
    /// Permission required to view this navigation item
    /// </summary>
    public string? Permission { get; set; }

    /// <summary>
    /// Window target to open this link, e.g. _blank etc.
    /// </summary>
    public string? Target { get; set; }
}