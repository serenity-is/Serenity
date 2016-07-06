declare namespace Serenity {
    namespace EnumTypeRegistry {
        function get(key: string): Function;
    }

    class Flexify extends Widget<FlexifyOptions> {
        constructor(container: JQuery, options: FlexifyOptions);
    }

    interface FlexifyOptions {
        getXFactor?: (p1: JQuery) => any;
        getYFactor?: (p1: JQuery) => any;
        designWidth?: any;
        designHeight?: any;
    }

    namespace FLX {
        function flexHeightOnly(element: JQuery, flexY?: number): JQuery;
        function flexWidthOnly(element: JQuery, flexX?: number): JQuery;
        function flexWidthHeight(element: JQuery, flexX?: number, flexY?: number): JQuery;
        function flexXFactor(element: JQuery, flexX: number): JQuery;
        function flexYFactor(element: JQuery, flexY: number): JQuery;
    }

    class IAsyncInit {
    }

    class IBooleanValue {
    }

    interface IBooleanValue {
        get_value(): boolean;
        set_value(value: boolean): void;
    }

    class IDialog {
    }

    interface IDialog {
        dialogOpen(): void;
    }

    class IDoubleValue {
    }

    interface IDoubleValue {
        get_value(): any;
        set_value(value: any): void;
    }

    interface IEditDialog {
        load(entityOrId: any, done: () => void, fail: (p1: any) => void): void;
    }

    class IGetEditValue {
    }

    interface IGetEditValue {
        getEditValue(property: PropertyItem, target: any): void;
    }

    class IInitializeColumn {
    }

    interface IInitializeColumn {
        initializeColumn(column: Slick.Column): void;
    }

    namespace CustomValidation {
        function registerValidationMethods(): void;
    }

    type CustomValidationRule = (element: JQuery) => string;

    interface IValidateRequired {
        get_required(): boolean;
        set_required(value: boolean): void;
    }

    interface DataChangeInfo {
        type: string;
        entityId: any;
        entity: any;
    }

    namespace WX {
        function getWidget<TWidget>(element: JQuery, type: Function): any;
        function tryGetWidget(element: JQuery): any;
        function getWidgetName(type: Function): string;
        function hasOriginalEvent(e: any): boolean;
        function change(widget: any, handler: any): void;
        function changeSelect2(widget: any, handler: any): void;
        function getGridField(widget: Serenity.Widget<any>): JQuery;
        function create(initElement: (p1: JQuery) => void, options?: any): any;
    }

    namespace ReflectionOptionsSetter {
        function set(target: any, options: any): void;
    }

    namespace ReflectionUtils {
        function getPropertyValue(o: any, property: string): any;
        function setPropertyValue(o: any, property: string, value: any): void;
        function makeCamelCase(s: string): string;
    }

    const enum RetrieveColumnSelection {
        details = 0,
        keyOnly = 1,
        list = 2
    }

    interface PropertyItem {
        name?: string;
        title?: string;
        hint?: string;
        placeholder?: string;
        editorType?: string;
        editorParams?: any;
        category?: string;
        cssClass?: string;
        maxLength?: number;
        required?: boolean;
        insertable?: boolean;
        hideOnInsert?: boolean;
        updatable?: boolean;
        hideOnUpdate?: boolean;
        readOnly?: boolean;
        oneWay?: boolean;
        defaultValue?: any;
        localizable?: boolean;
        visible?: boolean;
        formatterType?: string;
        formatterParams?: any;
        displayFormat?: string;
        alignment?: string;
        width?: number;
        minWidth?: number;
        maxWidth?: number;
        resizable?: boolean;
        sortable?: boolean;
        sortOrder?: number;
        editLink?: boolean;
        editLinkItemType?: string;
        editLinkIdField?: string;
        editLinkCssClass?: string;
        filteringType?: string;
        filteringParams?: any;
        filteringIdField?: string;
        notFilterable?: boolean;
        filterOnly?: boolean;
        quickFilter?: boolean;
        quickFilterParams?: any;
    }

    class ISlickFormatter {
    }

    class ScriptContext {
    }

    class PrefixedContext extends ScriptContext {
        constructor(prefix: string);
        w(id: string, type: Function): any;
    }

    class IStringValue {
    }

    interface IStringValue {
        get_value(): string;
        set_value(value: string): void;
    }

    class StringEditor extends Widget<any> {
        value: string;
    }

    namespace Select2Extensions {
        function select2(element: JQuery): JQuery;
        function select2(element: JQuery, options: Select2Options): JQuery;
        function select2(element: JQuery, action: string): JQuery;
        function select2(element: JQuery, option: string, value: any): JQuery;
        function select2(element: JQuery, option: string): any;
    }

    class Select2Editor<TOptions, TItem> extends Widget<TOptions> {
        items: Select2Item[];
        itemById: any;
        pageSize: number;
        lastCreateTerm: string;
        constructor(hidden: JQuery, opt: any);
        emptyItemText(): string;
        getSelect2Options(): Select2Options;
        clearItems(): void;
        addItem(item: Select2Item): void;
        addItem(key: string, text: string, source?: any, disabled?: boolean): void;
        addInplaceCreate(addTitle?: string, editTitle?: string): void;
        inplaceCreateClick(e: any): void;
        getCreateSearchChoice(getName?: (p1: any) => string): (p1: string) => any;
        setEditValue(source: any, property: PropertyItem): void;
        getEditValue(property: PropertyItem, target: any): void;
        get_delimited(): boolean;
        get_select2Container(): JQuery;
        get_items(): Select2Item[];
        get_itemByKey(): any;
        value: string;
        values: string[];
        get_text(): string;
    }

    class CheckTreeEditor<TItem, TOptions> extends DataGrid<TItem, TOptions> {
        constructor(input: JQuery, opt?: TOptions);
        protected getTreeItems(): TItem[];
        protected updateItems(): void;
        protected itemSelectedChanged(item: TItem): void;
        protected getSelectAllText(): string;
        protected isThreeStateHierarchy(): boolean;
        protected getInitialCollapse(): boolean;
        protected updateSelectAll(): void;
        protected updateFlags(): void;
        protected getDescendantsSelected(item: TItem): boolean;
        protected allDescendantsSelected(item: TItem): boolean;
        protected getItemText(ctx: Slick.FormatterContext): string;
        protected sortItems(): void;
        protected moveSelectedUp(): boolean;
        public value: string[];
    }

    interface EmailEditorOptions {
        domain?: string;
        readOnlyDomain?: boolean;
    }

    class EmailEditor extends Widget<EmailEditorOptions> {
        constructor(input: JQuery, opt: EmailEditorOptions);
        static registerValidationMethods(): void;
        value: string;
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
    }

    class PasswordEditor extends StringEditor {
        constructor(input: JQuery);
    }

    class PersonNameEditor extends Widget<any> {
        constructor(input: JQuery);
        get_value(): string;
        set_value(value: string): void;
    }

    class DateEditor extends Widget<any> {
        constructor(input: JQuery);
        static dateInputChange(e: any): void;
        static dateInputKeyup(e: any): void;
        static defaultAutoNumericOptions(): any;
        value: string;
        valueAsDate: Date;
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
        get_minValue(): string;
        set_minValue(value: string): void;
        get_maxValue(): string;
        set_maxValue(value: string): void;
        get_minDate(): Date;
        set_minDate(value: Date): void;
        get_maxDate(): Date;
        set_maxDate(value: Date): void;
        get_sqlMinMax(): boolean;
        set_sqlMinMax(value: boolean): void;
    }

    class DateFiltering extends BaseEditorFiltering<DateEditor> {
    }

    class DateFormatter implements Slick.Formatter {
        format(ctx: Slick.FormatterContext): string;
        static format(value: any, format: string): string;
        get_displayFormat(): string;
        set_displayFormat(value: string): void;
    }

    class DateTimeEditor extends Widget<DateTimeEditorOptions> {
        constructor(input: JQuery, opt: DateTimeEditorOptions);
        static roundToMinutes(date: Date, minutesStep: number): Date;
        value: string;
        valueAsDate: Date;
        get_minValue(): string;
        set_minValue(value: string): void;
        get_maxValue(): string;
        set_maxValue(value: string): void;
        get_minDate(): Date;
        set_minDate(value: Date): void;
        get_maxDate(): Date;
        set_maxDate(value: Date): void;
        get_sqlMinMax(): boolean;
        set_sqlMinMax(value: boolean): void;
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
    }

    interface DateTimeEditorOptions {
        startHour?: any;
        endHour?: any;
        intervalMinutes?: any;
    }

    class DateTimeFiltering extends BaseEditorFiltering<DateEditor> {
    }

    class DateTimeFormatter extends DateFormatter {
    }

    interface Select2Item {
        id: string;
        text: string;
        source: any;
        disabled: boolean;
    }

    class SelectEditor extends Select2Editor<SelectEditorOptions, Select2Item> {
        constructor(hidden: JQuery, opt: SelectEditorOptions);
        getItems(): any[];
        updateItems(): void;
    }

    interface SelectEditorOptions {
        items?: any[];
        emptyOptionText?: string;
    }

    class DateYearEditor extends SelectEditor {
        constructor(hidden: JQuery, opt: DateYearEditorOptions);
    }

    interface DateYearEditorOptions extends SelectEditorOptions {
        minYear?: string;
        maxYear?: string;
        descending?: boolean;
    }

    class DecimalEditor extends Widget<DecimalEditorOptions> {
        constructor(input: JQuery, opt?: DecimalEditorOptions);
        static defaultAutoNumericOptions(): any;
        value: number;
        get_isValid(): boolean;
    }

    interface DecimalEditorOptions {
        minValue?: string;
        maxValue?: string;
        decimals?: any;
        padDecimals?: any;
    }

    class DecimalFiltering extends BaseEditorFiltering<DecimalEditor> {
    }

    interface LookupEditorOptions {
        lookupKey?: string;
        minimumResultsForSearch?: any;
        inplaceAdd?: boolean;
        dialogType?: string;
        cascadeFrom?: string;
        cascadeField?: string;
        cascadeValue?: any;
        filterField?: string;
        filterValue?: any;
        multiple?: boolean;
        delimited?: boolean;
    }

    class LookupEditorBase<TOptions extends LookupEditorOptions, TItem> extends Select2Editor<TOptions, TItem> {
        constructor(input: JQuery, opt?: TOptions);
        protected cascadeItems(items: TItem[]): TItem[];
        protected filterItems(items: TItem[]): TItem[];
        protected getCasecadeFromValue(parent: Widget<any>): any;
        protected getItems(lookup: Q.Lookup<TItem>): TItem[];
        protected getItemText(item: TItem, lookup: Q.Lookup<TItem>): string;
        protected getItemDisabled(item: TItem, lookup: Q.Lookup<TItem>): boolean;
        protected getLookup(): Q.Lookup<TItem>;
        protected getLookupKey(): string;
        protected initNewEntity(entity: TItem): void;
        protected updateItems(): void;
        protected getDialogTypeKey(): string;
        protected createEditDialog(callback: (dlg: Serenity.IEditDialog) => void): void;
        onInitNewEntity: (entity: TItem) => void;
        value: string;
        cascadeField: string;
        cascadeValue: any;
        filterField: string;
        filterValue: any;
    }

    class LookupEditor extends LookupEditorBase<LookupEditorOptions, any> {
        constructor(input: JQuery, opt?: LookupEditorOptions);
    }

    class AsyncLookupEditor extends LookupEditorBase<LookupEditorOptions, any> {
        constructor(hidden: JQuery, opt: LookupEditorOptions);
    }

    interface ToolbarOptions {
        buttons?: ToolButton[];
        hotkeyContext?: any;
    }

    class Toolbar extends Widget<ToolbarOptions> {
        constructor(div: JQuery, options: ToolbarOptions);
        findButton(className: string): JQuery;
    }

    class BooleanFiltering extends BaseFiltering {
    }

    class BooleanFormatter implements Slick.Formatter {
        format(ctx: Slick.FormatterContext): string;
        get_falseText(): string;
        set_falseText(value: string): void;
        get_trueText(): string;
        set_trueText(value: string): void;
    }

    class CascadedWidgetLink<TParent> {
        constructor(widget: Serenity.Widget<any>, parentChange: (p1: TParent) => void);
        bind(): TParent;
        unbind(): TParent;
        get_parentID(): string;
        set_parentID(value: string): void;
    }

    class CheckboxFormatter implements Slick.Formatter {
        format(ctx: Slick.FormatterContext): string;
    }

    class CheckListEditor extends Widget<CheckListEditorOptions> {
        constructor(div: JQuery, opt: CheckListEditorOptions);
        getItems(): CheckListItem[];
        updateItems(): void;
    }

    interface CheckListEditorOptions {
        items?: CheckListItem[];
        selectAllOptionText?: string;
    }

    class CheckListItem {
        id: string;
        text: string;
        parentId: string;
    }

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

    const enum ColumnSelection {
        List = 0,
        KeyOnly = 1,
        Details = 2
    }

    class IntegerEditor extends Widget<IntegerEditorOptions> {
        constructor(input: JQuery, opt?: IntegerEditorOptions);
        value: number;
    }

    interface IntegerEditorOptions {
        minValue?: number;
        maxValue?: number;
    }

    class IntegerFiltering extends BaseEditorFiltering<IntegerEditor> {
    }

    interface IReadOnly {
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
    }

    interface HtmlContentEditorOptions {
    }

    class ISetEditValue {
    }

    interface ISetEditValue {
        setEditValue(source: any, property: PropertyItem): void;
    }

    interface IStringValue {
        get_value(): string;
        set_value(value: string): void;
    }

    class TemplatedPanel<TOptions> extends TemplatedWidget<TOptions> {
        constructor(container: JQuery, options?: TOptions);
        protected tabs: JQuery;
        protected toolbar: Serenity.Toolbar;
        protected validator: JQueryValidation.Validator;
        protected arrange(): void;
        protected isPanel: boolean;
        protected responsive: boolean;
        protected arrange(): void;
        protected getToolbarButtons(): ToolButton[];
        protected getValidatorOptions(): JQueryValidation.ValidationOptions;
        protected initTabs(): void;
        protected initToolbar(): void;
        protected initValidator(): void;
        protected resetValidation(): void;
        protected validateForm(): boolean;
    }

    class PropertyDialog<TItem, TOptions> extends TemplatedDialog<TOptions> {
        constructor(options?: TOptions);
        protected entity: TItem;
        protected entityId: any;
        protected getFormKey(): string;
        protected getPropertyGridOptions(): PropertyGridOptions;
        protected getPropertyGridOptionsAsync(): PromiseLike<PropertyGridOptions>;
        protected getPropertyItems(): PropertyItem[];
        protected getPropertyItemsAsync(): PromiseLike<PropertyItem[]>;
        protected getSaveEntity(): TItem;
        protected initializeAsync(): PromiseLike<void>;
        protected loadInitialEntity(): void;
        protected set_entity(entity: TItem): void;
        protected set_entityId(value: any): void;
        protected validateBeforeSave(): boolean;
    }

    class PropertyPanel<TItem, TOptions> extends TemplatedPanel<TOptions> {
        constructor(container: JQuery, options?: TOptions);
        protected getFormKey(): string;
        protected getPropertyGridOptions(): PropertyGridOptions;
        protected getPropertyGridOptionsAsync(): PromiseLike<PropertyGridOptions>;
        protected getPropertyItems(): PropertyItem[];
        protected getPropertyItemsAsync(): PromiseLike<PropertyItem[]>;
        protected getSaveEntity(): TItem;
        protected get_entity(): TItem;
        protected get_entityId(): any;
        protected initializeAsync(): PromiseLike<void>;
        protected loadInitialEntity(): void;
        protected set_entity(entity: TItem): void;
        protected set_entityId(value: any): void;
        protected validateBeforeSave(): boolean;
    }

    class EntityDialog<TItem, TOptions> extends TemplatedDialog<TOptions> {
        constructor(options?: TOptions);
        protected saveAndCloseButton: JQuery;
        protected applyChangesButton: JQuery;
        protected deleteButton: JQuery;
        protected undeleteButton: JQuery;
        protected cloneButton: JQuery;
        protected entity: TItem;
        protected entityId: any;
        protected toolbar: Toolbar;
        dialogOpen(): void;
        loadByIdAndOpenDialog(id: any): void;
        protected afterLoadEntity(): void;
        protected beforeLoadEntity(entity: TItem): void;
        protected deleteHandler(options: ServiceOptions<DeleteResponse>, callback: (response: DeleteResponse) => void): void;
        protected doDelete(callback: (response: DeleteResponse) => void): void;
        protected getCloningEntity(): TItem;
        protected getDeleteOptions(callback: (response: DeleteResponse) => void): ServiceOptions<DeleteResponse>;
        protected getEntityIdField(): string;
        protected getEntityIsActiveField(): string;
        protected getEntityNameField(): string;
        protected getEntityNameFieldValue(): any;
        protected getEntitySingular(): string;
        protected getEntityTitle(): string;
        protected getEntityType(): string;
        protected getFormKey(): string;
        protected getLanguages(): string[][];
        protected getLoadByIdOptions(id: any, callback: (response: RetrieveResponse<TItem>) => void): ServiceOptions<RetrieveResponse<TItem>>;
        protected getLoadByIdRequest(id: any): RetrieveRequest;
        protected getLocalTextPrefix(): string;
        protected getPropertyGridOptions(): PropertyGridOptions;
        protected getPropertyGridOptionsAsync(): PromiseLike<PropertyGridOptions>;
        protected getPropertyItems(): PropertyItem[];
        protected getPropertyItemsAsync(): PromiseLike<PropertyItem[]>;
        protected getSaveEntity(): TItem;
        protected getSaveOptions(callback: (response: SaveResponse) => void): ServiceOptions<SaveResponse>;
        protected getSaveRequest(): SaveRequest<TItem>;
        protected getService(): string;
        protected getToolbarButtons(): ToolButton[];
        protected getUndeleteOptions(callback: (response: UndeleteResponse) => void): ServiceOptions<UndeleteResponse>;
        protected get_entity(): TItem;
        protected get_entityId(): any;
        protected isCloneMode(): boolean;
        protected isDeleted(): boolean;
        protected isEditMode(): boolean;
        protected isLocalizationMode(): boolean;
        protected isNew(): boolean;
        protected isNewOrDeleted(): boolean;
        protected initToolbar(): void;
        protected initializeAsync(): PromiseLike<void>;
        public load(entityOrId: any, done: () => void, fail: () => void): void;
        public loadById(id: any): void;
        public loadByIdAndOpenDialog(id: any): void;
        protected loadByIdHandler(options: ServiceOptions<RetrieveResponse<TItem>>, callback: (response: RetrieveResponse<TItem>) => void, fail: () => void): void;
        public loadEntity(entity: any): void;
        public loadEntityAndOpenDialog(entity: any): void;
        public loadNewAndOpenDialog(): void;
        public loadResponse(response: RetrieveResponse<TItem>): void;
        protected onDeleteSuccess(response: DeleteResponse): void;
        protected onLoadingData(data: RetrieveResponse<TItem>): void;
        protected onSaveSuccess(response: SaveResponse): void;
        protected reloadById(): void;
        protected save(callback: (response: SaveResponse) => void): void;
        protected saveHandler(options: ServiceOptions<SaveResponse>, callback: (response: SaveResponse) => void): void;
        protected save_submitHandler(callback: (response: SaveResponse) => void): void;
        protected set_entity(entity: any): void;
        protected set_entityId(id: any): void;
        protected showSaveSuccessMessage(response: SaveResponse): void;;
        protected undelete(): void;
        protected undeleteHandler(options: ServiceOptions<UndeleteResponse>, callback: (response: UndeleteResponse) => void): void;
        protected updateInterface(): void;
        protected updateTitle(): void;
        protected validateBeforeSave(): boolean;
    }

    interface QuickFilter<TWidget extends Widget<TOptions>, TOptions> {
        field?: string;
        type?: new (element: JQuery, options: TOptions) => TWidget;
        handler?: (h: QuickFilterArgs<TWidget>) => void;
        title?: string;
        options?: TOptions;
        element?: (e: JQuery) => void;
        init?: (w: TWidget) => void;
    }

    interface GridPersistanceFlags {
        columnWidths?: boolean;
        columnVisibility?: boolean;
        sortColumns?: boolean;
        filterItems?: boolean;
        quickFilters?: boolean;
        includeDeleted?: boolean;
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
        includeDeleted?: boolean;
    }

    class DataGrid<TItem, TOptions> extends Widget<TOptions> {
        constructor(container: JQuery, options?: TOptions);
        dialogOpen(): void;
        loadByIdAndOpenDialog(id: any): void;
        protected allColumns: Slick.Column[];
        protected defaultColumns: string[];
        protected titleDiv: JQuery;
        protected filterBar: FilterDisplayBar;
        protected quickFiltersDiv: JQuery;
        protected slickContainer: JQuery;
        protected toolbar: Toolbar;
        protected addDateRangeFilter(field: string, title?: string): DateEditor;
        protected addQuickFilter<TWidget extends Widget<any>, TOptions>(filter: QuickFilter<TWidget, TOptions>): TWidget;
        protected addFilterSeperator(): void;
        protected add_submitHandlers(action: () => void): void;
        protected remove_submitHandlers(action: () => void): void;
        protected bindToSlickEvents(): void;
        protected bindToViewEvents(): void;
        protected createFilterBar(): void;
        protected createIncludeDeletedButton(): void;
        protected createPager(): void;
        protected createQuickFilters(): void;
        protected createQuickSearchInput(): void;
        protected createSlickContainer(): JQuery;
        protected createSlickGrid(): Slick.Grid;
        protected createToolbar(): void;
        protected createToolbarExtensions(): void;
        protected createView(): Slick.RemoteView<TItem>;
        protected determineText(text: string, getKey: (s: string) => string): string;
        protected editItem(entityOrId: any): void;
        protected editItemOfType(itemType: string, entityOrId: any): void;
        protected enableFiltering(): boolean;
        protected findQuickFilter<TWidget>(type: { new (...args: any[]): TWidget }, field: string): TWidget
        protected getAddButtonCaption(): string;
        protected getButtons(): ToolButton[];
        protected getColumns(): Slick.Column[];
        protected getColumnsAsync(): PromiseLike<Slick.Column[]>;
        protected getColumnsKey(): string;
        protected getCurrentSettings(flags?: GridPersistanceFlags): PersistedGridSettings;
        protected getPersistanceFlags(): GridPersistanceFlags;
        protected getPersistanceStorage(): SettingStorage;
        protected getPersistanceKey(): string;
        protected persistSettings(flags?: GridPersistanceFlags): void;
        protected restoreSettings(settings?: PersistedGridSettings, flags?: GridPersistanceFlags): void;
        protected getDefaultSortBy(): string[];
        protected getGridCanLoad(): boolean;
        protected getIdProperty(): string;
        protected getIncludeColumns(include: { [key: string]: boolean }): void;
        protected getInitialTitle(): string;
        protected getIsActiveFieldName(): string;
        protected getItemCssClass(item: TItem, index: number): string;
        protected getItemMetadata(item: TItem): any;
        protected getItemType(): string;
        protected getLocalTextPrefix(): string;
        protected getPropertyItems(): PropertyItem[];
        protected getPropertyItemsAsync(): PromiseLike<PropertyItem[]>;
        protected getQuickFilters(): QuickFilter<Widget<any>, any>[];
        protected getQuickSearchFields(): QuickSearchField[];
        protected getSlickOptions(): Slick.GridOptions;
        protected getViewOptions(): Slick.RemoteViewOptions;
        protected initialPopulate(): void;
        protected initializeAsync(): PromiseLike<void>;
        protected internalRefresh(): void;
        protected invokeSubmitHandlers(): void;
        protected itemLink(itemType?: string, idField?: string, text?: (ctx: Slick.FormatterContext) => string,
            cssClass?: (ctx: Slick.FormatterContext) => string, encode?: boolean): void;
        protected layout(): void;
        protected markupReady(): void;
        protected onClick(e: JQueryEventObject, row: number, cell: number): void;
        protected onViewFilter(item: TItem): boolean;
        protected onViewProcessData(response: ListResponse<TItem>): ListResponse<TItem>;
        protected onViewSubmit(): boolean;
        protected populateLock(): void;
        protected populateUnlock(): void;
        protected populateWhenVisible(): void;
        protected postProcessColumns(columns: Slick.Column[]): Slick.Column[];
        protected propertyItemsToSlickColumns(items: PropertyItem[]): Slick.Column[];
        protected quickFilterChange(e: JQueryEventObject): void;
        protected resizeCanvas(): void;
        protected setCriteriaParameter(): void;
        protected setEquality(field: string, value: any): void;
        protected setIncludeColumnsParameter(): void;
        protected setInitialSortOrder(): void;
        protected subDialogDataChange(): void;
        protected updateDisabledState(): void;
        protected usePager(): boolean;
        public refresh(): void;
        public getItems(): TItem[];
        public setItems(value: TItem[]): void;
        public isDisabled: boolean;
        public setIsDisabled(value: boolean): void;
        public getTitle(): string;
        public setTitle(value: string): void;
        public itemAt(row: number): TItem;
        public rowCount(): number;
        public view: Slick.RemoteView<TItem>;
        public slickGrid: Slick.Grid;
        public getElement(): JQuery;
        public getFilterStore(): FilterStore;
        public getGrid(): Slick.Grid;
        public getView(): Slick.RemoteView<TItem>;
        public static defaultHeaderHeight: number;
        public static defaultRowHeight: number;
        public static defaultPersistanceStorage: SettingStorage;
    }

    interface SettingStorage {
        getItem(key: string): string;
        setItem(key: string, value: string): void;
    }

    class EntityGrid<TItem, TOptions> extends DataGrid<TItem, TOptions> {
        constructor(container: JQuery, options?: TOptions);
        protected addButtonClick(): void;
        protected createEntityDialog(itemType: string, callback?: (dlg: Widget<any>) => void): Widget<any>;
        protected getDialogOptions(): any;
        protected getDialogOptionsFor(itemType: string): any;
        protected getDialogType(): { new (...args: any[]): Widget<any> };
        protected getDialogTypeFor(itemType: string): { new (...args: any[]): Widget<any> };
        protected getDisplayName(): string;
        protected getItemName(): string;
        protected getEntityType(): string;
        protected getService(): string;
        protected initDialog(dialog: Widget<any>): void;
        protected initEntityDialog(itemType: string, dialog: Widget<any>): void;
        protected newRefreshButton(noText?: boolean): ToolButton;
    }

    interface CKEditorConfig {
    }

    interface IDataGrid {
        getElement(): JQuery;
        getGrid(): Slick.Grid;
        getView(): Slick.RemoteView<any>;
        getFilterStore(): Serenity.FilterStore;
    }

    enum CaptureOperationType {
        Before = 0,
        Delete = 1,
        Insert = 2,
        Update = 3
    }

    namespace CustomValidation {
        function registerValidationMethods(): void;
    }

    class BaseEditorFiltering<TEditor> extends BaseFiltering {
        editor: Serenity.Widget<any>;
        useEditor(): boolean;
        useIdField(): boolean;
        getEditorOptions(): any;
    }

    interface FilterOperator {
        key?: string;
        title?: string;
        format?: string;
    }

    namespace FilterOperators {
        let isTrue: string;
        let isFalse: string;
        let contains: string;
        let startsWith: string;
        let eQ: string;
        let nE: string;
        let gT: string;
        let gE: string;
        let lT: string;
        let lE: string;
        let bW: string;
        let iN: string;
        let isNull: string;
        let isNotNull: string;
        let toCriteriaOperator: { [key: string]: string };
    }

    class BaseFiltering {
        getOperators(): FilterOperator[];
        appendNullableOperators(list: FilterOperator[]): FilterOperator[];
        appendComparisonOperators(list: FilterOperator[]): FilterOperator[];
        isNullable(): boolean;
        createEditor(): void;
        operatorFormat(op: FilterOperator): string;
        getTitle(field: PropertyItem): string;
        displayText(op: FilterOperator, values: any): string;
        getCriteriaField(): string;
        getCriteria(displayText: any): any[];
        loadState(state: any): void;
        saveState(): any;
        argumentNull(): any;
        validateEditorValue(value: string): any;
        getEditorValue(): any;
        getEditorText(): string;
        get_field(): PropertyItem;
        set_field(value: PropertyItem): void;
        get_container(): JQuery;
        set_container(value: JQuery): void;
        get_operator(): FilterOperator;
        set_operator(value: FilterOperator): void;
    }

    namespace DialogExtensions {
        function dialogFlexify(dialog: JQuery): JQuery;
        function dialogResizable(dialog: JQuery, w?: any, h?: any, mw?: any, mh?: any): JQuery;
        function dialogMaximizable(dialog: JQuery): JQuery;
        function dialogCloseOnEnter(dialog: JQuery): JQuery;
    }

    interface DialogButton {
        text: string;
        click: () => void;
    }

    namespace DialogTypeRegistry {
        function get(key: string): Function;
    }

    class EditorFiltering extends BaseEditorFiltering<Serenity.Widget<any>> {
        get_editorType(): string;
        set_editorType(value: string): void;
        get_useRelative(): boolean;
        set_useRelative(value: boolean): void;
        get_useLike(): boolean;
        set_useLike(value: boolean): void;
    }

    class EditorTypeEditor extends SelectEditor {
        constructor(select: JQuery);
    }

    interface EditorTypeInfo {
        type?: Function;
        displayName?: string;
        optionsType?: Function;
    }

    namespace EditorTypeRegistry {
        function get(key: string): Function;
        function initialize(): void;
        function reset(): void;
    }

    namespace Reporting {
        class ReportDialog extends TemplatedDialog<ReportDialogOptions> {
            constructor(opt: ReportDialogOptions);
            createPropertyGrid(): void;
            loadReport(reportKey: string): void;
            executeReport(targetFrame: string, exportType: string): void;
        }

        interface ReportDialogOptions {
            reportKey?: string;
        }

        interface ReportExecuteRequest extends ServiceRequest {
            ExportType?: string;
            ReportKey?: string;
            DesignId?: string;
            Parameters?: any;
        }

        class ReportPage extends Serenity.Widget<any> {
            constructor(div: JQuery);
        }

        interface ReportRetrieveRequest extends ServiceRequest {
            ReportKey?: string;
        }

        interface ReportRetrieveResponse extends ServiceResponse {
            ReportKey?: string;
            Properties?: PropertyItem[];
            Title?: string;
            InitialSettings?: any;
            IsDataOnlyReport?: boolean;
        }
    }
}