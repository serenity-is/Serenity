namespace Serenity {
    export interface IDataGrid {
        getElement(): JQuery;
        getGrid(): Slick.Grid;
        getView(): Slick.RemoteView<any>;
        getFilterStore(): Serenity.FilterStore;
    }

    @Serenity.Decorators.registerInterface('Serenity.IDataGrid')
    export class IDataGrid {
    }
}

namespace Serenity {

    @Serenity.Decorators.registerClass('Serenity.DataGrid', [IDataGrid])
    export class DataGrid<TItem, TOptions> extends Widget<TOptions> implements IDataGrid {

        protected titleDiv: JQuery;
        protected toolbar: Toolbar;
        protected filterBar: FilterDisplayBar;
        protected quickFiltersDiv: JQuery;
        protected slickContainer: JQuery;
        protected allColumns: Slick.Column[];
        protected initialSettings: PersistedGridSettings;
        protected restoringSettings: number = 0;
        private idProperty: string;
        private isActiveProperty: string;
        private localTextDbPrefix: string;
        private isDisabled: boolean;
        private submitHandlers: any;
        private rows: any;
        private slickGridOnSort: any;
        private slickGridOnClick: any;
        public view: Slick.RemoteView<TItem>;
        public slickGrid: Slick.Grid;
        public openDialogsAsPanel: boolean;

        public static defaultRowHeight: number;
        public static defaultHeaderHeight: number;
        public static defaultPersistanceStorage: SettingStorage;

        constructor(container: JQuery, options?: TOptions) {
            super(container, options);

            var self = this;

            this.element.addClass('s-DataGrid').html('');
            this.element.addClass('s-' + (ss as any).getTypeName((ss as any).getInstanceType(this)));
            this.element.addClass('require-layout').bind('layout.' + this.uniqueName, function () {
                self.layout();
            });

            this.setTitle(this.getInitialTitle());

            var buttons = this.getButtons();
            if (buttons != null) {
                this.createToolbar(buttons);
            }

            this.slickContainer = this.createSlickContainer();
            this.view = this.createView();

            this.slickGrid = this.createSlickGrid();

            if (this.enableFiltering()) {
                this.createFilterBar();
            }

            if (this.usePager()) {
                this.createPager();
            }
            this.bindToSlickEvents();
            this.bindToViewEvents();

            if (buttons != null) {
                this.createToolbarExtensions();
            }

            this.createQuickFilters();

            this.updateDisabledState();

            if (!this.isAsyncWidget()) {
                this.initialSettings = this.getCurrentSettings(null);
                this.restoreSettings(null, null);
                window.setTimeout(() => this.initialPopulate(), 0);
            }
        }

        protected attrs<TAttr>(attrType: { new(...args: any[]): TAttr }): TAttr[] {
            return (ss as any).getAttributes((ss as any).getInstanceType(this), attrType, true);
        }

        protected add_submitHandlers(action: () => void): void {
            this.submitHandlers = (ss as any).delegateCombine(this.submitHandlers, action);
        }

        protected remove_submitHandlers(action: () => void): void {
            this.submitHandlers = (ss as any).delegateRemove(this.submitHandlers, action);
        }

        protected layout(): void {
            if (!this.element.is(':visible')) {
                return;
            }

            if (this.slickContainer == null) {
                return;
            }

            Q.layoutFillHeight(this.slickContainer);

            if (this.element.hasClass('responsive-height')) {
                if (this.slickGrid != null && this.slickGrid.getOptions().autoHeight) {
                    this.slickContainer.children('.slick-viewport').css('height', 'auto');
                    this.slickGrid.setOptions({ autoHeight: false });
                }
                if (this.slickGrid != null && (this.slickContainer.height() < 200 || $(window.window).width() < 768)) {
                    this.element.css('height', 'auto');
                    this.slickContainer.css('height', 'auto').children('.slick-viewport').css('height', 'auto');
                    this.slickGrid.setOptions({ autoHeight: true });
                }
            }

            if (this.slickGrid != null) {
                this.slickGrid.resizeCanvas();
                this.slickGrid.invalidate();
            }
        }

        protected getInitialTitle(): string {
            return null;
        }

        protected createToolbarExtensions(): void {
        }

        protected createQuickFilters(): void {
            var filters = this.getQuickFilters();
            for (var f = 0; f < filters.length; f++) {
                var filter = filters[f];
                this.addQuickFilter(filter);
            }
        }

        protected getQuickFilters(): QuickFilter<Widget<any>, any>[] {
            var list = [];

            var columns = this.allColumns.filter(function (x) {
                return x.sourceItem && x.sourceItem.quickFilter === true;
            });

            for (var column of columns) {
                var item = column.sourceItem;
                var quick: any = {};

                var name = item.name;
                var title = Q.tryGetText(item.title);
                if (title == null) {
                    title = item.title;
                    if (title == null) {
                        title = name;
                    }
                }

                var filteringType = Serenity.FilteringTypeRegistry.get(Q.coalesce(item.filteringType, 'String'));
                if (filteringType === Serenity.DateFiltering) {
                    quick = this.dateRangeQuickFilter(name, title);
                }
                else if (filteringType === Serenity.DateTimeFiltering) {
                    quick = this.dateTimeRangeQuickFilter(name, title);
                }
                else if (filteringType === Serenity.BooleanFiltering) {
                    var q = item.quickFilterParams || {};
                    var f = item.filteringParams || {};
                    var trueText = q['trueText'];
                    if (trueText == null) {
                        trueText = f['trueText'];
                    }
                    var falseText = q['falseText'];
                    if (falseText == null) {
                        falseText = f['falseText'];
                    }
                    quick = this.booleanQuickFilter(name, title, trueText, falseText);
                }
                else {
                    var filtering = new (filteringType as any)() as IFiltering;
                    if (filtering && (ss as any).isInstanceOfType(filtering, Serenity.IQuickFiltering)) {
                        Serenity.ReflectionOptionsSetter.set(filtering, item.filteringParams);
                        filtering.set_field(item);
                        filtering.set_operator({ key: Serenity.FilterOperators.EQ });
                        (filtering as any).initQuickFilter(quick);
                        quick.options = Q.deepClone(quick.options, item.quickFilterParams);
                    }
                    else {
                        continue;
                    }
                }

                if (!!item.quickFilterSeparator) {
                    quick.separator = true;
                }

                quick.cssClass = item.quickFilterCssClass;
                list.push(quick);
            }

            return list;
        }

