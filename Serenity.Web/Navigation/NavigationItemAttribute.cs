﻿using Serenity.Web;
using System;
using System.Linq;
using System.Reflection;
using Serenity.CodeGeneration;
using Serenity.Web.Common;
#if ASPNETCORE
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace Serenity.Navigation
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public abstract class NavigationItemAttribute : Attribute
    {
        private string[] EmptyCategory = new string[0];

        protected NavigationItemAttribute(int order, string path, string url, object permission, string icon)
        {
            path = (path ?? "");
            this.FullPath = path;

            var idx = path.LastIndexOf('/');

            if (idx > 0 && path[idx - 1] == '/')
                idx = path.Replace("//", "\x1\x1").LastIndexOf('/');

            if (idx >= 0)
            {
                this.Category = path.Substring(0, idx);
                this.Title = path.Substring(idx + 1);
            }
            else
            { 
                this.Title = path;
            }

            this.Order = order;
            this.Permission = permission == null ? null : permission.ToString();
            this.IconClass = icon;
            this.Url = url;
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

            var actionMethod = controller.GetMethod(action, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (actionMethod == null)
                throw new ArgumentOutOfRangeException("action");

            var routeController = AttributeReader.GetAttributeWithAssemblyVersionChecking<RouteAttribute>(controller); 
            var routeAction = AttributeReader.GetAttributeWithAssemblyVersionChecking<RouteAttribute>(actionMethod);

            if (routeController == null && routeAction == null)
                throw new InvalidOperationException(String.Format(
                    "Route attribute for {0} action of {1} controller is not found!",
                        actionMethod, controller.FullName));

            string url = (routeAction ?? routeController).Template ?? "";

#if ASPNETCORE
            if (routeAction != null && !url.StartsWith("~/") && !url.StartsWith("/") && routeController != null)
            {
                var tmp = routeController.Template ?? "";
                if (tmp.Length > 0 && tmp[tmp.Length - 1] != '/')
                    tmp += "/";

                url = tmp + url;
            }

            const string ControllerSuffix = "Controller";
            var controllerName = controller.Name;
            if (controllerName.EndsWith(ControllerSuffix))
                controllerName = controllerName.Substring(0, controllerName.Length - ControllerSuffix.Length);
            url = url.Replace("[controller]", controllerName);
            url = url.Replace("[action]", action);
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

            if (!url.StartsWith("~/") && !url.StartsWith("/"))
                url = "~/" + url;

            while (true)
            {
                var idx1 = url.IndexOf('{');
                if (idx1 <= 0)
                    break;

                var idx2 = url.IndexOf("}", idx1 + 1);
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