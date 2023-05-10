/// <reference types="jquery" />
/// <reference types="jquery.validation" />
/**
 * Tests if any of array elements matches given predicate. Prefer Array.some() over this function (e.g. `[1, 2, 3].some(predicate)`).
 * @param array Array to test.
 * @param predicate Predicate to test elements.
 * @returns True if any element matches.
 */
declare function any<TItem>(array: TItem[], predicate: (x: TItem) => boolean): boolean;
/**
 * Counts number of array elements that matches a given predicate.
 * @param array Array to test.
 * @param predicate Predicate to test elements.
 */
declare function count<TItem>(array: TItem[], predicate: (x: TItem) => boolean): number;
/**
 * Gets first element in an array that matches given predicate similar to LINQ's First.
 * Throws an error if no match is found.
 * @param array Array to test.
 * @param predicate Predicate to test elements.
 * @returns First element that matches.
 */
declare function first<TItem>(array: TItem[], predicate: (x: TItem) => boolean): TItem;
/**
 * A group item returned by `groupBy()`.
 */
type GroupByElement<TItem> = {
    /** index of the item in `inOrder` array */
    order: number;
    /** key of the group */
    key: string;
    /** the items in the group */
    items: TItem[];
    /** index of the first item of this group in the original array */
    start: number;
};
/**
 * Return type of the `groupBy` function.
 */
type GroupByResult<TItem> = {
    byKey: {
        [key: string]: GroupByElement<TItem>;
    };
    inOrder: GroupByElement<TItem>[];
};
/**
 * Groups an array with keys determined by specified getKey() callback.
 * Resulting object contains group objects in order and a dictionary to access by key.
 * This is similar to LINQ's ToLookup function with some additional details like start index.
 * @param items Array to group.
 * @param getKey Function that returns key for each item.
 * @returns GroupByResult object.
 */
declare function groupBy<TItem>(items: TItem[], getKey: (x: TItem) => any): GroupByResult<TItem>;
/**
 * Gets index of first element in an array that matches given predicate.
 * @param array Array to test.
 * @param predicate Predicate to test elements.
 */
declare function indexOf<TItem>(array: TItem[], predicate: (x: TItem) => boolean): number;
/**
 * Inserts an item to the array at specified index. Prefer Array.splice unless
 * you need to support IE.
 * @param obj Array or array like object to insert to.
 * @param index Index to insert at.
 * @param item Item to insert.
 * @throws Error if object does not support insert.
 * @example
 * insert([1, 2, 3], 1, 4); // [1, 4, 2, 3]
 * insert({ insert: (index, item) => { this.splice(index, 0, item); } }
 */
declare function insert(obj: any, index: number, item: any): void;
/**
 * Determines if the object is an array. Prefer Array.isArray over this function (e.g. `Array.isArray(obj)`).
 * @param obj Object to test.
 * @returns True if the object is an array.
 * @example
 * isArray([1, 2, 3]); // true
 * isArray({}); // false
 */
declare const isArray: (arg: any) => arg is any[];
/**
* Gets first element in an array that matches given predicate.
* Throws an error if no matches is found, or there are multiple matches.
* @param array Array to test.
* @param predicate Predicate to test elements.
* @returns First element that matches.
* @example
* first([1, 2, 3], x => x == 2); // 2
* first([1, 2, 3], x => x == 4); // throws error.
*/
declare function single<TItem>(array: TItem[], predicate: (x: TItem) => boolean): TItem;
type Grouping<TItem> = {
    [key: string]: TItem[];
};
/**
 * Maps an array into a dictionary with keys determined by specified getKey() callback,
 * and values that are arrays containing elements for a particular key.
 * @param items Array to map.
 * @param getKey Function that returns key for each item.
 * @returns Grouping object.
 * @example
 * toGrouping([1, 2, 3], x => x % 2 == 0 ? "even" : "odd"); // { odd: [1, 3], even: [2] }
 */
declare function toGrouping<TItem>(items: TItem[], getKey: (x: TItem) => any): Grouping<TItem>;
/**
 * Gets first element in an array that matches given predicate (similar to LINQ's FirstOrDefault).
 * Returns null if no match is found.
 * @param array Array to test.
 * @param predicate Predicate to test elements.
 * @returns First element that matches.
 * @example
 * tryFirst([1, 2, 3], x => x == 2); // 2
 * tryFirst([1, 2, 3], x => x == 4); // null
 */
declare function tryFirst<TItem>(array: TItem[], predicate: (x: TItem) => boolean): TItem;

interface UserDefinition {
    /**
     * Username of the logged user
     */
    Username?: string;
    /**
     * Display name of the logged user
     */
    DisplayName?: string;
    /**
     * This indicates that the user is a super "admin", e.g. assumed to have all the permissions available.
     * It does not mean a member of Administrators, who might not have some of the permissions */
    IsAdmin?: boolean;
    /**
     * A hashset of permission keys that the current user have, explicitly assigned or via its
     * roles. Note that client side permission checks should only be used for UI enable/disable etc.
     * You should not rely on client side permission checks and always re-check permissions server side.
     */
    Permissions?: {
        [key: string]: boolean;
    };
}

/**
 * Contains permission related functions.
 *
 * ## Note
 * We use a namespace here both for compatibility and for allowing users to override
 * these functions easily in ES modules environment, which is normally hard to do.
 */
