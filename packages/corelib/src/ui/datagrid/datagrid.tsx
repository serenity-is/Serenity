import { bindThis } from "@serenity-is/domwise";
import { AutoTooltips, Column, ColumnSort, FormatterContext, SleekGrid, type CellMouseEvent, type GridOptions, type GridSortEvent, type ISleekGrid } from "@serenity-is/sleekgrid";
import { Authorization, Criteria, DataGridTexts, Fluent, ListResponse, cssEscape, debounce, getInstanceType, getTypeFullName, getjQuery, nsSerenity, tryGetText, type PropertyItem, type PropertyItemsData } from "../../base";
import { PubSub } from "../../base/pubsub";
import { LayoutTimer, ScriptData, getColumnsData, getColumnsDataAsync, setEquality } from "../../compat";
import { IReadOnly } from "../../interfaces";
import { Format, IRemoteView, PagerOptions, RemoteView, RemoteViewOptions } from "../../slick";
import { AdvancedFilteringAttribute } from "../../types/attributes";
import { DateEditor } from "../editors/dateeditor";
import { SelectEditor } from "../editors/selecteditor";
import { FilterDisplayBar } from "../filtering/filterdisplaybar";
import { FilterStore } from "../filtering/filterstore";
import { EditLink, skipEditLinkFormatPurposes } from "../helpers/editlink";
import { GridUtils } from "../helpers/gridutils";
import { LazyLoadHelper } from "../helpers/lazyloadhelper";
import { PropertyItemColumnConverter } from "../helpers/propertyitemcolumnconverter";
import { SlickFormatting } from "../helpers/slickformatting";
import { SlickHelper } from "../helpers/slickhelper";
import { ToolButton, Toolbar } from "../widgets/toolbar";
import { Widget, WidgetProps } from "../widgets/widget";
import { getWidgetFrom, tryGetWidget } from "../widgets/widgetutils";
import type { AutoRegisterArgs } from "./datagrid-autoregisterargs";
import { dataGridDefaults } from "./datagrid-defaults";
import { getDefaultSortBy, getItemCssClass, propertyItemToQuickFilter, sleekGridOnSort } from "./datagrid-internal";
import { GridPersistenceFlags, PersistedGridSettings, SettingStorage, getCurrentSettings, restoreSettingsFrom, type DataGridPersistenceEvent } from "./datagrid-persistence";
import { IDataGrid } from "./idatagrid";
import { IRowDefinition } from "./irowdefinition";
import { QuickFilter } from "./quickfilter";
import { QuickFilterBar } from "./quickfilterbar";
import { QuickSearchField } from "./quicksearchinput";
import { SlickPager } from "./slickpager";

export type { AutoRegisterArgs, AutoRegisterHandler } from "./datagrid-autoregisterargs";
export { omitAllGridPersistenceFlags } from "./datagrid-persistence";
export type { DataGridPersistenceEvent, GridPersistenceFlags, PersistedGridColumn, PersistedGridSettings, SettingStorage } from "./datagrid-persistence";

/**
 * Base data grid widget that renders tabular data using SleekGrid, with
 * support for columns, sorting, filtering, quick filters, paging, persistence,
 * and toolbar integration.
 * @typeParam TItem - Row type displayed in the grid.
 * @typeParam P - Widget props type.
 */
export class DataGrid<TItem, P = {}> extends Widget<P> implements IDataGrid, IReadOnly {

    static override[Symbol.typeInfo] = this.registerClass(nsSerenity, [IReadOnly]);

    declare private _grid: ISleekGrid<TItem>;
    declare private _initialSettings: PersistedGridSettings;
    declare private _layoutTimer: number;

    /** The title element. */
    declare protected titleDiv: Fluent;
    /** The toolbar widget. */
    declare protected toolbar: Toolbar;
    /** The advanced filter bar widget. */
    declare protected filterBar: FilterDisplayBar;
    /** The quick filters container element. */
    declare protected quickFiltersDiv: Fluent;
    /** The quick filter bar widget. */
    declare protected quickFiltersBar: QuickFilterBar;
    /** The container element that hosts the grid. */
    declare protected slickContainer: Fluent;
    /** The property items data for this grid. */
    declare protected propertyItemsData: PropertyItemsData;
    /** Counter tracking nested settings restoration. */
    declare protected restoringSettings: number;
    /** The remote view used for paging and server communication. */
    declare public view: IRemoteView<TItem>;

    /** Whether dialogs opened from this grid should be shown as panels. */
    declare public openDialogsAsPanel: boolean;

    /** Default options shared by all data grid instances. */
    public static readonly defaultOptions = dataGridDefaults;

    /** Default row height used when creating grids. */
    static get defaultRowHeight() { return dataGridDefaults.rowHeight; }
    /** Default storage used for grid persistence. */
    static get defaultPersistenceStorage() { return dataGridDefaults.persistenceStorage; }
    /** Sets the default storage used for grid persistence. */
    static set defaultPersistenceStorage(value: SettingStorage) { dataGridDefaults.persistenceStorage = value; }
    /** Default column width scale applied to all grids. */
    static get defaultColumnWidthScale() { return dataGridDefaults.columnWidthScale; }
    /** Sets the default column width scale applied to all grids. */
    static set defaultColumnWidthScale(value: number) { dataGridDefaults.columnWidthScale = value; }
    /** Default column width delta applied to all grids. */
    static get defaultColumnWidthDelta() { return dataGridDefaults.columnWidthDelta; }
    /** Sets the default column width delta applied to all grids. */
    static set defaultColumnWidthDelta(value: number) { dataGridDefaults.columnWidthDelta = value; }

