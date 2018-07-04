
namespace Serenity {

    @Decorators.registerClass()
    @Decorators.resizable()
    @Decorators.responsive()
    export class ColumnPickerDialog extends Serenity.TemplatedDialog<any> {

        private ulVisible: JQuery;
        private ulHidden: JQuery;
        private colById: Q.Dictionary<Slick.Column>;

        public allColumns: Slick.Column[];
        public visibleColumns: string[];
        public defaultColumns: string[];
        public done: () => void;

        constructor() {
            super();

            new Serenity.QuickSearchInput(this.byId("Search"), {
                onSearch: (fld, txt, done) => {
                    txt = Q.trimToNull(txt);
                    if (txt != null)
                        txt = Select2.util.stripDiacritics(txt.toLowerCase());

                    this.element.find('li').each((x, e) => {
                        $(e).toggle(!txt || Select2.util.stripDiacritics(
                            $(e).text().toLowerCase()).indexOf(txt) >= 0);
                    });
                }
            });

            this.ulVisible = this.byId("VisibleCols");
            this.ulHidden = this.byId("HiddenCols");
        }

        public static createToolButton(grid: DataGrid<any, any>): ToolButton {
            function onClick() {
                var picker = new Serenity.ColumnPickerDialog();
                picker.allColumns = (grid as any).allColumns;
                if ((grid as any).initialSettings) {
                    var initialSettings = (grid as any).initialSettings as PersistedGridSettings;
                    if (initialSettings.columns && initialSettings.columns.length)
                        picker.defaultColumns = initialSettings.columns.map(x => x.id);
                }
                picker.visibleColumns = grid.slickGrid.getColumns().map(x => x.id);
                picker.done = () => {
                    (grid as any).allColumns = picker.allColumns;
                    var visible = picker.allColumns.filter(x => x.visible === true);
                    grid.slickGrid.setColumns(visible);
                    (grid as any).persistSettings();
                    grid.refresh();
                };

                Q.Router.dialog(grid.element, picker.element, () => "columns");
                picker.dialogOpen();
            }

            grid.element.on('handleroute.' + (grid as any).uniqueName, (e, arg) => {
                if (arg && !arg.handled && arg.route == "columns") {
                    onClick();
                }
            });

            return {
                hint: Q.text("Controls.ColumnPickerDialog.Title"),
                cssClass: "column-picker-button",
                onClick: onClick
            }
        }

