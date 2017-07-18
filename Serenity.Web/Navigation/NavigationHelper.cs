namespace Serenity.Navigation
{
    using Serenity.Extensibility;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class NavigationHelper
    {
        public static List<NavigationItem> GetNavigationItems(Func<string, string> resolveUrl = null,
            Func<NavigationItemAttribute, bool> filter = null)
        {
            var menuItems = GetNavigationItemAttributes(filter);
            return ConvertToNavigationItems(menuItems, resolveUrl);
        }

        public static List<NavigationItem> ConvertToNavigationItems(ILookup<string, NavigationItemAttribute> attrByCategory,
            Func<string, string> resolveUrl)
        {
            var result = new List<NavigationItem>();

            Action<List<NavigationItem>, NavigationItemAttribute> processAttr = null;
            processAttr = (parent, attr) =>
            {
                var item = new NavigationItem
                {
                    Title = (attr.Title ?? "").Replace("//", "/"),
                    FullPath = attr.FullPath,
                    Url = (!string.IsNullOrEmpty(attr.Url) && resolveUrl != null) ? resolveUrl(attr.Url) : attr.Url,
                    IconClass = attr.IconClass.TrimToNull(),
                    ItemClass = attr.ItemClass.TrimToNull(),
                    Target = attr.Target.TrimToNull()
                };

                bool isAuthorizedSection = !attr.Url.IsEmptyOrNull() &&
                    (attr.Permission.IsEmptyOrNull() || Authorization.HasPermission(attr.Permission));

                var path = (attr.Category.IsEmptyOrNull() ? "" : (attr.Category + "/"));
                path += (attr.Title ?? "");

                var children = attrByCategory[path];
                foreach (var child in children)
                    processAttr(item.Children, child);

                if (item.Children.Count > 0 || isAuthorizedSection)
                    parent.Add(item);
            };

            foreach (var menu in attrByCategory[""])
                processAttr(result, menu);

            return result;
        }

        private static ILookup<string, NavigationItemAttribute> GetNavigationItemAttributes(
            Func<NavigationItemAttribute, bool> filter)
        {
            return LocalCache.Get("NavigationHelper:NavigationItems", TimeSpan.Zero, () =>
            {
                var list = new List<NavigationItemAttribute>();

                foreach (var assembly in ExtensibilityHelper.SelfAssemblies)
                {
                    foreach (NavigationItemAttribute attr in assembly.GetCustomAttributes<NavigationItemAttribute>())
                    {
                        if (filter == null || filter(attr))
                            list.Add(attr);
                    }
                }

                foreach (var navItemType in ExtensibilityHelper.GetTypesWithInterface(typeof(INavigationItemSource)))
                {
                    var navItem = (INavigationItemSource)Activator.CreateInstance(navItemType);
                    foreach (var item in navItem.GetItems())
                    {
                        if (filter == null || filter(item))
                            list.Add(item);
                    }
                }

                return ByCategory(list);
            });
        }

        public static ILookup<string, NavigationItemAttribute> ByCategory(IEnumerable<NavigationItemAttribute> list)
        {
            var result = list.OrderBy(x => x.Category ?? "")
                .ThenBy(x => x.Order)
                .ToLookup(x => x.Category ?? "");

            var missing = new Dictionary<string, NavigationItemAttribute>();
            foreach (var group in result)
            {
                string path = group.Key;
                while (!string.IsNullOrEmpty(path) && !missing.ContainsKey(path))
                {
                    var idx = path.Replace("//", "\x1\x1").LastIndexOf('/');
                    string parent;
                    string title;
                    if (idx < 0)
                    {
                        parent = "";
                        title = path;
                    }
                    else
                    {
                        parent = path.Substring(0, idx);
                        title = path.Substring(idx + 1);
                    }

                    if (!result[parent].Any(x => x.Title == title))
                        missing.Add(path, new NavigationMenuAttribute(group.Min(x => x.Order), path));

                    path = parent;
                }
            }

            if (missing.Count > 0)
                return list.Concat(missing.Values)
                    .OrderBy(x => x.Category ?? "")
                    .ThenBy(x => x.Order)
                    .ToLookup(x => x.Category ?? "");

            return result;
        }
    }
}