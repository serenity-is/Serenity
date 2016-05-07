declare interface JQuery {
    getWidget<TWidget>(widgetType: { new (...args: any[]): TWidget }): TWidget;
    tryGetWidget<TWidget>(widgetType: { new (...args: any[]): TWidget }): TWidget;
    flexHeightOnly(flexY?: number): JQuery;
    flexWidthOnly(flexX?: number): JQuery;
    flexWidthHeight(flexX: number, flexY: number): JQuery;
    flexX(flexX: number): JQuery;
    flexY(flexY: number): JQuery;
}

declare class RSVP<TResult> {
    constructor(constructor: (p1: (p1: any) => void, p2: any) => void);
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

declare namespace Select2 {
    namespace util {
        function stripDiacritics(input: string): string;
    }
}

declare namespace Serenity {

    namespace SlickTreeHelper {
        function filterCustom<TItem>(item: TItem, getParent: (x: TItem) => any): boolean;
        function filterById<TItem>(item: TItem, view: Slick.RemoteView<TItem>, getParentId: (x: TItem) => any): boolean;
        function setCollapsed<TItem>(items: TItem[], collapsed: boolean): void;
        function setCollapsedFlag<TItem>(item: TItem, collapsed: boolean): void;
        function setIndents<TItem>(items: TItem[], getId: (x: TItem) => any, getParentId: (x: TItem) => any, setCollapsed?: boolean): void;
        function toggleClick<TItem>(e: JQueryEventObject, row: number, cell: number, view: Slick.RemoteView<TItem>, getId: (x: TItem) => any): void;
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
        constructor(input: JQuery, opt?: TextAreaEditorOptions);
        get_value(): string;
        set_value(value: string): void;
    }

    interface TextAreaEditorOptions {
        cols?: number;
        rows?: number;
    }

    class TimeEditor extends Widget<TimeEditorOptions> {
        constructor(input: JQuery, opt?: TimeEditorOptions);
        get_value(): any;
        set_value(value: any): void;
    }

    interface TimeEditorOptions {
        noEmptyOption?: boolean;
        startHour?: any;
        endHour?: any;
        intervalMinutes?: any;
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
        icon?: string;
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
        container?: JQuery;
        progress?: JQuery;
        inputName?: string;
        allowMultiple?: boolean;
        fileDone?: (p1: UploadResponse, p2: string, p3: any) => void;
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
        get_displayFormat(): string;
        set_displayFormat(value: string): void;
        get_urlProperty(): string;
        set_urlProperty(value: string): void;
        get_urlFormat(): string;
        set_urlFormat(value: string): void;
        get_target(): string;
        set_target(value: string): void;
    }

    namespace ValidationHelper {
        function asyncSubmit(form: JQuery, validateBeforeSave: () => boolean, submitHandler: () => void): boolean;
        function submit(form: JQuery, validateBeforeSave: () => boolean, submitHandler: () => void): boolean;
        function getValidator(element: JQuery): JQueryValidation.Validator;
    }

    namespace VX {
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
        class CategoryAttribute  {
            constructor(category: string);
            category: string;
        }

        class CssClassAttribute  {
            constructor(cssClass: string);
            cssClass: string;
        }

        class DefaultValueAttribute  {
            constructor(defaultValue: any);
            value: any;
        }

        class EditorOptionAttribute  {
            constructor(key: string, value: any);
            key: string;
            value: any;
        }

        class EditorTypeAttribute extends EditorTypeAttributeBase {
            constructor(editorType: string);
        }

        class EditorTypeAttributeBase  {
            constructor(type: string);
            setParams(editorParams: any): void;
            editorType: string;
        }

        class HiddenAttribute  {
        }

        class HintAttribute  {
            constructor(hint: string);
            hint: string;
        }

        class InsertableAttribute  {
            constructor(insertable?: boolean);
            value: boolean;
        }

        class MaxLengthAttribute  {
            constructor(maxLength: number);
            maxLength: number;
        }

        class OneWayAttribute  {
        }

        class PlaceholderAttribute  {
            constructor(value: string);
            value: string;
        }

        class ReadOnlyAttribute  {
            constructor(readOnly?: boolean);
            value: boolean;
        }

        class RequiredAttribute  {
            constructor(isRequired: boolean);
            isRequired: boolean;
        }

        class UpdatableAttribute  {
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
        get_mode(): PropertyGridMode;
        set_mode(value: PropertyGridMode): void;
    }

    const enum PropertyGridMode {
        insert = 1,
        update = 2
    }

    interface PropertyGridOptions {
        idPrefix?: string;
        items?: PropertyItem[];
        useCategories?: boolean;
        categoryOrder?: string;
        defaultCategory?: string;
        localTextPrefix?: string;
        mode?: PropertyGridMode;
    }

    namespace SlickFormatting {
        function getEnumText(value: any): string;
        function getEnumText(enumKey: string, name: string): string;
        function treeToggle<TItem>(getView: () => Slick.RemoteView<TItem>, getId: (x: TItem) => any,
            formatter: Slick.Format): Slick.Format;
        function date(format?: string): Slick.Format;
        function dateTime(format?: string): Slick.Format;
        function checkBox(): Slick.Format;
        function number(format: string): Slick.Format;
        function getItemType(link: JQuery): string;
        function getItemId(link: JQuery): string;
        function itemLinkText(itemType: string, id: any, text: any, extraClass: string, encode: boolean): string;
        function itemLink(itemType: string, idField: string, getText: Slick.Format,
            cssClass?: Slick.Format, encode?: boolean): Slick.Format;
    }

    namespace SlickHelper {
        function setDefaults(columns: any, localTextPrefix?: string): any;
        function convertToFormatter(format: Slick.Format): Slick.ColumnFormatter;
    }

    class PhoneEditor extends Widget<PhoneEditorOptions> {
        constructor(input: JQuery, opt?: PhoneEditorOptions);
        validate(value: string): string;
        formatValue(): void;
        getFormattedValue(): string;
        get_value(): string;
        set_value(value: string): void;
    }

    interface PhoneEditorOptions {
        multiple?: boolean;
        internal?: boolean;
        mobile?: boolean;
        allowExtension?: boolean;
        allowInternational?: boolean;
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

    interface HtmlContentEditorOptions {
        cols?: any;
        rows?: any;
    }

    class HtmlReportContentEditor extends HtmlContentEditor {
        constructor(textArea: JQuery, opt: HtmlContentEditorOptions);
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

    class IGetEditValue {
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
        transport?: any;
        url?: any;
        dataType?: string;
        quietMillis?: number;
        cache?: boolean;
        jsonpCallback?: any;
        data?: (p1: string, p2: number, p3: any) => any;
        results?: (p1: any, p2: number, p3: any) => any;
        params?: any;
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
        element?: JQuery;
        term?: string;
        page?: number;
        context?: any;
        callback?: (p1: Select2Result) => void;
    }

    interface Select2Result {
        results: any;
        more: boolean;
        context: any;
    }

    interface Select2Options {
        width?: any;
        minimumInputLength?: number;
        maximumInputLength?: number;
        minimumResultsForSearch?: number;
        maximumSelectionSize?: any;
        placeHolder?: string;
        placeHolderOption?: any;
        separator?: string;
        allowClear?: boolean;
        multiple?: boolean;
        closeOnSelect?: boolean;
        openOnEnter?: boolean;
        id?: (p1: any) => string;
        matcher?: (p1: string, p2: string, p3: JQuery) => boolean;
        sortResults?: (p1: any, p2: JQuery, p3: any) => any;
        formatSelection?: (p1: any, p2: JQuery, p3: (p1: string) => string) => string;
        formatResult?: (p1: any, p2: JQuery, p3: any, p4: (p1: string) => string) => string;
        formatResultCssClass?: (p1: any) => string;
        formatNoMatches?: (p1: string) => string;
        formatSearching?: () => string;
        formatInputTooShort?: (p1: string, p2: number) => string;
        formatSelectionTooBig?: (p1: string) => string;
        createSearchChoice?: (p1: string) => any;
        createSearchChoicePosition?: string;
        initSelection?: (p1: JQuery, p2: (p1: any) => void) => void;
        tokenizer?: (p1: string, p2: any, p3: (p1: any) => any, p4: any) => string;
        tokenSeparators?: any;
        query?: (p1: Select2QueryOptions) => void;
        ajax?: Select2AjaxOptions;
        data?: any;
        tags?: any;
        containerCss?: any;
        containerCssClass?: any;
        dropdownCss?: any;
        dropdownCssClass?: any;
        dropdownAutoWidth?: boolean;
        adaptContainerCssClass?: (p1: string) => string;
        adaptDropdownCssClass?: (p1: string) => string;
        escapeMarkup?: (p1: string) => string;
        selectOnBlur?: boolean;
        loadMorePadding?: number;
        nextSearchTerm?: (p1: any, p2: string) => string;
    }


    interface EnumEditorOptions {
        enumKey?: string;
        enumType?: any;
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
        latitude?: any;
        longitude?: any;
        zoom?: any;
        mapTypeId?: any;
        markerTitle?: string;
        markerLatitude?: any;
        markerLongitude?: any;
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
        Localizations?: { [key: string]: TEntity };
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
        Entities?: { [key: string]: TEntity };
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
        typeDelay?: number;
        loadingParentClass?: string;
        filteredParentClass?: string;
        onSearch?: (p1: string, p2: string, p3: (p1: boolean) => void) => void;
        fields?: QuickSearchField[];
    }

    class Recaptcha extends Widget<RecaptchaOptions> {
        constructor(div: JQuery, opt: RecaptchaOptions);
        get_value(): string;
        set_value(value: string): void;
    }

    interface RecaptchaOptions {
        siteKey?: string;
        language?: string;
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