    /** Static event raised after any grid is initialized. */
    public static readonly onAfterInit = new PubSub<DataGridInitEvent>();
    /** Raised after this grid is initialized. */
    public readonly onAfterInit = new PubSub<DataGridInitEvent>();
    /** Raised to determine whether the grid can submit its view. */
    public readonly onCanSubmit = new PubSub<DataGridSubmitEvent>();
    /** Raised when the grid data changes. */
    public readonly onDataChanged = new PubSub<DataGridChangeEvent>();
    /** Raised while filtering items in the view. */
    public readonly onFiltering = new PubSub<DataGridFilteringEvent<TItem>>();
    /** Raised before/after persisting or restoring grid settings. */
    public readonly onPersistence = new PubSub<DataGridPersistenceEvent>();
    /** Raised when the view processes a list response. */
    public readonly onProcessData = new PubSub<DataGridProcessEvent<TItem>>();
    /** Raised to determine whether the view submit should proceed. */
    public readonly onSubmitting = new PubSub<DataGridSubmitEvent>();
    /** Raised after view parameters are prepared for submission. */
    public readonly onSetViewParams = new PubSub<DataGridEvent>();

    /**
     * Creates a data grid widget.
     * @param props - Widget props forwarded to the base widget.
     */
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

    /**
     * Hook invoked when the grid is registered as an auto-registering plugin.
     * @param args - Auto-registration arguments.
     */
    protected autoRegisteringPlugin(args: AutoRegisterArgs): void {
    }

    private layoutTimerCallback() {
        this.layout();
        if (this._layoutTimer != null)
            LayoutTimer.store(this._layoutTimer);
    }

    /**
     * Called once property items are available; creates the grid, filter bar,
     * pager, quick filters, and restores persisted settings.
     * @param itemsData - Property items and additional items for the grid.
     */
    protected propertyItemsReady(itemsData: PropertyItemsData) {
        this.propertyItemsData = itemsData;
        const sleekGrid = (this as any).createSlickGrid();
        this._grid ??= sleekGrid;
        this.initSleekGrid();

        if (this.enableAdvancedFiltering()) {
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

    /**
     * Hook invoked after the grid is initialized and settings are restored.
     */
    protected afterInit() {
    }

    /**
     * Whether the grid should load property items asynchronously.
     * @returns True when async loading is used.
     */
    protected useAsync() {
        return false;
    }

    /**
     * Whether the grid should use the layout timer for responsive resizing.
     * @returns True when the layout timer is used.
     */
    protected useLayoutTimer() {
        return true;
    }

    /**
     * Recalculates the grid layout, handling responsive height behavior.
     */
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

    /**
     * Returns the initial title shown above the grid.
     * @returns The title text, or null for no title.
     */
    protected getInitialTitle(): string {
        return null;
    }

    /**
     * Hook for subclasses to add extra toolbar buttons or controls.
     */
    protected createToolbarExtensions(): void {
    }

    /**
     * Ensures the quick filter bar exists and returns it.
     * @returns The quick filter bar instance.
     */
    protected ensureQuickFilterBar(): QuickFilterBar {

        if (this.quickFiltersDiv == null)
            this.createQuickFilters([]);

        return this.quickFiltersBar;
    }

    /**
     * Creates the quick filter bar with the given filters.
     * @param filters - Quick filter definitions to render.
     */
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

    /**
     * Returns the quick filter definitions derived from the grid columns.
     * @returns Quick filter definitions for columns marked as quick filters.
     */
    protected getQuickFilters(): QuickFilter<Widget<any>, any>[] {
        return this.allColumns.filter(function (x) {
            return x.sourceItem &&
                x.sourceItem.quickFilter === true &&
                (x.sourceItem.readPermission == null ||
                    Authorization.hasPermission(x.sourceItem.readPermission));
        }).map(x => DataGrid.propertyItemToQuickFilter(x.sourceItem))
            .filter(x => x != null);
    }

    /**
     * Converts a property item to a quick filter definition.
     * @param item - Property item to convert.
     * @returns The quick filter definition, or null if not applicable.
     */
    public static propertyItemToQuickFilter(item: PropertyItem): QuickFilter<any, any> | null {
        return propertyItemToQuickFilter(item);
    }

    /**
     * Finds a quick filter widget by field name.
     * @param type - Widget constructor type.
     * @param field - Field name of the quick filter.
     * @returns The widget instance.
     */
    protected findQuickFilter<TWidget>(type: { new(...args: any[]): TWidget }, field: string): TWidget {
        if (this.quickFiltersBar != null)
            return this.quickFiltersBar.find(type, field);

        const selector = '#' + cssEscape(this.uniqueName + '_QuickFilter_' + field);
        return getWidgetFrom(this.domNode?.querySelector(selector) ?? selector, type);
    }

    /**
     * Tries to find a quick filter widget by field name.
     * @param type - Widget constructor type.
     * @param field - Field name of the quick filter.
     * @returns The widget instance, or null if not found.
     */
    protected tryFindQuickFilter<TWidget>(type: { new(...args: any[]): TWidget }, field: string): TWidget {
        if (this.quickFiltersBar != null)
            return this.quickFiltersBar.tryFind(type, field);

        const selector = '#' + cssEscape(this.uniqueName + '_QuickFilter_' + field);
        return tryGetWidget(this.domNode?.querySelector(selector) ?? selector, type);
    }

    /**
     * Creates the include-deleted toggle button when the row type supports it.
     */
    protected createIncludeDeletedButton(): void {
        if (this.getIsActiveProperty() || this.getIsDeletedProperty())
            GridUtils.addIncludeDeletedToggle(this.toolbar.domNode, this.view, null, false);
    }

    /**
     * Returns the quick search fields available for this grid.
     * @returns The quick search fields, or null for none.
     */
    protected getQuickSearchFields(): QuickSearchField[] {
        return null;
    }

    /**
     * Creates the quick search input in the toolbar.
     */
    protected createQuickSearchInput(): void {
        const input = GridUtils.addQuickSearch({
            container: this.toolbar.domNode, 
            view: this.view, 
            fields: this.getQuickSearchFields(), 
            beforeSearch: () => this.persistSettings(null)
        });
        input?.domNode?.setAttribute('id', this.idPrefix + 'QuickSearchInput');
    }

    /**
     * Cleans up event subscriptions, widgets, and the underlying grid.
     */
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
            this._grid.onClick.unsubscribe(this.handleGridClick);
            this._grid.onSort.unsubscribe(this.handleGridSort);
            this._grid.onColumnsResized.unsubscribe(this.handleGridColumnsResized);
            this._grid.onColumnsReordered.unsubscribe(this.handleGridColumnsReordered);
            this._grid.destroy();
            this._grid = null;
        }

        if (this.view) {
            this.view.onDataChanged.unsubscribe(this.viewDataChanged);
            this.view.onSubmit = null;
            this.view.setFilter(null);
            this.view = null;
        }

        this.titleDiv = null;
        super.destroy();
    }

