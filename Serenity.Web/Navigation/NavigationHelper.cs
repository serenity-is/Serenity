namespace Serenity.Navigation
{
    using Serenity.Extensibility;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Hosting;

    public class NavigationHelper
    {
        public static List<NavigationItem> GetNavigationItems(Func<string, string> resolveUrl = null)
        {
            var result = new List<NavigationItem>();
            var menuItems = GetNavigationItemAttributes();
            var remaining = new HashSet<string>();
            foreach (var item in menuItems)
                remaining.Add(item.Key);

            Action<NavigationItemAttribute> processMenu = menu =>
            {
                remaining.Remove(menu.Title.TrimToNull());

                var section = new NavigationItem
                {
                    Title = menu.Title,
                    Url = (!string.IsNullOrEmpty(menu.Url) && resolveUrl != null) ? resolveUrl(menu.Url) : menu.Url,
                    IconClass = menu.IconClass.TrimToNull(),
                    Target = menu.Target.TrimToNull()
                };

                bool isAuthorizedSection = !menu.Url.IsEmptyOrNull() &&
                    (menu.Permission.IsEmptyOrNull() || Authorization.HasPermission(menu.Permission));

                var children = menuItems[menu.Title.TrimToNull() ?? "???"];
                foreach (var child in children)
                {
                    if (child.Url.IsEmptyOrNull())
                        continue;

                    if (child.Permission != null &&
                        !Authorization.HasPermission(child.Permission))
                        continue;

                    section.Children.Add(new NavigationItem
                    {
                        Title = child.Title,
                        Url = (!string.IsNullOrEmpty(child.Url) && resolveUrl != null) ? resolveUrl(child.Url) : child.Url,
                        IconClass = child.IconClass.TrimToNull(),
                        Target = child.Target.TrimToNull()
                    });
                }

                if (section.Children.Count > 0 || isAuthorizedSection)
                {
                    result.Add(section);
                }
            };

            foreach (var menu in menuItems[null])
                processMenu(menu);

            while (remaining.Count > 0)
            {
                var first = remaining.First();
                remaining.Remove(first);
                var menu = new NavigationMenuAttribute(Int32.MaxValue, first);
                processMenu(menu);
            }

            return result;
        }

        private static ILookup<string, NavigationItemAttribute> GetNavigationItemAttributes()
        {
            return LocalCache.Get("NavigationHelper:NavigationItems", TimeSpan.Zero, () =>
            {
                var list = new List<NavigationItemAttribute>();

                foreach (var assembly in ExtensibilityHelper.SelfAssemblies)
                {
                    foreach (NavigationItemAttribute attr in assembly.GetCustomAttributes(typeof(NavigationItemAttribute), false))
                    {
                        list.Add(attr);
                    }
                }

                return list.OrderBy(x => x.Category)
                    .ThenBy(x => x.Order)
                    .ToLookup(x => x.Category.TrimToNull());
            });
        }
    }
}