        protected findQuickFilter<TWidget>(type: { new(...args: any[]): TWidget }, field: string): TWidget {
            return $('#' + this.uniqueName + '_QuickFilter_' + field).getWidget(type);
        }

        protected tryFindQuickFilter<TWidget>(type: { new(...args: any[]): TWidget }, field: string): TWidget {
            var el = $('#' + this.uniqueName + '_QuickFilter_' + field);
            if (!el.length)
                return null;

            return el.tryGetWidget(type);
        }

        protected createIncludeDeletedButton(): void {
            if (!Q.isEmptyOrNull(this.getIsActiveProperty())) {
                Serenity.GridUtils.addIncludeDeletedToggle(this.toolbar.element, this.view, null, false);
            }
        }

        protected getQuickSearchFields(): QuickSearchField[] {
            return null;
        }

        protected createQuickSearchInput(): void {
            Serenity.GridUtils.addQuickSearchInput(this.toolbar.element, this.view, this.getQuickSearchFields());
        }

        public destroy() {
            this.submitHandlers = null;
            if (this.toolbar) {
                this.toolbar.destroy();
                this.toolbar = null;
            }

            if (this.slickGrid) {
                this.slickGrid.onClick.clear();
                this.slickGrid.onSort.clear();
                this.slickGrid.onColumnsResized.clear();
                this.slickGrid.onColumnsReordered.clear();
                this.slickGrid.destroy();
                this.slickGrid = null;
            }

            if (this.view) {
                this.view.onDataChanged.clear();
                this.view.onSubmit = null;
                this.view.setFilter(null);
                this.view = null;
            }

            this.titleDiv = null;
            super.destroy();
        }

        protected getItemCssClass(item: TItem, index: number): string {
            var activeFieldName = this.getIsActiveProperty();
            if (Q.isEmptyOrNull(activeFieldName)) {
                return null;
            }
            var value = item[activeFieldName];
            if (value == null) {
                return null;
            }

            if (typeof (value) === 'number') {
                if (value < 0) {
                    return 'deleted';
                }
                else if (value === 0) {
                    return 'inactive';
                }
            }
            else if (typeof (value) === 'boolean') {
                if (value === false) {
                    return 'deleted';
                }
            }

            return null;
        }

        protected getItemMetadata(item: TItem, index: number): any {
            var itemClass = this.getItemCssClass(item, index);
            if (Q.isEmptyOrNull(itemClass)) {
                return new Object();
            }
            return { cssClasses: itemClass };
        }

        protected postProcessColumns(columns: Slick.Column[]): Slick.Column[] {
            Serenity.SlickHelper.setDefaults(columns, this.getLocalTextDbPrefix());
            return columns;
        }

        protected initialPopulate(): void {
            var self = this;
            if (this.populateWhenVisible()) {
                Serenity.LazyLoadHelper.executeEverytimeWhenShown(this.element, function () {
                    self.refreshIfNeeded();
                }, false);
                if (this.element.is(':visible') && this.view) {
                    this.view.populate();
                }
            }
            else if (this.view) {
                this.view.populate();
            }
        }

        protected initializeAsync(): PromiseLike<void> {
            return super.initializeAsync()
                .then<Slick.Column[]>(() => this.getColumnsAsync())
                .then(columns => {
                    this.allColumns = columns;
                this.postProcessColumns(this.allColumns);
                var self = this;
                if (this.filterBar) {
                    this.filterBar.set_store(new Serenity.FilterStore(this.allColumns.filter(function (x) {
                        return x.sourceItem && x.sourceItem.notFilterable !== true;
                    }).map(function (x1) {
                        return x1.sourceItem;
                        })));
                    this.filterBar.get_store().add_changed((s: JQueryEventObject, e: any) => {
                        if (this.restoringSettings <= 0) {
                            self.persistSettings(null);
                            self.refresh();
                        }
                    });
                }

                var visibleColumns = this.allColumns.filter(function (x2) {
                    return x2.visible !== false;
                });

                if (this.slickGrid) {
                    this.slickGrid.setColumns(visibleColumns);
                }
                this.setInitialSortOrder();
                this.initialSettings = this.getCurrentSettings(null);
                this.restoreSettings(null, null);
                this.initialPopulate();
            }, null);
        }

        protected createSlickGrid(): Slick.Grid {

            var visibleColumns: Slick.Column[];

            if (this.isAsyncWidget()) {
                visibleColumns = [];
            }
            else {
                this.allColumns = this.getColumns();
                visibleColumns = this.postProcessColumns(this.allColumns).filter(function (x) {
                    return x.visible !== false;
                });
            }

            var slickOptions = this.getSlickOptions();
            var grid = new Slick.Grid(this.slickContainer, this.view as any, visibleColumns, slickOptions);
            grid.registerPlugin(new Slick.AutoTooltips({
                enableForHeaderCells: true
            }));

            this.slickGrid = grid;
            this.rows = this.slickGrid;
            if (!this.isAsyncWidget()) {
                this.setInitialSortOrder();
            }

            return grid;
        }

        protected setInitialSortOrder(): void {
            var sortBy = this.getDefaultSortBy();

            if (this.view) {
                this.view.sortBy = Array.prototype.slice.call(sortBy);
            }

            var mapped = sortBy.map(function (s) {
                var x: Slick.ColumnSort = {};
                if (s && Q.endsWith(s.toLowerCase(), ' desc')) {
                    x.columnId = (ss as any).trimEndString(s.substr(0, s.length - 5));
                    x.sortAsc = false;
                }
                else {
                    x.columnId = s;
                    x.sortAsc = true;
                }
                return x;
            });

            this.slickGrid.setSortColumns(mapped);
        }

        itemAt(row: number) {
            return this.slickGrid.getDataItem(row);
        }

        rowCount() {
            return this.slickGrid.getDataLength();
        }

        getItems(): TItem[] {
            return this.view.getItems();
        }

        setItems(value: TItem[]) {
            this.view.setItems(value, true);
        }