    /**
     * Returns the CSS class for a grid row based on its active/deleted state.
     * @param item - The row item.
     * @param index - The row index.
     * @returns The CSS class name, or an empty string.
     */
    protected getItemCssClass(item: TItem, index: number): string {
        return getItemCssClass(item, this.getIsActiveProperty(), this.getIsDeletedProperty());
    }

    /**
     * Returns row metadata (e.g. CSS classes) for the given item.
     * @param item - The row item.
     * @param index - The row index.
     * @returns Row metadata object.
     */
    protected getItemMetadata(item: TItem, index: number): any {
        var itemClass = this.getItemCssClass(item, index);
        if (!itemClass) {
            return new Object();
        }
        return { cssClasses: itemClass };
    }

    /**
     * Applies defaults and width adjustments to the given columns.
     * @param columns - Columns to post-process.
     * @returns The processed columns.
     */
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

    /**
     * Returns the width delta applied to all columns.
     * @returns The column width delta.
     */
    protected getColumnWidthDelta() {
        return DataGrid.defaultColumnWidthDelta ?? 0;
    }

    /**
     * Returns the width scale applied to all columns.
     * @returns The column width scale.
     */
    protected getColumnWidthScale() {
        return DataGrid.defaultColumnWidthScale ?? 1;
    }

    /**
     * Performs the initial data population, optionally waiting until visible.
     */
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

    /**
     * Whether the given column can be used in the advanced filter bar.
     * @param column - Column to check.
     * @returns True when the column is filterable.
     */
    protected canFilterColumn(column: Column): boolean {
        return (column.sourceItem != null &&
            column.sourceItem.notFilterable !== true &&
            (column.sourceItem.readPermission == null ||
                Authorization.hasPermission(column.sourceItem.readPermission)));
    }

    /**
     * Initializes the filter bar store with the filterable columns.
     */
    protected initializeFilterBar() {

        this.filterBar.set_store(new FilterStore(
            this.allColumns
                .filter(c => this.canFilterColumn(c))
                .map(x => x.sourceItem)));

        this.filterBar.get_store().add_changed(bindThis(this).filterStoreChanged);
    }

