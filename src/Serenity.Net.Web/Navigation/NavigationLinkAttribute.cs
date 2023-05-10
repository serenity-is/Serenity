using Microsoft.AspNetCore.Mvc;
using Serenity.Web;

namespace Serenity.Navigation;

/// <summary>
/// A navigation item with a link
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class NavigationLinkAttribute : NavigationItemAttribute
{
    /// <summary>
    /// Creates a new instance of the attribute
    /// </summary>
    /// <param name="order">Order</param>
    /// <param name="path">Path</param>
    /// <param name="url">URL</param>
    /// <param name="permission">Permission</param>
    /// <param name="icon">Icon</param>
    public NavigationLinkAttribute(int order, string path, string url, object permission, string icon = null)
        : base(order, path, url, permission, icon)
    {
    }

    /// <summary>
    /// Creates a new instance of the attribute
    /// </summary>
    /// <param name="order">Order</param>
    /// <param name="path">Path</param>
    /// <param name="controller">Controller to get URL and action from</param>
    /// <param name="icon">Icon</param>
    /// <param name="action">Action name</param>
    public NavigationLinkAttribute(int order, string path, Type controller, string icon = null, string action = "Index")
        : this(order, path, GetUrlFromController(controller, action), 
              GetPermissionFromController(controller, action), icon)
    {
    }

    /// <summary>
    /// Creates a new instance of the attribute
    /// </summary>
    /// <param name="path">Path</param>
    /// <param name="url">URL</param>
    /// <param name="permission">Permission</param>
    /// <param name="icon">Icon</param>
    public NavigationLinkAttribute(string path, string url, object permission, string icon = null)
        : base(int.MaxValue, path, url, permission, icon)
    {
    }

    /// <summary>
    /// Creates a new instance of the attribute
    /// </summary>
    /// <param name="path">Path</param>
    /// <param name="controller">Controller to get URL and action from</param>
    /// <param name="icon">Icon</param>
    /// <param name="action">Action name</param>
    public NavigationLinkAttribute(string path, Type controller, string icon = null, string action = "Index")
        : base(int.MaxValue, path, GetUrlFromController(controller, action), 
            GetPermissionFromController(controller, action), icon)
    {
    }

    /// <summary>
    /// Tries to extract URL from a controller action
    /// </summary>
    /// <param name="controller">Controller</param>
    /// <param name="action">Action name</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">Controll or action is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">Action name is invalid</exception>
    /// <exception cref="InvalidOperationException">Route attribute is not found</exception>
    public static string GetUrlFromController(Type controller, string action)
    {
        if (controller == null)
            throw new ArgumentNullException(nameof(controller));

        if (action.IsEmptyOrNull())
            throw new ArgumentNullException(nameof(action));

        var actionMethod = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(x => x.Name == action)
            .FirstOrDefault(x => x.GetAttribute<NonActionAttribute>() == null);

        if (actionMethod == null)
            throw new ArgumentOutOfRangeException(nameof(action),
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
                url.StartsWith("/", StringComparison.Ordinal);
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

            var idx2 = url.IndexOf("}", idx1 + 1, StringComparison.Ordinal);
            if (idx2 <= 0)
                break;

            url = url[..idx1] + url[(idx2 + 1)..];
        }

        return url;
    }

    /// <summary>
    /// Tries to extract permission from a controller action
    /// </summary>
    /// <param name="controller">Controller</param>
    /// <param name="action">Action</param>
    /// <exception cref="ArgumentNullException">Controller or action is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">Action name is invalid</exception>
    public static string GetPermissionFromController(Type controller, string action)
    {
        if (controller == null)
            throw new ArgumentNullException(nameof(controller));

        if (action.IsEmptyOrNull())
            throw new ArgumentNullException(nameof(action));

        var actionMethod = controller.GetMethod(action, BindingFlags.Public | BindingFlags.Instance) 
            ?? throw new ArgumentOutOfRangeException(nameof(action));
        var pageAuthorize = actionMethod.GetCustomAttribute<PageAuthorizeAttribute>() 
            ?? controller.GetCustomAttribute<PageAuthorizeAttribute>();
        return pageAuthorize?.Permission;
    }
}