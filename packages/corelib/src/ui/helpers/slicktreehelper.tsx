import { IRemoteView } from "../../slick";

export namespace SlickTreeHelper {
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

    export function setCollapsed<TItem>(items: TItem[], collapsed: boolean): void {
        if (items != null) {
            for (var item of items) {
                (item as any)._collapsed = collapsed;
            }
        }
    }

    export function setCollapsedFlag<TItem>(item: TItem, collapsed: boolean): void {
        (item as any)._collapsed = collapsed;
    }

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

    export function toggleClick<TItem>(e: Event, row: number, cell: number,
        view: IRemoteView<TItem>, getId: (x: TItem) => any): void {
        var target = e.target as HTMLElement;
        if (!target.classList.contains('s-TreeToggle')) {
            return;
        }
        if (target.classList.contains('s-TreeCollapse') || target.classList.contains('s-TreeExpand')) {
            var item = view.getItem(row) as any;
            if (item != null) {
                if (!!!item._collapsed) {
                    item._collapsed = true;
                }
                else {
                    item._collapsed = false;
                }
                view.updateItem(getId(item), item);
            }
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

