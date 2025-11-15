import { bindThis } from "@serenity-is/domwise";
import { Column, type ISleekGrid } from "@serenity-is/sleekgrid";
import { ColumnPickerDialogTexts, DialogButton, faIcon, Fluent, localText, nsSerenity, tryGetText } from "../../base";
import { Router } from "../../compat/router";
import type { IRemoteView } from "../../slick";
import { Attributes } from "../../types";
import { BaseDialog } from "../dialogs/basedialog";
import { stripDiacritics } from "../editors/combobox";
import { GridUtils } from "../helpers/gridutils";
import { ToolButton } from "../widgets/toolbar";
import type { DataGrid } from "./datagrid";
import { IDataGrid } from "./idatagrid";

export type ColumnPickerChangeArgs = {
    toggledColumn: Column;
    reorderedColumns: boolean;
    restoredDefaults: boolean;
};

export interface ColumnPickerDialogOptions {
    columns?: Column[] | (() => Column[]);
    defaultOrder?: string[] | (() => string[]);
    defaultVisible?: string[] | (() => string[]);
    dataGrid?: IDataGrid;
    sleekGrid?: ISleekGrid;
    onChange?: (args: ColumnPickerChangeArgs) => Promise<any>;
    toggleColumn?: (columnId: string, show?: boolean) => void;
    reorderColumns?: (columnIds: string[], setVisible?: string[]) => void;
}

export class ColumnPickerDialog<P extends ColumnPickerDialogOptions = ColumnPickerDialogOptions> extends BaseDialog<P> {

    static override[Symbol.typeInfo] = this.registerClass(nsSerenity, [Attributes.resizable()]);

    declare private list: HTMLUListElement;
    declare private colById: { [key: string]: Column };
    declare private defaultOrder: string[];
    declare private defaultVisible: string[];
    declare private columns: Column[];
    declare private reorderColumns: (columnIds: string[], setVisible?: string[]) => void;
    declare private toggleColumn: (columnId: string, show?: boolean) => void;
    declare private toggleAllCheckbox: HTMLInputElement;
    declare private searchInput: HTMLInputElement;
    declare private onChange: (args: ColumnPickerChangeArgs) => PromiseLike<any>;

    constructor(opt: P) {
        super(opt);

        opt = this.options;

        this.columns = (typeof opt.columns === "function" ? opt.columns() : opt.columns)
            ?? opt.dataGrid?.getGrid()?.getAllColumns?.()
            ?? opt.sleekGrid?.getAllColumns?.()
            ?? [];

        this.columns = this.columns.slice(); // make a copy

        this.defaultOrder = typeof opt.defaultOrder === "function" ? opt.defaultOrder() : opt.defaultOrder;
        this.defaultVisible = typeof opt.defaultVisible === "function" ? opt.defaultVisible() : opt.defaultVisible;

        if (!this.defaultOrder || !this.defaultVisible) {
            const initialColumns = (opt.dataGrid as DataGrid<any>)?.initialSettings?.columns;
            if (initialColumns && initialColumns.length) {
                this.defaultOrder ??= initialColumns.map(x => x.id);
                // in persisted settings, visible is true for shown columns, unlike column.visible which is false for hidden ones, null or true for shown
                this.defaultVisible ??= initialColumns.filter(x => x.visible === true).map(x => x.id);
            }
        }

        this.defaultOrder ??= this.columns.map(x => x.id);
        this.defaultVisible ??= this.columns.filter(x => x.visible !== false).map(x => x.id);
        this.colById = {};
        for (let c of this.columns) {
            this.colById[c.id] = c;
        }

        const sleekGrid = opt.sleekGrid ?? opt.dataGrid?.getGrid();

        this.toggleColumn = opt.toggleColumn ?? ((columnId: string, show?: boolean) => {
            const column = this.colById[columnId];
            if (!column)
                return;
            show ??= (column?.visible === false);
            let shown = false;
            if (column.visible === false && show) {
                column.visible = true;
                shown = true;
            }
            else if (column.visible !== false && !show) {
                column.visible = false;
            }
            else
                return;
            sleekGrid?.invalidateColumns();
            this.onChange({ toggledColumn: column, reorderedColumns: false, restoredDefaults: false });
        });

        this.reorderColumns = opt.reorderColumns ?? ((columnIds: string[], setVisible?: string[]) => {
            sleekGrid?.reorderColumns(columnIds, { notify: true, setVisible });
        });

        this.onChange = opt.onChange ?? (args => {
            const persistPromise = Promise.resolve((this.options.dataGrid as any)?.persistSettings?.());
            if (args.toggledColumn?.visible !== false ||
                args.reorderedColumns ||
                args.restoredDefaults) {
                return persistPromise.then(() => {
                    const sleekGrid = this.options.sleekGrid ?? this.options.dataGrid?.getGrid();
                    const remoteView = sleekGrid?.getData?.() as IRemoteView<any>;
                    if (remoteView?.populate) {
                        remoteView.populate();
                    }
                });
            }
            return persistPromise;
        });

        this.element.on("click", ".toggle-visibility", bindThis(this).handleToggleClick);
    }

