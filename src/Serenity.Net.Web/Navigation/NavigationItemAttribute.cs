using Microsoft.AspNetCore.Mvc;
using Serenity.Web;

namespace Serenity.Navigation
{
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
        protected NavigationItemAttribute(int order, string path, string url, object permission, string icon)
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
        /// Creates a new instance of the attribute
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="path">Path</param>
        /// <param name="controller">Controller to get URL and the permission from</param>
        /// <param name="icon">Icon class</param>
        /// <param name="action">The action name</param>
        protected NavigationItemAttribute(int order, string path, Type controller, string icon, string action)
            : this(order, path, GetUrlFromController(controller, action), GetPermissionFromController(controller, action), icon)
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

            var actionMethod = controller.GetMethod(action, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (actionMethod == null)
                throw new ArgumentOutOfRangeException(nameof(action));

            var pageAuthorize = actionMethod.GetCustomAttribute<PageAuthorizeAttribute>() ?? controller.GetCustomAttribute<PageAuthorizeAttribute>();
            if (pageAuthorize != null)
                return pageAuthorize.Permission;

            return null;
        }

        /// <summary>
        /// Gets / sets the order (only) among its siblings.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Url of this navigation item, should be null for menu
        /// </summary>
        public string Url { get; set; }
        
        /// <summary>
        /// The full path to navigation item like A/B/C
        /// This is used to generate local text key for this item
        /// like Navigation.A/B/C
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// This is full path of its parent, e.g. A/B for A/B/C
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Title of the navigation item. It is the part after last slash,
        /// e.g. C for A/B/C
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Icon class
        /// </summary>
        public string IconClass { get; set; }

        /// <summary>
        /// Extra css class to apply to its navigation element e.g. LI
        /// </summary>
        public string ItemClass { get; set; }
        
        /// <summary>
        /// Permission required to view this navigation item
        /// </summary>
        public string Permission { get; set; }

        /// <summary>
        /// Window target to open this link, e.g. _blank etc.
        /// </summary>
        public string Target { get; set; }
   }
}