/// <reference types="toastr" />
/// <reference types="jquery" />
/// <reference types="jquery.validation" />
/// <reference types="jqueryui" />
declare var Reflect: any;
declare var __decorate: any;
declare var __extends: any;
declare class RSVP<TResult> {
    constructor(constructor: (p1: (p1: any) => void, p2: any) => void);
}
declare module RSVP {
    function on(handler: (e: any) => void): void;
    function resolve(): PromiseLike<any>;
}
declare module RSVP {
    class Promise<R> implements PromiseLike<R> {
        /**
         * If you call resolve in the body of the callback passed to the constructor,
         * your promise is fulfilled with result object passed to resolve.
         * If you call reject your promise is rejected with the object passed to resolve.
         * For consistency and debugging (eg stack traces), obj should be an instanceof Error.
         * Any errors thrown in the constructor callback will be implicitly passed to reject().
         */
        constructor(callback: (resolve: (result?: R) => void, reject: (error: any) => void) => void, label?: string);
        /**
         * If you call resolve in the body of the callback passed to the constructor,
         * your promise will be fulfilled/rejected with the outcome of thenable passed to resolve.
         * If you call reject your promise is rejected with the object passed to resolve.
         * For consistency and debugging (eg stack traces), obj should be an instanceof Error.
         * Any errors thrown in the constructor callback will be implicitly passed to reject().
         */
        constructor(callback: (resolve: (thenable?: PromiseLike<R>) => void, reject: (error: any) => void) => void, label?: string);
        /**
         * onFulfilled is called when/if "promise" resolves. onRejected is called when/if "promise" rejects.
         * Both are optional, if either/both are omitted the next onFulfilled/onRejected in the chain is called.
         * Both callbacks have a single parameter , the fulfillment value or rejection reason.
         * "then" returns a new promise equivalent to the value you return from onFulfilled/onRejected after being passed through Promise.resolve.
         * If an error is thrown in the callback, the returned promise rejects with that error.
         *
         * @param onFulfilled called when/if "promise" resolves
         * @param onRejected called when/if "promise" rejects
         */
        then<U>(onFulfilled?: (value: R) => PromiseLike<U>, onRejected?: (error: any) => PromiseLike<U>): Promise<U>;
        /**
         * onFulfilled is called when/if "promise" resolves. onRejected is called when/if "promise" rejects.
         * Both are optional, if either/both are omitted the next onFulfilled/onRejected in the chain is called.
         * Both callbacks have a single parameter , the fulfillment value or rejection reason.
         * "then" returns a new promise equivalent to the value you return from onFulfilled/onRejected after being passed through Promise.resolve.
         * If an error is thrown in the callback, the returned promise rejects with that error.
         *
         * @param onFulfilled called when/if "promise" resolves
         * @param onRejected called when/if "promise" rejects
         */
        then<U>(onFulfilled?: (value: R) => PromiseLike<U>, onRejected?: (error: any) => U): Promise<U>;
        /**
         * onFulfilled is called when/if "promise" resolves. onRejected is called when/if "promise" rejects.
         * Both are optional, if either/both are omitted the next onFulfilled/onRejected in the chain is called.
         * Both callbacks have a single parameter , the fulfillment value or rejection reason.
         * "then" returns a new promise equivalent to the value you return from onFulfilled/onRejected after being passed through Promise.resolve.
         * If an error is thrown in the callback, the returned promise rejects with that error.
         *
         * @param onFulfilled called when/if "promise" resolves
         * @param onRejected called when/if "promise" rejects
         */
        then<U>(onFulfilled?: (value: R) => PromiseLike<U>, onRejected?: (error: any) => void): Promise<U>;
        /**
         * onFulfilled is called when/if "promise" resolves. onRejected is called when/if "promise" rejects.
         * Both are optional, if either/both are omitted the next onFulfilled/onRejected in the chain is called.
         * Both callbacks have a single parameter , the fulfillment value or rejection reason.
         * "then" returns a new promise equivalent to the value you return from onFulfilled/onRejected after being passed through Promise.resolve.
         * If an error is thrown in the callback, the returned promise rejects with that error.
         *
         * @param onFulfilled called when/if "promise" resolves
         * @param onRejected called when/if "promise" rejects
         */
        then<U>(onFulfilled?: (value: R) => U, onRejected?: (error: any) => PromiseLike<U>): Promise<U>;
        /**
         * onFulfilled is called when/if "promise" resolves. onRejected is called when/if "promise" rejects.
         * Both are optional, if either/both are omitted the next onFulfilled/onRejected in the chain is called.
         * Both callbacks have a single parameter , the fulfillment value or rejection reason.
         * "then" returns a new promise equivalent to the value you return from onFulfilled/onRejected after being passed through Promise.resolve.
         * If an error is thrown in the callback, the returned promise rejects with that error.
         *
         * @param onFulfilled called when/if "promise" resolves
         * @param onRejected called when/if "promise" rejects
         */
        then<U>(onFulfilled?: (value: R) => U, onRejected?: (error: any) => U): Promise<U>;
        /**
         * onFulfilled is called when/if "promise" resolves. onRejected is called when/if "promise" rejects.
         * Both are optional, if either/both are omitted the next onFulfilled/onRejected in the chain is called.
         * Both callbacks have a single parameter , the fulfillment value or rejection reason.
         * "then" returns a new promise equivalent to the value you return from onFulfilled/onRejected after being passed through Promise.resolve.
         * If an error is thrown in the callback, the returned promise rejects with that error.
         *
         * @param onFulfilled called when/if "promise" resolves
         * @param onRejected called when/if "promise" rejects
         */
        then<U>(onFulfilled?: (value: R) => U, onRejected?: (error: any) => void): Promise<U>;
        /**
         * Sugar for promise.then(undefined, onRejected)
         *
         * @param onRejected called when/if "promise" rejects
         */
        catch<U>(onRejected?: (error: any) => PromiseLike<U>): Promise<U>;
        /**
         * Sugar for promise.then(undefined, onRejected)
         *
         * @param onRejected called when/if "promise" rejects
         */
        catch<U>(onRejected?: (error: any) => U): Promise<U>;
        /**
         * Sugar for promise.then(undefined, onRejected)
         *
         * @param onRejected called when/if "promise" rejects
         */
        catch<U>(onRejected?: (error: any) => void): Promise<U>;
        finally(finallyCallback: () => any): Promise<R>;
        static all<T>(promises: PromiseLike<T>[]): Promise<T[]>;
        static all<T>(promises: any[]): Promise<T[]>;
        static race<R>(promises: Promise<R>[]): Promise<R>;
        /**
         @method resolve
         @param {Any} value value that the returned promise will be resolved with
         @param {String} label optional string for identifying the returned promise.
         Useful for tooling.
         @return {Promise} a promise that will become fulfilled with the given
         `value`
         */
        static resolve<T>(object: PromiseLike<T>): Promise<T>;
        static resolve<T>(object: T): Promise<T>;
        /**
         @method cast (Deprecated in favor of resolve
         @param {Any} value value that the returned promise will be resolved with
         @param {String} label optional string for identifying the returned promise.
         Useful for tooling.
         @return {Promise} a promise that will become fulfilled with the given
         `value`
         */
        static cast<T>(object: PromiseLike<T>, label?: string): Promise<T>;
        static cast<T>(object: T, label?: string): Promise<T>;
        /**
         `RSVP.Promise.reject` returns a promise rejected with the passed `reason`.
         */
        static reject(reason?: any): Promise<any>;
    }
    interface PromiseState<T> {
        state: string;
        value?: T;
        reason?: any;
    }
    interface InstrumentEvent {
        guid: string;
        childGuid: string;
        eventName: string;
        detail: any;
        label: string;
        timeStamp: number;
    }
    function on(eventName: string, callback: (value: any) => void): void;
    function on(eventName: "error", errorHandler: (reason: any) => void): void;
    function on(eventName: "created", listener: (event: InstrumentEvent) => void): void;
    function on(eventName: "chained", listener: (event: InstrumentEvent) => void): void;
    function on(eventName: "fulfilled", listener: (event: InstrumentEvent) => void): void;
    function on(eventName: "rejected", listener: (event: InstrumentEvent) => void): void;
    function configure(configName: string, value: any): void;
    function configure(configName: "instrument", shouldInstrument: boolean): void;
    /**
     * configure('onerror', handler) is deprecated in favor of on('error', handler)
     * @param configName
     * @param errorHandler
     */
    function configure(configName: "onerror", errorHandler: (reason: any) => void): void;
    /**
     * Make a promise that fulfills when every item in the array fulfills, and rejects if (and when) any item rejects.
     * the array passed to all can be a mixture of promise-like objects and other objects.
     * The fulfillment value is an array (in order) of fulfillment values. The rejection value is the first rejection value.
     */
    function all<T>(promises: PromiseLike<T>[]): Promise<T[]>;
    function all<T>(promises: any[]): Promise<T[]>;
    function race<R>(promises: Promise<R>[]): Promise<R>;
    /**
     `RSVP.Promise.reject` returns a promise rejected with the passed `reason`.
     */
    function reject(reason?: any): Promise<any>;
    /**
     `RSVP.Promise.resolve` returns a promise that will become resolved with the
     passed `value`.
     */
    function resolve<T>(object: PromiseLike<T>): Promise<T>;
    function resolve<T>(object: T): Promise<T>;
    /**
     `RSVP.rethrow` will rethrow an error on the next turn of the JavaScript event
     loop in order to aid debugging.
     Promises A+ specifies that any exceptions that occur with a promise must be
     caught by the promises implementation and bubbled to the last handler. For
     this reason, it is recommended that you always specify a second rejection
     handler function to `then`. However, `RSVP.rethrow` will throw the exception
     outside of the promise, so it bubbles up to your console if in the browser,
     or domain/cause uncaught exception in Node. `rethrow` will also throw the
     error again so the error can be handled by the promise per the spec.
     */
    function rethrow(reason: any): void;
}
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
    let __assemblies: {
        [name: string]: AssemblyReg;
    };
    class Exception {
        constructor(msg: string);
    }
    class NotSupportedException extends Exception {
        constructor(msg: string);
    }
}
interface JQueryStatic {
    extend<T>(target: T, object1?: T, ...objectN: T[]): T;
    toJSON(obj: any): string;
}
interface JQBlockUIOptions {
    useTimeout?: boolean;
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
    function insert<TItem>(array: TItem[], index: number, item: TItem): void;
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
    /**
     * Gets first element in an array that matches given predicate.
     * Returns null if no match is found.
     */
    function tryFirst<TItem>(array: TItem[], predicate: (x: TItem) => boolean): TItem;
    function endsWith(s: string, search: string): boolean;
    function isEmptyOrNull(s: string): boolean;
    function isTrimmedEmpty(s: string): boolean;
    function format(msg: string, ...prm: any[]): string;
    function padLeft(s: string, len: number, ch?: string): string;
    function startsWith(s: string, search: string): boolean;
    function toSingleLine(str: string): string;
    function trim(s: string): string;
    function trimToEmpty(s: string): string;
    function trimToNull(s: string): string;
    function turkishLocaleCompare(a: string, b: string): number;
    function turkishLocaleToUpper(a: string): string;
    function replaceAll(s: string, f: string, r: string): string;
    function zeroPad(n: number, digits: number): string;
    function deepClone<TItem>(arg1: TItem, ...args: TItem[]): TItem;
}
declare namespace Q {
    namespace Culture {
        let decimalSeparator: string;
        let dateSeparator: string;
        let dateOrder: string;
        let dateFormat: string;
        let dateTimeFormat: string;
        function get_groupSeparator(): string;
    }
    function formatNumber(n: number, fmt: string, dec?: string, grp?: string): string;
    function parseInteger(s: string): number;
    function parseDecimal(s: string): number;
    function toId(id: any): any;
    function formatDate(d: Date | string, format?: string): string;
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
    function tryGetText(key: string): string;
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
    function alert(message: string, options?: AlertOptions): void;
    interface ConfirmOptions extends CommonDialogOptions {
        yesButton?: string;
        noButton?: string;
        cancelButton?: string;
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
         * Email validation by default only allows ASCII characters. Set this to true if you want to allow unicode.
         */
        let emailAllowOnlyAscii: boolean;
        /**
         * Set this to true, to enable responsive dialogs by default, without having to add Serenity.Decorators.responsive()"
         * on dialog classes manually. It's false by default for backward compability.
         */
        let responsiveDialogs: boolean;
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
    function autoFullHeight(element: JQuery): void;
    function initFullHeightGridPage(gridDiv: JQuery): void;
    function layoutFillHeightValue(element: JQuery): number;
    function layoutFillHeight(element: JQuery): void;
    function setMobileDeviceMode(): void;
    function triggerLayoutOnShow(element: JQuery): void;
    function centerDialog(el: JQuery): void;
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
    function getColumns(key: string): any;
    function getColumnsAsync(key: string): PromiseLike<any>;
    function getForm(key: string): any;
    function getFormAsync(key: string): PromiseLike<any>;
    function getTemplate(key: string): any;
    function getTemplateAsync(key: string): PromiseLike<any>;
    function canLoadScriptData(name: string): boolean;
}
declare namespace Q {
    function initFormType(typ: Function, nameWidgetPairs: any[]): void;
    function prop(type: any, name: string, getter?: string, setter?: string): void;
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
declare namespace Q.Router {
    let enabled: boolean;
    function navigate(hash: string, tryBack?: boolean, silent?: boolean): void;
    function replace(hash: string, tryBack?: boolean): void;
    function replaceLast(hash: string, tryBack?: boolean): void;
    function dialog(owner: JQuery, element: JQuery, hash: () => string): void;
    function resolve(hash?: string): void;
}
declare namespace Q {
    function h<PropsType = any>(type: Q.FunctionalComponent<PropsType>, props: PropsType, children?: (JSX.Element | JSX.Element[] | string)[]): JSX.Element;
    function h(type: string, props: JSX.HTMLAttributes & JSX.SVGAttributes & {
        [propName: string]: any;
    }, children?: (JSX.Element | JSX.Element[] | string)[]): JSX.Element;
    function maybeFlatten(arr: any[], isSVG?: boolean): any[];
    function mountTo(parent: Element, c: VNode | VNode[]): void;
    type Empty = null | void | boolean;
    class Fragment implements IComponent {
        constructor(props?: any, context?: any);
        mount(props: any, content: VNode[]): Node;
        patch(el: Node, newProps: any, oldProps: any, newContent: VNode[], oldContent: VNode[]): Node;
        unmount(el: Node): void;
    }
    function mount(c: VNode, node?: Node): Node;
}
declare const H: typeof Q.h;
declare namespace Q {
    interface IComponent<PropsType = any> {
        mount(props: PropsType, content: any[]): Node;
        patch(el: Node, newProps: any, oldProps: any, newContent: VNode[], oldContent: VNode[]): Node;
        unmount(el: Node): void;
    }
    interface ComponentProps<C extends FunctionalComponent<any> | Serenity.Widget<any>> {
        children?: JSX.Element[];
        key?: string | number | any;
        ref?: (el: C) => void;
    }
    interface VDomHtmlAttrs {
        setInnerHTML?: string;
        key?: string;
        ref?: (el?: Element) => void;
    }
    interface VNode {
        _vnode?: true;
        _text?: string;
        isSVG?: boolean;
        type?: IComponent<any> | FunctionalComponent<any> | string;
        key?: string;
        props?: {
            [name: string]: any;
        };
        content?: any[];
        _node?: Node;
        _data?: any;
    }
    interface FunctionalComponent<PropsType> {
        (props?: PropsType & ComponentProps<this>, context?: any): JSX.Element;
        displayName?: string;
        defaultProps?: any;
    }
    type AnyComponent<PropsType, StateType> = FunctionalComponent<PropsType>;
}
declare namespace JSX {
    interface Element extends Q.VNode {
    }
    interface ElementClass extends Q.IComponent {
    }
    interface ElementAttributesProperty {
        props: any;
    }
    interface SVGAttributes extends HTMLAttributes {
        accentHeight?: number | string;
        accumulate?: "none" | "sum";
        additive?: "replace" | "sum";
        alignmentBaseline?: "auto" | "baseline" | "before-edge" | "text-before-edge" | "middle" | "central" | "after-edge" | "text-after-edge" | "ideographic" | "alphabetic" | "hanging" | "mathematical" | "inherit";
        allowReorder?: "no" | "yes";
        alphabetic?: number | string;
        amplitude?: number | string;
        arabicForm?: "initial" | "medial" | "terminal" | "isolated";
        ascent?: number | string;
        attributeName?: string;
        attributeType?: string;
        autoReverse?: number | string;
        azimuth?: number | string;
        baseFrequency?: number | string;
        baselineShift?: number | string;
        baseProfile?: number | string;
        bbox?: number | string;
        begin?: number | string;
        bias?: number | string;
        by?: number | string;
        calcMode?: number | string;
        capHeight?: number | string;
        clip?: number | string;
        clipPath?: string;
        clipPathUnits?: number | string;
        clipRule?: number | string;
        colorInterpolation?: number | string;
        colorInterpolationFilters?: "auto" | "sRGB" | "linearRGB" | "inherit";
        colorProfile?: number | string;
        colorRendering?: number | string;
        contentScriptType?: number | string;
        contentStyleType?: number | string;
        cursor?: number | string;
        cx?: number | string;
        cy?: number | string;
        d?: string;
        decelerate?: number | string;
        descent?: number | string;
        diffuseConstant?: number | string;
        direction?: number | string;
        display?: number | string;
        divisor?: number | string;
        dominantBaseline?: number | string;
        dur?: number | string;
        dx?: number | string;
        dy?: number | string;
        edgeMode?: number | string;
        elevation?: number | string;
        enableBackground?: number | string;
        end?: number | string;
        exponent?: number | string;
        externalResourcesRequired?: number | string;
        fill?: string;
        fillOpacity?: number | string;
        fillRule?: "nonzero" | "evenodd" | "inherit";
        filter?: string;
        filterRes?: number | string;
        filterUnits?: number | string;
        floodColor?: number | string;
        floodOpacity?: number | string;
        focusable?: number | string;
        fontFamily?: string;
        fontSize?: number | string;
        fontSizeAdjust?: number | string;
        fontStretch?: number | string;
        fontStyle?: number | string;
        fontVariant?: number | string;
        fontWeight?: number | string;
        format?: number | string;
        from?: number | string;
        fx?: number | string;
        fy?: number | string;
        g1?: number | string;
        g2?: number | string;
        glyphName?: number | string;
        glyphOrientationHorizontal?: number | string;
        glyphOrientationVertical?: number | string;
        glyphRef?: number | string;
        gradientTransform?: string;
        gradientUnits?: string;
        hanging?: number | string;
        horizAdvX?: number | string;
        horizOriginX?: number | string;
        ideographic?: number | string;
        imageRendering?: number | string;
        in2?: number | string;
        in?: string;
        intercept?: number | string;
        k1?: number | string;
        k2?: number | string;
        k3?: number | string;
        k4?: number | string;
        k?: number | string;
        kernelMatrix?: number | string;
        kernelUnitLength?: number | string;
        kerning?: number | string;
        keyPoints?: number | string;
        keySplines?: number | string;
        keyTimes?: number | string;
        lengthAdjust?: number | string;
        letterSpacing?: number | string;
        lightingColor?: number | string;
        limitingConeAngle?: number | string;
        local?: number | string;
        markerEnd?: string;
        markerHeight?: number | string;
        markerMid?: string;
        markerStart?: string;
        markerUnits?: number | string;
        markerWidth?: number | string;
        mask?: string;
        maskContentUnits?: number | string;
        maskUnits?: number | string;
        mathematical?: number | string;
        mode?: number | string;
        numOctaves?: number | string;
        offset?: number | string;
        opacity?: number | string;
        operator?: number | string;
        order?: number | string;
        orient?: number | string;
        orientation?: number | string;
        origin?: number | string;
        overflow?: number | string;
        overlinePosition?: number | string;
        overlineThickness?: number | string;
        paintOrder?: number | string;
        panose1?: number | string;
        pathLength?: number | string;
        patternContentUnits?: string;
        patternTransform?: number | string;
        patternUnits?: string;
        pointerEvents?: number | string;
        points?: string;
        pointsAtX?: number | string;
        pointsAtY?: number | string;
        pointsAtZ?: number | string;
        preserveAlpha?: number | string;
        preserveAspectRatio?: string;
        primitiveUnits?: number | string;
        r?: number | string;
        radius?: number | string;
        refX?: number | string;
        refY?: number | string;
        renderingIntent?: number | string;
        repeatCount?: number | string;
        repeatDur?: number | string;
        requiredExtensions?: number | string;
        requiredFeatures?: number | string;
        restart?: number | string;
        result?: string;
        rotate?: number | string;
        rx?: number | string;
        ry?: number | string;
        scale?: number | string;
        seed?: number | string;
        shapeRendering?: number | string;
        slope?: number | string;
        spacing?: number | string;
        specularConstant?: number | string;
        specularExponent?: number | string;
        speed?: number | string;
        spreadMethod?: string;
        startOffset?: number | string;
        stdDeviation?: number | string;
        stemh?: number | string;
        stemv?: number | string;
        stitchTiles?: number | string;
        stopColor?: string;
        stopOpacity?: number | string;
        strikethroughPosition?: number | string;
        strikethroughThickness?: number | string;
        string?: number | string;
        stroke?: string;
        strokeDasharray?: string | number;
        strokeDashoffset?: string | number;
        strokeLinecap?: "butt" | "round" | "square" | "inherit";
        strokeLinejoin?: "miter" | "round" | "bevel" | "inherit";
        strokeMiterlimit?: string;
        strokeOpacity?: number | string;
        strokeWidth?: number | string;
        surfaceScale?: number | string;
        systemLanguage?: number | string;
        tableValues?: number | string;
        targetX?: number | string;
        targetY?: number | string;
        textAnchor?: string;
        textDecoration?: number | string;
        textLength?: number | string;
        textRendering?: number | string;
        to?: number | string;
        transform?: string;
        u1?: number | string;
        u2?: number | string;
        underlinePosition?: number | string;
        underlineThickness?: number | string;
        unicode?: number | string;
        unicodeBidi?: number | string;
        unicodeRange?: number | string;
        unitsPerEm?: number | string;
        vAlphabetic?: number | string;
        values?: string;
        vectorEffect?: number | string;
        version?: string;
        vertAdvY?: number | string;
        vertOriginX?: number | string;
        vertOriginY?: number | string;
        vHanging?: number | string;
        vIdeographic?: number | string;
        viewBox?: string;
        viewTarget?: number | string;
        visibility?: number | string;
        vMathematical?: number | string;
        widths?: number | string;
        wordSpacing?: number | string;
        writingMode?: number | string;
        x1?: number | string;
        x2?: number | string;
        x?: number | string;
        xChannelSelector?: string;
        xHeight?: number | string;
        xlinkActuate?: string;
        xlinkArcrole?: string;
        xlinkHref?: string;
        xlinkRole?: string;
        xlinkShow?: string;
        xlinkTitle?: string;
        xlinkType?: string;
        xmlBase?: string;
        xmlLang?: string;
        xmlns?: string;
        xmlnsXlink?: string;
        xmlSpace?: string;
        y1?: number | string;
        y2?: number | string;
        y?: number | string;
        yChannelSelector?: string;
        z?: number | string;
        zoomAndPan?: string;
    }
    interface PathAttributes {
        d: string;
    }
    interface EventHandler<E extends Event> {
        (event: E): void;
    }
    type ClipboardEventHandler = EventHandler<ClipboardEvent>;
    type CompositionEventHandler = EventHandler<CompositionEvent>;
    type DragEventHandler = EventHandler<DragEvent>;
    type FocusEventHandler = EventHandler<FocusEvent>;
    type KeyboardEventHandler = EventHandler<KeyboardEvent>;
    type MouseEventHandler = EventHandler<MouseEvent>;
    type TouchEventHandler = EventHandler<TouchEvent>;
    type UIEventHandler = EventHandler<UIEvent>;
    type WheelEventHandler = EventHandler<WheelEvent>;
    type AnimationEventHandler = EventHandler<AnimationEvent>;
    type TransitionEventHandler = EventHandler<TransitionEvent>;
    type GenericEventHandler = EventHandler<Event>;
    /**
     * Interface defining all event handlers that can be
     * attached a DOM node.
     */
    interface DOMAttributes {
        onLoad?: GenericEventHandler;
        onCopy?: ClipboardEventHandler;
        onCut?: ClipboardEventHandler;
        onPaste?: ClipboardEventHandler;
        onCompositionEnd?: CompositionEventHandler;
        onCompositionStart?: CompositionEventHandler;
        onCompositionUpdate?: CompositionEventHandler;
        onFocus?: FocusEventHandler;
        onBlur?: FocusEventHandler;
        onChange?: GenericEventHandler;
        onInput?: GenericEventHandler;
        onSearch?: GenericEventHandler;
        onSubmit?: GenericEventHandler;
        onKeyDown?: KeyboardEventHandler;
        onKeyPress?: KeyboardEventHandler;
        onKeyUp?: KeyboardEventHandler;
        onAbort?: GenericEventHandler;
        onCanPlay?: GenericEventHandler;
        onCanPlayThrough?: GenericEventHandler;
        onDurationChange?: GenericEventHandler;
        onEmptied?: GenericEventHandler;
        onEncrypted?: GenericEventHandler;
        onEnded?: GenericEventHandler;
        onLoadedData?: GenericEventHandler;
        onLoadedMetadata?: GenericEventHandler;
        onLoadStart?: GenericEventHandler;
        onPause?: GenericEventHandler;
        onPlay?: GenericEventHandler;
        onPlaying?: GenericEventHandler;
        onProgress?: GenericEventHandler;
        onRateChange?: GenericEventHandler;
        onSeeked?: GenericEventHandler;
        onSeeking?: GenericEventHandler;
        onStalled?: GenericEventHandler;
        onSuspend?: GenericEventHandler;
        onTimeUpdate?: GenericEventHandler;
        onVolumeChange?: GenericEventHandler;
        onWaiting?: GenericEventHandler;
        onClick?: MouseEventHandler;
        onContextMenu?: MouseEventHandler;
        onDblClick?: MouseEventHandler;
        onDrag?: DragEventHandler;
        onDragEnd?: DragEventHandler;
        onDragEnter?: DragEventHandler;
        onDragExit?: DragEventHandler;
        onDragLeave?: DragEventHandler;
        onDragOver?: DragEventHandler;
        onDragStart?: DragEventHandler;
        onDrop?: DragEventHandler;
        onMouseDown?: MouseEventHandler;
        onMouseEnter?: MouseEventHandler;
        onMouseLeave?: MouseEventHandler;
        onMouseMove?: MouseEventHandler;
        onMouseOut?: MouseEventHandler;
        onMouseOver?: MouseEventHandler;
        onMouseUp?: MouseEventHandler;
        onSelect?: GenericEventHandler;
        onTouchCancel?: TouchEventHandler;
        onTouchEnd?: TouchEventHandler;
        onTouchMove?: TouchEventHandler;
        onTouchStart?: TouchEventHandler;
        onScroll?: UIEventHandler;
        onWheel?: WheelEventHandler;
        onAnimationStart?: AnimationEventHandler;
        onAnimationEnd?: AnimationEventHandler;
        onAnimationIteration?: AnimationEventHandler;
        onTransitionEnd?: TransitionEventHandler;
    }
    interface HTMLAttributes extends Q.VDomHtmlAttrs, DOMAttributes {
        accept?: string;
        acceptCharset?: string;
        accessKey?: string;
        action?: string;
        allowFullScreen?: boolean;
        allowTransparency?: boolean;
        alt?: string;
        as?: string;
        async?: boolean;
        autocomplete?: string;
        autofocus?: boolean;
        autoPlay?: boolean;
        capture?: boolean;
        cellPadding?: number | string;
        cellSpacing?: number | string;
        charSet?: string;
        challenge?: string;
        checked?: boolean;
        class?: string | {
            [key: string]: boolean;
        };
        className?: string | {
            [key: string]: boolean;
        };
        cols?: number;
        colSpan?: number;
        content?: string;
        contentEditable?: boolean;
        contextMenu?: string;
        controls?: boolean;
        coords?: string;
        crossOrigin?: string;
        data?: string;
        dateTime?: string;
        default?: boolean;
        defer?: boolean;
        dir?: string;
        disabled?: boolean;
        download?: any;
        draggable?: boolean;
        encType?: string;
        form?: string;
        formAction?: string;
        formEncType?: string;
        formMethod?: string;
        formNoValidate?: boolean;
        formTarget?: string;
        frameBorder?: number | string;
        headers?: string;
        height?: number | string;
        hidden?: boolean;
        high?: number;
        href?: string;
        hrefLang?: string;
        for?: string;
        httpEquiv?: string;
        icon?: string;
        id?: string;
        inputMode?: string;
        integrity?: string;
        is?: string;
        keyParams?: string;
        keyType?: string;
        kind?: string;
        label?: string;
        lang?: string;
        list?: string;
        loop?: boolean;
        low?: number;
        manifest?: string;
        marginHeight?: number;
        marginWidth?: number;
        max?: number | string;
        maxLength?: number;
        media?: string;
        mediaGroup?: string;
        method?: string;
        min?: number | string;
        minLength?: number;
        multiple?: boolean;
        muted?: boolean;
        name?: string;
        noValidate?: boolean;
        open?: boolean;
        optimum?: number;
        pattern?: string;
        placeholder?: string;
        poster?: string;
        preload?: string;
        radioGroup?: string;
        readOnly?: boolean;
        rel?: string;
        required?: boolean;
        role?: string;
        rows?: number;
        rowSpan?: number;
        sandbox?: string;
        scope?: string;
        scoped?: boolean;
        scrolling?: string;
        seamless?: boolean;
        selected?: boolean;
        shape?: string;
        size?: number;
        sizes?: string;
        slot?: string;
        span?: number;
        spellCheck?: boolean;
        src?: string;
        srcset?: string;
        srcDoc?: string;
        srcLang?: string;
        srcSet?: string;
        start?: number;
        step?: number | string;
        style?: any;
        summary?: string;
        tabIndex?: number;
        target?: string;
        title?: string;
        type?: string;
        useMap?: string;
        value?: string | string[];
        width?: number | string;
        wmode?: string;
        wrap?: string;
        about?: string;
        datatype?: string;
        inlist?: any;
        prefix?: string;
        property?: string;
        resource?: string;
        typeof?: string;
        vocab?: string;
    }
    interface IntrinsicElements {
        a: HTMLAttributes;
        abbr: HTMLAttributes;
        address: HTMLAttributes;
        area: HTMLAttributes;
        article: HTMLAttributes;
        aside: HTMLAttributes;
        audio: HTMLAttributes;
        b: HTMLAttributes;
        base: HTMLAttributes;
        bdi: HTMLAttributes;
        bdo: HTMLAttributes;
        big: HTMLAttributes;
        blockquote: HTMLAttributes;
        body: HTMLAttributes;
        br: HTMLAttributes;
        button: HTMLAttributes;
        canvas: HTMLAttributes;
        caption: HTMLAttributes;
        cite: HTMLAttributes;
        code: HTMLAttributes;
        col: HTMLAttributes;
        colgroup: HTMLAttributes;
        data: HTMLAttributes;
        datalist: HTMLAttributes;
        dd: HTMLAttributes;
        del: HTMLAttributes;
        details: HTMLAttributes;
        dfn: HTMLAttributes;
        dialog: HTMLAttributes;
        div: HTMLAttributes;
        dl: HTMLAttributes;
        dt: HTMLAttributes;
        em: HTMLAttributes;
        embed: HTMLAttributes;
        fieldset: HTMLAttributes;
        figcaption: HTMLAttributes;
        figure: HTMLAttributes;
        footer: HTMLAttributes;
        form: HTMLAttributes;
        h1: HTMLAttributes;
        h2: HTMLAttributes;
        h3: HTMLAttributes;
        h4: HTMLAttributes;
        h5: HTMLAttributes;
        h6: HTMLAttributes;
        head: HTMLAttributes;
        header: HTMLAttributes;
        hr: HTMLAttributes;
        html: HTMLAttributes;
        i: HTMLAttributes;
        iframe: HTMLAttributes;
        img: HTMLAttributes;
        input: HTMLAttributes;
        ins: HTMLAttributes;
        kbd: HTMLAttributes;
        keygen: HTMLAttributes;
        label: HTMLAttributes;
        legend: HTMLAttributes;
        li: HTMLAttributes;
        link: HTMLAttributes;
        main: HTMLAttributes;
        map: HTMLAttributes;
        mark: HTMLAttributes;
        menu: HTMLAttributes;
        menuitem: HTMLAttributes;
        meta: HTMLAttributes;
        meter: HTMLAttributes;
        nav: HTMLAttributes;
        noscript: HTMLAttributes;
        object: HTMLAttributes;
        ol: HTMLAttributes;
        optgroup: HTMLAttributes;
        option: HTMLAttributes;
        output: HTMLAttributes;
        p: HTMLAttributes;
        param: HTMLAttributes;
        picture: HTMLAttributes;
        pre: HTMLAttributes;
        progress: HTMLAttributes;
        q: HTMLAttributes;
        rp: HTMLAttributes;
        rt: HTMLAttributes;
        ruby: HTMLAttributes;
        s: HTMLAttributes;
        samp: HTMLAttributes;
        script: HTMLAttributes;
        section: HTMLAttributes;
        select: HTMLAttributes;
        slot: HTMLAttributes;
        small: HTMLAttributes;
        source: HTMLAttributes;
        span: HTMLAttributes;
        strong: HTMLAttributes;
        style: HTMLAttributes;
        sub: HTMLAttributes;
        summary: HTMLAttributes;
        sup: HTMLAttributes;
        table: HTMLAttributes;
        tbody: HTMLAttributes;
        td: HTMLAttributes;
        textarea: HTMLAttributes;
        tfoot: HTMLAttributes;
        th: HTMLAttributes;
        thead: HTMLAttributes;
        time: HTMLAttributes;
        title: HTMLAttributes;
        tr: HTMLAttributes;
        track: HTMLAttributes;
        u: HTMLAttributes;
        ul: HTMLAttributes;
        "var": HTMLAttributes;
        video: HTMLAttributes;
        wbr: HTMLAttributes;
        svg: SVGAttributes;
        animate: SVGAttributes;
        circle: SVGAttributes;
        clipPath: SVGAttributes;
        defs: SVGAttributes;
        ellipse: SVGAttributes;
        feBlend: SVGAttributes;
        feColorMatrix: SVGAttributes;
        feComponentTransfer: SVGAttributes;
        feComposite: SVGAttributes;
        feConvolveMatrix: SVGAttributes;
        feDiffuseLighting: SVGAttributes;
        feDisplacementMap: SVGAttributes;
        feFlood: SVGAttributes;
        feGaussianBlur: SVGAttributes;
        feImage: SVGAttributes;
        feMerge: SVGAttributes;
        feMergeNode: SVGAttributes;
        feMorphology: SVGAttributes;
        feOffset: SVGAttributes;
        feSpecularLighting: SVGAttributes;
        feTile: SVGAttributes;
        feTurbulence: SVGAttributes;
        filter: SVGAttributes;
        foreignObject: SVGAttributes;
        g: SVGAttributes;
        image: SVGAttributes;
        line: SVGAttributes;
        linearGradient: SVGAttributes;
        marker: SVGAttributes;
        mask: SVGAttributes;
        path: SVGAttributes;
        pattern: SVGAttributes;
        polygon: SVGAttributes;
        polyline: SVGAttributes;
        radialGradient: SVGAttributes;
        rect: SVGAttributes;
        stop: SVGAttributes;
        symbol: SVGAttributes;
        text: SVGAttributes;
        tspan: SVGAttributes;
        use: SVGAttributes;
    }
}
declare namespace Serenity {
    namespace Decorators {
        function registerClass(nameOrIntf?: string | any[], intf2?: any[]): (target: Function) => void;
        function registerInterface(nameOrIntf?: string | any[], intf2?: any[]): (target: Function) => void;
        function registerEditor(nameOrIntf?: string | any[], intf2?: any[]): (target: Function) => void;
    }
}
declare namespace System.ComponentModel {
    class DisplayNameAttribute {
        displayName: string;
        constructor(displayName: string);
    }
}
declare namespace Serenity {
    class ISlickFormatter {
    }
    class CategoryAttribute {
        category: string;
        constructor(category: string);
    }
    class CollapsibleAttribute {
        value: boolean;
        constructor(value: boolean);
        collapsed: boolean;
    }
    class ColumnsKeyAttribute {
        value: string;
        constructor(value: string);
    }
    class CssClassAttribute {
        cssClass: string;
        constructor(cssClass: string);
    }
    class DefaultValueAttribute {
        value: any;
        constructor(value: any);
    }
    class DialogTypeAttribute {
        value: Function;
        constructor(value: Function);
    }
    class EditorAttribute {
        constructor();
        key: string;
    }
    class EditorOptionAttribute {
        key: string;
        value: any;
        constructor(key: string, value: any);
    }
    class EditorTypeAttributeBase {
        editorType: string;
        constructor(editorType: string);
        setParams(editorParams: any): void;
    }
    class EditorTypeAttribute extends EditorTypeAttributeBase {
        constructor(editorType: string);
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
    class FilterableAttribute {
        value: boolean;
        constructor(value?: boolean);
    }
    class FormKeyAttribute {
        value: string;
        constructor(value: string);
    }
    class GeneratedCodeAttribute {
        origin: string;
        constructor(origin?: string);
    }
    class HiddenAttribute {
        constructor();
    }
    class HintAttribute {
        hint: string;
        constructor(hint: string);
    }
    class IdPropertyAttribute {
        value: string;
        constructor(value: string);
    }
    class InsertableAttribute {
        value: boolean;
        constructor(value?: boolean);
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
    class MaxLengthAttribute {
        maxLength: number;
        constructor(maxLength: number);
    }
    class NamePropertyAttribute {
        value: string;
        constructor(value: string);
    }
    class OneWayAttribute {
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
    class PlaceholderAttribute {
        value: string;
        constructor(value: string);
    }
    class ReadOnlyAttribute {
        value: boolean;
        constructor(value?: boolean);
    }
    class RequiredAttribute {
        isRequired: boolean;
        constructor(isRequired?: boolean);
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
    class UpdatableAttribute {
        value: boolean;
        constructor(value?: boolean);
    }
}
declare namespace Serenity.Decorators {
    function registerFormatter(nameOrIntf?: string | any[], intf2?: any[]): (target: Function) => void;
    function addAttribute(type: any, attr: any): void;
    function columnsKey(value: string): (target: Function) => void;
    function dialogType(value: Function): (target: Function) => void;
    function editor(key?: string): (target: Function) => void;
    function element(value: string): (target: Function) => void;
    function entityType(value: string): (target: Function) => void;
    function enumKey(value: string): (target: Function) => void;
    function flexify(value?: boolean): (target: Function) => void;
    function formKey(value: string): (target: Function) => void;
    function generatedCode(origin?: string): (target: Function) => void;
    function idProperty(value: string): (target: Function) => void;
    function registerEnum(target: any, enumKey?: string, name?: string): void;
    function registerEnumType(target: any, name?: string, enumKey?: string): void;
    function filterable(value?: boolean): (target: Function) => void;
    function itemName(value: string): (target: Function) => void;
    function isActiveProperty(value: string): (target: Function) => void;
    function localTextPrefix(value: string): (target: Function) => void;
    function maximizable(value?: boolean): (target: Function) => void;
    function nameProperty(value: string): (target: Function) => void;
    function option(): (target: Object, propertyKey: string) => void;
    function optionsType(value: Function): (target: Function) => void;
    function panel(value?: boolean): (target: Function) => void;
    function resizable(value?: boolean): (target: Function) => void;
    function responsive(value?: boolean): (target: Function) => void;
    function service(value: string): (target: Function) => void;
}
declare namespace Serenity {
    function Criteria(field: string): any[];
    namespace Criteria {
        function isEmpty(c: any[]): boolean;
        function join(c1: any[], op: string, c2: any[]): any[];
        function paren(c: any[]): any[];
        function and(c1: any[], c2: any[], ...rest: any[][]): any[];
        function or(c1: any[], c2: any[], ...rest: any[][]): any[];
    }
}
declare namespace Serenity {
    interface QuickSearchField {
        name: string;
        title: string;
    }
    class GridRowSelectionMixin {
        private idField;
        private include;
        private grid;
        constructor(grid: IDataGrid);
        updateSelectAll(): void;
        clear(): void;
        resetCheckedAndRefresh(): void;
        selectKeys(keys: string[]): void;
        getSelectedKeys(): string[];
        getSelectedAsInt32(): number[];
        getSelectedAsInt64(): number[];
        setSelectedKeys(keys: string[]): void;
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
        function addQuickSearchInputCustom(container: JQuery, onSearch: (p1: string, p2: string, done: (p3: boolean) => void) => void, fields?: QuickSearchField[]): void;
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
    namespace LazyLoadHelper {
        function executeOnceWhenShown(element: JQuery, callback: Function): void;
        function executeEverytimeWhenShown(element: JQuery, callback: Function, callNowIfVisible: boolean): void;
    }
}
declare namespace Serenity {
    namespace SubDialogHelper {
        function bindToDataChange(dialog: any, owner: Serenity.Widget<any>, dataChange: (p1: any, p2: DataChangeInfo) => void, useTimeout?: boolean): any;
        function triggerDataChange(dialog: Serenity.Widget<any>): any;
        function triggerDataChanged(element: JQuery): JQuery;
        function bubbleDataChange(dialog: any, owner: Serenity.Widget<any>, useTimeout?: boolean): any;
        function cascade(cascadedDialog: any, ofElement: JQuery): any;
        function cascadedDialogOffset(element: JQuery): any;
    }
}
declare namespace Serenity {
    namespace TabsExtensions {
        function setDisabled(tabs: JQuery, tabKey: string, isDisabled: boolean): void;
        function activeTabKey(tabs: JQuery): string;
        function indexByKey(tabs: JQuery): any;
        function selectTab(tabs: JQuery, tabKey: string): void;
    }
}
declare namespace Serenity {
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
    interface UploadedFile {
        Filename?: string;
        OriginalName?: string;
    }
    interface UploadInputOptions {
        container?: JQuery;
        zone?: JQuery;
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
}
declare namespace Serenity {
    class IAsyncInit {
    }
    class Widget<TOptions> {
        private static nextWidgetNumber;
        element: JQuery;
        protected options: TOptions;
        protected widgetName: string;
        protected uniqueName: string;
        protected asyncPromise: PromiseLike<void>;
        constructor(element: JQuery, options?: TOptions);
        destroy(): void;
        protected addCssClass(): void;
        protected getCssClass(): string;
        protected initializeAsync(): PromiseLike<void>;
        protected isAsyncWidget(): boolean;
        static getWidgetName(type: Function): string;
        static elementFor<TWidget>(editorType: {
            new (...args: any[]): TWidget;
        }): JQuery;
        static create<TWidget extends Widget<TOpt>, TOpt>(params: CreateWidgetParams<TWidget, TOpt>): TWidget;
        init(action?: (widget: any) => void): this;
        initialize(): PromiseLike<void>;
    }
    interface Widget<TOptions> {
        addValidationRule(eventClass: string, rule: (p1: JQuery) => string): JQuery;
        getGridField(): JQuery;
        change(handler: (e: JQueryEventObject) => void): void;
        changeSelect2(handler: (e: JQueryEventObject) => void): void;
    }
}
declare namespace Serenity {
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
}
declare namespace Serenity {
    class TemplatedWidget<TOptions> extends Widget<TOptions> {
        protected idPrefix: string;
        private static templateNames;
        constructor(container: JQuery, options?: TOptions);
        protected byId(id: string): JQuery;
        private byID<TWidget>(id, type);
        private static noGeneric(s);
        private getDefaultTemplateName();
        protected getTemplateName(): string;
        protected getFallbackTemplate(): string;
        protected getTemplate(): string;
    }
}
declare namespace Serenity {
    class IBooleanValue {
    }
    interface IBooleanValue {
        get_value(): boolean;
        set_value(value: boolean): void;
    }
}
declare namespace Serenity {
    class IDoubleValue {
    }
    interface IDoubleValue {
        get_value(): any;
        set_value(value: any): void;
    }
}
declare namespace Serenity {
    class IGetEditValue {
    }
    interface IGetEditValue {
        getEditValue(property: PropertyItem, target: any): void;
    }
}
declare namespace Serenity {
    class ISetEditValue {
    }
    interface ISetEditValue {
        setEditValue(source: any, property: PropertyItem): void;
    }
}
declare namespace Serenity {
    class IStringValue {
    }
    interface IStringValue {
        get_value(): string;
        set_value(value: string): void;
    }
}
declare namespace Serenity {
    interface IReadOnly {
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
    }
    class IReadOnly {
    }
}
declare namespace Serenity {
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
        formatterType?: string;
        formatterParams?: any;
        displayFormat?: string;
        alignment?: string;
        width?: number;
        minWidth?: number;
        maxWidth?: number;
        labelWidth?: string;
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
        quickFilterSeparator?: boolean;
        quickFilterCssClass?: string;
    }
}
declare namespace Serenity {
    class DateEditor extends Widget<any> implements IStringValue, IReadOnly {
        private minValue;
        private maxValue;
        private minDate;
        private maxDate;
        private sqlMinMax;
        constructor(input: JQuery);
        get_value(): string;
        value: string;
        set_value(value: string): void;
        private get_valueAsDate();
        valueAsDate: Date;
        private set_valueAsDate(value);
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
        yearRange: string;
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
        static dateInputChange: (e: JQueryEventObject) => void;
        static dateInputKeyup(e: JQueryEventObject): void;
    }
}
declare namespace Serenity {
    class DateTimeEditor extends Widget<DateTimeEditorOptions> implements IStringValue, IReadOnly {
        private minValue;
        private maxValue;
        private minDate;
        private maxDate;
        private sqlMinMax;
        private time;
        constructor(input: JQuery, opt?: DateTimeEditorOptions);
        get_value(): string;
        value: string;
        set_value(value: string): void;
        private get_valueAsDate();
        valueAsDate: Date;
        private set_valueAsDate(value);
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
        static roundToMinutes(date: Date, minutesStep: number): Date;
        static getTimeOptions: (fromHour: number, fromMin: number, toHour: number, toMin: number, stepMins: number) => string[];
    }
    interface DateTimeEditorOptions {
        startHour?: any;
        endHour?: any;
        intervalMinutes?: any;
        yearRange?: string;
        useUtc?: boolean;
    }
}
declare namespace Serenity {
    class Select2Editor<TOptions, TItem> extends Widget<TOptions> implements Serenity.ISetEditValue, Serenity.IGetEditValue, Serenity.IStringValue, Serenity.IReadOnly {
        items: Select2Item[];
        protected multiple: boolean;
        protected itemById: Q.Dictionary<Select2Item>;
        protected pageSize: number;
        protected lastCreateTerm: string;
        constructor(hidden: JQuery, opt?: any);
        destroy(): void;
        protected emptyItemText(): any;
        protected getSelect2Options(): Select2Options;
        get_delimited(): boolean;
        protected clearItems(): void;
        protected addItem(item: Select2Item): void;
        protected addOption(key: string, text: string, source?: any, disabled?: boolean): void;
        protected addInplaceCreate(addTitle: string, editTitle: string): void;
        protected inplaceCreateClick(e: JQueryEventObject): void;
        protected isAutoComplete(): boolean;
        getCreateSearchChoice(getName: (z: any) => string): (s: string) => {
            id: string;
            text: string;
        };
        setEditValue(source: any, property: PropertyItem): void;
        getEditValue(property: PropertyItem, target: any): void;
        protected get_select2Container(): JQuery;
        protected get_items(): Select2Item[];
        protected get_itemByKey(): Q.Dictionary<Select2Item>;
        get_value(): any;
        value: string;
        set_value(value: string): void;
        protected get_values(): string[];
        values: string[];
        protected set_values(value: string[]): void;
        protected get_text(): string;
        readonly text: string;
        get_readOnly(): boolean;
        readOnly: boolean;
        set_readOnly(value: boolean): void;
    }
    interface Select2Item {
        id: string;
        text: string;
        source?: any;
        disabled?: boolean;
    }
    class SelectEditor extends Select2Editor<SelectEditorOptions, Select2Item> {
        constructor(hidden: JQuery, opt?: SelectEditorOptions);
        getItems(): any[];
        protected emptyItemText(): any;
        updateItems(): void;
    }
    interface SelectEditorOptions {
        items?: any[];
        emptyOptionText?: string;
    }
}
declare namespace Serenity {
    class DateYearEditor extends SelectEditor {
        constructor(hidden: JQuery, opt: DateYearEditorOptions);
        getItems(): any[];
    }
    interface DateYearEditorOptions extends SelectEditorOptions {
        minYear?: string;
        maxYear?: string;
        descending?: boolean;
    }
}
declare namespace Serenity {
    interface LookupEditorOptions {
        lookupKey?: string;
        minimumResultsForSearch?: any;
        autoComplete?: boolean;
        inplaceAdd?: boolean;
        inplaceAddPermission?: string;
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
        protected initializeAsync(): PromiseLike<void>;
        destroy(): void;
        protected getLookupKey(): string;
        protected getLookup(): Q.Lookup<TItem>;
        protected getLookupAsync(): PromiseLike<Q.Lookup<TItem>>;
        protected getItems(lookup: Q.Lookup<TItem>): TItem[];
        protected getItemText(item: TItem, lookup: Q.Lookup<TItem>): any;
        protected getItemDisabled(item: TItem, lookup: Q.Lookup<TItem>): boolean;
        updateItems(): void;
        updateItemsAsync(): PromiseLike<void>;
        protected getDialogTypeKey(): string;
        protected createEditDialog(callback: (dlg: IEditDialog) => void): void;
        onInitNewEntity: (entity: TItem) => void;
        protected initNewEntity(entity: TItem): void;
        protected inplaceCreateClick(e: JQueryEventObject): void;
        protected cascadeItems(items: TItem[]): TItem[];
        protected filterItems(items: TItem[]): TItem[];
        protected getCascadeFromValue(parent: Serenity.Widget<any>): any;
        protected cascadeLink: Serenity.CascadedWidgetLink<Widget<any>>;
        protected setCascadeFrom(value: string): void;
        protected isAutoComplete(): boolean;
        protected getSelect2Options(): Select2Options;
        protected get_cascadeFrom(): string;
        cascadeFrom: string;
        protected set_cascadeFrom(value: string): void;
        protected get_cascadeField(): any;
        cascadeField: string;
        protected set_cascadeField(value: string): void;
        protected get_cascadeValue(): any;
        cascadeValue: any;
        protected set_cascadeValue(value: any): void;
        protected get_filterField(): string;
        filterField: string;
        protected set_filterField(value: string): void;
        protected get_filterValue(): any;
        filterValue: any;
        protected set_filterValue(value: any): void;
        openDialogAsPanel: boolean;
    }
    class LookupEditor extends LookupEditorBase<LookupEditorOptions, any> {
        constructor(hidden: JQuery, opt?: LookupEditorOptions);
    }
}
declare namespace Serenity {
    namespace EditorTypeRegistry {
        function get(key: string): Function;
        function reset(): void;
    }
    namespace EditorUtils {
        function getDisplayText(editor: Serenity.Widget<any>): string;
        function getValue(editor: Serenity.Widget<any>): any;
        function saveValue(editor: Serenity.Widget<any>, item: PropertyItem, target: any): void;
        function setValue(editor: Serenity.Widget<any>, value: any): void;
        function loadValue(editor: Serenity.Widget<any>, item: PropertyItem, source: any): void;
        function setReadonly(elements: JQuery, isReadOnly: boolean): JQuery;
        function setReadOnly(widget: Serenity.Widget<any>, isReadOnly: boolean): void;
        function setRequired(widget: Serenity.Widget<any>, isRequired: boolean): void;
    }
    class BooleanEditor extends Widget<any> {
        constructor(input: JQuery);
        value: boolean;
        protected get_value(): boolean;
        protected set_value(value: boolean): void;
    }
    class DecimalEditor extends Widget<DecimalEditorOptions> implements IDoubleValue {
        constructor(input: JQuery, opt?: DecimalEditorOptions);
        get_value(): number;
        value: number;
        set_value(value: number): void;
        get_isValid(): boolean;
        static defaultAutoNumericOptions(): any;
    }
    interface IntegerEditorOptions {
        minValue?: number;
        maxValue?: number;
    }
    class IntegerEditor extends Widget<IntegerEditorOptions> implements IDoubleValue {
        constructor(input: JQuery, opt?: IntegerEditorOptions);
        get_value(): number;
        value: number;
        set_value(value: number): void;
        get_isValid(): boolean;
    }
    interface DecimalEditorOptions {
        minValue?: string;
        maxValue?: string;
        decimals?: any;
        padDecimals?: any;
    }
    interface EmailEditorOptions {
        domain?: string;
        readOnlyDomain?: boolean;
    }
    class EmailEditor extends Widget<EmailEditorOptions> {
        constructor(input: JQuery, opt: EmailEditorOptions);
        static registerValidationMethods(): void;
        get_value(): string;
        value: string;
        set_value(value: string): void;
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
    }
    interface EnumEditorOptions {
        enumKey?: string;
        enumType?: any;
        allowClear?: boolean;
    }
    class EnumEditor extends Select2Editor<EnumEditorOptions, Select2Item> {
        constructor(hidden: JQuery, opt: EnumEditorOptions);
        protected updateItems(): void;
        protected getSelect2Options(): Select2Options;
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
    class GoogleMap extends Widget<GoogleMapOptions> {
        private map;
        constructor(container: JQuery, opt: GoogleMapOptions);
        get_map(): any;
    }
    interface HtmlContentEditorOptions {
        cols?: any;
        rows?: any;
    }
    class HtmlContentEditor extends Widget<HtmlContentEditorOptions> implements IStringValue, IReadOnly {
        private _instanceReady;
        constructor(textArea: JQuery, opt?: HtmlContentEditorOptions);
        protected instanceReady(x: any): void;
        protected getLanguage(): string;
        protected getConfig(): CKEditorConfig;
        protected getEditorInstance(): any;
        destroy(): void;
        get_value(): string;
        value: string;
        set_value(value: string): void;
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
        static includeCKEditor(): void;
    }
    class HtmlNoteContentEditor extends HtmlContentEditor {
        constructor(textArea: JQuery, opt?: HtmlContentEditorOptions);
        protected getConfig(): CKEditorConfig;
    }
    class HtmlReportContentEditor extends HtmlContentEditor {
        constructor(textArea: JQuery, opt?: HtmlContentEditorOptions);
        protected getConfig(): CKEditorConfig;
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
        displayFileName?: boolean;
    }
    class ImageUploadEditor extends Widget<ImageUploadEditorOptions> implements IReadOnly, IGetEditValue, ISetEditValue {
        constructor(div: JQuery, opt: ImageUploadEditorOptions);
        protected getUploadInputOptions(): UploadInputOptions;
        protected addFileButtonText(): string;
        protected getToolButtons(): ToolButton[];
        protected populate(): void;
        protected updateInterface(): void;
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
        get_value(): UploadedFile;
        value: UploadedFile;
        set_value(value: UploadedFile): void;
        getEditValue(property: PropertyItem, target: any): void;
        setEditValue(source: any, property: PropertyItem): void;
        protected entity: UploadedFile;
        protected toolbar: Toolbar;
        protected progress: JQuery;
        protected fileSymbols: JQuery;
        protected uploadInput: JQuery;
    }
    class MultipleImageUploadEditor extends Widget<ImageUploadEditorOptions> implements IReadOnly, IGetEditValue, ISetEditValue {
        private entities;
        private toolbar;
        private fileSymbols;
        private uploadInput;
        constructor(div: JQuery, opt: ImageUploadEditorOptions);
        protected addFileButtonText(): string;
        protected getToolButtons(): ToolButton[];
        protected populate(): void;
        protected updateInterface(): void;
        get_readOnly(): boolean;
        set_readOnly(value: boolean): void;
        get_value(): UploadedFile[];
        value: UploadedFile[];
        set_value(value: UploadedFile[]): void;
        getEditValue(property: PropertyItem, target: any): void;
        setEditValue(source: any, property: PropertyItem): void;
        jsonEncodeValue: boolean;
    }
    class MaskedEditor extends Widget<MaskedEditorOptions> {
        constructor(input: JQuery, opt?: MaskedEditorOptions);
        value: string;
        protected get_value(): string;
        protected set_value(value: string): void;
    }
    interface MaskedEditorOptions {
        mask?: string;
        placeholder?: string;
    }
    class StringEditor extends Widget<any> {
        constructor(input: JQuery);
        value: string;
        protected get_value(): string;
        protected set_value(value: string): void;
    }
    class EmailAddressEditor extends Serenity.StringEditor {
        constructor(input: JQuery);
    }
    class PasswordEditor extends StringEditor {
        constructor(input: JQuery);
    }
    interface RadioButtonEditorOptions {
        enumKey?: string;
        enumType?: any;
        lookupKey?: string;
    }
    class RadioButtonEditor extends Widget<RadioButtonEditorOptions> {
        constructor(input: JQuery, opt: RadioButtonEditorOptions);
        protected addRadio(value: string, text: string): void;
        get_value(): string;
        value: string;
        set_value(value: string): void;
    }
    interface RecaptchaOptions {
        siteKey?: string;
        language?: string;
    }
    class Recaptcha extends Widget<RecaptchaOptions> implements IStringValue {
        constructor(div: JQuery, opt: RecaptchaOptions);
        get_value(): string;
        set_value(value: string): void;
    }
    interface TextAreaEditorOptions {
        cols?: number;
        rows?: number;
    }
    class TextAreaEditor extends Widget<TextAreaEditorOptions> {
        constructor(input: JQuery, opt?: TextAreaEditorOptions);
        value: string;
        protected get_value(): string;
        protected set_value(value: string): void;
    }
    interface TimeEditorOptions {
        noEmptyOption?: boolean;
        startHour?: any;
        endHour?: any;
        intervalMinutes?: any;
    }
    class TimeEditor extends Widget<TimeEditorOptions> {
        private minutes;
        constructor(input: JQuery, opt?: TimeEditorOptions);
        value: number;
        protected get_value(): number;
        protected set_value(value: number): void;
    }
    class URLEditor extends StringEditor {
        constructor(input: JQuery);
    }
    class Select2AjaxEditor<TOptions, TItem> extends Widget<TOptions> implements IStringValue {
        pageSize: number;
        constructor(hidden: JQuery, opt: TOptions);
        protected emptyItemText(): string;
        protected getService(): string;
        protected query(request: ListRequest, callback: (p1: ListResponse<any>) => void): void;
        protected executeQuery(options: ServiceOptions<ListResponse<any>>): void;
        protected queryByKey(key: string, callback: (p1: any) => void): void;
        protected executeQueryByKey(options: ServiceOptions<RetrieveResponse<any>>): void;
        protected getItemKey(item: any): string;
        protected getItemText(item: any): string;
        protected getTypeDelay(): number;
        protected getSelect2Options(): Select2Options;
        protected addInplaceCreate(title: string): void;
        protected inplaceCreateClick(e: any): void;
        protected get_select2Container(): JQuery;
        get_value(): string;
        value: string;
        set_value(value: string): void;
    }
}
declare namespace Serenity {
    interface FilterOperator {
        key?: string;
        title?: string;
        format?: string;
    }
    namespace FilterOperators {
        const isTrue = "true";
        const isFalse = "false";
        const contains = "contains";
        const startsWith = "startswith";
        const EQ = "eq";
        const NE = "ne";
        const GT = "gt";
        const GE = "ge";
        const LT = "lt";
        const LE = "le";
        const BW = "bw";
        const IN = "in";
        const isNull = "isnull";
        const isNotNull = "isnotnull";
        const toCriteriaOperator: Q.Dictionary<string>;
    }
}
declare namespace Serenity {
    interface FilterLine {
        field?: string;
        operator?: string;
        isOr?: boolean;
        leftParen?: boolean;
        rightParen?: boolean;
        validationError?: string;
        criteria?: any[];
        displayText?: string;
        state?: any;
    }
}
declare namespace Serenity {
    class FilterStore {
        constructor(fields: PropertyItem[]);
        static getCriteriaFor(items: FilterLine[]): any[];
        static getDisplayTextFor(items: FilterLine[]): string;
        private changed;
        private displayText;
        private fields;
        private fieldByName;
        private items;
        get_fields(): PropertyItem[];
        get_fieldByName(): Q.Dictionary<PropertyItem>;
        get_items(): FilterLine[];
        raiseChanged(): void;
        add_changed(value: (e: JQueryEventObject, a: any) => void): void;
        remove_changed(value: (e: JQueryEventObject, a: any) => void): void;
        get_activeCriteria(): any[];
        get_displayText(): string;
    }
}
declare namespace Serenity {
    interface IFiltering {
        createEditor(): void;
        getCriteria(): CriteriaWithText;
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
    class IFiltering {
    }
    interface CriteriaWithText {
        criteria?: any[];
        displayText?: string;
    }
    interface IQuickFiltering {
        initQuickFilter(filter: QuickFilter<Widget<any>, any>): void;
    }
    class IQuickFiltering {
    }
    abstract class BaseFiltering implements IFiltering, IQuickFiltering {
        private field;
        get_field(): PropertyItem;
        set_field(value: PropertyItem): void;
        private container;
        get_container(): JQuery;
        set_container(value: JQuery): void;
        private operator;
        get_operator(): FilterOperator;
        set_operator(value: FilterOperator): void;
        abstract getOperators(): FilterOperator[];
        protected appendNullableOperators(list: FilterOperator[]): FilterOperator[];
        protected appendComparisonOperators(list: FilterOperator[]): FilterOperator[];
        protected isNullable(): boolean;
        createEditor(): void;
        protected operatorFormat(op: FilterOperator): any;
        protected getTitle(field: PropertyItem): any;
        protected displayText(op: FilterOperator, values?: any[]): string;
        protected getCriteriaField(): string;
        getCriteria(): CriteriaWithText;
        loadState(state: any): void;
        saveState(): any;
        protected argumentNull(): any;
        validateEditorValue(value: string): string;
        getEditorValue(): string;
        getEditorText(): any;
        initQuickFilter(filter: QuickFilter<Widget<any>, any>): void;
    }
    abstract class BaseEditorFiltering<TEditor extends Widget<any>> extends BaseFiltering {
        editorType: any;
        constructor(editorType: any);
        protected useEditor(): boolean;
        protected editor: TEditor;
        createEditor(): void;
        protected useIdField(): boolean;
        getCriteriaField(): string;
        getEditorOptions(): any;
        loadState(state: any): void;
        saveState(): any;
        getEditorValue(): any;
        initQuickFilter(filter: QuickFilter<Widget<any>, any>): void;
    }
    class DateFiltering extends BaseEditorFiltering<DateEditor> {
        constructor();
        getOperators(): FilterOperator[];
    }
    class BooleanFiltering extends BaseFiltering {
        getOperators(): FilterOperator[];
    }
    class DateTimeFiltering extends BaseEditorFiltering<DateEditor> {
        constructor();
        getOperators(): FilterOperator[];
        getCriteria(): CriteriaWithText;
    }
    class DecimalFiltering extends BaseEditorFiltering<DecimalEditor> {
        constructor();
        getOperators(): Serenity.FilterOperator[];
    }
    class EditorFiltering extends BaseEditorFiltering<Serenity.Widget<any>> {
        constructor();
        editorType: string;
        useRelative: boolean;
        useLike: boolean;
        getOperators(): Serenity.FilterOperator[];
        protected useEditor(): boolean;
        getEditorOptions(): any;
        createEditor(): void;
        protected useIdField(): boolean;
        initQuickFilter(filter: QuickFilter<Widget<any>, any>): void;
    }
    class EnumFiltering extends BaseEditorFiltering<EnumEditor> {
        constructor();
        getOperators(): FilterOperator[];
    }
    class IntegerFiltering extends BaseEditorFiltering<IntegerEditor> {
        constructor();
        getOperators(): FilterOperator[];
    }
    class LookupFiltering extends BaseEditorFiltering<LookupEditor> {
        constructor();
        getOperators(): FilterOperator[];
        protected useEditor(): boolean;
        protected useIdField(): boolean;
        getEditorText(): string;
    }
    class StringFiltering extends BaseFiltering {
        getOperators(): Serenity.FilterOperator[];
        validateEditorValue(value: string): string;
    }
    namespace FilteringTypeRegistry {
        function get(key: string): Function;
    }
}
declare namespace Serenity {
    class FilterWidgetBase<TOptions> extends TemplatedWidget<TOptions> {
        private store;
        private onFilterStoreChanged;
        constructor(div: JQuery, opt?: TOptions);
        destroy(): void;
        protected filterStoreChanged(): void;
        get_store(): FilterStore;
        set_store(value: FilterStore): void;
    }
}
declare namespace Serenity {
    class FilterDisplayBar extends FilterWidgetBase<any> {
        constructor(div: JQuery);
        protected filterStoreChanged(): void;
        protected getTemplate(): string;
    }
}
declare namespace Serenity {
    class FilterPanel extends FilterWidgetBase<any> {
        private rowsDiv;
        constructor(div: JQuery);
        private showInitialLine;
        get_showInitialLine(): boolean;
        set_showInitialLine(value: boolean): void;
        protected filterStoreChanged(): void;
        updateRowsFromStore(): void;
        private showSearchButton;
        get_showSearchButton(): boolean;
        set_showSearchButton(value: boolean): void;
        private updateStoreOnReset;
        get_updateStoreOnReset(): boolean;
        set_updateStoreOnReset(value: boolean): void;
        protected getTemplate(): string;
        protected initButtons(): void;
        protected searchButtonClick(e: JQueryEventObject): void;
        get_hasErrors(): boolean;
        search(): void;
        protected addButtonClick(e: JQueryEventObject): void;
        protected resetButtonClick(e: JQueryEventObject): void;
        protected findEmptyRow(): JQuery;
        protected addEmptyRow(popupField: boolean): JQuery;
        protected onRowFieldChange(e: JQueryEventObject): void;
        protected rowFieldChange(row: JQuery): void;
        protected removeFiltering(row: JQuery): void;
        protected populateOperatorList(row: JQuery): void;
        protected getFieldFor(row: JQuery): PropertyItem;
        protected getFilteringFor(row: JQuery): IFiltering;
        protected onRowOperatorChange(e: JQueryEventObject): void;
        protected rowOperatorChange(row: JQuery): void;
        protected deleteRowClick(e: JQueryEventObject): void;
        protected updateButtons(): void;
        protected andOrClick(e: JQueryEventObject): void;
        protected leftRightParenClick(e: JQueryEventObject): void;
        protected updateParens(): void;
    }
}
declare namespace Serenity {
    interface QuickFilterArgs<TWidget> {
        field?: string;
        widget?: TWidget;
        request?: ListRequest;
        equalityFilter?: any;
        value?: any;
        active?: boolean;
        handled?: boolean;
    }
    interface QuickFilter<TWidget extends Widget<TOptions>, TOptions> {
        field?: string;
        type?: new (element: JQuery, options: TOptions) => TWidget;
        handler?: (h: QuickFilterArgs<TWidget>) => void;
        title?: string;
        options?: TOptions;
        element?: (e: JQuery) => void;
        init?: (w: TWidget) => void;
        separator?: boolean;
        cssClass?: string;
        loadState?: (w: TWidget, state: any) => void;
        saveState?: (w: TWidget) => any;
        displayText?: (w: TWidget, label: string) => string;
    }
}
declare namespace Serenity {
    interface QuickSearchInputOptions {
        typeDelay?: number;
        loadingParentClass?: string;
        filteredParentClass?: string;
        onSearch?: (p1: string, p2: string, p3: (p1: boolean) => void) => void;
        fields?: QuickSearchField[];
    }
    class QuickSearchInput extends Widget<QuickSearchInputOptions> {
        private lastValue;
        private field;
        private fieldChanged;
        private timer;
        constructor(input: JQuery, opt: QuickSearchInputOptions);
        protected checkIfValueChanged(): void;
        get_field(): QuickSearchField;
        set_field(value: QuickSearchField): void;
        protected updateInputPlaceHolder(): void;
        protected searchNow(value: string): void;
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
        static getName(enumType: any, value: any): any;
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
    class Flexify extends Widget<FlexifyOptions> {
        private xDifference;
        private yDifference;
        constructor(container: JQuery, options: FlexifyOptions);
        storeInitialSize(): void;
        private getXFactor(element);
        private getYFactor(element);
        private resizeElements();
        private resizeElement(element);
    }
    interface FlexifyOptions {
        getXFactor?: (p1: JQuery) => any;
        getYFactor?: (p1: JQuery) => any;
        designWidth?: any;
        designHeight?: any;
    }
}
declare namespace Serenity {
    namespace EnumTypeRegistry {
        function tryGet(key: string): Function;
        function get(key: string): Function;
    }
}
declare namespace Serenity {
    class PrefixedContext {
        readonly idPrefix: string;
        constructor(idPrefix: string);
        byId(id: string): JQuery;
        w<TWidget>(id: string, type: {
            new (...args: any[]): TWidget;
        }): TWidget;
    }
    namespace ReflectionOptionsSetter {
        function set(target: any, options: any): void;
    }
    namespace ReflectionUtils {
        function getPropertyValue(o: any, property: string): any;
        function setPropertyValue(o: any, property: string, value: any): void;
        function makeCamelCase(s: string): string;
    }
    interface ScriptContext {
    }
    class ScriptContext {
    }
}
declare namespace Serenity {
    const enum RetrieveColumnSelection {
        details = 0,
        keyOnly = 1,
        list = 2,
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
        Details = 2,
    }
    interface HtmlContentEditorOptions {
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
    interface SettingStorage {
        getItem(key: string): string;
        setItem(key: string, value: string): void;
    }
    interface CKEditorConfig {
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
    interface DialogButton {
        text: string;
        click: () => void;
    }
    interface EditorTypeInfo {
        type?: Function;
        displayName?: string;
        optionsType?: Function;
    }
}
declare namespace Serenity {
    namespace WX {
        function getWidget<TWidget>(element: JQuery, type: Function): any;
        function getWidgetName(type: Function): string;
        function hasOriginalEvent(e: any): boolean;
        function change(widget: any, handler: any): void;
        function changeSelect2(widget: any, handler: any): void;
        function getGridField(widget: Serenity.Widget<any>): JQuery;
    }
}
declare namespace Serenity {
    class PropertyGrid extends Widget<PropertyGridOptions> {
        private editors;
        private items;
        constructor(div: JQuery, opt: PropertyGridOptions);
        destroy(): void;
        private createItems(container, items);
        private createCategoryDiv(categoriesDiv, categoryIndexes, category, collapsed);
        private categoryLinkClick;
        private determineText(text, getKey);
        private createField(container, item);
        private getCategoryOrder(items);
        private createCategoryLinks(container, items);
        get_editors(): Widget<any>[];
        get_items(): PropertyItem[];
        get_idPrefix(): string;
        get_mode(): PropertyGridMode;
        set_mode(value: PropertyGridMode): void;
        static loadEditorValue(editor: Serenity.Widget<any>, item: PropertyItem, source: any): void;
        static saveEditorValue(editor: Serenity.Widget<any>, item: PropertyItem, target: any): void;
        private static setReadOnly(widget, isReadOnly);
        private static setReadonly(elements, isReadOnly);
        private static setRequired(widget, isRequired);
        private static setMaxLength(widget, maxLength);
        load(source: any): void;
        save(target: any): void;
        private canModifyItem(item);
        updateInterface(): void;
        enumerateItems(callback: (p1: PropertyItem, p2: Serenity.Widget<any>) => void): void;
    }
    const enum PropertyGridMode {
        insert = 1,
        update = 2,
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
    namespace PropertyItemHelper {
        function getPropertyItemsFor(type: Function): PropertyItem[];
    }
}
declare namespace Serenity {
    interface ToolButton {
        title?: string;
        hint?: string;
        cssClass?: string;
        icon?: string;
        onClick?: any;
        htmlEncode?: any;
        hotkey?: string;
        hotkeyAllowDefault?: boolean;
        hotkeyContext?: any;
        separator?: boolean;
        disabled?: boolean;
    }
    interface PopupMenuButtonOptions {
        menu?: JQuery;
        onPopup?: () => void;
        positionMy?: string;
        positionAt?: string;
    }
    class PopupMenuButton extends Widget<PopupMenuButtonOptions> {
        constructor(div: JQuery, opt: PopupMenuButtonOptions);
        destroy(): void;
    }
    interface PopupToolButtonOptions extends PopupMenuButtonOptions {
    }
    class PopupToolButton extends PopupMenuButton {
        constructor(div: JQuery, opt: PopupToolButtonOptions);
    }
    interface ToolbarOptions {
        buttons?: ToolButton[];
        hotkeyContext?: any;
    }
}
declare namespace Serenity {
    interface CreateWidgetParams<TWidget extends Widget<TOptions>, TOptions> {
        type?: new (element: JQuery, options?: TOptions) => TWidget;
        options?: TOptions;
        container?: JQuery;
        element?: (e: JQuery) => void;
        init?: (w: TWidget) => void;
    }
}
declare namespace Serenity {
    namespace CustomValidation {
        function registerValidationMethods(): void;
    }
    type CustomValidationRule = (element: JQuery) => string;
}
declare namespace Serenity {
    interface IValidateRequired {
        get_required(): boolean;
        set_required(value: boolean): void;
    }
    class IValidateRequired {
    }
}
declare namespace Serenity {
    interface DataChangeInfo {
        type: string;
        entityId: any;
        entity: any;
    }
}
declare namespace Serenity {
    class IDialog {
    }
    interface IDialog {
        dialogOpen(asPanel?: boolean): void;
    }
    class TemplatedDialog<TOptions> extends TemplatedWidget<TOptions> {
        protected tabs: JQuery;
        protected toolbar: Serenity.Toolbar;
        protected validator: JQueryValidation.Validator;
        constructor(options?: TOptions);
        private readonly isMarkedAsPanel;
        private readonly isResponsive;
        private static getCssSize(element, name);
        private static applyCssSizes(opt, dialogClass);
        destroy(): void;
        protected initDialog(): void;
        protected initToolbar(): void;
        protected getToolbarButtons(): ToolButton[];
        protected getValidatorOptions(): JQueryValidation.ValidationOptions;
        protected initValidator(): void;
        protected resetValidation(): void;
        protected validateForm(): boolean;
        dialogOpen(asPanel?: boolean): void;
        static openPanel(element: JQuery, uniqueName: string): void;
        static closePanel(element: JQuery, e?: JQueryEventObject): void;
        protected onDialogOpen(): void;
        protected arrange(): void;
        protected onDialogClose(): void;
        protected addCssClass(): void;
        protected getDialogOptions(): JQueryUI.DialogOptions;
        protected getDialogTitle(): string;
        dialogClose(): void;
        dialogTitle: string;
        private setupPanelTitle();
        set_dialogTitle(value: string): void;
        protected initTabs(): void;
        protected handleResponsive(): void;
    }
}
declare namespace Serenity {
    class PropertyDialog<TItem, TOptions> extends TemplatedDialog<TOptions> {
        protected _entity: TItem;
        protected _entityId: any;
        constructor(opt?: TOptions);
        destroy(): void;
        protected initPropertyGridAsync(): PromiseLike<void>;
        protected getDialogOptions(): JQueryUI.DialogOptions;
        protected getDialogButtons(): JQueryUI.DialogButtonOptions[];
        protected okClick(): void;
        protected okClickValidated(): void;
        protected cancelClick(): void;
        protected initPropertyGrid(): void;
        protected getFormKey(): string;
        protected getPropertyGridOptions(): PropertyGridOptions;
        protected getPropertyGridOptionsAsync(): PromiseLike<PropertyGridOptions>;
        protected getPropertyItems(): PropertyItem[];
        protected getPropertyItemsAsync(): PromiseLike<PropertyItem[]>;
        protected getSaveEntity(): TItem;
        protected initializeAsync(): PromiseLike<void>;
        protected loadInitialEntity(): void;
        protected get_entity(): TItem;
        protected set_entity(value: TItem): void;
        protected get_entityId(): any;
        protected set_entityId(value: any): void;
        protected validateBeforeSave(): boolean;
        protected updateTitle(): void;
        protected propertyGrid: Serenity.PropertyGrid;
    }
}
declare namespace Serenity {
    class FilterDialog extends TemplatedDialog<any> {
        private filterPanel;
        constructor();
        get_filterPanel(): FilterPanel;
        protected getTemplate(): string;
        protected getDialogOptions(): JQueryUI.DialogOptions;
    }
}
declare namespace Serenity {
    interface IDataGrid {
        getElement(): JQuery;
        getGrid(): Slick.Grid;
        getView(): Slick.RemoteView<any>;
        getFilterStore(): Serenity.FilterStore;
    }
    class IDataGrid {
    }
    class DataGrid<TItem, TOptions> extends Widget<TOptions> implements IDataGrid {
        protected titleDiv: JQuery;
        protected toolbar: Toolbar;
        protected filterBar: FilterDisplayBar;
        protected quickFiltersDiv: JQuery;
        protected slickContainer: JQuery;
        protected allColumns: Slick.Column[];
        protected initialSettings: PersistedGridSettings;
        protected restoringSettings: number;
        private idProperty;
        private isActiveProperty;
        private localTextDbPrefix;
        private isDisabled;
        private submitHandlers;
        private rows;
        private slickGridOnSort;
        private slickGridOnClick;
        view: Slick.RemoteView<TItem>;
        slickGrid: Slick.Grid;
        openDialogsAsPanel: boolean;
        static defaultRowHeight: number;
        static defaultHeaderHeight: number;
        static defaultPersistanceStorage: SettingStorage;
        constructor(container: JQuery, options?: TOptions);
        protected attrs<TAttr>(attrType: {
            new (...args: any[]): TAttr;
        }): TAttr[];
        protected add_submitHandlers(action: () => void): void;
        protected remove_submitHandlers(action: () => void): void;
        protected layout(): void;
        protected getInitialTitle(): string;
        protected createToolbarExtensions(): void;
        protected createQuickFilters(): void;
        protected getQuickFilters(): QuickFilter<Widget<any>, any>[];
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
        protected initializeAsync(): PromiseLike<void>;
        protected createSlickGrid(): Slick.Grid;
        protected setInitialSortOrder(): void;
        itemAt(row: number): any;
        rowCount(): number;
        getItems(): TItem[];
        setItems(value: TItem[]): void;
        protected bindToSlickEvents(): void;
        protected getAddButtonCaption(): string;
        protected getButtons(): ToolButton[];
        protected editItem(entityOrId: any): void;
        protected editItemOfType(itemType: string, entityOrId: any): void;
        protected onClick(e: JQueryEventObject, row: number, cell: number): void;
        protected viewDataChanged(e: JQuery, rows: TItem[]): void;
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
        protected getPropertyItemsAsync(): PromiseLike<PropertyItem[]>;
        protected getPropertyItems(): PropertyItem[];
        protected getColumns(): Slick.Column[];
        protected propertyItemsToSlickColumns(propertyItems: PropertyItem[]): Slick.Column[];
        protected getColumnsAsync(): PromiseLike<Slick.Column[]>;
        protected getSlickOptions(): Slick.GridOptions;
        protected populateLock(): void;
        protected populateUnlock(): void;
        protected getGridCanLoad(): boolean;
        refresh(): void;
        protected refreshIfNeeded(): void;
        protected internalRefresh(): void;
        setIsDisabled(value: boolean): void;
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
    class CheckTreeEditor<TItem extends CheckTreeItem<any>, TOptions> extends DataGrid<TItem, TOptions> implements IGetEditValue, ISetEditValue {
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
        private get_value();
        value: string[];
        private set_value(value);
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
        cascadeFrom: string;
        protected getCascadeFromValue(parent: Serenity.Widget<any>): any;
        protected cascadeLink: Serenity.CascadedWidgetLink<Widget<any>>;
        protected setCascadeFrom(value: string): void;
        protected set_cascadeFrom(value: string): void;
        protected get_cascadeField(): any;
        cascadeField: string;
        protected set_cascadeField(value: string): void;
        protected get_cascadeValue(): any;
        cascadeValue: any;
        protected set_cascadeValue(value: any): void;
        protected get_filterField(): string;
        filterField: string;
        protected set_filterField(value: string): void;
        protected get_filterValue(): any;
        filterValue: any;
        protected set_filterValue(value: any): void;
    }
}
declare namespace Serenity {
    class TemplatedPanel<TOptions> extends TemplatedWidget<TOptions> {
        constructor(container: JQuery, options?: TOptions);
        destroy(): void;
        protected tabs: JQuery;
        protected toolbar: Serenity.Toolbar;
        protected validator: JQueryValidation.Validator;
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
}
declare namespace Serenity {
    class PropertyPanel<TItem, TOptions> extends TemplatedPanel<TOptions> {
        private _entity;
        private _entityId;
        constructor(container: JQuery, options?: TOptions);
        destroy(): void;
        protected initPropertyGrid(): void;
        protected initPropertyGridAsync(): PromiseLike<void>;
        protected loadInitialEntity(): void;
        protected initializeAsync(): PromiseLike<void>;
        protected getFormKey(): string;
        protected getPropertyGridOptions(): PropertyGridOptions;
        protected getPropertyGridOptionsAsync(): PromiseLike<PropertyGridOptions>;
        protected getPropertyItems(): PropertyItem[];
        protected getPropertyItemsAsync(): PromiseLike<PropertyItem[]>;
        protected getSaveEntity(): TItem;
        protected get_entity(): TItem;
        protected get_entityId(): any;
        protected set_entity(value: TItem): void;
        protected set_entityId(value: any): void;
        protected validateBeforeSave(): boolean;
        protected propertyGrid: Serenity.PropertyGrid;
    }
}
declare namespace Serenity {
    class IEditDialog {
    }
    interface IEditDialog {
        load(entityOrId: any, done: () => void, fail: (p1: any) => void): void;
    }
    class EntityDialog<TItem, TOptions> extends TemplatedDialog<TOptions> implements IEditDialog {
        protected entity: TItem;
        protected entityId: any;
        protected propertyGrid: PropertyGrid;
        protected toolbar: Toolbar;
        protected saveAndCloseButton: JQuery;
        protected applyChangesButton: JQuery;
        protected deleteButton: JQuery;
        protected undeleteButton: JQuery;
        protected cloneButton: JQuery;
        protected localizationGrid: PropertyGrid;
        protected localizationButton: JQuery;
        protected localizationPendingValue: any;
        protected localizationLastValue: any;
        static defaultLanguageList: () => string[][];
        constructor(opt?: TOptions);
        protected initializeAsync(): PromiseLike<void>;
        destroy(): void;
        protected get_entity(): TItem;
        protected set_entity(entity: any): void;
        protected get_entityId(): any;
        protected set_entityId(value: any): void;
        protected getEntityNameFieldValue(): any;
        protected getEntityTitle(): string;
        protected updateTitle(): void;
        protected isCloneMode(): boolean;
        protected isEditMode(): boolean;
        protected isDeleted(): boolean;
        protected isNew(): boolean;
        protected isNewOrDeleted(): boolean;
        protected getDeleteOptions(callback: (response: DeleteResponse) => void): ServiceOptions<DeleteResponse>;
        protected deleteHandler(options: ServiceOptions<DeleteResponse>, callback: (response: DeleteResponse) => void): void;
        protected doDelete(callback: (response: DeleteResponse) => void): void;
        protected onDeleteSuccess(response: DeleteResponse): void;
        protected attrs<TAttr>(attrType: {
            new (...args: any[]): TAttr;
        }): TAttr[];
        private entityType;
        protected getEntityType(): string;
        private formKey;
        protected getFormKey(): string;
        private localTextDbPrefix;
        protected getLocalTextDbPrefix(): string;
        protected getLocalTextPrefix(): string;
        private entitySingular;
        protected getEntitySingular(): string;
        private nameProperty;
        protected getNameProperty(): string;
        private idProperty;
        protected getIdProperty(): string;
        protected isActiveProperty: string;
        protected getIsActiveProperty(): string;
        protected getIsDeletedProperty(): string;
        protected service: string;
        protected getService(): string;
        load(entityOrId: any, done: () => void, fail: (ex: ss.Exception) => void): void;
        loadNewAndOpenDialog(asPanel?: boolean): void;
        loadEntityAndOpenDialog(entity: TItem, asPanel?: boolean): void;
        protected loadResponse(data: any): void;
        protected loadEntity(entity: TItem): void;
        protected beforeLoadEntity(entity: TItem): void;
        protected afterLoadEntity(): void;
        loadByIdAndOpenDialog(entityId: any, asPanel?: boolean): void;
        protected onLoadingData(data: RetrieveResponse<TItem>): void;
        protected getLoadByIdOptions(id: any, callback: (response: RetrieveResponse<TItem>) => void): ServiceOptions<RetrieveResponse<TItem>>;
        protected getLoadByIdRequest(id: any): RetrieveRequest;
        protected reloadById(): void;
        loadById(id: any, callback?: (response: RetrieveResponse<TItem>) => void, fail?: () => void): void;
        protected loadByIdHandler(options: ServiceOptions<RetrieveResponse<TItem>>, callback: (response: RetrieveResponse<TItem>) => void, fail: () => void): void;
        protected initLocalizationGrid(): void;
        protected initLocalizationGridAsync(): PromiseLike<void>;
        protected initLocalizationGridCommon(pgOptions: PropertyGridOptions): void;
        protected isLocalizationMode(): boolean;
        protected isLocalizationModeAndChanged(): boolean;
        protected localizationButtonClick(): void;
        protected getLanguages(): any[];
        private getLangs();
        protected loadLocalization(): void;
        protected setLocalizationGridCurrentValues(): void;
        protected getLocalizationGridValue(): any;
        protected getPendingLocalizations(): any;
        protected initPropertyGrid(): void;
        protected initPropertyGridAsync(): PromiseLike<void>;
        protected getPropertyItems(): any;
        protected getPropertyGridOptions(): PropertyGridOptions;
        protected getPropertyGridOptionsAsync(): PromiseLike<PropertyGridOptions>;
        protected getPropertyItemsAsync(): PromiseLike<PropertyItem[]>;
        protected validateBeforeSave(): boolean;
        protected getSaveOptions(callback: (response: SaveResponse) => void): ServiceOptions<SaveResponse>;
        protected getSaveEntity(): TItem;
        protected getSaveRequest(): SaveRequest<TItem>;
        protected onSaveSuccess(response: SaveResponse): void;
        protected save_submitHandler(callback: (response: SaveResponse) => void): void;
        protected save(callback?: (response: SaveResponse) => void): void | boolean;
        protected saveHandler(options: ServiceOptions<SaveResponse>, callback: (response: SaveResponse) => void): void;
        protected initToolbar(): void;
        protected showSaveSuccessMessage(response: SaveResponse): void;
        protected getToolbarButtons(): ToolButton[];
        protected getCloningEntity(): TItem;
        protected updateInterface(): void;
        protected getUndeleteOptions(callback?: (response: UndeleteResponse) => void): ServiceOptions<UndeleteResponse>;
        protected undeleteHandler(options: ServiceOptions<UndeleteResponse>, callback: (response: UndeleteResponse) => void): void;
        protected undelete(callback?: (response: UndeleteResponse) => void): void;
    }
}
declare namespace Serenity {
    namespace Reporting {
        interface ReportDialogOptions {
            reportKey?: string;
        }
        class ReportDialog extends TemplatedDialog<ReportDialogOptions> {
            constructor(opt: ReportDialogOptions);
            protected propertyGrid: Serenity.PropertyGrid;
            protected propertyItems: Serenity.PropertyItem[];
            protected reportKey: string;
            protected createPropertyGrid(): void;
            loadReport(reportKey: string): void;
            executeReport(targetFrame: string, exportType: string): void;
            protected getToolbarButtons(): {
                title: string;
                cssClass: string;
                onClick: () => void;
            }[];
        }
        interface ReportExecuteRequest extends ServiceRequest {
            ExportType?: string;
            ReportKey?: string;
            DesignId?: string;
            Parameters?: any;
        }
        class ReportPage extends Serenity.Widget<any> {
            constructor(div: JQuery);
            protected updateMatchFlags(text: string): void;
            protected categoryClick(e: JQueryEventObject): void;
            protected reportLinkClick(e: JQueryEventObject): void;
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
        protected getDialogOptions(): JQueryUI.DialogOptions;
        private getTitle(col);
        private createLI(col);
        private updateListStates();
        protected setupColumns(): void;
        protected onDialogOpen(): void;
        protected getTemplate(): string;
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
interface JQuery {
    flexHeightOnly(flexY?: number): JQuery;
    flexWidthOnly(flexX?: number): JQuery;
    flexWidthHeight(flexX: number, flexY: number): JQuery;
    flexX(flexX: number): JQuery;
    flexY(flexY: number): JQuery;
}
interface JQuery {
    getWidget<TWidget>(widgetType: {
        new (...args: any[]): TWidget;
    }): TWidget;
    tryGetWidget<TWidget>(widgetType: {
        new (...args: any[]): TWidget;
    }): TWidget;
}
interface JQuery {
    slickPager(options: Slick.PagerOptions): JQuery;
}
declare namespace Slick.Data {
    class GroupItemMetadataProvider {
        constructor();
    }
}
declare namespace Slick {
    interface PagerOptions {
        view?: Slick.RemoteView<any>;
        showRowsPerPage?: boolean;
        rowsPerPage?: number;
        rowsPerPageOptions?: number[];
        onChangePage?: (newPage: number) => void;
    }
    interface FormatterContext {
        row?: number;
        cell?: number;
        value?: any;
        column?: any;
        item?: any;
    }
    interface Formatter {
        format(ctx: FormatterContext): string;
    }
    type Format = (ctx: Slick.FormatterContext) => string;
    class Event {
        subscribe(handler: (p1: any, p2?: any) => void): void;
        subscribe(handler: (p1: any, p2?: any) => any): void;
        unsubscribe(handler: (p1: any, p2?: any) => void): void;
        notify(p1?: any, p2?: any, p3?: any): void;
        clear(): void;
    }
    interface PositionInfo {
        bottom: number;
        height: number;
        left: number;
        right: number;
        top: number;
        visible: boolean;
        width: number;
    }
    interface SlickRangeInfo {
        top: number;
        bottom: number;
        leftPx: number;
        rightPx: number;
    }
    class EventData {
        constructor();
    }
    type AsyncPostRender = (cellNode: any, row: number, item: any, column: Slick.Column, clean?: boolean) => void;
    type CancellableViewCallback<TEntity> = (view: Slick.RemoteView<TEntity>) => boolean;
    type ColumnFormatter = (row: number, cell: number, value: any, column: Slick.Column, item: any) => string;
    type RemoteViewAjaxCallback<TEntity> = (view: Slick.RemoteView<TEntity>, options: JQueryAjaxSettings) => boolean;
    type RemoteViewFilter<TEntity> = (item: TEntity, view: Slick.RemoteView<TEntity>) => boolean;
    type RemoteViewProcessCallback<TEntity> = (data: Serenity.ListResponse<TEntity>, view: Slick.RemoteView<TEntity>) => Serenity.ListResponse<TEntity>;
    interface Column {
        asyncPostRender?: Slick.AsyncPostRender;
        behavior?: any;
        cannotTriggerInsert?: boolean;
        cssClass?: string;
        defaultSortAsc?: boolean;
        editor?: Function;
        field: string;
        focusable?: boolean;
        formatter?: Slick.ColumnFormatter;
        headerCssClass?: string;
        id?: string;
        maxWidth?: any;
        minWidth?: number;
        name?: string;
        rerenderOnResize?: boolean;
        resizable?: boolean;
        selectable?: boolean;
        sortable?: boolean;
        toolTip?: string;
        width?: number;
        format?: (ctx: Slick.FormatterContext) => string;
        referencedFields?: string[];
        sourceItem?: Serenity.PropertyItem;
        sortOrder?: number;
        groupTotalsFormatter?: (p1?: GroupTotals<any>, p2?: Column) => string;
        visible?: boolean;
    }
    class RowMoveManager {
        constructor(options: Slick.RowMoveManagerOptions);
        onBeforeMoveRows: Slick.Event;
        onMoveRows: Slick.Event;
    }
    class RowSelectionModel {
    }
    class AutoTooltips {
        constructor(options: Slick.AutoTooltipsOptions);
    }
    interface AutoTooltipsOptions {
        enableForHeaderCells?: boolean;
        enableForCells?: boolean;
        maxToolTipLength?: number;
    }
    interface GroupInfo<TItem> {
        getter?: any;
        formatter?: (p1: Slick.Group<TItem>) => string;
        comparer?: (a: Slick.Group<TItem>, b: Slick.Group<TItem>) => number;
        aggregators?: any[];
        aggregateCollapsed?: boolean;
        lazyTotalsCalculation?: boolean;
    }
    interface RowCell {
        row: number;
        cell: number;
    }
    interface RowMoveManagerOptions {
        cancelEditOnDrag: boolean;
    }
    interface SummaryOptions {
        aggregators: any[];
    }
    class Group<TEntity> {
        isGroup: boolean;
        level: number;
        count: number;
        value: any;
        title: string;
        collapsed: boolean;
        totals: any;
        rows: any;
        groups: Group<TEntity>[];
        groupingKey: string;
    }
    class GroupTotals<TEntity> {
        isGroupTotals: boolean;
        group: Group<TEntity>;
        initialized: boolean;
        sum: any;
        avg: any;
        min: any;
        max: any;
    }
    interface GridOptions {
        asyncEditorLoading?: boolean;
        asyncEditorLoadDelay?: number;
        asyncPostRenderDelay?: number;
        asyncPostRenderCleanupDelay?: number;
        autoEdit?: boolean;
        autoHeight?: boolean;
        cellFlashingCssClass?: string;
        cellHighlightCssClass?: string;
        dataItemColumnValueExtractor?: () => void;
        defaultColumnWidth?: number;
        defaultFormatter?: () => void;
        editable?: boolean;
        editCommandHandler?: () => void;
        editorFactory?: () => void;
        editorLock?: any;
        enableAddRow?: boolean;
        enableAsyncPostRender?: boolean;
        enableAsyncPostRenderCleanup?: boolean;
        enableCellRangeSelection?: boolean;
        enableCellNavigation?: boolean;
        enableColumnReorder?: boolean;
        enableRowReordering?: boolean;
        enableTextSelectionOnCells?: boolean;
        explicitInitialization?: boolean;
        forceFitColumns?: boolean;
        forceSyncScrolling?: boolean;
        formatterFactory?: () => void;
        fullWidthRows?: boolean;
        frozenColumn?: number;
        frozenRow?: number;
        frozenBottom?: boolean;
        headerRowHeight?: number;
        leaveSpaceForNewRows?: boolean;
        minBuffer?: number;
        multiColumnSort?: boolean;
        multiSelect?: boolean;
        renderAllCells?: boolean;
        rowHeight?: number;
        selectedCellCssClass?: string;
        showHeaderRow?: boolean;
        showFooterRow?: boolean;
        syncColumnCellResize?: boolean;
        topPanelHeight?: number;
    }
    interface RemoteView<TEntity> {
        constructor(options: RemoteViewOptions): void;
        onSubmit: Slick.CancellableViewCallback<TEntity>;
        onDataChanged: Slick.Event;
        onDataLoaded: Slick.Event;
        onAjaxCall: Slick.RemoteViewAjaxCallback<TEntity>;
        onProcessData: Slick.RemoteViewProcessCallback<TEntity>;
        addData(data: Serenity.ListResponse<TEntity>): void;
        beginUpdate(): void;
        endUpdate(): void;
        deleteItem(id: any): void;
        getItems(): TEntity[];
        setFilter(filter: RemoteViewFilter<TEntity>): void;
        setItems(items: any[], fullReset: boolean): void;
        getItemById(id: any): TEntity;
        getRowById(id: any): number;
        updateItem(id: any, item: TEntity): void;
        addItem(item: TEntity): void;
        getIdxById(id: any): any;
        getItemByIdx(index: number): any;
        setGrouping(groupInfo: Slick.GroupInfo<TEntity>[]): void;
        collapseAllGroups(level: number): void;
        expandAllGroups(level: number): void;
        expandGroup(keys: any[]): void;
        collapseGroup(keys: any[]): void;
        setSummaryOptions(options: Slick.SummaryOptions): void;
        refresh(): void;
        populate(): void;
        populateLock(): void;
        populateUnlock(): void;
        getItem(row: number): any;
        getLength(): number;
        params: any;
        sortBy: string[];
        url: string;
        seekToPage?: number;
    }
    interface RemoteViewOptions {
        autoLoad?: boolean;
        idField?: string;
        contentType?: string;
        dataType?: string;
        filter?: any;
        params?: any;
        onSubmit?: Slick.Event;
        url?: string;
        sortBy?: any;
        rowsPerPage?: number;
        seekToPage?: number;
        onProcessData?: Slick.Event;
        method?: string;
        getItemMetadata?: (p1?: any, p2?: number) => any;
        errorMsg?: string;
    }
    interface ColumnSort {
        columnId?: string;
        sortAsc?: boolean;
    }
    interface RangeInfo {
        top?: number;
        bottom?: number;
        leftPx?: number;
        rightPx?: number;
    }
    class Grid {
        constructor(container: JQuery, data: any, columns: Column[], options: GridOptions);
    }
    interface Grid {
        init(): void;
        destroy(): void;
        getData(): any[];
        getDataItem(index: number): any;
        setData(data: any[], scrollToTop: boolean): void;
        getDataLength(): number;
        getOptions(): GridOptions;
        setOptions(options: GridOptions): void;
        getSelectedRows(): any;
        getSelectionModel(): any;
        setSelectionModel(model: any): any;
        setSelectedRows(rows: any): void;
        autoSizeColumns(): void;
        getColumnIndex(id: string): number;
        getColumns(): Column[];
        setColumns(columns: Column[]): void;
        setSortColumn(columnId: string, ascending: boolean): void;
        setSortColumns(cols: Slick.ColumnSort[]): void;
        updateColumnHeader(columnId: string, title: string, toolTip: string): void;
        addCellCssStyles(key: string, hash: any): void;
        canCellBeActive(row: number, col: number): boolean;
        canCellBeSelected(row: number, col: number): boolean;
        editActiveCell(editor: Function): void;
        flashCell(row: number, cell: number, speed: number): void;
        getActiveCell(): Slick.RowCell;
        getActiveCellNode(): any;
        getActiveCellPosition(): Slick.PositionInfo;
        getCellCssStyles(key: string): any;
        getCellEditor(): any;
        getCellFromEvent(e: any): Slick.RowCell;
        getCellFromPoint(x: number, y: number): Slick.RowCell;
        getCellNode(row: number, cell: number): any;
        getCellNodeBox(row: number, cell: number): Slick.PositionInfo;
        goToCell(row: number, cell: number, forceEdit: boolean): void;
        navigateDown(): void;
        navigateLeft(): void;
        navigateNext(): void;
        navigatePrev(): void;
        navigateRight(): void;
        navigateUp(): void;
        removeCellCssStyles(key: string): void;
        resetActiveCell(): void;
        registerPlugin(plugin: any): void;
        setActiveCell(row: number, cell: number): void;
        setCellCssStyles(key: string, hash: any): void;
        getCanvasNode(): any;
        getGridPosition(): Slick.PositionInfo;
        getRenderedRange(viewportTop: number, viewportLeft: number): Slick.RangeInfo;
        getViewport(viewportTop: number, viewportLeft: number): Slick.RangeInfo;
        getViewport(): Slick.RangeInfo;
        invalidate(): void;
        invalidateAllRows(): void;
        invalidateRow(row: number): void;
        invalidateRows(rows: any): void;
        render(): void;
        resizeCanvas(): void;
        scrollCellIntoView(row: number, cell: number): void;
        scrollRowIntoView(row: number, doPaging: boolean): void;
        scrollRowToTop(row: number): void;
        updateCell(row: number, cell: number): void;
        updateRow(row: number): void;
        updateRowCount(): void;
        getHeaderRow(): any;
        getHeaderRowColumn(columnId: string): any;
        getSortColumns(): any;
        getTopPanel(): any;
        setHeaderRowVisibility(visible: boolean): void;
        onScroll?: Slick.Event;
        onSort?: Slick.Event;
        onHeaderContextMenu?: Slick.Event;
        onHeaderClick?: Slick.Event;
        onMouseEnter?: Slick.Event;
        onMouseLeave?: Slick.Event;
        onClick?: Slick.Event;
        onDblClick?: Slick.Event;
        onContextMenu?: Slick.Event;
        onKeyDown?: Slick.Event;
        onAddNewRow?: Slick.Event;
        onValidationError?: Slick.Event;
        onViewportChanged?: Slick.Event;
        onColumnsReordered?: Slick.Event;
        onColumnsResized?: Slick.Event;
        onCellChange?: Slick.Event;
        onBeforeEditCell?: Slick.Event;
        onBeforeCellEditorDestroy?: Slick.Event;
        onHeaderCellRendered?: Slick.Event;
        onBeforeHeaderCellDestroy?: Slick.Event;
        onBeforeDestroy?: Slick.Event;
        onActiveCellChanged?: Slick.Event;
        onActiveCellPositionChanged?: Slick.Event;
        onDragInit?: Slick.Event;
        onDragStart?: Slick.Event;
        onDrag?: Slick.Event;
        onDragEnd?: Slick.Event;
        onSelectedRowsChanged?: Slick.Event;
        onCellCssStylesChanged?: Slick.Event;
    }
}
declare namespace Slick.Data {
}
declare namespace Slick {
    class RemoteView<TEntity> {
        constructor(options: any);
    }
}
declare namespace Slick.Aggregators {
    function Avg(field: string): void;
    function WeightedAvg(field: string, weightedField: string): void;
    function Min(field: string): void;
    function Max(field: string): void;
    function Sum(field: string): void;
}
declare var Vue: any;
declare namespace Q {
    function validatorAbortHandler(validator: any): void;
    function validateOptions(options: JQueryValidation.ValidationOptions): any;
}
declare namespace Serenity {
    class AsyncLookupEditor extends LookupEditorBase<LookupEditorOptions, any> {
        constructor(hidden: JQuery, opt: LookupEditorOptions);
        getLookupKey(): any;
    }
}
declare namespace Serenity {
    class CascadedWidgetLink<TParent extends Widget<any>> {
        private parentType;
        private widget;
        private parentChange;
        constructor(parentType: {
            new (...args: any[]): TParent;
        }, widget: Serenity.Widget<any>, parentChange: (p1: TParent) => void);
        private _parentID;
        bind(): TParent;
        unbind(): TParent;
        get_parentID(): string;
        set_parentID(value: string): void;
    }
}
declare namespace Serenity.DialogExtensions {
    function dialogFlexify(dialog: JQuery): JQuery;
    function dialogResizable(dialog: JQuery, w?: any, h?: any, mw?: any, mh?: any): JQuery;
    function dialogMaximizable(dialog: JQuery): JQuery;
    function dialogCloseOnEnter(dialog: JQuery): JQuery;
}
declare namespace Serenity.DialogTypeRegistry {
    function tryGet(key: string): Function;
    function get(key: string): Function;
}
declare namespace Serenity {
    class Toolbar extends Widget<ToolbarOptions> {
        constructor(div: JQuery, options: ToolbarOptions);
        protected mouseTrap: any;
        destroy(): void;
        protected setupMouseTrap(): void;
        render(options: ToolbarOptions & Q.ComponentProps<this>): Q.VNode;
        static buttonSelector: string;
        adjustIconClass(icon: string): string;
        renderButtons(buttons: ToolButton[]): JSX.Element;
        buttonClass(btn: ToolButton): {
            [x: string]: boolean;
            "tool-button": boolean;
            "icon-tool-button": boolean;
            "no-text": boolean;
            disabled: boolean;
        };
        renderButtonText(btn: ToolButton): JSX.Element;
        buttonClick(e: MouseEvent, btn: ToolButton): void;
        renderButton(btn: ToolButton): JSX.Element;
        findButton(className: string): JQuery;
    }
}
