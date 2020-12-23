using Serenity.Web;
using System;
using System.Linq;
using System.Reflection;
using System.Globalization;
#if !ASPNETMVC
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace Serenity.Navigation
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public abstract class NavigationItemAttribute : Attribute
    {
        protected NavigationItemAttribute(int order, string path, string url, object permission, string icon)
        {
            path = (path ?? "");
            FullPath = path;

            var idx = path.LastIndexOf('/');

            if (idx > 0 && path[idx - 1] == '/')
                idx = path.Replace("//", "\x1\x1", StringComparison.Ordinal).LastIndexOf('/');

            if (idx >= 0)
            {
                Category = path.Substring(0, idx);
                Title = path.Substring(idx + 1);
            }
            else
            { 
                Title = path;
            }

            Order = order;
            Permission = permission == null ? null : permission.ToString();
            IconClass = icon;
            Url = url;
        }

        protected NavigationItemAttribute(int order, string path, Type controller, string icon, string action)
            : this(order, path, GetUrlFromController(controller, action), GetPermissionFromController(controller, action), icon)
        {
        }

        public static string GetUrlFromController(Type controller, string action)
        {
            if (controller == null)
                throw new ArgumentNullException("controller");

            if (action.IsEmptyOrNull())
                throw new ArgumentNullException("action");

            var actionMethod = controller.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                .Where(x => x.Name == action)
                .FirstOrDefault(x => x.GetAttribute<NonActionAttribute>() == null);

            if (actionMethod == null)
                throw new ArgumentOutOfRangeException("action",
                    string.Format(CultureInfo.CurrentCulture, "Controller {1} doesn't have an action with name {0}!",
                        action, controller.FullName));

            var routeController = controller.GetCustomAttributes<RouteAttribute>().FirstOrDefault();
            var routeAction = actionMethod.GetCustomAttributes<RouteAttribute>().FirstOrDefault();

            if (routeController == null && routeAction == null)
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    "Route attribute for {0} action of {1} controller is not found!",
                        action, controller.FullName));

            string url = (routeAction ?? routeController).Template ?? "";

#if !ASPNETMVC
            if (routeAction != null && !url.StartsWith("~/", StringComparison.Ordinal) && !url.StartsWith("/", StringComparison.Ordinal) && routeController != null)
            {
                var tmp = routeController.Template ?? "";
                if (url.Length > 0 && tmp.Length > 0 && tmp[tmp.Length - 1] != '/')
                    tmp += "/";

                url = tmp + url;
            }

            const string ControllerSuffix = "Controller";
            var controllerName = controller.Name;
            if (controllerName.EndsWith(ControllerSuffix, StringComparison.Ordinal))
                controllerName = controllerName.Substring(0, controllerName.Length - ControllerSuffix.Length);
            url = url.Replace("[controller]", controllerName, StringComparison.Ordinal);
            url = url.Replace("[action]", action, StringComparison.Ordinal);
#else
            if (!url.StartsWith("~/"))
            {

                var routePrefix = controller.GetCustomAttribute<RoutePrefixAttribute>();
                if (routePrefix != null)
                    url = UriHelper.Combine(routePrefix.Prefix, url);
            }

            var act = "{action=";
            var act1 = url.IndexOf(act, StringComparison.OrdinalIgnoreCase);
            if (act1 >= 0)
            {
                var act2 = url.IndexOf("}", act1 + 1);
                if (act2 >= 0)
                {
                    var defaultAction = url.Substring(act1 + act.Length, act2 - act1 - act.Length);
                    bool isDefaultAction = String.Compare(defaultAction, action, StringComparison.OrdinalIgnoreCase) == 0;
                    bool startsWithSlash = act1 > 0 && url[act1 - 1] == '/';
                    url = url.Substring(0, act1) +  
                        (startsWithSlash ? "" : "/") + 
                        (isDefaultAction ? "" : action) + 
                        url.Substring(act2 + 1);

                    if (url.Length > 2 && url.EndsWith("/"))
                        url = url.Substring(0, url.Length - 1);
                }
            }
#endif

            if (!url.StartsWith("~/", StringComparison.Ordinal) && !url.StartsWith("/", StringComparison.Ordinal))
                url = "~/" + url;

            while (true)
            {
                var idx1 = url.IndexOf('{', StringComparison.Ordinal);
                if (idx1 <= 0)
                    break;

                var idx2 = url.IndexOf("}", idx1 + 1, StringComparison.Ordinal);
                if (idx2 <= 0)
                    break;

                url = url.Substring(0, idx1) + url.Substring(idx2 + 1);
            }

            return url;
        }

        public static string GetPermissionFromController(Type controller, string action)
        {
            if (controller == null)
                throw new ArgumentNullException("controller");

            if (action.IsEmptyOrNull())
                throw new ArgumentNullException("action");

            var actionMethod = controller.GetMethod(action, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (actionMethod == null)
                throw new ArgumentOutOfRangeException("action");

            var pageAuthorize = actionMethod.GetCustomAttribute<PageAuthorizeAttribute>() ?? controller.GetCustomAttribute<PageAuthorizeAttribute>();
            if (pageAuthorize != null)
                return pageAuthorize.Permission;

            return null;
        }

        public int Order { get; set; }
        public string Url { get; set; }
        public string FullPath { get; set; }
        public string Category { get; set; }
        public string Title { get; set; }
        public string IconClass { get; set; }
        public string ItemClass { get; set; }
        public string Permission { get; set; }
        public string Target { get; set; }
    }
}