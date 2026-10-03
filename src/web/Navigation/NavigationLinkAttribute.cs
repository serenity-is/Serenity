using Serenity.Web;

namespace Serenity.Navigation;

/// <summary>
/// A navigation item with a link.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class NavigationLinkAttribute : NavigationItemAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationLinkAttribute"/> class.
    /// </summary>
    /// <param name="order">The order.</param>
    /// <param name="path">The path.</param>
    /// <param name="url">The URL.</param>
    /// <param name="permission">The permission.</param>
    /// <param name="icon">The icon.</param>
    public NavigationLinkAttribute(int order, string path, string url, object? permission, string? icon = null)
        : base(order, path, url, permission, icon)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationLinkAttribute"/> class.
    /// </summary>
    /// <param name="order">The order.</param>
    /// <param name="path">The path.</param>
    /// <param name="controller">The controller to get the URL and action from.</param>
    /// <param name="icon">The icon.</param>
    /// <param name="action">The action name.</param>
    public NavigationLinkAttribute(int order, string path, Type controller, string? icon = null, string action = "Index")
        : this(order, path, GetUrlFromController(controller, action)!,
              GetPermissionFromController(controller, action), icon)
    {
        if (GetFeaturesFromController(controller, action, out var requireAny) is string[] features)
        {
            RequireFeatures = features;
            RequireAnyFeature = requireAny;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationLinkAttribute"/> class.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="url">The URL.</param>
    /// <param name="permission">The permission.</param>
    /// <param name="icon">The icon.</param>
    public NavigationLinkAttribute(string path, string url, object? permission, string? icon = null)
        : base(int.MaxValue, path, url, permission, icon)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationLinkAttribute"/> class.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="controller">The controller to get the URL and action from.</param>
    /// <param name="icon">The icon.</param>
    /// <param name="action">The action name.</param>
    public NavigationLinkAttribute(string path, Type controller, string? icon = null, string action = "Index")
        : base(int.MaxValue, path, GetUrlFromController(controller, action)!,
            GetPermissionFromController(controller, action), icon)
    {
        if (GetFeaturesFromController(controller, action, out var requireAny) is string[] features)
        {
            RequireFeatures = features;
            RequireAnyFeature = requireAny;
        }
    }

    /// <summary>
    /// Tries to extract the permission from a controller action.
    /// </summary>
    /// <param name="controller">The controller.</param>
    /// <param name="action">The action.</param>
    /// <returns>The permission key, or <c>null</c> if none is found.</returns>
    /// <exception cref="ArgumentNullException">Controller or action is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The action name is invalid.</exception>
    public static string? GetPermissionFromController(Type controller, string action)
    {
        ArgumentNullException.ThrowIfNull(controller);

        if (string.IsNullOrEmpty(action))
            throw new ArgumentNullException(nameof(action));

        var actionMethod = controller.GetMethod(action, BindingFlags.Public | BindingFlags.Instance) 
            ?? throw new ArgumentOutOfRangeException(nameof(action));
        var pageAuthorize = actionMethod.GetCustomAttribute<PageAuthorizeAttribute>() 
            ?? controller.GetCustomAttribute<PageAuthorizeAttribute>();
        return pageAuthorize?.Permission;
    }

    /// <summary>
    /// Tries to extract features from a controller action.
    /// </summary>
    /// <param name="controller">The controller.</param>
    /// <param name="action">The action.</param>
    /// <param name="requireAny">Whether any of the features are required.</param>
    /// <returns>The list of required features, or <c>null</c> if none is found.</returns>
    /// <exception cref="ArgumentNullException">Controller or action is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The action name is invalid.</exception>
    public static string[]? GetFeaturesFromController(Type controller, string action, out bool requireAny)
    {
        ArgumentNullException.ThrowIfNull(controller);

        if (string.IsNullOrEmpty(action))
            throw new ArgumentNullException(nameof(action));

        requireAny = false;
        var actionMethod = controller.GetMethod(action, BindingFlags.Public | BindingFlags.Instance)
            ?? throw new ArgumentOutOfRangeException(nameof(action));
        var barrier = actionMethod.GetCustomAttribute<FeatureBarrierAttribute>()
            ?? controller.GetCustomAttribute<FeatureBarrierAttribute>();

        if (barrier != null && barrier.Features?.Any() == true)
        {
            requireAny = barrier.RequireAny;
            return [.. barrier.Features];
        }

        return null;
    }
}