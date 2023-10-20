import { Column, FormatterContext } from "@serenity-is/sleekgrid";
import { htmlEncode, ListResponse, toGrouping, tryFirst } from "../../q";
import { SlickFormatting, SlickHelper, SlickTreeHelper } from "../helpers/slickhelpers";
import { DataGrid } from "./datagrid";

/**
 * A mixin that can be applied to a DataGrid for tree functionality
 */
export class TreeGridMixin<TItem> {

    private dataGrid: DataGrid<TItem, any>;
    private getId: (item: TItem) => any;

    constructor(private options: TreeGridMixinOptions<TItem>) {
        var dg = this.dataGrid = options.grid;
        var idProperty = (dg as any).getIdProperty();
        var getId = this.getId = (item: TItem) => (item as any)[idProperty];

        dg.element.find('.grid-container').on('click', e => {
            if ($(e.target).hasClass('s-TreeToggle')) {
                var src = dg.slickGrid.getCellFromEvent(e);
                if (src.cell >= 0 &&
                    src.row >= 0) {
                    SlickTreeHelper.toggleClick<TItem>(e, src.row, src.row, dg.view, getId);
                }
            }
        });

        var oldViewFilter = (dg as any).onViewFilter;
        (dg as any).onViewFilter = function (item: TItem) {
            if (!oldViewFilter.apply(this, [item]))
                return false;

            return SlickTreeHelper.filterById(item, dg.view, options.getParentId);
        };

        var oldProcessData = (dg as any).onViewProcessData;
        (dg as any).onViewProcessData = function (response: ListResponse<TItem>) {

            response = oldProcessData.apply(this, [response]);
            response.Entities = TreeGridMixin.applyTreeOrdering(response.Entities, getId, options.getParentId);
            SlickTreeHelper.setIndents(response.Entities, getId, options.getParentId,
                (options.initialCollapse && options.initialCollapse()) || false);
            return response;
        };

        if (options.toggleField) {
            var col = tryFirst(dg['allColumns'] || dg.slickGrid.getColumns() || [], x => x.field == options.toggleField) as Column<TItem>;
            if (col) {
                col.format = SlickFormatting.treeToggle(() => dg.view, getId,
                    col.format || ((ctx: FormatterContext<TItem>) => htmlEncode(ctx.value)));
            }
        }
    }

    /**
     * Expands / collapses all rows in a grid automatically
     */
    toggleAll(): void {
        SlickTreeHelper.setCollapsed(this.dataGrid.view.getItems(),
            !this.dataGrid.view.getItems().every(x => (x as any)._collapsed == true));

        this.dataGrid.view.setItems(this.dataGrid.view.getItems(), true);
    }

    collapseAll(): void {
        SlickTreeHelper.setCollapsed(this.dataGrid.view.getItems(), true);
        this.dataGrid.view.setItems(this.dataGrid.view.getItems(), true);
    }
    
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

        for (var item of items)
        {
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

export interface TreeGridMixinOptions<TItem> {
    // data grid object
    grid: DataGrid<TItem, any>;
    // a function to get parent id
    getParentId: (item: TItem) => any;
    // where should the toggle button be placed
    toggleField: string;
    // a delegate that should return initial collapsing state
    initialCollapse?: () => boolean;
}