declare namespace Authorization {
    /**
     * Checks if the current user has the permission specified.
     * This should only be used for UI purposes and it is strongly recommended to check permissions server side.
     *
     * > Please prefer the `hasPermissionAsync` variant as this may block the UI thread if the `UserData` script is not already loaded.
     * @param permission Permission key. It may contain logical operators like A&B|C.
     * @returns `false` for "null or undefined", true for "*", `IsLoggedIn` for "?". For other permissions,
     * if the user has the permission or if the user has the `IsAdmin` flag (super admin) `true`, otherwise `false`.
     */
    function hasPermission(permission: string): boolean;
    /**
     * Checks if the current user has the permission specified.
     * This should only be used for UI purposes and it is strongly recommended to check permissions server side.
     *
     * @param permission Permission key. It may contain logical operators like A&B|C.
     * @returns `false` for "null or undefined", true for "*", `IsLoggedIn` for "?". For other permissions,
     * if the user has the permission or if the user has the `IsAdmin` flag (super admin) `true`, otherwise `false`.
     */
    function hasPermissionAsync(permission: string): Promise<boolean>;
    /**
     * Checks if the hashset contains the specified permission, also handling logical "|" and "&" operators
     * @param permissionSet Set of permissions
     * @param permission Permission key or a permission expression containing & | operators
     * @returns true if set contains permission
     */
    function isPermissionInSet(permissionSet: {
        [key: string]: boolean;
    }, permission: string): boolean;
    /**
     * Throws an error if the current user does not have the specified permission.
     * Prefer `await validatePermissionAsync()` as this one might block the UI if the `UserData`
     * is not already loaded.
     * @param permission Permission key. It may contain logical operators like A&B|C.
     */
    function validatePermission(permission: string): void;
    /**
    * Throws an error if the current user does not have the specified permission.
    * @param permission Permission key. It may contain logical operators like A&B|C.
    * @example
    * await Authorization.validatePermissionAsync("A&B|C");
    */
    function validatePermissionAsync(permission: string): Promise<void>;
}
declare namespace Authorization {
    /**
     * Checks if the current user is logged in. Prefer `isLoggedInAsync` as this one might block the UI if the `UserData`
     * is not already loaded.
     * @returns `true` if the user is logged in, `false` otherwise.
     * @example
     * if (Authorization.isLoggedIn) {
     *     // do something
     * }
     */
    let isLoggedIn: boolean;
    /**
     * Checks if the current user is logged in.
     * @returns `true` if the user is logged in, `false` otherwise.
     * @example
     * if (await Authorization.isLoggedInAsync) {
     *     // do something
     * }
     */
    let isLoggedInAsync: Promise<boolean>;
    /** Returns the username for currently logged user. Prefer `usernameAsync` as this one might block the UI if the `UserData`
     * is not already loaded.
     * @returns Username for currently logged user.
     * @example
     * if (Authorization.username) {
     *     // do something
     * }
     */
    let username: string;
    /** Returns the username for currently logged user.
     * @returns Username for currently logged user.
     * @example
     * if (await Authorization.usernameAsync) {
     *     // do something
     * }
     */
    let usernameAsync: Promise<string>;
    /** Returns the user data for currently logged user. Prefer `userDefinitionAsync` as this one might block the UI if the `UserData`
     * is not already loaded.
     * @returns User data for currently logged user.
     * @example
     * if (Authorization.userDefinition.IsAdmin) {
     *     // do something
     * }
     */
    let userDefinition: UserDefinition;
    /** Returns the user data for currently logged user.
     * @returns User data for currently logged user.
     * @example
     * if ((await Authorization.userDefinitionAsync).IsAdmin) {
     *     // do something
     * }
     */
    let userDefinitionAsync: Promise<UserDefinition>;
}

/** Options for the BlockUI plugin. */
interface JQBlockUIOptions {
    useTimeout?: boolean;
}
/**
 * Uses jQuery BlockUI plugin to block access to whole page (default) or
 * a part of it, by using a transparent overlay covering the whole area.
 * @param options Parameters for the BlockUI plugin
 * @remarks If options are not specified, this function blocks
 * whole page with a transparent overlay. Default z-order of the overlay
 * div is 2000, so a higher z-order shouldn't be used in page.
 */
declare function blockUI(options: JQBlockUIOptions): void;
/**
 * Unblocks the page.
 */
declare function blockUndo(): void;

declare var Config: {
    /**
     * This is the root path of your application. If your application resides under http://localhost/mysite/,
     * your root path is "mysite/". This variable is automatically initialized by reading from a <link> element
     * with ID "ApplicationPath" from current page, which is usually located in your _LayoutHead.cshtml file
     */
    applicationPath: string;
    /**
     * Email validation by default only allows ASCII characters. Set this to true if you want to allow unicode.
     */
    emailAllowOnlyAscii: boolean;
    /**
     * @Obsolete defaulted to false before for backward compatibility, now its true by default
     */
    responsiveDialogs: boolean;
    /**
     * Set this to true, to prefer bootstrap dialogs over jQuery UI dialogs by default for message dialogs
     */
    bootstrapMessages: boolean;
    /**
     * This is the list of root namespaces that may be searched for types. For example, if you specify an editor type
     * of "MyEditor", first a class with name "MyEditor" will be searched, if not found, search will be followed by
     * "Serenity.MyEditor" and "MyApp.MyEditor" if you added "MyApp" to the list of root namespaces.
     *
     * You should usually add your application root namespace to this list in ScriptInitialization.ts file.
     */
    rootNamespaces: string[];
    /**
     * This is an optional method for handling when user is not logged in. If a users session is expired
     * and when a NotAuthorized response is received from a service call, Serenity will call this handler, so
     * you may intercept it and notify user about this situation and ask if she wants to login again...
     */
    notLoggedInHandler: Function;
};

