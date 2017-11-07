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
    context: any;
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
declare namespace System.ComponentModel {
    class DisplayNameAttribute {
        constructor(displayName: string);
        displayName: string;
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
}
declare namespace Q {
    function coalesce(a: any, b: any): any;
    function isValue(a: any): boolean;
    function deepClone<TItem>(arg1: TItem, ...args: TItem[]): TItem;
}
declare namespace Q {
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
}
declare namespace Q {
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
}
declare namespace Q {
    namespace Culture {
        let decimalSeparator: string;
        let dateSeparator: string;
        let dateOrder: string;
        let dateFormat: string;
        let dateTimeFormat: string;
        function get_groupSeperator(): string;
    }
}
declare namespace Q {
    function formatNumber(n: number, fmt: string, dec?: string, grp?: string): string;
    function parseInteger(s: string): number;
    function parseDecimal(s: string): number;
    function toId(id: any): any;
}
declare namespace Q {
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
}
declare namespace Serenity {
    type ServiceOptions<TResponse extends Serenity.ServiceResponse> = Q.ServiceOptions<TResponse>;
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
    class GeneratedCodeAttribute {
        origin: string;
        constructor(origin?: string);
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
}
declare namespace Serenity {
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
    class FilterableAttribute {
        constructor(value: boolean);
        value: boolean;
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
declare namespace Serenity {
    namespace Decorators {
        function addAttribute(type: any, attr: any): void;
        function addMemberAttr(type: any, memberName: string, attr: any): void;
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
        function registerClass(intf?: any[], asm?: ss.AssemblyReg): (target: Function) => void;
        function registerInterface(intf?: any[], asm?: ss.AssemblyReg): (target: Function) => void;
        function registerEnum(target: any, enumKey?: string, asm?: ss.AssemblyReg): void;
        function registerEditor(intf?: any[], asm?: ss.AssemblyReg): (target: Function) => void;
        function registerFormatter(intf?: (typeof ISlickFormatter)[], asm?: ss.AssemblyReg): (target: Function) => void;
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
    class GridRows<TItem> {
    }
    class GridRowSelectionMixin extends ScriptContext {
        constructor(grid: IDataGrid);
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
    namespace SlickTreeHelper {
        function filterCustom<TItem>(item: TItem, getParent: (x: TItem) => any): boolean;
        function filterById<TItem>(item: TItem, view: Slick.RemoteView<TItem>, getParentId: (x: TItem) => any): boolean;
        function setCollapsed<TItem>(items: TItem[], collapsed: boolean): void;
        function setCollapsedFlag<TItem>(item: TItem, collapsed: boolean): void;
        function setIndents<TItem>(items: TItem[], getId: (x: TItem) => any, getParentId: (x: TItem) => any, setCollapsed?: boolean): void;
        function toggleClick<TItem>(e: JQueryEventObject, row: number, cell: number, view: Slick.RemoteView<TItem>, getId: (x: TItem) => any): void;
    }
    namespace SlickFormatting {
        function getEnumText(value: any): string;
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
        function setDefaults(columns: any, localTextPrefix?: string): any;
        function convertToFormatter(format: Slick.Format): Slick.ColumnFormatter;
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
        Filename: string;
        OriginalName: string;
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
        private initialize();
    }
    interface Widget<TOptions> {
        addValidationRule(eventClass: string, rule: (p1: JQuery) => string): JQuery;
        getGridField(): JQuery;
        change(handler: (e: JQueryEventObject) => void): void;
        changeSelect2(handler: (e: JQueryEventObject) => void): void;
    }
}
declare namespace Serenity {
    class TemplatedWidget<TOptions> extends Widget<TOptions> {
        protected idPrefix: string;
        private static templateNames;
        constructor(container: JQuery, options?: TOptions);
        protected byId(id: string): JQuery;
        private byID<TWidget>(id, type);
        protected getTemplateName(): string;
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
    interface PropertyItem {
        name?: string;
        title?: string;
        hint?: string;
        placeholder?: string;
        editorType?: string;
        editorParams?: any;
        category?: string;
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
    }
}
declare namespace Serenity {
    class BooleanEditor extends Widget<any> {
        constructor(input: JQuery);
        value: boolean;
        protected get_value(): boolean;
        protected set_value(value: boolean): void;
    }
}
declare namespace Serenity {
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
        value: string[];
    }
}
declare namespace Serenity {
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
        yearRange: string;
    }
}
declare namespace Serenity {
    class DateTimeEditor extends Widget<DateTimeEditorOptions> {
        constructor(input: JQuery, opt?: DateTimeEditorOptions);
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
        yearRange?: string;
    }
}
declare namespace Serenity {
    class DateYearEditor extends SelectEditor {
        constructor(hidden: JQuery, opt: DateYearEditorOptions);
    }
    interface DateYearEditorOptions extends SelectEditorOptions {
        minYear?: string;
        maxYear?: string;
        descending?: boolean;
    }
}
declare namespace Serenity {
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
}
declare namespace Serenity {
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
    class LookupEditor extends LookupEditorBase<LookupEditorOptions, any> {
        constructor(input: JQuery, opt?: LookupEditorOptions);
    }
}
declare namespace Serenity {
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
}
declare namespace Serenity {
    class StringEditor extends Widget<any> {
        constructor(input: JQuery);
        value: string;
        protected get_value(): string;
        protected set_value(value: string): void;
    }
}
declare namespace Serenity {
    class TextAreaEditor extends Widget<TextAreaEditorOptions> {
        constructor(input: JQuery, opt?: TextAreaEditorOptions);
        value: string;
        protected get_value(): string;
        protected set_value(value: string): void;
    }
    interface TextAreaEditorOptions {
        cols?: number;
        rows?: number;
    }
}
declare namespace Serenity {
    class TimeEditor extends Widget<TimeEditorOptions> {
        private minutes;
        constructor(input: JQuery, opt?: TimeEditorOptions);
        value: number;
        protected get_value(): number;
        protected set_value(value: number): void;
    }
    interface TimeEditorOptions {
        noEmptyOption?: boolean;
        startHour?: any;
        endHour?: any;
        intervalMinutes?: any;
    }
}
declare namespace Serenity {
    namespace EditorUtils {
        function getValue(editor: Serenity.Widget<any>): any;
        function saveValue(editor: Serenity.Widget<any>, item: PropertyItem, target: any): void;
        function setValue(editor: Serenity.Widget<any>, value: any): void;
        function loadValue(editor: Serenity.Widget<any>, item: PropertyItem, source: any): void;
        function setReadonly(elements: JQuery, isReadOnly: boolean): JQuery;
        function setReadOnly(widget: Serenity.Widget<any>, isReadOnly: boolean): void;
        function setRequired(widget: Serenity.Widget<any>, isRequired: boolean): void;
    }
    class PublicEditorTypes {
        static get_registeredTypes(): any;
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
    interface RadioButtonEditorOptions {
        enumKey?: string;
        enumType?: any;
        lookupKey?: string;
    }
    class RadioButtonEditor extends Widget<RadioButtonEditorOptions> {
        constructor(input: JQuery);
        value: string;
    }
    interface EnumEditorOptions {
        enumKey?: string;
        enumType?: any;
        allowClear?: boolean;
    }
    interface HtmlContentEditorOptions {
        cols?: any;
        rows?: any;
    }
    class HtmlContentEditor extends Widget<HtmlContentEditorOptions> {
        constructor(textArea: JQuery, opt?: HtmlContentEditorOptions);
        instanceReady(x: any): void;
        getLanguage(): string;
        getConfig(): CKEditorConfig;
        value: string;
    }
    class HtmlNoteContentEditor extends HtmlContentEditor {
        constructor(textArea: JQuery, opt?: HtmlContentEditorOptions);
    }
    class HtmlReportContentEditor extends HtmlContentEditor {
        constructor(textArea: JQuery, opt?: HtmlContentEditorOptions);
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
        value: UploadedFile;
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
        value: UploadedFile[];
        get_jsonEncodeValue(): boolean;
        set_jsonEncodeValue(value: boolean): void;
    }
    interface PhoneEditorOptions {
        multiple?: boolean;
        internal?: boolean;
        mobile?: boolean;
        allowExtension?: boolean;
        allowInternational?: boolean;
    }
    class PhoneEditor extends Widget<PhoneEditorOptions> {
        constructor(input: JQuery, opt?: PhoneEditorOptions);
        validate(value: string): string;
        formatValue(): void;
        getFormattedValue(): string;
        value: string;
    }
    class EnumEditor extends Select2Editor<EnumEditorOptions, Select2Item> {
        constructor(hidden: JQuery, opt: EnumEditorOptions);
        updateItems(): void;
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
        value: string;
    }
    class URLEditor extends StringEditor {
        constructor(input: JQuery);
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
}
declare namespace Serenity {
    interface FilterOperator {
        key?: string;
        title?: string;
        format?: string;
    }
}
declare namespace Serenity {
    namespace FilterOperators {
        let isTrue: string;
        let isFalse: string;
        let contains: string;
        let startsWith: string;
        let EQ: string;
        let NE: string;
        let GT: string;
        let GE: string;
        let LT: string;
        let LE: string;
        let BW: string;
        let IN: string;
        let isNull: string;
        let isNotNull: string;
        let toCriteriaOperator: {
            [key: string]: string;
        };
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
}
declare namespace Serenity {
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
}
declare namespace Serenity {
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
}
declare namespace Serenity {
    class StringFiltering extends BaseFiltering {
    }
}
declare namespace Serenity {
    class BaseEditorFiltering<TEditor> extends BaseFiltering {
        editor: Serenity.Widget<any>;
        useEditor(): boolean;
        useIdField(): boolean;
        getEditorOptions(): any;
    }
}
declare namespace Serenity {
    class DateFiltering extends BaseEditorFiltering<DateEditor> {
    }
}
declare namespace Serenity {
    class DateTimeFiltering extends BaseEditorFiltering<DateEditor> {
    }
}
declare namespace Serenity {
    class DecimalFiltering extends BaseEditorFiltering<DecimalEditor> {
    }
}
declare namespace Serenity {
    class EditorFiltering extends BaseEditorFiltering<Serenity.Widget<any>> {
        get_editorType(): string;
        set_editorType(value: string): void;
        get_useRelative(): boolean;
        set_useRelative(value: boolean): void;
        get_useLike(): boolean;
        set_useLike(value: boolean): void;
    }
}
declare namespace Serenity {
    class LookupFiltering extends BaseEditorFiltering<LookupEditor> {
    }
}
declare namespace Serenity {
    namespace FilteringTypeRegistry {
        function get(key: string): Function;
        function initialize(): void;
        function reset(): void;
    }
}
declare namespace Serenity {
    class FilterWidgetBase<TOptions> extends TemplatedWidget<TOptions> {
        constructor(div: JQuery, opt: any);
        filterStoreChanged(): void;
        get_store(): FilterStore;
        set_store(value: FilterStore): void;
    }
}
declare namespace Serenity {
    class FilterDisplayBar extends FilterWidgetBase<any> {
        constructor(div: JQuery);
    }
}
declare namespace Serenity {
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
}
declare namespace Serenity {
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
    }
}
declare namespace Serenity {
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
}
declare namespace Serenity {
    class DateFormatter implements Slick.Formatter {
        format(ctx: Slick.FormatterContext): string;
        static format(value: any, format: string): string;
        get_displayFormat(): string;
        set_displayFormat(value: string): void;
    }
    class DateTimeFormatter extends DateFormatter {
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
    class FileDownloadFormatter {
        format(ctx: Slick.FormatterContext): string;
        static dbFileUrl(filename: string): string;
        initializeColumn(column: Slick.Column): void;
        get_displayFormat(): string;
        set_displayFormat(value: string): void;
        get_originalNameProperty(): string;
        set_originalNameProperty(value: string): void;
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
    namespace FormatterTypeRegistry {
        function get(key: string): Function;
        function initialize(): void;
        function reset(): void;
    }
}
declare namespace Serenity {
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
}
declare namespace Serenity {
    namespace EnumTypeRegistry {
        function get(key: string): Function;
    }
}
declare namespace Serenity {
    class IDialog {
    }
    interface IDialog {
        dialogOpen(): void;
    }
}
declare namespace Serenity {
    interface IEditDialog {
        load(entityOrId: any, done: () => void, fail: (p1: any) => void): void;
    }
}
declare namespace Serenity {
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
    class ISlickFormatter {
    }
    class ScriptContext {
    }
    class PrefixedContext extends ScriptContext {
        constructor(prefix: string);
        w(id: string, type: Function): any;
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
        Details = 2,
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
    class IReadOnly {
    }
    interface HtmlContentEditorOptions {
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
        Update = 3,
    }
    namespace CustomValidation {
        function registerValidationMethods(): void;
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
}
declare namespace Serenity {
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
}
declare namespace Serenity {
    class IAsyncInit {
    }
}
declare namespace Serenity {
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
    class PropertyItemHelper {
        static getPropertyItemsFor(type: Function): PropertyItem[];
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
        separator?: boolean;
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
}
declare namespace Serenity {
    interface DataChangeInfo {
        type: string;
        entityId: any;
        entity: any;
    }
}
declare namespace Serenity {
    class Select2Editor<TOptions, TItem> extends Widget<TOptions> {
        items: Select2Item[];
        itemById: any;
        pageSize: number;
        lastCreateTerm: string;
        constructor(hidden: JQuery, opt?: any);
        emptyItemText(): string;
        getSelect2Options(): Select2Options;
        clearItems(): void;
        addItem(item: Select2Item): void;
        addOption(key: string, text: string, source?: any, disabled?: boolean): void;
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
        readOnly: boolean;
    }
    namespace Select2Extensions {
        function select2(element: JQuery): JQuery;
        function select2(element: JQuery, options: Select2Options): JQuery;
        function select2(element: JQuery, action: string): JQuery;
        function select2(element: JQuery, option: string, value: any): JQuery;
        function select2(element: JQuery, option: string): any;
    }
    interface Select2Item {
        id: string;
        text: string;
        source?: any;
        disabled?: boolean;
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
}
declare namespace Serenity {
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
        protected propertyGrid: Serenity.PropertyGrid;
    }
}
declare namespace Serenity {
    class FilterDialog extends TemplatedDialog<any> {
        get_filterPanel(): FilterPanel;
    }
}
declare namespace Serenity {
    class IInitializeColumn {
    }
    interface IInitializeColumn {
        initializeColumn(column: Slick.Column): void;
    }
}
declare namespace Serenity {
    class DataGrid<TItem, TOptions> extends Widget<TOptions> {
        constructor(container: JQuery, options?: TOptions);
        protected allColumns: Slick.Column[];
        protected defaultColumns: string[];
        protected titleDiv: JQuery;
        protected filterBar: FilterDisplayBar;
        protected quickFiltersDiv: JQuery;
        protected slickContainer: JQuery;
        protected toolbar: Toolbar;
        protected addBooleanFilter(field: string, title?: string, yes?: string, no?: string): SelectEditor;
        protected booleanQuickFilter(field: string, title?: string, yes?: string, no?: string): QuickFilter<SelectEditor, SelectEditorOptions>;
        protected addDateRangeFilter(field: string, title?: string): DateEditor;
        protected addDateTimeRangeFilter(field: string, title?: string): DateTimeEditor;
        protected dateRangeQuickFilter(field: string, title?: string): QuickFilter<DateEditor, DateTimeEditorOptions>;
        protected dateTimeRangeQuickFilter(field: string, title?: string): QuickFilter<DateTimeEditor, DateTimeEditorOptions>;
        protected addQuickFilter<TWidget extends Widget<any>, TOptions>(filter: QuickFilter<TWidget, TOptions>): TWidget;
        protected addFilterSeparator(): void;
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
        protected findQuickFilter<TWidget>(type: {
            new (...args: any[]): TWidget;
        }, field: string): TWidget;
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
        protected getIncludeColumns(include: {
            [key: string]: boolean;
        }): void;
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
        protected itemLink(itemType?: string, idField?: string, text?: (ctx: Slick.FormatterContext) => string, cssClass?: (ctx: Slick.FormatterContext) => string, encode?: boolean): void;
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
        openDialogsAsPanel?: boolean;
        refresh(): void;
        getItems(): TItem[];
        setItems(value: TItem[]): void;
        isDisabled: boolean;
        setIsDisabled(value: boolean): void;
        getTitle(): string;
        setTitle(value: string): void;
        itemAt(row: number): TItem;
        rowCount(): number;
        view: Slick.RemoteView<TItem>;
        slickGrid: Slick.Grid;
        getElement(): JQuery;
        getFilterStore(): FilterStore;
        getGrid(): Slick.Grid;
        getView(): Slick.RemoteView<TItem>;
        static defaultHeaderHeight: number;
        static defaultRowHeight: number;
        static defaultPersistanceStorage: SettingStorage;
    }
}
declare namespace Serenity {
    class EntityGrid<TItem, TOptions> extends DataGrid<TItem, TOptions> {
        constructor(container: JQuery, options?: TOptions);
        protected addButtonClick(): void;
        protected createEntityDialog(itemType: string, callback?: (dlg: Widget<any>) => void): Widget<any>;
        protected getDialogOptions(): any;
        protected getDialogOptionsFor(itemType: string): any;
        protected getDialogType(): {
            new (...args: any[]): Widget<any>;
        };
        protected getDialogTypeFor(itemType: string): {
            new (...args: any[]): Widget<any>;
        };
        protected getDisplayName(): string;
        protected getItemName(): string;
        protected getEntityType(): string;
        protected getService(): string;
        protected initDialog(dialog: Widget<any>): void;
        protected initEntityDialog(itemType: string, dialog: Widget<any>): void;
        protected newRefreshButton(noText?: boolean): ToolButton;
    }
}
declare namespace Serenity {
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
}
declare namespace Serenity {
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
        protected propertyGrid: Serenity.PropertyGrid;
    }
}
declare namespace Serenity {
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
        load(entityOrId: any, done: () => void, fail: () => void): void;
        loadById(id: any): void;
        loadByIdAndOpenDialog(id: any): void;
        protected loadByIdHandler(options: ServiceOptions<RetrieveResponse<TItem>>, callback: (response: RetrieveResponse<TItem>) => void, fail: () => void): void;
        loadEntity(entity: any): void;
        loadEntityAndOpenDialog(entity: any): void;
        loadNewAndOpenDialog(): void;
        loadResponse(response: RetrieveResponse<TItem>): void;
        protected onDeleteSuccess(response: DeleteResponse): void;
        protected onLoadingData(data: RetrieveResponse<TItem>): void;
        protected onSaveSuccess(response: SaveResponse): void;
        protected reloadById(): void;
        protected save(callback: (response: SaveResponse) => void): void;
        protected saveHandler(options: ServiceOptions<SaveResponse>, callback: (response: SaveResponse) => void): void;
        protected save_submitHandler(callback: (response: SaveResponse) => void): void;
        protected set_entity(entity: any): void;
        protected set_entityId(id: any): void;
        protected showSaveSuccessMessage(response: SaveResponse): void;
        protected undelete(): void;
        protected undeleteHandler(options: ServiceOptions<UndeleteResponse>, callback: (response: UndeleteResponse) => void): void;
        protected updateInterface(): void;
        protected updateTitle(): void;
        protected validateBeforeSave(): boolean;
        protected propertyGrid: Serenity.PropertyGrid;
        static defaultLanguageList: () => string[][];
    }
}
declare namespace Serenity {
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
declare namespace Slick.Data {
    class GroupItemMetadataProvider {
        constructor();
    }
}
declare namespace Slick {
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
        get_onBeforeMoveRows(): Slick.Event;
        get_onMoveRows(): Slick.Event;
    }
    class RowSelectionModel {
    }
    class AutoTooltips {
        constructor(options: Slick.AutoTooltipsOptions);
    }
    interface AutoTooltipsOptions {
        enableForHeaderCells: boolean;
        enableForCells: boolean;
        maxToolTipLength: number;
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
        onAjaxCall: Slick.RemoteViewAjaxCallback<TEntity>;
        onProcessData: Slick.RemoteViewProcessCallback<TEntity>;
        addData(data: Serenity.ListResponse<TEntity>): void;
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
        getItem(row: number): any;
        params: any;
        sortBy: string[];
        url: string;
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