    class Widget<TOptions> {
        constructor(element: JQuery, options?: TOptions);
        protected destroy();
        protected addCssClass();
        protected getCssClass(): string;
        protected initializeAsync(): PromiseLike<void>;
        protected asyncPromise: PromiseLike<void>;
        widgetName: string;
        uniqueName: string;
        element: JQuery;
        protected options: TOptions;
        addValidationRule(eventClass: string, rule: (p1: JQuery) => string): JQuery;
        getGridField(): JQuery;
        change(handler: (e: JQueryEventObject) => void);
        changeSelect2(handler: (e: JQueryEventObject) => void);
        initialize(): PromiseLike<void>;
        isAsyncWidget(): boolean;

        static create<TWidget>(type: { new (...args: any[]): TWidget }):
            (element?: (e: JQuery) => void, options?: any, init?: (w: TWidget) => void) => TWidget;
        static createInside<TWidget>(type: { new (...args: any[]): TWidget }):
            (container: JQuery, options?: any, init?: (w: TWidget) => void) => TWidget;
        static createOfType<TWidget>(type: { new (...args: any[]): TWidget },
            element?: (e: JQuery) => void, options?: any, init?: (w: TWidget) => void): TWidget;
        static elementFor<TEditor>(type: TEditor): () => JQuery;
        static elementFor$1<TEditor>(editorType: { new (...args: any[]): any }): JQuery;
    }

    class IStringValue {
    }

    interface IStringValue {
        get_value(): string;
        set_value(value: string): void;
    }

    class StringEditor extends Widget<any> implements IStringValue {
        get_value(): string;
        set_value(value: string): void;
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
        public get_value(): string[];
        public set_value(value: string[]);
    }

    interface EmailEditorOptions {
        domain?: string;
        readOnlyDomain?: boolean;
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
        get_value(): any;
        set_value(value: any): void;
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
        protected initNewEntity(entity: TItem);
        protected updateItems(): void;
        protected getDialogTypeKey(): string;
        protected createEditDialog(callback: (dlg: Serenity.EditDialog) => void): void;
        onInitNewEntity: (entity: TItem) => void;
        get_value(): string;
        set_value(value: string): void;
        get_cascadeField(): string;
        set_cascadeField(name: string): void;
        get_cascadeValue(): any;
        set_cascadeValue(value: any): void;
        get_filterField(): string;
        set_filterField(name: string): void;
        get_filterValue(): any;
        set_filterValue(value: any): void;
    }

    class LookupEditor extends LookupEditorBase<LookupEditorOptions, any> {
        constructor(input: JQuery, opt?: LookupEditorOptions);
    }

    class AsyncLookupEditor extends LookupEditorBase<LookupEditorOptions, any> {
        constructor(hidden: JQuery, opt: LookupEditorOptions);
    }

    interface UploadedFile {
        Filename: string;
        OriginalName: string;
    }


    interface ToolbarOptions {
        buttons?: ToolButton[];
        hotkeyContext?: any;
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
        minWidth?: number;
        maxWidth?: number;
        minHeight?: number;
        maxHeight?: number;
        minSize?: number;
        maxSize?: number;
        originalNameProperty?: string;
        urlPrefix?: string;
        allowNonImage?: boolean;
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
        get_value(): any;
        set_value(value: any): void;
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

    class TemplatedWidget<TOptions> extends Widget<TOptions> {
        protected idPrefix: string;
        protected byId(id: string): JQuery;
        protected byID<TWidget>(type: TWidget): (id: string) => TWidget;
        protected getTemplate(): string;
        protected getTemplateName(): string;
    }

    class TemplatedDialog<TOptions> extends TemplatedWidget<TOptions> {
        constructor(options?: TOptions);
        protected arrange(): void;
        public dialogClose(): void;
        public dialogOpen(): void;
        protected getDialogOptions(): JQueryUI.DialogOptions;
        protected getToolbarButtons(): ToolButton[];
        protected getValidatorOptions(): JQueryValidation.ValidationOptions;
        protected handleResponsive();
        protected initDialog();
        protected initTabs();
        protected initToolbar();
        protected initValidator();
        protected onDialogClose();
        protected onDialogOpen();
        protected resetValidation();
        protected validateForm(): boolean;
        public get_dialogTitle(): string;
        public set_dialogTitle(value: string): void;
    }

    class TemplatedPanel<TOptions> extends TemplatedWidget<TOptions> {
        protected arrange(): void;
        protected getToolbarButtons(): ToolButton[];
        protected getValidatorOptions(): JQueryValidation.ValidationOptions;
        protected initTabs();
        protected initToolbar();
        protected initValidator();
        protected resetValidation();
        protected validateForm(): boolean;
    }

    class PropertyDialog<TItem, TOptions> extends TemplatedDialog<TOptions> {
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
        protected set_entity(entity: TItem);
        protected set_entityId(value: any);
        protected validateBeforeSave(): boolean;
    }

    class PropertyPanel<TItem, TOptions> extends TemplatedPanel<TOptions> {
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
        protected set_entity(entity: TItem);
        protected set_entityId(value: any);
        protected validateBeforeSave(): boolean;
    }

    class EntityDialog<TItem, TOptions> extends TemplatedDialog<TOptions> {
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
        protected deleteHandler(options: ServiceOptions<DeleteResponse>, callback: (response: DeleteResponse) => void);
        protected doDelete(callback: (response: DeleteResponse) => void);
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
        public load(entityOrId, done: () => void, fail: () => void): void;
        public loadById(id: any): void;
        public loadByIdAndOpenDialog(id: any): void;
        protected loadByIdHandler(options: ServiceOptions<RetrieveResponse<TItem>>, callback: (response: RetrieveResponse<TItem>) => void, fail: () => void);
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
        protected undeleteHandler(options: ServiceOptions<UndeleteResponse>, callback: (response: UndeleteResponse) => void);
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

    class DataGrid<TItem, TOptions> extends Widget<TOptions> {
        constructor(container: JQuery, options?: TOptions);
        dialogOpen(): void;
        loadByIdAndOpenDialog(id: any): void;
        protected titleDiv: JQuery;
        protected filterBar: FilterDisplayBar;
        protected quickFiltersDiv: JQuery;
        protected slickContainer: JQuery;
        protected toolbar: Toolbar;
        protected addDateRangeFilter(field: string, title?: string);
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
        protected determineText(text: string, getKey: (s: string) => string);
        protected editItem(entityOrId: any);
        protected editItemOfType(itemType: string, entityOrId: any);
        protected enableFiltering(): boolean;
        protected findQuickFilter<TWidget>(type: { new (...args: any[]): TWidget }, field: string): TWidget
        protected getAddButtonCaption(): string;
        protected getButtons(): ToolButton[];
        protected getColumns(): Slick.Column[];
        protected getColumnsAsync(): PromiseLike<Slick.Column[]>;
        protected getColumnsKey(): string;
        protected getDefaultSortBy(): string[];
        protected getGridCanLoad(): boolean;
        protected getIdProperty(): string;
        protected getIncludeColumns(include: { [key: string]: boolean });
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
        protected initialPopulate();
        protected initializeAsync(): PromiseLike<void>;
        protected internalRefresh();
        protected invokeSubmitHandlers();
        protected itemLink(itemType?: string, idField?: string, text?: (ctx: Slick.FormatterContext) => string,
            cssClass?: (ctx: Slick.FormatterContext) => string, encode?: boolean);
        protected layout(): void;
        protected markupReady(): void;
        protected onClick(e: JQueryEventObject, row: number, cell: number);
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
        protected setIncludeColumnsParameter();
        protected setInitialSortOrder();
        protected subDialogDataChange();
        protected updateDisabledState();
        protected usePager(): boolean;
        public refresh(): void;
        public getItems(): TItem[];
        public setItems(value: TItem[]);
        public isDisabled: boolean;
        public setIsDisabled(value: boolean);
        public getTitle(): string;
        public setTitle(value: string);
        public itemAt(row: number): TItem;
        public rowCount(): number;
        public view: Slick.RemoteView<TItem>;
        public slickGrid: Slick.Grid;
        public getElement(): JQuery;
        public getFilterStore(): FilterStore;
        public getGrid(): Slick.Grid;
        public getView(): Slick.RemoteView<TItem>;
        static defaultHeaderHeight: number;
        static defaultRowHeight: number;
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
        protected initDialog(dialog: Widget<any>);
        protected initEntityDialog(itemType: string, dialog: Widget<any>);
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

interface JQueryStatic {
    extend<T>(target: T, object1?: T, ...objectN: T[]): T;
    toJSON(obj: any): string;
}

interface JQBlockUIOptions {
    useTimeout?: boolean;
}

declare module RSVP {
    function on(handler: (e: any) => void);
    function resolve(): Thenable<any>;
}

interface Toastr {
    getContainer(options?: ToastrOptions, create?: boolean): JQuery;
}

// this class will go obsolete once all code is ported to TypeScript
// prefer alternative methods under Q
declare namespace ss {
    interface AssemblyReg {
        name: string;
        __types: ClassReg[];
    }

    interface ClassReg {
        __register: boolean;
        __class: boolean;
        __assembly: AssemblyReg;
        __interfaces: any[];
    }

    let __assemblies: { [name: string]: AssemblyReg; };

    class Exception {
        constructor(msg: string);
    }

    class NotSupportedException extends Exception {
        constructor(msg: string);
    }

    function arrayClone<T>(a: T[]): T[];
    function cast<T>(a: any, type: { new (...args: any[]): T }): T;
    function coalesce(a: any, b: any): any;
    function insert(obj: any, index: number, item: any): void;
    function isArray(a: any): boolean;
    function isValue(a: any): boolean;
    function formatString(msg: string, ...prm: any[]): string;
    function getEnumerator(e: any): any;
    function getBaseType(e: any): any;
    function initAssembly(obj, name: string, res: { [name: string]: any });
    function padLeftString(s: string, m: number, n: number);
    function replaceAllString(s: string, f: string, r: string): string;
    function endsWithString(s: string, search: string): boolean;
    function startsWithString(s: string, search: string): boolean;
}

namespace Q {
    import S = Serenity;

    export function arrayClone<T>(a: T[]): T[] {
        return ss.arrayClone(a);
    }

    export function isArray(a: any): boolean {
        return ss.isArray(a);
    }

