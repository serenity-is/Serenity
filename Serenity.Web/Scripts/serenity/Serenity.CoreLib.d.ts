declare class RSVP {
}
interface RSVP<TResult> {
    constructor(constructor: (p1: (p1: any) => void, p2: any) => void): any;
    then(onFulfilled: (p1: any) => void, onRejected?: any): RSVP<any>;
    then(onFulfilled: (p1: any) => any, onRejected?: any): RSVP<any>;
    then(onFulfilled: (p1: any) => RSVP<any>, onRejected?: any): RSVP<any>;
    then(onFulfilled: (p1: any) => RSVP<any>, onRejected?: any): RSVP<any>;
}
declare namespace JsRender {
    function render(markup: string, data?: any): string;
}
declare namespace System.ComponentModel {
    class DisplayNameAttribute {
        constructor(displayName: string);
        displayName: string;
    }
}
declare namespace Serenity {
    namespace SlickTreeHelper {
        function filterCustom(item: any, getParent: (p1: any) => any): boolean;
        function filterById(item: any, view: Slick.RemoteView<any>, getParentId: (p1: any) => any): boolean;
        function setCollapsed(items: any, collapsed: boolean): void;
        function setCollapsedFlag(item: any, collapsed: boolean): void;
        function setIndents(items: any[], getId: (p1: any) => any, getParentId: (p1: any) => any, setCollapsed?: any): void;
        function toggleClick(e: any, row: number, cell: number, view: Slick.RemoteView<any>, getId: (p1: any) => any): void;
    }
    class StringFiltering extends BaseFiltering {
    }
    namespace SubDialogHelper {
        function bindToDataChange(dialog: any, owner: Serenity.Widget<any>, dataChange: (p1: any, p2: DataChangeInfo) => void, useTimeout?: boolean): any;
        function triggerDataChange(dialog: any): any;
        function triggerDataChange(element: JQuery): JQuery;
        function bubbleDataChange(dialog: any, owner: Serenity.Widget<any>, useTimeout?: boolean): any;
        function cascade(cascadedDialog: any, ofElement: JQuery): any;
        function cascadedDialogOffset(element: JQuery): any;
    }
    class TextAreaEditor extends Widget<TextAreaEditorOptions> {
        constructor(input: JQuery, opt: TextAreaEditorOptions);
        get_value(): string;
        set_value(value: string): void;
    }
    class TextAreaEditorOptions {
    }
    interface TextAreaEditorOptions {
        cols?: number;
        rows?: number;
    }
    class TimeEditor extends Widget<TimeEditorOptions> {
        constructor(input: JQuery, opt: TimeEditorOptions);
        get_value(): any;
        set_value(value: any): void;
    }
    interface TimeEditorOptions {
        noEmptyOption: boolean;
        startHour: any;
        endHour: any;
        intervalMinutes: any;
    }
    interface ToastrOptions {
        target?: any;
        containerId?: string;
        positionClass?: string;
        timeOut?: number;
        showDuration?: number;
        hideDuration?: number;
        extendedTimeOut?: number;
        progressBar?: boolean;
        closeButton?: boolean;
    }
    interface ToolButton {
        title?: string;
        hint?: string;
        cssClass?: string;
        onClick?: any;
        htmlEncode?: any;
        hotkey?: string;
        hotkeyAllowDefault?: boolean;
    }
    namespace UploadHelper {
        function addUploadInput(options: UploadInputOptions): JQuery;
        function checkImageConstraints(file: UploadResponse, opt: ImageUploadEditorOptions): boolean;
        function fileNameSizeDisplay(name: string, bytes: number): string;
        function fileSizeDisplay(bytes: number): string;
        function hasImageExtension(filename: string): boolean;
        function thumbFileName(filename: string): string;
        function dbFileUrl(filename: string): string;
        function colorBox(link: JQuery, options: any): void;
        function populateFileSymbols(container: JQuery, items: UploadedFile[], displayOriginalName?: boolean, urlPrefix?: string): void;
    }
    interface UploadInputOptions {
        container: JQuery;
        progress: JQuery;
        inputName: string;
        allowMultiple: boolean;
        fileDone: (p1: UploadResponse, p2: string, p3: any) => void;
    }
    interface UploadResponse {
        TemporaryFile: string;
        Size: number;
        IsImage: boolean;
        Width: number;
        Height: number;
    }
    class URLEditor extends StringEditor {
        constructor(input: JQuery);
    }
    class UrlFormatter implements Slick.Formatter {
        format(ctx: Slick.FormatterContext): string;
        get_displayProperty(): string;
        set_displayProperty(value: string): void;
        get_urlProperty(): string;
        set_urlProperty(value: string): void;
    }
    namespace ValidationHelper {
        function asyncSubmit(form: JQuery, validateBeforeSave: () => boolean, submitHandler: () => void): boolean;
        function submit(form: JQuery, validateBeforeSave: () => boolean, submitHandler: () => void): boolean;
        function getValidator(element: JQuery): JQueryValidation.Validator;
    }
    namespace VX {
        function addValidationRule(widget: Serenity.Widget<any>, eventClass: string, rule: (p1: JQuery) => string): JQuery;
        function addValidationRule(element: JQuery, eventClass: string, rule: (p1: JQuery) => string): JQuery;
        function removeValidationRule(element: JQuery, eventClass: string): JQuery;
        function validateElement(validator: JQueryValidation.Validator, widget: Serenity.Widget<any>): boolean;
    }
    namespace WX {
        function getWidget(element: JQuery): any;
        function tryGetWidget(element: JQuery): any;
        function getWidgetName(type: Function): string;
        function hasOriginalEvent(e: any): boolean;
        function change(widget: any, handler: any): void;
        function changeSelect2(widget: any, handler: any): void;
        function getGridField(widget: Serenity.Widget<any>): JQuery;
        function create(initElement: (p1: JQuery) => void, options?: any): any;
    }
    namespace ComponentModel {
        class CategoryAttribute {
            constructor(category: string);
            category: string;
        }
        class CssClassAttribute {
            constructor(cssClass: string);
            cssClass: string;
        }
        class DefaultValueAttribute {
            constructor(defaultValue: any);
            value: any;
        }
        class EditorOptionAttribute {
            constructor(key: string, value: any);
            key: string;
            value: any;
        }
        class EditorTypeAttribute extends EditorTypeAttributeBase {
            constructor(editorType: string);
        }
        class EditorTypeAttributeBase {
            constructor(type: string);
            setParams(editorParams: any): void;
            editorType: string;
        }
        class HiddenAttribute {
        }
        class HintAttribute {
            constructor(hint: string);
            hint: string;
        }
        class InsertableAttribute {
            constructor(insertable?: boolean);
            value: boolean;
        }
        class MaxLengthAttribute {
            constructor(maxLength: number);
            maxLength: number;
        }
        class OneWayAttribute {
        }
        class PlaceholderAttribute {
            constructor(value: string);
            value: string;
        }
        class ReadOnlyAttribute {
            constructor(readOnly?: boolean);
            value: boolean;
        }
        class RequiredAttribute {
            constructor(isRequired: boolean);
            isRequired: boolean;
        }
        class UpdatableAttribute {
            constructor(updatable?: boolean);
            value: boolean;
        }
    }
    class PropertyGrid extends Widget<PropertyGridOptions> {
        constructor(div: JQuery, opt: PropertyGridOptions);
        load(source: any): void;
        static loadEditorValue(editor: Serenity.Widget<any>, item: PropertyItem, source: any): void;
        save(target: any): void;
        static saveEditorValue(editor: Serenity.Widget<any>, item: PropertyItem, target: any): void;
        enumerateItems(callback: (p1: PropertyItem, p2: Serenity.Widget<any>) => void): void;
        static setRequired(widget: Serenity.Widget<any>, isRequired: boolean): void;
        static setReadOnly(widget: Serenity.Widget<any>, isReadOnly: boolean): void;
        static setReadOnly(elements: JQuery, isReadOnly: boolean): JQuery;
        get_editors(): any;
        get_items(): any;
        get_idPrefix(): string;
        get_mode(): PropertyGridMode;
        set_mode(value: PropertyGridMode): void;
    }
    const enum PropertyGridMode {
        insert = 1,
        update = 2,
    }
    interface PropertyGridOptions {
        idPrefix: string;
        items: PropertyItem[];
        useCategories: boolean;
        categoryOrder: string;
        defaultCategory: string;
        localTextPrefix: string;
        mode: PropertyGridMode;
    }
    namespace SlickFormatting {
        function getEnumText(value: any): string;
        function getEnumText(enumKey: string, name: string): string;
        function treeToggle(getView: () => Slick.RemoteView<any>, getId: (p1: any) => any, formatter: Slick.Formatter): Slick.Formatter;
        function date(format?: string): Slick.Formatter;
        function dateTime(format?: string): Slick.Formatter;
        function checkBox(): Slick.Formatter;
        function number(format: string): Slick.Formatter;
        function getItemType(link: JQuery): string;
        function getItemId(link: JQuery): string;
        function itemLinkText(itemType: string, id: any, text: any, extraClass: string, encode: boolean): string;
        function itemLink(itemType: string, idField: string, getText: (p1: Slick.FormatterContext) => string, cssClass?: (p1: Slick.FormatterContext) => string, encode?: boolean): Slick.Formatter;
    }
    namespace SlickHelper {
        function setDefaults(columns: any, localTextPrefix?: string): any;
        function convertToFormatter(format: Slick.Formatter): Slick.ColumnFormatter;
    }
    class PhoneEditor extends Widget<PhoneEditorOptions> {
        constructor(input: JQuery, opt: PhoneEditorOptions);
        validate(value: string): string;
        formatValue(): void;
        getFormattedValue(): string;
        get_value(): string;
        set_value(value: string): void;
    }
    class PhoneEditorOptions {
        multiple: boolean;
        internal: boolean;
        mobile: boolean;
        allowExtension: boolean;
        allowInternational: boolean;
    }
    class PopupMenuButton extends Widget<PopupMenuButtonOptions> {
        constructor(div: JQuery, opt: PopupMenuButtonOptions);
    }
    interface PopupMenuButtonOptions {
        menu?: JQuery;
        onPopup?: () => void;
        positionMy?: string;
        positionAt?: string;
    }
    class PopupToolButton extends PopupMenuButton {
        constructor(div: JQuery, opt: PopupToolButtonOptions);
    }
    interface PopupToolButtonOptions extends PopupMenuButtonOptions {
    }
    class MultipleImageUploadEditor extends Widget<ImageUploadEditorOptions> {
        entities: UploadedFile[];
        toolbar: Toolbar;
        fileSymbols: JQuery;
        uploadInput: JQuery;
        constructor(div: JQuery, opt: ImageUploadEditorOptions);
        addFileButtonText(): string;
        getToolButtons(): ToolButton[];
        populate(): void;
        updateInterface(): void;
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
        get_value(): UploadedFile[];
        set_value(value: UploadedFile[]): void;
        get_jsonEncodeValue(): boolean;
        set_jsonEncodeValue(value: boolean): void;
    }
    class NumberFormatter {
        format(ctx: Slick.FormatterContext): string;
        static format(value: any, format: string): string;
        get_displayFormat(): string;
        set_displayFormat(value: string): void;
    }
    class MinuteFormatter implements Slick.Formatter {
        format(ctx: Slick.FormatterContext): string;
        static format(value: any): string;
    }
    class MaskedEditor extends Widget<MaskedEditorOptions> {
        constructor(input: JQuery, opt: MaskedEditorOptions);
        get_value(): string;
        set_value(value: string): void;
    }
    interface MaskedEditorOptions {
        mask?: string;
        placeholder?: string;
    }
    class LookupFiltering extends BaseEditorFiltering<LookupEditor> {
    }
    interface IValidateRequired {
        get_required(): boolean;
        set_required(value: boolean): void;
    }
    class HtmlContentEditor extends Widget<HtmlContentEditorOptions> {
        constructor(textArea: JQuery, opt: HtmlContentEditorOptions);
        instanceReady(x: any): void;
        getLanguage(): string;
        getConfig(): CKEditorConfig;
        get_value(): string;
        set_value(value: string): void;
    }
    class HtmlContentEditorOptions {
        cols: any;
        rows: any;
    }
    class HtmlReportContentEditor extends HtmlContentEditor {
        constructor(textArea: JQuery, opt: HtmlContentEditorOptions);
    }
    class IAsyncInit {
    }
    class IBooleanValue {
    }
    interface BooleanValue {
        get_value(): boolean;
        set_value(value: boolean): void;
    }
    class IDialog {
    }
    interface Dialog {
        dialogOpen(): void;
    }
    class IDoubleValue {
    }
    interface DoubleValue {
        get_value(): any;
        set_value(value: any): void;
    }
    interface EditDialog {
        load(entityOrId: any, done: () => void, fail: (p1: any) => void): void;
    }
    interface IFiltering {
        createEditor(): void;
        getCriteria(displayText: any): any[];
        getOperators(): FilterOperator[];
        loadState(state: any): void;
        saveState(): any;
        get_field(): PropertyItem;
        set_field(value: PropertyItem): void;
        get_container(): JQuery;
        set_container(value: JQuery): void;
        get_operator(): FilterOperator;
        set_operator(value: FilterOperator): void;
    }
    interface IGetEditValue {
        getEditValue(property: PropertyItem, target: any): void;
    }
    interface IInitializeColumn {
        initializeColumn(column: Slick.Column): void;
    }
    class EnumEditor extends Select2Editor<EnumEditorOptions, Select2Item> {
        constructor(hidden: JQuery, opt: EnumEditorOptions);
        updateItems(): void;
    }
    interface Select2AjaxOptions {
        transport: any;
        url: any;
        dataType: string;
        quietMillis: number;
        cache: boolean;
        jsonpCallback: any;
        data: (p1: string, p2: number, p3: any) => any;
        results: (p1: any, p2: number, p3: any) => any;
        params: any;
    }
    class Select2AjaxEditor<TOptions, TItem> extends Widget<TOptions> {
        pageSize: number;
        constructor(hidden: JQuery, opt: any);
        emptyItemText(): string;
        getService(): string;
        query(request: ListRequest, callback: (p1: ListResponse<any>) => void): void;
        executeQuery(options: ServiceOptions<ListResponse<any>>): void;
        queryByKey(key: string, callback: (p1: any) => void): void;
        executeQueryByKey(options: ServiceOptions<RetrieveResponse<any>>): void;
        getItemKey(item: any): string;
        getItemText(item: any): string;
        getTypeDelay(): number;
        getSelect2Options(): Select2Options;
        addInplaceCreate(title: string): void;
        inplaceCreateClick(e: any): void;
        get_select2Container(): JQuery;
        get_value(): string;
        set_value(value: string): void;
    }
    interface Select2QueryOptions {
        element: JQuery;
        term: string;
        page: number;
        context: any;
        callback: (p1: Select2Result) => void;
    }
    interface Select2Result {
        results: any;
        more: boolean;
        context: any;
    }
    interface Select2Options {
        width: any;
        minimumInputLength: number;
        maximumInputLength: number;
        minimumResultsForSearch: number;
        maximumSelectionSize: any;
        placeHolder: string;
        placeHolderOption: any;
        separator: string;
        allowClear: boolean;
        multiple: boolean;
        closeOnSelect: boolean;
        openOnEnter: boolean;
        id: (p1: any) => string;
        matcher: (p1: string, p2: string, p3: JQuery) => boolean;
        sortResults: (p1: any, p2: JQuery, p3: any) => any;
        formatSelection: (p1: any, p2: JQuery, p3: (p1: string) => string) => string;
        formatResult: (p1: any, p2: JQuery, p3: any, p4: (p1: string) => string) => string;
        formatResultCssClass: (p1: any) => string;
        formatNoMatches: (p1: string) => string;
        formatSearching: () => string;
        formatInputTooShort: (p1: string, p2: number) => string;
        formatSelectionTooBig: (p1: string) => string;
        createSearchChoice: (p1: string) => any;
        createSearchChoicePosition: string;
        initSelection: (p1: JQuery, p2: (p1: any) => void) => void;
        tokenizer: (p1: string, p2: any, p3: (p1: any) => any, p4: any) => string;
        tokenSeparators: any;
        query: (p1: Select2QueryOptions) => void;
        ajax: Select2AjaxOptions;
        data: any;
        tags: any;
        containerCss: any;
        containerCssClass: any;
        dropdownCss: any;
        dropdownCssClass: any;
        dropdownAutoWidth: boolean;
        adaptContainerCssClass: (p1: string) => string;
        adaptDropdownCssClass: (p1: string) => string;
        escapeMarkup: (p1: string) => string;
        selectOnBlur: boolean;
        loadMorePadding: number;
        nextSearchTerm: (p1: any, p2: string) => string;
    }
    interface EnumEditorOptions {
        enumKey: string;
    }
    class EnumFormatter implements Slick.Formatter {
        format(ctx: Slick.FormatterContext): string;
        static format(enumType: Function, value: any): string;
        static getText(enumKey: string, name: string): string;
        static getText(value: any): string;
        static getName(value: any): string;
        get_enumKey(): string;
        set_enumKey(value: string): void;
    }
    namespace EnumTypeRegistry {
        function get(key: string): Function;
    }
    class FileDownloadFormatter {
        format(ctx: Slick.FormatterContext): string;
        static dbFileUrl(filename: string): string;
        initializeColumn(column: Slick.Column): void;
        get_displayFormat(): string;
        set_displayFormat(value: string): void;
        get_originalNameProperty(): string;
        set_originalNameProperty(value: string): void;
    }
    class FilterableAttribute {
        constructor(value: boolean);
        value: boolean;
    }
    class FilterDialog extends TemplatedDialog<any> {
        get_filterPanel(): FilterPanel;
    }
    class FilterDisplayBar extends FilterWidgetBase<any> {
        constructor(div: JQuery);
    }
    namespace FilteringTypeRegistry {
        function get(key: string): Function;
        function initialize(): void;
        function reset(): void;
    }
    class FilterLine {
        field: string;
        operator: string;
        isOr: boolean;
        leftParen: boolean;
        rightParen: boolean;
        validationError: string;
        criteria: any[];
        displayText: string;
        state: any;
    }
    class Flexify extends Widget<FlexifyOptions> {
        constructor(container: JQuery, options: FlexifyOptions);
    }
    class FlexifyOptions {
        getXFactor: (p1: JQuery) => any;
        getYFactor: (p1: JQuery) => any;
        designWidth: any;
        designHeight: any;
    }
    namespace FLX {
        function flexHeightOnly(element: JQuery, flexY?: number): JQuery;
        function flexWidthOnly(element: JQuery, flexX?: number): JQuery;
        function flexWidthHeight(element: JQuery, flexX?: number, flexY?: number): JQuery;
        function flexXFactor(element: JQuery, flexX: number): JQuery;
        function flexYFactor(element: JQuery, flexY: number): JQuery;
    }
    namespace FormatterTypeRegistry {
        function get(key: string): Function;
        function initialize(): void;
        function reset(): void;
    }
    class FilterPanel extends FilterWidgetBase<any> {
        static panelTemplate: string;
        static rowTemplate: string;
        constructor(div: JQuery);
        updateRowsFromStore(): void;
        search(): void;
        get_showInitialLine(): boolean;
        set_showInitialLine(value: boolean): void;
        get_showSearchButton(): boolean;
        set_showSearchButton(value: boolean): void;
        get_updateStoreOnReset(): boolean;
        set_updateStoreOnReset(value: boolean): void;
        get_hasErrors(): boolean;
    }
    class FilterStore {
        constructor(fields: any);
        raiseChanged(): void;
        add_Changed(value: any): void;
        remove_Changed(value: any): void;
        get_fields(): PropertyItem[];
        get_fieldByName(): any;
        get_items(): FilterLine[];
        get_activeCriteria(): any[];
        get_displayText(): string;
    }
    class FilterWidgetBase<TOptions> extends TemplatedWidget<TOptions> {
        constructor(div: JQuery, opt: any);
        filterStoreChanged(): void;
        get_store(): FilterStore;
        set_store(value: FilterStore): void;
    }
    namespace EditorUtils {
        function getValue(editor: Serenity.Widget<any>): any;
        function saveValue(editor: Serenity.Widget<any>, item: PropertyItem, target: any): void;
        function setValue(editor: Serenity.Widget<any>, value: any): void;
        function loadValue(editor: Serenity.Widget<any>, item: PropertyItem, source: any): void;
        function setReadOnly(elements: JQuery, isReadOnly: boolean): JQuery;
        function setReadOnly(widget: Serenity.Widget<any>, isReadOnly: boolean): void;
        function setRequired(widget: Serenity.Widget<any>, isRequired: boolean): void;
    }
    class GoogleMap extends Widget<GoogleMapOptions> {
        constructor(container: JQuery, opt: GoogleMapOptions);
        get_map(): any;
    }
    interface GoogleMapOptions {
        latitude: any;
        longitude: any;
        zoom: any;
        mapTypeId: any;
        markerTitle: string;
        markerLatitude: any;
        markerLongitude: any;
    }
    class GridRows<TItem> {
    }
    class GridRowSelectionMixin extends ScriptContext {
        constructor(grid: IDataGrid);
        clear(): void;
        resetCheckedAndRefresh(): void;
        getSelectedKeys(): string[];
        getSelectedAsInt32(): number[];
        getSelectedAsInt64(): number[];
        static createSelectColumn(getMixin: () => GridRowSelectionMixin): Slick.Column;
    }
    namespace GridSelectAllButtonHelper {
        function update(grid: IDataGrid, getSelected: (p1: any) => boolean): void;
        function define(getGrid: () => IDataGrid, getId: (p1: any) => any, getSelected: (p1: any) => boolean, setSelected: (p1: any, p2: boolean) => void, text?: string, onClick?: () => void): ToolButton;
    }
    namespace GridUtils {
        function addToggleButton(toolDiv: JQuery, cssClass: string, callback: (p1: boolean) => void, hint: string, initial?: boolean): void;
        function addIncludeDeletedToggle(toolDiv: JQuery, view: Slick.RemoteView<any>, hint?: string, initial?: boolean): void;
        function addQuickSearchInput(toolDiv: JQuery, view: Slick.RemoteView<any>, fields?: QuickSearchField[]): void;
        function addQuickSearchInputCustom(container: JQuery, onSearch: (p1: string, p2: string) => void, fields?: QuickSearchField[]): void;
        function addQuickSearchInputCustom(container: JQuery, onSearch: (p1: string, p2: string, p3: (p1: boolean) => void) => void, fields?: QuickSearchField[]): void;
        function makeOrderable(grid: Slick.Grid, handleMove: (p1: any, p2: number) => void): void;
        function makeOrderableWithUpdateRequest(grid: DataGrid<any, any>, getId: (p1: any) => number, getDisplayOrder: (p1: any) => any, service: string, getUpdateRequest: (p1: number, p2: number) => SaveRequest<any>): void;
    }
    interface QuickSearchField {
        name: string;
        title: string;
    }
    namespace PropertyItemSlickConverter {
        function toSlickColumns(items: PropertyItem[]): Slick.Column[];
        function toSlickColumn(item: PropertyItem): Slick.Column;
    }
    interface PostToServiceOptions {
        url?: string;
        service?: string;
        target?: string;
        request: any;
    }
    interface PostToUrlOptions {
        url?: string;
        target?: string;
        params: any;
    }
    interface CommonDialogOptions extends JQueryUI.DialogOptions {
        onOpen?: () => void;
        onClose?: () => void;
        htmlEncode?: boolean;
        dialogClass?: string;
        title?: string;
    }
    interface AlertOptions extends CommonDialogOptions {
        okButton?: string;
    }
    interface ConfirmOptions extends CommonDialogOptions {
        yesButton?: string;
        noButton?: string;
        cancelButton?: string;
        onCancel?: () => void;
        onNo?: () => void;
    }
    namespace CustomValidation {
        function registerValidationMethods(): void;
    }
    type CustomValidationRule = (element: JQuery) => string;
    interface DataChangeInfo {
        type: string;
        entityId: any;
        entity: any;
    }
    interface ServiceError {
        Code?: string;
        Arguments?: string;
        Message?: string;
    }
    interface ServiceResponse {
        Error?: ServiceError;
    }
    interface ServiceRequest {
    }
    interface SaveRequest<TEntity> extends ServiceRequest {
        EntityId?: any;
        Entity?: TEntity;
    }
    interface SaveRequestWithAttachment<TEntity> extends SaveRequest<TEntity> {
        Attachments: any[];
    }
    interface SaveResponse extends ServiceResponse {
        EntityId: any;
    }
    interface SaveWithLocalizationRequest<TEntity> extends SaveRequest<TEntity> {
        Localizations?: {
            [key: string]: TEntity;
        };
    }
    interface DeleteRequest extends ServiceRequest {
        EntityId?: any;
    }
    interface DeleteResponse extends ServiceResponse {
    }
    interface UndeleteRequest extends ServiceRequest {
        EntityId?: any;
    }
    interface UndeleteResponse extends ServiceResponse {
    }
    interface ListRequest extends ServiceRequest {
        Skip?: number;
        Take?: number;
        Sort?: any;
        ContainsText?: string;
        ContainsField?: string;
        Criteria?: any[];
        EqualityFilter?: any;
        IncludeDeleted?: boolean;
        ExcludeTotalCount?: boolean;
        ColumnSelection?: ColumnSelection;
        IncludeColumns?: string[];
        ExcludeColumns?: string[];
    }
    interface ListResponse<TEntity> extends ServiceResponse {
        Entities?: TEntity[];
        TotalCount?: number;
        Skip?: number;
        Take?: number;
    }
    interface RetrieveRequest extends ServiceRequest {
        EntityId?: any;
        ColumnSelection: RetrieveColumnSelection;
        IncludeColumns: string[];
        ExcludeColumns: string[];
    }
    interface RetrieveResponse<TEntity> extends ServiceResponse {
        Entity: TEntity;
    }
    interface RetrieveLocalizationRequest extends RetrieveRequest {
    }
    interface RetrieveLocalizationResponse<TEntity> extends ServiceResponse {
        Entities?: {
            [key: string]: TEntity;
        };
    }
    class PropertyItemHelper {
        static getPropertyItemsFor(type: Function): PropertyItem[];
    }
    class PublicEditorTypes {
        static get_registeredTypes(): any;
    }
    interface QuickFilterArgs<TWidget> {
        field: string;
        widget: TWidget;
        request: ListRequest;
        equalityFilter: any;
        value: any;
        active: boolean;
        handled: boolean;
    }
    class QuickSearchInput extends Widget<QuickSearchInputOptions> {
        constructor(input: JQuery, opt: QuickSearchInputOptions);
        checkIfValueChanged(): void;
        get_field(): QuickSearchField;
        set_field(value: QuickSearchField): void;
    }
    interface QuickSearchInputOptions {
        typeDelay: number;
        loadingParentClass: string;
        filteredParentClass: string;
        onSearch: (p1: string, p2: string, p3: (p1: boolean) => void) => void;
        fields: QuickSearchField[];
    }
    class Recaptcha extends Widget<RecaptchaOptions> {
        constructor(div: JQuery, opt: RecaptchaOptions);
        get_value(): string;
        set_value(value: string): void;
    }
    interface RecaptchaOptions {
        siteKey: string;
        language: string;
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
        list = 2,
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
        maxLength?: any;
        required?: any;
        insertable?: any;
        hideOnInsert?: any;
        updatable?: any;
        hideOnUpdate?: any;
        readOnly?: any;
        oneWay?: any;
        defaultValue?: any;
        localizable?: any;
        visible?: any;
        formatterType?: string;
        formatterParams?: any;
        displayFormat?: string;
        alignment?: string;
        width?: number;
        minWidth?: any;
        maxWidth?: any;
        resizable?: any;
        sortOrder?: any;
        editLink?: any;
        editLinkItemType?: string;
        editLinkIdField?: string;
        editLinkCssClass?: string;
        filteringType?: string;
        filteringParams?: any;
        filteringIdField?: string;
        notFilterable?: any;
        filterOnly?: any;
    }
    class ISlickFormatter {
    }
    class ScriptContext {
    }
    class PrefixedContext extends ScriptContext {
        constructor(prefix: string);
        w(id: string, type: Function): any;
        get_idPrefix(): string;
    }
    class Widget<TOptions> {
        constructor(element: JQuery, options?: TOptions);
    }
    class StringEditor extends Widget<any> {
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
        get_value(): string;
        set_value(value: string): void;
        get_values(): any;
        set_values(value: any): void;
        get_text(): string;
    }
    class CheckTreeEditor<TItem, TOptions> {
    }
    interface EmailEditorOptions {
        domain?: string;
        readOnlyDomain: boolean;
    }
    class EmailEditor extends Widget<EmailEditorOptions> {
        constructor(input: JQuery, opt: EmailEditorOptions);
        static registerValidationMethods(): void;
        get_value(): string;
        set_value(value: string): void;
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
        get_value(): string;
        set_value(value: string): void;
        get_valueAsDate(): Date;
        set_valueAsDate(value: Date): void;
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
        get_value(): string;
        set_value(value: string): void;
        get_valueAsDate(): Date;
        set_valueAsDate(value: Date): void;
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
        startHour: any;
        endHour: any;
        intervalMinutes: any;
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
    class SelectEditorOptions {
        items: any[];
        emptyOptionText: string;
    }
    class DateYearEditor extends SelectEditor {
        constructor(hidden: JQuery, opt: DateYearEditorOptions);
    }
    class DateYearEditorOptions extends SelectEditorOptions {
        minYear: string;
        maxYear: string;
        descending: boolean;
    }
    class DecimalEditor extends Widget<DecimalEditorOptions> {
        constructor(input: JQuery, opt: DecimalEditorOptions);
        static defaultAutoNumericOptions(): any;
        get_value(): any;
        set_value(value: any): void;
        get_isValid(): boolean;
    }
    class DecimalEditorOptions {
        minValue: string;
        maxValue: string;
        decimals: any;
        padDecimals: any;
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
    class LookupEditorBase<TOptions, TItem> extends Widget<TOptions> {
    }
    class LookupEditor extends LookupEditorBase<LookupEditorOptions, any> {
    }
    class AsyncLookupEditor extends LookupEditorBase<LookupEditorOptions, any> {
        constructor(hidden: JQuery, opt: LookupEditorOptions);
    }
    interface UploadedFile {
        Filename: string;
        OriginalName: string;
    }
    interface ToolbarOptions {
        buttons: ToolButton[];
        hotkeyContext: any;
    }
    class Toolbar extends Widget<ToolbarOptions> {
        constructor(div: JQuery, options: ToolbarOptions);
        findButton(className: string): JQuery;
    }
    class ImageUploadEditor extends Widget<ImageUploadEditorOptions> {
        entity: UploadedFile;
        toolbar: Toolbar;
        fileSymbols: JQuery;
        uploadInput: JQuery;
        constructor(div: JQuery, opt: ImageUploadEditorOptions);
        addFileButtonText(): string;
        getToolButtons(): ToolButton[];
        populate(): void;
        updateInterface(): void;
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
        get_value(): UploadedFile;
        set_value(value: UploadedFile): void;
    }
    interface ImageUploadEditorOptions {
        minWidth: number;
        maxWidth: number;
        minHeight: number;
        maxHeight: number;
        minSize: number;
        maxSize: number;
        originalNameProperty: string;
        urlPrefix: string;
        allowNonImage: boolean;
    }
    class BooleanEditor extends Widget<any> {
        constructor(input: JQuery);
        get_value(): boolean;
        set_value(value: boolean): void;
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
    class CheckListEditorOptions {
        items: CheckListItem[];
        selectAllOptionText: string;
    }
    class CheckListItem {
        id: string;
        text: string;
        parentId: string;
    }
    interface CheckTreeItem<TSource> {
        isSelected: boolean;
        hideCheckBox: boolean;
        isAllDescendantsSelected: boolean;
        id: string;
        text: string;
        parentId: string;
        children: CheckTreeItem<TSource>[];
        source: TSource;
    }
    const enum ColumnSelection {
        List = 0,
        KeyOnly = 1,
        Details = 2,
    }
    class IntegerEditor extends Widget<IntegerEditorOptions> {
        constructor(input: JQuery, opt: IntegerEditorOptions);
        get_value(): any;
        set_value(value: any): void;
    }
    class IntegerEditorOptions {
        minValue: number;
        maxValue: number;
    }
    class IntegerFiltering extends BaseEditorFiltering<IntegerEditor> {
    }
    interface IReadOnly {
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
    }
    interface HtmlContentEditorOptions {
    }
    interface ISetEditValue {
        setEditValue(source: any, property: PropertyItem): void;
    }
    interface IStringValue {
        get_value(): string;
        set_value(value: string): void;
    }
    class TemplatedWidget<TOptions> extends Widget<TOptions> {
    }
    class TemplatedDialog<TOptions> extends TemplatedWidget<TOptions> {
    }
    class TemplatedPanel<TOptions> extends TemplatedWidget<TOptions> {
    }
    class PropertyDialog<TItem, TOptions> extends TemplatedDialog<TOptions> {
    }
    class PropertyPanel<TItem, TOptions> extends TemplatedPanel<TOptions> {
    }
    class EntityDialog<TItem, TOptions> extends TemplatedDialog<TOptions> {
        dialogOpen(): void;
        loadByIdAndOpenDialog(id: any): void;
    }
    class DataGrid<TItem, TOptions> {
        dialogOpen(): void;
        loadByIdAndOpenDialog(id: any): void;
    }
    class EntityGrid<TItem, TOptions> extends DataGrid<TItem, TOptions> {
        constructor(container: JQuery);
        dialogOpen(): void;
        loadByIdAndOpenDialog(id: any): void;
    }
    interface CKEditorConfig {
    }
    interface IDataGrid {
        getElement(): JQuery;
        getGrid(): Slick.Grid;
        getView(): Slick.RemoteView<any>;
        getFilterStore(): Serenity.FilterStore;
    }
    interface ToolButton {
    }
    enum CaptureOperationType {
        Before = 0,
        Delete = 1,
        Insert = 2,
        Update = 3,
    }
    namespace CustomValidation {
        function registerValidationMethods(): void;
    }
    interface ServiceOptions<TResponse extends ServiceResponse> extends JQueryAjaxSettings {
        request?: any;
        service?: string;
        blockUI?: boolean;
        onError?(response: TResponse): void;
        onSuccess?(response: TResponse): void;
        onCleanup?(): void;
    }
    interface IFrameDialogOptions {
        html?: string;
    }
    class BaseEditorFiltering<TEditor> extends BaseFiltering {
        editor: Serenity.Widget<any>;
        useEditor(): boolean;
        useIdField(): boolean;
        getEditorOptions(): any;
    }
    class FilterOperator {
        key: string;
        title: string;
        format: string;
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
        let toCriteriaOperator: {
            [key: string]: string;
        };
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
        type: Function;
        displayName: string;
        optionsType: Function;
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
        class ReportDialogOptions {
            reportKey: string;
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
            ReportKey: string;
            Properties: PropertyItem[];
            Title: string;
            InitialSettings: any;
            IsDataOnlyReport: boolean;
        }
    }
}
interface JQueryStatic {
    extend<T>(target: T, object1?: T, ...objectN: T[]): T;
    toJSON(obj: any): string;
}
interface JQBlockUIOptions {
    useTimeout?: boolean;
}
declare module RSVP {
    function on(handler: (e: any) => void): any;
    function resolve(): Thenable<any>;
}
interface Toastr {
    getContainer(options?: ToastrOptions, create?: boolean): JQuery;
}
declare namespace ss {
    interface AssemblyReg {
        name: string;
        __types: ClassReg[];
    }
    interface ClassReg {
        __class: boolean;
        __assembly: AssemblyReg;
        __interfaces: any[];
    }
    let __assemblies: {
        [name: string]: AssemblyReg;
    };
    class Exception {
        constructor(msg: string);
    }
    class NotSupportedException extends Exception {
        constructor(msg: string);
    }
    function arrayClone<T>(a: T[]): T[];
    function formatString(msg: string, ...prm: any[]): string;
    function getEnumerator(e: any): any;
    function getBaseType(e: any): any;
    function initAssembly(obj: any, name: string, res: {
        [name: string]: any;
    }): any;
    function padLeftString(s: string, m: number, n: number): any;
    function replaceAllString(s: string, f: string, r: string): string;
    function startsWithString(s: string, search: string): boolean;
}
declare namespace Q {
    import S = Serenity;
    function alert(message: string, options?: S.AlertOptions): void;
    function confirm(message: string, onYes: () => void, options?: S.ConfirmOptions): void;
    function information(message: string, onOk: () => void, options?: S.ConfirmOptions): void;
    function warning(message: string, options?: S.AlertOptions): void;
    function iframeDialog(options: S.IFrameDialogOptions): void;
    function toId(id: any): any;
    /**
     * Uses jQuery BlockUI plugin to block access to whole page (default) or
     * a part of it, by using a transparent overlay covering the whole area.
     * @param options Parameters for the BlockUI plugin
     * @remarks If options are not specified, this function blocks
     * whole page with a transparent overlay. Default z-order of the overlay
     * div is 2000, so a higher z-order shouldn't be used in page.
     */
    function blockUI(options: JQBlockUIOptions): void;
    function blockUndo(): void;
    function formatDate(date: Date, format?: string): string;
    /**
     * Html encodes a string
     * @param s String to be HTML encoded
     */
    function htmlEncode(s: any): string;
    function newBodyDiv(): JQuery;
    function addOption(select: JQuery, key: string, text: string): void;
    function clearOptions(select: JQuery): void;
    function addEmptyOption(select: JQuery): void;
    function trim(s: string): string;
    function isEmptyOrNull(s: string): boolean;
    function trimToNull(s: string): string;
    function isTrimmedEmpty(s: string): boolean;
    function trimToEmpty(s: string): string;
    function findElementWithRelativeId(element: JQuery, relativeId: string): JQuery;
    function outerHtml(element: JQuery): string;
    function layoutFillHeightValue(element: JQuery): number;
    function layoutFillHeight(element: JQuery): void;
    function initFullHeightGridPage(gridDiv: JQuery): void;
    function addFullHeightResizeHandler(handler: any): void;
    function triggerLayoutOnShow(element: any): void;
    function autoFullHeight(element: JQuery): void;
    function setMobileDeviceMode(): void;
    let defaultNotifyOptions: ToastrOptions;
    function positionToastContainer(create: boolean): void;
    function notifyWarning(message: string, title?: string, options?: ToastrOptions): void;
    function notifySuccess(message: string, title?: string, options?: ToastrOptions): void;
    function notifyInfo(message: string, title?: string, options?: ToastrOptions): void;
    function notifyError(message: string, title?: string, options?: ToastrOptions): void;
    function getRemoteData(key: any): any;
    function getRemoteDataAsync(key: any): RSVP.Thenable<any>;
    function getLookup(key: any): any;
    function getLookupAsync(key: any): RSVP.Thenable<any>;
    function reloadLookup(key: any): void;
    function reloadLookupAsync(key: any): RSVP.Thenable<any>;
    function getColumns(key: any): any;
    function getColumnsAsync(key: any): RSVP.Thenable<any>;
    function getForm(key: any): any;
    function getFormAsync(key: any): RSVP.Thenable<any>;
    function getTemplate(key: any): any;
    function getTemplateAsync(key: any): RSVP.Thenable<any>;
    function canLoadScriptData(name: any): boolean;
    function serviceCall<TResponse>(options: S.ServiceOptions<TResponse>): JQueryXHR;
    function serviceRequest<TResponse>(service: string, request?: any, onSuccess?: (response: TResponse) => void, options?: S.ServiceOptions<TResponse>): JQueryXHR;
    function setEquality(request: any, field: any, value: any): void;
    function toSingleLine(str: string): string;
    function text(key: string): string;
    function tryGetText(key: any): string;
    function resolveUrl(url: any): any;
    function zeroPad(n: number, digits: number): string;
    function formatDayHourAndMin(n: number): string;
    function formatISODateTimeUTC(d: Date): string;
    function formatNumber(n: number, fmt: string, dec?: string, grp?: string): string;
    function parseInteger(s: string): number;
    function parseDate(s: string, dateOrder?: string): any;
    function parseDecimal(s: string): number;
    function splitDateString(s: string): string[];
    function parseISODateTime(s: string): Date;
    function parseHourAndMin(value: string): any;
    function parseDayHourAndMin(s: string): number;
    function parseQueryString(s?: string): {};
    function turkishLocaleCompare(a: string, b: string): number;
    function turkishLocaleToUpper(a: string): string;
    function postToService(options: S.PostToServiceOptions): void;
    function postToUrl(options: S.PostToUrlOptions): void;
    function deepClone(arg1: any, arg2: any): any;
    namespace ErrorHandling {
        function showServiceError(error: any): void;
    }
    namespace Config {
        let applicationPath: string;
        let emailAllowOnlyAscii: boolean;
        let rootNamespaces: string[];
        let notLoggedInHandler: Function;
    }
    namespace Culture {
        let decimalSeparator: string;
        let dateSeparator: string;
        let dateOrder: string;
        let dateFormat: string;
        let dateTimeFormat: string;
        function get_groupSeperator(): string;
    }
    interface LookupOptions<TItem> {
        idField: string;
        parentIdField: string;
        textField: string;
        textFormatter(item: TItem): string;
    }
    class Lookup<TItem> {
        private options;
        private items;
        private itemById;
        constructor(options: LookupOptions<TItem>, items?: TItem[]);
        update(value: TItem[]): void;
        get_idField(): string;
        get_parentIdField(): string;
        get_textField(): string;
        get_textFormatter(): (item: TItem) => string;
        get_itemById(): {
            [key: string]: TItem;
        };
        get_items(): TItem[];
    }
    class LT {
        private key;
        static $table: {
            [key: string]: string;
        };
        static empty: LT;
        constructor(key: string);
        static add(obj: any, pre?: string): void;
        get(): string;
        toString(): string;
        static initializeTextClass: (type: any, prefix: any) => void;
        static getDefault: (key: any, defaultText: any) => string;
    }
    namespace ScriptData {
        function bindToChange(name: any, regClass: any, onChange: any): void;
        function triggerChange(name: any): void;
        function unbindFromChange(regClass: any): void;
        function ensure(name: string): any;
        function ensureAsync(name: string): RSVP.Thenable<any>;
        function reload(name: string): any;
        function reloadAsync(name: string): RSVP.Thenable<any>;
        function canLoad(name: string): boolean;
        function setRegisteredScripts(scripts: any): void;
        function set(name: string, value: any): void;
    }
}
declare namespace Serenity {
    class ColumnsKeyAttribute {
        value: string;
        constructor(value: string);
    }
    class DialogTypeAttribute {
        value: Function;
        constructor(value: Function);
    }
    class EditorAttribute {
        constructor();
        key: string;
    }
    class ElementAttribute {
        value: string;
        constructor(value: string);
    }
    class EntityTypeAttribute {
        value: string;
        constructor(value: string);
    }
    class EnumKeyAttribute {
        value: string;
        constructor(value: string);
    }
    class FlexifyAttribute {
        value: boolean;
        constructor(value?: boolean);
    }
    class FormKeyAttribute {
        value: string;
        constructor(value: string);
    }
    class IdPropertyAttribute {
        value: string;
        constructor(value: string);
    }
    class IsActivePropertyAttribute {
        value: string;
        constructor(value: string);
    }
    class ItemNameAttribute {
        value: string;
        constructor(value: string);
    }
    class LocalTextPrefixAttribute {
        value: string;
        constructor(value: string);
    }
    class MaximizableAttribute {
        value: boolean;
        constructor(value?: boolean);
    }
    class NamePropertyAttribute {
        value: string;
        constructor(value: string);
    }
    class OptionAttribute {
        constructor();
    }
    class OptionsTypeAttribute {
        value: Function;
        constructor(value: Function);
    }
    class PanelAttribute {
        value: boolean;
        constructor(value?: boolean);
    }
    class ResizableAttribute {
        value: boolean;
        constructor(value?: boolean);
    }
    class ResponsiveAttribute {
        value: boolean;
        constructor(value?: boolean);
    }
    class ServiceAttribute {
        value: string;
        constructor(value: string);
    }
    namespace Criteria {
        function isEmpty(c: any[]): boolean;
        function join(c1: any[], op: string, c2: any[]): any[];
        function paren(c: any[]): any[];
    }
    namespace Decorators {
        function addAttribute(type: any, attr: any): void;
        function columnsKey(value: string): (target: Function) => void;
        function dialogType(value: Function): (target: Function) => void;
        function editor(key?: string): (target: Function) => void;
        function element(value: string): (target: Function) => void;
        function entityType(value: string): (target: Function) => void;
        function enumKey(value: string): (target: Function) => void;
        function flexify(value?: boolean): (target: Function) => void;
        function formKey(value: string): (target: Function) => void;
        function idProperty(value: string): (target: Function) => void;
        function registerClass(intf?: any[], asm?: ss.AssemblyReg): (target: Function) => void;
        function registerFormatter(intf?: typeof ISlickFormatter[], asm?: ss.AssemblyReg): (target: Function) => void;
        function itemName(value: string): (target: Function) => void;
        function isActiveProperty(value: string): (target: Function) => void;
        function localTextPrefix(value: string): (target: Function) => void;
        function maximizable(value?: boolean): (target: Function) => void;
        function nameProperty(value: string): (target: Function) => void;
        function option(value: Function): (target: Function) => void;
        function optionsType(value: Function): (target: Function) => void;
        function panel(value?: boolean): (target: Function) => void;
        function resizable(value?: boolean): (target: Function) => void;
        function responsive(value?: boolean): (target: Function) => void;
        function service(value: string): (target: Function) => void;
    }
    namespace TabsExtensions {
        function setDisabled(tabs: JQuery, tabKey: string, isDisabled: boolean): void;
        function activeTabKey(tabs: JQuery): string;
        function indexByKey(tabs: JQuery): any;
    }
    namespace LazyLoadHelper {
        function executeOnceWhenShown(element: JQuery, callback: Function): void;
        function executeEverytimeWhenShown(element: JQuery, callback: Function, callNowIfVisible: boolean): void;
    }
}