        protected bindToSlickEvents() {
            var self = this;
            this.slickGridOnSort = (e: JQuery, p: any) => {
                self.view.populateLock();
                try {
                    var sortBy = [];
                    var col: any;
                    if (!!p.multiColumnSort) {
                        for (var i = 0; !!(i < p.sortCols.length); i++) {
                            var x = p.sortCols[i];
                            col = x.sortCol;
                            if (col == null) {
                                col = {};
                            }
                            sortBy.push(col.field + (!!x.sortAsc ? '' : ' DESC'));
                        }
                    }
                    else {
                        var col = p.sortCol;
                        if (col == null) {
                            col = {};
                        }
                        sortBy.push(col.field + (!!p.sortAsc ? '' : ' DESC'));
                    }

                    self.view.sortBy = sortBy;
                }
                finally {
                    self.view.populateUnlock();
                }
                self.view.populate();
                this.persistSettings(null);
            };

            this.slickGrid.onSort.subscribe(this.slickGridOnSort);

            this.slickGridOnClick = (e1: JQueryEventObject, p1: any) => {
                self.onClick(e1, p1.row, p1.cell);
            }

            this.slickGrid.onClick.subscribe(this.slickGridOnClick);

            this.slickGrid.onColumnsReordered.subscribe((e2: JQuery, p2: any) => {
                return this.persistSettings(null);
            });

            this.slickGrid.onColumnsResized.subscribe((e3: JQuery, p3: any) => {
                return this.persistSettings(null);
            });
        }

        protected getAddButtonCaption(): string {
            return Q.coalesce(Q.tryGetText('Controls.DataGrid.NewButton'), 'New');
        }

        protected getButtons(): ToolButton[] {
            return [];
        }

        protected editItem(entityOrId: any): void {
            throw new (ss as any).NotImplementedException();
        }

        protected editItemOfType(itemType: string, entityOrId: any): void {
            if (itemType === this.getItemType()) {
                this.editItem(entityOrId);
                return;
            }

            throw new (ss as any).NotImplementedException();
        }

        protected onClick(e: JQueryEventObject, row: number, cell: number): void {
            if (e.isDefaultPrevented()) {
                return;
            }

            var target = $(e.target);
            if (!target.hasClass('s-EditLink')) {
                target = target.closest('a');
            }

            if (target.hasClass('s-EditLink')) {
                e.preventDefault();
                this.editItemOfType(Serenity.SlickFormatting.getItemType(target), Serenity.SlickFormatting.getItemId(target));
            }
        }

        protected viewDataChanged(e: JQuery, rows: TItem[]): void {
            this.markupReady();
        }

        protected bindToViewEvents(): void {
            var self = this;

            this.view.onDataChanged.subscribe(function (e, d) {
                return self.viewDataChanged(e, d);
            });

            this.view.onSubmit = function (view) {
                return self.onViewSubmit();
            };

            this.view.setFilter(function (item, view1) {
                return self.onViewFilter(item);
            });

            this.view.onProcessData = function (response, view2) {
                return self.onViewProcessData(response);
            }
        }

        protected onViewProcessData(response: ListResponse<TItem>): ListResponse<TItem> {
            return response;
        }

        protected onViewFilter(item: TItem): boolean {
            return true;
        }

        protected getIncludeColumns(include: { [key: string]: boolean }): void {
            var columns = this.slickGrid.getColumns();
            for (var column of columns) {
                if (column.field) {
                    include[column.field] = true;
                }

                if (column.referencedFields) {
                    for (var x of column.referencedFields) {
                        include[x] = true;
                    }
                }
            }
        }

        protected setCriteriaParameter(): void {
            delete this.view.params['Criteria'];
            if (this.filterBar) {
                var criteria = this.filterBar.get_store().get_activeCriteria();
                if (!Serenity.Criteria.isEmpty(criteria)) {
                    this.view.params.Criteria = criteria;
                }
            }
        }

        protected setEquality(field: string, value: any): void {
            Q.setEquality(this.view.params, field, value);
        }

        protected setIncludeColumnsParameter(): void {
            var include = {};
            this.getIncludeColumns(include);
            var array = [];
            for (var key of Object.keys(include)) {
                array.push(key);
            }
            this.view.params.IncludeColumns = array;
        }

        protected onViewSubmit(): boolean {
            if (this.isDisabled || !this.getGridCanLoad()) {
                return false;
            }

            this.setCriteriaParameter();
            this.setIncludeColumnsParameter();
            this.invokeSubmitHandlers();

            return true;
        }

        protected markupReady(): void {
        }

        protected createSlickContainer(): JQuery {
            return $('<div class="grid-container"></div>').appendTo(this.element);
        }

        protected createView(): Slick.RemoteView<TItem> {
            var opt = this.getViewOptions();
            return new Slick.RemoteView<TItem>(opt);
        }

        protected getDefaultSortBy(): any[] {
            if (this.slickGrid) {

                var columns = this.slickGrid.getColumns().filter(function (x) {
                    return x.sortOrder && x.sortOrder !== 0;
                });

                if (columns.length > 0) {
                    columns.sort(function (x1, y) {
                        return (ss as any).compare(Math.abs(x1.sortOrder), Math.abs(y.sortOrder));
                    });

                    var list = [];
                    for (var i = 0; i < columns.length; i++) {
                        var col = columns[i];
                        list.push(col.field + ((col.sortOrder < 0) ? ' DESC' : ''));
                    }

                    return list;
                }
            }

            return [];
        }

        protected usePager(): boolean {
            return false;
        }

        protected enableFiltering(): boolean {
            var attr = this.attrs(Serenity.FilterableAttribute);
            return attr.length > 0 && attr[0].value;
        }

        protected populateWhenVisible(): boolean {
            return false;
        }

        protected createFilterBar(): void {
            var filterBarDiv = $('<div/>').appendTo(this.element);
            var self = this;
            this.filterBar = new Serenity.FilterDisplayBar(filterBarDiv);
            if (!this.isAsyncWidget()) {
                this.filterBar.set_store(new Serenity.FilterStore(this.allColumns.filter(function (x) {
                    return (x.sourceItem != null) && x.sourceItem.notFilterable !== true;
                }).map(function (x1) {
                    return x1.sourceItem;
                    })));
                this.filterBar.get_store().add_changed((s: JQueryEventObject, e: any) => {
                    if (this.restoringSettings <= 0) {
                        self.persistSettings(null);
                        self.refresh();
                    }
                });
            }
        }