    export function insert(obj: any, index: number, item: any): void {
        ss.insert(obj, index, item);
    }

    export function coalesce(a: any, b: any): any {
        return ss.coalesce(a, b);
    }

    export function isValue(a: any): boolean {
        return ss.isValue(a);
    }

    export function endsWith(s: string, search: string): boolean {
        return ss.endsWithString(s, search);
    }

    export function format(msg: string, ...prm: any[]): string {
        return ss.formatString(msg, ...prm);
    }

    export function padLeft(s: string, len: number, ch: string = ' ') {
        while (s.length < len)
            s = "0" + s;
        return s;
    }

    export function zeroPad(n: number, digits: number): string {
        let s = n.toString();
        while (s.length < digits)
            s = "0" + s;
        return s;
    }

    export function replaceAll(s: string, f: string, r: string): string {
        return ss.replaceAllString(s, f, r);
    }

    export function startsWith(s: string, search: string): boolean {
        return ss.startsWithString(s, search);
    }

    export function alert(message: string, options?: S.AlertOptions) {
        let dialog;
        options = <S.AlertOptions>$.extend({
            htmlEncode: true,
            okButton: text('Dialogs.OkButton'),
            title: text('Dialogs.AlertTitle'),
            onClose: null,
            onOpen: null,
            autoOpen: false,
            dialogClass: 's-MessageDialog s-AlertDialog',
            modal: true,
            width: '40%',
            maxWidth: '450',
            minWidth: '180',
            resizable: false,
            open: function () {
                if (options.onOpen)
                    options.onOpen.call(this);
            },
            close: function () {
                dialog.dialog('destroy');
                if (options.onClose)
                    options.onClose();
            }
        }, options);

        if (options.htmlEncode)
            message = htmlEncode(message);
        if (!options.buttons) {
            let buttons = [];
            buttons.push({
                text: options.okButton,
                click: function () {
                    dialog.dialog('close');
                }
            });
            options.buttons = buttons;
        }
        dialog = $('<div><div class="message"><\/div><\/div>')
            .dialog(options)
            .children('.message')
            .html(message)
            .parent()
            .dialog('open');
    }

    export function confirm(message: string, onYes: () => void, options?: S.ConfirmOptions): void {
        let dialog;
        options = $.extend({
            htmlEncode: true,
            yesButton: text('Dialogs.YesButton'),
            noButton: text('Dialogs.NoButton'),
            title: text('Dialogs.ConfirmationTitle'),
            onNo: null,
            onCancel: null,
            onClose: null,
            autoOpen: false,
            modal: true,
            dialogClass: 's-MessageDialog s-ConfirmDialog',
            width: '40%',
            maxWidth: '450',
            minWidth: '180',
            resizable: false,
            open: function () {
                if (options.onOpen)
                    options.onOpen.call(this);
            },
            close: function () {
                dialog.dialog('destroy');
                if (!clicked && options.onCancel)
                    options.onCancel();
            },
            overlay: {
                opacity: 0.77,
                background: "black"
            }
        }, options);
        if (options.htmlEncode)
            message = htmlEncode(message);
        let clicked = false;
        if (!options.buttons) {
            let buttons = [];
            buttons.push({
                text: options.yesButton,
                click: function () {
                    clicked = true;
                    dialog.dialog('close');
                    if (onYes)
                        onYes();
                }
            });
            if (options.noButton)
                buttons.push({
                    text: options.noButton,
                    click: function () {
                        clicked = true;
                        dialog.dialog('close');
                        if (options.onNo)
                            options.onNo();
                        else if (options.onCancel)
                            options.onCancel();
                    }
                });
            options.buttons = buttons;
        }
        dialog = $('<div><div class="message"><\/div><\/div>')
            .dialog(options)
            .children('.message')
            .html(message)
            .parent()
            .dialog('open');
    }

    export function information(message: string, onOk: () => void, options?: S.ConfirmOptions) {
        confirm(message, onOk, $.extend<S.ConfirmOptions>(
            {
                title: text("Dialogs.InformationTitle"),
                dialogClass: "s-MessageDialog s-InformationDialog",
                yesButton: text("Dialogs.OkButton"),
                noButton: null,
            }, options));
    }

    export function warning(message: string, options?: S.AlertOptions) {
        alert(message, $.extend<S.AlertOptions>(
            {
                title: text("Dialogs.WarningTitle"),
                dialogClass: "s-MessageDialog s-WarningDialog"
            }, options));
    }

    export function iframeDialog(options: S.IFrameDialogOptions) {
        let doc;
        let e = $('<div><iframe></iframe></div>');
        let settings: S.IFrameDialogOptions = $.extend(<JQueryUI.DialogOptions>{
            autoOpen: true,
            modal: true,
            width: '60%',
            height: '400',
            title: text('Dialogs.AlertTitle'),
            open: function () {
                doc = (<HTMLIFrameElement>(e.find('iframe').css({
                    border: 'none',
                    width: '100%',
                    height: '100%'
                })[0])).contentDocument;
                doc.open();
                doc.write(settings.html);
                doc.close();
            },
            close: function () {
                doc.open();
                doc.write('');
                doc.close();
                e.dialog('destroy').html('');
            }
        }, options);
        e.dialog(settings);
    }

    export function toId(id: any): any {
        if (id == null)
            return null;
        if (typeof id == "number")
            return id;
        id = $.trim(id);
        if (id == null || !id.length)
            return null;
        if (id.length >= 15 || !(/^\d+$/.test(id)))
            return id;
        let v = parseInt(id, 10);
        if (isNaN(v))
            return id;
        return v;
    }

    function log(m: any) {
        (<any>window).console && (<any>window).console.log(m);
    }

    let blockUICount: number = 0;

    function blockUIWithCheck(opt: JQBlockUIOptions) {
        if (blockUICount > 0) {
            blockUICount++;
            return;
        }

        $.blockUI(opt);
        blockUICount++;
    }

    /**
     * Uses jQuery BlockUI plugin to block access to whole page (default) or 
     * a part of it, by using a transparent overlay covering the whole area.
     * @param options Parameters for the BlockUI plugin
     * @remarks If options are not specified, this function blocks 
     * whole page with a transparent overlay. Default z-order of the overlay
     * div is 2000, so a higher z-order shouldn't be used in page.
     */
    export function blockUI(options: JQBlockUIOptions) {
        options = $.extend<JQBlockUIOptions>({
            baseZ: 2000,
            message: '',
            overlayCSS: {
                opacity: '0.0',
                zIndex: 2000,
                cursor: 'wait'
            }, fadeOut: 0
        }, options);

        if (options.useTimeout) {
            window.setTimeout(function () {
                blockUIWithCheck(options);
            }, 0);
        }
        else {
            blockUIWithCheck(options);
        }
    }

    export function blockUndo() {
        if (blockUICount > 1) {
            blockUICount--;
            return;
        }

        blockUICount--;
        $.unblockUI({ fadeOut: 0 });
    }

    export type Dictionary<TItem> = { [key: string]: TItem };
    export type Grouping<TItem> = { [key: string]: TItem[] };

    export function toGrouping<TItem>(items: TItem[], getKey: (x: TItem) => any): Q.Grouping<TItem> {
        let lookup: Grouping<TItem> = {};
        for (let x of items) {
            let key = getKey(x) || "";
            let d = lookup[key];
            if (!d) {
                d = lookup[key] = [];
            }

            d.push(x);
        }
        return lookup;
    }

    export function first<TItem>(array: TItem[], predicate: (x: TItem) => boolean): TItem {
        for (let x of array)
            if (predicate(x))
                return x;

        throw new Error("first:No element satisfies the condition.!");
    }

    export function tryFirst<TItem>(array: TItem[], predicate: (x: TItem) => boolean): TItem {
        for (let x of array)
            if (predicate(x))
                return x;
    }

    export function single<TItem>(array: TItem[], predicate: (x: TItem) => boolean): TItem {
        let match;
        let found = false;
        for (let x of array)
            if (predicate(x)) {
                if (found)
                    throw new Error("single:sequence contains more than one element.");
                found = true;
                match = x;
            }

        if (!found)
            throw new Error("single:No element satisfies the condition.");

        return match;
    }

    export function any<TItem>(array: TItem[], predicate: (x: TItem) => boolean): boolean {
        for (let x of array)
            if (predicate(x))
                return true;

        return false;
    }

    export function count<TItem>(array: TItem[], predicate: (x: TItem) => boolean): number {
        let count = 0;
        for (let x of array)
            if (predicate(x))
                count++;

        return count;
    }

    export function indexOf<TItem>(array: TItem[], predicate: (x: TItem) => boolean): number {
        for (var i = 0; i < array.length; i++)
            if (predicate(array[i]))
                return i;

        return -1;
    }

    export function formatDate(date: Date, format?: string) {
        if (!date) {
            return '';
        }

        if (format == null) {
            format = Culture.dateFormat;
        }

        let pad = function (i) {
            return Q.zeroPad(i, 2);
        };

        return format.replace(new RegExp('dd?|MM?|yy?y?y?|hh?|HH?|mm?|ss?|tt?|fff|zz?z?|\\/', 'g'),
            function (fmt): any {
                switch (fmt) {
                    case '/': return Culture.dateSeparator;
                    case 'hh': return pad(((date.getHours() < 13) ? date.getHours() : (date.getHours() - 12)));
                    case 'h': return ((date.getHours() < 13) ? date.getHours() : (date.getHours() - 12));
                    case 'HH': return pad(date.getHours());
                    case 'H': return date.getHours();
                    case 'mm': return pad(date.getMinutes());
                    case 'm': return date.getMinutes();
                    case 'ss': return pad(date.getSeconds());
                    case 's': return date.getSeconds();
                    case 'yyyy': return date.getFullYear();
                    case 'yy': return date.getFullYear().toString().substr(2, 4);
                    case 'dd': return pad(date.getDate());
                    case 'd': return date.getDate().toString();
                    case 'MM': return pad(date.getMonth() + 1);
                    case 'M': return date.getMonth() + 1;
                    case 't': return ((date.getHours() < 12) ? 'A' : 'P');
                    case 'tt': return ((date.getHours() < 12) ? 'AM' : 'PM');
                    case 'fff': return Q.zeroPad(date.getMilliseconds(), 3);
                    case 'zzz':
                    case 'zz':
                    case 'z': return '';
                    default: return fmt;
                }
            }
        );
    }

