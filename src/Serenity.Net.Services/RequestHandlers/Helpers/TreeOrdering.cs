using System;
using System.Collections.Generic;
using System.Linq;

namespace Serenity.Services
{
    public static class TreeOrdering
    {
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
}