        protected getPagerOptions(): Slick.PagerOptions {
            return {
                view: this.view,
                rowsPerPage: 20,
                rowsPerPageOptions: [20, 100, 500, 2500]
            };
        }

        protected createPager(): void {
            var pagerDiv = $('<div></div>').appendTo(this.element);
            pagerDiv.slickPager(this.getPagerOptions());
        }

        protected getViewOptions() {
            var opt: Slick.RemoteViewOptions = {};
            opt.idField = this.getIdProperty();
            opt.sortBy = this.getDefaultSortBy();

            if (!this.usePager()) {
                opt.rowsPerPage = 0;
            }
            else if (this.element.hasClass('responsive-height')) {
                opt.rowsPerPage = (($(window.window).width() < 768) ? 20 : 100);
            }
            else {
                opt.rowsPerPage = 100;
            }

            opt.getItemMetadata = (item, index) => {
                return this.getItemMetadata(item, index);
            };

            return opt;
        }

        protected createToolbar(buttons: ToolButton[]): void {
            var toolbarDiv = $('<div class="grid-toolbar"></div>').appendTo(this.element);
            this.toolbar = new Serenity.Toolbar(toolbarDiv, { buttons: buttons, hotkeyContext: this.element[0] });
        }

        getTitle(): string {
            if (!this.titleDiv) {
                return null;
            }

            return this.titleDiv.children().text();
        }

        setTitle(value: string) {
            if (value !== this.getTitle()) {
                if (value == null) {
                    if (this.titleDiv) {
                        this.titleDiv.remove();
                        this.titleDiv = null;
                    }
                }
                else {
                    if (!this.titleDiv) {
                        this.titleDiv = $('<div class="grid-title"><div class="title-text"></div></div>')
                            .prependTo(this.element);
                    }
                    this.titleDiv.children().text(value);
                }

                this.layout();
            }
        }

        protected getItemType(): string {
            return 'Item';
        }

        protected itemLink(itemType?: string, idField?: string, text?: (ctx: Slick.FormatterContext) => string,
            cssClass?: (ctx: Slick.FormatterContext) => string, encode: boolean = true): Slick.Format {

            if (itemType == null) {
                itemType = this.getItemType();
            }

            if (idField == null) {
                idField = this.getIdProperty();
            }

            return Serenity.SlickFormatting.itemLink(itemType, idField, text, cssClass, encode);
        }

        protected getColumnsKey(): string {
            var attr = this.attrs(Serenity.ColumnsKeyAttribute);
            if (attr && attr.length > 0) {
                return attr[0].value;
            }

            return null;
        }

        protected getPropertyItemsAsync(): PromiseLike<PropertyItem[]> {
            return Promise.resolve()
                .then<PropertyItem[]>(() => {
                    var columnsKey = this.getColumnsKey();
                    if (!Q.isEmptyOrNull(columnsKey)) {
                        return Q.getColumnsAsync(columnsKey);
                    }
                    return Promise.resolve<PropertyItem[]>([]);
                }, null);
        }

        protected getPropertyItems(): PropertyItem[] {
            var attr = this.attrs(Serenity.ColumnsKeyAttribute);

            var columnsKey = this.getColumnsKey();
            if (!Q.isEmptyOrNull(columnsKey)) {
                return Q.getColumns(columnsKey);
            }

            return [];
        }

        protected getColumns(): Slick.Column[] {
            var propertyItems = this.getPropertyItems();
            return this.propertyItemsToSlickColumns(propertyItems);
        }

        protected propertyItemsToSlickColumns(propertyItems: PropertyItem[]): Slick.Column[] {
            var columns = Serenity.PropertyItemSlickConverter.toSlickColumns(propertyItems);
            for (var i = 0; i < propertyItems.length; i++) {
                var item = propertyItems[i];
                var column = columns[i];
                if (item.editLink === true) {
                    var oldFormat = { $: column.format };
                    var css = { $: (item.editLinkCssClass) != null ? item.editLinkCssClass : null };
                    column.format = this.itemLink(
                        item.editLinkItemType != null ? item.editLinkItemType : null,
                        item.editLinkIdField != null ? item.editLinkIdField : null,
                        (ss as any).mkdel({ oldFormat: oldFormat }, function(ctx: Slick.FormatterContext) {
                            if (this.oldFormat.$ != null) {
                                return this.oldFormat.$(ctx);
                            }
                            return Q.htmlEncode(ctx.value);
                        }),
                        (ss as any).mkdel({ css: css }, function(ctx1: Slick.FormatterContext) {
                            return Q.coalesce(this.css.$, '');
                        }), false);

                    if (!Q.isEmptyOrNull(item.editLinkIdField)) {
                        column.referencedFields = column.referencedFields || [];
                        column.referencedFields.push(item.editLinkIdField);
                    }
                }
            }
            return columns;
        }

        protected getColumnsAsync(): PromiseLike<Slick.Column[]> {
            return this.getPropertyItemsAsync().then(propertyItems => {
                return this.propertyItemsToSlickColumns(propertyItems);
            }, null);
        }

        protected getSlickOptions(): Slick.GridOptions {
            var opt: Slick.GridOptions = {};
            opt.multiSelect = false;
            opt.multiColumnSort = true;
            opt.enableCellNavigation = false;
            opt.headerRowHeight = Serenity.DataGrid.defaultHeaderHeight;
            opt.rowHeight = Serenity.DataGrid.defaultRowHeight;
            return opt;
        }

        protected populateLock(): void {
            this.view.populateLock();
        }

        protected populateUnlock(): void {
            this.view.populateUnlock();
        }

        protected getGridCanLoad(): boolean {
            return true;
        }

        public refresh() {
            if (!this.populateWhenVisible()) {
                this.internalRefresh();
                return;
            }
            if (this.slickContainer.is(':visible')) {
                this.slickContainer.data('needsRefresh', false);
                this.internalRefresh();
                return;
            }
            this.slickContainer.data('needsRefresh', true);
        }

