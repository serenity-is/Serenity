import { bindThis } from "@serenity-is/domwise";
import { Column, type ArgsCell } from "@serenity-is/sleekgrid";
import { classTypeInfo, Fluent, nsSerenity, registerType } from "../../base";
import { clearKeys } from "../../compat";
import type { IRemoteView } from "../../slick/iremoteview";
import { IDataGrid } from "../datagrid/idatagrid";

/**
 * Options for the {@link GridRowSelectionMixin}.
 */
export interface GridRowSelectionMixinOptions {
    /**
     * A function that determines whether an item can be selected.
     */
    selectable?: (item: any) => boolean;
}

/**
 * A mixin that adds multi (checkbox) row selection behavior to a data grid,
 * including a select-all header checkbox.
 */
export class GridRowSelectionMixin {

    static [Symbol.typeInfo] = classTypeInfo(nsSerenity); static { registerType(this); }

    declare private idField: string;
    declare private include: { [key: string]: boolean }
    declare private grid: IDataGrid;
    declare private options: GridRowSelectionMixinOptions;

    /**
     * Creates a new GridRowSelectionMixin for the given grid.
     * @param grid - The data grid to attach the mixin to.
     * @param options - Optional mixin options.
     */
    constructor(grid: IDataGrid, options?: GridRowSelectionMixinOptions) {

        this.include = Object.create(null);
        this.grid = grid;
        this.idField = grid.getView().getIdPropertyName();
        this.options = options || {};
        const boundThis = bindThis(this);
        grid.getGrid().onClick.subscribe(boundThis.handleGridClick);
        grid.getGrid().onHeaderClick.subscribe(boundThis.handleHeaderClick);
        (grid.getView() as IRemoteView).onRowsChanged?.subscribe(boundThis.updateSelectAll);
    }

    /**
     * Detaches the mixin from the grid and cleans up event subscriptions.
     */
    destroy(): void {
        this.include = Object.create(null);
        this.grid?.getGrid()?.onClick?.unsubscribe(this.handleGridClick);
        this.grid?.getGrid()?.onHeaderClick?.unsubscribe(this.handleHeaderClick);
        (this.grid?.getView() as IRemoteView).onRowsChanged?.unsubscribe(this.updateSelectAll);
        delete this.grid;
        delete this.options?.selectable;
        this.options = null;
    }

    private handleGridClick(e: Event & Partial<ArgsCell>): void {
        if (!(e.target as HTMLElement).classList.contains('select-item'))
            return;
        const grid = this.grid;
        e.preventDefault();
        var item = grid.getView().getItem(e.row);
        var id = item[this.idField].toString();

        if (this.include[id]) {
            delete this.include[id];
        }
        else {
            this.include[id] = true;
        }

        for (var i = 0; i < (grid.getView() as any).getLength(); i++) {
            grid.getGrid().updateRow(i);
        }

        this.updateSelectAll();
    }

    private handleHeaderClick(e: Event): void {
        if (Fluent.isDefaultPrevented(e))
            return;

        if (!(e.target as HTMLElement).classList.contains('select-all-items'))
            return;

        e.preventDefault();
        
        const grid = this.grid;
        if (Object.keys(this.include).length > 0) {
            clearKeys(this.include);
        }
        else {
            var items = grid.getView().getItems();
            for (var x of items.filter(bindThis(this).isSelectable)) {
                var id1 = x[this.idField];
                this.include[id1] = true;
            }
        }
        this.updateSelectAll();
        grid.getView().setItems(grid.getView().getItems(), true);
        setTimeout(bindThis(this).updateSelectAll, 0);
    }

    /**
     * Updates the checked state of the select-all header button based on the
     * current selection.
     */
    updateSelectAll(): void {
        var selectAllButton = this.grid.getElement()
            .querySelector('.select-all-header .slick-column-name .select-all-items');

        if (selectAllButton) {
            var keys = Object.keys(this.include);
            selectAllButton.classList.toggle('checked',
                keys.length > 0 &&
                this.grid.getView().getItems().filter(
                    bindThis(this).isSelectable).length <= keys.length);
        }
    }

    /**
     * Clears the current selection.
     */
    clear(): void {
        clearKeys(this.include);
        this.updateSelectAll();
    }

    /**
     * Clears the current selection and refreshes the grid view.
     */
    resetCheckedAndRefresh(): void {
        this.include = Object.create(null);
        this.updateSelectAll();
        this.grid.getView().populate();
    }

    /**
     * Selects the items with the given keys, keeping any existing selection.
     * @param keys - The keys of the items to select.
     */
    selectKeys(keys: string[]): void {
        for (var k of keys) {
            this.include[k] = true;
        }

        this.updateSelectAll();
    }

    /**
     * Returns the keys of the currently selected items.
     * @returns The selected keys.
     */
    getSelectedKeys(): string[] {
        return Object.keys(this.include);
    }

    /**
     * Returns the selected keys parsed as 32-bit integers.
     * @returns The selected keys as int32 values.
     */
    getSelectedAsInt32(): number[] {
        return Object.keys(this.include).map(function (x) {
            return parseInt(x, 10);
        });
    }

    /**
     * Returns the selected keys parsed as 64-bit integers.
     * @returns The selected keys as int64 values.
     */
    getSelectedAsInt64(): number[] {
        return Object.keys(this.include).map(function (x) {
            return parseInt(x, 10);
        });
    }

    /**
     * Replaces the current selection with the items having the given keys.
     * @param keys - The keys of the items to select.
     */
    setSelectedKeys(keys: string[]): void {
        this.clear();
        for (var k of keys) {
            this.include[k] = true;
        }

        this.updateSelectAll();
    }

    private isSelectable(item: any) {
        return item && (
            this.options.selectable == null ||
            this.options.selectable(item));
    }

    /**
     * Creates a checkbox select column for the grid, including a select-all header.
     * @param getMixin - A function that returns the mixin instance.
     * @param columnOptions - Optional column options to merge into the select column.
     * @returns The select column definition.
     */
    static createSelectColumn(getMixin: () => GridRowSelectionMixin, columnOptions?: Partial<Column>): Column {
        return {
            name: "[×]",
            nameFormat: () => <span class="select-all-items check-box no-float"></span>,
            toolTip: ' ',
            id: '__select__',
            resizable: false,
            width: 27,
            minWidth: 27,
            maxWidth: 27,
            headerCssClass: 'select-all-header',
            sortable: false,
            format: function (ctx) {
                var item = ctx.item;
                var mixin = getMixin();
                if (!mixin || !mixin.isSelectable(item)) {
                    return '';
                }
                var isChecked = mixin.include[ctx.item[mixin.idField]];
                return <span class={'select-item check-box no-float' + (isChecked ? ' checked' : '')}></span>;
            },
            ...columnOptions
        };
    }
}
