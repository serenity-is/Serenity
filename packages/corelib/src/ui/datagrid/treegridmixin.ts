import { Column, FormatterContext } from "@serenity-is/sleekgrid";
import { Fluent, ListResponse } from "../../base";
import { toGrouping } from "../../compat";
import { SlickFormatting } from "../helpers/slickformatting";
import { SlickTreeHelper } from "../helpers/slicktreehelper";
import { DataGrid } from "./datagrid";

/**
 * Adds tree / hierarchy support to a {@link DataGrid} by handling indentation,
 * expand/collapse toggles, and parent-before-child ordering.
 * Attach by constructing the mixin with the target grid and hierarchy options.
 * @typeParam TItem - Row type displayed in the grid.
 */
export class TreeGridMixin<TItem> {

    /** Underlying data grid this mixin is attached to. */
    declare private dataGrid: DataGrid<TItem, any>;

    /**
     * Creates a tree mixin for the specified grid.
     * @param options - Hierarchy configuration including grid reference and parent id accessor.
     */
    constructor(private options: TreeGridMixinOptions<TItem>) {
        var dg = this.dataGrid = options.grid;
        var idProperty = (dg as any).getIdProperty();
        var getId = (item: TItem) => (item as any)[idProperty];

        var gridContainer = dg.domNode.querySelector('.grid-container');
        if (gridContainer) {
            Fluent.on(gridContainer, "click", (e) => {
                if ((e.target as HTMLElement).classList.contains('s-TreeToggle')) {
                    var src = dg.sleekGrid.getCellFromEvent(e);
                    if (src.cell >= 0 &&
                        src.row >= 0) {
                        SlickTreeHelper.toggleClick<TItem>(e as any, src.row, src.cell, dg.view, getId);
                    }
                }
            });
        }

        dg.onFiltering.subscribe((e) => {
            e.isMatch = SlickTreeHelper.filterById(e.item, dg.view, options.getParentId);
        });

        dg.onProcessData.subscribe((e) => {
            e.response.Entities = TreeGridMixin.applyTreeOrdering(e.response.Entities, getId, options.getParentId);
            SlickTreeHelper.setIndents(e.response.Entities, getId, options.getParentId,
                (options.initialCollapse && options.initialCollapse()) || false);
        });

        if (options.toggleField) {
            var col = dg.allColumns?.find(x => x.field == options.toggleField || x.id == options.toggleField) as Column<TItem>;
            if (col) {
                col.format = SlickFormatting.treeToggle(() => dg.view, getId,
                    col.format || ((ctx: FormatterContext<TItem>) => ctx.escape()));
            }
        }
    }

    /**
     * Toggles all rows between collapsed and expanded.
     * If every row is collapsed, all rows are expanded and vice versa.
     */
    toggleAll(): void {
        SlickTreeHelper.setCollapsed(this.dataGrid.view.getItems(),
            !this.dataGrid.view.getItems().every(x => (x as any)._collapsed == true));

        this.dataGrid.view.setItems(this.dataGrid.view.getItems(), true);
    }

    /** Collapses all rows in the associated grid. */
    collapseAll(): void {
        SlickTreeHelper.setCollapsed(this.dataGrid.view.getItems(), true);
        this.dataGrid.view.setItems(this.dataGrid.view.getItems(), true);
    }

    /** Expands all rows in the associated grid. */
    expandAll(): void {
        SlickTreeHelper.setCollapsed(this.dataGrid.view.getItems(), false);
        this.dataGrid.view.setItems(this.dataGrid.view.getItems(), true);
    }

    /**
     * Reorders a set of items so that parents comes before their children.
     * This method is required for proper tree ordering, as it is not so easy to perform with SQL.
     * @param items list of items to be ordered
     * @param getId a delegate to get ID of a record (must return same ID with grid identity field)
     * @param getParentId a delegate to get parent ID of a record
     */
    static applyTreeOrdering<TItem>(items: TItem[], getId: (item: TItem) => any, getParentId: (item: TItem) => any): TItem[] {
        var result: TItem[] = [];

        var byId = toGrouping(items, getId);
        var byParentId = toGrouping(items, getParentId);
        var visited: Record<string, boolean> = {};

        function takeChildren(theParentId: any) {
            if (visited[theParentId])
                return;

            visited[theParentId] = true;
            for (var child of (byParentId[theParentId] || [])) {
                result.push(child);
                takeChildren(getId(child));
            }
        }

        for (var item of items) {
            var parentId = getParentId(item);
            if (parentId == null ||
                !((byId[parentId] || []).length)) {
                result.push(item);
                takeChildren(getId(item));
            }
        }

        return result;
    }
}

/**
 * Options for {@link TreeGridMixin}.
 * @typeParam TItem - Row type displayed in the grid.
 */
export interface TreeGridMixinOptions<TItem> {
    /** Target data grid to enhance with tree behaviour. */
    grid: DataGrid<TItem, any>;
    /** Callback that returns the parent identifier for a row. */
    getParentId: (item: TItem) => any;
    /** Field / column id where the expand/collapse toggle is rendered. */
    toggleField: string;
    /** Optional callback that controls whether rows start collapsed. */
    initialCollapse?: () => boolean;
}
