import { Column, type ArgsCell, type FormatterContext, type FormatterResult, type ISleekGrid } from "@serenity-is/sleekgrid";
import { classTypeInfo, nsSerenity, registerType } from "../../base";
import { clearKeys } from "../../compat";
import { IDataGrid } from "../datagrid/idatagrid";
import { GridRowSelectionMixin } from "./gridrowselectionmixin";
import { UtilityColumns } from "./utilitycolumns";

/**
 * Options for the {@link GridRadioSelectionMixin}.
 */
export interface GridRadioSelectionMixinOptions {
    /**
     * A function that determines whether an item can be selected.
     */
    selectable?: (item: any) => boolean;
}

/** The mixin of each grid, so that its column and the widgets that act on its checked rows can find it. */
const instances = new WeakMap<ISleekGrid, GridRadioSelectionMixin>();

/**
 * A mixin that adds single (radio) row selection behavior to a data grid.
 */
export class GridRadioSelectionMixin {

    static [Symbol.typeInfo] = classTypeInfo(nsSerenity); static { registerType(this) }

    /** The id of the column that shows the radio buttons of the rows, which the mixin creates. */
    static get radioColumnId() { return UtilityColumns.radioId; }

    /**
     * Formats a cell of the column of the radio buttons that select a row, and is also what tells that
     * column apart from the column of the check boxes, see {@link GridRowSelectionMixin.formatSelectItem},
     * which has an id of its own, `__select__`, as this one has {@link radioColumnId}.
     * @param ctx - The formatter context.
     * @returns The radio button of the row, or an empty string for a row that has none.
     */
    static formatRadioItem(ctx: FormatterContext): FormatterResult {
        const mixin = GridRadioSelectionMixin.tryGetInstance(ctx.grid);
        if (!mixin || !mixin.isSelectable(ctx.item))
            return '';

        const isChecked = mixin.include[ctx.item[mixin.idField]];
        return <input type="radio" name="radio-selection-group" class="rad-select-item no-float"
            style={{ cursor: "pointer" }} checked={isChecked} />;
    }

    /**
     * Returns the mixin attached to a grid, if it has one.
     * @param grid - The data grid, or the sleek grid of it.
     * @returns The mixin, or `null` when the grid has none.
     */
    static tryGetInstance(grid: IDataGrid | ISleekGrid): GridRadioSelectionMixin | null {
        const sleekGrid = (grid as IDataGrid)?.getGrid?.() ?? grid as ISleekGrid;
        return sleekGrid ? (instances.get(sleekGrid) ?? null) : null;
    }

    declare private idField: string;
    declare private include: { [key: string]: boolean };
    declare private grid: IDataGrid;
    declare private options: GridRadioSelectionMixinOptions;

    /**
     * Creates a new GridRadioSelectionMixin for the given grid.
     * @param grid - The data grid to attach the mixin to.
     * @param options - Optional mixin options.
     */
    constructor(grid: IDataGrid, options?: GridRadioSelectionMixinOptions) {

        this.include = Object.create(null);
        this.grid = grid;
        this.idField = grid.getView().getIdPropertyName();
        this.options = options || {};
        instances.set(grid.getGrid(), this);

        this.clickHandler = (e: Event, p: Partial<ArgsCell>) => {
            if ((e.target as HTMLElement).classList.contains('rad-select-item')) {
                e.preventDefault();
                const item = grid.getView().getItem(p.row);

                if (!this.isSelectable(item)) {
                    return;
                }

                const id = item[this.idField].toString();

                const wasSelected = this.include[id] == true;
                clearKeys(this.include);
                if (!wasSelected)
                    this.include[id] = true;

                for (let i = 0; i < (grid.getView() as any).getLength(); i++) {
                    grid.getGrid().updateRow(i);
                }
            }
        };

        grid.getGrid().onClick.subscribe(this.clickHandler);
    }

    /** The handler of the clicks of the radio buttons, which is removed when the mixin is destroyed. */
    declare private clickHandler: (e: Event, p: Partial<ArgsCell>) => void;

    /**
     * Detaches the mixin from the grid and cleans up event subscriptions.
     */
    destroy(): void {
        this.include = Object.create(null);
        if (this.clickHandler) {
            this.grid?.getGrid()?.onClick?.unsubscribe(this.clickHandler);
            this.clickHandler = null;
        }

        instances.delete(this.grid?.getGrid());
        delete this.grid;
        delete this.options?.selectable;
        this.options = null;
    }

    private isSelectable(item: any) {
        // rows of a group or its totals are not data, so they can't be selected
        return item && !item.__nonDataRow && (
            this.options.selectable == null ||
            this.options.selectable(item));
    }

    /**
     * Clears the current selection.
     */
    clear(): void {
        clearKeys(this.include);
    }

    /**
     * Clears the current selection and refreshes the grid view.
     */
    resetCheckedAndRefresh(): void {
        this.include = Object.create(null);
        this.grid.getView().populate();
    }

    /**
     * Returns the key of the currently selected item, or null if none is selected.
     * @returns The selected key, or null.
     */
    getSelectedKey(): string {
        const items = Object.keys(this.include);
        if (items != null && items.length > 0) {
            return items[0];
        }

        return null;
    }

    /**
     * Returns the selected key parsed as a 32-bit integer, or null if none is selected.
     * @returns The selected key as an int32, or null.
     */
    getSelectedAsInt32(): number | null {
        const items = Object.keys(this.include).map(function (x) {
            return parseInt(x, 10);
        });

        if (items != null && items.length > 0) {
            return items[0];
        }

        return null;
    }

    /**
     * Returns the selected key parsed as a 64-bit integer, or null if none is selected.
     * @returns The selected key as an int64, or null.
     */
    getSelectedAsInt64(): number | null {
        const items = Object.keys(this.include).map(function (x) {
            return parseInt(x, 10);
        });

        if (items != null && items.length > 0) {
            return items[0];
        }

        return null;
    }

    /**
     * Selects the item with the given key, clearing any previous selection.
     * @param key - The key of the item to select.
     */
    setSelectedKey(key: string): void {
        this.clear();
        this.include[key] = true;
    }

    /**
     * Creates a radio select column for the grid.
     * @param getMixin - A function that returns the mixin instance.
     * @param columnOptions - Optional column options to merge into the select column.
     * @returns The select column definition.
     */
    static createSelectColumn(getMixin: () => GridRadioSelectionMixin, columnOptions?: Partial<Column>): Column {
        return {
            name: '(o)',
            nameFormat: () => "",
            toolTip: ' ',
            id: UtilityColumns.radioId,
            resizable: false,
            width: 27,
            minWidth: 27,
            maxWidth: 27,
            // not a data column, so the group cell should start after it and the data rows of a
            // group should be indented after it
            skipGroupCell: true,
            headerCssClass: '',
            sortable: false,
            format: GridRadioSelectionMixin.formatRadioItem
        };
    }
}
