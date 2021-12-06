using Microsoft.Extensions.DependencyInjection;
using Serenity.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Serenity.Navigation
{
    public class NavigationHelper
    {
        public static List<NavigationItem> GetNavigationItems(IPermissionService permissions, 
            ITypeSource typeSource, IServiceProvider serviceProvider, 
            Func<string, string> resolveUrl = null, 
            Func<NavigationItemAttribute, bool> filter = null)
        {
            var menuItems = GetNavigationItemAttributes(typeSource, serviceProvider, filter);
            return ConvertToNavigationItems(permissions, menuItems, resolveUrl);
        }

        public static List<NavigationItem> ConvertToNavigationItems(IPermissionService permissions, 
            ILookup<string, NavigationItemAttribute> attrByCategory, Func<string, string> resolveUrl)
        {
            if (permissions == null)
                throw new ArgumentNullException(nameof(permissions));

            var result = new List<NavigationItem>();

            void processAttr(NavigationItem parent, NavigationItemAttribute attr)
            {
                var item = new NavigationItem();

                bool isAuthorizedSection = !attr.Url.IsEmptyOrNull() &&
                    (attr.Permission.IsEmptyOrNull() || permissions.HasPermission(attr.Permission));

                var path = attr.Category.IsEmptyOrNull() ? "" : (attr.Category + "/");
                path += attr.Title ?? "";

                var children = attrByCategory[path];
                var target = parent?.Children ?? result;
                foreach (var child in children)
                    processAttr(item, child);

                if (item.Children.Count > 0 || isAuthorizedSection)
                {
                    item.Title = (attr.Title ?? "").Replace("//", "/", StringComparison.Ordinal);
                    item.FullPath = attr.FullPath;
                    item.Url = (!string.IsNullOrEmpty(attr.Url) && resolveUrl != null) ? resolveUrl(attr.Url) : attr.Url;
                    item.IconClass = attr.IconClass.TrimToNull();
                    item.ItemClass = attr.ItemClass.TrimToNull();
                    item.Target = attr.Target.TrimToNull();
                    item.Parent = parent;
                    target.Add(item);
                }
            }

            foreach (var menu in attrByCategory[""])
                processAttr(null, menu);

            return result;
        }

        private static ILookup<string, NavigationItemAttribute> GetNavigationItemAttributes(
            ITypeSource typeSource, IServiceProvider serviceProvider, 
            Func<NavigationItemAttribute, bool> filter)
        {
            var list = new List<NavigationItemAttribute>();

            foreach (NavigationItemAttribute attr in typeSource
                .GetAssemblyAttributes<NavigationItemAttribute>())
            {
                if (filter == null || filter(attr))
                    list.Add(attr);
            }

            foreach (var navItemType in typeSource.GetTypesWithInterface(typeof(INavigationItemSource))
                .Where(x => !x.IsAbstract && !x.IsInterface))
            {
                var navItem = (INavigationItemSource)ActivatorUtilities.CreateInstance(
                    serviceProvider, navItemType);
                foreach (var item in navItem.GetItems())
                {
                    if (filter == null || filter(item))
                        list.Add(item);
                }
            }

            return ByCategory(list);
        }

        public const string Rest = "...rest";

        private static IEnumerable<NavigationItemAttribute> Sort(IEnumerable<NavigationItemAttribute> list,
            Func<NavigationItemAttribute, string> getCategory)
        {
            var l = list.ToList();
            l.Sort((a, b) =>
            {
                var ac = getCategory(a) ?? "";
                var bc = getCategory(b) ?? "";
                var r = string.Compare(ac, bc, StringComparison.OrdinalIgnoreCase);
                if (r != 0)
                    return r;

                var abb = a.Before != null &&
                    a.Before.Any(x => x == "*" ||
                    string.Equals(x, b.Title, StringComparison.OrdinalIgnoreCase));

                var bba = b.Before != null &&
                    b.Before.Any(x => x == "*" ||
                    string.Equals(x, a.Title, StringComparison.OrdinalIgnoreCase));

                if (abb && !bba)
                    return -1;

                if (bba && !abb)
                    return 1;

                var aab = a.After != null &&
                    a.After.Any(x => x == "*" ||
                    string.Equals(x, b.Title, StringComparison.OrdinalIgnoreCase));

                var baa = b.After != null &&
                    b.After.Any(x => x == "*" ||
                    string.Equals(x, a.Title, StringComparison.OrdinalIgnoreCase));

                if (aab && !baa)
                    return 1;

                if (baa & !aab)
                    return -1;

                return a.Order.CompareTo(b.Order);
            });

            return l;
        }

        public static ILookup<string, NavigationItemAttribute> ByCategory(
            IEnumerable<NavigationItemAttribute> list)
        {
            if (list is null)
                throw new ArgumentNullException(nameof(list));

            var byCategory = Sort(list, x => x.Category)
                .ToLookup(x => x.Category ?? "", StringComparer.OrdinalIgnoreCase);

            var missing = new Dictionary<string, NavigationItemAttribute>();
            foreach (var group in byCategory)
            {
                string path = group.Key;
                while (!string.IsNullOrEmpty(path) && !missing.ContainsKey(path))
                {
                    var idx = path.Replace("//", "\x1\x1", StringComparison.Ordinal).LastIndexOf('/');
                    string parent;
                    string title;
                    if (idx < 0)
                    {
                        parent = "";
                        title = path;
                    }
                    else
                    {
                        parent = path[..idx];
                        title = path[(idx + 1)..];
                    }

                    if (!byCategory[parent].Any(x => x.Title == title))
                        missing.Add(path, new NavigationMenuAttribute(group.Min(x => x.Order), path));

                    path = parent;
                }
            }

            if (missing.Count > 0)
            {
                list = list.Concat(missing.Values);
                byCategory = Sort(list, x => x.Category)
                    .ToLookup(x => x.Category ?? "", StringComparer.OrdinalIgnoreCase);
            }

            var withIncludes = list.Where(x => x.Include != null);
            if (!withIncludes.Any())
                return byCategory;

            var newCategory = new Dictionary<NavigationItemAttribute, string>();

            foreach (var wi in withIncludes.Where(x => !x.Include.Any(l => l == Rest)))
            {
                foreach (var pattern in wi.Include)
                {
                    if (string.IsNullOrEmpty(pattern))
                        continue;

                    if (pattern.EndsWith('/'))
                    {
                        foreach (var child in byCategory[pattern[..^1]]
                            .Where(x => x != wi && !newCategory.ContainsKey(x)))
                        {
                            newCategory[child] = wi.FullPath;
                        }
                    }
                    else
                    {
                        var idx = pattern.LastIndexOf('/');
                        var search = idx < 0 ? pattern : pattern[(idx + 1)..];
                        var items = (idx < 0 ? byCategory[""] : byCategory[pattern.Substring(0, idx)])
                            .Where(x => string.Equals(x.FullPath, pattern, StringComparison.OrdinalIgnoreCase));
                            
                        foreach (var item in items.Where(x => 
                            x != wi && !newCategory.ContainsKey(x)))
                        {
                            newCategory[item] = wi.FullPath;
                        }
                    }
                }
            }

            foreach (var wr in withIncludes.Where(x => x.Include.Any(l => l == Rest)))
            {
                foreach (var item in byCategory[""])
                {
                    if (wr == item ||
                        newCategory.ContainsKey(item))
                        continue;

                    var isMatch = true;
                    foreach (var pattern in wr.Include)
                    {
                        if (string.IsNullOrEmpty(pattern) ||
                            pattern == Rest)
                            continue;

                        if (string.Equals(pattern, item.FullPath, StringComparison.OrdinalIgnoreCase))
                        {
                            isMatch = false;
                            break;
                        }
                    }

                    if (isMatch)
                        newCategory[item] = wr.FullPath;
                }
            }

            if (!newCategory.Any())
                return byCategory;

            string getCategory(NavigationItemAttribute x)
            {
                return newCategory.TryGetValue(x, out var c) ? c :
                    (x.Category ?? "");
            }

            return Sort(list, getCategory).ToLookup(getCategory, StringComparer.OrdinalIgnoreCase);
        }
    }
}