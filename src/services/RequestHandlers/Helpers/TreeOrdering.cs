namespace Serenity.Services;

/// <summary>
/// Tree based sorting helper. E.g. in a tree, a node's parents
/// should come before itself. Such an ordering is not easy
/// in SQL so we use this helper to do ordering client side.
/// </summary>
public static class TreeOrdering
{
    /// <summary>
    /// Applies tree based ordering to the items
    /// </summary>
    /// <typeparam name="TItem">Type of items</typeparam>
    /// <typeparam name="TIdentity">Type of ID fields of the items</typeparam>
    /// <param name="items">List of items</param>
    /// <param name="getId">Callback to get ID for an item</param>
    /// <param name="getParentId">Callback to get parent ID for an item</param>
    /// <returns></returns>
    public static List<TItem> Sort<TItem, TIdentity>(IEnumerable<TItem> items,
        Func<TItem, TIdentity> getId, Func<TItem, TIdentity?> getParentId)
        where TIdentity : struct
    {
        var result = new List<TItem>();

        var itemById = items.ToLookup(getId);
        var byParentId = items.ToLookup(getParentId);

        var visited = new HashSet<TIdentity>();

        void takeChildren(TIdentity theParentId)
        {
            if (visited.Contains(theParentId))
                return;

            visited.Add(theParentId);

            foreach (var item in byParentId[theParentId])
            {
                result.Add(item);
                takeChildren(getId(item));
            }
        }

        foreach (var item in items)
        {
            var parentId = getParentId(item);
            if (parentId == null ||
                !itemById[parentId.Value].Any())
            {
                result.Add(item);
                takeChildren(getId(item));
            }
        }

        return result;
    }
}