using Microsoft.Extensions.DependencyInjection;

namespace Serenity.Navigation;

/// <summary>
/// Contains helper methods for navigation models and items
/// </summary>
public class NavigationHelper
{
    /// <summary>
    /// Gets navigation items
    /// </summary>
    /// <param name="permissions">Permission service</param>
    /// <param name="typeSource">Type source</param>
    /// <param name="serviceProvider">Service provider</param>
    /// <param name="resolveUrl">Resolve URL callback</param>
    /// <param name="filter">Filter function</param>
    public static List<NavigationItem> GetNavigationItems(IPermissionService permissions, 
        ITypeSource typeSource, IServiceProvider serviceProvider, 
        Func<string, string> resolveUrl = null, 
        Func<NavigationItemAttribute, bool> filter = null)
    {
        var menuItems = GetNavigationItemAttributes(typeSource, serviceProvider, filter);
        return ConvertToNavigationItems(permissions, menuItems, resolveUrl);
    }

    /// <summary>
    /// Converts a list of <see cref="NavigationItemAttribute"/> objects to a list of
    /// <see cref="NavigationItem"/> classes.
    /// </summary>
    /// <param name="permissions">Permission service</param>
    /// <param name="attrByCategory">A lookup to find attributes by their category</param>
    /// <param name="resolveUrl">Resolve url callback</param>
    /// <exception cref="ArgumentNullException">One of the arguments is null</exception>
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
                item.IsSection = attr is NavigationSectionAttribute;
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

    private static IEnumerable<NavigationItemAttribute> Sort(IEnumerable<NavigationItemAttribute> list,
        Func<NavigationItemAttribute, string> getCategory)
    {
        return list.OrderBy(x => getCategory(x) ?? "")
            .ThenBy(x => x.Order);
    }

    /// <summary>
    /// Creates a lookup of navigation item attributes by their category
    /// </summary>
    /// <param name="list">List with navigation item attributes</param>
    /// <exception cref="ArgumentNullException">List is null</exception>
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

        var groups = list.OfType<NavigationGroupAttribute>();

        if (!groups.Any())
            return byCategory;

        var newCategory = new Dictionary<NavigationItemAttribute, string>();

        foreach (var group in groups.Where(x => x.Include != null && !x.Default))
        {
            var minOrder = int.MaxValue;
            foreach (var pattern in group.Include)
            {
                if (string.IsNullOrEmpty(pattern))
                    continue;

                if (pattern.EndsWith('/'))
                {
                    foreach (var child in byCategory[pattern[..^1]]
                        .Where(x => x != group && x is not NavigationGroupAttribute && !newCategory.ContainsKey(x)))
                    {
                        newCategory[child] = group.FullPath;
                        if (child.Order < minOrder)
                            minOrder = child.Order;
                    }
                }
                else
                {
                    var idx = pattern.LastIndexOf('/');
                    var search = idx < 0 ? pattern : pattern[(idx + 1)..];
                    var items = (idx < 0 ? byCategory[""] : byCategory[pattern[..idx]])
                        .Where(x => string.Equals(x.FullPath, pattern, StringComparison.OrdinalIgnoreCase));
                        
                    foreach (var item in items.Where(x => 
                        x != group && x is not NavigationGroupAttribute && !newCategory.ContainsKey(x)))
                    {
                        newCategory[item] = group.FullPath;
                        if (item.Order < minOrder)
                            minOrder = item.Order;
                    }
                }
            }

            if (group.Order == int.MaxValue &&
                minOrder < int.MaxValue)
                group.Order = minOrder;
        }

        foreach (var group in groups.Where(x => x.Default))
        {
            var minOrder = int.MaxValue;
            foreach (var item in byCategory[group.Category ?? ""])
            {
                if (group == item ||
                    item is NavigationGroupAttribute ||
                    newCategory.ContainsKey(item))
                    continue;

                var isMatch = true;
                if (group.Include != null)
                {
                    foreach (var pattern in group.Include)
                    {
                        if (string.IsNullOrEmpty(pattern))
                            continue;

                        if (string.Equals(pattern, item.FullPath, StringComparison.OrdinalIgnoreCase))
                        {
                            isMatch = false;
                            break;
                        }
                    }
                }

                if (isMatch)
                {
                    newCategory[item] = group.FullPath;
                    if (item.Order < minOrder)
                        minOrder = item.Order;
                }
            }

            if (group.Order == int.MaxValue &&
                minOrder < int.MaxValue)
                group.Order = minOrder;
        }

        if (!newCategory.Any())
            return byCategory;

        var usedGroupPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var group in groups)
            usedGroupPaths.Add(group.FullPath);

        list = list.Where(x => x is NavigationGroupAttribute ||
            !usedGroupPaths.Contains(x.FullPath));

        string getCategory(NavigationItemAttribute x)
        {
            return newCategory.TryGetValue(x, out var c) ? c :
                (x.Category ?? "");
        }

        return Sort(list, getCategory).ToLookup(getCategory, StringComparer.OrdinalIgnoreCase);
    }
}