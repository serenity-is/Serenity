namespace Serenity {

    export interface SettingStorage {
        getItem(key: string): string;
        setItem(key: string, value: string): void;
    }

    export interface PersistedGridColumn {
        id: string;
        width?: number;
        sort?: number;
        visible?: boolean;
    }

    export interface PersistedGridSettings {
        columns?: PersistedGridColumn[];
        filterItems?: FilterLine[];
        quickFilters?: Q.Dictionary<any>;
        quickFilterText?: string;
        quickSearchField?: QuickSearchField;
        quickSearchText?: string;
        includeDeleted?: boolean;
    }

    export interface GridPersistanceFlags {
        columnWidths?: boolean;
        columnVisibility?: boolean;
        sortColumns?: boolean;
        filterItems?: boolean;
        quickFilters?: boolean;
        quickFilterText?: boolean;
        quickSearch?: boolean;
        includeDeleted?: boolean;
    }

    export interface IDataGrid {
        getElement(): JQuery;
        getGrid(): Slick.Grid;
        getView(): Slick.RemoteView<any>;
        getFilterStore(): Serenity.FilterStore;
    }

    @Serenity.Decorators.registerInterface('Serenity.IDataGrid')
    export class IDataGrid {
    }

    @Serenity.Decorators.registerClass('Serenity.DataGrid', [IDataGrid, IReadOnly])
    @Serenity.Decorators.element("<div/>")
    export class DataGrid<TItem, TOptions> extends Widget<TOptions> implements IDataGrid, IReadOnly {

        protected titleDiv: JQuery;
        protected toolbar: Toolbar;
        protected filterBar: FilterDisplayBar;
        protected quickFiltersDiv: JQuery;
        protected quickFiltersBar: QuickFilterBar;
        protected slickContainer: JQuery;
        protected allColumns: Slick.Column[];
        protected initialSettings: PersistedGridSettings;
        protected restoringSettings: number = 0;
        private idProperty: string;
        private isActiveProperty: string;
        private localTextDbPrefix: string;
        private isDisabled: boolean;
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
            this.element.addClass('s-' + Q.getTypeName(Q.getInstanceType(this)));
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
            this.updateInterface();

            this.initialSettings = this.getCurrentSettings(null);
            this.restoreSettings(null, null);
            window.setTimeout(() => this.initialPopulate(), 0);
        }

        protected attrs<TAttr>(attrType: { new(...args: any[]): TAttr }): TAttr[] {
            return Q.getAttributes(Q.getInstanceType(this), attrType, true);
        }

        protected layout(): void {
            if (!this.element.is(':visible') || this.slickContainer == null)
                return;

            var responsiveHeight = this.element.hasClass('responsive-height');
            var madeAutoHeight = this.slickGrid != null && this.slickGrid.getOptions().autoHeight;
            var shouldAutoHeight = responsiveHeight && window.innerWidth < 768;

            if (shouldAutoHeight) {
                if (this.element[0] && this.element[0].style.height != "auto")
                    this.element[0].style.height = "auto";

                if (!madeAutoHeight) {

                    this.slickContainer.css('height', 'auto')
                        .children('.slick-pane').each((i, e: HTMLElement) => {
                            if (e.style.height != null && e.style.height != "auto")
                                e.style.height = "auto";
                        });

                    this.slickGrid.setOptions({ autoHeight: true });
                }
            }
            else {
                if (madeAutoHeight) {
                    this.slickContainer.children('.slick-viewport').css('height', 'auto');
                    this.slickGrid.setOptions({ autoHeight: false });
                }

                Q.layoutFillHeight(this.slickContainer);
            }

            this.slickGrid.resizeCanvas();
        }

        protected getInitialTitle(): string {
            return null;
        }

        protected createToolbarExtensions(): void {
        }

        protected ensureQuickFilterBar(): QuickFilterBar {

            if (this.quickFiltersDiv == null)
                this.createQuickFilters([]);

            return this.quickFiltersBar;
        }

        protected createQuickFilters(filters?: QuickFilter<Widget<any>, any>[]): void {

            if (this.quickFiltersDiv == null && (filters != null ||
                ((filters = this.getQuickFilters()) && filters != null && filters.length))) {
                this.quickFiltersDiv = $('<div/>').addClass('quick-filters-bar');
                if (this.toolbar) {
                    $('<div/>').addClass('clear').appendTo(this.toolbar.element);
                    this.quickFiltersDiv.appendTo(this.toolbar.element);
                }
                else {
                    this.quickFiltersDiv.appendTo($('<div/>').addClass('s-Toolbar').insertBefore(this.slickContainer));
                }

                this.quickFiltersBar = new QuickFilterBar(this.quickFiltersDiv, {
                    filters: filters,
                    getTitle: (filter: QuickFilter<Widget<any>, any>) => this.determineText(pre => pre + filter.field),
                    idPrefix: this.uniqueName + '_QuickFilter_'
                });
                this.quickFiltersBar.onChange = (e) => this.quickFilterChange(e);
            }
        }

        protected getQuickFilters(): QuickFilter<Widget<any>, any>[] {
            return this.allColumns.filter(function (x) {
                return x.sourceItem &&
                    x.sourceItem.quickFilter === true &&
                    (x.sourceItem.readPermission == null ||
                        Q.Authorization.hasPermission(x.sourceItem.readPermission));
            }).map(x => QuickFilterBar.propertyItemToQuickFilter(x.sourceItem))
                .filter(x => x != null);
        }

        protected findQuickFilter<TWidget>(type: { new(...args: any[]): TWidget }, field: string): TWidget {
            if (this.quickFiltersBar != null)
                return this.quickFiltersBar.find(type, field);

            return $('#' + this.uniqueName + '_QuickFilter_' + field).getWidget(type);
        }

        protected tryFindQuickFilter<TWidget>(type: { new(...args: any[]): TWidget }, field: string): TWidget {
            if (this.quickFiltersBar != null)
                return this.quickFiltersBar.tryFind(type, field);

            var el = $('#' + this.uniqueName + '_QuickFilter_' + field);
            if (!el.length)
                return null;

            return el.tryGetWidget(type);
        }

        protected createIncludeDeletedButton(): void {
            if (!Q.isEmptyOrNull(this.getIsActiveProperty()) ||
                !Q.isEmptyOrNull(this.getIsDeletedProperty())) {
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
            if (this.quickFiltersBar) {
                this.quickFiltersBar.destroy();
                this.quickFiltersBar = null;
            }

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
            var deletedFieldName = this.getIsDeletedProperty();
            if (Q.isEmptyOrNull(activeFieldName) && Q.isEmptyOrNull(deletedFieldName)) {
                return null;
            }

            if (activeFieldName) {
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
            }
            else {
                return item[deletedFieldName] ? 'deleted' : null;
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

        protected canFilterColumn(column: Slick.Column): boolean {
            return (column.sourceItem != null && 
                column.sourceItem.notFilterable !== true &&
                (column.sourceItem.readPermission == null ||
                    Q.Authorization.hasPermission(column.sourceItem.readPermission)));
        }

        protected initializeFilterBar() {

            this.filterBar.set_store(new Serenity.FilterStore(
                this.allColumns
                    .filter(c => this.canFilterColumn(c))
                    .map(x => x.sourceItem)));

            this.filterBar.get_store().add_changed((s: JQueryEventObject, e: any) => {
                if (this.restoringSettings <= 0) {
                    this.persistSettings(null);
                    this.view && (this.view.seekToPage = 1);
                    this.refresh();
                }
            });
        }

        protected createSlickGrid(): Slick.Grid {

            var visibleColumns: Slick.Column[];

            this.allColumns = this.getColumns();
            visibleColumns = this.postProcessColumns(this.allColumns).filter(function (x) {
                return x.visible !== false;
            });

            var slickOptions = this.getSlickOptions();
            var grid = new Slick.Grid(this.slickContainer, this.view as any, visibleColumns, slickOptions);
            grid.registerPlugin(new Slick.AutoTooltips({
                enableForHeaderCells: true
            }));

            this.slickGrid = grid;
            this.rows = this.slickGrid;
            
            this.setInitialSortOrder();

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
                    x.columnId = Q.trimEnd(s.substr(0, s.length - 5));
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

        itemAt(row: number): TItem {
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

                    self.view.seekToPage = 1;
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

            this.slickGrid.onColumnsReordered.subscribe((e2: JQueryEventObject, p2: any) => {
                return this.persistSettings(null);
            });

            this.slickGrid.onColumnsResized.subscribe((e3: JQueryEventObject, p3: any) => {
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
            throw new Error("Not Implemented!");
        }

        protected editItemOfType(itemType: string, entityOrId: any): void {
            if (itemType === this.getItemType()) {
                this.editItem(entityOrId);
                return;
            }

            throw new Error("Not Implemented!");
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

        protected viewDataChanged(e: any, rows: TItem[]): void {
            this.markupReady();
            this.layout();
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
                        return x1.sortOrder < y.sortOrder ? -1 : (x1.sortOrder > y.sortOrder ? 1 : 0);
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
            this.filterBar = new Serenity.FilterDisplayBar(filterBarDiv);
            this.initializeFilterBar();
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

            return this.titleDiv.children('.title-text').text();
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
                    this.titleDiv.children('.title-text').text(value);
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
                        function(ctx: Slick.FormatterContext) {
                            if (this.oldFormat.$ != null) {
                                return this.oldFormat.$(ctx);
                            }
                            return Q.htmlEncode(ctx.value);
                        }.bind({ oldFormat: oldFormat }),
                        function(ctx1: Slick.FormatterContext) {
                            return Q.coalesce(this.css.$, '');
                        }.bind({ css: css }), false);

                    if (!Q.isEmptyOrNull(item.editLinkIdField)) {
                        column.referencedFields = column.referencedFields || [];
                        column.referencedFields.push(item.editLinkIdField);
                    }
                }
            }
            return columns;
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

        private _readonly: boolean;

        public get readOnly(): boolean {
            return this.get_readOnly();
        }

        public set readOnly(value: boolean) {
            this.set_readOnly(value);
        }

        public get_readOnly() {
            return !!this._readonly;
        }

        public set_readOnly(value: boolean) {
            if (!!this._readonly != !!value) {
                this._readonly = !!value;
                this.updateInterface();
            }
        }

        protected updateInterface() {
            this.toolbar && this.toolbar.updateInterface();
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

        protected getIsDeletedProperty(): string {
            return null;
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
            this.ensureQuickFilterBar().addSeparator();
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
            return this.ensureQuickFilterBar().add(opt);
        }

        protected addDateRangeFilter(field: string, title?: string): Serenity.DateEditor {
            return this.ensureQuickFilterBar().addDateRange(field, title);
        }

        protected dateRangeQuickFilter(field: string, title?: string) {
            return QuickFilterBar.dateRange(field, title);
        }

        protected addDateTimeRangeFilter(field: string, title?: string) {
            return this.ensureQuickFilterBar().addDateTimeRange(field, title);
        }

        protected dateTimeRangeQuickFilter(field: string, title?: string) {
            return QuickFilterBar.dateTimeRange(field, title);
        }

        protected addBooleanFilter(field: string, title?: string, yes?: string, no?: string): SelectEditor {
            return this.ensureQuickFilterBar().addBoolean(field, title, yes, no);
        }

        protected booleanQuickFilter(field: string, title?: string, yes?: string, no?: string) {
            return QuickFilterBar.boolean(field, title, yes, no);
        }

        protected invokeSubmitHandlers() {
            if (this.quickFiltersBar != null) {
                this.quickFiltersBar.onSubmit(this.view.params);
            }
        }

        protected quickFilterChange(e: JQueryEventObject) {
            this.persistSettings(null);
            this.view && (this.view.seekToPage = 1);
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

            key += Q.getTypeFullName(Q.getInstanceType(this));
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

        protected getPersistedSettings(): PersistedGridSettings {
            var storage = this.getPersistanceStorage();
            if (storage == null)
                return null;

            var json = Q.trimToNull(storage.getItem(this.getPersistanceKey()));
            if (json != null && Q.startsWith(json, '{') && Q.endsWith(json, '}'))
                return JSON.parse(json);

            return null;
        }

        protected restoreSettings(settings?: PersistedGridSettings, flags?: GridPersistanceFlags): void {
            if (!this.slickGrid)
                return;

            if (settings == null) {
                settings = this.getPersistedSettings();
                if (settings == null)
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
                    var items = this.filterBar.get_store().get_items();
                    items.length = 0;
                    items.push.apply(items, settings.filterItems);
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
                var sortColumns = this.slickGrid.getSortColumns() as any[];
                var columns = this.slickGrid.getColumns();
                for (var column of columns) {
                    var p: PersistedGridColumn = {
                        id: column.id
                    };

                    if (flags.columnVisibility !== false) {
                        p.visible = true;
                    }
                    if (flags.columnWidths !== false) {
                        p.width = column.width;
                    }

                    if (flags.sortColumns !== false) {
                        var sort = Q.indexOf(sortColumns, x => x.columnId == column.id);
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