interface DebouncedFunction<T extends (...args: any[]) => any> {
    /**
     * Call the original function, but applying the debounce rules.
     *
     * If the debounced function can be run immediately, this calls it and returns its return
     * value.
     *
     * Otherwise, it returns the return value of the last invocation, or undefined if the debounced
     * function was not invoked yet.
     */
    (...args: Parameters<T>): ReturnType<T> | undefined;
    /**
     * Throw away any pending invocation of the debounced function.
     */
    clear(): void;
    /**
     * If there is a pending invocation of the debounced function, invoke it immediately and return
     * its return value.
     *
     * Otherwise, return the value from the last invocation, or undefined if the debounced function
     * was never invoked.
     */
    flush(): ReturnType<T> | undefined;
}
/**
 * Returns a function, that, as long as it continues to be invoked, will not
 * be triggered. The function also has a property 'clear' that can be used
 * to clear the timer to prevent previously scheduled executions, and flush method
 * to invoke scheduled executions now if any.
 * @param wait The function will be called after it stops being called for
 * N milliseconds.
 * @param immediate If passed, trigger the function on the leading edge, instead of the trailing.
 *
 * @source underscore.js
 */
declare function debounce<T extends (...args: any) => any>(func: T, wait?: number, immediate?: boolean): DebouncedFunction<T>;

/**
 * Options for a message dialog button
 */
interface DialogButton {
    /** Button text */
    text?: string;
    /** Button hint */
    hint?: string;
    /** Button icon */
    icon?: string;
    /** Click handler */
    click?: (e: MouseEvent) => void;
    /** CSS class for button */
    cssClass?: string;
    /** HTML encode button text. Default is true. */
    htmlEncode?: boolean;
    /** The code that is returned from message dialog function when this button is clicked */
    result?: string;
}
/**
 * Options that apply to all message dialog types
 */
interface CommonDialogOptions {
    /** Event handler that is called when dialog is opened */
    onOpen?: () => void;
    /** Event handler that is called when dialog is closed */
    onClose?: (result: string) => void;
    /** Dialog title */
    title?: string;
    /** HTML encode the message, default is true */
    htmlEncode?: boolean;
    /** Wrap the message in a `<pre>` element, so that line endings are preserved, default is true */
    preWrap?: boolean;
    /** Dialog css class. Default is based on the message dialog type */
    dialogClass?: string;
    /** List of buttons to show on the dialog */
    buttons?: DialogButton[];
    /** Class to use for the modal element for Bootstrap dialogs */
    modalClass?: string;
    /** True to use Bootstrap dialogs even when jQuery UI  present, default is based on `Q.Config.bootstrapMessages */
    bootstrap?: boolean;
    /** The result code of the button used to close the dialog is returned via this variable in the options object */
    result?: string;
}
/** Returns true if Bootstrap 3 is loaded */
declare function isBS3(): boolean;
/** Returns true if Bootstrap 5+ is loaded */
declare function isBS5Plus(): boolean;
/**
 * Builds HTML DIV element for a Bootstrap modal dialog
 * @param title Modal title
 * @param body Modal body, it will not be HTML encoded, so make sure it is encoded
 * @param modalClass Optional class to add to the modal element
 * @param escapeHtml True to html encode body, default is true
 * @returns
 */
declare function bsModalMarkup(title: string, body: string, modalClass?: string, escapeHtml?: boolean): HTMLDivElement;
declare function dialogButtonToBS(x: DialogButton): HTMLButtonElement;
declare function dialogButtonToUI(x: DialogButton): any;
/**
 * Additional options for Alert dialogs
 */
interface AlertOptions extends CommonDialogOptions {
    /** The title of OK button, or false to hide the OK button */
    okButton?: string | boolean;
    /** CSS class for OK button */
    okButtonClass?: string;
}
/**
 * Displays an alert dialog
 * @param message The message to display
 * @param options Additional options.
 * @see AlertOptions
 * @example
 * alertDialog("An error occured!"); }
 */
declare function alertDialog(message: string, options?: AlertOptions): void;
/** @obsolete use alertDialog */
declare const alert: typeof alertDialog;
/** Additional options for confirm dialog */
interface ConfirmOptions extends CommonDialogOptions {
    /** Title of the Yes button, or false to hide the Yes button. Default is value of local text: "Dialogs.YesButton" */
    yesButton?: string | boolean;
    /** CSS class for the Yes button. */
    yesButtonClass?: string;
    /** Title of the NO button, or false to hide the No button. Default is value of local text: "Dialogs.NoButton" */
    noButton?: string | boolean;
    /** Title of the CANCEL button, or false to hide the Cancel button. Default is value of local text: "Dialogs.NoButton" */
    cancelButton?: string | boolean;
    /** Event handler for cancel button click */
    onCancel?: () => void;
    /** Event handler for no button click */
    onNo?: () => void;
}
/**
 * Display a confirmation dialog
 * @param message The message to display
 * @param onYes Callback for Yes button click
 * @param options Additional options.
 * @see ConfirmOptions
 * @example
 * confirmDialog("Are you sure you want to delete?", () => {
 *     // do something when yes is clicked
 * }
 */
declare function confirmDialog(message: string, onYes: () => void, options?: ConfirmOptions): void;
/** @obsolete use confirmDialog */
declare const confirm: typeof confirmDialog;
/** Options for `iframeDialog` **/
interface IFrameDialogOptions {
    html?: string;
}
/**
 * Display a dialog that shows an HTML block in an IFRAME, which is usually returned from server callbacks
 * @param options The options
 */
