namespace Serenity {

    

    export interface GridRowSelectionMixinOptions {
        selectable?: (item: any) => boolean;
    }

    @Decorators.registerClass('Serenity.GridRowSelectionMixin')
    export class GridRowSelectionMixin {

        private idField: string;
        private include: Q.Dictionary<boolean>;
        private grid: IDataGrid;
        private options: GridRowSelectionMixinOptions;

        constructor(grid: IDataGrid, options?: GridRowSelectionMixinOptions) {

            this.include = {};
            this.grid = grid;
            this.idField = (grid.getView() as any).idField;
            this.options = options || {};

            grid.getGrid().onClick.subscribe((e, p) => {
                if ($(e.target).hasClass('select-item')) {
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
            });

            grid.getGrid().onHeaderClick.subscribe((e1, u) => {
                if (e1.isDefaultPrevented()) {
                    return;
                }
                if ($(e1.target).hasClass('select-all-items')) {
                    e1.preventDefault();
                    var view = grid.getView();
                    if (Object.keys(this.include).length > 0) {
                        Q.clearKeys(this.include);
                    }
                    else {
                        var items = grid.getView().getItems();
                        for (var x of items.filter(this.isSelectable.bind(this))) {
                            var id1 = x[this.idField];
                            this.include[id1] = true;
                        }
                    }
                    this.updateSelectAll();
                    grid.getView().setItems(grid.getView().getItems(), true);
                }
            });

            (grid.getView() as any).onRowsChanged.subscribe(() => {
                return this.updateSelectAll();
            });
        }

        updateSelectAll(): void {
            var selectAllButton = this.grid.getElement()
                .find('.select-all-header .slick-column-name .select-all-items');

            if (selectAllButton) {
                var keys = Object.keys(this.include);
                selectAllButton.toggleClass('checked',
                    keys.length > 0 &&
                    this.grid.getView().getItems().filter(
                        this.isSelectable.bind(this)).length <= keys.length);
            }
        }

        clear(): void {
            Q.clearKeys(this.include);
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

        static createSelectColumn(getMixin: () => GridRowSelectionMixin): Slick.Column {
            return {
                name: '<span class="select-all-items check-box no-float "></span>',
                toolTip: ' ',
                field: '__select__',
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
                    return '<span class="select-item check-box no-float ' + (isChecked ? ' checked' : '') + '"></span>';
                }
            };
        }
    }

    export interface GridRadioSelectionMixinOptions {
        selectable?: (item: any) => boolean;
    }

    @Serenity.Decorators.registerClass('Serenity.GridRadioSelectionMixin')
    export class GridRadioSelectionMixin {

        private idField: string;
        private include: Q.Dictionary<boolean>;
        private grid: Serenity.IDataGrid;
        private options: GridRadioSelectionMixinOptions;