    function htmlEncoder(a: string): string {
        switch (a) {
            case '&': return '&amp;';
            case '>': return '&gt;';
            case '<': return '&lt;';
        }
        return a;
    }

    /**
     * Html encodes a string
     * @param s String to be HTML encoded
     */
    export function htmlEncode(s: any): string {
        let text = (s == null ? '' : s.toString());
        if ((new RegExp('[><&]', 'g')).test(text)) {
            return text.replace(new RegExp('[><&]', 'g'), htmlEncoder);
        }
        return text;
    }

    export function newBodyDiv(): JQuery {
        return $('<div/>').appendTo(document.body);
    }

    export function addOption(select: JQuery, key: string, text: string) {
        $('<option/>').val(key).text(text).appendTo(select);
    }

    export function clearOptions(select: JQuery) {
        select.html('');
    }

    export function addEmptyOption(select: JQuery) {
        addOption(select, '', text("Controls.SelectEditor.EmptyItemText"));
    }

    export function trim(s: string) {
        return (s == null ? '' : s).replace(new RegExp('^\\s+|\\s+$', 'g'), '');
    }

    export function isEmptyOrNull(s: string) {
        return s == null || s.length === 0;
    }

    export function trimToNull(s: string) {
        if (s == null || s.length === 0) {
            return null;
        }

        s = trim(s);
        if (s.length === 0) {
            return null;
        }
        else {
            return s;
        }
    }

    export function isTrimmedEmpty(s: string) {
        return trimToNull(s) == null;
    }

    export function trimToEmpty(s: string) {
        if (s == null || s.length === 0) {
            return '';
        }

        return trim(s);
    }

    export function findElementWithRelativeId(element: JQuery, relativeId: string) {
        let elementId = element.attr('id');
        if (isEmptyOrNull(elementId)) {
            return $('#' + relativeId);
        }

        let result = $(elementId + relativeId);
        if (result.length > 0) {
            return result;
        }

        result = $(elementId + '_' + relativeId);

        if (result.length > 0) {
            return result;
        }

        while (true) {
            let idx = elementId.lastIndexOf('_');
            if (idx <= 0) {
                return $('#' + relativeId);
            }

            elementId = elementId.substr(0, idx);
            result = $('#' + elementId + '_' + relativeId);

            if (result.length > 0) {
                return result;
            }
        }
    }

    export function outerHtml(element: JQuery) {
        return $('<i/>').append(element.eq(0).clone()).html();
    }

    export function layoutFillHeightValue(element: JQuery) {
        let h = 0;
        element.parent().children().not(element).each(function (i, e) {
            let q = $(e);
            if (q.is(':visible')) {
                h += q.outerHeight(true);
            }
        });
        h = element.parent().height() - h;
        if (element.css('box-sizing') !== 'border-box') {
            h = h - (element.outerHeight(true) - element.height());
        }
        return h;
    }

    export function layoutFillHeight(element: JQuery) {
        let h = layoutFillHeightValue(element);
        let n = h + 'px';
        if (element.css('height') != n) {
            element.css('height', n);
        }
    }

    export function initFullHeightGridPage(gridDiv: JQuery) {
        $('body').addClass('full-height-page');
        gridDiv.addClass('responsive-height');

        let layout = function () {
            let inPageContent = gridDiv.parent().hasClass('page-content') ||
                gridDiv.parent().is('section.content');

            if (inPageContent) {
                gridDiv.css('height', '1px').css('overflow', 'hidden');
            }

            layoutFillHeight(gridDiv);

            if (inPageContent) {
                gridDiv.css('overflow', '');
            }

            gridDiv.triggerHandler('layout');
        };

        if ($('body').hasClass('has-layout-event')) {
            $('body').bind('layout', layout);
        }
        else if ((window as any).Metronic) {
            (window as any).Metronic.addResizeHandler(layout);
        }
        else {
            $(window).resize(layout);
        }

        layout();
    }

    export function addFullHeightResizeHandler(handler) {
        $('body').addClass('full-height-page');
        let layout = function () {
            let avail;
            try {
                avail = parseInt($('.page-content').css('min-height') || '0')
                    - parseInt($('.page-content').css('padding-top') || '0')
                    - parseInt($('.page-content').css('padding-bottom') || '0');
            }
            catch ($t1) {
                avail = 100;
            }
            handler(avail);
        };

        if ((window as any).Metronic) {
            (window as any).Metronic.addResizeHandler(layout);
        }
        else {
            $(window).resize(layout);
        }

        layout();
    }

    export function triggerLayoutOnShow(element) {
        Serenity.LazyLoadHelper.executeEverytimeWhenShown(element, function () {
            element.triggerHandler('layout');
        }, true);
    }

    export function autoFullHeight(element: JQuery) {
        element.css('height', '100%');
        triggerLayoutOnShow(element);
    }

    export function setMobileDeviceMode() {
        let isMobile = navigator.userAgent.indexOf('Mobi') >= 0 ||
            window.matchMedia('(max-width: 767px)').matches;

        let body = $(document.body);
        if (body.hasClass('mobile-device')) {
            if (!isMobile) {
                body.removeClass('mobile-device');
            }
        }
        else if (isMobile) {
            body.addClass('mobile-device');
        }
    }

    export let defaultNotifyOptions: ToastrOptions = {
        timeOut: 3000,
        showDuration: 250,
        hideDuration: 500,
        extendedTimeOut: 500,
        positionClass: 'toast-top-full-width'
    }

    export function positionToastContainer(create: boolean) {
        if (typeof toastr === 'undefined') {
            return;
        }

        var dialog = $(window.document.body).children('.ui-dialog:visible').last();
        var container = toastr.getContainer(null, create);
        if (container.length === 0) {
            return;
        }
        if (dialog.length > 0) {
            var position = dialog.position();
            container.addClass('positioned-toast toast-top-full-width');
            container.css({ position: 'absolute', top: position.top + 28 + 'px', left: position.left + 6 + 'px', width: dialog.width() - 12 + 'px' });
        }
        else {
            container.addClass('toast-top-full-width');
            if (container.hasClass('positioned-toast')) {
                container.removeClass('positioned-toast');
                container.css({ position: '', top: '', left: '', width: '' });
            }
        }
    }

    function getToastrOptions(options: ToastrOptions) {
        options = $.extend<ToastrOptions>({}, defaultNotifyOptions, options);
        positionToastContainer(true);
        return options;
    }

    export function notifyWarning(message: string, title?: string, options?: ToastrOptions) {
        toastr.warning(message, title, getToastrOptions(options));
    }

    export function notifySuccess(message: string, title?: string, options?: ToastrOptions) {
        toastr.success(message, title, getToastrOptions(options));
    }

    export function notifyInfo(message: string, title?: string, options?: ToastrOptions) {
        toastr.info(message, title, getToastrOptions(options));
    }

    export function notifyError(message: string, title?: string, options?: ToastrOptions) {
        toastr.error(message, title, getToastrOptions(options));
    }

    export function getRemoteData(key) {
        return ScriptData.ensure('RemoteData.' + key);
    }

    export function getRemoteDataAsync(key) {
        return ScriptData.ensureAsync('RemoteData.' + key);
    }

    export function getLookup<TItem>(key): Lookup<TItem> {
        return ScriptData.ensure('Lookup.' + key);
    }

    export function getLookupAsync<TItem>(key) {
        return ScriptData.ensureAsync('Lookup.' + key);
    }

    export function reloadLookup(key) {
        ScriptData.reload('Lookup.' + key);
    }

    export function reloadLookupAsync(key) {
        return ScriptData.reloadAsync('Lookup.' + key);
    }

    export function getColumns(key) {
        return ScriptData.ensure('Columns.' + key);
    }

    export function getColumnsAsync(key) {
        return ScriptData.ensureAsync('Columns.' + key);
    }

    export function getForm(key) {
        return ScriptData.ensure('Form.' + key);
    }

    export function getFormAsync(key) {
        return ScriptData.ensureAsync('Form.' + key);
    }

    export function getTemplate(key) {
        return ScriptData.ensure('Template.' + key);
    }

    export function getTemplateAsync(key) {
        return ScriptData.ensureAsync('Template.' + key);
    }

    export function canLoadScriptData(name) {
        return ScriptData.canLoad(name);
    }

    export function serviceCall<TResponse>(options: S.ServiceOptions<TResponse>) {
        let handleError = function (response: any) {
            if (Config.notLoggedInHandler != null &&
                response &&
                response.Error &&
                Config.notLoggedInHandler(options, response)) {
                return;
            }

            if (options.onError != null) {
                options.onError(response);
                return;
            }

            ErrorHandling.showServiceError(response.Error);
        };

        var url = options.service;
        if (url && url.length && url.charAt(0) != '~' && url.charAt(0) != '/' && url.indexOf('://') < 0)
            url = resolveUrl("~/services/") + url;

        options = $.extend<S.ServiceOptions<TResponse>>({
            dataType: 'json',
            contentType: 'application/json',
            type: 'POST',
            cache: false,
            blockUI: true,
            url: url,
            data: $.toJSON(options.request),
            success: function (data, textStatus, request) {
                try {
                    if (!data.Error && options.onSuccess) {
                        options.onSuccess(data);
                    }
                }
                finally {
                    if (options.blockUI) {
                        blockUndo();
                    }
                    options.onCleanup && options.onCleanup();
                }
            },
            error: function (xhr, status, ev) {
                try {
                    if (xhr.status === 403) {
                        var l = null;
                        try {
                            l = xhr.getResponseHeader('Location');
                        }
                        catch ($t1) {
                            l = null;
                        }
                        if (l) {
                            window.top.location.href = l;
                            return;
                        }
                    }
                    if ((xhr.getResponseHeader('content-type') || '')
                        .toLowerCase().indexOf('application/json') >= 0) {
                        var json = $.parseJSON(xhr.responseText);
                        if (json && json.Error) {
                            handleError(json);
                            return;
                        }
                    }
                    var html = xhr.responseText;
                    iframeDialog({ html: html });
                }
                finally {
                    if (options.blockUI) {
                        blockUndo();
                    }
                    options.onCleanup && options.onCleanup();
                }
            }
        }, options);

        if (options.blockUI) {
            blockUI(null);
        }

        return $.ajax(options);
    }

