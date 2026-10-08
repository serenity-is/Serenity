namespace Serenity.Services;

/// <summary>
/// Tree based sorting helper. E.g. in a tree, a node's parents
/// should come before itself. Such an ordering is not easy
/// in SQL so we use this helper to do ordering client side.
/// </summary>
public static class TreeOrdering
{
    /// <summary>
    /// Applies tree based ordering to the items.
    /// </summary>
    /// <typeparam name="TItem">Type of items</typeparam>
    /// <typeparam name="TIdentity">Type of ID fields of the items</typeparam>
    /// <param name="items">List of items</param>
    /// <param name="getId">Callback to get ID for an item</param>
    /// <param name="getParentId">Callback to get parent ID for an item</param>
    /// <returns>The tree ordered list of items.</returns>
    public static List<TItem> Sort<TItem, TIdentity>(IEnumerable<TItem> items,
        Func<TItem, TIdentity> getId, Func<TItem, TIdentity?> getParentId)
        where TIdentity : struct
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(getId);
        ArgumentNullException.ThrowIfNull(getParentId);

        var itemList = items as IReadOnlyList<TItem> ?? items.ToList();
        var result = new List<TItem>(itemList.Count);

        if (itemList.Count == 0)
            return result;

        var itemById = itemList.ToLookup(getId);
        var byParentId = itemList.ToLookup(getParentId);

        var emitted = new HashSet<TItem>();
        var expanded = new HashSet<TIdentity>();
        var stack = new Stack<TItem>();

        void processStack()
        {
            while (stack.Count > 0)
            {
                var item = stack.Pop();
                if (!emitted.Add(item))
                    continue;

                result.Add(item);

                var id = getId(item);
                if (!expanded.Add(id))
                    continue;

                var children = byParentId[id].ToList();
                for (var i = children.Count - 1; i >= 0; i--)
                    stack.Push(children[i]);
            }
        }

        for (var i = itemList.Count - 1; i >= 0; i--)
        {
            var item = itemList[i];
            var parentId = getParentId(item);
            if (parentId == null || !itemById[parentId.Value].Any())
                stack.Push(item);
        }

        processStack();

        for (var i = itemList.Count - 1; i >= 0; i--)
            if (!emitted.Contains(itemList[i]))
                stack.Push(itemList[i]);

        processStack();

        return result;
    }
}