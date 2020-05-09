/// <reference types="jquery" />
/// <reference types="jqueryui" />
/// <reference types="toastr" />
/// <reference types="jquery.validation" />
declare var __decorate: any;
declare const __skipExtends: {
    __metadata: boolean;
    __typeName: boolean;
    __componentFactory: boolean;
};
declare var __extends: any;
declare var __assign: any;
declare var __rest: (s: any, e: any) => {};
declare var __spreadArrays: () => any[];
/**
 * Represents the completion of an asynchronous operation
 */
interface PromiseConstructor {
    /**
     * Creates a new Promise.
     * @param executor A callback used to initialize the promise. This callback is passed two arguments:
     * a resolve callback used resolve the promise with a value or the result of another promise,
     * and a reject callback used to reject the promise with a provided reason or error.
     */
    new <T>(executor: (resolve: (value?: T | PromiseLike<T>) => void, reject: (reason?: any) => void) => void): PromiseLike<T>;
    /**
     * Creates a new rejected promise for the provided reason.
     * @param reason The reason the promise was rejected.
     * @returns A new rejected Promise.
     */
    reject(reason: any): PromiseLike<never>;
    /**
     * Creates a new rejected promise for the provided reason.
     * @param reason The reason the promise was rejected.
     * @returns A new rejected Promise.
     */
    reject<T>(reason: any): PromiseLike<T>;
    /**
     * Creates a new resolved promise for the provided value.
     * @param value A promise.
     * @returns A promise whose internal state matches the provided promise.
     */
    resolve<T>(value: T | PromiseLike<T>): PromiseLike<T>;
    /**
     * Creates a new resolved promise .
     * @returns A resolved promise.
     */
    resolve(): PromiseLike<void>;
}
declare var Promise: PromiseConstructor;
interface JQueryStatic {
    extend<T>(target: T, object1?: T, ...objectN: T[]): T;
    toJSON(obj: any): string;
}
declare namespace Select2 {
    namespace util {
        function stripDiacritics(input: string): string;
    }
}
interface Select2QueryOptions {
    element?: JQuery;
    term?: string;
    page?: number;
    context?: any;
    callback?: (p1: Select2Result) => void;
}
interface Select2Item {
    id: string;
    text: string;
    source?: any;
    disabled?: boolean;
}
interface Select2Result {
    results: any;
    more: boolean;
    context?: any;
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
interface Select2Data {
    text?: string;
}
interface JQuery {
    select2(options: Select2Options): JQuery;
    select2(cmd: 'focus' | 'open'): JQuery;
    select2(cmd: 'destroy'): void;
    select2(cmd: 'val'): any;
    select2(cmd: 'val', value: string | string[]): JQuery;
    select2(cmd: 'data'): Select2Data;
}
declare namespace Serenity {
    const enum ColumnSelection {
        List = 0,
        KeyOnly = 1,
        Details = 2
    }
    const enum RetrieveColumnSelection {
        details = 0,
        keyOnly = 1,
        list = 2
    }
}
declare namespace Serenity {
    class ISlickFormatter {
    }
}
declare namespace Serenity {
    interface ServiceError {
        Code?: string;
        Arguments?: string;
        Message?: string;
    }
}
declare namespace Serenity {
    interface ServiceResponse {
        Error?: ServiceError;
    }
    interface ServiceRequest {
    }
    interface SaveRequest<TEntity> extends ServiceRequest {
        EntityId?: any;
        Entity?: TEntity;
        Localizations?: any;
    }
    interface SaveRequestWithAttachment<TEntity> extends SaveRequest<TEntity> {
        Attachments?: any[];
    }
    interface SaveResponse extends ServiceResponse {
        EntityId?: any;
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
        ContainsText?: string;
        ContainsField?: string;
        Criteria?: any[];
        EqualityFilter?: any;
        IncludeDeleted?: boolean;
        ExcludeTotalCount?: boolean;
        ColumnSelection?: ColumnSelection;
        IncludeColumns?: string[];
        ExcludeColumns?: string[];
        DistinctFields?: string[];
    }
    interface ListResponse<TEntity> extends ServiceResponse {
        Entities?: TEntity[];
        Values?: any[];
        TotalCount?: number;
        Skip?: number;
        Take?: number;
    }
    interface RetrieveRequest extends ServiceRequest {
        EntityId?: any;
        ColumnSelection?: RetrieveColumnSelection;
        IncludeColumns?: string[];
        ExcludeColumns?: string[];
    }
    interface RetrieveResponse<TEntity> extends ServiceResponse {
        Entity?: TEntity;
    }
    interface RetrieveLocalizationRequest extends RetrieveRequest {
    }
    interface RetrieveLocalizationResponse<TEntity> extends ServiceResponse {
        Entities?: {
            [key: string]: TEntity;
        };
    }
}
declare let globalObj: any;
declare namespace Q {
    interface Type {
        prototype: any;
        name?: string;
        __typeName?: string;
        __metadata?: {
            __interfaces: any[];
        };
        __propByName?: {
            [key: string]: any;
        };
        __fieldByName?: {
            [key: string]: any;
        };
        isInstanceOfType?: (type: Function) => boolean;
    }
    let types: {
        [key: string]: Type;
    };
    function getNested(from: any, name: string): any;
    let getType: (name: string, target?: any) => Type;
    let getTypeFullName: (type: Type) => string;
    let getTypeName: (type: Type) => string;
    let getInstanceType: (instance: any) => any;
    let isAssignableFrom: (target: any, type: Type) => any;
    let isInstanceOfType: (instance: any, type: Type) => any;
    let safeCast: (instance: any, type: Type) => any;
    let cast: (instance: any, type: Type) => any;
    let getBaseType: (type: any) => any;
    let getAttributes: (type: any, attrType: any, inherit?: boolean) => any[];
    const enum MemberType {
        field = 4,
        property = 16
    }
    let getMembers: (type: any, memberTypes: MemberType) => any[];
    let getTypes: (from?: any) => any[];
    class Exception extends Error {
        constructor(message: string);
    }
    class NullReferenceException extends Exception {
        constructor(message?: string);
    }
    class ArgumentNullException extends Exception {
        constructor(paramName: string, message?: string);
    }
    class ArgumentOutOfRangeException extends Exception {
        constructor(paramName: string, message?: string);
    }
    class InvalidCastException extends Exception {
        constructor(message: string);
    }
    let clearKeys: (d: any) => void;
    let delegateCombine: (delegate1: any, delegate2: any) => any;
    namespace Enum {
        let toString: (enumType: any, value: number) => string;
        let getValues: (enumType: any) => any[];
    }
    let delegateRemove: (delegate1: any, delegate2: any) => any;
    let isEnum: (type: any) => boolean;
    function initFormType(typ: Function, nameWidgetPairs: any[]): void;
    function prop(type: any, name: string, getter?: string, setter?: string): void;
    function initializeTypes(root: any, pre: string, limit: number): void;
}
declare namespace Q {
    type Dictionary<TItem> = {
        [key: string]: TItem;
    };
    type Grouping<TItem> = {
        [key: string]: TItem[];
    };
    function coalesce(a: any, b: any): any;
    function isValue(a: any): boolean;
    /**
         * Tests if any of array elements matches given predicate
         */
    function any<TItem>(array: TItem[], predicate: (x: TItem) => boolean): boolean;
    /**
     * Counts number of array elements that matches a given predicate
     */
    function count<TItem>(array: TItem[], predicate: (x: TItem) => boolean): number;
    /**
     * Gets first element in an array that matches given predicate.
     * Throws an error if no match is found.
     */
    function first<TItem>(array: TItem[], predicate: (x: TItem) => boolean): TItem;
    /**
     * Gets index of first element in an array that matches given predicate
     */
    function indexOf<TItem>(array: TItem[], predicate: (x: TItem) => boolean): number;
    /**
     * Inserts an item to the array at specified index
     */
    function insert(obj: any, index: number, item: any): void;
    /**
     * Determines if the object is an array
     */
    function isArray(obj: any): boolean;
    /**
    * Gets first element in an array that matches given predicate.
    * Throws an error if no matches is found, or there are multiple matches.
    */
    function single<TItem>(array: TItem[], predicate: (x: TItem) => boolean): TItem;
    /**
     * Maps an array into a dictionary with keys determined by specified getKey() callback,
     * and values that are arrays containing elements for a particular key.
     */
    function toGrouping<TItem>(items: TItem[], getKey: (x: TItem) => any): Q.Grouping<TItem>;
    type Group<TItem> = {
        order: number;
        key: string;
        items: TItem[];
        start: number;
    };
    type Groups<TItem> = {
        byKey: Q.Dictionary<Group<TItem>>;
        inOrder: Group<TItem>[];
    };
    /**
     * Groups an array with keys determined by specified getKey() callback.
     * Resulting object contains group objects in order and a dictionary to access by key.
     */
    function groupBy<TItem>(items: TItem[], getKey: (x: TItem) => any): Q.Groups<TItem>;
    /**
     * Gets first element in an array that matches given predicate.
     * Returns null if no match is found.
     */
    function tryFirst<TItem>(array: TItem[], predicate: (x: TItem) => boolean): TItem;
    function endsWith(s: string, suffix: string): boolean;
    function isEmptyOrNull(s: string): boolean;
    function isTrimmedEmpty(s: string): boolean;
    function padLeft(s: string | number, len: number, ch?: string): any;
    function startsWith(s: string, prefix: string): boolean;
    function toSingleLine(str: string): string;
    let today: () => Date;
    var trimEnd: (s: string) => string;
    var trimStart: (s: string) => string;
    function trim(s: string): string;
    function trimToEmpty(s: string): string;
    function trimToNull(s: string): string;
    function replaceAll(s: string, f: string, r: string): string;
    function zeroPad(n: number, digits: number): string;
    /**
     * Returns a function, that, as long as it continues to be invoked, will not
     * be triggered. The function will be called after it stops being called for
     * N milliseconds. If `immediate` is passed, trigger the function on the
     * leading edge, instead of the trailing. The function also has a property 'clear'
     * that is a function which will clear the timer to prevent previously scheduled executions.
     *
     * @source underscore.js
     */
    function debounce(func: Function, wait?: number, immediate?: boolean): () => any;
    function extend<T = any>(a: T, b: T): T;
    function deepClone<T = any>(a: T, a2?: any, a3?: any): T;
}
declare namespace Q {
    interface NumberFormat {
        decimalSeparator: string;
        groupSeparator?: string;
        decimalDigits?: number;
        positiveSign?: string;
        negativeSign?: string;
        nanSymbol?: string;
        percentSymbol?: string;
        currencySymbol?: string;
    }
    interface DateFormat {
        dateSeparator?: string;
        dateFormat?: string;
        dateOrder?: string;
        dateTimeFormat?: string;
        amDesignator?: string;
        pmDesignator?: string;
        timeSeparator?: string;
        firstDayOfWeek?: number;
        dayNames?: string[];
        shortDayNames?: string[];
        minimizedDayNames?: string[];
        monthNames?: string[];
        shortMonthNames?: string[];
    }
    interface Locale extends NumberFormat, DateFormat {
        stringCompare?: (a: string, b: string) => number;
        toUpper?: (a: string) => string;
    }
    let Invariant: Locale;
    function compareStringFactory(order: string): ((a: string, b: string) => number);
    let Culture: Locale;
    function turkishLocaleToUpper(a: string): string;
    let turkishLocaleCompare: (a: string, b: string) => number;
    function format(format: string, ...prm: any[]): string;
    function localeFormat(format: string, l: Locale, ...prm: any[]): string;
    let round: (n: number, d?: number, rounding?: boolean) => number;
    let trunc: (n: number) => number;
    function formatNumber(num: number, format?: string, decOrLoc?: string | Q.NumberFormat, grp?: string): string;
    function parseInteger(s: string): number;
    function parseDecimal(s: string): number;
    function toId(id: any): any;
    function formatDate(d: Date | string, format?: string, locale?: Locale): string;
    function formatDayHourAndMin(n: number): string;
    function formatISODateTimeUTC(d: Date): string;
    function parseISODateTime(s: string): Date;
    function parseHourAndMin(value: string): number;
    function parseDayHourAndMin(s: string): number;
    function parseDate(s: string, dateOrder?: string): any;
    function splitDateString(s: string): string[];
}
declare namespace Q {
    function text(key: string): string;
    function dbText(prefix: string): ((key: string) => string);
    function prefixedText(prefix: string): (text: string, key: string | ((p?: string) => string)) => string;
    function tryGetText(key: string): string;
    function dbTryText(prefix: string): ((key: string) => string);
    function proxyTexts(o: Object, p: string, t: Object): Object;
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
        static initializeTextClass: (type: any, prefix: string) => void;
        static getDefault: (key: string, defaultText: string) => string;
    }
}
declare namespace Q {
    interface LookupOptions<TItem> {
        idField?: string;
        parentIdField?: string;
        textField?: string;
        textFormatter?(item: TItem): string;
    }
    class Lookup<TItem> {
        items: TItem[];
        itemById: {
            [key: string]: TItem;
        };
        idField: string;
        parentIdField: string;
        textField: string;
        textFormatter: (item: TItem) => string;
        constructor(options: LookupOptions<TItem>, items?: TItem[]);
        update(value: TItem[]): void;
        protected get_idField(): string;
        protected get_parentIdField(): string;
        protected get_textField(): string;
        protected get_textFormatter(): (item: TItem) => string;
        protected get_itemById(): {
            [key: string]: TItem;
        };
        protected get_items(): TItem[];
    }
}
declare interface JQBlockUIOptions {
    useTimeout?: boolean;
}
declare namespace Q {
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
}
declare namespace Q {
    function addOption(select: JQuery, key: string, text: string): void;
    function addEmptyOption(select: JQuery): void;
    function clearOptions(select: JQuery): void;
    function findElementWithRelativeId(element: JQuery, relativeId: string): JQuery;
    /**
     * Html attribute encodes a string (encodes quotes in addition to &, > and <)
     * @param s String to be HTML attribute encoded
     */
    function attrEncode(s: any): string;
    /**
     * Html encodes a string
     * @param s String to be HTML encoded
     */
    function htmlEncode(s: any): string;
    function jsRender(markup: string, data?: any): any;
    function log(m: any): void;
    function newBodyDiv(): JQuery;
    function outerHtml(element: JQuery): string;
}
declare namespace Q {
    interface DialogButton {
        text?: string;
        hint?: string;
        icon?: string;
        click?: (e: JQueryEventObject) => void;
        cssClass?: string;
        htmlEncode?: boolean;
        result?: string;
    }
    interface CommonDialogOptions {
        onOpen?: () => void;
        onClose?: (result: string) => void;
        title?: string;
        htmlEncode?: boolean;
        preWrap?: boolean;
        dialogClass?: string;
        buttons?: DialogButton[];
        modalClass?: string;
        bootstrap?: boolean;
        result?: string;
    }
    interface AlertOptions extends CommonDialogOptions {
        okButton?: string | boolean;
    }
    function isBS3(): boolean;
    function bsModalMarkup(title: string, body: string, modalClass?: string): string;
    function dialogButtonToBS(x: DialogButton): string;
    function dialogButtonToUI(x: DialogButton): JQueryUI.DialogButtonOptions;
    function alert(message: string, options?: AlertOptions): void;
    interface ConfirmOptions extends CommonDialogOptions {
        yesButton?: string | boolean;
        noButton?: string | boolean;
        cancelButton?: string | boolean;
        onCancel?: () => void;
        onNo?: () => void;
    }
    function confirm(message: string, onYes: () => void, options?: ConfirmOptions): void;
    interface IFrameDialogOptions {
        html?: string;
    }
    function iframeDialog(options: IFrameDialogOptions): void;
    function information(message: string, onOk: () => void, options?: Q.ConfirmOptions): void;
    function warning(message: string, options?: Q.AlertOptions): void;
}
declare namespace Q {
    let defaultNotifyOptions: ToastrOptions;
    function notifyWarning(message: string, title?: string, options?: ToastrOptions): void;
    function notifySuccess(message: string, title?: string, options?: ToastrOptions): void;
    function notifyInfo(message: string, title?: string, options?: ToastrOptions): void;
    function notifyError(message: string, title?: string, options?: ToastrOptions): void;
    function positionToastContainer(create: boolean): void;
}
declare namespace Q {
    namespace ErrorHandling {
        function showServiceError(error: Serenity.ServiceError): void;
        function runtimeErrorHandler(message: string, filename?: string, lineno?: number, colno?: number, error?: Error): void;
    }
}
declare namespace Q {
    namespace Config {
        /**
         * This is the root path of your application. If your application resides under http://localhost/mysite/,
         * your root path is "mysite/". This variable is automatically initialized by reading from a <link> element
         * with ID "ApplicationPath" from current page, which is usually located in your _LayoutHead.cshtml file
         */
        let applicationPath: string;
        /**
         * Set this to true, to enable responsive dialogs by default, without having to add Serenity.Decorators.responsive()"
         * on dialog classes manually. It's false by default for backward compatibility.
         */
        let responsiveDialogs: boolean;
        /**
         * Set this to true, to prefer bootstrap dialogs over jQuery UI dialogs by default for message dialogs
         */
        let bootstrapMessages: boolean;
        /**
         * This is the list of root namespaces that may be searched for types. For example, if you specify an editor type
         * of "MyEditor", first a class with name "MyEditor" will be searched, if not found, search will be followed by
         * "Serenity.MyEditor" and "MyApp.MyEditor" if you added "MyApp" to the list of root namespaces.
         *
         * You should usually add your application root namespace to this list in ScriptInitialization.ts file.
         */
        let rootNamespaces: string[];
        /**
         * This is an optional method for handling when user is not logged in. If a users session is expired
         * and when a NotAuthorized response is received from a service call, Serenity will call this handler, so
         * you may intercept it and notify user about this situation and ask if she wants to login again...
         */
        let notLoggedInHandler: Function;
    }
}
declare namespace Q {
    function getCookie(name: string): any;
    interface ServiceOptions<TResponse extends Serenity.ServiceResponse> extends JQueryAjaxSettings {
        request?: any;
        service?: string;
        blockUI?: boolean;
        onError?(response: TResponse): void;
        onSuccess?(response: TResponse): void;
        onCleanup?(): void;
    }
    function serviceCall<TResponse>(options: Q.ServiceOptions<TResponse>): JQueryXHR;
    function serviceRequest<TResponse>(service: string, request?: any, onSuccess?: (response: TResponse) => void, options?: Q.ServiceOptions<TResponse>): JQueryXHR;
    function setEquality(request: Serenity.ListRequest, field: string, value: any): void;
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
    function parseQueryString(s?: string): {};
    function postToService(options: Q.PostToServiceOptions): void;
    function postToUrl(options: Q.PostToUrlOptions): void;
    function resolveUrl(url: string): string;
}
declare namespace Serenity {
    type ServiceOptions<TResponse extends ServiceResponse> = Q.ServiceOptions<TResponse>;
}
declare namespace Q {
    namespace ScriptData {
        function bindToChange(name: string, regClass: string, onChange: () => void): void;
        function triggerChange(name: string): void;
        function unbindFromChange(regClass: string): void;
        function ensure(name: string): any;
        function ensureAsync(name: string): PromiseLike<any>;
        function reload(name: string): any;
        function reloadAsync(name: string): PromiseLike<any>;
        function canLoad(name: string): boolean;
        function setRegisteredScripts(scripts: any[]): void;
        function set(name: string, value: any): void;
    }
    function getRemoteData(key: string): any;
    function getRemoteDataAsync(key: string): PromiseLike<any>;
    function getLookup<TItem>(key: string): Lookup<TItem>;
    function getLookupAsync<TItem>(key: string): PromiseLike<Lookup<TItem>>;
    function reloadLookup(key: string): void;
    function reloadLookupAsync(key: string): PromiseLike<any>;
    function getColumns(key: string): Serenity.PropertyItem[];
    function getColumnsAsync(key: string): PromiseLike<Serenity.PropertyItem[]>;
    function getForm(key: string): Serenity.PropertyItem[];
    function getFormAsync(key: string): PromiseLike<Serenity.PropertyItem[]>;
    function getTemplate(key: string): string;
    function getTemplateAsync(key: string): PromiseLike<string>;
    function canLoadScriptData(name: string): boolean;
}
declare namespace JQueryValidation {
    interface ValidationOptions {
        normalizer?: (v: string) => string;
    }
}
declare namespace Q {
    function validateTooltip(form: JQuery, opt: JQueryValidation.ValidationOptions): JQueryValidation.Validator;
    function addValidationRule(element: JQuery, eventClass: string, rule: (p1: JQuery) => string): JQuery;
    function removeValidationRule(element: JQuery, eventClass: string): JQuery;
}
declare namespace Q {
    namespace Authorization {
        function hasPermission(permission: string): boolean;
        function validatePermission(permission: string): void;
    }
    namespace Authorization {
        let isLoggedIn: boolean;
        let username: string;
        let userDefinition: Serenity.UserDefinition;
    }
}
declare namespace Serenity {
    interface UserDefinition {
        Username?: string;
        DisplayName?: string;
        IsAdmin?: boolean;
        Permissions?: {
            [key: string]: boolean;
        };
    }
}
declare namespace Q {
    namespace LayoutTimer {
        function on(key: string, handler: () => void): () => void;
        function onSizeChange(key: string, element: HTMLElement, handler: () => void): () => void;
        function onWidthChange(key: string, element: HTMLElement, handler: () => void): () => void;
        function onHeightChange(key: string, element: HTMLElement, handler: () => void): () => void;
        function off(key: string, handler?: () => void): void;
    }
}
declare namespace Serenity {
    function Criteria(field: string): any[];
    namespace Criteria {
        function isEmpty(c: any[]): boolean;
        function join(c1: any[], op: string, c2: any[]): any[];
        function paren(c: any[]): any[];
        function and(c1: any[], c2: any[], ...rest: any[][]): any[];
        function or(c1: any[], c2: any[], ...rest: any[][]): any[];
        const enum Operator {
            paren = "()",
            not = "not",
            isNull = "is null",
            isNotNull = "is not null",
            exists = "exists",
            and = "and",
            or = "or",
            xor = "xor",
            eq = "=",
            ne = "!=",
            gt = ">",
            ge = ">=",
            lt = "<",
            le = "<=",
            in = "in",
            notIn = "not in",
            like = "like",
            notLike = "not like"
        }
    }
}
declare namespace Serenity {
    enum SummaryType {
        Disabled = -1,
        None = 0,
        Sum = 1,
        Avg = 2,
        Min = 3,
        Max = 4
    }
}
declare namespace Serenity {
    type Constructor<T> = new (...args: any[]) => T;
    interface PropertyItem {
        name?: string;
        title?: string;
        hint?: string;
        placeholder?: string;
        editorType?: string;
        editorParams?: any;
        category?: string;
        collapsible?: boolean;
        collapsed?: boolean;
        tab?: string;
        cssClass?: string;
        headerCssClass?: string;
        formCssClass?: string;
        maxLength?: number;
        required?: boolean;
        insertable?: boolean;
        insertPermission?: string;
        hideOnInsert?: boolean;
        updatable?: boolean;
        updatePermission?: string;
        hideOnUpdate?: boolean;
        readOnly?: boolean;
        readPermission?: string;
        oneWay?: boolean;
        defaultValue?: any;
        localizable?: boolean;
        visible?: boolean;
        allowHide?: boolean;
        formatterType?: string;
        formatterParams?: any;
        displayFormat?: string;
        alignment?: string;
        width?: number;
        widthSet?: boolean;
        minWidth?: number;
        maxWidth?: number;
        labelWidth?: string;
        resizable?: boolean;
        sortable?: boolean;
        sortOrder?: number;
        groupOrder?: number;
        summaryType?: SummaryType;
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
        quickFilterSeparator?: boolean;
        quickFilterCssClass?: string;
    }
}