        constructor(grid: Serenity.IDataGrid, options?: GridRadioSelectionMixinOptions) {

            this.include = {};
            this.grid = grid;
            this.idField = (grid.getView() as any).idField;
            this.options = options || {};

            grid.getGrid().onClick.subscribe((e, p) => {
                if ($(e.target).hasClass('rad-select-item')) {
                    e.preventDefault();
                    var item = grid.getView().getItem(p.row);

                    if (!this.isSelectable(item)) {
                        return;
                    }

                    var id = item[this.idField].toString();

                    if (this.include[id] == true) {
                        Q.clearKeys(this.include);
                    }
                    else {
                        Q.clearKeys(this.include);
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
            Q.clearKeys(this.include);
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

        static createSelectColumn(getMixin: () => Serenity.GridRadioSelectionMixin): Slick.Column {
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

    export namespace GridSelectAllButtonHelper {
        export function update(grid: IDataGrid, getSelected: (p1: any) => boolean): void {
            var toolbar = (grid as any).element.children('.s-Toolbar') as JQuery;
            if (toolbar.length === 0) {
                return;
            }
            var btn = toolbar.getWidget(Toolbar).findButton('select-all-button');
            var items = grid.getView().getItems();
            btn.toggleClass('checked', items.length > 0 && !items.some(function (x) {
                return !getSelected(x);
            }));
        }

        export function define(getGrid: () => IDataGrid, getId: (p1: any) => any,
            getSelected: (p1: any) => boolean,
            setSelected: (p1: any, p2: boolean) => void,
            text?: string, onClick?: () => void): ToolButton {

            if (text == null) {
                text = Q.coalesce(Q.tryGetText('Controls.CheckTreeEditor.SelectAll'),
                    'Select All');
            }
            return {
                title: text,
                cssClass: 'select-all-button',
                onClick: function () {
                    var grid = getGrid();
                    var view = grid.getView();
                    var btn = (grid as any).element.children('.s-Toolbar')
                        .getWidget(Toolbar).findButton('select-all-button');
                    var makeSelected = !btn.hasClass('checked');
                    view.beginUpdate();
                    try {
                        for (var item of view.getItems()) {
                            setSelected(item, makeSelected);
                            view.updateItem(getId(item), item);
                        }
                        onClick && onClick();
                    }
                    finally {
                        view.endUpdate();
                    }

                    btn.toggleClass('checked', makeSelected);
                }
            };
        }
    }

    export namespace GridUtils {
        export function addToggleButton(toolDiv: JQuery, cssClass: string,
            callback: (p1: boolean) => void, hint: string, initial?: boolean): void {

            var div = $('<div><a href="#"></a></div>')
                .addClass('s-ToggleButton').addClass(cssClass)
                .prependTo(toolDiv);
            div.children('a').click(function (e) {
                e.preventDefault();
                div.toggleClass('pressed');
                var pressed = div.hasClass('pressed');
                callback && callback(pressed);
            }).attr('title', Q.coalesce(hint, ''));
            if (initial) {
                div.addClass('pressed');
            }
        }

        export function addIncludeDeletedToggle(toolDiv: JQuery,
            view: Slick.RemoteView<any>, hint?: string, initial?: boolean): void {

            var includeDeleted = false;
            var oldSubmit = view.onSubmit;
            view.onSubmit = function (v) {
                v.params.IncludeDeleted = includeDeleted;
                if (oldSubmit != null) {
                    return oldSubmit(v);
                }
                return true;
            };

            if (hint == null) 
                hint = Q.text('Controls.EntityGrid.IncludeDeletedToggle');
            
            addToggleButton(toolDiv, 's-IncludeDeletedToggle',
                function (pressed) {
                    includeDeleted = pressed;
                    view.seekToPage = 1;
                    view.populate();
                }, hint, initial);
            toolDiv.bind('remove', function () {
                view.onSubmit = null;
                oldSubmit = null;
            });
        }

        export function addQuickSearchInput(toolDiv: JQuery,
            view: Slick.RemoteView<any>, fields?: QuickSearchField[], onChange?: () => void): void {

            var oldSubmit = view.onSubmit;
            var input: QuickSearchInput;
            view.onSubmit = function (v) {
                if (input) {
                    var searchText = input.get_value();
                    if (searchText && searchText.length > 0) {
                        v.params.ContainsText = searchText;
                    }
                    else {
                        delete v.params['ContainsText'];
                    }
                    var searchField = input.get_field()?.name;
                    if (searchField != null && searchField.length > 0) {
                        v.params.ContainsField = searchField;
                    }
                    else {
                        delete v.params['ContainsField'];
                    }
                }

                if (oldSubmit != null) 
                    return oldSubmit(v);
                
                return true;
            };

            var lastDoneEvent: any = null;
            input = addQuickSearchInputCustom(toolDiv, (field, query, done) => {
                onChange && onChange();
                view.seekToPage = 1;
                lastDoneEvent = done;
                view.populate();
            }, fields);

            view.onDataLoaded.subscribe(function (e, ui) {
                if (lastDoneEvent != null) {
                    lastDoneEvent(view.getLength() > 0);
                    lastDoneEvent = null;
                }
            });
        }

        export function addQuickSearchInputCustom(container: JQuery,
            onSearch: (p1: string, p2: string, done: (p3: boolean) => void) => void,
            fields?: QuickSearchField[]): QuickSearchInput {

            var div = $('<div><input type="text"/></div>')
                .addClass('s-QuickSearchBar').prependTo(container);

            if (fields != null && fields.length > 0) {
                div.addClass('has-quick-search-fields');
            }

            return new QuickSearchInput(div.children(), {
                fields: fields,
                onSearch: onSearch as any
            });
        }

        export function makeOrderable(grid: Slick.Grid,
            handleMove: (p1: any, p2: number) => void): void {

            var moveRowsPlugin = new Slick.RowMoveManager({ cancelEditOnDrag: true });
            moveRowsPlugin.onBeforeMoveRows.subscribe(function (e, data) {
                for (var i = 0; !!(i < data.rows.length); i++) {
                    if (!!(data.rows[i] === data.insertBefore ||
                        data.rows[i] === data.insertBefore - 1)) {
                        e.stopPropagation();
                        return false;
                    }
                }

                return true;
            });

            moveRowsPlugin.onMoveRows.subscribe(function (e1, data1) {
                handleMove(data1.rows, data1.insertBefore);
                try {
                    grid.setSelectedRows([]);
                }
                catch ($t1) {
                }
            });
            grid.registerPlugin(moveRowsPlugin);
        }

        export function makeOrderableWithUpdateRequest(grid: DataGrid<any, any>,
            getId: (p1: any) => number, getDisplayOrder: (p1: any) => any, service: string,
            getUpdateRequest: (p1: number, p2: number) => SaveRequest<any>): void {

            makeOrderable(grid.slickGrid, function (rows, insertBefore) {
                if (rows.length === 0) {
                    return;
                }

                var order: number;
                var index = insertBefore;
                if (index < 0) {
                    order = 1;
                }
                else if (insertBefore >= grid.rowCount()) {
                    order = Q.coalesce(getDisplayOrder(
                        grid.itemAt(grid.rowCount() - 1)), 0);
                    if (order === 0) {
                        order = insertBefore + 1;
                    }
                    else {
                        order = order + 1;
                    }
                }
                else {
                    order = Q.coalesce(getDisplayOrder(
                        grid.itemAt(insertBefore)), 0);
                    if (order === 0) {
                        order = insertBefore + 1;
                    }
                }

                var i = 0;
                var next: any = null;
                next = function () {
                    Q.serviceCall({
                        service: service,
                        request: getUpdateRequest(getId(
                            grid.itemAt(rows[i])), order++),
                        onSuccess: function (response) {
                            i++;
                            if (i < rows.length) {
                                next();
                            }
                            else {
                                grid.view.populate();
                            }
                        }
                    });
                };
                next();
            });
        }
    }

    export namespace PropertyItemSlickConverter {
        export function toSlickColumns(items: PropertyItem[]): Slick.Column[] {
            var result: Slick.Column[] = [];
            if (items == null) {
                return result;
            }
            for (var i = 0; i < items.length; i++) {
                result.push(toSlickColumn(items[i]));
            }
            return result;
        }

        export function toSlickColumn(item: PropertyItem): Slick.Column {
            var result: Slick.Column = {
                field: item.name,
                sourceItem: item,
                cssClass: item.cssClass,
                headerCssClass: item.headerCssClass,
                sortable: item.sortable !== false,
                sortOrder: Q.coalesce(item.sortOrder, 0),
                width: item.width != null ? item.width : 80,
                minWidth: Q.coalesce(item.minWidth, 30),
                maxWidth: (item.maxWidth == null || item.maxWidth === 0) ? null : item.maxWidth,
                resizable: item.resizable == null || !!item.resizable
            };

            result.visible = item.visible !== false && item.filterOnly !== true &&
                (item.readPermission == null || Q.Authorization.hasPermission(item.readPermission));

            var name = Q.tryGetText(item.title);
            if (name == null)
                name = item.title;
            result.name = name;

            if (item.alignment != null && item.alignment.length > 0) {
                if (!Q.isEmptyOrNull(result.cssClass)) {
                    result.cssClass += ' align-' + item.alignment;
                }
                else {
                    result.cssClass = 'align-' + item.alignment;
                }
            }

            if (item.formatterType != null && item.formatterType.length > 0) {

                var formatterType = FormatterTypeRegistry.get(item.formatterType) as any;
                var formatter = new formatterType() as Slick.Formatter;

                if (item.formatterParams != null) {
                    ReflectionOptionsSetter.set(formatter, item.formatterParams);
                }

                var initializer = Q.safeCast(formatter, Serenity.IInitializeColumn);
                if (initializer != null) {
                    initializer.initializeColumn(result);
                }

                result.format = (ctx) => formatter.format(ctx);
            }

            return result;
        }
    }

    export namespace SlickFormatting {
        export function getEnumText(enumKey: string, name: string): string {
            return Serenity.EnumFormatter.getText(enumKey, name);
        }

        export function treeToggle<TItem>(getView: () => Slick.RemoteView<TItem>, getId: (x: TItem) => any,
            formatter: Slick.Format): Slick.Format {
            return function (ctx: Slick.FormatterContext) {
                var text = formatter(ctx);
                var view = getView();
                var indent = Q.coalesce(ctx.item._indent, 0);
                var spacer = '<span class="s-TreeIndent" style="width:' + 15 * indent + 'px"></span>';
                var id = getId(ctx.item);
                var idx = view.getIdxById(id);
                var next = view.getItemByIdx(idx + 1);
                if (next != null) {
                    var nextIndent = Q.coalesce(next._indent, 0);
                    if (nextIndent > indent) {
                        if (!!!!ctx.item._collapsed) {
                            return spacer + '<span class="s-TreeToggle s-TreeExpand"></span>' + text;
                        }
                        else {
                            return spacer + '<span class="s-TreeToggle s-TreeCollapse"></span>' + text;
                        }
                    }
                }
                return spacer + '<span class="s-TreeToggle"></span>' + text;
            };
        }

        export function date(format?: string): Slick.Format {
            if (format == null) {
                format = Q.Culture.dateFormat;
            }

            return function (ctx: Slick.FormatterContext) {
                return Q.htmlEncode(Serenity.DateFormatter.format(ctx.value, format));
            };
        }

        export function dateTime(format?: string): Slick.Format {
            if (format == null) {
                format = Q.Culture.dateTimeFormat;
            }
            return function (ctx: Slick.FormatterContext) {
                return Q.htmlEncode(Serenity.DateFormatter.format(ctx.value, format));
            };
        }

        export function checkBox(): Slick.Format {
            return function (ctx: Slick.FormatterContext) {
                return '<span class="check-box no-float ' + (!!ctx.value ? ' checked' : '') + '"></span>';
            };
        }

        export function number(format: string): Slick.Format {
            return function (ctx: Slick.FormatterContext) {
                return Serenity.NumberFormatter.format(ctx.value, format);
            };
        }

        export function getItemType(link: JQuery): string {
            return link.data('item-type');
        }

        export function getItemId(link: JQuery): string {
            var value = link.data('item-id');
            return value == null ? null : value.toString();
        }

        export function itemLinkText(itemType: string, id: any, text: any,
            extraClass: string, encode: boolean): string {
            return '<a' + (id != null ? (' href="#' + Q.replaceAll(itemType, '.', '-') +
                '/' + id + '"') : '') + ' data-item-type="' +
                Q.attrEncode(itemType) + '"' + ' data-item-id="' +
                Q.attrEncode(id) + '"' + ' class="s-EditLink s-' +
                Q.replaceAll(itemType, '.', '-') + 'Link' +
                (Q.isEmptyOrNull(extraClass) ? '' : (' ' + extraClass)) + '">' +
                (encode ? Q.htmlEncode(Q.coalesce(text, '')) : Q.coalesce(text, '')) + '</a>';
        }

        export function itemLink(itemType: string, idField: string, getText: Slick.Format,
            cssClass?: Slick.Format, encode?: boolean): Slick.Format {
            return function (ctx: Slick.FormatterContext) {
                return itemLinkText(itemType, ctx.item[idField],
                    (getText == null ? ctx.value : getText(ctx)),
                    (cssClass == null ? '' : cssClass(ctx)), encode);
            };
        }
    }

    export namespace SlickHelper {
        export function setDefaults(columns: Slick.Column[], localTextPrefix?: string): any {
            for (var col of columns) {
                col.sortable = (col.sortable != null ? col.sortable : true);
                var id = col.id;
                if (id == null) {
                    id = col.field;
                }
                col.id = id;

                if (localTextPrefix != null && col.id != null &&
                    (col.name == null || Q.startsWith(col.name, '~'))) {
                    var key = (col.name != null ? col.name.substr(1) : col.id);
                    col.name = Q.text(localTextPrefix + key);
                }

                if (col.formatter == null && col.format != null) {
                    col.formatter = convertToFormatter(col.format);
                }
                else if (col.formatter == null) {
                    col.formatter = function (row, cell, value, column, item) {
                        return Q.htmlEncode(value);
                    };
                }
            }

            return columns;
        }

        export function convertToFormatter(format: Slick.Format): Slick.ColumnFormatter {
            if (format == null) {
                return null;
            }
            else {
                return function (row: number, cell: number, value: any, column: Slick.Column, item: any) {
                    return format({
                        row: row,
                        cell: cell,
                        value: value,
                        column: column,
                        item: item
                    });
                }
            }
        }
    }

    export namespace SlickTreeHelper {
        export function filterCustom<TItem>(item: TItem, getParent: (x: TItem) => any): boolean {
            var parent = getParent(item);
            var loop = 0;
            while (parent != null) {
                if (!!parent._collapsed) {
                    return false;
                }
                parent = getParent(parent);
                if (loop++ > 1000) {
                    throw new Error(
                        'Possible infinite loop, check parents has no circular reference!');
                }
            }
            return true;
        }

        export function filterById<TItem>(item: TItem, view: Slick.RemoteView<TItem>,
            getParentId: (x: TItem) => any): boolean {
            return filterCustom(item, function (x) {
                var parentId = getParentId(x);
                if (parentId == null) {
                    return null;
                }
                return view.getItemById(parentId);
            });
        }

        export function setCollapsed<TItem>(items: TItem[], collapsed: boolean): void {
            if (items != null) {
                for (var item of items) {
                    (item as any)._collapsed = collapsed;
                }
            }
        }

        export function setCollapsedFlag<TItem>(item: TItem, collapsed: boolean): void {
            (item as any)._collapsed = collapsed;
        }

        export function setIndents<TItem>(items: TItem[], getId: (x: TItem) => any,
            getParentId: (x: TItem) => any, setCollapsed?: boolean): void {
            var depth = 0;
            var depths = {};
            for (var line = 0; line < items.length; line++) {
                var item = items[line];
                if (line > 0) {
                    var parentId = getParentId(item);
                    if (parentId != null && parentId === getId(items[line - 1])) {
                        depth += 1;
                    }
                    else if (parentId == null) {
                        depth = 0;
                    }
                    else if (parentId !== getParentId(items[line - 1])) {
                        if (depths[parentId] != null) {
                            depth = depths[parentId] + 1;
                        }
                        else {
                            depth = 0;
                        }
                    }
                }
                depths[getId(item)] = depth;
                (item as any)._indent = depth;
                if (setCollapsed != null) {
                    (item as any)._collapsed = setCollapsed;
                }
            }
        }

        export function toggleClick<TItem>(e: JQueryEventObject, row: number, cell: number,
            view: Slick.RemoteView<TItem>, getId: (x: TItem) => any): void {
            var target = $(e.target);
            if (!target.hasClass('s-TreeToggle')) {
                return;
            }
            if (target.hasClass('s-TreeCollapse') || target.hasClass('s-TreeExpand')) {
                var item = view.getItem(row);
                if (item != null) {
                    if (!!!item._collapsed) {
                        item._collapsed = true;
                    }
                    else {
                        item._collapsed = false;
                    }
                    view.updateItem(getId(item), item);
                }
                if (e.shiftKey) {
                    view.beginUpdate();
                    try {
                        setCollapsed(view.getItems(), !!item._collapsed);
                        view.setItems(view.getItems(), true);
                    }
                    finally {
                        view.endUpdate();
                    }
                }
            }
        }
    }

}