    /**
     * Handles filter store changes by persisting settings and refreshing.
     */
    protected filterStoreChanged() {
        if (this.restoringSettings <= 0) {
            this.persistSettings();
            this.view && (this.view.seekToPage = 1);
            this.refresh();
        }
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

    /**
     * Creates the underlying SleekGrid instance with the processed columns.
     * @returns The created grid instance.
     */
    protected createSlickGrid(): ISleekGrid<TItem> | null {
        const columns = this.createSleekColumns();
        const slickOptions = this.getSlickOptions();
        this._grid = new SleekGrid(this.slickContainer.getNode(), this.view as any, columns, slickOptions);
        this._grid.registerPlugin(new AutoTooltips({ enableForHeaderCells: true }));
        this.setInitialSortOrder();
        return this._grid;
    }

    /**
     * Hook for subclasses to initialize the grid after creation.
     */
    protected initSleekGrid(): void {
    }

    /**
     * Applies the default sort order to the grid and view.
     */
    protected setInitialSortOrder(): void {
        var sortBy = this.getDefaultSortBy();

        if (this.view) {
            this.view.sortBy = Array.prototype.slice.call(sortBy);
        }

        var mapped = sortBy.map(function (s): ColumnSort {
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
        });

        this._grid.setSortColumns(mapped);
    }

    /**
     * Returns the item at the given row index.
     * @param row - Row index.
     * @returns The item at that row.
     */
    itemAt(row: number): TItem {
        return this._grid.getDataItem(row);
    }

    /**
     * Returns the id of the given item using the grid id property.
     * @param item - The item.
     * @returns The item id.
     */
    itemId(item: TItem): any {
        return (item as any)[this.getIdProperty()];
    }

    /**
     * Returns the number of rows in the grid.
     * @returns The row count.
     */
    rowCount() {
        return this._grid.getDataLength();
    }

    /**
     * Returns the items currently displayed in the grid.
     * @returns The grid items.
     */
    getItems(): TItem[] {
        return this.view.getItems();
    }

    /**
     * Sets the items displayed in the grid.
     * @param value - The items to display.
     */
    setItems(value: TItem[]) {
        this.view.setItems(value, true);
    }

    /**
     * Handles grid sort events by applying the sort and persisting settings.
     * @param e - Grid sort event.
     */
    protected handleGridSort(e: GridSortEvent) {
        sleekGridOnSort(this.view, e.args);
        this.persistSettings();
    }

    /**
     * Handles grid cell click events by delegating to onClick.
     * @param e - Cell mouse event.
     */
    protected handleGridClick(e: CellMouseEvent) {
        this.onClick(e, e.row, e.cell);
    }

    /**
     * Persists settings when columns are reordered.
     */
    protected handleGridColumnsReordered() {
        this.persistSettings();
    }

    /**
     * Persists settings when columns are resized.
     */
    protected handleGridColumnsResized() {
        this.persistSettings();
    }

    /**
     * Subscribes to the underlying grid events.
     */
    protected bindToSlickEvents() {
        const boundThis = bindThis(this);
        this._grid.onSort.subscribe(boundThis.handleGridSort);
        this._grid.onClick.subscribe(boundThis.handleGridClick);
        this._grid.onColumnsReordered.subscribe(boundThis.handleGridColumnsReordered);
        this._grid.onColumnsResized.subscribe(boundThis.handleGridColumnsResized);
    }

    /**
     * Returns the caption for the add/new button.
     * @returns The add button caption.
     */
    protected getAddButtonCaption(): string {
        return DataGridTexts.asTry().NewButton ?? 'New';
    }

    /**
     * Returns the toolbar buttons for this grid.
     * @returns Tool button definitions.
     */
    protected getButtons(): ToolButton[] {
        return [];
    }

    /**
     * Opens an edit dialog for the given entity or id.
     * @param entityOrId - Entity instance or identifier to edit.
     */
    protected editItem(entityOrId: any): void {
        throw new Error("Not Implemented!");
    }

    /**
     * Opens an edit dialog for a specific item type.
     * @param itemType - Item type key.
     * @param entityOrId - Entity instance or identifier to edit.
     */
    protected editItemOfType(itemType: string, entityOrId: any): void {
        if (itemType === this.getItemType()) {
            this.editItem(entityOrId);
            return;
        }

        throw new Error("Not Implemented!");
    }

    /**
     * Handles cell clicks, opening edit links when clicked.
     * @param e - Click event.
     * @param row - Row index.
     * @param cell - Cell index.
     */
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

    /**
     * Handles view data changes by notifying subscribers and relaying out.
     */
    protected viewDataChanged(): void {
        this.onDataChanged.notify({ dataGrid: this });
        this.markupReady();
        this.layout();
    }

    /**
     * Subscribes to view events for filtering, submitting, and processing data.
     */
    protected bindToViewEvents(): void {
        const boundThis = bindThis(this);
        this.view.onDataChanged.subscribe(boundThis.viewDataChanged);
        this.view.setFilter(boundThis.handleViewFilter);
        this.view.onSubmit = boundThis.handleViewSubmit;
        this.view.onProcessData = boundThis.handleViewProcessData;
    }

    /**
     * Filters a view item, notifying the onFiltering subscribers.
     * @param item - The item to filter.
     * @returns True when the item matches.
     */
    protected handleViewFilter(item: TItem): boolean { 
        if (!this.onViewFilter(item))
            return false;

        const e: DataGridFilteringEvent<TItem> = { dataGrid: this, item: item, isMatch: true };
        this.onFiltering.notify(e, { isCancelled: x => !x.isMatch });
        return e.isMatch;
    }

    /**
     * Processes a list response, notifying the onProcessData subscribers.
     * @param response - The list response.
     * @returns The processed response.
     */
    protected handleViewProcessData(response: ListResponse<TItem>): ListResponse<TItem> {
        response = this.onViewProcessData(response);
        const e: DataGridProcessEvent<TItem> = { dataGrid: this, response: response };
        this.onProcessData.notify(e);
        return e.response;
    }

    /**
     * Handles view submission, notifying the onSubmitting subscribers.
     * @returns True when the submit should proceed.
     */
    protected handleViewSubmit(): boolean {
        if (!this.onViewSubmit())
            return false;

        const e: DataGridSubmitEvent = { dataGrid: this, cancel: false };
        this.onSubmitting.notify(e, { isCancelled: x => x.cancel });
        return !e.cancel;
    }

    /**
     * Hook for subclasses to process a list response before it is applied.
     * @param response - The list response.
     * @returns The processed response.
     */
    protected onViewProcessData(response: ListResponse<TItem>): ListResponse<TItem> {
        return response;
    }

    /**
     * Hook for subclasses to filter view items.
     * @param item - The item to filter.
     * @returns True when the item should be included.
     */
    protected onViewFilter(item: TItem): boolean {
        return true;
    }

    /**
     * Collects the fields and referenced fields of all columns into the given map.
     * @param include - Map to populate with column field names.
     */
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

    /**
     * Sets the Criteria view parameter from the active filter store criteria.
     */
    protected setCriteriaParameter(): void {
        delete this.view.params['Criteria'];
        if (this.filterBar) {
            var criteria = this.filterBar.get_store().get_activeCriteria();
            if (!Criteria.isEmpty(criteria)) {
                this.view.params.Criteria = criteria;
            }
        }
    }

    /**
     * Sets an equality filter on the view parameters.
     * @param field - Field name.
     * @param value - Equality value.
     */
    protected setEquality(field: string, value: any): void {
        setEquality(this.view.params, field, value);
    }

    /**
     * Sets the IncludeColumns view parameter from the grid columns.
     */
    protected setIncludeColumnsParameter(): void {
        var include = {};
        this.getIncludeColumns(include);
        var array = [];
        for (var key of Object.keys(include)) {
            array.push(key);
        }
        this.view.params.IncludeColumns = array;
    }

    /**
     * Prepares all view parameters and notifies the onSetViewParams subscribers.
     */
    protected setViewParams(): void {
        this.setCriteriaParameter();
        this.setIncludeColumnsParameter();
        this.invokeSubmitHandlers();
        this.onSetViewParams.notify({ dataGrid: this });
    }

    /**
     * Hook invoked before the view submits; prepares parameters and checks loadability.
     * @returns True when the view can load.
     */
    protected onViewSubmit(): boolean {
        this.setViewParams();

        if (!this.getGridCanLoad())
            return false;

        return true;
    }

    /**
     * Hook invoked when the grid markup is ready after data changes.
     */
    protected markupReady(): void {

    }

    /**
     * Creates the container element that hosts the grid.
     * @returns The grid container element.
     */
    protected createSlickContainer(): Fluent {
        return Fluent(<div class="grid-container" />).appendTo(this.domNode);
    }

    /**
     * Creates the remote view used for paging and server communication.
     * @returns The remote view instance.
     */
    protected createView(): IRemoteView<TItem> {
        var opt = this.getViewOptions();
        return new RemoteView<TItem>(opt) as any;
    }

    /**
     * Returns the default sort order for the grid.
     * @returns Array of sort descriptors.
     */
    protected getDefaultSortBy(): any[] {
        return getDefaultSortBy(this._grid);
    }

    /**
     * Whether the grid should render a pager.
     * @returns True when paging is enabled.
     */
    protected usePager(): boolean {
        return false;
    }

    /**
     * Whether advanced filtering is enabled for this grid.
     * @returns True when advanced filtering is enabled.
     */
    protected enableAdvancedFiltering(): boolean {
        return this.getCustomAttribute(AdvancedFilteringAttribute)?.value ??
            (typeof dataGridDefaults.enableAdvancedFiltering === "function" ?
                dataGridDefaults.enableAdvancedFiltering(this)
                : dataGridDefaults.enableAdvancedFiltering) ?? false;
    }

    /**
     * Whether the grid should wait until visible before populating data.
     * @returns True when population waits for visibility.
     */
    protected populateWhenVisible(): boolean {
        return false;
    }

    /**
     * Creates the advanced filter bar and initializes its store.
     */
    protected createFilterBar(): void {
        this.filterBar = new FilterDisplayBar({
            element: el => {
                const after = this.quickFiltersBar?.domNode ?? this.toolbar?.domNode;
                after ? Fluent(el).insertAfter(after) : this.domNode.prepend(el);
            }
        });
        this.initializeFilterBar();
    }

    /**
     * Returns the pager options for this grid.
     * @returns Pager options.
     */
    protected getPagerOptions(): PagerOptions {
        return {
            view: this.view,
            rowsPerPage: 20,
            rowsPerPageOptions: [20, 100, 500, 2500]
        };
    }

    /**
     * Creates the pager widget for this grid.
     */
    protected createPager(): void {
        new SlickPager({ ...this.getPagerOptions(), element: el => this.domNode.append(el) });
    }

    /**
     * Returns the remote view options for this grid.
     * @returns Remote view options.
     */
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

    /**
     * Creates the toolbar with the given buttons.
     * @param buttons - Tool button definitions.
     */
    protected createToolbar(buttons: ToolButton[]): void {
        this.toolbar = new Toolbar({
            buttons: buttons,
            hotkeyContext: this.domNode,
            element: el => this.domNode.appendChild(el).classList.add("grid-toolbar")
        }).init();
    }

    /**
     * Returns the current grid title text.
     * @returns The title text, or null if no title is set.
     */
    getTitle(): string {
        if (!this.titleDiv) {
            return null;
        }

        return this.titleDiv.findFirst('.title-text').text();
    }

    /**
     * Sets the grid title text, creating or removing the title element as needed.
     * @param value - The title text, or null to remove the title.
     */
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

    /**
     * Returns the item type key for this grid.
     * @returns The item type key.
     */
    protected getItemType(): string {
        return 'Item';
    }

    /**
     * Creates a formatter that renders a link to an item.
     * @param itemType - Item type key; defaults to the grid item type.
     * @param idField - Id field name; defaults to the grid id property.
     * @param text - Optional text formatter.
     * @param cssClass - Optional CSS class formatter.
     * @param encode - Whether to HTML-encode the link text.
     * @returns A formatter function.
     */
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
            (props.context?.purpose && skipEditLinkFormatPurposes.has(props.context.purpose))) {
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

    /**
     * Returns the columns key used to load property items.
     * @returns The columns key, or null for none.
     */
    protected getColumnsKey(): string {
        return null;
    }

    /**
     * Returns the property items for this grid.
     * @returns The property items.
     */
    protected getPropertyItems(): PropertyItem[] {
        return this.propertyItemsData?.items || [];
    }

    /**
     * Loads the property items data, either from script data or local items.
     * @returns The property items data.
     */
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

    /**
     * Asynchronously loads the property items data.
     * @returns A promise resolving to the property items data.
     */
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

    /**
     * Wraps a column formatter with an edit link formatter.
     * @param column - Column to wrap.
     * @param item - Property item describing the edit link.
     */
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

    /**
     * Converts property items to grid columns, wrapping edit-link columns.
     * @param propertyItems - Property items to convert.
     * @returns The grid columns.
     */
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

    /**
     * Returns the SleekGrid options for this grid.
     * @returns Grid options.
     */
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

    /**
     * Locks the view against population.
     */
    protected populateLock(): void {
        this.view.populateLock();
    }

    /**
     * Unlocks the view population.
     */
    protected populateUnlock(): void {
        this.view.populateUnlock();
    }

    /**
     * Determines whether the grid can load data, notifying onCanSubmit subscribers.
     * @returns True when the grid can load.
     */
    protected getGridCanLoad(): boolean {
        const e: DataGridSubmitEvent = { dataGrid: this, cancel: false };
        this.onCanSubmit.notify(e);
        if (e.cancel)
            return false;

        return true;
    }

    /**
     * Prepares submit arguments in this.view.params by calling this.view.onSubmit if available, or this.handleViewSubmit if not.
     * Note that if getGridCanLoad returns false, the prepared arguments might be in an incomplete state.
     * @returns True when the submit should proceed.
     */
    public prepareSubmit(): boolean {
        if (this.view?.onSubmit) {
            const result = this.view.onSubmit(this.view);
            return result == null || !!result;
        }

        return this.handleViewSubmit();
    }

    /**
     * Refreshes the grid data, waiting for visibility if configured to do so.
     */
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
        this.slickContainer.data("needsRefresh", "true");
    }