        protected refreshIfNeeded(): void {
            if (!!this.slickContainer.data('needsRefresh')) {
                this.slickContainer.data('needsRefresh', false);
                this.internalRefresh();
            }
        }

        protected internalRefresh(): void {
            this.view.populate();
        }

        public setIsDisabled(value: boolean): void {
            if (this.isDisabled !== value) {
                this.isDisabled = value;
                if (this.isDisabled) {
                    this.view.setItems([], true);
                }

                this.updateDisabledState();
            }
        }

        protected getLocalTextDbPrefix(): string {
            if (this.localTextDbPrefix == null) {
                this.localTextDbPrefix = Q.coalesce(this.getLocalTextPrefix(), '');
                if (this.localTextDbPrefix.length > 0 && !Q.endsWith(this.localTextDbPrefix, '.')) {
                    this.localTextDbPrefix = 'Db.' + this.localTextDbPrefix + '.';
                }
            }

            return this.localTextDbPrefix;
        }

        protected getLocalTextPrefix(): string {
            var attr = this.attrs(LocalTextPrefixAttribute);

            if (attr.length >= 1)
                return attr[0].value;

            return '';
        }

        protected getIdProperty(): string {
            if (this.idProperty == null) {
                var attr = this.attrs(IdPropertyAttribute);

                if (attr.length === 1) {
                    this.idProperty = attr[0].value;
                }
                else {
                    this.idProperty = 'ID';
                }
            }

            return this.idProperty;
        }

        protected getIsActiveProperty(): string {
            if (this.isActiveProperty == null) {
                var attr = this.attrs(IsActivePropertyAttribute);

                if (attr.length === 1) {
                    this.isActiveProperty = attr[0].value;
                }
                else {
                    this.isActiveProperty = '';
                }
            }
            return this.isActiveProperty;
        }

        protected updateDisabledState(): void {
            this.slickContainer.toggleClass('ui-state-disabled', !!this.isDisabled);
        }

        protected resizeCanvas(): void {
            this.slickGrid.resizeCanvas();
        }

        protected subDialogDataChange(): void {
            this.refresh();
        }

        protected addFilterSeparator(): void {
            if (this.quickFiltersDiv) {
                this.quickFiltersDiv.append($('<hr/>'));
            }
        }

        protected determineText(getKey: (prefix: string) => string) {
            var localTextPrefix = this.getLocalTextDbPrefix();
            if (!Q.isEmptyOrNull(localTextPrefix)) {
                var local = Q.tryGetText(getKey(localTextPrefix));
                if (local != null) {
                    return local;
                }
            }

            return null;
        }

        protected addQuickFilter<TWidget extends Widget<any>, TOptions>(opt: QuickFilter<TWidget, TOptions>): TWidget {
            if (opt == null) {
                throw new (ss as any).ArgumentNullException('opt');
            }

            if (this.quickFiltersDiv == null) {
                $('<div/>').addClass('clear').appendTo(this.toolbar.element);
                this.quickFiltersDiv = $('<div/>').addClass('quick-filters-bar').appendTo(this.toolbar.element);
            }

            if (opt.separator) {
                this.addFilterSeparator();
            }

            var item = $("<div class='quick-filter-item'><span class='quick-filter-label'></span></div>")
                .appendTo(this.quickFiltersDiv)
                .data('qffield', opt.field).children();

            var title = opt.title;
            if (title == null) {
                title = this.determineText(function (pre) {
                    return pre + opt.field;
                });
                if (title == null) {
                    title = opt.field;
                }
            }

            var quickFilter = item.text(title).parent();

            if (opt.displayText != null) {
                quickFilter.data('qfdisplaytext', opt.displayText);
            }

            if (opt.saveState != null) {
                quickFilter.data('qfsavestate', opt.saveState);
            }

            if (opt.loadState != null) {
                quickFilter.data('qfloadstate', opt.loadState);
            }

            if (!Q.isEmptyOrNull(opt.cssClass)) {
                quickFilter.addClass(opt.cssClass);
            }

            var widget = Serenity.Widget.create({
                type: opt.type,
                element: e => {
                    if (!Q.isEmptyOrNull(opt.field)) {
                        e.attr('id', this.uniqueName + '_QuickFilter_' + opt.field);
                    }
                    e.attr('placeholder', ' ');
                    e.appendTo(quickFilter);
                    if (opt.element != null) {
                        opt.element(e);
                    }
                },
                options: opt.options,
                init: opt.init
            });

            var submitHandler = () => {
                if (quickFilter.hasClass('ignore')) {
                    return;
                }
                var request = this.view.params;
                request.EqualityFilter = request.EqualityFilter || {};
                var value = Serenity.EditorUtils.getValue(widget);
                var active = value != null && !Q.isEmptyOrNull(value.toString());
                if (opt.handler != null) {
                    var args = {
                        field: opt.field,
                        request: request,
                        equalityFilter: request.EqualityFilter,
                        value: value,
                        active: active,
                        widget: widget,
                        handled: true
                    };
                    opt.handler(args);
                    quickFilter.toggleClass('quick-filter-active', args.active);
                    if (!args.handled) {
                        request.EqualityFilter[opt.field] = value;
                    }
                }
                else {
                    request.EqualityFilter[opt.field] = value;
                    quickFilter.toggleClass('quick-filter-active', active);
                }
            };

            Serenity.WX.changeSelect2(widget, (e1: JQueryEventObject) => {
                this.quickFilterChange(e1);
            });

            this.add_submitHandlers(submitHandler);
            widget.element.bind('remove.' + this.uniqueName, x => {
                this.remove_submitHandlers(submitHandler);
            });

            return widget;
        }

        protected addDateRangeFilter(field: string, title?: string): Serenity.DateEditor {
            return this.addQuickFilter(this.dateRangeQuickFilter(field, title)) as Serenity.DateEditor;
        }

