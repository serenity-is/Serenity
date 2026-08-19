import { Column } from "@serenity-is/sleekgrid";
import { classTypeInfo, nsSerenity, registerType } from "../../base";
import { clearKeys } from "../../compat";
import { IDataGrid } from "../datagrid/idatagrid";

/**
 * Options for the {@link GridRadioSelectionMixin}.
 */
export interface GridRadioSelectionMixinOptions {
    /**
     * A function that determines whether an item can be selected.
     */
    selectable?: (item: any) => boolean;
}

/**
 * A mixin that adds single (radio) row selection behavior to a data grid.
 */
export class GridRadioSelectionMixin {

    static [Symbol.typeInfo] = classTypeInfo(nsSerenity); static { registerType(this) }

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

        grid.getGrid().onClick.subscribe((e, p) => {
            if ((e.target as HTMLElement).classList.contains('rad-select-item')) {
                e.preventDefault();
                var item = grid.getView().getItem(p.row);

                if (!this.isSelectable(item)) {
                    return;
                }

                var id = item[this.idField].toString();

                var wasSelected = this.include[id] == true;
                clearKeys(this.include);
                if (!wasSelected)
                    this.include[id] = true;

                for (var i = 0; i < (grid.getView() as any).getLength(); i++) {
                    grid.getGrid().updateRow(i);
                }
            }
        });
    }

    private isSelectable(item: any) {
        return item && (
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
        var items = Object.keys(this.include);
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
        var items = Object.keys(this.include).map(function (x) {
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
        var items = Object.keys(this.include).map(function (x) {
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
            name: '[×]',
            nameFormat: () => "",
            toolTip: ' ',
            id: '__select__',
            resizable: false,
            width: 27,
            minWidth: 27,
            maxWidth: 27,
            headerCssClass: '',
            sortable: false,
            format: function (ctx) {
                const mixin = getMixin();
                if (!mixin || !mixin.isSelectable(ctx.item)) {
                    return '';
                }

                const isChecked = mixin.include[ctx.item[mixin.idField]];
                return <input type="radio" name="radio-selection-group" class="rad-select-item no-float"
                    style={{ cursor: "pointer" }} checked={isChecked} />;
            }
        };
    }
}