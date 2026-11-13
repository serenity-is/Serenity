import { bindThis } from "@serenity-is/domwise";
import { Column, type ArgsCell } from "@serenity-is/sleekgrid";
import { classTypeInfo, Fluent, localText, nsSerenity, registerType, stringFormat } from "../../base";
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
    /**
     * Shows a "N selected" bar with a Clear button in the grid toolbar while rows are selected.
     */
    selectionBar?: boolean;
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
    declare private anchor: string;
    declare private selectionBar: HTMLElement;
    declare private itemMetadata: (item: any, row: number) => any;

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
        grid.getElement().addEventListener("mousedown", boundThis.handleMouseDown);
        grid.getElement().addEventListener("keydown", boundThis.handleKeyDown);

        const view = grid.getView();
        this.itemMetadata = view.getItemMetadataCallback();
        view.setItemMetadataCallback((item, row) => {
            const metadata = this.itemMetadata?.(item, row);
            if (!item || !this.include[item[this.idField]])
                return metadata;
            return { ...metadata, cssClasses: metadata?.cssClasses ? metadata.cssClasses + " selected" : "selected" };
        });
    }

    /**
     * Detaches the mixin from the grid and cleans up event subscriptions.
     */
    destroy(): void {
        this.include = Object.create(null);
        this.grid?.getGrid()?.onClick?.unsubscribe(this.handleGridClick);
        this.grid?.getGrid()?.onHeaderClick?.unsubscribe(this.handleHeaderClick);
        (this.grid?.getView() as IRemoteView).onRowsChanged?.unsubscribe(this.updateSelectAll);
        this.grid?.getElement()?.removeEventListener("mousedown", this.handleMouseDown);
        this.grid?.getElement()?.removeEventListener("keydown", this.handleKeyDown);
        this.grid?.getView()?.setItemMetadataCallback(this.itemMetadata);
        this.selectionBar?.remove();
        delete this.grid;
        delete this.options?.selectable;
        this.options = null;
    }

    private handleGridClick(e: MouseEvent & Partial<ArgsCell>): void {
        if (!(e.target as HTMLElement).classList.contains('select-item'))
            return;
        const grid = this.grid;
        e.preventDefault();
        const view = grid.getView();
        const id = view.getItem(e.row)[this.idField].toString();
        const select = !this.include[id];
        const from = e.shiftKey && this.anchor != null ? this.rowOfKey(this.anchor) : -1;
        const rows: number[] = [];
        if (from < 0)
            rows.push(e.row);
        else
            for (let row = Math.min(from, e.row); row <= Math.max(from, e.row); row++)
                rows.push(row);

        for (const row of rows) {
            const item = view.getItem(row);
            if (!this.isSelectable(item))
                continue;
            const key = item[this.idField].toString();
            if (select)
                this.include[key] = true;
            else
                delete this.include[key];
        }

        this.anchor = id;
        this.updateSelectAll();
        grid.getGrid().invalidateRows(rows);
        grid.getGrid().render();
        grid.getGrid().getCellNode(e.row, e.cell)?.querySelector<HTMLElement>(".select-item")?.focus();
    }

    private handleMouseDown(e: MouseEvent): void {
        if (e.shiftKey && (e.target as HTMLElement).classList.contains("select-item"))
            e.preventDefault();
    }

    private handleKeyDown(e: KeyboardEvent): void {
        const target = e.target as HTMLElement;
        if (e.key === " ") {
            const box = target.classList.contains("slick-cell") ? target.querySelector<HTMLElement>(".select-item") : null;
            if (box) {
                e.preventDefault();
                box.click();
            }
        }
        else if (e.key === "Escape" && this.getSelectedKeys().length > 0 &&
            !target.matches("input:not(.select-item, .select-all-items), textarea, select")) {
            e.stopPropagation();
            this.clearAndRefresh();
        }
    }

    private rowOfKey(id: string): number {
        const view = this.grid.getView();
        for (let i = 0, n = view.getLength(); i < n; i++) {
            if (view.getItem(i)?.[this.idField]?.toString() === id)
                return i;
        }
        return -1;
    }

    private clearAndRefresh(): void {
        this.clear();
        this.grid.getGrid().invalidateAllRows();
        this.grid.getGrid().render();
    }

    private handleHeaderClick(e: Event): void {
        if (Fluent.isDefaultPrevented(e))
            return;

        if (!(e.target as HTMLElement).classList.contains('select-all-items'))
            return;

        e.preventDefault();

        const grid = this.grid;
        this.anchor = null;
        if (Object.keys(this.include).length > 0) {
            clearKeys(this.include);
        }
        else {
            const items = grid.getView().getItems();
            for (const x of items.filter(bindThis(this).isSelectable)) {
                const id1 = x[this.idField];
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
        const selectAllButton = this.grid.getElement()
            .querySelector<HTMLInputElement>(".select-all-header .slick-column-name .select-all-items");

        if (selectAllButton) {
            const selectable = this.grid.getView().getItems().filter(bindThis(this).isSelectable);
            const count = selectable.filter(x => this.include[x[this.idField]]).length;
            const all = count > 0 && count >= selectable.length;
            selectAllButton.classList.toggle("checked", all);
            selectAllButton.checked = all;
            selectAllButton.indeterminate = count > 0 && !all;
        }

        this.updateSelectionBar();
    }

    private updateSelectionBar(): void {
        if (!this.options.selectionBar)
            return;

        this.selectionBar ??= this.createSelectionBar();
        if (!this.selectionBar)
            return;

        const count = this.getSelectedKeys().length;
        const view = this.grid.getView();
        let onPage = 0;
        for (let i = 0; i < view.getLength(); i++) {
            const item = view.getItem(i);
            if (item && this.include[item[this.idField]])
                onPage++;
        }

        this.selectionBar.hidden = count === 0;
        this.selectionBar.querySelector(".selection-count").textContent =
            stringFormat(localText("Controls.GridSelection.Selected", "{0} selected"), count);
        this.selectionBar.querySelector(".selection-page").textContent = onPage === count ? "" :
            stringFormat(localText("Controls.GridSelection.OnPage", "{0} on this page"), onPage);
    }

    private createSelectionBar(): HTMLElement {
        const toolbar = this.grid.getElement().querySelector(".grid-toolbar");
        return toolbar?.insertBefore(<div class="selection-bar" hidden>
            <span class="selection-count"></span>
            <span class="selection-page"></span>
            <button type="button" class="selection-clear btn btn-link btn-sm" onClick={bindThis(this).clearAndRefresh}>
                {localText("Controls.GridSelection.Clear", "Clear")}
            </button>
        </div> as HTMLElement, toolbar.querySelector(":scope > .clear"));
    }

    /**
     * Clears the current selection.
     */
    clear(): void {
        this.anchor = null;
        clearKeys(this.include);
        this.updateSelectAll();
    }

    /**
     * Clears the current selection and refreshes the grid view.
     */
    resetCheckedAndRefresh(): void {
        this.include = Object.create(null);
        this.anchor = null;
        this.updateSelectAll();
        this.grid.getView().populate();
    }

    /**
     * Selects the items with the given keys, keeping any existing selection.
     * @param keys - The keys of the items to select.
     */
    selectKeys(keys: string[]): void {
        for (const k of keys) {
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
        for (const k of keys) {
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
            nameFormat: () => <input type="checkbox" class="select-all-items check-box no-float"
                aria-label={localText("Controls.GridSelection.SelectAll", "Select all rows")} />,
            toolTip: ' ',
            id: '__select__',
            resizable: false,
            width: 27,
            minWidth: 27,
            maxWidth: 27,
            headerCssClass: 'select-all-header',
            sortable: false,
            format: function (ctx) {
                const item = ctx.item;
                const mixin = getMixin();
                if (!mixin || !mixin.isSelectable(item)) {
                    return '';
                }
                const isChecked = mixin.include[ctx.item[mixin.idField]];
                return <input type="checkbox" class={"select-item check-box no-float" + (isChecked ? " checked" : "")}
                    checked={!!isChecked} aria-label={localText("Controls.GridSelection.SelectRow", "Select row")} />;
            },
            ...columnOptions
        };
    }
}