        protected dateRangeQuickFilter(field: string, title?: string): QuickFilter<DateEditor, DateTimeEditorOptions> {
            var end: Serenity.DateEditor = null;
            return <QuickFilter<DateEditor, DateTimeEditorOptions>>{
                field: field,
                type: Serenity.DateEditor,
                title: title,
                element: function (e1) {
                    end = Serenity.Widget.create({
                        type: Serenity.DateEditor,
                        element: function (e2) {
                            e2.insertAfter(e1);
                        },
                        options: null,
                        init: null
                    });

                    end.element.change(function (x) {
                        e1.triggerHandler('change');
                    });

                    $('<span/>').addClass('range-separator').text('-').insertAfter(e1);
                },
                handler: function (args) {
                    var active1 = !Q.isTrimmedEmpty(args.widget.value);
                    var active2 = !Q.isTrimmedEmpty(end.value);
                    if (active1 && !Q.parseDate(args.widget.element.val())) {
                        active1 = false;
                        Q.notifyWarning(Q.text('Validation.DateInvalid'), '', null);
                        args.widget.element.val('');
                    }
                    if (active2 && !Q.parseDate(end.element.val())) {
                        active2 = false;
                        Q.notifyWarning(Q.text('Validation.DateInvalid'), '', null);
                        end.element.val('');
                    }
                    args.active = active1 || active2;
                    if (active1) {
                        args.request.Criteria = Serenity.Criteria.join(args.request.Criteria, 'and',
                            [[args.field], '>=', args.widget.value]);
                    }
                    if (active2) {
                        var next = new Date(end.valueAsDate.valueOf());
                        next.setDate(next.getDate() + 1);
                        args.request.Criteria = Serenity.Criteria.join(args.request.Criteria, 'and',
                            [[args.field], '<', Q.formatDate(next, 'yyyy-MM-dd')]);
                    }
                },
                displayText: function (w, l) {
                    var v1 = Serenity.EditorUtils.getDisplayText(w);
                    var v2 = Serenity.EditorUtils.getDisplayText(end);
                    if (Q.isEmptyOrNull(v1) && Q.isEmptyOrNull(v2)) {
                        return null;
                    }
                    var text1 = l + ' >= ' + v1;
                    var text2 = l + ' <= ' + v2;
                    if (!Q.isEmptyOrNull(v1) && !Q.isEmptyOrNull(v2)) {
                        return text1 + ' ' + Q.coalesce(Q.tryGetText('Controls.FilterPanel.And'), 'and') + ' ' + text2;
                    }
                    else if (!Q.isEmptyOrNull(v1)) {
                        return text1;
                    }
                    else {
                        return text2;
                    }
                },
                saveState: function (w1) {
                    return [Serenity.EditorUtils.getValue(w1), Serenity.EditorUtils.getValue(end)];
                },
                loadState: function (w2, state) {
                    if (state == null || !Q.isArray(state) || state.length !== 2) {
                        state = [null, null];
                    }

                    Serenity.EditorUtils.setValue(w2, state[0]);
                    Serenity.EditorUtils.setValue(end, state[1]);
                }
            };
        }

        protected addDateTimeRangeFilter(field: string, title?: string) {
            return this.addQuickFilter(this.dateTimeRangeQuickFilter(field, title)) as Serenity.DateTimeEditor;
        }

        protected dateTimeRangeQuickFilter(field: string, title?: string): QuickFilter<DateTimeEditor, DateTimeEditorOptions> {
            var end: Serenity.DateTimeEditor = null;

            return <QuickFilter<DateTimeEditor, DateTimeEditorOptions>>{
                field: field,
                type: Serenity.DateTimeEditor,
                title: title,
                element: function (e1) {
                    end = Serenity.Widget.create({
                        type: Serenity.DateTimeEditor,
                        element: function (e2) {
                            e2.insertAfter(e1);
                        },
                        options: null,
                        init: null
                    });

                    end.element.change(function(x) {
                        e1.triggerHandler('change');
                    });

                    $('<span/>').addClass('range-separator').text('-').insertAfter(e1);
                },
                init: function(i) {
                    i.element.parent().find('.time').change(function (x1) {
                        i.element.triggerHandler('change');
                    });
                },
                handler: function (args) {
                    var active1 = !Q.isTrimmedEmpty(args.widget.value);
                    var active2 = !Q.isTrimmedEmpty(end.value);
                    if (active1 && !Q.parseDate(args.widget.element.val())) {
                        active1 = false;
                        Q.notifyWarning(Q.text('Validation.DateInvalid'), '', null);
                        args.widget.element.val('');
                    }
                    if (active2 && !Q.parseDate(end.element.val())) {
                        active2 = false;
                        Q.notifyWarning(Q.text('Validation.DateInvalid'), '', null);
                        end.element.val('');
                    }
                    args.active = active1 || active2;
                    if (active1) {
                        args.request.Criteria = Serenity.Criteria.join(args.request.Criteria, 'and',
                            [[args.field], '>=', args.widget.value]);
                    }
                    if (active2) {
                        args.request.Criteria = Serenity.Criteria.join(args.request.Criteria, 'and',
                            [[args.field], '<=', end.value]);
                    }
                },
                displayText: function (w, l) {
                    var v1 = Serenity.EditorUtils.getDisplayText(w);
                    var v2 = Serenity.EditorUtils.getDisplayText(end);
                    if (Q.isEmptyOrNull(v1) && Q.isEmptyOrNull(v2)) {
                        return null;
                    }
                    var text1 = l + ' >= ' + v1;
                    var text2 = l + ' <= ' + v2;
                    if (!Q.isEmptyOrNull(v1) && !Q.isEmptyOrNull(v2)) {
                        return text1 + ' ' + Q.coalesce(Q.tryGetText('Controls.FilterPanel.And'), 'and') + ' ' + text2;
                    }
                    else if (!Q.isEmptyOrNull(v1)) {
                        return text1;
                    }
                    else {
                        return text2;
                    }
                },
                saveState: function (w1) {
                    return [Serenity.EditorUtils.getValue(w1), Serenity.EditorUtils.getValue(end)];
                },
                loadState: function (w2, state) {
                    if (state == null || !Q.isArray(state) || state.length !== 2) {
                        state = [null, null];
                    }
                    Serenity.EditorUtils.setValue(w2, state[0]);
                    Serenity.EditorUtils.setValue(end, state[1]);
                }
            };
        }

        protected addBooleanFilter(field: string, title?: string, yes?: string, no?: string): SelectEditor {
            return this.addQuickFilter(this.booleanQuickFilter(field, title, yes, no));
        }

