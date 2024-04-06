import { Criteria, Fluent, ListResponse, debounce, getInstanceType, getTypeFullName, getjQuery, htmlEncode, isInstanceOfType, tryGetText, type PropertyItem, type PropertyItemsData } from "../../base";
import { ArgsCell, AutoTooltips, Column, ColumnSort, FormatterContext, Grid, GridOptions } from "@serenity-is/sleekgrid";
import { IReadOnly } from "../../interfaces";
import { Authorization, LayoutTimer, ScriptData, deepClone, extend, getColumnsData, getColumnsDataAsync, setEquality } from "../../q";
import { Format, PagerOptions, RemoteView, RemoteViewOptions } from "../../slick";
import { ColumnsKeyAttribute, FilterableAttribute, IdPropertyAttribute, IsActivePropertyAttribute, LocalTextPrefixAttribute } from "../../types/attributes";
import { Decorators } from "../../types/decorators";
import { DateEditor } from "../editors/dateeditor";
import { EditorUtils } from "../editors/editorutils";
import { SelectEditor } from "../editors/selecteditor";
import { FilterDisplayBar } from "../filtering/filterdisplaybar";
import { BooleanFiltering, DateFiltering, DateTimeFiltering, FilteringTypeRegistry, IFiltering, IQuickFiltering } from "../filtering/filtering";
import { FilterLine } from "../filtering/filterline";
import { FilterOperators } from "../filtering/filteroperator";
import { FilterStore } from "../filtering/filterstore";
import { LazyLoadHelper } from "../helpers/lazyloadhelper";
import { GridUtils, PropertyItemSlickConverter, SlickFormatting, SlickHelper } from "../helpers/slickhelpers";
import { ReflectionOptionsSetter } from "../widgets/reflectionoptionssetter";
import { ToolButton, Toolbar } from "../widgets/toolbar";
import { Widget, WidgetProps } from "../widgets/widget";
import { getWidgetFrom, tryGetWidget } from "../widgets/widgetutils";
import { IDataGrid } from "./idatagrid";
import { IRowDefinition } from "./irowdefinition";
import { QuickFilter } from "./quickfilter";
import { QuickFilterBar } from "./quickfilterbar";
import { QuickSearchField, QuickSearchInput } from "./quicksearchinput";
import { SlickPager } from "./slickpager";

export interface SettingStorage {
    getItem(key: string): string | Promise<string>;
    setItem(key: string, value: string): void | Promise<void>;
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
    quickFilters?: { [key: string]: any };
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

@Decorators.registerClass('Serenity.DataGrid', [IReadOnly])
export class DataGrid<TItem, P = {}> extends Widget<P> implements IDataGrid, IReadOnly {

    private _isDisabled: boolean;
    private _layoutTimer: number;
    private _slickGridOnSort: any;
    private _slickGridOnClick: any;
    protected titleDiv: Fluent;
    protected toolbar: Toolbar;
    protected filterBar: FilterDisplayBar;
    protected quickFiltersDiv: Fluent;
    protected quickFiltersBar: QuickFilterBar;
    protected slickContainer: Fluent;
    protected allColumns: Column[];
    protected propertyItemsData: PropertyItemsData;
    protected initialSettings: PersistedGridSettings;
    protected restoringSettings: number = 0;
    public view: RemoteView<TItem>;
    public slickGrid: Grid;
    public openDialogsAsPanel: boolean;

    public static defaultRowHeight: number;
    public static defaultHeaderHeight: number;
    public static defaultPersistanceStorage: SettingStorage;
    public static defaultColumnWidthScale: number;
    public static defaultColumnWidthDelta: number;

