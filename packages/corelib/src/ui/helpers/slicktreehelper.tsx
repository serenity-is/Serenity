import { Fluent } from "../../base";
import { IRemoteView } from "../../slick";

/**
 * Helper functions for tree-structured data in grids.
 */
export namespace SlickTreeHelper {
    /**
     * Returns whether an item should be visible given the collapsed state of its
     * ancestors.
     * @typeParam TItem - The type of the item.
     * @param item - The item to check.
     * @param getParent - A function that returns the parent of an item.
     * @returns True if the item is visible, otherwise false.
     */
    export function filterCustom<TItem>(item: TItem, getParent: (x: TItem) => any): boolean {
        var parent = getParent(item);
        var loop = 0;
        while (parent != null) {
            if (!!parent._collapsed) {
                return false;
            }
            parent = getParent(parent);
            if (loop++ > 1000) {
                throw new Error(
                    'Possible infinite loop, check parents has no circular reference!');
            }
        }
        return true;
    }

    /**
     * Returns whether an item should be visible by resolving its parent chain
     * through the view.
     * @typeParam TItem - The type of the item.
     * @param item - The item to check.
     * @param view - The remote view used to resolve parents.
     * @param getParentId - A function that returns the parent id of an item.
     * @returns True if the item is visible, otherwise false.
     */
    export function filterById<TItem>(item: TItem, view: IRemoteView<TItem>,
        getParentId: (x: TItem) => any): boolean {
        return filterCustom(item, function (x) {
            var parentId = getParentId(x);
            if (parentId == null) {
                return null;
            }
            return view.getItemById(parentId);
        });
    }

    /**
     * Sets the collapsed state of all given items.
     * @typeParam TItem - The type of the item.
     * @param items - The items to update.
     * @param collapsed - The collapsed state to set.
     */
    export function setCollapsed<TItem>(items: TItem[], collapsed: boolean): void {
        if (items != null) {
            for (var item of items) {
                (item as any)._collapsed = collapsed;
            }
        }
    }

    /**
     * Sets the collapsed state of a single item.
     * @typeParam TItem - The type of the item.
     * @param item - The item to update.
     * @param collapsed - The collapsed state to set.
     */
    export function setCollapsedFlag<TItem>(item: TItem, collapsed: boolean): void {
        (item as any)._collapsed = collapsed;
    }

    /**
     * Computes and sets the indent level of each item based on its parent chain.
     * @typeParam TItem - The type of the item.
     * @param items - The items to update.
     * @param getId - A function that returns the id of an item.
     * @param getParentId - A function that returns the parent id of an item.
     * @param setCollapsed - Optional collapsed state to set on each item.
     */
    export function setIndents<TItem>(items: TItem[], getId: (x: TItem) => any,
        getParentId: (x: TItem) => any, setCollapsed?: boolean): void {
        var depth = 0;
        var depths: Record<any, any> = {};
        for (var line = 0; line < items.length; line++) {
            var item = items[line];
            if (line > 0) {
                var parentId = getParentId(item);
                if (parentId != null && parentId === getId(items[line - 1])) {
                    depth += 1;
                }
                else if (parentId == null) {
                    depth = 0;
                }
                else if (parentId !== getParentId(items[line - 1])) {
                    if (depths[parentId] != null) {
                        depth = depths[parentId] + 1;
                    }
                    else {
                        depth = 0;
                    }
                }
            }
            depths[getId(item)] = depth;
            (item as any)._indent = depth;
            if (setCollapsed != null) {
                (item as any)._collapsed = setCollapsed;
            }
        }
    }

    /**
     * Handles a click on a tree toggle, expanding or collapsing the item and its
     * descendants when the shift key is held.
     * @typeParam TItem - The type of the item.
     * @param e - The click event.
     * @param row - The row index of the clicked item.
     * @param cell - The cell index of the clicked item.
     * @param view - The remote view.
     * @param getId - A function that returns the id of an item.
     */
    export function toggleClick<TItem>(e: Event, row: number, cell: number,
        view: IRemoteView<TItem>, getId: (x: TItem) => any): void {
        if (!e || !e.target || Fluent.isDefaultPrevented(e)) 
            return;

        var target = e.target as HTMLElement;
        if (!target.classList.contains('s-TreeToggle')) {
            return;
        }
        
        e.preventDefault();

        if (target.classList.contains('s-TreeCollapse') || target.classList.contains('s-TreeExpand')) {
            var item = view.getItem(row) as any;
            if (item != null) {
                if (!item._collapsed) {
                    item._collapsed = true;
                }
                else {
                    item._collapsed = false;
                }
                view.updateItem(getId(item), item);

                if ((e as any).shiftKey) {
                    view.beginUpdate();
                    try {
                        setCollapsed(view.getItems(), !!item._collapsed);
                        view.setItems(view.getItems(), true);
                    }
                    finally {
                        view.endUpdate();
                    }
                }
            }
        }
    }
}

