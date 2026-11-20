import { bindThis } from "@serenity-is/domwise";
import { Column, Group, type ArgsCell, type CellKeyboardEvent, type FormatterContext, type FormatterResult, type ISleekGrid, type ItemMetadata } from "@serenity-is/sleekgrid";
import { classTypeInfo, Fluent, nsSerenity, PubSub, registerType } from "../../base";
import { clearKeys } from "../../compat";
import type { IRemoteView } from "../../slick/iremoteview";
import { IDataGrid } from "../datagrid/idatagrid";
import { UtilityColumns } from "./utilitycolumns";

/**
 * Options for the {@link GridRowSelectionMixin}.
 */
export interface GridRowSelectionMixinOptions {
    /**
     * A function that determines whether an item can be selected.
     */
    selectable?: (item: any) => boolean;
    /**
     * Whether the checkbox of a group row selects all the rows of the group, and deselects them when
     * they are all selected. Defaults to `true`. Set it to `false` for the views that don't have the
     * rows of a group on the client, like a virtual scroll view, where the checkbox is hidden anyway
     * because the ids of the rows are not known, or when the groups should not be selectable at all.
     */
    selectGroups?: boolean;
}

/**
 * The state of the checked rows of a grid, as {@link GridRowSelectionMixin.getSelectionState} reports it.
 */
export interface GridRowSelectionState {
    /** The keys of the checked rows, which may be on the pages the grid does not show. */
    keys: string[];
    /** The number of the checked rows, which is the length of {@link GridRowSelectionState.keys}. */
    count: number;
    /** The number of the checked rows that the page the grid shows has, e.g. to tell it apart from the rest. */
    onPage: number;
    /** The number of the rows of the page that can be checked, e.g. to tell whether all of them are. */
    selectableOnPage: number;
}

/** The mixin of each grid, so that its column and the widgets that act on its checked rows can find it. */
const instances = new WeakMap<ISleekGrid, GridRowSelectionMixin>();

/**
 * A mixin that adds multi (checkbox) row selection behavior to a data grid,
 * including a select-all header checkbox.
 */
export class GridRowSelectionMixin {

    static [Symbol.typeInfo] = classTypeInfo(nsSerenity); static { registerType(this); }

    /** The id of the column that shows the checkboxes of the rows, which the mixin creates. */
    static get selectColumnId() { return UtilityColumns.selectId; }

    /**
     * Formats a cell of the column of the checkboxes that select the rows, and is also what tells that
     * column apart from the others, see {@link isSelectColumn}. It is a static method, instead of a closure
     * of {@link createSelectColumn}, so that the same function is the format of every such column, and the
     * column can be found whatever its id is.
     * @param ctx - The formatter context.
     * @returns The check box of the row, or an empty string for a row that has none.
     */
    static formatSelectItem(ctx: FormatterContext): FormatterResult {
        const item = ctx.item;
        // the mixin of the grid that is rendered, which is the one the grid or the page created for it
        const mixin = GridRowSelectionMixin.tryGetInstance(ctx.grid);
        if (!mixin)
            return '';

        // the group cell starts after the select column, so the rows of a group show it too,
        // and it selects and deselects all the rows of the group
        if ((item as Group)?.__group) {
            const cssClass = mixin.getGroupCheckBoxCssClass(item as Group);
            return cssClass == null ? '' :
                <span class={'select-item check-box no-float' + cssClass}></span>;
        }

        if (!mixin.isSelectable(item))
            return '';

        const isChecked = mixin.include[ctx.item[mixin.idField]];
        return <span class={'select-item check-box no-float' + (isChecked ? ' checked' : '')}></span>;
    }

    /**
     * The css class of the rows that are checked, which is not the one the selection models of the grid
     * add to the cells of a selection range, so the two can be styled apart.
     */
    static readonly checkedRowClass = "checked-row";

    declare private idField: string;
    declare private include: { [key: string]: boolean }
    declare private grid: IDataGrid;
    declare private options: GridRowSelectionMixinOptions;
    /** What the view used to tell the css classes of a row before the mixin, restored when it is destroyed. */
    declare private itemMetadata: (item: any, row: number) => ItemMetadata<any>;
    /**
     * Notified with the state of the checked rows, after every change of them, e.g. to enable or disable
     * the actions of a bar that acts on them. A mixin that replaces another one for the same grid keeps the
     * subscribers of it, as the rows are the same ones, so a widget that was given the one before it goes
     * on being told about them.
     */
    onSelectionChanged = new PubSub<GridRowSelectionState>();

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

