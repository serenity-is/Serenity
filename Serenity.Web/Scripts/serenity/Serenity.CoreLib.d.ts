interface RSVP<TResult> {
}
declare namespace Serenity {
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
    interface SaveResponse extends ServiceResponse {
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
        Sort?: string[];
    }
    interface ListResponse<TEntity> extends ServiceResponse {
        Entities?: TEntity[];
        TotalCount: number;
        Skip: number;
        Take: number;
    }
    interface RetrieveRequest extends ServiceRequest {
        EntityId?: any;
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
    interface CheckTreeItem {
    }
    interface PropertyItem {
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
    }
    class StringEditor extends Widget<any> {
    }
    class Select2Editor<TItem, TOptions> {
    }
    class CheckTreeEditor<TItem, TOptions> {
    }
    interface EmailEditorOptions {
        domain?: string;
        readOnlyDomain: boolean;
    }
    class EmailEditor extends Widget<EmailEditorOptions> {
    }
    class PasswordEditor extends StringEditor {
    }
    class DateEditor extends Widget<any> {
    }
    class DateTimeEditor extends Widget<any> {
    }
    interface LookupEditorOptions {
    }
    class LookupEditorBase<TOptions, TItem> extends Widget<TOptions> {
    }
    class LookupEditor extends LookupEditorBase<LookupEditorOptions, any> {
    }
    class ImageUploadEditor extends Widget<any> {
    }
    class BooleanEditor extends Widget<any> {
    }
    class IntegerEditor extends Widget<any> {
    }
    class DecimalEditor extends Widget<any> {
    }
    interface HtmlContentEditorOptions {
    }
    class HtmlContentEditor {
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
        dialogOpen(): void;
        loadByIdAndOpenDialog(id: any): void;
    }
    class FilterStore {
    }
    interface CKEditorConfig {
    }
    interface IDataGrid {
        getElement(): JQuery;
        getGrid(): Slick.Grid;
        getView(): Slick.Data.RemoteView;
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
declare namespace Slick {
    interface FormatterContext {
        row: number;
        cell: number;
        value: any;
        column: any;
    }
    interface Formatter {
        format(ctx: FormatterContext): string;
    }
    class Grid {
    }
    interface GridOptions {
    }
    interface Column {
    }
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
        getDefault: (key: any, defaultText: any) => string;
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
