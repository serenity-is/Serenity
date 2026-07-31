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
        ArgumentNullException.ThrowIfNull(permissions);

        var result = new List<NavigationItem>();

        void processAttr(NavigationItem parent, NavigationItemAttribute attr, decimal order, int depth)
        {
            var item = new NavigationItem();

            bool isAuthorizedSection = !string.IsNullOrEmpty(attr.Url) &&
                (string.IsNullOrEmpty(attr.Permission) || permissions.HasPermission(attr.Permission));

            var path = string.IsNullOrEmpty(attr.Category) ? "" : (attr.Category + "/");
            path += attr.Title ?? "";

            // attrByCategory[path] is already sorted ascending by Order (ties in stable original order)
            var children = attrByCategory[path].ToList();
            var childOrders = HealOrders([.. children.Select(x => x.Order)],
                parentOrder: order, step: GetOrderStep(depth + 1));
            var target = parent?.Children ?? result;
            for (var i = 0; i < children.Count; i++)
                processAttr(item, children[i], childOrders[i], depth + 1);

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
                item.Order = order;
                target.Add(item);
            }
        }

        var topLevel = attrByCategory[""].ToList();
        var topOrders = HealOrders([.. topLevel.Select(x => x.Order)], step: GetOrderStep(0));
        for (var i = 0; i < topLevel.Count; i++)
            processAttr(null, topLevel[i], topOrders[i], 0);

        return result;
    }

    private static decimal GetOrderStep(int depth)
    {
        return depth switch
        {
            0 => 1000m,
            1 => 100m,
            2 => 10m,
            _ => 1m
        };
    }

    /// <summary>
    /// Resolves duplicate order values among a set of siblings into unique decimal values, e.g. so
    /// that statically declared navigation items (whose <see cref="int"/> order may collide, as it is
    /// only guaranteed unique by developer convention, not enforced) get distinct sort keys that a
    /// consumer (e.g. a CMS module) can also use to compute a midpoint when inserting new items between
    /// them.
    /// This only changes orders for items that actually collide with another sibling; non-colliding
    /// orders (other than <paramref name="sentinel"/>, see below) are returned unchanged, and within a
    /// group of colliding siblings, the first one (in the order it was given) also keeps its original
    /// value, minimizing the number of altered orders.
    /// Any order equal to <paramref name="sentinel"/> (the code generator's default, meaning "not
    /// explicitly set") is never left as-is: it is always resolved to <paramref name="parentOrder"/>
    /// (or the largest real sibling order, if there is one) plus a multiple of <paramref name="step"/>,
    /// so a forgotten order ends up near its siblings instead of near <see cref="int.MaxValue"/>.
    /// </summary>
    /// <param name="orders">Sibling order values, already sorted ascending. Ties should be in a stable,
    /// deterministic order (e.g. as produced by a stable sort), as that relative order is preserved.</param>
    /// <param name="parentOrder">The parent item's own resolved order, used as the fallback baseline for
    /// <paramref name="sentinel"/> orders when there is no real sibling order to anchor to. Null (the
    /// default) for a top level item, or if the parent's order isn't known/relevant.</param>
    /// <param name="sentinel">The order value that means "not explicitly set". Defaults to
    /// <see cref="int.MaxValue"/>, matching the code generator's navigation link template and the
    /// string-only <c>NavigationLinkAttribute</c>/<c>NavigationMenuAttribute</c> constructors.</param>
    /// <param name="step">The spacing used both between resolved <paramref name="sentinel"/> orders, and
    /// as the fallback gap for a trailing duplicate group that has no following distinct order.</param>
    public static IReadOnlyList<decimal> HealOrders(IReadOnlyList<decimal> orders,
        decimal? parentOrder = null, decimal sentinel = int.MaxValue, decimal step = 100m)
    {
        ArgumentNullException.ThrowIfNull(orders);

        var result = new decimal[orders.Count];
        var i = 0;
        decimal? lastResolved = null;
        while (i < orders.Count)
        {
            var j = i + 1;
            while (j < orders.Count && orders[j] == orders[i])
                j++;

            var groupSize = j - i;
            if (orders[i] == sentinel)
            {
                // never leave an unset order near int.MaxValue; anchor it to the last real
                // sibling order seen so far, or the parent's, so it stays a "reasonable" value
                var baseline = lastResolved ?? parentOrder ?? 0m;
                for (var k = 0; k < groupSize; k++)
                    result[i + k] = baseline + step * (k + 1);
            }
            else if (groupSize == 1)
            {
                result[i] = orders[i];
            }
            else
            {
                // spread the duplicates across the gap to the next distinct order, so none of them
                // collides with each other or with that next value; the first keeps its declared order
                var baseOrder = orders[i];
                var gap = j < orders.Count ? orders[j] - baseOrder : step;
                var groupStep = gap / groupSize;
                for (var k = 0; k < groupSize; k++)
                    result[i + k] = baseOrder + k * groupStep;
            }

            lastResolved = result[j - 1];
            i = j;
        }

        return result;
    }

    private static ILookup<string, NavigationItemAttribute> GetNavigationItemAttributes(
        ITypeSource typeSource, IServiceProvider serviceProvider,
        Func<NavigationItemAttribute, bool> filter)
    {
        var list = new List<NavigationItemAttribute>();
        var featureToggles = serviceProvider?.GetService<IFeatureToggles>();

        foreach (NavigationItemAttribute attr in typeSource
            .GetAssemblyAttributes<NavigationItemAttribute>())
        {
            if (featureToggles != null &&
                attr.RequireFeatures != null &&
                attr.RequireFeatures.Length > 0 &&
                !featureToggles.IsEnabled(attr.RequireFeatures, attr.RequireAnyFeature))
                continue;

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
        ArgumentNullException.ThrowIfNull(list);

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
                {
                    var menu = new NavigationMenuAttribute(0, path)
                    {
                        Order = group.Min(x => x.Order)
                    };
                    missing.Add(path, menu);
                }

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
            decimal? minOrder = null;
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
                        if (!minOrder.HasValue || child.Order < minOrder)
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
                        if (!minOrder.HasValue || item.Order < minOrder)
                            minOrder = item.Order;
                    }
                }
            }

            if (group.Order == int.MaxValue &&
                minOrder.HasValue)
                group.Order = minOrder.Value;
        }

        foreach (var group in groups.Where(x => x.Default))
        {
            decimal? minOrder = null;
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
                    if (!minOrder.HasValue || item.Order < minOrder)
                        minOrder = item.Order;
                }
            }

            if (group.Order == int.MaxValue &&
                minOrder.HasValue)
                group.Order = minOrder.Value;
        }

        if (newCategory.Count == 0)
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