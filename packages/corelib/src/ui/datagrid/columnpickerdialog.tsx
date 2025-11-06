import { Column } from "@serenity-is/sleekgrid";
import { Authorization, ColumnPickerDialogTexts, Culture, Dialog, DialogButton, Fluent, cancelDialogButton, faIcon, nsSerenity, okDialogButton } from "../../base";
import { Router } from "../../compat";
import { ResizableAttribute } from "../../types";
import { BaseDialog } from "../dialogs/basedialog";
import { ToolButton } from "../widgets/toolbar";
import { IDataGrid } from "./idatagrid";
import type { PersistedGridSettings } from "./datagrid-persistence";

export interface ColumnPickerDialogOptions {
    columns: Column[];
    defaultColumns: string[];
}

export class ColumnPickerDialog<P extends ColumnPickerDialogOptions = ColumnPickerDialogOptions> extends BaseDialog<P> {

    static override[Symbol.typeInfo] = this.registerClass(nsSerenity, [new ResizableAttribute()]);

    declare private ulVisible: HTMLUListElement;
    declare private ulHidden: HTMLUListElement;
    declare private colById: { [key: string]: Column };

    declare private visibleColumns: string[];
    declare public done: (newColumns: string[]) => void;

    constructor(opt: P) {
        super(opt);

        this.options.columns ??= [];
        this.visibleColumns = this.options.columns.filter(x => x.visible !== false).map(x => x.id);
        this.options.defaultColumns ??= this.visibleColumns.slice(0);
    }

    protected override renderContents(): any {
        this.dialogTitle = ColumnPickerDialogTexts.Title;

        return (
            <div class="columns-container">
                <div class="column-list visible-list bg-success">
                    <h5>
                        <i class={faIcon("eye")} />
                        {" "}
                        {ColumnPickerDialogTexts.VisibleColumns}
                    </h5>
                    <ul ref={ref => this.ulVisible = ref} />
                </div>
                <div class="column-list hidden-list bg-info">
                    <h5>
                        <i class={faIcon("eye-slash")} />
                        {" "}
                        {ColumnPickerDialogTexts.HiddenColumns}
                    </h5>
                    <ul ref={ref => this.ulHidden = ref} />
                </div>
            </div>
        );
    }