        protected booleanQuickFilter(field: string, title?: string, yes?: string, no?: string): QuickFilter<SelectEditor, SelectEditorOptions> {
            var opt: SelectEditorOptions = {};
            var items = [];

            var trueText = yes;
            if (trueText == null) {
                trueText = Q.text('Controls.FilterPanel.OperatorNames.true');
            }
            items.push(['1', trueText]);

            var falseText = no;
            if (falseText == null) {
                falseText = Q.text('Controls.FilterPanel.OperatorNames.false');
            }

            items.push(['0', falseText]);

            opt.items = items;

            return {
                field: field,
                type: Serenity.SelectEditor,
                title: title,
                options: opt,
                handler: function(args) {
                    args.equalityFilter[args.field] = args.value == null || Q.isEmptyOrNull(args.value.toString()) ? 
                        null : !!Q.toId(args.value);
                }
            };
        }

        protected invokeSubmitHandlers() {
            if (this.submitHandlers != null) {
                this.submitHandlers();
            }
        }

        protected quickFilterChange(e: JQueryEventObject) {
            this.persistSettings(null);
            this.refresh();
        }

        protected getPersistanceStorage(): SettingStorage {
            return Serenity.DataGrid.defaultPersistanceStorage;
        }

        protected getPersistanceKey(): string {
            var key = 'GridSettings:';
            var path = window.location.pathname;
            if (!Q.isEmptyOrNull(path)) {
                key += path.substr(1).split(String.fromCharCode(47)).slice(0, 2).join('/') + ':';
            }

            key += (ss as any).getTypeFullName((ss as any).getInstanceType(this));
            return key;
        }

        protected gridPersistanceFlags(): GridPersistanceFlags {
            return {};
        }

        protected canShowColumn(column: Slick.Column) {
            if (column == null) {
                return false;
            }

            var item = column.sourceItem;
            if (item == null) {
                return true;
            }

            if (item.filterOnly === true) {
                return false;
            }

            if (item.readPermission == null) {
                return true;
            }

            return Q.Authorization.hasPermission(item.readPermission);
        }

        protected restoreSettings(settings?: PersistedGridSettings, flags?: GridPersistanceFlags): void {
            if (settings == null) {
                var storage = this.getPersistanceStorage();
                if (storage == null) {
                    return;
                }
                var json = Q.trimToNull(storage.getItem(this.getPersistanceKey()));
                if (json != null && Q.startsWith(json, '{') && Q.endsWith(json, '}')) {
                    settings = JSON.parse(json);
                }
                else {
                    return;
                }
            }

            if (!this.slickGrid) {
                return;
            }

            var columns = this.slickGrid.getColumns();
            var colById: Q.Dictionary<Slick.Column> = null;
            var updateColById = function(cl: Slick.Column[]) {
                colById = {};
                for (var $t1 = 0; $t1 < cl.length; $t1++) {
                    var c = cl[$t1];
                    colById[c.id] = c;
                }
            };

            this.view.beginUpdate();
            this.restoringSettings++;
            try {
                flags = flags || this.gridPersistanceFlags();
                if (settings.columns != null) {
                    if (flags.columnVisibility !== false) {
                        var visible = {};
                        updateColById(this.allColumns);
                        var newColumns = [];
                        for (var $t2 = 0; $t2 < settings.columns.length; $t2++) {
                            var x = settings.columns[$t2];
                            if (x.id != null && x.visible === true) {
                                var column = colById[x.id];
                                if (this.canShowColumn(column)) {
                                    column.visible = true;
                                    newColumns.push(column);
                                    delete colById[x.id];
                                }
                            }
                        }
                        for (var $t3 = 0; $t3 < this.allColumns.length; $t3++) {
                            var c1 = this.allColumns[$t3];
                            if (colById[c1.id] != null) {
                                c1.visible = false;
                                newColumns.push(c1);
                            }
                        }
                        this.allColumns = newColumns;
                        columns = this.allColumns.filter(function (x1) {
                            return x1.visible === true;
                        });
                    }
                    if (flags.columnWidths !== false) {
                        updateColById(columns);
                        for (var $t4 = 0; $t4 < settings.columns.length; $t4++) {
                            var x2 = settings.columns[$t4];
                            if (x2.id != null && x2.width != null && x2.width !== 0) {
                                var column1 = colById[x2.id];
                                if (column1 != null) {
                                    column1.width = x2.width;
                                }
                            }
                        }
                    }

                    if (flags.sortColumns !== false) {
                        updateColById(columns);
                        var list = [];
                        var sortColumns = settings.columns.filter(function (x3) {
                            return x3.id != null && Q.coalesce(x3.sort, 0) !== 0;
                        });

                        sortColumns.sort(function (a, b) {
                            return a.sort - b.sort;
                        });

                        for (var $t5 = 0; $t5 < sortColumns.length; $t5++) {
                            var x4 = sortColumns[$t5];
                            var column2 = colById[x4.id];
                            if (column2 != null) {
                                list.push({
                                    columnId: x4.id,
                                    sortAsc: x4.sort > 0
                                });
                            }
                        }
                        this.view.sortBy = list.map(function (x5) {
                            return x5.columnId + ((x5.sortAsc === false) ? ' DESC' : '');
                        });
                        this.slickGrid.setSortColumns(list);
                    }
                    this.slickGrid.setColumns(columns);
                    this.slickGrid.invalidate();
                }

                if (settings.filterItems != null &&
                    flags.filterItems !== false &&
                    this.filterBar != null &&
                    this.filterBar.get_store() != null) {
                    (ss as any).clear(this.filterBar.get_store().get_items());
                    (ss as any).arrayAddRange(this.filterBar.get_store().get_items(), settings.filterItems);
                    this.filterBar.get_store().raiseChanged();
                }

                if (settings.includeDeleted != null &&
                    flags.includeDeleted !== false) {
                    var includeDeletedToggle = this.element.find('.s-IncludeDeletedToggle');
                    if (!!settings.includeDeleted !== includeDeletedToggle.hasClass('pressed')) {
                        includeDeletedToggle.children('a').click();
                    }
                }

                if (settings.quickFilters != null &&
                    flags.quickFilters !== false &&
                    this.quickFiltersDiv != null &&
                    this.quickFiltersDiv.length > 0) {
                    this.quickFiltersDiv.find('.quick-filter-item').each((i, e) => {
                        var field = $(e).data('qffield');

                        if (Q.isEmptyOrNull(field)) {
                            return;
                        }

                        var widget = $('#' + this.uniqueName + '_QuickFilter_' + field).tryGetWidget(Widget);
                        if (widget == null) {
                            return;
                        }

                        var state = settings.quickFilters[field];
                        var loadState = $(e).data('qfloadstate');
                        if (loadState != null) {
                            loadState(widget, state);
                        }
                        else {
                            Serenity.EditorUtils.setValue(widget, state);
                        }
                    });
                }

                if (flags.quickSearch === true && (settings.quickSearchField != null || settings.quickSearchText != null)) {
                    var qsInput = this.toolbar.element.find('.s-QuickSearchInput').first();
                    if (qsInput.length > 0) {
                        var qsWidget = qsInput.tryGetWidget(Serenity.QuickSearchInput);
                        if (qsWidget != null) {
                            this.view.populateLock();
                            try {
                                qsWidget.element.addClass('ignore-change');
                                try {
                                    if (settings.quickSearchField != null) {
                                        qsWidget.set_field(settings.quickSearchField);
                                    }
                                    if (settings.quickSearchText != null &&
                                        Q.trimToNull(settings.quickSearchText) !== Q.trimToNull(qsWidget.element.val())) {
                                        qsWidget.element.val(settings.quickSearchText);
                                    }
                                }
                                finally {
                                    qsWidget.element.removeClass('ignore-change');
                                    qsWidget.element.triggerHandler('execute-search');
                                }
                            }
                            finally {
                                this.view.populateUnlock();
                            }
                        }
                    }
                }
            }
            finally {
                this.restoringSettings--;
                this.view.endUpdate();
            }
        }