declare function iframeDialog(options: IFrameDialogOptions): void;
/**
 * Display an information dialog
 * @param message The message to display
 * @param onOk Callback for OK button click
 * @param options Additional options.
 * @see ConfirmOptions
 * @example
 * informationDialog("Operation complete", () => {
 *     // do something when OK is clicked
 * }
 */
declare function informationDialog(message: string, onOk?: () => void, options?: ConfirmOptions): void;
/** @obsolete use informationDialog */
declare const information: typeof informationDialog;
/**
 * Display a success dialog
 * @param message The message to display
 * @param onOk Callback for OK button click
 * @param options Additional options.
 * @see ConfirmOptions
 * @example
 * successDialog("Operation complete", () => {
 *     // do something when OK is clicked
 * }
 */
declare function successDialog(message: string, onOk?: () => void, options?: ConfirmOptions): void;
/** @obsolete use successDialog */
declare const success: typeof successDialog;
/**
 * Display a warning dialog
 * @param message The message to display
 * @param options Additional options.
 * @see AlertOptions
 * @example
 * warningDialog("Something is odd!");
 */
declare function warningDialog(message: string, options?: AlertOptions): void;
/** @obsolete use warningDialog */
declare const warning: typeof warningDialog;
/**
 * Closes a panel, triggering panelbeforeclose and panelclose events.
 * If the panelbeforeclose prevents the default, the operation is cancelled.
 * @param element The panel element
 * @param e  The event triggering the close
 */
declare function closePanel(element: JQuery | HTMLElement, e?: Event): void;
declare function openPanel(element: JQuery | HTMLElement, uniqueName?: string): void;