    constructor(props: WidgetProps<P>) {
        super(props);

        this.domNode.classList.add('s-DataGrid');

        var layout = function () {
            this.layout();
            if (this._layoutTimer != null)
                LayoutTimer.store(this._layoutTimer);
        }.bind(this);

        this.element.addClass('require-layout').on('layout.' + this.uniqueName, layout);

        if (this.useLayoutTimer())
            this._layoutTimer = LayoutTimer.onSizeChange(() => this.domNode && this.domNode, debounce(layout, 50));

        this.setTitle(this.getInitialTitle());

        var buttons = this.getButtons();
        if (buttons != null) {
            this.createToolbar(buttons);
        }

        this.slickContainer = this.createSlickContainer();
        this.view = this.createView();

        this.syncOrAsyncThen(this.getPropertyItemsData, this.getPropertyItemsDataAsync, itemsData => {
            this.propertyItemsReady(itemsData);
            this.afterInit();
        });
    }

    protected propertyItemsReady(itemsData: PropertyItemsData) {
        this.propertyItemsData = itemsData;
        this.allColumns = this.allColumns ?? this.getColumns();
        this.slickGrid = this.createSlickGrid();

        if (this.enableFiltering()) {
            this.createFilterBar();
        }

        if (this.usePager()) {
            this.createPager();
        }
        this.bindToSlickEvents();
        this.bindToViewEvents();

        if (this.toolbar) {
            this.createToolbarExtensions();
        }

        this.createQuickFilters();

        this.updateDisabledState();
        this.updateInterface();

        this.initialSettings = this.getCurrentSettings(null);

        var restoreResult = this.restoreSettings(null, null);
        if ((restoreResult as any)?.then)
            (restoreResult as Promise<void>).then(() => window.setTimeout(() => this.initialPopulate(), 0));
        else
            window.setTimeout(() => this.initialPopulate(), 0);
    }

    protected afterInit() {
    }

    protected useAsync() {
        return false;
    }

    protected useLayoutTimer() {
        return true;
    }

    protected layout(): void {
        if (!this.domNode || !Fluent.isVisibleLike(this.domNode) || !this.slickContainer || !this.slickGrid)
            return;

        var responsiveHeight = this.domNode.classList.contains('responsive-height');
        var madeAutoHeight = this.slickGrid != null && this.slickGrid.getOptions().autoHeight;
        var shouldAutoHeight = responsiveHeight && window.innerWidth < 768;

        if (shouldAutoHeight) {
            if (!madeAutoHeight) {
                this.slickGrid.setOptions({ autoHeight: true });
            }
        }
        else if (madeAutoHeight) {
            this.slickContainer.getNode().style.height = "";
            this.slickContainer.findAll('.slick-viewport').forEach(x => x.style.height = "");
            this.slickGrid.setOptions({ autoHeight: false });
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
            this.quickFiltersDiv = Fluent("div").class('quick-filters-bar');
            if (this.toolbar) {
                Fluent("div").class('clear').appendTo(this.toolbar.domNode);
                this.quickFiltersDiv.appendTo(this.toolbar.domNode);
            }
            else {
                this.quickFiltersDiv.appendTo(Fluent("div").class('s-Toolbar').insertBefore(this.slickContainer));
            }

            this.quickFiltersBar = new QuickFilterBar({
                filters: filters,
                getTitle: (filter: QuickFilter<Widget<any>, any>) => this.determineText(pre => pre + filter.field),
                idPrefix: this.uniqueName + '_QuickFilter_',
                element: this.quickFiltersDiv
            });
            this.quickFiltersBar.onChange = (e) => this.quickFilterChange(e);
        }
    }

    protected getQuickFilters(): QuickFilter<Widget<any>, any>[] {
        return this.allColumns.filter(function (x) {
            return x.sourceItem &&
                x.sourceItem.quickFilter === true &&
                (x.sourceItem.readPermission == null ||
                    Authorization.hasPermission(x.sourceItem.readPermission));
        }).map(x => DataGrid.propertyItemToQuickFilter(x.sourceItem))
            .filter(x => x != null);
    }