    export function serviceRequest<TResponse>(service: string, request?: any,
        onSuccess?: (response: TResponse) => void, options?: S.ServiceOptions<TResponse>) {
        return serviceCall($.extend<S.ServiceOptions<TResponse>>({
            service: service,
            request: request,
            onSuccess: onSuccess
        }, options));
    }

    export function setEquality(request, field, value) {
        if (request.EqualityFilter == null) {
            request.EqualityFilter = {};
        }
        request.EqualityFilter[field] = value;
    }

    export function toSingleLine(str: string) {
        return Q.replaceAll(Q.replaceAll(trimToEmpty(str), '\r\n', ' '), '\n', ' ').trim();
    }

    export function text(key: string): string {
        let t = LT.$table[key];
        if (t == null) {
            t = key || '';
        }
        return t;
    }

    export function tryGetText(key): string {
        return LT.$table[key];
    }

    export function resolveUrl(url) {
        if (url && url.substr(0, 2) === '~/') {
            return Config.applicationPath + url.substr(2);
        }

        return url;
    }

    export function formatDayHourAndMin(n: number): string {
        if (n === 0)
            return '0';
        else if (!n)
            return '';
        let days = Math.floor(n / 24 / 60);
        let txt = "";
        if (days > 0) {
            txt += days.toString();
        }
        let mins = zeroPad(Math.floor((n % (24 * 60)) / (60)), 2) + ':' + zeroPad(n % 60, 2);
        if (mins != '00:00') {
            if (days > 0)
                txt += ".";
            txt += mins;
        }
        return txt;
    }

    export function formatISODateTimeUTC(d: Date): string {
        if (d == null)
            return "";
        let zeropad = function (num) { return ((num < 10) ? '0' : '') + num; };
        let str = d.getUTCFullYear() + "-" +
            zeropad(d.getUTCMonth() + 1) + "-" +
            zeropad(d.getUTCDate()) + "T" +
            zeropad(d.getUTCHours()) + ":" +
            zeropad(d.getUTCMinutes());
        let secs = Number(d.getUTCSeconds() + "." +
            ((d.getUTCMilliseconds() < 100) ? '0' : '') +
            zeropad(d.getUTCMilliseconds()));
        str += ":" + zeropad(secs) + "Z";
        return str;
    }

    export function formatNumber(n: number, fmt: string, dec?: string, grp?: string): string {
        let neg = '-';
        if (isNaN(n)) {
            return null;
        }

        dec = dec || Culture.decimalSeparator;
        grp = grp || Culture.get_groupSeperator();

        let r = "";
        if (fmt.indexOf(".") > -1) {
            let dp = dec;
            let df = fmt.substring(fmt.lastIndexOf(".") + 1);
            n = roundNumber(n, df.length);
            let dv = n % 1;
            let ds = new String(dv.toFixed(df.length));
            ds = ds.substring(ds.lastIndexOf(".") + 1);
            for (let i = 0; i < df.length; i++) {
                if (df.charAt(i) == '#' && ds.charAt(i) != '0') {
                    dp += ds.charAt(i);
                    continue;
                }
                else if (df.charAt(i) == '#' && ds.charAt(i) == '0') {
                    let notParsed = ds.substring(i);
                    if (notParsed.match('[1-9]')) {
                        dp += ds.charAt(i);
                        continue;
                    }
                    else
                        break;
                }
                else if (df.charAt(i) == "0")
                    dp += ds.charAt(i);
                else
                    dp += df.charAt(i);
            }
            r += dp;
        }
        else
            n = Math.round(n);

        let ones = Math.floor(n);
        if (n < 0)
            ones = Math.ceil(n);
        let of = "";
        if (fmt.indexOf(".") == -1)
            of = fmt;
        else
            of = fmt.substring(0, fmt.indexOf("."));

        let op = "";
        if (!(ones == 0 && of.substr(of.length - 1) == '#')) {
            // find how many digits are in the group
            let oneText = new String(Math.abs(ones));
            let gl = 9999;
            if (of.lastIndexOf(",") != -1)
                gl = of.length - of.lastIndexOf(",") - 1;
            let gc = 0;
            for (let i = oneText.length - 1; i > -1; i--) {
                op = oneText.charAt(i) + op;
                gc++;
                if (gc == gl && i != 0) {
                    op = grp + op;
                    gc = 0;
                }
            }

            // account for any pre-data padding
            if (of.length > op.length) {
                let padStart = of.indexOf('0');
                if (padStart != -1) {
                    let padLen = of.length - padStart;
                    // pad to left with 0's or group char
                    let pos = of.length - op.length - 1;
                    while (op.length < padLen) {
                        let pc = of.charAt(pos);
                        // replace with real group char if needed
                        if (pc == ',')
                            pc = grp;
                        op = pc + op;
                        pos--;
                    }
                }
            }
        }

        if (!op && of.indexOf('0', of.length - 1) !== -1)
            op = '0';

        r = op + r;
        if (n < 0)
            r = neg + r;

        if (r.lastIndexOf(dec) == r.length - 1) {
            r = r.substring(0, r.length - 1);
        }

        return r;
    }

    function roundNumber(n: number, dec?: number): number {
        let power = Math.pow(10, dec || 0);
        let value = (Math.round(n * power) / power).toString();
        // ensure the decimal places are there
        if (dec > 0) {
            let dp = value.indexOf(".");
            if (dp == -1) {
                value += '.';
                dp = 0;
            }
            else {
                dp = value.length - (dp + 1);
            }
            while (dp < dec) {
                value += '0';
                dp++;
            }
        }
        return parseFloat(value);
    }

    let isoRegexp = /(\d{4,})(?:-(\d{1,2})(?:-(\d{1,2})(?:[T ](\d{1,2}):(\d{1,2})(?::(\d{1,2})(?:\.(\d+))?)?(?:(Z)|([+-])(\d{1,2})(?::(\d{1,2}))?)?)?)?)?/;
    
    export function parseInteger(s: string): number {
        s = trim(s.toString());
        let ts = Culture.get_groupSeperator();
        if (s && s.length && s.indexOf(ts) > 0) {
            s = s.replace(new RegExp("(\\b\\d{1,3})\\" + ts + "(?=\\d{3}(\\D|$))", "g"), '$1');
        }
        if (!(/^[-\+]?\d+$/.test(s)))
            return NaN;
        return parseInt(s, 10);
    }

    export function parseDate(s: string, dateOrder?: string): any {
        if (!s || !s.length)
            return null;
        let dateVal;
        let dArray;
        let d, m, y;
        dArray = splitDateString(s);
        if (!dArray)
            return false;

        if (dArray.length == 3) {
            dateOrder = dateOrder || Culture.dateOrder;
            switch (dateOrder) {
                case "dmy":
                    d = parseInt(dArray[0], 10);
                    m = parseInt(dArray[1], 10) - 1;
                    y = parseInt(dArray[2], 10);
                    break;
                case "ymd":
                    d = parseInt(dArray[2], 10);
                    m = parseInt(dArray[1], 10) - 1;
                    y = parseInt(dArray[0], 10);
                    break;
                case "mdy":
                default:
                    d = parseInt(dArray[1], 10);
                    m = parseInt(dArray[0], 10) - 1;
                    y = parseInt(dArray[2], 10);
                    break;
            }

            if (isNaN(d) || isNaN(m) || isNaN(y) || d < 1 || d > 31 || m < 0 || m > 11 || y > 9999 || y < 0)
                return false;

            if (y < 100) {
                let fullYear = new Date().getFullYear();
                let shortYearCutoff = (fullYear % 100) + 10;
                y += fullYear - fullYear % 100 + (y <= shortYearCutoff ? 0 : -100);
            }
            try {
                dateVal = new Date(y, m, d);
                if (isNaN(dateVal.getFullYear()))
                    return false;
            }
            catch (e) {
                return false;
            }
        }
        else if (dArray.length == 1) {
            try {
                dateVal = new Date(dArray[0]);
                if (isNaN(dateVal.getFullYear()))
                    return false;
            }
            catch (e) {
                return false;
            }
        }
        return dateVal;
    }

    export function parseDecimal(s: string): number {
        if (s == null)
            return null;

        s = trim(s.toString());
        if (s.length == 0)
            return null;

        let ts = Culture.get_groupSeperator();

        if (s && s.length && s.indexOf(ts) > 0) {
            s = s.replace(new RegExp("(\\b\\d{1,3})\\" + ts + "(?=\\d{3}(\\D|$))", "g"), '$1');
        }

        if (!(new RegExp("^\\s*([-\\+])?(\\d*)\\" + Culture.decimalSeparator + "?(\\d*)\\s*$").test(s)))
            return NaN;

        return parseFloat(s.toString().replace(Culture.decimalSeparator, '.'));
    }

    export function splitDateString(s: string): string[] {
        s = trim(s);
        if (!s.length)
            return;
        if (s.indexOf("/") >= 0)
            return s.split("/");
        else if (s.indexOf(".") >= 0)
            return s.split(".");
        else if (s.indexOf("-") >= 0)
            return s.split("-");
        else if (s.indexOf("\\") >= 0)
            return s.split("\\");
        else
            return [s];
    }

