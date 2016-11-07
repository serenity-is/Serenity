using System;
using System.Collections.Generic;

namespace Serenity.Navigation
{    
    public interface IDynamicNavItems
    {
        List<NavItem> NavItems { get; }
    }

    public enum NavItemType
    {
        Menu,
        Link,
        Action
    }

    public class NavItem
    {
        public NavItem(int order, string title, string icon = null)
        {
            Type = NavItemType.Menu;
            Order = order;
            Title = title;
            Icon = icon;
        }
        public NavItem(int order, string path, string url, object permission, string icon = null)
        {
            Type = NavItemType.Link;
            Order = order;
            Path = path;
            Url = url;
            Permission = permission;
            Icon = icon;
        }
        public NavItem(int order, string path, Type controller, string icon = null, string action = "Index")
        {
            Type = NavItemType.Action;
            Order = order;
            Path = path;
            Controller = controller;
            Action = action;
            Icon = icon;
        }
        public NavItemType Type { get; private set; }
        public int Order { get; private set; }
        public string Title { get; private set; }
        public string Icon { get; private set; }
        public string Path { get; private set; }
        public string Url { get; private set; }
        public object Permission { get; private set; }
        public Type Controller { get; private set; }
        public string Action { get; private set; }        
    }
}