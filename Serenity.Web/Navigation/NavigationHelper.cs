namespace Serenity.Navigation
{
    using Serenity.Extensibility;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Hosting;

    public class NavigationHelper
    {
        public static List<NavigationItem> GetNavigationItems(Func<string, string> resolveUrl = null,
            Func<NavigationItemAttribute, bool> filter = null)
        {
            var result = new List<NavigationItem>();
            var menuItems = GetNavigationItemAttributes(filter);
            var remaining = new HashSet<string>();
            foreach (var item in menuItems)
                remaining.Add(item.Key);

            Action<List<NavigationItem>, NavigationItemAttribute> processMenu = null;
            processMenu = (parent, menu) =>
            {
                var path = (menu.Category.IsEmptyOrNull() ? "" : (menu.Category + "/"));
                path += (menu.Title.TrimToNull() ?? "");
                remaining.Remove(path);

                var section = new NavigationItem
                {
                    Title = menu.Title,
                    Url = (!string.IsNullOrEmpty(menu.Url) && resolveUrl != null) ? resolveUrl(menu.Url) : menu.Url,
                    IconClass = menu.IconClass.TrimToNull(),
                    Target = menu.Target.TrimToNull()
                };

                bool isAuthorizedSection = !menu.Url.IsEmptyOrNull() &&
                    (menu.Permission.IsEmptyOrNull() || Authorization.HasPermission(menu.Permission));

                var children = menuItems[path];
                foreach (var child in children)
                    processMenu(section.Children, child);

                if (section.Children.Count > 0 || isAuthorizedSection)
                    parent.Add(section);
            };

            remaining.Remove("");
            foreach (var menu in menuItems[""])
                processMenu(result, menu);

            while (remaining.Count > 0)
            {
                var first = remaining.First();
                remaining.Remove(first);
                var menu = new NavigationMenuAttribute(Int32.MaxValue, first);
                processMenu(result, menu);
            }

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
                    foreach (NavigationItemAttribute attr in assembly.GetCustomAttributes(typeof(NavigationItemAttribute), false))
                    {
                        if (filter == null || filter(attr))
                            list.Add(attr);
                    }
                }

                return list.OrderBy(x => (x.Category.TrimToNull() ?? ""))
                    .ThenBy(x => x.Order)
                    .ToLookup(x => (x.Category.TrimToNull() ?? ""));
            });
        }
    }
}