        protected getDialogOptions() {
            var opt = super.getDialogOptions();
            opt.title = Q.text("Controls.ColumnPickerDialog.Title");
            opt.width = 600;
            opt.buttons = [
                {
                    text: Q.text("Controls.ColumnPickerDialog.RestoreDefaults"),
                    click: () => {
                        let liByKey: Q.Dictionary<JQuery> = {};
                        this.ulVisible.children().add(this.ulHidden.children()).each((x, e) => {
                            let el = $(e);
                            liByKey[el.data('key')] = el;
                        });

                        let last: JQuery = null;
                        for (let id of this.defaultColumns) {
                            let li = liByKey[id];
                            if (!li)
                                continue;

                            if (last == null)
                                li.prependTo(this.ulVisible);
                            else
                                li.insertAfter(last);

                            last = li;
                            let key: string = li.data('key');
                            delete liByKey[key];
                        }

                        for (let key in liByKey)
                            liByKey[key].appendTo(this.ulHidden);

                        this.updateListStates();
                    }
                },
                {
                    text: Q.text("Dialogs.OkButton"),
                    click: () => {
                        let newColumns: Slick.Column[] = [];

                        for (var col of this.allColumns)
                            col.visible = false;

                        this.visibleColumns = this.ulVisible.children().toArray().map(x => {
                            let id = $(x).data("key");
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
                        this.dialogClose()
                    }
                },
                {
                    text: Q.text("Dialogs.CancelButton"),
                    click: () => {
                        this.dialogClose()
                    }
                }
            ];

            return opt;
        }

        private getTitle(col: Slick.Column) {
            if (col.id == "__select__")
                return "[x]";

            return col.name || col.toolTip || col.id;
        }

        private allowHide(col: Slick.Column): boolean {
            return col.sourceItem == null || col.sourceItem.allowHide == null || col.sourceItem.allowHide;
        }

        private createLI(col: Slick.Column): JQuery {
            var allowHide = this.allowHide(col);
            return $(`
<li data-key="${col.id}" class="${allowHide ? "" : "cant-hide"}">
  <span class="drag-handle">☰</span>
  ${ Q.htmlEncode(this.getTitle(col)) }
  ${ allowHide ? `<i class="js-hide" title="${ Q.text("Controls.ColumnPickerDialog.HideHint") }">✖</i>` : '' }
  <i class="js-show fa fa-eye" title="${ Q.text("Controls.ColumnPickerDialog.ShowHint") }"></i>
</li>`);
        }

        private updateListStates() {
            this.ulVisible.children().removeClass("bg-info").addClass("bg-success");
            this.ulHidden.children().removeClass("bg-success").addClass("bg-info");
        }

        protected setupColumns(): void {
            this.allColumns = this.allColumns || [];
            this.visibleColumns = this.visibleColumns || [];

            let visible: Q.Dictionary<boolean> = {};
            for (let id of this.visibleColumns) {
                visible[id] = true;
            }

            this.colById = {};
            for (let c of this.allColumns) {
                this.colById[c.id] = c;
            }

            if (this.defaultColumns == null)
                this.defaultColumns = this.visibleColumns.slice(0);

            let hidden: Slick.Column[] = [];

            for (let c of this.allColumns) {
                if (!visible[c.id] && (!c.sourceItem ||
                    (c.sourceItem.filterOnly !== true &&
                        (c.sourceItem.readPermission == null || Q.Authorization.hasPermission(c.sourceItem.readPermission))))) {
                    hidden.push(c);
                }
            }

            let hiddenColumns = hidden.sort((a, b) => Q.turkishLocaleCompare(this.getTitle(a), this.getTitle(b)));

            for (let id of this.visibleColumns) {
                var c = this.colById[id];
                if (!c)
                    continue;

                this.createLI(c).appendTo(this.ulVisible);
            }

            for (let c of hiddenColumns) {
                this.createLI(c).appendTo(this.ulHidden);
            }

            this.updateListStates();

            if (typeof Sortable !== "undefined" &&
                Sortable.create) {

                Sortable.create(this.ulVisible[0], {
                    group: this.uniqueName + "_group",
                    filter: '.js-hide',
                    onFilter: evt => {
                        $(evt.item).appendTo(this.ulHidden);
                        this.updateListStates();
                    },
                    onMove: x => {
                        if ($(x.dragged).hasClass('cant-hide') &&
                            x.from == this.ulVisible[0] &&
                            x.to !== x.from)
                            return false;
                        return true;
                    },
                    onEnd: evt => this.updateListStates()
                });

                Sortable.create(this.ulHidden[0], {
                    group: this.uniqueName + "_group",
                    sort: false,
                    filter: '.js-show',
                    onFilter: evt => {
                        $(evt.item).appendTo(this.ulVisible);
                        this.updateListStates();
                    },
                    onEnd: evt => this.updateListStates()
                });
            }
        }

        protected onDialogOpen(): void {
            super.onDialogOpen();
            this.element.find("input").removeAttr("disabled")
            this.element.closest('.ui-dialog').children(".ui-dialog-buttonpane")
                .find('button').eq(0).addClass("restore-defaults")
                .next().focus();

            this.setupColumns();
            Q.centerDialog(this.element);
        }

        protected getTemplate() {
            return `
<div class="search"><input id="~_Search" type="text" disabled /></div>
<div class="columns-container">
<div class="column-list visible-list bg-success">
  <h5><i class="fa fa-eye"></i> ${ Q.text("Controls.ColumnPickerDialog.VisibleColumns") }</h5>
  <ul id="~_VisibleCols"></ul>
</div>
<div class="column-list hidden-list bg-info">
  <h5><i class="fa fa-list"></i> ${ Q.text("Controls.ColumnPickerDialog.HiddenColumns") }</h5>
  <ul id="~_HiddenCols"></ul>
</div>
</div>`;
        }
    }
}