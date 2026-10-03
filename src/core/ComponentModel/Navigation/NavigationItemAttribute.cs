namespace Serenity.Navigation;

/// <summary>
/// Navigation item attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public abstract class NavigationItemAttribute : Attribute
{
    private const string RouteAttributeName = "RouteAttribute";

    /// <summary>
    /// Creates a new instance of the attribute.
    /// </summary>
    /// <param name="order">Order.</param>
    /// <param name="path">Path.</param>
    /// <param name="url">URL.</param>
    /// <param name="permission">Permission.</param>
    /// <param name="icon">Icon class.</param>
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
    /// Tries to extract the URL from a controller action.
    /// </summary>
    /// <param name="controller">The controller.</param>
    /// <param name="action">The action name.</param>
    /// <param name="throwIfAbsent">Whether to throw if the action is not found.</param>
    /// <returns>The resolved URL, or <c>null</c> if the action is not found and <paramref name="throwIfAbsent"/> is <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">Controller or action is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The action name is invalid.</exception>
    /// <exception cref="InvalidOperationException">The route attribute is not found.</exception>
    public static string? GetUrlFromController(Type controller, string action, bool throwIfAbsent = true)
    {
        if (controller is null)
            throw new ArgumentNullException(nameof(controller));

        if (string.IsNullOrEmpty(action))
            throw new ArgumentNullException(nameof(action));

        var actionMethod = GetActionMethod(controller, action, throwIfAbsent);
        if (actionMethod is null)
            return null;

        var routeController = GetRouteAttribute(controller);
        var routeAction = GetRouteAttribute(actionMethod);

        if (routeController == null && routeAction == null)
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                "Route attribute for {0} action of {1} controller is not found!",
                    action, controller.FullName));

        string url = GetRouteTemplate(routeAction ?? routeController!) ?? "";

        static bool isRooted(string value)
        {
            return value.StartsWith("~/", StringComparison.Ordinal) ||
                value.StartsWith("/", StringComparison.Ordinal);
        }

        if (routeAction != null &&
            routeController != null &&
            !isRooted(url))
        {
            var tmp = GetRouteTemplate(routeController) ?? "";
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

        if (!url.StartsWith("~/", StringComparison.Ordinal))
            url = url.StartsWith("/", StringComparison.Ordinal) ?
                "~" + url : "~/" + url;

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

    internal static bool HasComplexRouteTemplate(Type controller, string action)
    {
        if (controller is null)
            throw new ArgumentNullException(nameof(controller));

        if (string.IsNullOrEmpty(action))
            throw new ArgumentNullException(nameof(action));

        var actionMethod = GetActionMethod(controller, action)!;
        var routeController = GetRouteAttribute(controller);
        var routeAction = GetRouteAttribute(actionMethod);

        if (routeController is null && routeAction is null)
            return true;

        if (routeAction is not null && IsComplexRouteTemplate(GetRouteTemplate(routeAction)))
            return true;

        if (routeAction is not null &&
            IsRootedRoute(GetRouteTemplate(routeAction)))
            return false;

        return routeController is not null &&
            IsComplexRouteTemplate(GetRouteTemplate(routeController));
    }

    internal static bool HasActionRoute(Type controller, string action)
    {
        if (controller is null)
            throw new ArgumentNullException(nameof(controller));

        if (string.IsNullOrEmpty(action))
            throw new ArgumentNullException(nameof(action));

        return GetRouteAttribute(GetActionMethod(controller, action)!) is not null;
    }

    internal static bool HasRouteForAction(Type controller, string action)
    {
        if (controller is null)
            throw new ArgumentNullException(nameof(controller));

        if (string.IsNullOrEmpty(action))
            throw new ArgumentNullException(nameof(action));

        var actionMethod = GetActionMethod(controller, action, throwIfAbsent: false);
        return actionMethod is not null &&
            (GetRouteAttribute(actionMethod) is not null || GetRouteAttribute(controller) is not null);
    }

    private static MethodInfo? GetActionMethod(Type controller, string action, bool throwIfAbsent = true)
    {
        var actionMethod = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(method => method.Name == action)
            .FirstOrDefault(method => !HasMvcAttribute(method, "NonActionAttribute"));

        if (actionMethod is not null || !throwIfAbsent)
            return actionMethod;

        throw new ArgumentOutOfRangeException(nameof(action),
            string.Format(CultureInfo.CurrentCulture,
                "Controller {1} doesn't have an action with name {0}!",
                action, controller.FullName));
    }

    private static CustomAttributeData? GetRouteAttribute(MemberInfo member)
    {
        var routeAttribute = member.GetCustomAttributesData()
            .FirstOrDefault(attribute => HasMvcAttribute(attribute, RouteAttributeName));
        if (routeAttribute is not null)
            return routeAttribute;

        if (member is Type type)
        {
            for (var baseType = type.BaseType; baseType is not null; baseType = baseType.BaseType)
            {
                routeAttribute = baseType.GetCustomAttributesData()
                    .FirstOrDefault(attribute => HasMvcAttribute(attribute, RouteAttributeName));
                if (routeAttribute is not null)
                    return routeAttribute;
            }
        }
        else if (member is MethodInfo method)
        {
            var baseMethod = method.GetBaseDefinition();
            if (baseMethod != method)
                return GetRouteAttribute(baseMethod);
        }

        return null;
    }

    private static string? GetRouteTemplate(CustomAttributeData? routeAttribute)
    {
        if (routeAttribute is null)
            return null;

        var template = routeAttribute.ConstructorArguments
            .FirstOrDefault(argument => argument.ArgumentType == typeof(string)).Value as string;
        template ??= routeAttribute.NamedArguments
            .FirstOrDefault(argument => argument.MemberName is "Template" or "Url").TypedValue.Value as string;

        return template;
    }

    private static bool HasMvcAttribute(MemberInfo member, string attributeName) =>
        member.GetCustomAttributesData().Any(attribute => HasMvcAttribute(attribute, attributeName));

    private static bool HasMvcAttribute(CustomAttributeData attribute, string attributeName) =>
        attribute.AttributeType.Name == attributeName &&
        attribute.AttributeType.Namespace is "Microsoft.AspNetCore.Mvc" or "System.Web.Mvc";

    private static bool IsRootedRoute(string? route) =>
        route is not null &&
        (route.StartsWith("~/", StringComparison.Ordinal) ||
            route.StartsWith("/", StringComparison.Ordinal));

    private static bool IsComplexRouteTemplate(string? route)
    {
        if (route is null)
            return false;

        route = route.Replace("[action]", "", StringComparison.Ordinal);
        return route.IndexOfAny(['{', '}', '?', '#', '*', ':', '[', ']']) >= 0;
    }

    /// <summary>
    /// Gets or sets the order (only) among its siblings.
    /// </summary>
    public decimal Order { get; set; }

    /// <summary>
    /// URL of this navigation item, should be null for a menu.
    /// </summary>
    public string? Url { get; set; }
    
    /// <summary>
    /// The full path to a navigation item like A/B/C.
    /// This is used to generate the local text key for this item
    /// like Navigation.A/B/C.
    /// </summary>
    public string? FullPath { get; set; }

    /// <summary>
    /// This is the full path of its parent, e.g. A/B for A/B/C.
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Title of the navigation item. It is the part after the last slash,
    /// e.g. C for A/B/C.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Icon class.
    /// </summary>
    public string? IconClass { get; set; }

    /// <summary>
    /// Extra CSS class to apply to its navigation element, e.g. LI.
    /// </summary>
    public string? ItemClass { get; set; }
    
    /// <summary>
    /// Permission required to view this navigation item.
    /// </summary>
    public string? Permission { get; set; }

    /// <summary>
    /// Window target to open this link, e.g. _blank etc.
    /// </summary>
    public string? Target { get; set; }

    /// <summary>
    /// The set of feature toggles that this navigation item depends on.
    /// </summary>
    public string[]? RequireFeatures { get; set; }

    /// <summary>
    /// True to require any of the RequireFeatures to be enabled in order to pass.
    /// </summary>
    public bool RequireAnyFeature { get; set; }
}