    export function parseISODateTime(s: string): Date {
        if (!s || !s.length)
            return null;

        let timestamp = Date.parse(s);
        if (!isNaN(timestamp) && typeof timestamp == "Date")
            return <Date><any>timestamp;

        s = s + "";
        if (typeof (s) != "string" || s.length === 0) {
            return null;
        }

        let res = s.match(isoRegexp);
        if (typeof (res) == "undefined" || res === null) {
            return null;
        }

        let year, month, day, hour, min, sec, msec;
        year = parseInt(res[1], 10);

        if (typeof (res[2]) == "undefined" || res[2] === '') {
            return new Date(year);
        }

        month = parseInt(res[2], 10) - 1;
        day = parseInt(res[3], 10);
        if (typeof (res[4]) == "undefined" || res[4] === '') {
            return new Date(year, month, day);
        }

        hour = parseInt(res[4], 10);
        min = parseInt(res[5], 10);
        sec = (typeof (res[6]) != "undefined" && res[6] !== '') ? parseInt(res[6], 10) : 0;

        if (typeof (res[7]) != "undefined" && res[7] !== '') {
            msec = Math.round(1000.0 * parseFloat("0." + res[7]));
        }
        else {
            msec = 0;
        }

        if ((typeof (res[8]) == "undefined" || res[8] === '') && (typeof (res[9]) == "undefined" || res[9] === '')) {
            return new Date(year, month, day, hour, min, sec, msec);
        }

        let ofs;
        if (typeof (res[9]) != "undefined" && res[9] !== '') {
            ofs = parseInt(res[10], 10) * 3600000;
            if (typeof (res[11]) != "undefined" && res[11] !== '') {
                ofs += parseInt(res[11], 10) * 60000;
            }
            if (res[9] == "-") {
                ofs = -ofs;
            }
        }
        else {
            ofs = 0;
        }
        return new Date(Date.UTC(year, month, day, hour, min, sec, msec) - ofs);
    }

    export function parseHourAndMin(value: string) {
        let v = trim(value);
        if (v.length < 4 || v.length > 5)
            return NaN;
        let h, m;
        if (v.charAt(1) == ':') {
            h = parseInteger(v.substr(0, 1));
            m = parseInteger(v.substr(2, 2));
        }
        else {
            if (v.charAt(2) != ':')
                return NaN;
            h = parseInteger(v.substr(0, 2));
            m = parseInteger(v.substr(3, 2));
        }
        if (isNaN(h) || isNaN(m) || h < 0 || h > 23 || m < 0 || m > 59)
            return NaN;
        return h * 60 + m;
    }

    export function parseDayHourAndMin(s: string): number {
        let days;
        let v = trim(s);
        if (!v)
            return NaN;
        let p = v.split('.');
        if (p.length == 0 || p.length > 2)
            return NaN;
        if (p.length == 1) {
            days = parseInteger(p[0]);
            if (!isNaN(days))
                return days * 24 * 60;
            return parseHourAndMin(p[0]);
        }
        else {
            days = parseInteger(p[0]);
            let hm = parseHourAndMin(p[1]);
            if (isNaN(days) || isNaN(hm))
                return NaN;
            return days * 24 * 60 + hm;
        }
    }

    export function parseQueryString(s?: string): {} {
        let qs: string;
        if (s === undefined)
            qs = location.search.substring(1, location.search.length);
        else
            qs = s || '';
        let result = {};
        let parts = qs.split('&');
        for (let i = 0; i < parts.length; i++) {
            let pair = parts[i].split('=');
            let name = decodeURIComponent(pair[0]);
            result[name] = (pair.length >= 2 ? decodeURIComponent(pair[1]) : name);
        }
        return result;
    }

    let turkishOrder: {};

    export function turkishLocaleCompare(a: string, b: string): number {
        let alphabet = "AaBbCcÇçFfGgĞğHhIıİiJjKkLlMmNnOoÖöPpRrSsŞşTtUuÜüVvYyZz";
        a = a || "";
        b = b || "";
        if (a == b)
            return 0;
        if (!turkishOrder) {
            turkishOrder = {};
            for (let z = 0; z < alphabet.length; z++) {
                turkishOrder[alphabet.charAt(z)] = z + 1;
            }
        }
        for (let i = 0, _len = Math.min(a.length, b.length); i < _len; i++) {
            let x = a.charAt(i), y = b.charAt(i);
            if (x === y)
                continue;
            let ix = turkishOrder[x], iy = turkishOrder[y];
            if (ix != null && iy != null)
                return ix < iy ? -1 : 1;
            let c = x.localeCompare(y);
            if (c == 0)
                continue;
            return c;
        }
        return a.localeCompare(b);
    }

    export function turkishLocaleToUpper(a: string): string {
        if (!a)
            return a;
        return a.replace(/i/g, 'İ').replace(/ı/g, 'I').toUpperCase();
    }

    export function postToService(options: S.PostToServiceOptions) {
        let form = $('<form/>')
            .attr('method', 'POST')
            .attr('action', options.url ? (resolveUrl(options.url)) : resolveUrl('~/services/' + options.service))
            .appendTo(document.body);
        if (options.target)
            form.attr('target', options.target);
        let div = $('<div/>').appendTo(form);
        $('<input/>').attr('type', 'hidden').attr('name', 'request')
            .val($['toJSON'](options.request))
            .appendTo(div);
        $('<input/>').attr('type', 'submit')
            .appendTo(div);
        form.submit();
        window.setTimeout(function () { form.remove(); }, 0);
    }

    export function postToUrl(options: S.PostToUrlOptions) {
        let form = $('<form/>')
            .attr('method', 'POST')
            .attr('action', resolveUrl(options.url))
            .appendTo(document.body);
        if (options.target)
            form.attr('target', options.target);
        let div = $('<div/>').appendTo(form);
        if (options.params != null) {
            for (let k in options.params) {
                $('<input/>').attr('type', 'hidden').attr('name', k)
                    .val(options.params[k])
                    .appendTo(div);
            }
        }
        $('<input/>').attr('type', 'submit')
            .appendTo(div);
        form.submit();
        window.setTimeout(function () { form.remove(); }, 0);
    }

    if ($.fn.button && $.fn.button.noConflict) {
        let btn = $.fn.button.noConflict();
        $.fn.btn = btn;
    }

    $.fn.flexHeightOnly = function (flexY = 1) {
        return this.flexWidthHeight(0, flexY);
    }

    $.fn.flexWidthOnly = function (flexX = 1) {
        return this.flexWidthHeight(flexX, 0);
    }

    $.fn.flexWidthHeight = function (flexX = 1, flexY = 1) {
        return this.addClass('flexify').data('flex-x', flexX).data('flex-y', flexY);
    }

    $.fn.flexX = function (flexX: number): JQuery {
        return this.data('flex-x', flexX);
    }

    $.fn.flexY = function (flexY): JQuery {
        return this.data('flex-y', flexY);
    }

    // derived from https://github.com/mistic100/jQuery.extendext/blob/master/jQuery.extendext.js
    export function deepClone<TItem>(arg1: TItem, ...args: TItem[]): TItem {
        let options, name, src, copy, copyIsArray, clone, target = arguments[0] || {}, i = 1, length = arguments.length;
        // Handle case when target is a string or something (possible in deep copy)
        if (typeof target !== "object" && !$.isFunction(target)) {
            target = {};
        }
        if (i === length) {
            target = {};
            i = 0;
        }
        for (; i < length; i++) {
            // Only deal with non-null/undefined values
            if ((options = arguments[i]) !== null) {
                // Special operations for arrays
                if ($.isArray(options)) {
                    target = $.extend(true, [], options);
                }
                else {
                    // Extend the base object
                    for (name in options) {
                        src = target[name];
                        copy = options[name];
                        // Prevent never-ending loop
                        if (target === copy) {
                            continue;
                        }
                        // Recurse if we're merging plain objects or arrays
                        if (copy && ($.isPlainObject(copy) ||
                            (copyIsArray = $.isArray(copy)))) {
                            if (copyIsArray) {
                                copyIsArray = false;
                                clone = src && $.isArray(src) ? src : [];
                            }
                            else {
                                clone = src && $.isPlainObject(src) ? src : {};
                            }
                            // Never move original objects, clone them
                            target[name] = deepClone(clone, copy);
                        }
                        else if (copy !== undefined) {
                            target[name] = copy;
                        }
                    }
                }
            }
        }
        // Return the modified object
        return target;
    }

    function enumerateTypes(global, namespaces: string[], callback: (type: any, fullName: string) => void) {
        function scan(root, fullName, depth) {
            if (!root)
                return;

            if ($.isArray(root) ||
                root instanceof Date)
                return;

            var t = typeof (root);

            if (t == "string" ||
                t == "number")
                return;

            if ($.isFunction(root))
                callback(root, fullName);

            if (depth > 3)
                return;

            for (var k of Object.getOwnPropertyNames(root)) {
                if (k.charAt(0) < 'A' || k.charAt(0) > 'Z')
                    continue;

                if (k.indexOf('$') >= 0)
                    continue;

                if (k == "prototype")
                    continue;

                scan(root[k], fullName + '.' + k, depth + 1);
            }
        }

        for (var nsRoot of namespaces) {
            if (nsRoot == null || !nsRoot.length) {
                continue;
            }

            if (nsRoot.indexOf('.') >= 0) {
                let g = global;
                let parts = nsRoot.split('.');
                for (var p of parts) {
                    if (!p.length)
                        continue;

                    g = g[p];
                    if (!g)
                        continue;
                }
                scan(g, nsRoot, 0);
            }

            scan(global[nsRoot], nsRoot, 0);
        }
    }

    export namespace ErrorHandling {

        export function showServiceError(error: any) {
            let msg: any;
            if (error == null) {
                msg = '??ERROR??';
            }
            else {
                msg = error.Message;
                if (msg == null) {
                    msg = error.Code;
                }
            }
            Q.alert(msg);
        }
    }

    export namespace Config {
        export let applicationPath = '/';
        export let emailAllowOnlyAscii = true;
        export let rootNamespaces = ['Serenity'];
        export let notLoggedInHandler: Function = null;

