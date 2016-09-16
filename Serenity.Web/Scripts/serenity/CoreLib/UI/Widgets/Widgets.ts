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
        cascadeFrom: string;
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

    interface SettingStorage {
        getItem(key: string): string;
        setItem(key: string, value: string): void;
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