    public static propertyItemToQuickFilter(item: PropertyItem) {
        var quick: any = {};

        var name = item.name;
        var title = tryGetText(item.title);
        if (title == null) {
            title = item.title;
            if (title == null) {
                title = name;
            }
        }

        var filteringType = FilteringTypeRegistry.get((item.filteringType ?? 'String'));
        if (filteringType === DateFiltering) {
            quick = QuickFilterBar.dateRange(name, title);
        }
        else if (filteringType === DateTimeFiltering) {
            quick = QuickFilterBar.dateTimeRange(name, title, item.editorParams?.useUtc);
        }
        else if (filteringType === BooleanFiltering) {
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
            quick = QuickFilterBar.boolean(name, title, trueText, falseText);
        }
        else {
            var filtering = new (filteringType as any)(item.filteringParams ?? {}) as IFiltering;
            if (filtering && isInstanceOfType(filtering, IQuickFiltering)) {
                ReflectionOptionsSetter.set(filtering, item.filteringParams);
                filtering.set_field(item);
                filtering.set_operator({ key: FilterOperators.EQ });
                (filtering as any).initQuickFilter(quick);
                quick.options = extend(deepClone(quick.options), item.quickFilterParams);
            }
            else {
                return null;
            }
        }

        if (!!item.quickFilterSeparator) {
            quick.separator = true;
        }

        quick.cssClass = item.quickFilterCssClass;
        return quick;
    }

    protected findQuickFilter<TWidget>(type: { new(...args: any[]): TWidget }, field: string): TWidget {
        if (this.quickFiltersBar != null)
            return this.quickFiltersBar.find(type, field);

        return getWidgetFrom('#' + this.uniqueName + '_QuickFilter_' + field, type);
    }

    protected tryFindQuickFilter<TWidget>(type: { new(...args: any[]): TWidget }, field: string): TWidget {
        if (this.quickFiltersBar != null)
            return this.quickFiltersBar.tryFind(type, field);

        return tryGetWidget('#' + this.uniqueName + '_QuickFilter_' + field, type);
    }

    protected createIncludeDeletedButton(): void {
        if (this.getIsActiveProperty() || this.getIsDeletedProperty())
            GridUtils.addIncludeDeletedToggle(this.toolbar.domNode, this.view, null, false);
    }

    protected getQuickSearchFields(): QuickSearchField[] {
        return null;
    }

    protected createQuickSearchInput(): void {
        var input = GridUtils.addQuickSearchInput(this.toolbar.domNode, this.view, this.getQuickSearchFields(), () => this.persistSettings(null));
        input?.domNode?.setAttribute('id', this.idPrefix + 'QuickSearchInput');
    }

    public destroy() {
        if (this._layoutTimer) {
            this._layoutTimer = LayoutTimer.off(this._layoutTimer);
        }
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
        if (activeFieldName && deletedFieldName)
            return null;

        if (activeFieldName) {
            var value = (item as any)[activeFieldName];
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
            return (item as any)[deletedFieldName] ? 'deleted' : null;
        }

        return null;
    }

    protected getItemMetadata(item: TItem, index: number): any {
        var itemClass = this.getItemCssClass(item, index);
        if (!itemClass) {
            return new Object();
        }
        return { cssClasses: itemClass };
    }

    protected postProcessColumns(columns: Column[]): Column[] {
        SlickHelper.setDefaults(columns, this.getLocalTextDbPrefix());

        var delta = this.getColumnWidthDelta();
        var scale = this.getColumnWidthScale();
        if (scale < 0)
            scale = 1;
        if (delta !== 0 || scale !== 1) {
            for (var col of columns) {
                if (typeof col.width === "number")
                    col.width = Math.round(col.width * scale + delta);
                if (typeof col.minWidth === "number")
                    col.minWidth = Math.round(col.minWidth * scale + delta);
                if (typeof col.maxWidth === "number")
                    col.maxWidth = Math.round(col.maxWidth * scale + delta);
            }
        }

        return columns;
    }

    protected getColumnWidthDelta() {
        return DataGrid.defaultColumnWidthDelta ?? 0;
    }

    protected getColumnWidthScale() {
        return DataGrid.defaultColumnWidthScale ?? 1;
    }

    protected initialPopulate(): void {
        var self = this;
        if (this.populateWhenVisible()) {
            LazyLoadHelper.executeEverytimeWhenShown(this.domNode, function () {
                self.refreshIfNeeded();
            }, false);
            if (Fluent.isVisibleLike(this.domNode) && this.view) {
                this.view.populate();
            }
        }
        else if (this.view) {
            this.view.populate();
        }
    }