    public override destroy() {
        delete this.toggleColumn;
        delete this.colById;
        delete this.columns;
        delete this.defaultOrder;
        delete this.defaultVisible;
        delete this.onChange;
        delete this.reorderColumns;
        delete this.searchInput;
        delete this.toggleAllCheckbox;
        this.element.off("click", this.handleToggleClick);
        super.destroy();
    }

    protected handleToggleClick(e: MouseEvent) {
        const columnId = (e.target as HTMLElement)?.closest("li")?.dataset.key;
        if (!columnId)
            return;
        this.toggleColumn(columnId);

        queueMicrotask(() => { // sortablejs somehow saves checked inputs on touch start, 
            // and restores them on destroy of any sortable instance, so we need to reapply checked states
            const column = this.colById[columnId];
            if (column) {
                const input = this.list?.querySelector<HTMLInputElement>(`li[data-key='${columnId}'] input`);
                input && (input.checked = column.visible !== false);
            }

            this.updateToggleAllValue();
        });

        this.updateToggleAllValue();
    }

    protected override renderContents(): any {
        this.dialogTitle = ColumnPickerDialogTexts.Title;

        return (
            <div class="columns-container">
                <div class="search-bar d-flex align-items-center mb-2">
                    <div class="form-check">
                        <input type="checkbox" id={this.uniqueName + "_ToggleAll"} class="form-check-input toggle-all" ref={ref => this.toggleAllCheckbox = ref} onClick={bindThis(this).handleToggleAllClick} />
                    </div>
                    <div class="s-QuickSearchBar flex-grow-1" ref={bindThis(this).createSearch}>
                    </div>
                    <button id={this.uniqueName + "_RestoreDefaults"} class="btn btn-sm btn-outline-danger float-end ms-2"
                        type="button" title={ColumnPickerDialogTexts.RestoreDefaults} onClick={bindThis(this).handleRestoreDefaults}>
                        <i class={faIcon("redo")} />
                    </button>
                </div>
                <div class="column-list">
                    <ul ref={ref => this.list = ref} />
                </div>
            </div>
        );
    }

    protected createSearch(div: HTMLElement) {
        const input = GridUtils.addQuickSearchInputCustom(div, bindThis(this).handleSearch);
        input.element.attr("id", this.uniqueName + "_Search");
        this.searchInput = input.domNode;
    }

    protected handleRestoreDefaults() {
        this.reorderColumns(this.defaultOrder, this.defaultVisible);
        this.onChange({ toggledColumn: null, reorderedColumns: true, restoredDefaults: true });

        let liByKey: { [key: string]: HTMLElement } = {};
        Array.from(this.list.childNodes)
            .forEach((el: HTMLElement) => {
                liByKey[el.dataset.key] = el;
            });

        let last: HTMLElement = null;
        for (let id of this.defaultOrder) {
            let li = liByKey[id];
            if (!li)
                continue;

            if (last == null)
                this.list.prepend(li);
            else
                Fluent(li).insertAfter(last);

            last = li;
            let key: string = li.dataset.key;
            delete liByKey[key];
        }

        for (let key in liByKey)
            this.list.append(liByKey[key]);

        if (this.defaultVisible) {
            this.list.querySelectorAll<HTMLInputElement>("input.toggle-visibility").forEach(input => input.checked = false);
            for (let id of this.defaultVisible) {
                const li = this.list.querySelector<HTMLElement>(`li[data-key='${id}']`);
                if (li) {
                    const input = li.querySelector<HTMLInputElement>("input.toggle-visibility");
                    if (input) {
                        input.checked = true;
                    }
                }
            }
        }

        this.updateToggleAllValue();
    }

