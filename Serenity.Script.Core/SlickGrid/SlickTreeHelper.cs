using jQueryApi;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    /// <summary>
    /// Helper for slick views (remote view)
    /// </summary>
    public static class SlickTreeHelper
    {
        [IncludeGenericArguments(false)]
        public static bool FilterCustom<TEntity>(TEntity item, Func<TEntity, TEntity> getParent)
            where TEntity : class
        {
            var parent = getParent(item);

            while (parent != null)
            {
                if (((dynamic)parent)._collapsed)
                    return false;

                parent = getParent(parent);
            }

            return true;
        }

        [IncludeGenericArguments(false)]
        public static bool FilterById<TEntity>(TEntity item, SlickRemoteView<TEntity> view, 
            Func<TEntity, object> getParentId)
            where TEntity: class
        {
            return FilterCustom(item, x =>
            {
                var parentId = getParentId(x);

                if (parentId == null)
                    return null;

                return view.GetItemById(parentId).As<TEntity>();
            });
        }

        [IncludeGenericArguments(false)]
        public static void SetCollapsed<TEntity>(IEnumerable<TEntity> items, bool collapsed)
        {
            if (items != null)
                foreach (dynamic item in items)
                    item._collapsed = collapsed;
        }

        [IncludeGenericArguments(false)]
        public static void SetIndents<TEntity>(IList<TEntity> items, Func<TEntity, object> getId, Func<TEntity, object> getParentId, 
            bool? setCollapsed = true)
        {
            var depth = 0;
            var depths = new JsDictionary<object, int>();
            for (var line = 0; line < items.Count; line++) 
            {
                var item = items[line];
                if (line > 0) 
                {
                    var parentId = getParentId(item);

                    if (parentId != null && 
                        parentId == getId(items[line - 1])) 
                    {
                        depth += 1;
                    }
                    else if (parentId == null) 
                    {
                        depth = 0;
                    } else if (parentId != getParentId(items[line - 1])) {
                        if (depths.ContainsKey(parentId))
                            depth = depths[parentId] + 1;
                        else
                            depth = 0;
                    }
                }
                depths[getId(item)] = depth;
                ((dynamic)item)._indent = depth;
                if (setCollapsed != null)
                    ((dynamic)item)._collapsed = setCollapsed.Value;
            }
        }

        public static void ToggleClick<TEntity>(jQueryEvent e, int row, int cell, SlickRemoteView<TEntity> view, Func<TEntity, object> getId)
        {
            var target = jQuery.FromElement(e.Target);
            if (!target.HasClass("s-TreeToggle"))
                return;

            if (target.HasClass("s-TreeCollapse") ||
                target.HasClass("s-TreeExpand"))
            {
                dynamic item = view.Rows[row];
                if (item != null)
                {
                    if (Q.IsFalse(item._collapsed))
                        item._collapsed = true;
                    else
                        item._collapsed = false;
                    view.UpdateItem(getId(item), item);
                }
            }
        }
    }
}