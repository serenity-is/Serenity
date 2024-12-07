import { Column } from "@serenity-is/sleekgrid";
import { Authorization, Culture, Dialog, DialogButton, Fluent, cancelDialogButton, faIcon, localText, okDialogButton } from "../../base";
import { Router } from "../../compat";
import { Decorators } from "../../types/decorators";
import { BaseDialog } from "../dialogs/basedialog";
import { ToolButton } from "../widgets/toolbar";
import { IDataGrid } from "./idatagrid";

@Decorators.registerClass('Serenity.ColumnPickerDialog')
@Decorators.resizable()
@Decorators.responsive()
export class ColumnPickerDialog<P = {}> extends BaseDialog<P> {

    declare private ulVisible: HTMLUListElement;
    declare private ulHidden: HTMLUListElement;
    declare private colById: { [key: string]: Column };

    declare public allColumns: Column[];
    declare public visibleColumns: string[];
    declare public defaultColumns: string[];
    declare public done: () => void;

    protected renderContents(): any {
        this.dialogTitle = localText("Controls.ColumnPickerDialog.Title");

        return (
            <div class="columns-container">
                <div class="column-list visible-list bg-success">
                    <h5>
                        <i class={faIcon("eye")} />
                        {" "}
                        {localText(localText("Controls.ColumnPickerDialog.VisibleColumns"))}
                    </h5>
                    <ul ref={ref => this.ulVisible = ref} />
                </div>
                <div class="column-list hidden-list bg-info">
                    <h5>
                        <i class={faIcon("eye-slash")} />
                        {" "}
                        {localText(localText("Controls.ColumnPickerDialog.HiddenColumns"))}
                    </h5>
                    <ul ref={ref => this.ulHidden = ref} />
                </div>
            </div>
        );
    }

    public static createToolButton(grid: IDataGrid): ToolButton {
        function onClick() {
            var picker = new ColumnPickerDialog({});
            picker.allColumns = (grid as any).allColumns;
            if ((grid as any).initialSettings) {
                var initialSettings = (grid as any).initialSettings;
                if (initialSettings.columns && initialSettings.columns.length)
                    picker.defaultColumns = initialSettings.columns.map((x: any) => x.id);
            }
            picker.visibleColumns = grid.getGrid().getColumns().map(x => x.id);
            picker.done = () => {
                (grid as any).allColumns = picker.allColumns;
                var visible = picker.allColumns.filter(x => x.visible === true);
                grid.getGrid().setColumns(visible);
                Promise.resolve((grid as any).persistSettings()).then(() => grid.getView().populate());
            };

            Router && Router.dialog && Router.dialog((grid as any).element, picker.domNode, () => "columns");
            picker.dialogOpen();
        }

        (grid as any).element.on('handleroute.' + (grid as any).uniqueName, (e: any, arg: any) => {
            if (arg && !arg.handled && arg.route == "columns") {
                onClick();
            }
        });

        return {
            hint: localText("Controls.ColumnPickerDialog.Title"),
            action: 'column-picker',
            cssClass: "column-picker-button",
            icon: faIcon("th-list", "blue"),
            onClick: onClick
        }
    }

    protected getDialogButtons(): DialogButton[] {
        return [
            {
                text: localText("Controls.ColumnPickerDialog.RestoreDefaults"),
                cssClass: "btn btn-secondary restore-defaults",
                click: () => {
                    let liByKey: { [key: string]: HTMLElement } = {};
                    Array.from(this.ulVisible.childNodes).concat(Array.from(this.ulHidden.childNodes))
                        .forEach((el: HTMLElement) => {
                            liByKey[el.dataset.key] = el;
                        });

                    let last: HTMLElement = null;
                    for (let id of this.defaultColumns) {
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
                    let newColumns: Column[] = [];

                    for (var col of this.allColumns)
                        col.visible = false;

                    this.visibleColumns = Array.from(this.ulVisible.childNodes).map((x: HTMLElement) => {
                        let id = x.dataset.key;
                        var col = this.colById[id];
                        col.visible = true;
                        newColumns.push(col);
                        return id;
                    });

                    for (var col of this.allColumns) {
                        if (!col.visible)
                            newColumns.push(col);
                    }

                    this.allColumns = newColumns;
                    this.done && this.done();
                }
            }),
            cancelDialogButton()
        ];
    }

    protected getDialogOptions() {
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
                {allowHide && <i class={["js-hide", faIcon("eye-slash")]} title={localText("Controls.ColumnPickerDialog.HideHint")} />}
                <i class={["js-show", faIcon("eye")]} title={localText("Controls.ColumnPickerDialog.ShowHint")} />
            </li> as HTMLElement);
    }

    private updateListStates() {
        this.ulVisible.childNodes.forEach((x: Element) => { x.classList.remove("bg-info"); x.classList.add("bg-success"); });
        this.ulHidden.childNodes.forEach((x: Element) => { x.classList.remove("bg-success"); x.classList.add("bg-info"); });
    }

    protected setupColumns(): void {
        this.allColumns = this.allColumns || [];
        this.visibleColumns = this.visibleColumns || [];

        let visible: { [key: string]: boolean } = {};
        for (let id of this.visibleColumns) {
            visible[id] = true;
        }

        this.colById = {};
        for (let c of this.allColumns) {
            this.colById[c.id] = c;
        }

        if (this.defaultColumns == null)
            this.defaultColumns = this.visibleColumns.slice(0);

        let hidden: Column[] = [];

        for (let c of this.allColumns) {
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

    protected onDialogOpen(): void {
        this.setupColumns();

        super.onDialogOpen();

        var restoreButton = Dialog.getInstance(this.domNode)?.getFooterNode()?.querySelector(".restore-defaults");
        if (restoreButton)
            (restoreButton.nextElementSibling as HTMLElement)?.focus?.();

    }
}