    public static createToolButton(grid: IDataGrid): ToolButton {
        function onClick() {
            ColumnPickerDialog.openDialog({ grid });
        }

        (grid as any).element.on('handleroute.' + (grid as any).uniqueName, (e: any, arg: any) => {
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

    protected override getDialogButtons(): DialogButton[] {
        return [
            {
                text: ColumnPickerDialogTexts.RestoreDefaults,
                cssClass: "btn btn-secondary restore-defaults",
                click: () => {
                    let liByKey: { [key: string]: HTMLElement } = {};
                    Array.from(this.ulVisible.childNodes).concat(Array.from(this.ulHidden.childNodes))
                        .forEach((el: HTMLElement) => {
                            liByKey[el.dataset.key] = el;
                        });

                    let last: HTMLElement = null;
                    for (let id of (this.options.defaultColumns || this.visibleColumns)) {
                        let li = liByKey[id];
                        if (!li)
                            continue;

                        if (last == null)
                            this.ulVisible.prepend(li);
                        else
                            Fluent(li).insertAfter(last);

                        last = li;
                        let key: string = li.dataset.key;
                        delete liByKey[key];
                    }

                    for (let key in liByKey)
                        this.ulHidden.append(liByKey[key]);

                    this.updateListStates();
                }
            },
            okDialogButton({
                click: () => {
                    this.visibleColumns = Array.from(this.ulVisible.childNodes).map((x: HTMLElement) => x.dataset.key);
                    this.done && this.done(this.visibleColumns);
                }
            }),
            cancelDialogButton()
        ];
    }

    protected override getDialogOptions() {
        var opt = super.getDialogOptions();
        opt.width = 600;
        return opt;
    }

    private getTitle(col: Column) {
        if (col.id == "__select__")
            return "[x]";

        return col.name || col.toolTip || col.id;
    }

    private allowHide(col: Column): boolean {
        return col.sourceItem == null || col.sourceItem.allowHide == null || col.sourceItem.allowHide;
    }

    private createLI(col: Column): HTMLElement {
        var allowHide = this.allowHide(col);
        return (
            <li class={!allowHide && "cant-hide"} data-key={col.id} >
                <span class="drag-handle">☰</span>
                {this.getTitle(col)}
                {allowHide && <i class={["js-hide", faIcon("eye-slash")]} title={ColumnPickerDialogTexts.HideHint} />}
                <i class={["js-show", faIcon("eye")]} title={ColumnPickerDialogTexts.ShowHint} />
            </li> as HTMLElement);
    }

    private updateListStates() {
        this.ulVisible.childNodes.forEach((x: Element) => { x.classList.remove("bg-info"); x.classList.add("bg-success"); });
        this.ulHidden.childNodes.forEach((x: Element) => { x.classList.remove("bg-success"); x.classList.add("bg-info"); });
    }

    protected setupColumns(): void {
        this.options.columns = this.options.columns || [];
        this.visibleColumns = this.visibleColumns || [];

        let visible: { [key: string]: boolean } = {};
        for (let id of this.visibleColumns) {
            visible[id] = true;
        }

        this.colById = {};
        for (let c of this.options.columns) {
            this.colById[c.id] = c;
        }

        this.options.defaultColumns ??= this.visibleColumns.slice(0);

        let hidden: Column[] = [];

        for (let c of this.options.columns) {
            if (!visible[c.id] && (!c.sourceItem ||
                (c.sourceItem.filterOnly !== true &&
                    (c.sourceItem.readPermission == null || Authorization.hasPermission(c.sourceItem.readPermission))))) {
                hidden.push(c);
            }
        }

        let hiddenColumns = hidden.sort((a, b) => Culture.stringCompare(this.getTitle(a), this.getTitle(b)));

        for (let id of this.visibleColumns) {
            var c = this.colById[id];
            if (!c)
                continue;

            this.ulVisible.append(this.createLI(c));
        }

        for (let c of hiddenColumns) {
            this.ulHidden.append(this.createLI(c));
        }

        this.updateListStates();

        // @ts-ignore
        if (typeof Sortable !== "undefined" && Sortable.create) {

            // @ts-ignore
            Sortable.create(this.ulVisible, {
                group: this.uniqueName + "_group",
                filter: '.js-hide',
                onFilter: (evt: Event & { item: HTMLElement }) => {
                    this.ulHidden.append(evt.item);
                    this.updateListStates();
                },
                onMove: (x: Event & { dragged: HTMLElement, from: HTMLElement, to: HTMLElement }) => {
                    if (x.dragged.classList.contains('cant-hide') &&
                        x.from == this.ulVisible &&
                        x.to !== x.from)
                        return false;
                    return true;
                },
                onEnd: (evt: any) => this.updateListStates()
            });

            // @ts-ignore
            Sortable.create(this.ulHidden, {
                group: this.uniqueName + "_group",
                sort: false,
                filter: '.js-show',
                onFilter: (evt: Event & { item: HTMLElement }) => {
                    this.ulVisible.append(evt.item);
                    this.updateListStates();
                },
                onEnd: () => this.updateListStates()
            });
        }
    }

    protected override onDialogOpen(): void {
        this.setupColumns();

        super.onDialogOpen();

        var restoreButton = Dialog.getInstance(this.domNode)?.getFooterNode()?.querySelector(".restore-defaults");
        if (restoreButton)
            (restoreButton.nextElementSibling as HTMLElement)?.focus?.();

    }

    static openDialog({ grid }: { grid: IDataGrid }) {
        let defaultColumns: string[];
        if ((grid as any).initialSettings) {
            const initialSettings = (grid as any).initialSettings as PersistedGridSettings;
            if (initialSettings.columns && initialSettings.columns.length)
                defaultColumns = initialSettings.columns.map(x => x.id);
        }
        var picker = new ColumnPickerDialog({
            columns: grid.getGrid().getColumns(true),
            defaultColumns
        });
        picker.done = (newColumns) => {
            grid.getGrid().setVisibleColumns(newColumns);
            Promise.resolve((grid as any).persistSettings()).then(() => grid.getView().populate());
        };

        Router && Router.dialog && Router.dialog((grid as any).element, picker.domNode, () => "columns");
        picker.dialogOpen();
    }
}