    protected handleToggleAllClick() {
        const show = this.toggleAllCheckbox.checked;
        this.list.querySelectorAll<HTMLInputElement>("li:not([hidden]) input.toggle-visibility:not([disabled])").forEach(input => {
            if (!!input.checked !== !!show) {
                input.checked = show;
                const li = input.closest("li");
                const colId = li?.dataset.key;
                if (colId) {
                    this.toggleColumn(colId, show);
                }
            }
        });
        this.updateToggleAllValue();
    }

    protected updateToggleAllValue(): boolean {
        const inputs = this.list.querySelectorAll<HTMLInputElement>("li:not([hidden]) input.toggle-visibility:not([disabled])");
        return this.toggleAllCheckbox.checked = Array.from(inputs).every(x => x.checked);
    }

    protected handleSearch(_field: string, query: string, done: (found: boolean) => void): void {
        query = stripDiacritics(query?.trim().toLowerCase() ?? "");
        if (query.length && !this.list.style.height) {
            this.list.style.height = this.list.offsetHeight + "px";
        }
        else if (!query.length && this.list.style.height) {
            this.list.style.height = "";
        }
        let found = false;
        for (let li of Array.from(this.list.children)) {
            const colId = (li as HTMLElement).dataset.key;
            const col = this.colById[colId];
            const title = stripDiacritics(!col ? "" : (this.getTitle(col) ?? "").toLowerCase());
            const match = title.indexOf(query) >= 0;
            (li as HTMLElement).hidden = !match;
            if (match)
                found = true;
        }
        this.updateToggleAllValue();
        done(found);
    }

    public static createToolButton(optOrDataGrid: IDataGrid | ColumnPickerDialogOptions): ToolButton {
        // for compat
        const opt = optOrDataGrid && 'getGrid' in optOrDataGrid ?
            { dataGrid: optOrDataGrid as IDataGrid } :
            optOrDataGrid as ColumnPickerDialogOptions;

        function onClick() {
            ColumnPickerDialog.openDialog(opt);
        }

        (opt.dataGrid as any)?.element.on('handleroute.' + (opt.dataGrid as any).uniqueName, (e: any, arg: any) => {
            if (arg && !arg.handled && arg.route == "columns") {
                onClick();
            }
        });

        return {
            hint: ColumnPickerDialogTexts.Title,
            action: 'column-picker',
            cssClass: "column-picker-button",
            icon: faIcon("th-list", "blue"),
            onClick: onClick
        }
    }

    protected override getDialogOptions() {
        var opt = super.getDialogOptions();
        opt.size = "sm";
        opt.width = 300;
        return opt;
    }

    protected override getDialogButtons(): DialogButton[] {
        return null;
    }

    private getTitle(col: Column) {
        if (col.id == "__select__")
            return "[x]";

        return col.name || col.toolTip || col.id;
    }

    private isTogglable(col: Column): boolean {
        return col.togglable !== false;
    }

    private isMovable(col: Column): boolean {
        return col.movable !== false;
    }

    private getPinInfo(col: Column) {
        const sleekGrid = this.options?.sleekGrid ?? this.options?.dataGrid?.getGrid?.();
        const rtl = sleekGrid?.getOptions?.()?.rtl;
        const layoutInfo = sleekGrid?.getLayoutInfo?.();
        const pinned = col.frozen && layoutInfo?.supportPinnedCols;
        let pinnedRight = pinned && layoutInfo?.supportPinnedEnd && col.frozen === "end";
        if (rtl && pinned) {
            pinnedRight = !pinnedRight;
        }
        return { pinned, pinnedRight };
    }