        var pathLink = $('link#ApplicationPath');
        if (pathLink.length > 0) {
            applicationPath = pathLink.attr('href');
        }
    }

    export namespace Culture {
        export let decimalSeparator = '.';
        export let dateSeparator = '/';
        export let dateOrder = 'dmy';
        export let dateFormat = 'dd/MM/yyyy';
        export let dateTimeFormat = 'dd/MM/yyyy HH:mm:ss';

        export function get_groupSeperator(): string {
            return ((decimalSeparator === ',') ? '.' : ',');
        };

        var s = Q.trimToNull($('script#ScriptCulture').html());
        if (s != null) {
            var sc = $.parseJSON(s);
            if (sc.DecimalSeparator != null)
                decimalSeparator = sc.DecimalSeparator;
            if (sc.DateSeparator != null)
                dateSeparator = sc.DateSeparator;
            if (sc.DateOrder != null)
                dateOrder = sc.DateOrder;
            if (sc.DateFormat != null)
                dateFormat = sc.DateFormat;
            if (sc.DateTimeFormat != null)
                dateTimeFormat = sc.DateTimeFormat;
        }
    }

    export interface LookupOptions<TItem> {
        idField?: string;
        parentIdField?: string;
        textField?: string;
        textFormatter?(item: TItem): string;
    }

    export class Lookup<TItem> {
        public items: TItem[] = [];
        public itemById: { [key: string]: TItem } = {};
        public idField: string;
        public parentIdField: string;
        public textField: string;
        public textFormatter: (item: TItem) => string;

        constructor(options: LookupOptions<TItem>, items?: TItem[]) {
            options = options || {};
            this.textFormatter = options.textFormatter;
            this.idField = options.idField;
            this.parentIdField = options.parentIdField;
            this.textField = options.textField;
            this.textFormatter = options.textFormatter;

            if (items != null)
                this.update(items);
        }
        
        update(value: TItem[]) {
            this.items = [];
            this.itemById = {};
            if (value) {
                for (var k of value)
                    this.items.push(k);
            }
            var idField = this.idField;
            if (!Q.isEmptyOrNull(idField)) {
                for (var r of this.items) {
                    var v = r[idField];
                    if (v != null) {
                        this.itemById[v] = r;
                    }
                }
            }
        }

        protected get_idField() {
            return this.idField;
        }

        protected get_parentIdField() {
            return this.parentIdField;
        }

        protected get_textField() {
            return this.textField;
        }

        protected get_textFormatter() {
            return this.textFormatter;
        }

        protected get_itemById() {
            return this.itemById;
        }

        protected get_items() {
            return this.items;
        }
    }

    export class LT {
        static $table: { [key: string]: string } = {};
        static empty: LT = new LT('');

        constructor(private key: string) {
        }

        static add(obj: any, pre?: string) {
            if (!obj) {
                return;
            }
            pre = pre || '';
            for (let k of Object.keys(obj)) {
                let actual = pre + k;
                let o = obj[k];
                if (typeof (o) === 'object') {
                    LT.add(o, actual + '.');
                }
                else {
                    LT.$table[actual] = o;
                }
            }
        }

        get() {
            var t = LT.$table[this.key];
            if (t == null) {
                t = this.key || '';
            }
            return t;
        }

        toString() {
            var t = LT.$table[this.key];
            if (t == null) {
                t = this.key || '';
            }
            return t;
        }

        static initializeTextClass = function (type, prefix) {
            var $t1 = Q.arrayClone(Object.keys(type));
            for (var $t2 = 0; $t2 < $t1.length; $t2++) {
                var member = $t1[$t2];
                var value = type[member];
                if (value instanceof LT) {
                    var lt = value;
                    var key = prefix + member;
                    LT.$table[key] = lt.$key;
                    type[member] = new LT(key);
                }
            }
        }

        static getDefault = function (key, defaultText) {
            var t = LT.$table[key];
            if (t == null) {
                t = defaultText;
                if (t == null) {
                    t = key || '';
                }
            }
            return t;
        }
    }

    export namespace ScriptData {
        let registered: { [key: string]: any } = {};
        let loadedData: { [key: string]: any } = {};

        export function bindToChange(name, regClass, onChange) {
            ($(document.body) as any).bind('scriptdatachange.' + regClass, function (e, s) {
                if (s == name) {
                    onChange();
                }
            });
        }

        export function triggerChange(name) {
            $(document.body).triggerHandler('scriptdatachange', [name]);
        }

        export function unbindFromChange(regClass) {
            $(document.body).unbind('scriptdatachange.' + regClass);
        }

        function syncLoadScript(url) {
            $.ajax({ async: false, cache: true, type: 'GET', url: url, data: null, dataType: 'script' });
        }

        function loadScriptAsync(url) {
            return RSVP.resolve().then(function () {
                Q.blockUI(null);
                return RSVP.resolve($.ajax({ async: true, cache: true, type: 'GET', url: url, data: null, dataType: 'script' }).always(function () {
                    Q.blockUndo();
                }));
            }, null);
        }

        function loadScriptData(name: string) {
            if (registered[name] == null) {
                throw new Error(Q.format('Script data {0} is not found in registered script list!', name));
            }

            name = name + '.js?' + registered[name];
            syncLoadScript(Q.resolveUrl('~/DynJS.axd/') + name);
        }

        function loadScriptDataAsync(name: string): RSVP.Thenable<any> {
            return RSVP.resolve().then(function () {
                if (registered[name] == null) {
                    throw new Error(Q.format('Script data {0} is not found in registered script list!', name));
                }

                name = name + '.js?' + registered[name];
                return loadScriptAsync(Q.resolveUrl('~/DynJS.axd/') + name);
            }, null);
        }

        export function ensure(name: string) {
            var data = loadedData[name];
            if (data == null) {
                loadScriptData(name);
            }

            data = loadedData[name];

            if (data == null)
                throw new Error(Q.format("Can't load script data: {0}!", name));

            return data;
        }

        export function ensureAsync(name: string): RSVP.Thenable<any> {
            return RSVP.resolve().then(function () {
                var data = loadedData[name];
                if (data != null) {
                    return RSVP.resolve(data);
                }

                return loadScriptDataAsync(name).then(function () {
                    data = loadedData[name];
                    if (data == null) {
                        throw new Error(Q.format("Can't load script data: {0}!", name));
                    }
                    return data;
                }, null);
            }, null);
        }

        export function reload(name: string) {
            if (registered[name] == null) {
                throw new Error(Q.format('Script data {0} is not found in registered script list!', name));
            }
            registered[name] = (new Date()).getTime().toString();
            loadScriptData(name);
            var data = loadedData[name];
            return data;
        }

        export function reloadAsync(name: string) {
            return RSVP.resolve().then(function () {
                if (registered[name] == null) {
                    throw new Error(Q.format('Script data {0} is not found in registered script list!', name));
                }
                registered[name] = (new Date()).getTime().toString();
                return loadScriptDataAsync(name).then(function () {
                    return loadedData[name];
                }, null);
            }, null);
        }

        export function canLoad(name: string) {
            return (loadedData[name] != null || registered[name] != null);
        }

        export function setRegisteredScripts(scripts) {
            registered = {};
            for (var k in scripts) {
                registered[k] = scripts[k].toString();
            }
        }

        export function set(name: string, value) {
            loadedData[name] = value;
            triggerChange(name);
        }
    }

    (function (global: any) {
        if (typeof RSVP !== undefined) {
            RSVP.on && RSVP.on(function (e) {
                log(e);
                log((e.get_stack && e.get_stack()) || e.stack);
            });
        }

        // fake assembly for typescript apps
        ss.initAssembly({}, 'App', {});

        // for backward compability, avoid!
        global.Q$Externals = Q;
        global.Q$Config = Q.Config;
        global.Q$Culture = Q.Culture;
        global.Q$Lookup = Q.Lookup;
        global.Q$ScriptData = Q.ScriptData;
        global.Q$LT = Q.LT;

        function initializeTypes() {
            enumerateTypes(global, Q.Config.rootNamespaces, function (obj, fullName) {
                // probably Saltaralle class
                if (obj.hasOwnProperty("__typeName") &&
                    !obj.__register)
                    return;

                if (!obj.__interfaces &&
                    obj.prototype.format &&
                    fullName.substr(-9) == "Formatter") {
                    obj.__class = true;
                    obj.__interfaces = [Serenity.ISlickFormatter]
                }

                if (!obj.__class) {
                    var baseType = ss.getBaseType(obj);
                    if (baseType.__class)
                        obj.__class = true;
                }

                if (obj.__class) {
                    obj.__typeName = fullName;
                    if (!obj.__assembly) {
                        obj.__assembly = ss.__assemblies['App'];
                    }
                    obj.__assembly.__types[fullName] = obj;
                }

                delete obj.__register;
            });
        }

        $(function () {
            initializeTypes();

            setMobileDeviceMode();
            $(global).bind('resize', function () {
                setMobileDeviceMode();
            });
        });
    })(window || {});
}

namespace Serenity {

    export class ColumnsKeyAttribute {
        constructor(public value: string) { }
    }

    export class DialogTypeAttribute {
        constructor(public value: Function) { }
    }

    export class EditorAttribute {
        constructor() { }
        key: string;
    }

    export class ElementAttribute {
        constructor(public value: string) { }
    }

    export class EntityTypeAttribute {
        constructor(public value: string) { }
    }

    export class EnumKeyAttribute {
        constructor(public value: string) { }
    }

    export class FlexifyAttribute {
        constructor(public value = true) { }
    }

    export class FormKeyAttribute {
        constructor(public value: string) { }
    }

    export class GeneratedCodeAttribute {
        constructor(public origin?: string) { }
    }

    export class IdPropertyAttribute {
        constructor(public value: string) { }
    }

    export class IsActivePropertyAttribute {
        constructor(public value: string) { }
    }

    export class ItemNameAttribute {
        constructor(public value: string) { }
    }

    export class LocalTextPrefixAttribute {
        constructor(public value: string) { }
    }

    export class MaximizableAttribute {
        constructor(public value = true) { }
    }