    /**
     * Refreshes the grid if a refresh was requested while hidden.
     */
    protected refreshIfNeeded(): void {
        if (!!this.slickContainer.data("needsRefresh")) {
            this.slickContainer.data('needsRefresh', null);
            this.internalRefresh();
        }
    }

    /**
     * Performs the actual data refresh by populating the view.
     */
    protected internalRefresh(): void {
        this.view.populate();
    }

    declare private _readonly: boolean;

    /** Whether the grid is in read-only mode. */
    public get readOnly(): boolean {
        return this.get_readOnly();
    }

    /** Sets whether the grid is in read-only mode. */
    public set readOnly(value: boolean) {
        this.set_readOnly(value);
    }

    /**
     * Returns whether the grid is in read-only mode.
     * @returns True when read-only.
     */
    public get_readOnly() {
        return !!this._readonly;
    }

    /**
     * Sets whether the grid is in read-only mode and updates the interface.
     * @param value - True to enable read-only mode.
     */
    public set_readOnly(value: boolean) {
        if (!!this._readonly != !!value) {
            this._readonly = !!value;
            this.updateInterface();
        }
    }

    /**
     * Updates the toolbar interface to reflect the current grid state.
     */
    public updateInterface() {
        this.toolbar && this.toolbar.updateInterface();
    }