interface ServiceError {
    Code?: string;
    Arguments?: string;
    Message?: string;
    Details?: string;
    ErrorId?: string;
}
interface ServiceResponse {
    Error?: ServiceError;
}
interface ServiceRequest {
}
interface ServiceOptions<TResponse extends ServiceResponse> extends JQueryAjaxSettings {
    request?: any;
    service?: string;
    blockUI?: boolean;
    onError?(response: TResponse): void;
    onSuccess?(response: TResponse): void;
    onCleanup?(): void;
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
declare enum ColumnSelection {
    List = 0,
    KeyOnly = 1,
    Details = 2,
    None = 3,
    IdOnly = 4,
    Lookup = 5
}
declare enum RetrieveColumnSelection {
    details = 0,
    keyOnly = 1,
    list = 2,
    none = 3,
    idOnly = 4,
    lookup = 5
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
    ExportColumns?: string[];
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

declare namespace ErrorHandling {
    function showServiceError(error: ServiceError): void;
    function runtimeErrorHandler(message: string, filename?: string, lineno?: number, colno?: number, error?: Error): void;
}

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
declare let Invariant: Locale;
declare function compareStringFactory(order: string): ((a: string, b: string) => number);
declare let Culture: Locale;
declare function turkishLocaleToUpper(a: string): string;
declare let turkishLocaleCompare: (a: string, b: string) => number;
declare function format(format: string, ...prm: any[]): string;
declare function localeFormat(format: string, l: Locale, ...prm: any[]): string;
declare let round: (n: number, d?: number, rounding?: boolean) => number;
declare let trunc: (n: number) => number;
declare function formatNumber(num: number, format?: string, decOrLoc?: string | NumberFormat, grp?: string): string;
declare function parseInteger(s: string): number;
declare function parseDecimal(s: string): number;
declare function toId(id: any): any;
declare function formatDate(d: Date | string, format?: string, locale?: Locale): string;
declare function formatDayHourAndMin(n: number): string;
declare function formatISODateTimeUTC(d: Date): string;
declare function parseISODateTime(s: string): Date;
declare function parseHourAndMin(value: string): number;
declare function parseDayHourAndMin(s: string): number;
declare function parseDate(s: string, dateOrder?: string): Date;
declare function splitDateString(s: string): string[];

/**
 * Adds an empty option to the select.
 * @param select the select element
 */
declare function addEmptyOption(select: JQuery | HTMLSelectElement): void;
/**
 * Adds an option to the select.
 */
declare function addOption(select: JQuery | HTMLSelectElement, key: string, text: string): void;
/** @obsolete use htmlEncode as it also encodes quotes */
declare const attrEncode: typeof htmlEncode;
/** Clears the options in the select element */
declare function clearOptions(select: JQuery): void;
/**
 * Finds the first element with the given relative id to the source element.
 * It can handle underscores in the source element id.
 * @param element the source element
 * @param relativeId the relative id to the source element
 * @param context the context element (optional)
 * @returns the element with the given relative id to the source element.
 */
declare function findElementWithRelativeId(element: JQuery, relativeId: string, context?: HTMLElement): JQuery;
/**
 * Finds the first element with the given relative id to the source element.
 * It can handle underscores in the source element id.
 * @param element the source element
 * @param relativeId the relative id to the source element
 * @param context the context element (optional)
 * @returns the element with the given relative id to the source element.
 */
declare function findElementWithRelativeId(element: HTMLElement, relativeId: string, context?: HTMLElement): HTMLElement;
/**
 * Html encodes a string (encodes single and double quotes, & (ampersand), > and < characters)
 * @param s String (or number etc.) to be HTML encoded
 */
declare function htmlEncode(s: any): string;
/**
 * Creates a new DIV and appends it to the body.
 * @returns the new DIV element.
 */
declare function newBodyDiv(): JQuery;
/**
 * Returns the outer HTML of the element.
 */
declare function outerHtml(element: JQuery): string;
/**
 * Toggles the class on the element handling spaces like jQuery addClass does.
 * @param el the element
 * @param cls the class to toggle
 * @param remove if true, the class will be added, if false the class will be removed, otherwise it will be toggled.
 */
declare function toggleClass(el: Element, cls: string, remove?: boolean): void;

declare function autoFullHeight(element: JQuery): void;
declare function initFullHeightGridPage(gridDiv: JQuery, opt?: {
    noRoute?: boolean;
}): void;
declare function layoutFillHeightValue(element: JQuery): number;
declare function layoutFillHeight(element: JQuery): void;
declare function setMobileDeviceMode(): void;
declare function triggerLayoutOnShow(element: JQuery): void;
declare function centerDialog(el: JQuery): void;

declare namespace LayoutTimer {
    function store(key: number): void;
    function trigger(key: number): void;
    function onSizeChange(element: () => HTMLElement, handler: () => void, width?: boolean, height?: boolean): number;
    function onWidthChange(element: () => HTMLElement, handler: () => void): number;
    function onHeightChange(element: () => HTMLElement, handler: () => void): number;
    function onShown(element: () => HTMLElement, handler: () => void): number;
    function off(key: number): number;
}
declare function executeOnceWhenVisible(element: JQuery, callback: Function): void;
declare function executeEverytimeWhenVisible(element: JQuery, callback: Function, callNowIfVisible: boolean): void;

declare function localText(key: string): string;
/** @obsolete prefer localText for better discoverability */
declare const text: typeof localText;
declare function dbText(prefix: string): ((key: string) => string);
declare function prefixedText(prefix: string): (text: string, key: string | ((p?: string) => string)) => string;
declare function tryGetText(key: string): string;
declare function dbTryText(prefix: string): ((key: string) => string);
declare function proxyTexts(o: Record<string, any>, p: string, t: Record<string, any>): Object;
declare class LT {
    private key;
    static empty: LT;
    constructor(key: string);
    static add(key: string, value: string): void;
    get(): string;
    toString(): string;
    static initializeTextClass: (type: any, prefix: string) => void;
    static getDefault: (key: string, defaultText: string) => string;
}

interface LookupOptions<TItem> {
    idField?: string;
    parentIdField?: string;
    textField?: string;
    textFormatter?(item: TItem): string;
}
interface Lookup<TItem> {
    items: TItem[];
    itemById: {
        [key: string]: TItem;
    };
    idField: string;
    parentIdField: string;
    textField: string;
    textFormatter: (item: TItem) => string;
}
declare class Lookup<TItem> {
    items: TItem[];
    itemById: {
        [key: string]: TItem;
    };
    idField: string;
    parentIdField: string;
    textField: string;
    textFormatter: (item: TItem) => string;
    constructor(options: LookupOptions<TItem>, items?: TItem[]);
    update?(value: TItem[]): void;
}

type ToastContainerOptions = {
    containerId?: string;
    positionClass?: string;
    target?: string;
};
type ToastrOptions = ToastContainerOptions & {
    tapToDismiss?: boolean;
    toastClass?: string;
    showDuration?: number;
    onShown?: () => void;
    hideDuration?: number;
    onHidden?: () => void;
    closeMethod?: boolean;
    closeDuration?: number | false;
    closeEasing?: boolean;
    closeOnHover?: boolean;
    extendedTimeOut?: number;
    iconClass?: string;
    positionClass?: string;
    timeOut?: number;
    titleClass?: string;
    messageClass?: string;
    escapeHtml?: boolean;
    target?: string;
    closeHtml?: string;
    closeClass?: string;
    newestOnTop?: boolean;
    preventDuplicates?: boolean;
    onclick?: (event: MouseEvent) => void;
    onCloseClick?: (event: Event) => void;
    closeButton?: boolean;
    rtl?: boolean;
};
type NotifyMap = {
    type: string;
    iconClass: string;
    title?: string;
    message?: string;
};
declare class Toastr {
    private listener;
    private toastId;
    private previousToast;
    options: ToastrOptions;
    constructor(options?: ToastrOptions);
    private createContainer;
    getContainer(options?: ToastContainerOptions, create?: boolean): HTMLElement;
    error(message?: string, title?: string, opt?: ToastrOptions): HTMLElement | null;
    warning(message?: string, title?: string, opt?: ToastrOptions): HTMLElement | null;
    success(message?: string, title?: string, opt?: ToastrOptions): HTMLElement | null;
    info(message?: string, title?: string, opt?: ToastrOptions): HTMLElement | null;
    subscribe(callback: (response: Toastr) => void): void;
    publish(args: Toastr): void;
    clear(toastElement?: HTMLElement | null, clearOptions?: {
        force?: boolean;
    }): void;
    remove(toastElement?: HTMLElement | null): void;
    removeToast(toastElement: HTMLElement, options?: ToastrOptions): void;
    private clearContainer;
    private clearToast;
    private notify;
}

declare let defaultNotifyOptions: ToastrOptions;
declare function notifyWarning(message: string, title?: string, options?: ToastrOptions): void;
declare function notifySuccess(message: string, title?: string, options?: ToastrOptions): void;
declare function notifyInfo(message: string, title?: string, options?: ToastrOptions): void;
declare function notifyError(message: string, title?: string, options?: ToastrOptions): void;
declare function positionToastContainer(create: boolean, options?: ToastrOptions): void;

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
interface PropertyItemsData {
    items: PropertyItem[];
    additionalItems: PropertyItem[];
}
declare enum SummaryType {
    Disabled = -1,
    None = 0,
    Sum = 1,
    Avg = 2,
    Min = 3,
    Max = 4
}

interface HandleRouteEventArgs {
    handled: boolean;
    route: string;
    parts: string[];
    index: number;
}
declare namespace Router {
    let enabled: boolean;
    function navigate(hash: string, tryBack?: boolean, silent?: boolean): void;
    function replace(hash: string, tryBack?: boolean): void;
    function replaceLast(hash: string, tryBack?: boolean): void;
    function dialog(owner: JQuery, element: JQuery, hash: () => string): void;
    function resolve(hash?: string): void;
}

declare namespace ScriptData {
    function bindToChange(name: string, regClass: string, onChange: () => void): void;
    function triggerChange(name: string): void;
    function unbindFromChange(regClass: string): void;
    function ensure<TData = any>(name: string): TData;
    function ensureAsync<TData = any>(name: string): Promise<TData>;
    function reload<TData = any>(name: string): TData;
    function reloadAsync<TData = any>(name: string): Promise<TData>;
    function canLoad(name: string): boolean;
    function setRegisteredScripts(scripts: any[]): void;
    function set(name: string, value: any): void;
}
declare function getRemoteData<TData = any>(key: string): TData;
declare function getRemoteDataAsync<TData = any>(key: string): Promise<TData>;
declare function getLookup<TItem>(key: string): Lookup<TItem>;
declare function getLookupAsync<TItem>(key: string): Promise<Lookup<TItem>>;
declare function reloadLookup<TItem = any>(key: string): Lookup<TItem>;
declare function reloadLookupAsync<TItem = any>(key: string): Promise<Lookup<TItem>>;
declare function getColumns(key: string): PropertyItem[];
declare function getColumnsData(key: string): PropertyItemsData;
declare function getColumnsAsync(key: string): Promise<PropertyItem[]>;
declare function getColumnsDataAsync(key: string): Promise<PropertyItemsData>;
declare function getForm(key: string): PropertyItem[];
declare function getFormData(key: string): PropertyItemsData;
declare function getFormAsync(key: string): Promise<PropertyItem[]>;
declare function getFormDataAsync(key: string): Promise<PropertyItemsData>;
declare function getTemplate(key: string): string;
declare function getTemplateAsync(key: string): Promise<string>;
declare function canLoadScriptData(name: string): boolean;

declare function getCookie(name: string): any;
declare function serviceCall<TResponse extends ServiceResponse>(options: ServiceOptions<TResponse>): JQueryXHR;
declare function serviceRequest<TResponse extends ServiceResponse>(service: string, request?: any, onSuccess?: (response: TResponse) => void, options?: ServiceOptions<TResponse>): JQueryXHR;
declare function setEquality(request: ListRequest, field: string, value: any): void;
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
declare function parseQueryString(s?: string): {};
declare function postToService(options: PostToServiceOptions): void;
declare function postToUrl(options: PostToUrlOptions): void;
declare function resolveUrl(url: string): string;

/**
 * Checks if the string ends with the specified substring.
 * @param s String to check.
 * @param suffix Suffix to check.
 * @returns True if the string ends with the specified substring.
 */
declare function endsWith(s: string, suffix: string): boolean;
/**
 * Checks if the string is empty or null.
 * @param s String to check.
 * @returns True if the string is empty or null.
 */
declare function isEmptyOrNull(s: string): boolean;
/**
 * Checks if the string is empty or null or whitespace.
 * @param s String to check.
 * @returns True if the string is empty or null or whitespace.
 */
declare function isTrimmedEmpty(s: string): boolean;
/**
 * Pads the string to the left with the specified character.
 * @param s String to pad.
 * @param len Target length of the string.
 * @param ch Character to pad with.
 * @returns Padded string.
 */
declare function padLeft(s: string | number, len: number, ch?: string): any;
/**
 * Checks if the string starts with the prefix
 * @param s String to check.
 * @param prefix Prefix to check.
 * @returns True if the string starts with the prefix.
 */
declare function startsWith(s: string, prefix: string): boolean;
/**
 * Converts the string to single line by removing line end characters
 * @param str String to convert.
 */
declare function toSingleLine(str: string): string;
/**
 * Trims the whitespace characters from the end of the string
 */
declare var trimEnd: (s: string) => any;
/**
 * Trims the whitespace characters from the start of the string
 */
declare var trimStart: (s: string) => any;
/**
 * Trims the whitespace characters from the start and end of the string
 * This returns empty string even when the string is null or undefined.
 */
declare function trim(s: string): string;
/**
 * Trims the whitespace characters from the start and end of the string
 * Returns empty string if the string is null or undefined.
 */
declare function trimToEmpty(s: string): string;
/**
 * Trims the whitespace characters from the start and end of the string
 * Returns null if the string is null, undefined or whitespace.
 */
declare function trimToNull(s: string): string;
/**
 * Replaces all occurrences of the search string with the replacement string.
 * @param str String to replace.
 * @param find String to find.
 * @param replace String to replace with.
 * @returns Replaced string.
 */
declare function replaceAll(str: string, find: string, replace: string): string;
/**
 * Pads the start of string to make it the specified length.
 * @param s String to pad.
 * @param len Target length of the string.
 */
declare function zeroPad(n: number, len: number): string;

type Dictionary<TItem> = {
    [key: string]: TItem;
};
declare function coalesce(a: any, b: any): any;
declare function isValue(a: any): boolean;
declare let today: () => Date;
declare function extend<T = any>(a: T, b: T): T;
declare function deepClone<T = any>(a: T, a2?: any, a3?: any): T;
type Type = Function | Object;
interface TypeMember {
    name: string;
    type: MemberType;
    attr?: any[];
    getter?: string;
    setter?: string;
}
declare function getNested(from: any, name: string): any;
declare function getGlobalThis(): any;
declare function getType(name: string, target?: any): Type;
declare function getTypeNameProp(type: Type): string;
declare function setTypeNameProp(type: Type, value: string): void;
declare function getTypeFullName(type: Type): string;
declare function getTypeShortName(type: Type): string;
declare function getInstanceType(instance: any): any;
declare function isAssignableFrom(target: any, type: Type): boolean;
declare function isInstanceOfType(instance: any, type: Type): boolean;
declare function safeCast(instance: any, type: Type): any;
declare function cast(instance: any, type: Type): any;
declare function getBaseType(type: any): any;
declare function getAttributes(type: any, attrType: any, inherit?: boolean): any[];
declare enum MemberType {
    field = 4,
    property = 16
}
declare function getMembers(type: any, memberTypes: MemberType): TypeMember[];
declare function addTypeMember(type: any, member: TypeMember): TypeMember;
declare function getTypes(from?: any): any[];
declare function clearKeys(d: any): void;
declare function delegateCombine(delegate1: any, delegate2: any): any;
declare function getStateStore(key?: string): any;
declare namespace Enum {
    let toString: (enumType: any, value: number) => string;
    let getValues: (enumType: any) => any[];
}
declare let delegateRemove: (delegate1: any, delegate2: any) => any;
declare let isEnum: (type: any) => boolean;
declare function initFormType(typ: Function, nameWidgetPairs: any[]): void;
declare function prop(type: any, name: string, getter?: string, setter?: string): void;
declare function fieldsProxy<TRow>(): Readonly<Record<keyof TRow, string>>;
declare function keyOf<T>(prop: keyof T): keyof T;
declare function registerClass(type: any, name: string, intf?: any[]): void;
declare function registerEditor(type: any, name: string, intf?: any[]): void;
declare function registerEnum(type: any, name: string, enumKey?: string): void;
declare function registerInterface(type: any, name: string, intf?: any[]): void;
declare function addAttribute(type: any, attr: any): void;
declare class ISlickFormatter {
}
declare class EditorAttribute {
}
declare function initializeTypes(root: any, pre: string, limit: number): void;
declare class Exception extends Error {
    constructor(message: string);
}
declare class ArgumentNullException extends Exception {
    constructor(paramName: string, message?: string);
}
declare class InvalidCastException extends Exception {
    constructor(message: string);
}

declare function validatorAbortHandler(validator: any): void;
declare function validateOptions(options?: JQueryValidation.ValidationOptions): JQueryValidation.ValidationOptions;

declare function loadValidationErrorMessages(): void;
declare function getHighlightTarget(el: HTMLElement): HTMLElement;
declare function baseValidateOptions(): JQueryValidation.ValidationOptions;
declare function validateForm(form: JQuery, opt: JQueryValidation.ValidationOptions): JQueryValidation.Validator;
declare function addValidationRule(element: JQuery, eventClass: string, rule: (p1: JQuery) => string): JQuery;
declare function removeValidationRule(element: JQuery, eventClass: string): JQuery;

/**
 * CriteriaBuilder is a class that allows to build unary or binary criteria with completion support.
 */
declare class CriteriaBuilder extends Array {
    /**
     * Creates a between criteria.
     * @param fromInclusive from value
     * @param toInclusive to value
     */
    bw(fromInclusive: any, toInclusive: any): Array<any>;
    /**
     * Creates a contains criteria
     * @param value contains value
     */
    contains(value: string): Array<any>;
    /**
     * Creates a endsWith criteria
     * @param value endsWith value
     */
    endsWith(value: string): Array<any>;
    /**
     * Creates an equal (=) criteria
     * @param value equal value
     */
    eq(value: any): Array<any>;
    /**
     * Creates a greater than criteria
     * @param value greater than value
     */
    gt(value: any): Array<any>;
    /**
     * Creates a greater than or equal criteria
     * @param value greater than or equal value
     */
    ge(value: any): Array<any>;
    /**
     * Creates a in criteria
     * @param values in values
     */
    in(values: any[]): Array<any>;
    /**
     * Creates a IS NULL criteria
     */
    isNull(): Array<any>;
    /**
     * Creates a IS NOT NULL criteria
     */
    isNotNull(): Array<any>;
    /**
     * Creates a less than or equal to criteria
     * @param value less than or equal to value
     */
    le(value: any): Array<any>;
    /**
     * Creates a less than criteria
     * @param value less than value
     */
    lt(value: any): Array<any>;
    /**
     * Creates a not equal criteria
     * @param value not equal value
     */
    ne(value: any): Array<any>;
    /**
     * Creates a LIKE criteria
     * @param value like value
     */
    like(value: any): Array<any>;
    /**
     * Creates a STARTS WITH criteria
     * @param value startsWith value
     */
    startsWith(value: string): Array<any>;
    /**
     * Creates a NOT IN criteria
     * @param values array of NOT IN values
     */
    notIn(values: any[]): Array<any>;
    /**
     * Creates a NOT LIKE criteria
     * @param value not like value
     */
    notLike(value: any): Array<any>;
}
/**
 * Parses a criteria expression to Serenity Criteria array format.
 * The string may optionally contain parameters like `A >= @p1 and B < @p2`.
 * @param expression The criteria expression.
 * @param params The dictionary containing parameter values like { p1: 10, p2: 20 }.
 * @example
 * parseCriteria('A >= @p1 and B < @p2', { p1: 5, p2: 4 }) // [[[a], '>=' 5], 'and', [[b], '<', 4]]
 */
declare function parseCriteria(expression: string, params?: any): any[];
/**
 * Parses a criteria expression to Serenity Criteria array format.
 * The expression may contain parameter placeholders like `A >= ${p1}`
 * where p1 is a variable in the scope.
 * @param strings The string fragments.
 * @param values The tagged template arguments.
 * @example
 * var a = 5, b = 4;
 * parseCriteria`A >= ${a} and B < ${b}` // [[[a], '>=' 5], 'and', [[b], '<', 4]]
 */
declare function parseCriteria(strings: TemplateStringsArray, ...values: any[]): any[];
declare enum CriteriaOperator {
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
/**
 * Creates a new criteria builder containg the passed field name.
 * @param field The field name.
 */
declare function Criteria(field: string): CriteriaBuilder;
declare namespace Criteria {
    var and: (c1: any[], c2: any[], ...rest: any[][]) => any[];
    var Operator: typeof CriteriaOperator;
    var isEmpty: (c: any[]) => boolean;
    var join: (c1: any[], op: string, c2: any[]) => any[];
    var not: (c: any[]) => (string | any[])[];
    var or: (c1: any[], c2: any[], ...rest: any[][]) => any[];
    var paren: (c: any[]) => any[];
    var parse: typeof parseCriteria;
}

export { AlertOptions, ArgumentNullException, Authorization, ColumnSelection, CommonDialogOptions, Config, ConfirmOptions, Criteria, CriteriaBuilder, CriteriaOperator, Culture, DateFormat, DebouncedFunction, DeleteRequest, DeleteResponse, DialogButton, Dictionary, EditorAttribute, Enum, ErrorHandling, Exception, GroupByElement, GroupByResult, Grouping, HandleRouteEventArgs, IFrameDialogOptions, ISlickFormatter, InvalidCastException, Invariant, JQBlockUIOptions, LT, LayoutTimer, ListRequest, ListResponse, Locale, Lookup, LookupOptions, MemberType, NotifyMap, NumberFormat, PostToServiceOptions, PostToUrlOptions, PropertyItem, PropertyItemsData, RetrieveColumnSelection, RetrieveLocalizationRequest, RetrieveLocalizationResponse, RetrieveRequest, RetrieveResponse, Router, SaveRequest, SaveRequestWithAttachment, SaveResponse, SaveWithLocalizationRequest, ScriptData, ServiceError, ServiceOptions, ServiceRequest, ServiceResponse, SummaryType, ToastContainerOptions, Toastr, ToastrOptions, Type, TypeMember, UndeleteRequest, UndeleteResponse, UserDefinition, addAttribute, addEmptyOption, addOption, addTypeMember, addValidationRule, alert, alertDialog, any, attrEncode, autoFullHeight, baseValidateOptions, blockUI, blockUndo, bsModalMarkup, canLoadScriptData, cast, centerDialog, clearKeys, clearOptions, closePanel, coalesce, compareStringFactory, confirm, confirmDialog, count, dbText, dbTryText, debounce, deepClone, defaultNotifyOptions, delegateCombine, delegateRemove, dialogButtonToBS, dialogButtonToUI, endsWith, executeEverytimeWhenVisible, executeOnceWhenVisible, extend, fieldsProxy, findElementWithRelativeId, first, format, formatDate, formatDayHourAndMin, formatISODateTimeUTC, formatNumber, getAttributes, getBaseType, getColumns, getColumnsAsync, getColumnsData, getColumnsDataAsync, getCookie, getForm, getFormAsync, getFormData, getFormDataAsync, getGlobalThis, getHighlightTarget, getInstanceType, getLookup, getLookupAsync, getMembers, getNested, getRemoteData, getRemoteDataAsync, getStateStore, getTemplate, getTemplateAsync, getType, getTypeFullName, getTypeNameProp, getTypeShortName, getTypes, groupBy, htmlEncode, iframeDialog, indexOf, information, informationDialog, initFormType, initFullHeightGridPage, initializeTypes, insert, isArray, isAssignableFrom, isBS3, isBS5Plus, isEmptyOrNull, isEnum, isInstanceOfType, isTrimmedEmpty, isValue, keyOf, layoutFillHeight, layoutFillHeightValue, loadValidationErrorMessages, localText, localeFormat, newBodyDiv, notifyError, notifyInfo, notifySuccess, notifyWarning, openPanel, outerHtml, padLeft, parseCriteria, parseDate, parseDayHourAndMin, parseDecimal, parseHourAndMin, parseISODateTime, parseInteger, parseQueryString, positionToastContainer, postToService, postToUrl, prefixedText, prop, proxyTexts, registerClass, registerEditor, registerEnum, registerInterface, reloadLookup, reloadLookupAsync, removeValidationRule, replaceAll, resolveUrl, round, safeCast, serviceCall, serviceRequest, setEquality, setMobileDeviceMode, setTypeNameProp, single, splitDateString, startsWith, success, successDialog, text, toGrouping, toId, toSingleLine, today, toggleClass, triggerLayoutOnShow, trim, trimEnd, trimStart, trimToEmpty, trimToNull, trunc, tryFirst, tryGetText, turkishLocaleCompare, turkishLocaleToUpper, validateForm, validateOptions, validatorAbortHandler, warning, warningDialog, zeroPad };
