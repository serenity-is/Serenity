import { bindThis } from "@serenity-is/domwise";
import { AutoTooltips, Column, ColumnSort, FormatterContext, GridOptions, SleekGrid, type CellMouseEvent, type ISleekGrid } from "@serenity-is/sleekgrid";
import { Authorization, Criteria, DataGridTexts, Fluent, ListResponse, cssEscape, debounce, getInstanceType, getTypeFullName, getjQuery, nsSerenity, tryGetText, type PropertyItem, type PropertyItemsData } from "../../base";
import { PubSub } from "../../base/pubsub";
import { LayoutTimer, ScriptData, getColumnsData, getColumnsDataAsync, setEquality } from "../../compat";
import { IReadOnly } from "../../interfaces";
import { Format, IRemoteView, PagerOptions, RemoteView, RemoteViewOptions } from "../../slick";
import { FilterableAttribute } from "../../types/attributes";
import { DateEditor } from "../editors/dateeditor";
import { SelectEditor } from "../editors/selecteditor";
import { FilterDisplayBar } from "../filtering/filterdisplaybar";
import { FilterStore } from "../filtering/filterstore";
import { EditLink } from "../helpers/editlink";
import { GridUtils } from "../helpers/gridutils";
import { LazyLoadHelper } from "../helpers/lazyloadhelper";
import { PropertyItemColumnConverter } from "../helpers/propertyitemcolumnconverter";
import { SlickFormatting } from "../helpers/slickformatting";
import { SlickHelper } from "../helpers/slickhelper";
import { ToolButton, Toolbar } from "../widgets/toolbar";
import { Widget, WidgetProps } from "../widgets/widget";
import { getWidgetFrom, tryGetWidget } from "../widgets/widgetutils";
import { getDefaultSortBy, getItemCssClass, propertyItemToQuickFilter, sleekGridOnSort } from "./datagrid-internal";
import { GridPersistenceFlags, PersistedGridSettings, SettingStorage, getCurrentSettings, restoreSettingsFrom, defaultGridPersistenceFlags } from "./datagrid-persistence";
import { IDataGrid } from "./idatagrid";
import { IRowDefinition } from "./irowdefinition";
import { QuickFilter } from "./quickfilter";
import { QuickFilterBar } from "./quickfilterbar";
import { QuickSearchField } from "./quicksearchinput";
import { SlickPager } from "./slickpager";

export { omitAllGridPersistenceFlags } from "./datagrid-persistence";
export type { GridPersistenceFlags, PersistedGridColumn, PersistedGridSettings, SettingStorage } from "./datagrid-persistence";

export class DataGrid<TItem, P = {}> extends Widget<P> implements IDataGrid, IReadOnly {

    static override[Symbol.typeInfo] = this.registerClass(nsSerenity, [IReadOnly]);

    declare private _grid: ISleekGrid<TItem>;
    declare private _isDisabled: boolean;
    declare private _layoutTimer: number;

    declare protected titleDiv: Fluent;
    declare protected toolbar: Toolbar;
    declare protected filterBar: FilterDisplayBar;
    declare protected quickFiltersDiv: Fluent;
    declare protected quickFiltersBar: QuickFilterBar;
    declare protected slickContainer: Fluent;
    declare protected propertyItemsData: PropertyItemsData;
    declare protected initialSettings: PersistedGridSettings;
    declare protected restoringSettings: number;
    declare public view: IRemoteView<TItem>;
    declare public openDialogsAsPanel: boolean;

    declare public static defaultRowHeight: number;
    public static readonly defaultPersistenceFlags = defaultGridPersistenceFlags;
    declare public static defaultPersistenceStorage: SettingStorage;

    declare public static defaultColumnWidthScale: number;
    declare public static defaultColumnWidthDelta: number;

    public static readonly onAfterInit = new PubSub<DataGridInitEvent>();
    public readonly onAfterInit = new PubSub<DataGridInitEvent>();
    public readonly onDataChanged = new PubSub<DataGridChangeEvent>();
    public readonly onPersistence = new PubSub<GridPersistenceEvent>();