    protected canFilterColumn(column: Column): boolean {
        return (column.sourceItem != null &&
            column.sourceItem.notFilterable !== true &&
            (column.sourceItem.readPermission == null ||
                Authorization.hasPermission(column.sourceItem.readPermission)));
    }

    protected initializeFilterBar() {

        this.filterBar.set_store(new FilterStore(
            this.allColumns
                .filter(c => this.canFilterColumn(c))
                .map(x => x.sourceItem)));

        this.filterBar.get_store().add_changed(() => {
            if (this.restoringSettings <= 0) {
                this.persistSettings(null);
                this.view && (this.view.seekToPage = 1);
                this.refresh();
            }
        });
    }

    protected createSlickGrid(): Grid {

        var visibleColumns = this.postProcessColumns(this.allColumns).filter(function (x) {
            return x.visible !== false;
        });

        var slickOptions = this.getSlickOptions();
        var grid = new Grid(this.slickContainer.getNode(), this.view as any, visibleColumns, slickOptions) as Grid;
        grid.registerPlugin(new AutoTooltips({
            enableForHeaderCells: true
        }));

        this.slickGrid = grid;

        this.setInitialSortOrder();

        return grid;
    }

    protected setInitialSortOrder(): void {
        var sortBy = this.getDefaultSortBy();

        if (this.view) {
            this.view.sortBy = Array.prototype.slice.call(sortBy);
        }

        var mapped = sortBy.map(function (s): ColumnSort {
            var x: ColumnSort;
            if (s && s.toLowerCase().endsWith(' desc')) {
                return {
                    columnId: s.substr(0, s.length - 5).trimEnd(),
                    sortAsc: false
                }
            }
            else return {
                columnId: s,
                sortAsc: true
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
        this._slickGridOnSort = (_: Event, p: any) => {
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

            if (self.view.getLocalSort && self.view.getLocalSort()) {
                self.view.sort();
            }
            else {
                self.view.populate();
            }
            this.persistSettings(null);
        };

        this.slickGrid.onSort.subscribe(this._slickGridOnSort);

        this._slickGridOnClick = (e1: MouseEvent, p1: ArgsCell) => {
            self.onClick(e1, p1.row, p1.cell);
        }

        this.slickGrid.onClick.subscribe(this._slickGridOnClick);

        this.slickGrid.onColumnsReordered.subscribe(() => {
            return this.persistSettings(null);
        });

        this.slickGrid.onColumnsResized.subscribe(() => {
            return this.persistSettings(null);
        });
    }

    protected getAddButtonCaption(): string {
        return tryGetText('Controls.DataGrid.NewButton') ?? 'New';
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

    protected onClick(e: Event, row: number, cell: number): void {
        if (Fluent.isDefaultPrevented(e)) {
            return;
        }

        var target = e.target as HTMLElement;
        if (!target.classList.contains('s-EditLink')) {
            target = target.closest('a');
        }

        if (target && target.classList.contains('s-EditLink')) {
            e.preventDefault();
            this.editItemOfType(SlickFormatting.getItemType(target), SlickFormatting.getItemId(target));
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
            if (!Criteria.isEmpty(criteria)) {
                this.view.params.Criteria = criteria;
            }
        }
    }

    protected setEquality(field: string, value: any): void {
        setEquality(this.view.params, field, value);
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
        if (this._isDisabled || !this.getGridCanLoad()) {
            return false;
        }

        this.setCriteriaParameter();
        this.setIncludeColumnsParameter();
        this.invokeSubmitHandlers();

        return true;
    }

    protected markupReady(): void {
    }

    protected createSlickContainer(): Fluent {
        return Fluent("div").class("grid-container").appendTo(this.domNode);
    }

    protected createView(): RemoteView<TItem> {
        var opt = this.getViewOptions();
        return new RemoteView<TItem>(opt) as any;
    }

    protected getDefaultSortBy(): any[] {
        if (this.slickGrid) {

            var columns = this.slickGrid.getColumns().filter(function (x) {
                return x.sortOrder && x.sortOrder !== 0;
            });

            if (columns.length > 0) {
                columns.sort(function (x1, y) {
                    return Math.abs(x1.sortOrder) < Math.abs(y.sortOrder) ? -1 : (Math.abs(x1.sortOrder) > Math.abs(y.sortOrder) ? 1 : 0);
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
        return this.getCustomAttribute(FilterableAttribute)?.value;
    }

    protected populateWhenVisible(): boolean {
        return false;
    }

    protected createFilterBar(): void {
        this.filterBar = new FilterDisplayBar({
            element: el => this.domNode.append(el)
        });
        this.initializeFilterBar();
    }

    protected getPagerOptions(): PagerOptions {
        return {
            view: this.view,
            rowsPerPage: 20,
            rowsPerPageOptions: [20, 100, 500, 2500]
        };
    }

    protected createPager(): void {
        new SlickPager({ ...this.getPagerOptions(), element: el => this.domNode.append(el) });
    }

    protected getViewOptions() {
        var opt: RemoteViewOptions = {};
        opt.idField = this.getIdProperty();
        opt.sortBy = this.getDefaultSortBy();

        if (!this.usePager()) {
            opt.rowsPerPage = 0;
        }
        else if (this.domNode.classList.contains('responsive-height')) {
            opt.rowsPerPage = window.innerWidth < 768 ? 20 : 100;
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
        this.toolbar = new Toolbar({
            buttons: buttons,
            hotkeyContext: this.domNode,
            element: el => this.domNode.appendChild(el).classList.add("grid-toolbar")
        });
    }

    getTitle(): string {
        if (!this.titleDiv) {
            return null;
        }

        return this.titleDiv.findFirst('.title-text').text();
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
                    this.titleDiv = Fluent("div")
                        .class("grid-title")
                        .append(Fluent("div").class("title-text"))
                        .prependTo(this.domNode);
                }
                this.titleDiv.findFirst('.title-text').text(value);
            }

            this.layout();
        }
    }

    protected getItemType(): string {
        return 'Item';
    }

    protected itemLink(itemType?: string, idField?: string, text?: Format<TItem>,
        cssClass?: (ctx: FormatterContext) => string, encode: boolean = true): Format<TItem> {

        if (itemType == null) {
            itemType = this.getItemType();
        }

        if (idField == null) {
            idField = this.getIdProperty();
        }

        return SlickFormatting.itemLink(itemType, idField, text, cssClass, encode);
    }

    protected getColumnsKey(): string {
        return this.getCustomAttribute(ColumnsKeyAttribute)?.value;
    }

    protected getPropertyItems(): PropertyItem[] {
        return this.propertyItemsData?.items || [];
    }

    protected getPropertyItemsData(): PropertyItemsData {
        var columnsKey = this.getColumnsKey();

        if (this.getColumnsKey === DataGrid.prototype.getColumnsKey &&
            this.getPropertyItems !== DataGrid.prototype.getPropertyItems &&
            !ScriptData.canLoad('Columns.' + columnsKey)) {
            return {
                items: this.getPropertyItems(),
                additionalItems: []
            }
        }


        if (columnsKey) {
            return getColumnsData(columnsKey);
        }

        return { items: [], additionalItems: [] };
    }

    protected async getPropertyItemsDataAsync(): Promise<PropertyItemsData> {
        var columnsKey = this.getColumnsKey();
        if (columnsKey) {
            return await getColumnsDataAsync(columnsKey);
        }

        return { items: [], additionalItems: [] };
    }

    protected getColumns(): Column<TItem>[] {
        return this.propertyItemsToSlickColumns(this.getPropertyItems());
    }

    protected wrapFormatterWithEditLink(column: Column, item: PropertyItem) {
        const orgFormat = column.format;
        const itemType = item.editLinkItemType || null;
        const idField = item.editLinkIdField || null;
        const linkClass = item.editLinkCssClass || null;
        column.format = this.itemLink(itemType, idField,
            ctx => orgFormat != null ? orgFormat(ctx) : htmlEncode(ctx.value),
            () => linkClass, false);

        if (idField) {
            column.referencedFields = column.referencedFields || [];
            column.referencedFields.push(idField);
        }
    }

    protected propertyItemsToSlickColumns(propertyItems: PropertyItem[]): Column[] {
        var columns = PropertyItemSlickConverter.toSlickColumns(propertyItems);
        for (var i = 0; i < propertyItems.length; i++) {
            var item = propertyItems[i];
            if (item.editLink) {
                this.wrapFormatterWithEditLink(columns[i], item);
            }
        }
        return columns;
    }

    protected getSlickOptions(): GridOptions {
        var opt: GridOptions = {};
        opt.multiSelect = false;
        opt.multiColumnSort = true;
        opt.enableCellNavigation = false;
        if (!getjQuery()) {
            opt.emptyNode = Fluent.empty;
            opt.removeNode = Fluent.remove;
        }
        if (DataGrid.defaultHeaderHeight)
            opt.headerRowHeight = DataGrid.defaultHeaderHeight;
        if (DataGrid.defaultRowHeight)
            opt.rowHeight = DataGrid.defaultRowHeight;
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
        if (Fluent.isVisibleLike(this.slickContainer.getNode())) {
            this.slickContainer.data("needsRefresh", null);
            this.internalRefresh();
            return;
        }
        this.slickContainer.getNode().dataset.needsRefresh = "true";
    }

    protected refreshIfNeeded(): void {
        if (!!this.slickContainer.data("needsRefresh")) {
            this.slickContainer.data('needsRefresh', null);
            this.internalRefresh();
        }
    }

    protected internalRefresh(): void {
        this.view.populate();
    }

    public setIsDisabled(value: boolean): void {
        if (this._isDisabled !== value) {
            this._isDisabled = value;
            if (this._isDisabled) {
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

    public updateInterface() {
        this.toolbar && this.toolbar.updateInterface();
    }

    protected getRowDefinition(): IRowDefinition {
        return null;
    }

    private _localTextDbPrefix: string;

    protected getLocalTextDbPrefix(): string {

        if (this._localTextDbPrefix != null)
            return this._localTextDbPrefix;

        this._localTextDbPrefix = this.getLocalTextPrefix() ?? '';
        if (this._localTextDbPrefix.length > 0 && !this._localTextDbPrefix.endsWith('.'))
            this._localTextDbPrefix = 'Db.' + this._localTextDbPrefix + '.';

        return this._localTextDbPrefix;
    }

    protected getLocalTextPrefix(): string {
        var rowDefinition = this.getRowDefinition();
        if (rowDefinition)
            return rowDefinition.localTextPrefix;

        return this.getCustomAttribute(LocalTextPrefixAttribute)?.value;
    }

    private _idProperty: string;

    protected getIdProperty(): string {
        if (this._idProperty != null)
            return this._idProperty;

        var rowDefinition = this.getRowDefinition();
        if (rowDefinition)
            return this._idProperty = rowDefinition.idProperty ?? '';

        var attr = this.getCustomAttribute(IdPropertyAttribute);
        if (attr)
            return this._idProperty = attr.value ?? '';

        return this._idProperty = 'ID';
    }

    protected getIsDeletedProperty(): string {
        return this.getRowDefinition()?.isDeletedProperty;
    }

    private _isActiveProperty: string;

    protected getIsActiveProperty(): string {
        if (this._isActiveProperty != null)
            return this._isActiveProperty;

        var rowDefinition = this.getRowDefinition();
        if (rowDefinition)
            return this._isActiveProperty = rowDefinition.isActiveProperty ?? '';

        var attr = this.getCustomAttribute(IsActivePropertyAttribute);
        if (attr)
            return this._isActiveProperty = attr.value ?? '';

        return this._isActiveProperty = '';
    }

    protected updateDisabledState(): void {
        this.slickContainer.toggleClass('ui-state-disabled', !!this._isDisabled);
    }

    protected resizeCanvas(): void {
        this.slickGrid?.resizeCanvas();
    }

    protected subDialogDataChange(): void {
        this.refresh();
    }

    protected addFilterSeparator(): void {
        this.ensureQuickFilterBar().addSeparator();
    }

    protected determineText(getKey: (prefix: string) => string) {
        var localTextPrefix = this.getLocalTextDbPrefix();
        if (localTextPrefix) {
            var local = tryGetText(getKey(localTextPrefix));
            if (local != null) {
                return local;
            }
        }

        return null;
    }

    protected addQuickFilter<TWidget extends Widget<any>, P>(opt: QuickFilter<TWidget, P>): TWidget {
        return this.ensureQuickFilterBar().add(opt);
    }

    protected addDateRangeFilter(field: string, title?: string): DateEditor {
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

    protected quickFilterChange(e: Event) {
        this.persistSettings(null);
        this.view && (this.view.seekToPage = 1);
        this.refresh();
    }

    protected getPersistanceStorage(): SettingStorage {
        return DataGrid.defaultPersistanceStorage;
    }

    protected getPersistanceKey(): string {
        var key = 'GridSettings:';
        var path = window.location.pathname;
        if (path) {
            key += path.substr(1).split(String.fromCharCode(47)).slice(0, 2).join('/') + ':';
        }

        key += getTypeFullName(getInstanceType(this));
        return key;
    }

    protected gridPersistanceFlags(): GridPersistanceFlags {
        return {};
    }

    protected canShowColumn(column: Column) {
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

        return Authorization.hasPermission(item.readPermission);
    }

    protected getPersistedSettings(): PersistedGridSettings | Promise<PersistedGridSettings> {
        var storage = this.getPersistanceStorage();
        if (storage == null)
            return null;

        function fromJson(json: string) {
            json = json?.trim();
            if (json?.startsWith('{') && json.endsWith('}'))
                return JSON.parse(json);
            return null;
        }

        var jsonOrPromise = storage.getItem(this.getPersistanceKey());
        if ((jsonOrPromise as any)?.then)
            return (jsonOrPromise as Promise<string>).then(json => fromJson(json));

        return fromJson(jsonOrPromise as string);
    }

    protected restoreSettings(settings?: PersistedGridSettings, flags?: GridPersistanceFlags): void | Promise<void> {
        if (settings != null)
            return this.restoreSettingsFrom(settings, flags);

        var settingsOrPromise = this.getPersistedSettings();
        if ((settingsOrPromise as any)?.then)
            return (settingsOrPromise as Promise<PersistedGridSettings>).then((s) => this.restoreSettingsFrom(s));

        this.restoreSettingsFrom(settingsOrPromise as PersistedGridSettings);
    }

    protected restoreSettingsFrom(settings: PersistedGridSettings, flags?: GridPersistanceFlags): void {
        if (!this.slickGrid || !settings)
            return;

        var columns = this.slickGrid.getColumns();
        var colById: { [key: string]: Column } = null;
        var updateColById = function (cl: Column[]) {
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
                        return x3.id != null && (x3.sort ?? 0) !== 0;
                    });

                    sortColumns.sort(function (a, b) {
                        // sort holds two informations:
                        // absolute value: order of sorting
                        // sign: positive = ascending, negative = descending
                        // so we have to compare absolute values here
                        return Math.abs(a.sort) - Math.abs(b.sort);
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
                var includeDeletedToggle = this.domNode.querySelector('.s-IncludeDeletedToggle');
                if (includeDeletedToggle && !!settings.includeDeleted !== includeDeletedToggle.classList.contains('pressed')) {
                    Fluent.trigger(includeDeletedToggle.querySelector('a'), "click");
                }
            }

            if (settings.quickFilters != null &&
                flags.quickFilters !== false &&
                this.quickFiltersDiv != null &&
                this.quickFiltersDiv.length > 0) {
                this.quickFiltersDiv.findAll('.quick-filter-item').forEach(e => {
                    var field = e.dataset.qffield;

                    if (!field?.length) {
                        return;
                    }

                    var widget = tryGetWidget('#' + this.uniqueName + '_QuickFilter_' + field, Widget);
                    if (widget == null) {
                        return;
                    }

                    var state = settings.quickFilters[field];
                    var loadState = (e as any).qfloadstate;
                    if (typeof loadState === "function") {
                        loadState(widget, state);
                    }
                    else {
                        EditorUtils.setValue(widget, state);
                    }
                });
            }

            if (flags.quickSearch === true && (settings.quickSearchField !== undefined || settings.quickSearchText !== undefined)) {
                var qsInput = this.toolbar?.domNode?.querySelector('.s-QuickSearchInput');
                if (qsInput) {
                    var qsWidget = tryGetWidget(qsInput, QuickSearchInput);
                    qsWidget && qsWidget.restoreState(settings.quickSearchText, settings.quickSearchField);
                }
            }
        }
        finally {
            this.restoringSettings--;
            this.view.endUpdate();
        }
    }

    protected persistSettings(flags?: GridPersistanceFlags): void | Promise<void> {
        var storage = this.getPersistanceStorage();
        if (!storage) {
            return;
        }

        var settings = this.getCurrentSettings(flags);
        return storage.setItem(this.getPersistanceKey(), JSON.stringify(settings));
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
                    var sort = sortColumns.findIndex(x => x.columnId == column.id);
                    p.sort = ((sort >= 0) ? ((sortColumns[sort].sortAsc !== false) ? (sort + 1) : (-sort - 1)) : 0);
                }
                settings.columns.push(p);
            }
        }

        if (flags.includeDeleted !== false) {
            settings.includeDeleted = !!this.domNode.querySelector(".s-IncludeDeletedToggle.pressed");
        }

        if (flags.filterItems !== false && (this.filterBar != null) && (this.filterBar.get_store() != null)) {
            settings.filterItems = this.filterBar.get_store().get_items().slice();
        }

        if (flags.quickSearch === true) {
            var qsInput = this.toolbar?.domNode?.querySelector('.s-QuickSearchInput');
            if (qsInput) {
                var qsWidget = tryGetWidget(qsInput, QuickSearchInput);
                if (qsWidget) {
                    settings.quickSearchField = qsWidget.get_field();
                    settings.quickSearchText = qsWidget.domNode.value;
                }
            }
        }

        if (flags.quickFilters !== false && (this.quickFiltersDiv != null) && this.quickFiltersDiv.length > 0) {
            settings.quickFilters = {};
            this.quickFiltersDiv.findAll('.quick-filter-item').forEach(e => {
                var field = e.dataset.qffield;
                if (!field?.length) {
                    return;
                }

                var widget = tryGetWidget('#' + this.uniqueName + '_QuickFilter_' + field, Widget);
                if (!widget)
                    return;

                var qfElement = e as any;
                var saveState = qfElement.qfsavestate;
                var state = typeof saveState === "function" ? saveState(widget) : EditorUtils.getValue(widget);
                settings.quickFilters[field] = state;
                if (flags.quickFilterText === true && e.classList.contains('quick-filter-active')) {

                    var getDisplayText = qfElement.qfdisplaytext;
                    var filterLabel = e.querySelector('.quick-filter-label')?.textContent ?? '';

                    var displayText;
                    if (typeof getDisplayText === "function") {
                        displayText = getDisplayText(widget, filterLabel);
                    }
                    else {
                        displayText = filterLabel + ' = ' + EditorUtils.getDisplayText(widget);
                    }

                    if (displayText?.length) {
                        if (settings.quickFilterText?.length) {
                            settings.quickFilterText += ' ' + (tryGetText('Controls.FilterPanel.And') ?? 'and') + ' ';
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

    getElement(): HTMLElement {
        return this.domNode;
    }

    getGrid(): Grid {
        return this.slickGrid;
    }

    getView(): RemoteView<TItem> {
        return this.view;
    }

    getFilterStore(): FilterStore {
        return (this.filterBar == null) ? null : this.filterBar.get_store();
    }
}