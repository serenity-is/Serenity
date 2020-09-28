/// <reference types="jquery" />
/// <reference types="jqueryui" />
declare namespace Serenity {
    class SlickPager extends Widget<Slick.PagerOptions> {
        constructor(div: JQuery, o: Slick.PagerOptions);
        _changePage(ctype: string): boolean;
        _updatePager(): void;
    }
}
declare namespace Slick {
    interface PagerOptions {
        view?: Slick.RemoteView<any>;
        showRowsPerPage?: boolean;
        rowsPerPage?: number;
        rowsPerPageOptions?: number[];
        onChangePage?: (newPage: number) => void;
        onRowsPerPageChange?: (n: number) => void;
    }
}
declare namespace Serenity {
    interface GridRowSelectionMixinOptions {
        selectable?: (item: any) => boolean;
    }
    class GridRowSelectionMixin {
        private idField;
        private include;
        private grid;
        private options;
        constructor(grid: IDataGrid, options?: GridRowSelectionMixinOptions);
        updateSelectAll(): void;
        clear(): void;
        resetCheckedAndRefresh(): void;
        selectKeys(keys: string[]): void;
        getSelectedKeys(): string[];
        getSelectedAsInt32(): number[];
        getSelectedAsInt64(): number[];
        setSelectedKeys(keys: string[]): void;
        private isSelectable;
        static createSelectColumn(getMixin: () => GridRowSelectionMixin): Slick.Column;
    }
    interface GridRadioSelectionMixinOptions {
        selectable?: (item: any) => boolean;
    }
    class GridRadioSelectionMixin {
        private idField;
        private include;
        private grid;
        private options;
        constructor(grid: Serenity.IDataGrid, options?: GridRadioSelectionMixinOptions);
        private isSelectable;
        clear(): void;
        resetCheckedAndRefresh(): void;
        getSelectedKey(): string;
        getSelectedAsInt32(): number;
        getSelectedAsInt64(): number;
        setSelectedKey(key: string): void;
        static createSelectColumn(getMixin: () => Serenity.GridRadioSelectionMixin): Slick.Column;
    }
    namespace GridSelectAllButtonHelper {
        function update(grid: IDataGrid, getSelected: (p1: any) => boolean): void;
        function define(getGrid: () => IDataGrid, getId: (p1: any) => any, getSelected: (p1: any) => boolean, setSelected: (p1: any, p2: boolean) => void, text?: string, onClick?: () => void): ToolButton;
    }
    namespace GridUtils {
        function addToggleButton(toolDiv: JQuery, cssClass: string, callback: (p1: boolean) => void, hint: string, initial?: boolean): void;
        function addIncludeDeletedToggle(toolDiv: JQuery, view: Slick.RemoteView<any>, hint?: string, initial?: boolean): void;
        function addQuickSearchInput(toolDiv: JQuery, view: Slick.RemoteView<any>, fields?: QuickSearchField[], onChange?: () => void): void;
        function addQuickSearchInputCustom(container: JQuery, onSearch: (p1: string, p2: string, done: (p3: boolean) => void) => void, fields?: QuickSearchField[]): QuickSearchInput;
        function makeOrderable(grid: Slick.Grid, handleMove: (p1: any, p2: number) => void): void;
        function makeOrderableWithUpdateRequest(grid: DataGrid<any, any>, getId: (p1: any) => number, getDisplayOrder: (p1: any) => any, service: string, getUpdateRequest: (p1: number, p2: number) => SaveRequest<any>): void;
    }
    namespace PropertyItemSlickConverter {
        function toSlickColumns(items: PropertyItem[]): Slick.Column[];
        function toSlickColumn(item: PropertyItem): Slick.Column;
    }
    namespace SlickFormatting {
        function getEnumText(enumKey: string, name: string): string;
        function treeToggle<TItem>(getView: () => Slick.RemoteView<TItem>, getId: (x: TItem) => any, formatter: Slick.Format): Slick.Format;
        function date(format?: string): Slick.Format;
        function dateTime(format?: string): Slick.Format;
        function checkBox(): Slick.Format;
        function number(format: string): Slick.Format;
        function getItemType(link: JQuery): string;
        function getItemId(link: JQuery): string;
        function itemLinkText(itemType: string, id: any, text: any, extraClass: string, encode: boolean): string;
        function itemLink(itemType: string, idField: string, getText: Slick.Format, cssClass?: Slick.Format, encode?: boolean): Slick.Format;
    }
    namespace SlickHelper {
        function setDefaults(columns: Slick.Column[], localTextPrefix?: string): any;
        function convertToFormatter(format: Slick.Format): Slick.ColumnFormatter;
    }
    namespace SlickTreeHelper {
        function filterCustom<TItem>(item: TItem, getParent: (x: TItem) => any): boolean;
        function filterById<TItem>(item: TItem, view: Slick.RemoteView<TItem>, getParentId: (x: TItem) => any): boolean;
        function setCollapsed<TItem>(items: TItem[], collapsed: boolean): void;
        function setCollapsedFlag<TItem>(item: TItem, collapsed: boolean): void;
        function setIndents<TItem>(items: TItem[], getId: (x: TItem) => any, getParentId: (x: TItem) => any, setCollapsed?: boolean): void;
        function toggleClick<TItem>(e: JQueryEventObject, row: number, cell: number, view: Slick.RemoteView<TItem>, getId: (x: TItem) => any): void;
    }
}
declare namespace Serenity {
    interface IInitializeColumn {
        initializeColumn(column: Slick.Column): void;
    }
    class IInitializeColumn {
    }
    class BooleanFormatter implements Slick.Formatter {
        format(ctx: Slick.FormatterContext): string;
        falseText: string;
        trueText: string;
    }
    class CheckboxFormatter implements Slick.Formatter {
        format(ctx: Slick.FormatterContext): string;
    }
    class DateFormatter implements Slick.Formatter {
        constructor();
        static format(value: any, format?: string): any;
        displayFormat: string;
        format(ctx: Slick.FormatterContext): string;
    }
    class DateTimeFormatter extends DateFormatter {
        constructor();
    }
    class EnumFormatter implements Slick.Formatter {
        format(ctx: Slick.FormatterContext): string;
        enumKey: string;
        static format(enumType: any, value: any): string;
        static getText(enumKey: string, name: string): string;
        static getName(enumType: any, value: any): string;
    }
    class FileDownloadFormatter implements Slick.Formatter, IInitializeColumn {
        format(ctx: Slick.FormatterContext): string;
        static dbFileUrl(filename: string): string;
        initializeColumn(column: Slick.Column): void;
        displayFormat: string;
        originalNameProperty: string;
    }
    class MinuteFormatter implements Slick.Formatter {
        format(ctx: Slick.FormatterContext): string;
        static format(value: number): string;
    }
    class NumberFormatter {
        format(ctx: Slick.FormatterContext): string;
        static format(value: any, format?: string): string;
        displayFormat: string;
    }
    class UrlFormatter implements Slick.Formatter, IInitializeColumn {
        format(ctx: Slick.FormatterContext): string;
        initializeColumn(column: Slick.Column): void;
        displayProperty: string;
        displayFormat: string;
        urlProperty: string;
        urlFormat: string;
        target: string;
    }
    namespace FormatterTypeRegistry {
        function get(key: string): Function;
        function reset(): void;
    }
}
declare namespace Serenity {
    interface SettingStorage {
        getItem(key: string): string;
        setItem(key: string, value: string): void;
    }
    interface PersistedGridColumn {
        id: string;
        width?: number;
        sort?: number;
        visible?: boolean;
    }
    interface PersistedGridSettings {
        columns?: PersistedGridColumn[];
        filterItems?: FilterLine[];
        quickFilters?: Q.Dictionary<any>;
        quickFilterText?: string;
        quickSearchField?: QuickSearchField;
        quickSearchText?: string;
        includeDeleted?: boolean;
    }
    interface GridPersistanceFlags {
        columnWidths?: boolean;
        columnVisibility?: boolean;
        sortColumns?: boolean;
        filterItems?: boolean;
        quickFilters?: boolean;
        quickFilterText?: boolean;
        quickSearch?: boolean;
        includeDeleted?: boolean;
    }
    interface IDataGrid {
        getElement(): JQuery;
        getGrid(): Slick.Grid;
        getView(): Slick.RemoteView<any>;
        getFilterStore(): Serenity.FilterStore;
    }
    class IDataGrid {
    }
    class DataGrid<TItem, TOptions> extends Widget<TOptions> implements IDataGrid, IReadOnly {
        protected titleDiv: JQuery;
        protected toolbar: Toolbar;
        protected filterBar: FilterDisplayBar;
        protected quickFiltersDiv: JQuery;
        protected quickFiltersBar: QuickFilterBar;
        protected slickContainer: JQuery;
        protected allColumns: Slick.Column[];
        protected initialSettings: PersistedGridSettings;
        protected restoringSettings: number;
        private idProperty;
        private isActiveProperty;
        private localTextDbPrefix;
        private isDisabled;
        private rows;
        private slickGridOnSort;
        private slickGridOnClick;
        view: Slick.RemoteView<TItem>;
        slickGrid: Slick.Grid;
        openDialogsAsPanel: boolean;
        static defaultRowHeight: number;
        static defaultHeaderHeight: number;
        static defaultPersistanceStorage: SettingStorage;
        private layoutTimer;
        constructor(container: JQuery, options?: TOptions);
        protected useLayoutTimer(): boolean;
        protected attrs<TAttr>(attrType: {
            new (...args: any[]): TAttr;
        }): TAttr[];
        protected layout(): void;
        protected getInitialTitle(): string;
        protected createToolbarExtensions(): void;
        protected ensureQuickFilterBar(): QuickFilterBar;
        protected createQuickFilters(filters?: QuickFilter<Widget<any>, any>[]): void;
        protected getQuickFilters(): QuickFilter<Widget<any>, any>[];
        static propertyItemToQuickFilter(item: PropertyItem): any;
        protected findQuickFilter<TWidget>(type: {
            new (...args: any[]): TWidget;
        }, field: string): TWidget;
        protected tryFindQuickFilter<TWidget>(type: {
            new (...args: any[]): TWidget;
        }, field: string): TWidget;
        protected createIncludeDeletedButton(): void;
        protected getQuickSearchFields(): QuickSearchField[];
        protected createQuickSearchInput(): void;
        destroy(): void;
        protected getItemCssClass(item: TItem, index: number): string;
        protected getItemMetadata(item: TItem, index: number): any;
        protected postProcessColumns(columns: Slick.Column[]): Slick.Column[];
        protected initialPopulate(): void;
        protected canFilterColumn(column: Slick.Column): boolean;
        protected initializeFilterBar(): void;
        protected createSlickGrid(): Slick.Grid;
        protected setInitialSortOrder(): void;
        itemAt(row: number): TItem;
        rowCount(): number;
        getItems(): TItem[];
        setItems(value: TItem[]): void;
        protected bindToSlickEvents(): void;
        protected getAddButtonCaption(): string;
        protected getButtons(): ToolButton[];
        protected editItem(entityOrId: any): void;
        protected editItemOfType(itemType: string, entityOrId: any): void;
        protected onClick(e: JQueryEventObject, row: number, cell: number): void;
        protected viewDataChanged(e: any, rows: TItem[]): void;
        protected bindToViewEvents(): void;
        protected onViewProcessData(response: ListResponse<TItem>): ListResponse<TItem>;
        protected onViewFilter(item: TItem): boolean;
        protected getIncludeColumns(include: {
            [key: string]: boolean;
        }): void;
        protected setCriteriaParameter(): void;
        protected setEquality(field: string, value: any): void;
        protected setIncludeColumnsParameter(): void;
        protected onViewSubmit(): boolean;
        protected markupReady(): void;
        protected createSlickContainer(): JQuery;
        protected createView(): Slick.RemoteView<TItem>;
        protected getDefaultSortBy(): any[];
        protected usePager(): boolean;
        protected enableFiltering(): boolean;
        protected populateWhenVisible(): boolean;
        protected createFilterBar(): void;
        protected getPagerOptions(): Slick.PagerOptions;
        protected createPager(): void;
        protected getViewOptions(): Slick.RemoteViewOptions;
        protected createToolbar(buttons: ToolButton[]): void;
        getTitle(): string;
        setTitle(value: string): void;
        protected getItemType(): string;
        protected itemLink(itemType?: string, idField?: string, text?: (ctx: Slick.FormatterContext) => string, cssClass?: (ctx: Slick.FormatterContext) => string, encode?: boolean): Slick.Format;
        protected getColumnsKey(): string;
        protected getPropertyItems(): PropertyItem[];
        protected getColumns(): Slick.Column[];
        protected propertyItemsToSlickColumns(propertyItems: PropertyItem[]): Slick.Column[];
        protected getSlickOptions(): Slick.GridOptions;
        protected populateLock(): void;
        protected populateUnlock(): void;
        protected getGridCanLoad(): boolean;
        refresh(): void;
        protected refreshIfNeeded(): void;
        protected internalRefresh(): void;
        setIsDisabled(value: boolean): void;
        private _readonly;
        get readOnly(): boolean;
        set readOnly(value: boolean);
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
        protected updateInterface(): void;
        protected getLocalTextDbPrefix(): string;
        protected getLocalTextPrefix(): string;
        protected getIdProperty(): string;
        protected getIsDeletedProperty(): string;
        protected getIsActiveProperty(): string;
        protected updateDisabledState(): void;
        protected resizeCanvas(): void;
        protected subDialogDataChange(): void;
        protected addFilterSeparator(): void;
        protected determineText(getKey: (prefix: string) => string): string;
        protected addQuickFilter<TWidget extends Widget<any>, TOptions>(opt: QuickFilter<TWidget, TOptions>): TWidget;
        protected addDateRangeFilter(field: string, title?: string): Serenity.DateEditor;
        protected dateRangeQuickFilter(field: string, title?: string): QuickFilter<DateEditor, DateTimeEditorOptions>;
        protected addDateTimeRangeFilter(field: string, title?: string): DateTimeEditor;
        protected dateTimeRangeQuickFilter(field: string, title?: string): QuickFilter<DateTimeEditor, DateTimeEditorOptions>;
        protected addBooleanFilter(field: string, title?: string, yes?: string, no?: string): SelectEditor;
        protected booleanQuickFilter(field: string, title?: string, yes?: string, no?: string): QuickFilter<SelectEditor, SelectEditorOptions>;
        protected invokeSubmitHandlers(): void;
        protected quickFilterChange(e: JQueryEventObject): void;
        protected getPersistanceStorage(): SettingStorage;
        protected getPersistanceKey(): string;
        protected gridPersistanceFlags(): GridPersistanceFlags;
        protected canShowColumn(column: Slick.Column): boolean;
        protected getPersistedSettings(): PersistedGridSettings;
        protected restoreSettings(settings?: PersistedGridSettings, flags?: GridPersistanceFlags): void;
        protected persistSettings(flags?: GridPersistanceFlags): void;
        protected getCurrentSettings(flags?: GridPersistanceFlags): PersistedGridSettings;
        getElement(): JQuery;
        getGrid(): Slick.Grid;
        getView(): Slick.RemoteView<TItem>;
        getFilterStore(): FilterStore;
    }
}
declare namespace Serenity {
    class ColumnPickerDialog extends Serenity.TemplatedDialog<any> {
        private ulVisible;
        private ulHidden;
        private colById;
        allColumns: Slick.Column[];
        visibleColumns: string[];
        defaultColumns: string[];
        done: () => void;
        constructor();
        static createToolButton(grid: DataGrid<any, any>): ToolButton;
        protected getDialogButtons(): {
            text: string;
            click: () => void;
        }[];
        protected getDialogOptions(): JQueryUI.DialogOptions;
        private getTitle;
        private allowHide;
        private createLI;
        private updateListStates;
        protected setupColumns(): void;
        protected onDialogOpen(): void;
        protected getTemplate(): string;
    }
}
declare namespace Serenity {
    class EntityGrid<TItem, TOptions> extends DataGrid<TItem, TOptions> {
        constructor(container: JQuery, options?: TOptions);
        protected usePager(): boolean;
        protected createToolbarExtensions(): void;
        protected getInitialTitle(): string;
        protected getLocalTextPrefix(): string;
        private entityType;
        protected getEntityType(): string;
        private displayName;
        protected getDisplayName(): string;
        private itemName;
        protected getItemName(): string;
        protected getAddButtonCaption(): string;
        protected getButtons(): ToolButton[];
        protected newRefreshButton(noText?: boolean): ToolButton;
        protected addButtonClick(): void;
        protected editItem(entityOrId: any): void;
        protected editItemOfType(itemType: string, entityOrId: any): void;
        private service;
        protected getService(): string;
        protected getViewOptions(): Slick.RemoteViewOptions;
        protected getItemType(): string;
        protected routeDialog(itemType: string, dialog: Widget<any>): void;
        protected getInsertPermission(): string;
        protected hasInsertPermission(): boolean;
        protected transferDialogReadOnly(dialog: Widget<any>): void;
        protected initDialog(dialog: Widget<any>): void;
        protected initEntityDialog(itemType: string, dialog: Widget<any>): void;
        protected createEntityDialog(itemType: string, callback?: (dlg: Widget<any>) => void): Widget<any>;
        protected getDialogOptions(): JQueryUI.DialogOptions;
        protected getDialogOptionsFor(itemType: string): JQueryUI.DialogOptions;
        protected getDialogTypeFor(itemType: string): {
            new (...args: any[]): Widget<any>;
        };
        private dialogType;
        protected getDialogType(): {
            new (...args: any[]): Widget<any>;
        };
    }
}
declare namespace Serenity {
    /**
     * A mixin that can be applied to a DataGrid for tree functionality
     */
    class TreeGridMixin<TItem> {
        private options;
        private dataGrid;
        private getId;
        constructor(options: TreeGridMixinOptions<TItem>);
        /**
         * Expands / collapses all rows in a grid automatically
         */
        toggleAll(): void;
        collapseAll(): void;
        expandAll(): void;
        /**
         * Reorders a set of items so that parents comes before their children.
         * This method is required for proper tree ordering, as it is not so easy to perform with SQL.
         * @param items list of items to be ordered
         * @param getId a delegate to get ID of a record (must return same ID with grid identity field)
         * @param getParentId a delegate to get parent ID of a record
         */
        static applyTreeOrdering<TItem>(items: TItem[], getId: (item: TItem) => any, getParentId: (item: TItem) => any): TItem[];
    }
    interface TreeGridMixinOptions<TItem> {
        grid: Serenity.DataGrid<TItem, any>;
        getParentId: (item: TItem) => any;
        toggleField: string;
        initialCollapse?: () => boolean;
    }
}
declare namespace Serenity {
    interface CheckTreeItem<TSource> {
        isSelected?: boolean;
        hideCheckBox?: boolean;
        isAllDescendantsSelected?: boolean;
        id?: string;
        text?: string;
        parentId?: string;
        children?: CheckTreeItem<TSource>[];
        source?: TSource;
    }
    class CheckTreeEditor<TItem extends CheckTreeItem<any>, TOptions> extends DataGrid<TItem, TOptions> implements IGetEditValue, ISetEditValue, IReadOnly {
        private byId;
        constructor(div: JQuery, opt?: TOptions);
        protected getIdProperty(): string;
        protected getTreeItems(): TItem[];
        protected updateItems(): void;
        getEditValue(property: PropertyItem, target: any): void;
        setEditValue(source: any, property: PropertyItem): void;
        protected getButtons(): ToolButton[];
        protected itemSelectedChanged(item: TItem): void;
        protected getSelectAllText(): string;
        protected isThreeStateHierarchy(): boolean;
        protected createSlickGrid(): Slick.Grid;
        protected onViewFilter(item: TItem): boolean;
        protected getInitialCollapse(): boolean;
        protected onViewProcessData(response: ListResponse<TItem>): ListResponse<TItem>;
        protected onClick(e: JQueryEventObject, row: number, cell: number): void;
        protected updateSelectAll(): void;
        protected updateFlags(): void;
        protected getDescendantsSelected(item: TItem): boolean;
        protected setAllSubTreeSelected(item: TItem, selected: boolean): boolean;
        protected allItemsSelected(): boolean;
        protected allDescendantsSelected(item: TItem): boolean;
        protected getDelimited(): boolean;
        protected anyDescendantsSelected(item: TItem): boolean;
        protected getColumns(): Slick.Column[];
        protected getItemText(ctx: Slick.FormatterContext): string;
        protected getSlickOptions(): Slick.GridOptions;
        protected sortItems(): void;
        protected moveSelectedUp(): boolean;
        private _readOnly;
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
        private get_value;
        get value(): string[];
        private set_value;
        set value(v: string[]);
    }
    interface CheckLookupEditorOptions {
        lookupKey?: string;
        checkedOnTop?: boolean;
        showSelectAll?: boolean;
        hideSearch?: boolean;
        delimited?: boolean;
        cascadeFrom?: string;
        cascadeField?: string;
        cascadeValue?: any;
        filterField?: string;
        filterValue?: any;
    }
    class CheckLookupEditor<TItem = any> extends CheckTreeEditor<Serenity.CheckTreeItem<TItem>, CheckLookupEditorOptions> {
        private searchText;
        private enableUpdateItems;
        constructor(div: JQuery, options: CheckLookupEditorOptions);
        protected updateItems(): void;
        protected getLookupKey(): string;
        protected getButtons(): Serenity.ToolButton[];
        protected createToolbarExtensions(): void;
        protected getSelectAllText(): string;
        protected cascadeItems(items: TItem[]): TItem[];
        protected filterItems(items: TItem[]): TItem[];
        protected getLookupItems(lookup: Q.Lookup<TItem>): TItem[];
        protected getTreeItems(): CheckTreeItem<TItem>[];
        protected onViewFilter(item: CheckTreeItem<TItem>): boolean;
        protected moveSelectedUp(): boolean;
        protected get_cascadeFrom(): string;
        get cascadeFrom(): string;
        protected getCascadeFromValue(parent: Serenity.Widget<any>): any;
        protected cascadeLink: Serenity.CascadedWidgetLink<Widget<any>>;
        protected setCascadeFrom(value: string): void;
        protected set_cascadeFrom(value: string): void;
        set cascadeFrom(value: string);
        protected get_cascadeField(): any;
        get cascadeField(): string;
        protected set_cascadeField(value: string): void;
        set cascadeField(value: string);
        protected get_cascadeValue(): any;
        get cascadeValue(): any;
        protected set_cascadeValue(value: any): void;
        set cascadeValue(value: any);
        protected get_filterField(): string;
        get filterField(): string;
        protected set_filterField(value: string): void;
        set filterField(value: string);
        protected get_filterValue(): any;
        get filterValue(): any;
        protected set_filterValue(value: any): void;
        set filterValue(value: any);
    }
}
