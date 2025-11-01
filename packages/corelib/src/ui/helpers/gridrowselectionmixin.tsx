import { bindThis } from "@serenity-is/sleekdom";
import { Column } from "@serenity-is/sleekgrid";
import { classTypeInfo, Fluent, nsSerenity, registerType } from "../../base";
import { clearKeys } from "../../compat";
import type { IRemoteView } from "../../slick/iremoteview";
import { IDataGrid } from "../datagrid/idatagrid";

export interface GridRowSelectionMixinOptions {
    selectable?: (item: any) => boolean;
}

export class GridRowSelectionMixin {

    static [Symbol.typeInfo] = classTypeInfo(nsSerenity); static { registerType(this); }

    declare private idField: string;
    declare private include: { [key: string]: boolean }
    declare private grid: IDataGrid;
    declare private options: GridRowSelectionMixinOptions;

    constructor(grid: IDataGrid, options?: GridRowSelectionMixinOptions) {

        this.include = {};
        this.grid = grid;
        this.idField = grid.getView().getIdPropertyName();
        this.options = options || {};
        const boundThis = bindThis(this);
        grid.getGrid().onClick.subscribe(boundThis.handleGridClick);
        grid.getGrid().onHeaderClick.subscribe(boundThis.handleHeaderClick);
        (grid.getView() as IRemoteView).onRowsChanged?.subscribe(boundThis.updateSelectAll);
    }

    destroy(): void {
        this.include = {};
        this.grid?.getGrid()?.onClick?.unsubscribe(this.handleGridClick);
        this.grid?.getGrid()?.onHeaderClick?.unsubscribe(this.handleHeaderClick);
        (this.grid?.getView() as IRemoteView).onRowsChanged?.unsubscribe(this.updateSelectAll);
        delete this.grid;
        delete this.options?.selectable;
        this.options = null;
    }

    private handleGridClick(e: MouseEvent): void {
        if (!(e.target as HTMLElement).classList.contains('select-item'))
            return;
        const grid = this.grid;
        e.preventDefault();
        var item = grid.getView().getItem(p.row);
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

    private handleHeaderClick(e: MouseEvent): void {
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

    clear(): void {
        clearKeys(this.include);
        this.updateSelectAll();
    }

    resetCheckedAndRefresh(): void {
        this.include = {};
        this.updateSelectAll();
        this.grid.getView().populate();
    }

    selectKeys(keys: string[]): void {
        for (var k of keys) {
            this.include[k] = true;
        }

        this.updateSelectAll();
    }

    getSelectedKeys(): string[] {
        return Object.keys(this.include);
    }

    getSelectedAsInt32(): number[] {
        return Object.keys(this.include).map(function (x) {
            return parseInt(x, 10);
        });
    }

    getSelectedAsInt64(): number[] {
        return Object.keys(this.include).map(function (x) {
            return parseInt(x, 10);
        });
    }

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

    static createSelectColumn(getMixin: () => GridRowSelectionMixin): Column {
        return {
            name: "[×]",
            nameFormat: () => <span class="select-all-items check-box no-float"></span>,
            toolTip: ' ',
            id: '__select__',
            width: 27,
            minWidth: 27,
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
            }
        };
    }
}