    /**
     * Returns the row definition for this grid.
     * @returns The row definition, or null for none.
     */
    protected getRowDefinition(): IRowDefinition {
        return null;
    }

    declare private _localTextDbPrefix: string;

    /**
     * Returns the local text database prefix for this grid.
     * @returns The local text db prefix.
     */
    protected getLocalTextDbPrefix(): string {

        if (this._localTextDbPrefix != null)
            return this._localTextDbPrefix;

        this._localTextDbPrefix = this.getLocalTextPrefix() ?? '';
        if (this._localTextDbPrefix.length > 0 && !this._localTextDbPrefix.endsWith('.'))
            this._localTextDbPrefix = 'Db.' + this._localTextDbPrefix + '.';

        return this._localTextDbPrefix;
    }

    /**
     * Returns the local text prefix for this grid.
     * @returns The local text prefix, or undefined.
     */
    protected getLocalTextPrefix(): string {
        var rowDefinition = this.getRowDefinition();
        if (rowDefinition)
            return rowDefinition.localTextPrefix;

        return void 0;
    }

    declare private _idProperty: string;

    /**
     * Returns the id property name for this grid.
     * @returns The id property name.
     */
    protected getIdProperty(): string {
        if (this._idProperty != null)
            return this._idProperty;

        var rowDefinition = this.getRowDefinition();
        if (rowDefinition)
            return this._idProperty = rowDefinition.idProperty ?? '';

        return this._idProperty = 'ID';
    }