    constructor(props: WidgetProps<P>) {
        super(props);

        this.restoringSettings = 0;
        this.domNode.classList.add('s-DataGrid');

        const layout = bindThis(this).layoutTimerCallback;
        this.element.addClass('require-layout').on('layout.' + this.uniqueName, layout);

        if (this.useLayoutTimer())
            this._layoutTimer = LayoutTimer.onSizeChange(() => this.domNode && this.domNode, debounce(layout, 50), { debounceTimes: 1 });

        this.setTitle(this.getInitialTitle());

        var buttons = this.getButtons();
        if (buttons != null) {
            this.createToolbar(buttons);
        }

        this.slickContainer = this.createSlickContainer();
        this.view = this.createView();

        this.syncOrAsyncThen(this.getPropertyItemsData, this.getPropertyItemsDataAsync, itemsData => {
            this.propertyItemsReady(itemsData);
        });
    }

    private layoutTimerCallback() {
        this.layout();
        if (this._layoutTimer != null)
            LayoutTimer.store(this._layoutTimer);
    }

    protected propertyItemsReady(itemsData: PropertyItemsData) {
        this.propertyItemsData = itemsData;
        const sleekGrid = (this as any).createSlickGrid();
        this._grid ??= sleekGrid;
        this.initSleekGrid();

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

        // call before restoring settings so global handlers can add mixins/plugins before that
        DataGrid.onAfterInit.notify({ dataGrid: this });
        this.onAfterInit.notify({ dataGrid: this });
        this.afterInit();

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
        if (!this.domNode || !Fluent.isVisibleLike(this.domNode) || !this.slickContainer || !this._grid)
            return;

        var responsiveHeight = this.domNode.classList.contains('responsive-height');
        var madeAutoHeight = this._grid != null && this._grid.getOptions().autoHeight;
        var shouldAutoHeight = responsiveHeight && window.innerWidth < 768;

        if (shouldAutoHeight) {
            if (!madeAutoHeight) {
                this._grid.setOptions({ autoHeight: true });
            }
        }
        else if (madeAutoHeight) {
            this.slickContainer.getNode().style.height = "";
            this.slickContainer.findAll('.slick-viewport').forEach(x => x.style.height = "");
            this._grid.setOptions({ autoHeight: false });
        }

        this._grid.resizeCanvas();
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
            this.quickFiltersDiv = Fluent(<div class="quick-filters-bar" />);
            if (this.toolbar) {
                this.toolbar.domNode.append(<div class="clear" />, this.quickFiltersDiv.getNode());
            }
            else {
                this.quickFiltersDiv.appendTo(Fluent(<div class="s-Toolbar" />).insertBefore(this.slickContainer));
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

    public static propertyItemToQuickFilter(item: PropertyItem): QuickFilter<any, any> | null {
        return propertyItemToQuickFilter(item);
    }

    protected findQuickFilter<TWidget>(type: { new(...args: any[]): TWidget }, field: string): TWidget {
        if (this.quickFiltersBar != null)
            return this.quickFiltersBar.find(type, field);

        const selector = '#' + cssEscape(this.uniqueName + '_QuickFilter_' + field);
        return getWidgetFrom(this.domNode?.querySelector(selector) ?? selector, type);
    }

    protected tryFindQuickFilter<TWidget>(type: { new(...args: any[]): TWidget }, field: string): TWidget {
        if (this.quickFiltersBar != null)
            return this.quickFiltersBar.tryFind(type, field);

        const selector = '#' + cssEscape(this.uniqueName + '_QuickFilter_' + field);
        return tryGetWidget(this.domNode?.querySelector(selector) ?? selector, type);
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

    public override destroy() {

        this.onAfterInit?.clear();
        this.onDataChanged?.clear();
        this.onPersistence?.clear();

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

        if (this._grid) {
            this._grid.onClick.clear();
            this._grid.onSort.clear();
            this._grid.onColumnsResized.clear();
            this._grid.onColumnsReordered.clear();
            this._grid.destroy();
            this._grid = null;
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
        return getItemCssClass(item, this.getIsActiveProperty(), this.getIsDeletedProperty());
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
                this.persistSettings();
                this.view && (this.view.seekToPage = 1);
                this.refresh();
            }
        });
    }

    /**
     * Creates initial column set for this grid. This column set is then passed
     * to postProcessColumns to adjust widths etc, and then used as the initial
     * columns for the slickgrid.
     */
    protected createColumns(): Column<TItem>[] {
        const items = this.getPropertyItems();
        return this.propertyItemsToColumns(items);
    }

    /**
     * Creates the SleekGrid columns. This method calls createColumns (via getColumns for compatibility) and then post processes them.
     * @returns The SleekGrid columns.
     */
    protected createSleekColumns(): Column<TItem>[] {
        const columns = (this as any).getColumns();
        return this.postProcessColumns(columns || []) || [];
    }

    /** Override initSleekGrid to add plugins to the sleekgrid */
    protected createSlickGrid(): ISleekGrid<TItem> | null {
        const columns = this.createSleekColumns();
        const slickOptions = this.getSlickOptions();
        this._grid = new SleekGrid(this.slickContainer.getNode(), this.view as any, columns, slickOptions);
        this._grid.registerPlugin(new AutoTooltips({ enableForHeaderCells: true }));
        this.setInitialSortOrder();
        return this._grid;
    }

    protected initSleekGrid(): void {
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

        this._grid.setSortColumns(mapped);
    }

    itemAt(row: number): TItem {
        return this._grid.getDataItem(row);
    }

    itemId(item: TItem): any {
        return (item as any)[this.getIdProperty()];
    }

    rowCount() {
        return this._grid.getDataLength();
    }

    getItems(): TItem[] {
        return this.view.getItems();
    }

    setItems(value: TItem[]) {
        this.view.setItems(value, true);
    }

    protected bindToSlickEvents() {
        this._grid.onSort.subscribe((_, p) => {
            sleekGridOnSort(this.view, p);
            this.persistSettings();
        });

        this._grid.onClick.subscribe((e: CellMouseEvent) => {
            this.onClick(e, e.row, e.cell);
        });

        this._grid.onColumnsReordered.subscribe(() => {
            return this.persistSettings();
        });

        this._grid.onColumnsResized.subscribe(() => {
            return this.persistSettings();
        });
    }

    protected getAddButtonCaption(): string {
        return DataGridTexts.asTry().NewButton ?? 'New';
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
            this.editItemOfType(target.dataset.itemType, target.dataset.itemId);
        }
    }