    export class NamePropertyAttribute {
        constructor(public value: string) { }
    }

    export class OptionAttribute {
        constructor() { }
    }

    export class OptionsTypeAttribute {
        constructor(public value: Function) { }
    }

    export class PanelAttribute {
        constructor(public value = true) { }
    }

    export class ResizableAttribute {
        constructor(public value = true) { }
    }

    export class ResponsiveAttribute {
        constructor(public value = true) { }
    }

    export class ServiceAttribute {
        constructor(public value: string) { }
    }

    export function Criteria(field: string): any[] {
        return [field];
    }

    export namespace Criteria {
        import C = Criteria;

        export function isEmpty(c: any[]): boolean {
            return c == null ||
                c.length === 0 ||
                (c.length === 1 &&
                    typeof c[0] == "string" &&
                    c[0].length === 0);
        }

        export function join(c1: any[], op: string, c2: any[]): any[] {
            if (C.isEmpty(c1))
                return c2;

            if (C.isEmpty(c2))
                return c1;

            return [c1, op, c2];
        }

        export function paren(c: any[]): any[] {
            return C.isEmpty(c) ? c : ['()', c];
        }

        export function and(c1: any[], c2: any[], ...rest: any[][]): any[] {
            var result = join(c1, 'and', c2);
            if (rest) {
                for (let k of rest)
                    result = join(result, 'and', k);
            }

            return result;
        }

        export function or(c1: any[], c2: any[], ...rest: any[][]): any[] {
            var result = join(c1, 'or', c2);

            if (rest) {
                for (let k of rest)
                    result = join(result, 'or', k);
            }

            return result;
        }
    }

    export namespace Decorators {

        export function addAttribute(type: any, attr: any) {
            let old: any = type.__metadata;
            type.__metadata = type.__metadata || {};
            type.__metadata.attr = type.__metadata.attr || [];
            type.__metadata.attr.push(attr);
        }

        export function addMemberAttr(type: any, memberName: string, attr: any) {
            let old: any = type.__metadata;
            type.__metadata = type.__metadata || {};
            type.__metadata.members = type.__metadata.members || [];
            let member: any = undefined;
            for (var m of type.__metadata.members) {
                if (m.name == memberName) {
                    member = m;
                    break;
                }
            }

            if (!member) {
                member = { name: memberName };
                type.__metadata.members.push(member);
            }

            member.attr = member.attr || [];
            member.attr.push(attr);
        }

        export function columnsKey(value: string) {
            return function (target: Function) {
                addAttribute(target, new ColumnsKeyAttribute(value));
            }
        }

        export function dialogType(value: Function) {
            return function (target: Function) {
                addAttribute(target, new DialogTypeAttribute(value));
            }
        }

        export function editor(key?: string) {
            return function (target: Function) {
                var attr = new EditorAttribute();
                if (key !== undefined)
                    attr.key = key;
                addAttribute(target, attr);
            }
        }

        export function element(value: string) {
            return function (target: Function) {
                addAttribute(target, new ElementAttribute(value));
            }
        }

        export function entityType(value: string) {
            return function (target: Function) {
                addAttribute(target, new EntityTypeAttribute(value));
            }
        }

        export function enumKey(value: string) {
            return function (target: Function) {
                addAttribute(target, new EnumKeyAttribute(value));
            }
        }

        export function flexify(value = true) {
            return function (target: Function) {
                addAttribute(target, new FlexifyAttribute(value));
            }
        }

        export function formKey(value: string) {
            return function (target: Function) {
                addAttribute(target, new FormKeyAttribute(value));
            }
        }

        export function generatedCode(origin?: string) {
            return function (target: Function) {
                addAttribute(target, new GeneratedCodeAttribute(origin));
            }
        }

        export function idProperty(value: string) {
            return function (target: Function) {
                addAttribute(target, new IdPropertyAttribute(value));
            }
        }

        export function registerClass(intf?: any[], asm?: ss.AssemblyReg) {
            return function (target: Function) {
                (target as any).__register = true;
                (target as any).__class = true;
                (target as any).__assembly = asm || ss.__assemblies['App'];
                if (intf)
                    (target as any).__interfaces = intf;
            }
        }

        export function registerEditor(intf?: any[], asm?: ss.AssemblyReg) {
            return registerClass(intf, asm);
        }

        export function registerFormatter(intf = [Serenity.ISlickFormatter], asm?: ss.AssemblyReg) {
            return registerClass(intf, asm);
        }

        export function filterable(value = true) {
            return function (target: Function) {
                addAttribute(target, new FilterableAttribute(value));
            }
        }

        export function itemName(value: string) {
            return function (target: Function) {
                addAttribute(target, new ItemNameAttribute(value));
            }
        }

        export function isActiveProperty(value: string) {
            return function (target: Function) {
                addAttribute(target, new IsActivePropertyAttribute(value));
            }
        }

        export function localTextPrefix(value: string) {
            return function (target: Function) {
                addAttribute(target, new LocalTextPrefixAttribute(value));
            }
        }

        export function maximizable(value = true) {
            return function (target: Function) {
                addAttribute(target, new MaximizableAttribute(value));
            }
        }

        export function nameProperty(value: string) {
            return function (target: Function) {
                addAttribute(target, new NamePropertyAttribute(value));
            }
        }

        export function option() {
            return function (target: Object, propertyKey: string): void {
                addMemberAttr(target.constructor, propertyKey, new OptionAttribute());
            }
        }

        export function optionsType(value: Function) {
            return function (target: Function) {
                addAttribute(target, new OptionsTypeAttribute(value));
            }
        }

        export function panel(value = true) {
            return function (target: Function) {
                addAttribute(target, new PanelAttribute(value));
            }
        }

        export function resizable(value = true) {
            return function (target: Function) {
                addAttribute(target, new ResizableAttribute(value));
            }
        }

        export function responsive(value = true) {
            return function (target: Function) {
                addAttribute(target, new ResponsiveAttribute(value));
            }
        }

        export function service(value: string) {
            return function (target: Function) {
                addAttribute(target, new ServiceAttribute(value));
            }
        }
    }

    export namespace TabsExtensions {
        export function setDisabled(tabs: JQuery, tabKey: string, isDisabled: boolean) {
            if (!tabs)
                return;

            var ibk = indexByKey(tabs);
            if (!ibk)
                return;

            var index = ibk[tabKey];
            if (index == null) {
                return;
            }

            if (index === tabs.tabs('option', 'active')) {
                tabs.tabs('option', 'active', 0);
            }

            tabs.tabs(isDisabled ? 'disable' : 'enable', index);
        }

        export function activeTabKey(tabs: JQuery) {
            var href = tabs.children('ul')
                .children('li')
                .eq(tabs.tabs('option', 'active'))
                .children('a')
                .attr('href')
                .toString();

            var prefix = '_Tab';
            var lastIndex = href.lastIndexOf(prefix);
            if (lastIndex >= 0) {
                href = href.substr(lastIndex + prefix.length);
            }
            return href;
        }

        export function indexByKey(tabs: JQuery): any {
            var indexByKey = tabs.data('indexByKey');
            if (!indexByKey) {
                indexByKey = {};
                tabs.children('ul').children('li').children('a').each(function (index, el) {
                    var href = el.getAttribute('href').toString();
                    var prefix = '_Tab';
                    var lastIndex = href.lastIndexOf(prefix);
                    if (lastIndex >= 0) {
                        href = href.substr(lastIndex + prefix.length);
                    }
                    indexByKey[href] = index;
                });
                tabs.data('indexByKey', indexByKey);
            }

            return indexByKey;
        }
    }

    export namespace LazyLoadHelper {
        let autoIncrement = 0;

        export function executeOnceWhenShown(element: JQuery, callback: Function) {
            autoIncrement++;
            var eventClass = 'ExecuteOnceWhenShown' + autoIncrement;
            var executed = false;
            if (element.is(':visible')) {
                callback();
            }
            else {
                var uiTabs = element.closest('.ui-tabs');
                if (uiTabs.length > 0) {
                    uiTabs.bind('tabsshow.' + eventClass, function (e) {
                        if (element.is(':visible')) {
                            uiTabs.unbind('tabsshow.' + eventClass);
                            if (!executed) {
                                executed = true;
                                element.unbind('shown.' + eventClass);
                                callback();
                            }
                        }
                    });
                }
                var dialog;
                if (element.hasClass('ui-dialog')) {
                    dialog = element.children('.ui-dialog-content');
                }
                else {
                    dialog = element.closest('.ui-dialog-content');
                }
                if (dialog.length > 0) {
                    dialog.bind('dialogopen.' + eventClass, function () {
                        dialog.unbind('dialogopen.' + eventClass);
                        if (element.is(':visible') && !executed) {
                            executed = true;
                            element.unbind('shown.' + eventClass);
                            callback();
                        }
                    });
                }
                element.bind('shown.' + eventClass, function () {
                    if (element.is(':visible')) {
                        element.unbind('shown.' + eventClass);
                        if (!executed) {
                            executed = true;
                            callback();
                        }
                    }
                });
            }
        }

        export function executeEverytimeWhenShown(element: JQuery, callback: Function, callNowIfVisible: boolean) {
            autoIncrement++;
            var eventClass = 'ExecuteEverytimeWhenShown' + autoIncrement;
            var wasVisible = element.is(':visible');

            if (wasVisible && callNowIfVisible) {
                callback();
            }

            var check = function (e) {
                if (element.is(':visible')) {
                    if (!wasVisible) {
                        wasVisible = true;
                        callback();
                    }
                }
                else {
                    wasVisible = false;
                }
            };

            var uiTabs = element.closest('.ui-tabs');
            if (uiTabs.length > 0) {
                uiTabs.bind('tabsactivate.' + eventClass, check);
            }

            var dialog = element.closest('.ui-dialog-content');
            if (dialog.length > 0) {
                dialog.bind('dialogopen.' + eventClass, check);
            }

            element.bind('shown.' + eventClass, check);
        }
    }
}