        protected persistSettings(flags?: GridPersistanceFlags): void {
            var storage = this.getPersistanceStorage();
            if (!storage) {
                return;
            }

            var settings = this.getCurrentSettings(flags);
            storage.setItem(this.getPersistanceKey(), $.toJSON(settings));
        }

        protected getCurrentSettings(flags?: GridPersistanceFlags) {
            flags = flags || this.gridPersistanceFlags();
            var settings: PersistedGridSettings = {};
            if (flags.columnVisibility !== false || flags.columnWidths !== false || flags.sortColumns !== false) {
                settings.columns = [];
                var sortColumns = this.slickGrid.getSortColumns();
                var $t1 = this.slickGrid.getColumns();
                for (var $t2 = 0; $t2 < $t1.length; $t2++) {
                    var column = { $: $t1[$t2] };
                    var p: PersistedGridColumn = {
                        id: column.$.id
                    };

                    if (flags.columnVisibility !== false) {
                        p.visible = true;
                    }
                    if (flags.columnWidths !== false) {
                        p.width = column.$.width;
                    }

                    if (flags.sortColumns !== false) {
                        var sort = Q.indexOf(sortColumns, (ss as any).mkdel({ column: column }, function(x: Slick.ColumnSort) {
                            return x.columnId !== this.column.$.id;
                        }));

                        p.sort = ((sort >= 0) ? ((sortColumns[sort].sortAsc !== false) ? (sort + 1) : (-sort - 1)) : 0);
                    }
                    settings.columns.push(p);
                }
            }

            if (flags.includeDeleted !== false) {
                settings.includeDeleted = this.element.find('.s-IncludeDeletedToggle').hasClass('pressed');
            }

            if (flags.filterItems !== false && (this.filterBar != null) && (this.filterBar.get_store() != null)) {
                settings.filterItems = this.filterBar.get_store().get_items().slice();
            }

            if (flags.quickSearch === true) {
                var qsInput = this.toolbar.element.find('.s-QuickSearchInput').first();
                if (qsInput.length > 0) {
                    var qsWidget = qsInput.tryGetWidget(Serenity.QuickSearchInput);
                    if (qsWidget != null) {
                        settings.quickSearchField = qsWidget.get_field();
                        settings.quickSearchText = qsWidget.element.val();
                    }
                }
            }

            if (flags.quickFilters !== false && (this.quickFiltersDiv != null) && this.quickFiltersDiv.length > 0) {
                settings.quickFilters = {};
                this.quickFiltersDiv.find('.quick-filter-item').each((i, e) => {
                    var field = $(e).data('qffield');
                    if (Q.isEmptyOrNull(field)) {
                        return;
                    }

                    var widget = $('#' + this.uniqueName + '_QuickFilter_' + field).tryGetWidget(Widget);
                    if (widget == null) {
                        return;
                    }

                    var saveState = $(e).data('qfsavestate');
                    var state = (saveState != null) ? saveState(widget) : Serenity.EditorUtils.getValue(widget);
                    settings.quickFilters[field] = state;
                    if (flags.quickFilterText === true && $(e).hasClass('quick-filter-active')) {

                        var getDisplayText = $(e).data('qfdisplaytext');
                        var filterLabel = $(e).find('.quick-filter-label').text();

                        var displayText;
                        if (getDisplayText != null) {
                            displayText = getDisplayText(widget, filterLabel);
                        }
                        else {
                            displayText = filterLabel + ' = ' + Serenity.EditorUtils.getDisplayText(widget);
                        }

                        if (!Q.isEmptyOrNull(displayText)) {
                            if (!Q.isEmptyOrNull(settings.quickFilterText)) {
                                settings.quickFilterText += ' ' + Q.coalesce(Q.tryGetText('Controls.FilterPanel.And'), 'and') + ' ';
                                settings.quickFilterText += displayText;
                            }
                            else {
                                settings.quickFilterText = displayText;
                            }
                        }
                    }
                });
            }
            return settings;
        }

        getElement(): JQuery {
            return this.element;
        }

        getGrid(): Slick.Grid {
            return this.slickGrid;
        }

        getView(): Slick.RemoteView<TItem> {
            return this.view;
        }

        getFilterStore(): FilterStore {
            return (this.filterBar == null) ? null : this.filterBar.get_store();
        }
    }
}