        // the grid makes a mixin for the select column of its columns, and a page may make one of its own
        // as well, e.g. in its constructor, so the later one replaces the earlier one, as leaving the
        // handlers of the earlier one on the grid would toggle a check box twice; the subscribers of the
        // checked rows go on with the new one, e.g. a bar of them that was made for the grid
        const previous = instances.get(grid.getGrid());
        if (previous && previous !== this) {
            this.onSelectionChanged = previous.onSelectionChanged;
            previous.onSelectionChanged = new PubSub<GridRowSelectionState>();
            previous.destroy();
        }

        instances.set(grid.getGrid(), this);
        const boundThis = bindThis(this);
        grid.getGrid().onClick.subscribe(boundThis.handleGridClick);
        grid.getGrid().onHeaderClick.subscribe(boundThis.handleHeaderClick);
        grid.getGrid().onKeyDown?.subscribe(boundThis.handleGridKeyDown);
        (grid.getView() as IRemoteView).onRowsChanged?.subscribe(boundThis.updateSelectAll);

        // the class is checked-row, and not the selected one, which the selection models of the grid add to
        // the cells of a range, so a row that is checked with its checkbox is told apart from a selection
        const view = grid.getView() as IRemoteView;
        this.itemMetadata = view.getItemMetadataCallback?.();
        view.setItemMetadataCallback?.((item: any, row: number) => {
            const metadata = this.itemMetadata?.(item, row);
            if (!this.isSelected(item))
                return metadata;

            return {
                ...metadata,
                cssClasses: metadata?.cssClasses ?
                    metadata.cssClasses + " " + GridRowSelectionMixin.checkedRowClass :
                    GridRowSelectionMixin.checkedRowClass
            };
        });
    }

    /**
     * Returns the mixin attached to a grid, if it has one, e.g. for the column that shows the checkboxes,
     * or for a widget that acts on the checked rows.
     * @param grid - The data grid, or the sleek grid of it.
     * @returns The mixin, or `null` when the grid has none.
     */
    static tryGetInstance(grid: IDataGrid | ISleekGrid): GridRowSelectionMixin | null {
        const sleekGrid = (grid as IDataGrid)?.getGrid?.() ?? grid as ISleekGrid;
        return sleekGrid ? (instances.get(sleekGrid) ?? null) : null;
    }

    /**
     * Detaches the mixin from the grid and cleans up event subscriptions.
     */
    destroy(): void {
        this.include = Object.create(null);
        // the subscribers go with the mixin, e.g. a bar of the checked rows that outlives it
        this.onSelectionChanged?.clear();
        this.grid?.getGrid()?.onClick?.unsubscribe(this.handleGridClick);
        this.grid?.getGrid()?.onHeaderClick?.unsubscribe(this.handleHeaderClick);
        this.grid?.getGrid()?.onKeyDown?.unsubscribe(this.handleGridKeyDown);
        // it may be destroyed twice, e.g. when a page destroys the mixin that a newer one replaced
        (this.grid?.getView() as IRemoteView)?.onRowsChanged?.unsubscribe(this.updateSelectAll);
        (this.grid?.getView() as IRemoteView)?.setItemMetadataCallback?.(this.itemMetadata);
        instances.delete(this.grid?.getGrid());
        delete this.itemMetadata;
        delete this.grid;
        delete this.options?.selectable;
        this.options = null;
    }

    private handleGridClick(e: Event & Partial<ArgsCell>): void {
        if (!(e.target as HTMLElement).classList.contains('select-item'))
            return;

        if (!this.toggleSelection(this.grid.getView().getItem(e.row)))
            return;

        e.preventDefault();
    }

    /**
     * Key handler that toggles the checkbox of the active row on Space, like clicking it. With cell navigation,
     * the cursor has to be on the checkbox; when the grid navigates rows (`enableCellNavigation: "row"`), the
     * cursor is the row, so Space, or X as in a mail client, toggles it from any column.
     * @param e - Cell keyboard event from the grid's `onKeyDown`.
     */
    private handleGridKeyDown(e: CellKeyboardEvent): void {
        if (!e || e.ctrlKey || e.altKey || e.metaKey)
            return;

        const grid = this.grid?.getGrid();
        const rowNavigation = grid?.getOptions?.()?.enableCellNavigation === "row";
        if (e.key !== " " && !(rowNavigation && (e.key === "x" || e.key === "X")))
            return;

        const activeCell = grid?.getActiveCell?.();
        if (!activeCell)
            return;

        const selectCell = this.getSelectColumnIndex(grid);
        if (selectCell == null || (!rowNavigation && activeCell.cell !== selectCell))
            return;

        if (!this.toggleSelection(this.grid.getView().getItem(activeCell.row)))
            return;

        e.preventDefault();
        e.stopImmediatePropagation();
    }

    /**
     * Returns the index of the select column of the grid.
     * @param grid - The grid, defaults to the one of the mixin.
     * @returns The index of the column, or `null` when the grid has no select column.
     */
    private getSelectColumnIndex(grid?: ISleekGrid): number | null {
        const columns = (grid ?? this.grid?.getGrid())?.getColumns?.() ?? [];
        const index = columns.findIndex(GridRowSelectionMixin.isSelectColumn);
        return index < 0 ? null : index;
    }

    /**
     * Selects or deselects a row, or all the rows of a group, and refreshes the grid.
     * @param item - The item of the row, or the group of the row of a group.
     * @returns `true` if the selection changed, `false` when the row can't be selected.
     */
    private toggleSelection(item: any): boolean {
        if ((item as Group)?.__group) {
            const ids = this.getGroupIds(item as Group);
            if (!ids)
                return false;

            const all = ids.every(id => this.include[id]);
            for (const id of ids) {
                if (all)
                    delete this.include[id];
                else
                    this.include[id] = true;
            }

            this.refreshSelection();
            return true;
        }

        // the cell of a totals row is rendered too, as the group cell starts after the select
        // column, but only the data rows have an id to select
        if (!this.isSelectable(item))
            return false;

        const id = item[this.idField].toString();
        if (this.include[id])
            delete this.include[id];
        else
            this.include[id] = true;

        this.refreshSelection();
        return true;
    }

    /**
     * Refreshes the rows after a change of the selection, and the state of the select all button.
     */
    private refreshSelection(): void {
        this.grid.getGrid().invalidate();
        this.updateSelectAll();
    }

    /**
     * Collects the rows of a group and of its subgroups, by the id of the rows, keeping the last of
     * the rows with the same id.
     * @param group - The group to collect the rows of.
     * @param rows - The rows collected so far.
     * @returns The rows, by their id.
     */
    private collectGroupRows(group: Group<any>, rows = new Map<string, any>()): Map<string, any> {
        // the rows of a group also include the rows of its subgroups, so some of them are repeated,
        // and a view that gets the groups from the server may only have the subgroups
        for (const item of group?.rows ?? [])
            rows.set(String(item?.[this.idField]), item);

        for (const child of group?.groups ?? [])
            this.collectGroupRows(child, rows);

        return rows;
    }

    /**
     * Returns the ids of the rows of a group that can be selected, so that the checkbox of the row of the
     * group can select and deselect them.
     * @param group - The group of the row.
     * @returns The ids, or `null` when the group can't be selected, or the view has none of its rows, e.g.
     * when it gets them from the server only as they are needed.
     */
    private getGroupIds(group: Group<any>): string[] | null {
        if (this.options.selectGroups === false)
            return null;

        const rows = this.collectGroupRows(group);
        // a group may have more rows than the ones the view has, e.g. when the view pages them or gets
        // them from the server, and its checkbox is about the ones that are there, like the select all of
        // the header is about the rows of the page, as the ids of the rest are not known to select them
        if (!rows.size)
            return null;

        const ids = [...rows.keys()].filter(id => this.isSelectable(rows.get(id)));
        return ids.length ? ids : null;
    }

    /**
     * Returns the state of the checkbox of the row of a group.
     * @param group - The group of the row.
     * @returns The css classes of the checkbox, or `null` when the group can't be selected.
     */
    private getGroupCheckBoxCssClass(group: Group<any>): string | null {
        const ids = this.getGroupIds(group);
        if (!ids)
            return null;

        const selected = ids.filter(id => !!this.include[id]).length;
        return selected === 0 ? "" : selected === ids.length ? " checked" : " partial";
    }

    /**
     * Whether the item of a row is in the selection, which is by its key, as the rows of the view may be
     * refreshed, and their items may be new objects.
     * @param item - The item of the row, which may be a group or a totals row, which have no id.
     */
    private isSelected(item: any): boolean {
        const id = item?.[this.idField];
        return id != null && !!this.include[String(id)];
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
     * Returns the state of the checked rows, including the ones the page the grid shows does not have.
     */
    getSelectionState(): GridRowSelectionState {
        const keys = this.getSelectedKeys();
        let onPage = 0;
        let selectableOnPage = 0;
        const view = this.grid?.getView() as IRemoteView;

        for (let i = 0, n = view?.getLength?.() ?? 0; i < n; i++) {
            const item = view.getItem(i);
            if (!this.isSelectable(item))
                continue;

            selectableOnPage++;
            if (this.isSelected(item))
                onPage++;
        }

        return {
            keys,
            count: keys.length,
            onPage,
            selectableOnPage
        };
    }

    /**
     * Updates the checked state of the select-all header button based on the
     * current selection, and tells the subscribers of its change.
     */
    updateSelectAll(): void {
        if (!this.grid)
            return;

        const selectAllButton = this.grid.getElement()
            ?.querySelector('.select-all-header .slick-column-name .select-all-items');

        if (selectAllButton) {
            const keys = Object.keys(this.include);
            selectAllButton.classList.toggle('checked',
                keys.length > 0 &&
                this.grid.getView().getItems().filter(
                    bindThis(this).isSelectable).length <= keys.length);
        }

        this.onSelectionChanged.notify(this.getSelectionState());
    }

    /**
     * Clears the checked rows, and repaints the rows, so that the check boxes and the rows that are told
     * apart by {@link checkedRowClass} are shown unchecked right away.
     */
    clear(): void {
        clearKeys(this.include);
        if (!this.grid)
            return;

        this.grid.getGrid().invalidate();
        this.updateSelectAll();
    }

    /**
     * Clears the checked rows, and refreshes the grid view, e.g. after the rows are changed, so that the
     * groups and the filters of the view are applied again, as well as the check boxes of the rows.
     */
    resetCheckedAndRefresh(): void {
        clearKeys(this.include);
        if (!this.grid)
            return;

        // the rows are rendered again by the view it populates, so it does not invalidate them as well
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
        // rows of a group or its totals are not data, so they can't be selected
        return item && !item.__nonDataRow && (
            this.options.selectable == null ||
            this.options.selectable(item));
    }

    /**
     * Whether a column is the column of the checkboxes that select the rows of this mixin, which is a
     * utility column of the grid and not a data column, e.g. for the column picker or an export that leaves
     * it out. The column is told apart by its format function, which {@link formatSelectItem} sets, rather
     * than by its id, as a columns class can declare it with a `RowSelectionColumn` attribute of the server.
     * A column of the radio buttons of {@link GridRadioSelectionMixin} has an id of its own, so it is not
     * taken for this one.
     * @param column - The column to check.
     */
    static isSelectColumn(column: Column): boolean {
        return column?.format === GridRowSelectionMixin.formatSelectItem ||
            (column?.id === GridRowSelectionMixin.selectColumnId &&
            column?.headerCssClass?.indexOf('select-all-header') >= 0);
    }

    /**
     * Creates a checkbox select column for the grid, including a select-all header.
     * @param getMixinOrOptions - A function that returns the mixin instance, or the options of the column.
     * When it is omitted, the mixin of the grid that is being rendered is used, which
     * {@link GridRowSelectionMixin.tryGetInstance} finds, so a grid that creates its mixin can just add
     * this column.
     * @param columnOptions - Optional column options to merge into the select column, when the first
     * argument is the function that returns the mixin.
     * @returns The select column definition.
     */
    static createSelectColumn(columnOptions?: Partial<Column>): Column;
    static createSelectColumn(getMixin?: () => GridRowSelectionMixin, columnOptions?: Partial<Column>): Column;
    static createSelectColumn(getMixinOrOptions?: (() => GridRowSelectionMixin) | Partial<Column>,
        columnOptions?: Partial<Column>): Column {

        // the getter is not called any more, as the format of the column finds the mixin of the grid that
        // is rendered, so a page that calls this with one keeps working as long as that is the mixin of
        // its grid; it is kept so that the call does not have to change

        const options: Partial<Column> = typeof getMixinOrOptions === "function" ?
            columnOptions : getMixinOrOptions;

        return {
            name: "[×]",
            nameFormat: () => <span class="select-all-items check-box no-float"></span>,
            toolTip: ' ',
            id: GridRowSelectionMixin.selectColumnId,
            resizable: false,
            width: 27,
            minWidth: 27,
            maxWidth: 27,
            // not a data column, so the group cell should start after it and the data rows of a
            // group should be indented after it
            skipGroupCell: true,
            headerCssClass: 'select-all-header',
            sortable: false,
            format: GridRowSelectionMixin.formatSelectItem,
            ...options
        };
    }
}
