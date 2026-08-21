using Microsoft.AspNetCore.Mvc;
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
    public NavigationLinkAttribute(int order, string path, string url, object permission, string icon = null)
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
    public NavigationLinkAttribute(int order, string path, Type controller, string icon = null, string action = "Index")
        : this(order, path, GetUrlFromController(controller, action), 
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
    public NavigationLinkAttribute(string path, string url, object permission, string icon = null)
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
    public NavigationLinkAttribute(string path, Type controller, string icon = null, string action = "Index")
        : base(int.MaxValue, path, GetUrlFromController(controller, action), 
            GetPermissionFromController(controller, action), icon)
    {
        if (GetFeaturesFromController(controller, action, out var requireAny) is string[] features)
        {
            RequireFeatures = features;
            RequireAnyFeature = requireAny;
        }
    }

    /// <summary>
    /// Tries to extract the URL from a controller action.
    /// </summary>
    /// <param name="controller">The controller.</param>
    /// <param name="action">The action name.</param>
    /// <returns>The resolved URL.</returns>
    /// <exception cref="ArgumentNullException">Controller or action is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The action name is invalid.</exception>
    /// <exception cref="InvalidOperationException">The route attribute is not found.</exception>
    public static string GetUrlFromController(Type controller, string action)
    {
        ArgumentNullException.ThrowIfNull(controller);

        if (string.IsNullOrEmpty(action))
            throw new ArgumentNullException(nameof(action));

        var actionMethod = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(x => x.Name == action)
            .FirstOrDefault(x => x.GetCustomAttribute<NonActionAttribute>(inherit: false) == null) ?? throw new ArgumentOutOfRangeException(nameof(action),
                string.Format(CultureInfo.CurrentCulture,
                    "Controller {1} doesn't have an action with name {0}!",
                    action, controller.FullName));
        var routeController = controller.GetCustomAttributes<RouteAttribute>()
            .FirstOrDefault();
        var routeAction = actionMethod.GetCustomAttributes<RouteAttribute>()
            .FirstOrDefault();

        if (routeController == null && routeAction == null)
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                "Route attribute for {0} action of {1} controller is not found!",
                    action, controller.FullName));

        string url = (routeAction ?? routeController).Template ?? "";

        static bool isRooted(string url)
        {
            return url.StartsWith("~/", StringComparison.Ordinal) ||
                url.StartsWith('/');
        }

        if (routeAction != null &&
            routeController != null &&
            !isRooted(url))
        {
            var tmp = routeController.Template ?? "";
            if (url.Length > 0 && tmp.Length > 0 && tmp[^1] != '/')
                tmp += "/";

            url = tmp + url;
        }

        const string ControllerSuffix = "Controller";

        var controllerName = controller.Name;
        if (controllerName.EndsWith(ControllerSuffix, StringComparison.Ordinal))
            controllerName = controllerName[..^ControllerSuffix.Length];

        url = url.Replace("[controller]", controllerName, StringComparison.Ordinal);
        url = url.Replace("[action]", action, StringComparison.Ordinal);

        if (!isRooted(url))
            url = "~/" + url;

        while (true)
        {
            var idx1 = url.IndexOf('{', StringComparison.Ordinal);
            if (idx1 <= 0)
                break;

            var idx2 = url.IndexOf('}', idx1 + 1);
            if (idx2 <= 0)
                break;

            url = url[..idx1] + url[(idx2 + 1)..];
        }

        return url;
    }

    /// <summary>
    /// Tries to extract the permission from a controller action.
    /// </summary>
    /// <param name="controller">The controller.</param>
    /// <param name="action">The action.</param>
    /// <returns>The permission key, or <c>null</c> if none is found.</returns>
    /// <exception cref="ArgumentNullException">Controller or action is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The action name is invalid.</exception>
    public static string GetPermissionFromController(Type controller, string action)
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
    public static string[] GetFeaturesFromController(Type controller, string action, out bool requireAny)
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
            return barrier.Features.ToArray();
        }

        return null;
    }
}