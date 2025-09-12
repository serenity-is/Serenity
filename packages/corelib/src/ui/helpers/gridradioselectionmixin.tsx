import { Column } from "@serenity-is/sleekgrid";
import { classTypeInfo, nsSerenity, registerType } from "../../base";
import { clearKeys } from "../../compat";
import { IDataGrid } from "../datagrid/idatagrid";

export interface GridRadioSelectionMixinOptions {
    selectable?: (item: any) => boolean;
}

export class GridRadioSelectionMixin {

    static typeInfo = classTypeInfo(nsSerenity); static { registerType(this) }

    declare private idField: string;
    declare private include: { [key: string]: boolean };
    declare private grid: IDataGrid;
    declare private options: GridRadioSelectionMixinOptions;

    constructor(grid: IDataGrid, options?: GridRadioSelectionMixinOptions) {

        this.include = {};
        this.grid = grid;
        this.idField = (grid.getView() as any).idField;
        this.options = options || {};

        grid.getGrid().onClick.subscribe((e, p) => {
            if ((e.target as HTMLElement).classList.contains('rad-select-item')) {
                e.preventDefault();
                var item = grid.getView().getItem(p.row);

                if (!this.isSelectable(item)) {
                    return;
                }

                var id = item[this.idField].toString();

                if (this.include[id] == true) {
                    clearKeys(this.include);
                }
                else {
                    clearKeys(this.include);
                    this.include[id] = true;
                }

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

    clear(): void {
        clearKeys(this.include);
    }

    resetCheckedAndRefresh(): void {
        this.include = {};
        this.grid.getView().populate();
    }

    getSelectedKey(): string {
        var items = Object.keys(this.include);
        if (items != null && items.length > 0) {
            return items[0];
        }

        return null;
    }

    getSelectedAsInt32(): number {
        var items = Object.keys(this.include).map(function (x) {
            return parseInt(x, 10);
        });

        if (items != null && items.length > 0) {
            return items[0];
        }

        return null;
    }

    getSelectedAsInt64(): number {
        var items = Object.keys(this.include).map(function (x) {
            return parseInt(x, 10);
        });

        if (items != null && items.length > 0) {
            return items[0];
        }

        return null;
    }

    setSelectedKey(key: string): void {
        this.clear();
        this.include[key] = true;
    }

    static createSelectColumn(getMixin: () => GridRadioSelectionMixin): Column {
        return {
            name: '',
            toolTip: ' ',
            field: '__select__',
            width: 27,
            minWidth: 27,
            headerCssClass: '',
            sortable: false,
            formatter: function (row, cell, value, column, item) {
                var mixin = getMixin();
                if (!mixin || !mixin.isSelectable(item)) {
                    return '';
                }

                var isChecked = mixin.include[item[mixin.idField]];
                return '<input type="radio" name="radio-selection-group" class="rad-select-item no-float" style="cursor: pointer;width: 13px; height:13px;" ' + (isChecked ? ' checked' : '') + ' /> ';
            }
        };
    }
}