    protected viewDataChanged(e: any, rows: TItem[]): void {
        this.onDataChanged.notify({ dataGrid: this });
        this.markupReady();
        this.layout();
    }

    protected bindToViewEvents(): void {
        var self = this;

        this.view.onDataChanged.subscribe(function (e, d) {
            return self.viewDataChanged(e, d);
        });

        this.view.onSubmit = function () {
            return self.onViewSubmit();
        };

        this.view.setFilter(function (item) {
            return self.onViewFilter(item);
        });

        this.view.onProcessData = function (response) {
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
        var columns = this._grid.getColumns();
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
        return Fluent(<div class="grid-container" />).appendTo(this.domNode);
    }

    protected createView(): IRemoteView<TItem> {
        var opt = this.getViewOptions();
        return new RemoteView<TItem>(opt) as any;
    }

    protected getDefaultSortBy(): any[] {
        return getDefaultSortBy(this._grid);
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
        }).init();
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
                    this.titleDiv = Fluent(<div class="grid-title"><div class="title-text" /></div>)
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
        return SlickFormatting.itemLink(itemType ?? this.getItemType(), idField ?? this.getIdProperty(), text, cssClass, encode);
    }

    /** Renders an edit link for the item in current row. Returns a DocumentFragment for non-data rows, and an anchor element otherwise. */
    public EditLink = (props: {
        /** formatter context (contains item, value etc) */
        context?: FormatterContext,
        /** The id of the entity to link to. If not provided it will be taken from ctx.item[idField] */
        id?: string,
        /** The name of the field in item that contains the entity id. Defaults to idProperty. Used if id is not provided. */
        idField?: string,
        /** The item type to link to. Defaults to this.getItemType() */
        itemType?: string,
        /** Extra CSS class to add to the link element besides s-EditLink. Optional. */
        cssClass?: string,
        /** The tabindex to assign to the link, default is undefined */
        tabindex?: number,
        /** @deprecated Use tabindex. */
        tabIndex?: number,
        /** The link text. If not provided it will be taken from ctx.escape(ctx.value) */
        children?: any
    }): any => {
        let children = props.children;
        if (children == null && props.context != null) {
            children = props.context.value?.toString() ?? "";
        }

        if ((props?.context?.item as any)?.__nonDataRow ||
            props.context?.purpose === "group-header" ||
            props.context?.purpose === "group-totals" ||
            props.context?.purpose === "grand-totals") {
            return <>{children}</>;
        }

        let id = props.id;
        if (id === void 0 && props.context?.item != null) {
            id = props.context.item[props.idField ?? this.getIdProperty()];
        }

        return EditLink({
            itemType: props.itemType ?? this.getItemType(),
            itemId: id,
            children: children,
            cssClass: props.cssClass,
            tabindex: props.tabindex ?? props.tabIndex,
        })
    }

    protected getColumnsKey(): string {
        return null;
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

    /** @deprecated override createColumns */
    protected getColumns(): Column<TItem>[] {
        return this.createColumns();
    }

    protected wrapFormatterWithEditLink(column: Column, item: PropertyItem) {
        const orgFormat = column.format;
        const itemType = item.editLinkItemType || null;
        const idField = item.editLinkIdField || null;
        const linkClass = item.editLinkCssClass || null;
        column.format = this.itemLink(itemType, idField,
            ctx => orgFormat != null ? orgFormat(ctx) : ctx.escape(),
            () => linkClass, /*encode*/false);

        if (idField) {
            column.referencedFields = column.referencedFields || [];
            column.referencedFields.push(idField);
        }
    }

    protected propertyItemsToColumns(propertyItems: PropertyItem[]): Column[] {
        var columns = PropertyItemColumnConverter.toColumns(propertyItems);
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

    declare private _readonly: boolean;

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

    declare private _localTextDbPrefix: string;

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

        return void 0;
    }

    declare private _idProperty: string;

    protected getIdProperty(): string {
        if (this._idProperty != null)
            return this._idProperty;

        var rowDefinition = this.getRowDefinition();
        if (rowDefinition)
            return this._idProperty = rowDefinition.idProperty ?? '';

        return this._idProperty = 'ID';
    }

    protected getIsDeletedProperty(): string {
        return this.getRowDefinition()?.isDeletedProperty;
    }

    declare private _isActiveProperty: string;

    protected getIsActiveProperty(): string {
        if (this._isActiveProperty != null)
            return this._isActiveProperty;

        var rowDefinition = this.getRowDefinition();
        if (rowDefinition)
            return this._isActiveProperty = rowDefinition.isActiveProperty ?? '';

        return this._isActiveProperty = '';
    }

    protected updateDisabledState(): void {
        this.slickContainer.toggleClass('ui-state-disabled', !!this._isDisabled);
    }

    protected resizeCanvas(): void {
        this._grid?.resizeCanvas();
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
        this.persistSettings();
        this.view && (this.view.seekToPage = 1);
        this.refresh();
    }

    protected getPersistenceStorage(): SettingStorage {
        if ((this as any).getPersistanceStorage) return (this as any).getPersistanceStorage(); // compat

        return DataGrid.defaultPersistenceStorage;
    }

    protected getPersistenceKey(): string {
        if ((this as any).getPersistanceKey) return ((this as any).getPersistanceKey); // compat

        var key = 'GridSettings:';
        var path = window.location.pathname;
        if (path) {
            key += path.substring(1).split(String.fromCharCode(47)).slice(0, 2).join('/') + ':';
        }

        key += getTypeFullName(getInstanceType(this));
        return key;
    }

    protected gridPersistenceFlags(): GridPersistenceFlags {
        if ((this as any).gridPersistanceFlags) return (this as any).gridPersistanceFlags; // compat

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
        var storage = this.getPersistenceStorage();
        if (storage == null)
            return null;

        function fromJson(json: string) {
            json = json?.trim();
            if (json?.startsWith('{') && json.endsWith('}'))
                return JSON.parse(json);
            return null;
        }

        var jsonOrPromise = storage.getItem(this.getPersistenceKey());
        if ((jsonOrPromise as any)?.then)
            return (jsonOrPromise as Promise<string>).then(json => fromJson(json));

        return fromJson(jsonOrPromise as string);
    }

    protected restoreSettings(settings?: PersistedGridSettings, flags?: GridPersistenceFlags): void | Promise<void> {
        if (settings != null)
            return this.restoreSettingsFrom(settings, flags);

        var settingsOrPromise = this.getPersistedSettings();
        if ((settingsOrPromise as any)?.then)
            return (settingsOrPromise as Promise<PersistedGridSettings>).then((s) => this.restoreSettingsFrom(s));

        this.restoreSettingsFrom(settingsOrPromise as PersistedGridSettings);
    }

    protected restoreSettingsFrom(settings: PersistedGridSettings, flags?: GridPersistenceFlags): void {
        if (!this._grid || !settings)
            return;

        this.view.beginUpdate();
        this.restoringSettings++;
        try {

            const flagsDefault = this.gridPersistenceFlags();
            const event: GridPersistenceEvent = {
                after: false,
                dataGrid: this,
                flagsArgument: flags,
                flagsDefault,
                flagsToUse: flags || flagsDefault,
                restoring: true,
                persisting: false,
                settings,
            };

            this.onPersistence.notify(event);
            event.flagsToUse ??= event.flagsDefault;

            restoreSettingsFrom({
                canShowColumn: bindThis(this).canShowColumn,
                filterStore: this.filterBar?.get_store(),
                flags: event.flagsToUse,
                includeDeletedToggle: this.domNode.querySelector('.s-IncludeDeletedToggle'),
                quickFiltersDiv: this.quickFiltersDiv,
                sleekGrid: this._grid,
                settings: event.settings,
                toolbarNode: this.toolbar?.domNode,
                uniqueName: this.uniqueName,
                view: this.view
            });

            event.after = true;
            this.onPersistence.notify(event);
        }
        finally {
            this.restoringSettings--;
            this.view.endUpdate();
        }
    }

    private _persistenceLock: number = 0;

    public persistenceLock() {
        this._persistenceLock++;
    }

    public persistenceUnlock() {
        this._persistenceLock--;
    }

    public persistSettings(flags?: GridPersistenceFlags): void | Promise<void> {
        if (this._persistenceLock > 0)
            return;

        var storage = this.getPersistenceStorage();
        if (!storage) {
            return;
        }

        var settings = this.getCurrentSettings(flags);
        return storage.setItem(this.getPersistenceKey(), JSON.stringify(settings));
    }

    public getCurrentSettings(flags?: GridPersistenceFlags) {

        const flagsDefault = this.gridPersistenceFlags();
        const event: GridPersistenceEvent = {
            after: false,
            dataGrid: this,
            flagsArgument: flags,
            flagsDefault,
            flagsToUse: flags || flagsDefault,
            restoring: false,
            persisting: true,
            settings: null
        }

        event.flagsToUse ||= event.flagsDefault;

        event.settings = getCurrentSettings({
            filterStore: this.filterBar?.get_store(),
            flags,
            includeDeletedToggle: this.domNode.querySelector('.s-IncludeDeletedToggle'),
            quickFiltersDiv: this.quickFiltersDiv,
            sleekGrid: this._grid,
            toolbarNode: this.toolbar?.domNode,
            uniqueName: this.uniqueName
        });
        event.after = true;
        this.onPersistence.notify(event);
        return event.settings;
    }

    getElement(): HTMLElement {
        return this.domNode;
    }

    getGrid(): ISleekGrid<TItem> {
        return this._grid;
    }

    public get sleekGrid() { return this._grid; }
    protected set sleekGrid(value: ISleekGrid<TItem>) { this._grid = value; }

    /** @deprecated Use sleekGrid or getGrid() */
    public get slickGrid() { return this._grid; }

    getView(): IRemoteView<TItem> {
        return this.view;
    }

    getFilterStore(): FilterStore {
        return (this.filterBar == null) ? null : this.filterBar.get_store();
    }

    public get allColumns(): Column[] { return this._grid?.getAllColumns() }
    public get columns() { return this._grid?.getColumns(); }

    /** @obsolete use defaultPersistenceStorage, this one has a typo */
    public static get defaultPersistanceStorage(): SettingStorage { return DataGrid.defaultPersistenceStorage; }
    /** @obsolete use defaultPersistenceStorage, this one has a typo */
    public static set defaultPersistanceStorage(value: SettingStorage) { DataGrid.defaultPersistenceStorage = value; }
}

export interface DataGridEvent {
    dataGrid: DataGrid<any>;
}

export type DataGridChangeEvent = DataGridEvent;
export type DataGridInitEvent = DataGridEvent;

export interface GridPersistenceEvent extends DataGridEvent {
    after: boolean;
    flagsArgument: GridPersistenceFlags;
    flagsDefault: GridPersistenceFlags;
    flagsToUse: GridPersistenceFlags;
    settings: PersistedGridSettings;
    readonly restoring: boolean;
    readonly persisting: boolean;
}