    /**
     * Returns the is-deleted property name for this grid.
     * @returns The is-deleted property name, or undefined.
     */
    protected getIsDeletedProperty(): string {
        return this.getRowDefinition()?.isDeletedProperty;
    }

    declare private _isActiveProperty: string;

    /**
     * Returns the is-active property name for this grid.
     * @returns The is-active property name.
     */
    protected getIsActiveProperty(): string {
        if (this._isActiveProperty != null)
            return this._isActiveProperty;

        var rowDefinition = this.getRowDefinition();
        if (rowDefinition)
            return this._isActiveProperty = rowDefinition.isActiveProperty ?? '';

        return this._isActiveProperty = '';
    }

    /**
     * Resizes the underlying grid canvas.
     */
    protected resizeCanvas(): void {
        this._grid?.resizeCanvas();
    }

    /**
     * Refreshes the grid when a sub-dialog reports a data change.
     */
    protected subDialogDataChange(): void {
        this.refresh();
    }

    /**
     * Adds a separator to the quick filter bar.
     */
    protected addFilterSeparator(): void {
        this.ensureQuickFilterBar().addSeparator();
    }

    /**
     * Resolves a localized text using the grid's local text db prefix.
     * @param getKey - Callback that builds the text key from the prefix.
     * @returns The localized text, or null if not found.
     */
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

    /**
     * Adds a quick filter to the quick filter bar.
     * @param opt - Quick filter definition.
     * @returns The created widget instance.
     */
    protected addQuickFilter<TWidget extends Widget<any>, P>(opt: QuickFilter<TWidget, P>): TWidget {
        return this.ensureQuickFilterBar().add(opt);
    }

    /**
     * Adds a date range quick filter for the specified field.
     * @param field - Field name.
     * @param title - Optional display title.
     * @returns The created date editor.
     */
    protected addDateRangeFilter(field: string, title?: string): DateEditor {
        return this.ensureQuickFilterBar().addDateRange(field, title);
    }

    /**
     * Creates a date range quick filter definition.
     * @param field - Field name.
     * @param title - Optional display title.
     * @returns A quick filter definition.
     */
    protected dateRangeQuickFilter(field: string, title?: string) {
        return QuickFilterBar.dateRange(field, title);
    }

    /**
     * Adds a date-time range quick filter for the specified field.
     * @param field - Field name.
     * @param title - Optional display title.
     * @returns The created date-time editor.
     */
    protected addDateTimeRangeFilter(field: string, title?: string) {
        return this.ensureQuickFilterBar().addDateTimeRange(field, title);
    }

    /**
     * Creates a date-time range quick filter definition.
     * @param field - Field name.
     * @param title - Optional display title.
     * @returns A quick filter definition.
     */
    protected dateTimeRangeQuickFilter(field: string, title?: string) {
        return QuickFilterBar.dateTimeRange(field, title);
    }

    /**
     * Adds a boolean quick filter for the specified field.
     * @param field - Field name.
     * @param title - Optional display title.
     * @param yes - Optional text for the true option.
     * @param no - Optional text for the false option.
     * @returns The created select editor.
     */
    protected addBooleanFilter(field: string, title?: string, yes?: string, no?: string): SelectEditor {
        return this.ensureQuickFilterBar().addBoolean(field, title, yes, no);
    }

    /**
     * Creates a boolean quick filter definition.
     * @param field - Field name.
     * @param title - Optional display title.
     * @param yes - Optional text for the true option.
     * @param no - Optional text for the false option.
     * @returns A quick filter definition.
     */
    protected booleanQuickFilter(field: string, title?: string, yes?: string, no?: string) {
        return QuickFilterBar.boolean(field, title, yes, no);
    }

    /**
     * Invokes the quick filter submit handlers with the current view params.
     */
    protected invokeSubmitHandlers() {
        if (this.quickFiltersBar != null) {
            this.quickFiltersBar.onSubmit(this.view.params);
        }
    }

    /**
     * Handles quick filter changes by persisting settings and refreshing.
     * @param e - Change event.
     */
    protected quickFilterChange(e: Event) {
        this.persistSettings();
        this.view && (this.view.seekToPage = 1);
        this.refresh();
    }

    /**
     * Returns the storage used for grid persistence.
     * @returns The persistence storage.
     */
    protected getPersistenceStorage(): SettingStorage {
        if ((this as any).getPersistanceStorage) return (this as any).getPersistanceStorage(); // compat

        return DataGrid.defaultPersistenceStorage;
    }

    /**
     * Returns the key used to store grid settings.
     * @returns The persistence key.
     */
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

    /**
     * Returns the default persistence flags for this grid.
     * @returns Grid persistence flags.
     */
    protected gridPersistenceFlags(): GridPersistenceFlags {
        if ((this as any).gridPersistanceFlags) return (this as any).gridPersistanceFlags; // compat

        return {};
    }

    /**
     * Retrieves the persisted grid settings from storage.
     * @returns The persisted settings, or a promise resolving to them.
     */
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