    private createColumnItem(col: Column): HTMLElement {
        const togglable = this.isTogglable(col);
        const movable = this.isMovable(col);
        const { pinned, pinnedRight } = this.getPinInfo(col);
        const pinText = tryGetText("Controls.ColumnHeaderMenu.Pin");
        const pinHint = (pinned && pinText) ? `${pinText}: ${pinnedRight ? localText("Controls.ColumnHeaderMenu.PinRight", "") : localText("Controls.ColumnHeaderMenu.PinLeft", "")}` : null;
        return (
            <li class={[!togglable && "cant-hide", !movable && "not-movable"]} data-key={col.id}>
                <div class="form-check">
                    <input type="checkbox" id={this.uniqueName + "_col_" + col.id} class="form-check-input toggle-visibility" disabled={!togglable} checked={col.visible !== false} />
                    <span class="drag-handle" style={{ visibility: !movable && "hidden" }}><i class={faIcon("braille")} /></span>
                    <span class="form-check-label">
                        {this.getTitle(col)}
                        {pinned ? <span class="ms-2 opacity-25" title={pinHint}>{PinImage({ right: pinnedRight })}</span> : null}
                    </span>
                </div>
            </li> as HTMLElement);
    }

    private handleSortableEnd() {
        const newOrder: string[] = Array.from(this.list.children)
            .map(li => (li as HTMLElement).dataset?.key)
            .filter(id => id != null);

        this.reorderColumns(newOrder);
        this.onChange({ toggledColumn: null, reorderedColumns: true, restoredDefaults: false });
    }

    protected createColumnItems(): void {
        const columns = this.columns;

        for (const column of columns) {
            this.list.append(this.createColumnItem(column));
        }

        this.updateToggleAllValue();

        if (typeof (globalThis as any).Sortable !== "undefined" && typeof (globalThis as any).Sortable.create !== "undefined") {

            (globalThis as any).Sortable.create(this.list, {
                handle: '.drag-handle, .form-check-label',
                filter: '.not-movable, input',
                group: this.uniqueName + "_group",
                onEnd: bindThis(this).handleSortableEnd
            });
        }
    }

    protected override onDialogOpen(): void {
        this.createColumnItems();

        super.onDialogOpen();

        this.searchInput?.focus();
    }

    static openDialog(opt: ColumnPickerDialogOptions): void {

        const picker = new ColumnPickerDialog(opt);
        picker.dialogOpen();

        if (opt.dataGrid) {
            Router?.dialog?.((opt.dataGrid as any).element, picker.domNode, () => "columns");
        }
    }
}

/** From Bootstrap icons (https://icons.getbootstrap.com/) */
const PinImage = ({ right }: { right?: boolean }) => {
    return <svg xmlns="http://www.w3.org/2000/svg" style={{ transform: right ? "rotate(270deg)" : null, width: "1rem", height: "1rem" }} fill="currentColor" class="bi bi-pin-angle" viewBox="0 0 16 16">
        <path d="M9.828.722a.5.5 0 0 1 .354.146l4.95 4.95a.5.5 0 0 1 0 .707c-.48.48-1.072.588-1.503.588-.177 0-.335-.018-.46-.039l-3.134 3.134a6 6 0 0 1 .16 1.013c.046.702-.032 1.687-.72 2.375a.5.5 0 0 1-.707 0l-2.829-2.828-3.182 3.182c-.195.195-1.219.902-1.414.707s.512-1.22.707-1.414l3.182-3.182-2.828-2.829a.5.5 0 0 1 0-.707c.688-.688 1.673-.767 2.375-.72a6 6 0 0 1 1.013.16l3.134-3.133a3 3 0 0 1-.04-.461c0-.43.108-1.022.589-1.503a.5.5 0 0 1 .353-.146m.122 2.112v-.002zm0-.002v.002a.5.5 0 0 1-.122.51L6.293 6.878a.5.5 0 0 1-.511.12H5.78l-.014-.004a5 5 0 0 0-.288-.076 5 5 0 0 0-.765-.116c-.422-.028-.836.008-1.175.15l5.51 5.509c.141-.34.177-.753.149-1.175a5 5 0 0 0-.192-1.054l-.004-.013v-.001a.5.5 0 0 1 .12-.512l3.536-3.535a.5.5 0 0 1 .532-.115l.096.022c.087.017.208.034.344.034q.172.002.343-.04L9.927 2.028q-.042.172-.04.343a1.8 1.8 0 0 0 .062.46z" />
    </svg>
}