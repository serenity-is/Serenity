namespace Serenity.Plugins;

/// <summary>
/// Plugin navigation entry abstraction.
/// </summary>
public class NavigationEntry
{
    /// <summary>
    /// Gets or sets the order.
    /// </summary>
    /// <value>
    /// The order.
    /// </value>
    public int Order { get; set; }

    /// <summary>
    /// Gets or sets the controller type.
    /// </summary>
    /// <value>
    /// The controller type.
    /// </value>
    public Type? Controller { get; set; }

    /// <summary>
    /// Gets or sets the action name.
    /// </summary>
    /// <value>
    /// The action name.
    /// </value>
    public string? Action { get; set; }

    /// <summary>
    /// Gets or sets the URL.
    /// </summary>
    /// <value>
    /// The URL.
    /// </value>
    public string? Url { get; set; }

    /// <summary>
    /// Gets or sets the full path.
    /// </summary>
    /// <value>
    /// The full path.
    /// </value>
    public string? FullPath { get; set; }

    /// <summary>
    /// Gets or sets the category.
    /// </summary>
    /// <value>
    /// The category.
    /// </value>
    public string? Category { get; set; }

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    /// <value>
    /// The title.
    /// </value>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the icon class.
    /// </summary>
    /// <value>
    /// The icon class.
    /// </value>
    public string? IconClass { get; set; }

    /// <summary>
    /// Gets or sets the item class.
    /// </summary>
    /// <value>
    /// The item class.
    /// </value>
    public string? ItemClass { get; set; }

    /// <summary>
    /// Gets or sets the permission.
    /// </summary>
    /// <value>
    /// The permission.
    /// </value>
    public string? Permission { get; set; }

    /// <summary>
    /// Gets or sets the target window.
    /// </summary>
    /// <value>
    /// The target window, e.g. _blank.
    /// </value>
    public string? Target { get; set; }
}