    /**
     * Restores grid settings from the given settings or from storage.
     * @param settings - Optional settings to restore; defaults to persisted settings.
     * @param flags - Optional persistence flags.
     * @returns Void or a promise that resolves when restoration completes.
     */
    protected restoreSettings(settings?: PersistedGridSettings, flags?: GridPersistenceFlags): void | Promise<void> {
        if (settings != null)
            return this.restoreSettingsFrom(settings, flags);

        var settingsOrPromise = this.getPersistedSettings();
        if ((settingsOrPromise as any)?.then)
            return (settingsOrPromise as Promise<PersistedGridSettings>).then((s) => this.restoreSettingsFrom(s));

        this.restoreSettingsFrom(settingsOrPromise as PersistedGridSettings);
    }

    /**
     * Restores grid state from a persisted settings snapshot.
     * @param settings - The settings to restore.
     * @param flags - Optional persistence flags.
     */
    protected restoreSettingsFrom(settings: PersistedGridSettings, flags?: GridPersistenceFlags): void {
        if (!this._grid || !settings)
            return;

        this.view.beginUpdate();
        this.restoringSettings++;
        try {

            const flagsDefault = this.gridPersistenceFlags();
            const event: DataGridPersistenceEvent = {
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
                filterStore: this.filterBar?.get_store(),
                flags: event.flagsToUse,
                includeDeletedToggle: this.domNode.querySelector('.s-IncludeDeletedToggle'),
                quickFiltersDiv: this.quickFiltersDiv?.getNode(),
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

    /**
     * Increments the persistence lock, preventing settings from being persisted.
     */
    public persistenceLock() {
        this._persistenceLock++;
    }

    /**
     * Decrements the persistence lock.
     */
    public persistenceUnlock() {
        this._persistenceLock--;
    }

    /**
     * Persists the current grid settings to storage.
     * @param flags - Optional persistence flags.
     * @returns Void or a promise that resolves when the write completes.
     */
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

    /**
     * Returns the current grid settings snapshot.
     * @param flags - Optional persistence flags.
     * @returns The current grid settings.
     */
    public getCurrentSettings(flags?: GridPersistenceFlags) {

        const flagsDefault = this.gridPersistenceFlags();
        const event: DataGridPersistenceEvent = {
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
            quickFiltersDiv: this.quickFiltersDiv?.getNode(),
            sleekGrid: this._grid,
            toolbarNode: this.toolbar?.domNode,
            uniqueName: this.uniqueName
        });
        event.after = true;
        this.onPersistence.notify(event);
        return event.settings;
    }

    /**
     * Returns the root DOM element of the grid widget.
     * @returns The grid container element.
     */
    getElement(): HTMLElement {
        return this.domNode;
    }

    /**
     * Returns the underlying SleekGrid instance.
     * @returns The grid instance.
     */
    getGrid(): ISleekGrid<TItem> {
        return this._grid;
    }

    /** The underlying SleekGrid instance. */
    public get sleekGrid() { return this._grid; }
    /** Sets the underlying SleekGrid instance. */
    protected set sleekGrid(value: ISleekGrid<TItem>) { this._grid = value; }

    /** @deprecated Use sleekGrid or getGrid() */
    public get slickGrid() { return this._grid; }

    /**
     * Returns the remote view used for paging and server communication.
     * @returns The remote view instance.
     */
    getView(): IRemoteView<TItem> {
        return this.view;
    }

    /**
     * Returns the filter store owned by the grid.
     * @returns The filter store, or null if no filter bar exists.
     */
    getFilterStore(): FilterStore {
        return (this.filterBar == null) ? null : this.filterBar.get_store();
    }

    /** All columns including hidden ones. */
    public get allColumns(): Column[] { return this._grid?.getAllColumns() }
    /** The currently visible columns. */
    public get columns() { return this._grid?.getColumns(); }
    /** The initial persisted settings captured at startup. */
    public get initialSettings() { return this._initialSettings; }
    /** Sets the initial persisted settings. */
    protected set initialSettings(value: PersistedGridSettings) { this._initialSettings = value; }

    /** @deprecated use defaultPersistenceStorage, this one has a typo */
    public static get defaultPersistanceStorage(): SettingStorage { return DataGrid.defaultOptions.persistenceStorage; }
    /** @deprecated use defaultPersistenceStorage, this one has a typo */
    public static set defaultPersistanceStorage(value: SettingStorage) { DataGrid.defaultOptions.persistenceStorage = value; }
}

/**
 * Base event arguments for data grid events.
 */
export interface DataGridEvent {
    /** The data grid that raised the event. */
    dataGrid: DataGrid<any>;
}

/** Event raised when the grid data changes. */
export type DataGridChangeEvent = DataGridEvent;
/** Event raised after the grid is initialized. */
export type DataGridInitEvent = DataGridEvent;


/**
 * Event raised to determine whether the grid view submit should proceed.
 */
export interface DataGridSubmitEvent extends DataGridEvent {
    /** When true, the submit is cancelled. */
    cancel?: boolean;
}

/**
 * Event raised while filtering items in the view.
 * @typeParam TItem - Row type displayed in the grid.
 */
export interface DataGridFilteringEvent<TItem = any> extends DataGridEvent {
    /** The item being filtered. */
    item: TItem;
    /** Whether the item matches the filter; subscribers may change this. */
    isMatch: boolean;
}

/**
 * Event raised when the view processes a list response.
 * @typeParam TItem - Row type displayed in the grid.
 */
export interface DataGridProcessEvent<TItem> extends DataGridEvent {
    /** The list response being processed. */
    response: ListResponse<TItem>;
}

