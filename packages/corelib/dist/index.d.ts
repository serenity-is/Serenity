/// <reference types="jquery" />
/// <reference types="jquery.validation" />
import { GroupTotals, Column, FormatterContext, Group, GroupItemMetadataProvider, EventEmitter, Grid, IPlugin, SelectionModel, Range, GridOptions } from '@serenity-is/sleekgrid';

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
     * @Obsolete defaulted to false before for backward compatibility, now it is true by default
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
     * You should usually add your application root namespace to this list in ScriptInit(ialization).ts file.
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
/** Converts a `DialogButton` declaration to Bootstrap button element
 * @param x Dialog button declaration
 * @returns Bootstrap button element
*/
declare function dialogButtonToBS(x: DialogButton): HTMLButtonElement;
/** Converts a `DialogButton` declaration to jQuery UI button type
 * @param x Dialog button declaration
 * @returns jQuery UI button type
 */
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
 * Closes a panel, triggering panelbeforeclose and panelclose events on the panel element.
 * If the panelbeforeclose prevents the default, the operation is cancelled.
 * @param element The panel element
 * @param e  The event triggering the close
 */
declare function closePanel(element: JQuery | HTMLElement, e?: Event): void;
/**
 * Opens a panel, triggering panelbeforeopen and panelopen events on the panel element,
 * and panelopening and panelopened events on the window.
 * If the panelbeforeopen prevents the default, the operation is cancelled.
 * @param element The panel element
 * @param uniqueName A unique name for the panel. If not specified, the panel id is used. If the panel has no id, a timestamp is used.
 * @param e The event triggering the open
 */
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
    /**
     * Shows a service error as an alert dialog. If the error
     * is null, has no message or code, it shows "??ERROR??".
     */
    function showServiceError(error: ServiceError): void;
    /**
     * Runtime error handler that shows a runtime error as a notification
     * by default only in development mode (@see isDevelopmentMode)
     * This function is assigned as window.onerror handler in
     * ScriptInit.ts for Serenity applications so that developers
     * can notice an error without having to check the browser console.
     */
    function runtimeErrorHandler(message: string, filename?: string, lineno?: number, colno?: number, error?: Error): void;
    /**
     * Determines if the current environment is development mode.
     * The runtimeErrorHandler (window.onerror) shows error notifications only
     * when this function returns true. The default implementation considers
     * the environment as development mode if the host is localhost, 127.0.0.1, ::1,
     * or a domain name that ends with .local/.localhost.
     * @returns true if the current environment is development mode, false otherwise.
     */
    function isDevelopmentMode(): boolean;
}

/**
 * Interface for number formatting, similar to .NET's NumberFormatInfo
 */
interface NumberFormat {
    /** Decimal separator */
    decimalSeparator: string;
    /** Group separator */
    groupSeparator?: string;
    /** Number of digits after decimal separator */
    decimalDigits?: number;
    /** Positive sign */
    positiveSign?: string;
    /** Negative sign */
    negativeSign?: string;
    /** Zero symbol */
    nanSymbol?: string;
    /** Percentage symbol */
    percentSymbol?: string;
    /** Currency symbol */
    currencySymbol?: string;
}
/** Interface for date formatting, similar to .NET's DateFormatInfo */
interface DateFormat {
    /** Date separator */
    dateSeparator?: string;
    /** Default date format string */
    dateFormat?: string;
    /** Date order, like dmy, or ymd */
    dateOrder?: string;
    /** Default date time format string */
    dateTimeFormat?: string;
    /** AM designator */
    amDesignator?: string;
    /** PM designator */
    pmDesignator?: string;
    /** Time separator */
    timeSeparator?: string;
    /** First day of week, 0 = Sunday, 1 = Monday */
    firstDayOfWeek?: number;
    /** Array of day names */
    dayNames?: string[];
    /** Array of short day names */
    shortDayNames?: string[];
    /** Array of two letter day names */
    minimizedDayNames?: string[];
    /** Array of month names */
    monthNames?: string[];
    /** Array of short month names */
    shortMonthNames?: string[];
}
/** Interface for a locale, similar to .NET's CultureInfo */
interface Locale extends NumberFormat, DateFormat {
    /** Locale string comparison function, similar to .NET's StringComparer */
    stringCompare?: (a: string, b: string) => number;
    /** Locale string to upper case function */
    toUpper?: (a: string) => string;
}
/** Invariant locale (e.g. CultureInfo.InvariantCulture) */
declare let Invariant: Locale;
/**
 * Factory for a function that compares two strings, based on a character order
 * passed in the `order` argument.
 */
declare function compareStringFactory(order: string): ((a: string, b: string) => number);
/**
 * Current culture, e.g. CultureInfo.CurrentCulture. This is overridden by
 * settings passed from a `<script>` element in the page with id `ScriptCulture`
 * containing a JSON object if available. This element is generally created in
 * the _LayoutHead.cshtml file for Serenity applications, so that the culture
 * settings determined server, can be passed to the client.
 */
declare let Culture: Locale;
/**
 * A string to lowercase function that handles special Turkish
 * characters like 'ı'. Left in for compatibility reasons.
 */
declare function turkishLocaleToLower(a: string): string;
/**
 * A string to uppercase function that handles special Turkish
 * characters like 'ı'. Left in for compatibility reasons.
 */
declare function turkishLocaleToUpper(a: string): string;
/**
 * This is an alias for Culture.stringCompare, left in for compatibility reasons.
 */
declare let turkishLocaleCompare: (a: string, b: string) => number;
/**
 * Formats a string with parameters similar to .NET's String.Format function
 * using current `Culture` locale settings.
 */
declare function format(format: string, ...prm: any[]): string;
/**
 * Formats a string with parameters similar to .NET's String.Format function
 * using the locale passed as the first argument.
 */
declare function localeFormat(l: Locale, format: string, ...prm: any[]): string;
/**
 * Rounds a number to specified digits or an integer number if digits are not specified.
 * @param n the number to round
 * @param d the number of digits to round to. default is zero.
 * @param rounding whether to use banker's rounding
 * @returns the rounded number
 */
declare let round: (n: number, d?: number, rounding?: boolean) => number;
/**
 * Truncates a number to an integer number.
 */
declare let trunc: (n: number) => number;
/**
 * Formats a number using the current `Culture` locale (or the passed locale) settings.
 * It supports format specifiers similar to .NET numeric formatting strings.
 * @param num the number to format
 * @param format the format specifier. default is 'g'.
 * See .NET numeric formatting strings documentation for more information.
 */
declare function formatNumber(num: number, format?: string, decOrLoc?: string | NumberFormat, grp?: string): string;
/**
 * Converts a string to an integer. The difference between parseInt and parseInteger
 * is that parseInteger will return null if the string is empty or null, whereas
 * parseInt will return NaN and parseInteger will use the current culture's group
 * and decimal separators.
 * @param s the string to parse
 */
declare function parseInteger(s: string): number;
/**
 * Converts a string to a decimal. The difference between parseFloat and parseDecimal
 * is that parseDecimal will return null if the string is empty or null, whereas
 * parseFloat will return NaN and parseDecimal will use the current culture's group
 * and decimal separators.
 * @param s the string to parse
 */
declare function parseDecimal(s: string): number;
/**
 * Converts a string to an ID. If the string is a number, it is returned as-is.
 * If the string is empty, null or whitespace, null is returned.
 * Otherwise, it is converted to a number if possible. If the string is not a
 * valid number or longer than 14 digits, the trimmed string is returned as-is.
 * @param id the string to convert to an ID
 */
declare function toId(id: any): any;
/**
 * Formats a date using the specified format string and optional culture.
 * Supports .NET style format strings including custom formats.
 * See .NET documentation for supported formats.
 * @param d the date to format. If null, it returns empty string.
 * @param format the format string to use. If null, it uses the current culture's default format.
 * 'G' uses the culture's datetime format.
 * 'g' uses the culture's datetime format with secs removed.
 * 'd' uses the culture's date format.
 * 't' uses the culture's time format.
 * 'u' uses the sortable ISO format with UTC time.
 * 'U' uses the culture's date format with UTC time.
 * @param locale the locale to use
 * @returns the formatted date
 * @example
 * // returns "2019-01-01"
 * formatDate(new Date(2019, 0, 1), "yyyy-MM-dd");
 * @example
 * // returns "2019-01-01 12:00:00"
 * formatDate(new Date(2019, 0, 1, 12), "yyyy-MM-dd HH:mm:ss");
 * @example
 * // returns "2019-01-01 12:00:00.000"
 * formatDate(new Date(2019, 0, 1, 12), "yyyy-MM-dd HH:mm:ss.fff");
 * @example
 * // returns "2019-01-01 12:00:00.000 AM"
 * formatDate(new Date(2019, 0, 1, 12), "yyyy-MM-dd HH:mm:ss.fff tt");
 */
declare function formatDate(d: Date | string, format?: string, locale?: Locale): string;
/**
 * Formats a number containing number of minutes into a string in the format "d.hh:mm".
 * @param n The number of minutes.
 */
declare function formatDayHourAndMin(n: number): string;
/**
 * Formats a date as the ISO 8601 UTC date/time format.
 * @param n The number of minutes.
 */
declare function formatISODateTimeUTC(d: Date): string;
/**
 * Parses a string in the ISO 8601 UTC date/time format.
 * @param s The string to parse.
 */
declare function parseISODateTime(s: string): Date;
/**
 * Parses a time string in the format "hh:mm" into a number containing number of minutes.
 * Returns NaN if the hours not in range 0-23 or minutes not in range 0-59.
 * @param s The string to parse.
 */
declare function parseHourAndMin(value: string): number;
/**
 * Parses a string in the format "d.hh:mm" into a number containing number of minutes.
 * Returns NaN if the hours not in range 0-23 or minutes not in range 0-59.
 * Returns NULL if the string is empty or whitespace.
 */
declare function parseDayHourAndMin(s: string): number;
/**
 * Parses a string to a date. If the string is empty or whitespace, returns null.
 * Returns a NaN Date if the string is not a valid date.
 * @param s The string to parse.
 * @param dateOrder The order of the date parts in the string. Defaults to culture's default date order.
  */
declare function parseDate(s: string, dateOrder?: string): Date;
/**
 * Splits a date string into an array of strings, each containing a single date part.
 * It can handle separators "/", ".", "-" and "\".
 * @param s The string to split.
 */
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

declare function initFullHeightGridPage(gridDiv: JQuery | HTMLElement, opt?: {
    noRoute?: boolean;
    setHeight?: boolean;
}): void;
declare function layoutFillHeightValue(element: JQuery): number;
declare function layoutFillHeight(element: JQuery): void;
declare function isMobileView(): boolean;
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
/**
 * Enumeration of Criteria operator keys.
 */
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

declare namespace Aggregators {
    function Avg(field: string): void;
    function WeightedAvg(field: string, weightedField: string): void;
    function Min(field: string): void;
    function Max(field: string): void;
    function Sum(field: string): void;
}
declare namespace AggregateFormatting {
    function formatMarkup<TItem = any>(totals: GroupTotals, column: Column<TItem>, aggType: string): string;
    function formatValue(column: Column, value: number): string;
    function groupTotalsFormatter<TItem = any>(totals: GroupTotals, column: Column<TItem>): string;
}

type Format<TItem = any> = (ctx: FormatterContext<TItem>) => string;
declare module "@serenity-is/sleekgrid" {
    interface Column<TItem = any> {
        referencedFields?: string[];
        sourceItem?: PropertyItem;
    }
}
interface Formatter {
    format(ctx: FormatterContext): string;
}
interface GroupInfo<TItem> {
    getter?: any;
    formatter?: (p1: Group<TItem>) => string;
    comparer?: (a: Group<TItem>, b: Group<TItem>) => number;
    aggregators?: any[];
    aggregateCollapsed?: boolean;
    lazyTotalsCalculation?: boolean;
}
interface PagerOptions {
    view?: any;
    showRowsPerPage?: boolean;
    rowsPerPage?: number;
    rowsPerPageOptions?: number[];
    onChangePage?: (newPage: number) => void;
    onRowsPerPageChange?: (n: number) => void;
}
interface SummaryOptions {
    aggregators: any[];
}
interface PagingOptions {
    rowsPerPage?: number;
    page?: number;
}

interface RemoteViewOptions {
    autoLoad?: boolean;
    idField?: string;
    contentType?: string;
    dataType?: string;
    filter?: any;
    params?: any;
    onSubmit?: CancellableViewCallback<any>;
    url?: string;
    localSort?: boolean;
    sortBy?: any;
    rowsPerPage?: number;
    seekToPage?: number;
    onProcessData?: RemoteViewProcessCallback<any>;
    method?: string;
    inlineFilters?: boolean;
    groupItemMetadataProvider?: GroupItemMetadataProvider;
    onAjaxCall?: RemoteViewAjaxCallback<any>;
    getItemMetadata?: (p1?: any, p2?: number) => any;
    errorMsg?: string;
}
interface PagingInfo {
    rowsPerPage: number;
    page: number;
    totalCount: number;
    loading: boolean;
    error: string;
    dataView: RemoteView<any>;
}
type CancellableViewCallback<TEntity> = (view: RemoteView<TEntity>) => boolean | void;
type RemoteViewAjaxCallback<TEntity> = (view: RemoteView<TEntity>, options: JQueryAjaxSettings) => boolean | void;
type RemoteViewFilter<TEntity> = (item: TEntity, view: RemoteView<TEntity>) => boolean;
type RemoteViewProcessCallback<TEntity> = (data: ListResponse<TEntity>, view: RemoteView<TEntity>) => ListResponse<TEntity>;
interface RemoteView<TEntity> {
    onSubmit: CancellableViewCallback<TEntity>;
    onDataChanged: EventEmitter;
    onDataLoading: EventEmitter;
    onDataLoaded: EventEmitter;
    onPagingInfoChanged: EventEmitter;
    onRowCountChanged: EventEmitter;
    onRowsChanged: EventEmitter;
    onRowsOrCountChanged: EventEmitter;
    getPagingInfo(): PagingInfo;
    onGroupExpanded: EventEmitter;
    onGroupCollapsed: EventEmitter;
    onAjaxCall: RemoteViewAjaxCallback<TEntity>;
    onProcessData: RemoteViewProcessCallback<TEntity>;
    addData(data: ListResponse<TEntity>): void;
    beginUpdate(): void;
    endUpdate(): void;
    deleteItem(id: any): void;
    getItems(): TEntity[];
    setFilter(filter: RemoteViewFilter<TEntity>): void;
    getFilter(): RemoteViewFilter<TEntity>;
    getFilteredItems(): any;
    getGroupItemMetadataProvider(): GroupItemMetadataProvider;
    setGroupItemMetadataProvider(value: GroupItemMetadataProvider): void;
    fastSort: any;
    setItems(items: any[], newIdProperty?: boolean | string): void;
    getIdPropertyName(): string;
    getItemById(id: any): TEntity;
    getGrandTotals(): any;
    getGrouping(): GroupInfo<TEntity>[];
    getGroups(): any[];
    getRowById(id: any): number;
    getRowByItem(item: any): number;
    getRows(): any[];
    mapItemsToRows(itemArray: any[]): any[];
    mapRowsToIds(rowArray: number[]): any[];
    mapIdsToRows(idAray: any[]): number[];
    setFilterArgs(args: any): void;
    setRefreshHints(hints: any[]): void;
    insertItem(insertBefore: number, item: any): void;
    sortedAddItem(item: any): void;
    sortedUpdateItem(id: any, item: any): void;
    syncGridSelection(grid: any, preserveHidden?: boolean, preserveHiddenOnSelectionChange?: boolean): void;
    syncGridCellCssStyles(grid: any, key: string): void;
    getItemMetadata(i: number): any;
    updateItem(id: any, item: TEntity): void;
    addItem(item: TEntity): void;
    getIdxById(id: any): any;
    getItemByIdx(index: number): any;
    setGrouping(groupInfo: GroupInfo<TEntity>[]): void;
    collapseAllGroups(level: number): void;
    expandAllGroups(level: number): void;
    expandGroup(keys: any[]): void;
    collapseGroup(keys: any[]): void;
    setSummaryOptions(options: SummaryOptions): void;
    setPagingOptions(options: PagingOptions): void;
    refresh(): void;
    populate(): void;
    populateLock(): void;
    populateUnlock(): void;
    getItem(row: number): any;
    getLength(): number;
    rowsPerPage: number;
    errormsg: string;
    params: any;
    getLocalSort(): boolean;
    setLocalSort(value: boolean): void;
    sort(comparer?: (a: any, b: any) => number, ascending?: boolean): void;
    reSort(): void;
    sortBy: string[];
    url: string;
    method: string;
    idField: string;
    seekToPage?: number;
}
declare class RemoteView<TEntity> {
    constructor(options: RemoteViewOptions);
}

declare global {
    namespace Select2 {
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
}

declare global {
    interface JQueryStatic {
        extend<T>(target: T, object1?: T, ...objectN: T[]): T;
        toJSON(obj: any): string;
    }
}

declare global {
    namespace JQueryValidation {
        interface ValidationOptions {
            normalizer?: (v: string) => string;
        }
    }
}

declare class IBooleanValue {
}
interface IBooleanValue {
    get_value(): boolean;
    set_value(value: boolean): void;
}

declare class IDoubleValue {
}
interface IDoubleValue {
    get_value(): any;
    set_value(value: any): void;
}

declare class IDialog {
}
interface IDialog {
    dialogOpen(asPanel?: boolean): void;
}

declare class IEditDialog {
}
interface IEditDialog {
    load(entityOrId: any, done: () => void, fail?: (p1: any) => void): void;
}

declare class IGetEditValue {
}
interface IGetEditValue {
    getEditValue(property: PropertyItem, target: any): void;
}

interface IReadOnly {
    get_readOnly(): boolean;
    set_readOnly(value: boolean): void;
}
declare class IReadOnly {
}

declare class ISetEditValue {
}
interface ISetEditValue {
    setEditValue(source: any, property: PropertyItem): void;
}

declare class IStringValue {
}
interface IStringValue {
    get_value(): string;
    set_value(value: string): void;
}

interface IValidateRequired {
    get_required(): boolean;
    set_required(value: boolean): void;
}
declare class IValidateRequired {
}

declare enum CaptureOperationType {
    Before = 0,
    Delete = 1,
    Insert = 2,
    Update = 3
}

interface DataChangeInfo {
    type: string;
    entityId: any;
    entity: any;
}

declare namespace ReflectionUtils {
    function getPropertyValue(o: any, property: string): any;
    function setPropertyValue(o: any, property: string, value: any): void;
    function makeCamelCase(s: string): string;
}

declare namespace DialogTypeRegistry {
    function get(key: string): any;
    function reset(): void;
    function tryGet(key: string): any;
}

declare namespace EditorTypeRegistry {
    function get(key: string): any;
    function reset(): void;
    function tryGet(key: string): any;
}

declare namespace EnumTypeRegistry {
    function get(key: string): Function;
    function reset(): void;
    function tryGet(key: string): any;
}

interface IRowDefinition {
    readonly deletePermission?: string;
    readonly idProperty?: string;
    readonly insertPermission?: string;
    readonly isActiveProperty?: string;
    readonly isDeletedProperty?: string;
    readonly localTextPrefix?: string;
    readonly nameProperty?: string;
    readonly readPermission?: string;
    readonly updatePermission?: string;
}

declare class EnumKeyAttribute {
    value: string;
    constructor(value: string);
}
declare class DisplayNameAttribute {
    displayName: string;
    constructor(displayName: string);
}
declare class CategoryAttribute {
    category: string;
    constructor(category: string);
}
declare class ColumnsKeyAttribute {
    value: string;
    constructor(value: string);
}
declare class CssClassAttribute {
    cssClass: string;
    constructor(cssClass: string);
}
declare class DefaultValueAttribute {
    value: any;
    constructor(value: any);
}
declare class DialogTypeAttribute {
    value: any;
    constructor(value: any);
}
declare class EditorOptionAttribute {
    key: string;
    value: any;
    constructor(key: string, value: any);
}
declare class EditorTypeAttributeBase {
    editorType: string;
    constructor(editorType: string);
    setParams(editorParams: any): void;
}
declare class EditorTypeAttribute extends EditorTypeAttributeBase {
    constructor(editorType: string);
}
declare class ElementAttribute {
    value: string;
    constructor(value: string);
}
declare class EntityTypeAttribute {
    value: string;
    constructor(value: string);
}
declare class FilterableAttribute {
    value: boolean;
    constructor(value?: boolean);
}
declare class FlexifyAttribute {
    value: boolean;
    constructor(value?: boolean);
}
declare class FormKeyAttribute {
    value: string;
    constructor(value: string);
}
declare class GeneratedCodeAttribute {
    origin?: string;
    constructor(origin?: string);
}
declare class HiddenAttribute {
    constructor();
}
declare class HintAttribute {
    hint: string;
    constructor(hint: string);
}
declare class IdPropertyAttribute {
    value: string;
    constructor(value: string);
}
declare class InsertableAttribute {
    value: boolean;
    constructor(value?: boolean);
}
declare class IsActivePropertyAttribute {
    value: string;
    constructor(value: string);
}
declare class ItemNameAttribute {
    value: string;
    constructor(value: string);
}
declare class LocalTextPrefixAttribute {
    value: string;
    constructor(value: string);
}
declare class MaximizableAttribute {
    value: boolean;
    constructor(value?: boolean);
}
declare class MaxLengthAttribute {
    maxLength: number;
    constructor(maxLength: number);
}
declare class NamePropertyAttribute {
    value: string;
    constructor(value: string);
}
declare class OneWayAttribute {
}
declare class OptionAttribute {
}
declare class OptionsTypeAttribute {
    value: Function;
    constructor(value: Function);
}
declare class PanelAttribute {
    value: boolean;
    constructor(value?: boolean);
}
declare class PlaceholderAttribute {
    value: string;
    constructor(value: string);
}
declare class ReadOnlyAttribute {
    value: boolean;
    constructor(value?: boolean);
}
declare class RequiredAttribute {
    isRequired: boolean;
    constructor(isRequired?: boolean);
}
declare class ResizableAttribute {
    value: boolean;
    constructor(value?: boolean);
}
declare class ResponsiveAttribute {
    value: boolean;
    constructor(value?: boolean);
}
declare class ServiceAttribute {
    value: string;
    constructor(value: string);
}
declare class UpdatableAttribute {
    value: boolean;
    constructor(value?: boolean);
}
declare namespace Decorators {
    function registerClass(nameOrIntf?: string | any[], intf2?: any[]): (target: Function) => void;
    function registerInterface(nameOrIntf?: string | any[], intf2?: any[]): (target: Function) => void;
    function registerEditor(nameOrIntf?: string | any[], intf2?: any[]): (target: Function) => void;
    function registerEnum(target: any, enumKey?: string, name?: string): void;
    function registerEnumType(target: any, name?: string, enumKey?: string): void;
    function registerFormatter(nameOrIntf?: string | any[], intf2?: any[]): (target: Function) => void;
    function enumKey(value: string): (target: Function) => void;
    function option(): (target: Object, propertyKey: string) => void;
    function dialogType(value: any): (target: Function) => void;
    function editor(): (target: Function) => void;
    function element(value: string): (target: Function) => void;
    function filterable(value?: boolean): (target: Function) => void;
    function flexify(value?: boolean): (target: Function) => void;
    function itemName(value: string): (target: Function) => void;
    function maximizable(value?: boolean): (target: Function) => void;
    function optionsType(value: Function): (target: Function) => void;
    function panel(value?: boolean): (target: Function) => void;
    function resizable(value?: boolean): (target: Function) => void;
    function responsive(value?: boolean): (target: Function) => void;
    function service(value: string): (target: Function) => void;
}

declare namespace LazyLoadHelper {
    const executeOnceWhenShown: typeof executeOnceWhenVisible;
    const executeEverytimeWhenShown: typeof executeEverytimeWhenVisible;
}

declare class PrefixedContext {
    readonly idPrefix: string;
    constructor(idPrefix: string);
    byId(id: string): JQuery;
    w<TWidget>(id: string, type: {
        new (...args: any[]): TWidget;
    }): TWidget;
}

interface WidgetClass<TOptions = object> {
    new (element: JQuery, options?: TOptions): Widget<TOptions>;
    element: JQuery;
}
interface WidgetDialogClass<TOptions = object> {
    new (options?: TOptions): Widget<TOptions> & IDialog;
    element: JQuery;
}
type AnyWidgetClass<TOptions = object> = WidgetClass<TOptions> | WidgetDialogClass<TOptions>;
declare function reactPatch(): void;
interface CreateWidgetParams<TWidget extends Widget<TOptions>, TOptions> {
    type?: new (element: JQuery, options?: TOptions) => TWidget;
    options?: TOptions;
    container?: JQuery;
    element?: (e: JQuery) => void;
    init?: (w: TWidget) => void;
}
interface WidgetComponentProps<W extends Widget<any>> {
    id?: string;
    name?: string;
    className?: string;
    maxLength?: number;
    placeholder?: string;
    setOptions?: any;
    required?: boolean;
    readOnly?: boolean;
    oneWay?: boolean;
    onChange?: (e: JQueryEventObject) => void;
    onChangeSelect2?: (e: JQueryEventObject) => void;
    value?: any;
    defaultValue?: any;
}
declare class Widget<TOptions = any> {
    private static nextWidgetNumber;
    element: JQuery;
    protected options: TOptions;
    protected widgetName: string;
    protected uniqueName: string;
    readonly idPrefix: string;
    constructor(element: JQuery, options?: TOptions);
    destroy(): void;
    protected addCssClass(): void;
    protected getCssClass(): string;
    static getWidgetName(type: Function): string;
    static elementFor<TWidget>(editorType: {
        new (...args: any[]): TWidget;
    }): JQuery;
    addValidationRule(eventClass: string, rule: (p1: JQuery) => string): JQuery;
    getGridField(): JQuery;
    static create<TWidget extends Widget<TOpt>, TOpt>(params: CreateWidgetParams<TWidget, TOpt>): TWidget;
    initialize(): void;
    init(action?: (widget: any) => void): this;
    protected renderContents(): void;
    private static __isWidgetType;
}
declare interface Widget<TOptions> {
    change(handler: (e: JQueryEventObject) => void): void;
    changeSelect2(handler: (e: JQueryEventObject) => void): void;
}

declare global {
    interface JQuery {
        getWidget<TWidget>(widgetType: {
            new (...args: any[]): TWidget;
        }): TWidget;
        tryGetWidget<TWidget>(widgetType: {
            new (...args: any[]): TWidget;
        }): TWidget;
        flexHeightOnly(flexY?: number): JQuery;
        flexWidthOnly(flexX?: number): JQuery;
        flexWidthHeight(flexX: number, flexY: number): JQuery;
        flexX(flexX: number): JQuery;
        flexY(flexY: number): JQuery;
    }
}

interface ToolButton {
    action?: string;
    title?: string;
    hint?: string;
    cssClass?: string;
    icon?: string;
    onClick?: any;
    htmlEncode?: any;
    hotkey?: string;
    hotkeyAllowDefault?: boolean;
    hotkeyContext?: any;
    separator?: (false | true | 'left' | 'right' | 'both');
    visible?: boolean | (() => boolean);
    disabled?: boolean | (() => boolean);
}
interface PopupMenuButtonOptions {
    menu?: JQuery;
    onPopup?: () => void;
    positionMy?: string;
    positionAt?: string;
}
declare class PopupMenuButton extends Widget<PopupMenuButtonOptions> {
    constructor(div: JQuery, opt: PopupMenuButtonOptions);
    destroy(): void;
}
interface PopupToolButtonOptions extends PopupMenuButtonOptions {
}
declare class PopupToolButton extends PopupMenuButton {
    constructor(div: JQuery, opt: PopupToolButtonOptions);
}
interface ToolbarOptions {
    buttons?: ToolButton[];
    hotkeyContext?: any;
}
declare class Toolbar extends Widget<ToolbarOptions> {
    constructor(div: JQuery, options: ToolbarOptions);
    destroy(): void;
    protected mouseTrap: any;
    protected createButtons(): void;
    protected createButton(container: JQuery, b: ToolButton): void;
    findButton(className: string): JQuery;
    updateInterface(): void;
}

declare class TemplatedWidget<TOptions> extends Widget<TOptions> {
    private static templateNames;
    protected byId(id: string): JQuery;
    private byID;
    private static noGeneric;
    private getDefaultTemplateName;
    protected getTemplateName(): string;
    protected getFallbackTemplate(): string;
    protected getTemplate(): string;
    protected renderContents(): void;
    protected useIdPrefix(): IdPrefixType;
}
type IdPrefixType = {
    [key: string]: string;
    Form: string;
    Tabs: string;
    Toolbar: string;
    PropertyGrid: string;
};
declare function useIdPrefix(prefix: string): IdPrefixType;

declare class TemplatedDialog<TOptions> extends TemplatedWidget<TOptions> {
    protected tabs: JQuery;
    protected toolbar: Toolbar;
    protected validator: JQueryValidation.Validator;
    constructor(options?: TOptions);
    private get isMarkedAsPanel();
    private get isResponsive();
    private static getCssSize;
    private static applyCssSizes;
    destroy(): void;
    protected initDialog(): void;
    protected getModalOptions(): ModalOptions;
    protected initModal(): void;
    protected initToolbar(): void;
    protected getToolbarButtons(): ToolButton[];
    protected getValidatorOptions(): JQueryValidation.ValidationOptions;
    protected initValidator(): void;
    protected resetValidation(): void;
    protected validateForm(): boolean;
    dialogOpen(asPanel?: boolean): void;
    private useBSModal;
    static bootstrapModal: boolean;
    static openPanel(element: JQuery, uniqueName: string): void;
    static closePanel(element: JQuery, e?: JQueryEventObject): void;
    protected onDialogOpen(): void;
    arrange(): void;
    protected onDialogClose(): void;
    protected addCssClass(): void;
    protected getDialogButtons(): DialogButton[];
    protected getDialogOptions(): any;
    protected getDialogTitle(): string;
    dialogClose(): void;
    get dialogTitle(): string;
    private setupPanelTitle;
    set dialogTitle(value: string);
    set_dialogTitle(value: string): void;
    protected initTabs(): void;
    protected handleResponsive(): void;
}
interface ModalOptions {
    backdrop?: boolean | 'static';
    keyboard?: boolean;
    size?: 'lg' | 'sm';
    modalClass?: string;
}

declare class TemplatedPanel<TOptions> extends TemplatedWidget<TOptions> {
    constructor(container: JQuery, options?: TOptions);
    destroy(): void;
    protected tabs: JQuery;
    protected toolbar: Toolbar;
    protected validator: JQueryValidation.Validator;
    protected isPanel: boolean;
    protected responsive: boolean;
    arrange(): void;
    protected getToolbarButtons(): ToolButton[];
    protected getValidatorOptions(): JQueryValidation.ValidationOptions;
    protected initTabs(): void;
    protected initToolbar(): void;
    protected initValidator(): void;
    protected resetValidation(): void;
    protected validateForm(): boolean;
}

declare namespace ValidationHelper {
    function asyncSubmit(form: JQuery, validateBeforeSave: () => boolean, submitHandler: () => void): boolean;
    function submit(form: JQuery, validateBeforeSave: () => boolean, submitHandler: () => void): boolean;
    function getValidator(element: JQuery): JQueryValidation.Validator;
}
declare namespace VX {
    function addValidationRule(element: JQuery, eventClass: string, rule: (p1: JQuery) => string): JQuery;
    function removeValidationRule(element: JQuery, eventClass: string): JQuery;
    function validateElement(validator: JQueryValidation.Validator, widget: Widget<any>): boolean;
}

declare class CascadedWidgetLink<TParent extends Widget<any>> {
    private parentType;
    private widget;
    private parentChange;
    constructor(parentType: {
        new (...args: any[]): TParent;
    }, widget: Widget<any>, parentChange: (p1: TParent) => void);
    private _parentID;
    bind(): TParent;
    unbind(): TParent;
    get_parentID(): string;
    set_parentID(value: string): void;
}

declare namespace TabsExtensions {
    function setDisabled(tabs: JQuery, tabKey: string, isDisabled: boolean): void;
    function toggle(tabs: JQuery, tabKey: string, visible: boolean): void;
    function activeTabKey(tabs: JQuery): string;
    function indexByKey(tabs: JQuery): any;
    function selectTab(tabs: JQuery, tabKey: string): void;
}

declare namespace ReflectionOptionsSetter {
    function set(target: any, options: any): void;
}

declare class PropertyGrid extends Widget<PropertyGridOptions> {
    private editors;
    private items;
    readonly idPrefix: string;
    constructor(div: JQuery, opt: PropertyGridOptions);
    destroy(): void;
    private createItems;
    private createCategoryDiv;
    private categoryLinkClick;
    private determineText;
    private createField;
    private getCategoryOrder;
    private createCategoryLinks;
    get_editors(): Widget<any>[];
    get_items(): PropertyItem[];
    get_idPrefix(): string;
    get_mode(): PropertyGridMode;
    set_mode(value: PropertyGridMode): void;
    static loadEditorValue(editor: Widget<any>, item: PropertyItem, source: any): void;
    static saveEditorValue(editor: Widget<any>, item: PropertyItem, target: any): void;
    private static setReadOnly;
    private static setReadonly;
    private static setRequired;
    private static setMaxLength;
    load(source: any): void;
    save(target?: any): any;
    get value(): any;
    set value(val: any);
    private canModifyItem;
    updateInterface(): void;
    enumerateItems(callback: (p1: PropertyItem, p2: Widget<any>) => void): void;
}
declare enum PropertyGridMode {
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

declare class PropertyPanel<TItem, TOptions> extends TemplatedPanel<TOptions> {
    private _entity;
    private _entityId;
    constructor(container: JQuery, options?: TOptions);
    destroy(): void;
    protected initPropertyGrid(): void;
    protected loadInitialEntity(): void;
    protected getFormKey(): string;
    protected getPropertyGridOptions(): PropertyGridOptions;
    protected getPropertyItems(): PropertyItem[];
    protected getSaveEntity(): TItem;
    protected get_entity(): TItem;
    protected get_entityId(): any;
    protected set_entity(value: TItem): void;
    protected set_entityId(value: any): void;
    protected validateBeforeSave(): boolean;
    protected propertyGrid: PropertyGrid;
}

declare namespace SubDialogHelper {
    function bindToDataChange(dialog: any, owner: Widget<any>, dataChange: (p1: any, p2: DataChangeInfo) => void, useTimeout?: boolean): any;
    function triggerDataChange(dialog: Widget<any>): any;
    function triggerDataChanged(element: JQuery): JQuery;
    function bubbleDataChange(dialog: any, owner: Widget<any>, useTimeout?: boolean): any;
    function cascade(cascadedDialog: any, ofElement: JQuery): any;
    function cascadedDialogOffset(element: JQuery): any;
}

declare namespace DialogExtensions {
    function dialogResizable(dialog: JQuery, w?: any, h?: any, mw?: any, mh?: any): JQuery;
    function dialogMaximizable(dialog: JQuery): JQuery;
    function dialogFlexify(dialog: JQuery): JQuery;
}

declare class PropertyDialog<TItem, TOptions> extends TemplatedDialog<TOptions> {
    protected entity: TItem;
    protected entityId: any;
    protected propertyItemsData: PropertyItemsData;
    constructor(opt?: TOptions);
    internalInit(): void;
    protected initSync(): void;
    protected initAsync(): Promise<void>;
    protected afterInit(): void;
    protected useAsync(): boolean;
    destroy(): void;
    protected getDialogOptions(): any;
    protected getDialogButtons(): DialogButton[];
    protected okClick(): void;
    protected okClickValidated(): void;
    protected cancelClick(): void;
    protected initPropertyGrid(): void;
    protected getFormKey(): string;
    protected getPropertyGridOptions(): PropertyGridOptions;
    protected getPropertyItems(): PropertyItem[];
    protected getPropertyItemsData(): PropertyItemsData;
    protected getPropertyItemsDataAsync(): Promise<PropertyItemsData>;
    protected getSaveEntity(): TItem;
    protected loadInitialEntity(): void;
    protected get_entity(): TItem;
    protected set_entity(value: TItem): void;
    protected get_entityId(): any;
    protected set_entityId(value: any): void;
    protected validateBeforeSave(): boolean;
    protected updateTitle(): void;
    protected propertyGrid: PropertyGrid;
    protected getFallbackTemplate(): string;
}

declare namespace EditorUtils {
    function getDisplayText(editor: Widget<any>): string;
    function getValue(editor: Widget<any>): any;
    function saveValue(editor: Widget<any>, item: PropertyItem, target: any): void;
    function setValue(editor: Widget<any>, value: any): void;
    function loadValue(editor: Widget<any>, item: PropertyItem, source: any): void;
    function setReadonly(elements: JQuery, isReadOnly: boolean): JQuery;
    function setReadOnly(widget: Widget<any>, isReadOnly: boolean): void;
    function setRequired(widget: Widget<any>, isRequired: boolean): void;
    function setContainerReadOnly(container: JQuery, readOnly: boolean): void;
}

declare class StringEditor extends Widget<any> {
    constructor(input: JQuery);
    get value(): string;
    protected get_value(): string;
    set value(value: string);
    protected set_value(value: string): void;
}

declare class PasswordEditor extends StringEditor {
    constructor(input: JQuery);
}

interface TextAreaEditorOptions {
    cols?: number;
    rows?: number;
}
declare class TextAreaEditor extends Widget<TextAreaEditorOptions> {
    constructor(input: JQuery, opt?: TextAreaEditorOptions);
    get value(): string;
    protected get_value(): string;
    set value(value: string);
    protected set_value(value: string): void;
}

declare class BooleanEditor extends Widget<any> {
    get value(): boolean;
    protected get_value(): boolean;
    set value(value: boolean);
    protected set_value(value: boolean): void;
}

interface DecimalEditorOptions {
    minValue?: string;
    maxValue?: string;
    decimals?: any;
    padDecimals?: any;
    allowNegatives?: boolean;
}
declare class DecimalEditor extends Widget<DecimalEditorOptions> implements IDoubleValue {
    constructor(input: JQuery, opt?: DecimalEditorOptions);
    get_value(): number;
    get value(): number;
    set_value(value: number): void;
    set value(v: number);
    get_isValid(): boolean;
    static defaultAutoNumericOptions(): any;
}

interface IntegerEditorOptions {
    minValue?: number;
    maxValue?: number;
    allowNegatives?: boolean;
}
declare class IntegerEditor extends Widget<IntegerEditorOptions> implements IDoubleValue {
    constructor(input: JQuery, opt?: IntegerEditorOptions);
    get_value(): number;
    get value(): number;
    set_value(value: number): void;
    set value(v: number);
    get_isValid(): boolean;
}

declare let datePickerIconSvg: string;
declare class DateEditor extends Widget<any> implements IStringValue, IReadOnly {
    private minValue;
    private maxValue;
    constructor(input: JQuery);
    static useFlatpickr: boolean;
    static flatPickrOptions(input: JQuery): {
        clickOpens: boolean;
        allowInput: boolean;
        dateFormat: string;
        onChange: () => void;
    };
    get_value(): string;
    get value(): string;
    set_value(value: string): void;
    set value(v: string);
    private get_valueAsDate;
    get valueAsDate(): Date;
    private set_valueAsDate;
    set valueAsDate(v: Date);
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
    static flatPickrTrigger(input: JQuery): JQuery;
    static dateInputKeyup(e: JQueryEventObject): void;
    static uiPickerZIndexWorkaround(input: JQuery): void;
}

declare class DateTimeEditor extends Widget<DateTimeEditorOptions> implements IStringValue, IReadOnly {
    private minValue;
    private maxValue;
    private time;
    private lastSetValue;
    private lastSetValueGet;
    constructor(input: JQuery, opt?: DateTimeEditorOptions);
    getFlatpickrOptions(): any;
    get_value(): string;
    get value(): string;
    set_value(value: string): void;
    private getInplaceNowText;
    private getDisplayFormat;
    set value(v: string);
    private get_valueAsDate;
    get valueAsDate(): Date;
    private set_valueAsDate;
    set valueAsDate(value: Date);
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
    seconds?: boolean;
    inputOnly?: boolean;
}

interface TimeEditorOptions {
    noEmptyOption?: boolean;
    startHour?: any;
    endHour?: any;
    intervalMinutes?: any;
}
declare class TimeEditor extends Widget<TimeEditorOptions> {
    private minutes;
    constructor(input: JQuery, opt?: TimeEditorOptions);
    get value(): number;
    protected get_value(): number;
    set value(value: number);
    protected set_value(value: number): void;
    get_readOnly(): boolean;
    set_readOnly(value: boolean): void;
}

interface EmailEditorOptions {
    domain?: string;
    readOnlyDomain?: boolean;
}
declare class EmailEditor extends Widget<EmailEditorOptions> {
    constructor(input: JQuery, opt: EmailEditorOptions);
    static registerValidationMethods(): void;
    get_value(): string;
    get value(): string;
    set_value(value: string): void;
    set value(v: string);
    get_readOnly(): boolean;
    set_readOnly(value: boolean): void;
}

declare class EmailAddressEditor extends StringEditor {
    constructor(input: JQuery);
}

declare class URLEditor extends StringEditor {
    constructor(input: JQuery);
}

interface RadioButtonEditorOptions {
    enumKey?: string;
    enumType?: any;
    lookupKey?: string;
}
declare class RadioButtonEditor extends Widget<RadioButtonEditorOptions> implements IReadOnly {
    constructor(input: JQuery, opt: RadioButtonEditorOptions);
    protected addRadio(value: string, text: string): void;
    get_value(): string;
    get value(): string;
    set_value(value: string): void;
    set value(v: string);
    get_readOnly(): boolean;
    set_readOnly(value: boolean): void;
}

interface Select2CommonOptions {
    allowClear?: boolean;
    delimited?: boolean;
    minimumResultsForSearch?: any;
    multiple?: boolean;
}
interface Select2FilterOptions {
    cascadeFrom?: string;
    cascadeField?: string;
    cascadeValue?: any;
    filterField?: string;
    filterValue?: any;
}
interface Select2InplaceAddOptions {
    inplaceAdd?: boolean;
    inplaceAddPermission?: string;
    dialogType?: string;
    autoComplete?: boolean;
}
interface Select2EditorOptions extends Select2FilterOptions, Select2InplaceAddOptions, Select2CommonOptions {
}
interface Select2SearchPromise {
    abort?(): void;
    catch?(callback: () => void): void;
    fail?(callback: () => void): void;
}
interface Select2SearchQuery {
    searchTerm?: string;
    idList?: string[];
    skip?: number;
    take?: number;
    checkMore?: boolean;
}
interface Select2SearchResult<TItem> {
    items: TItem[];
    more: boolean;
}
declare class Select2Editor<TOptions, TItem> extends Widget<TOptions> implements ISetEditValue, IGetEditValue, IStringValue, IReadOnly {
    private _items;
    private _itemById;
    protected lastCreateTerm: string;
    constructor(hidden: JQuery, opt?: any);
    destroy(): void;
    protected hasAsyncSource(): boolean;
    protected asyncSearch(query: Select2SearchQuery, results: (result: Select2SearchResult<TItem>) => void): Select2SearchPromise;
    protected getTypeDelay(): any;
    protected emptyItemText(): string;
    protected getPageSize(): number;
    protected getIdField(): any;
    protected itemId(item: TItem): string;
    protected getTextField(): any;
    protected itemText(item: TItem): string;
    protected itemDisabled(item: TItem): boolean;
    protected mapItem(item: TItem): Select2Item;
    protected mapItems(items: TItem[]): Select2Item[];
    protected allowClear(): boolean;
    protected isMultiple(): boolean;
    private initSelectionPromise;
    private queryPromise;
    private typeTimeout;
    protected abortPendingQuery(): void;
    protected getSelect2Options(): Select2Options;
    get_delimited(): boolean;
    get items(): Select2Item[];
    set items(value: Select2Item[]);
    protected get itemById(): {
        [key: string]: Select2Item;
    };
    protected set itemById(value: {
        [key: string]: Select2Item;
    });
    clearItems(): void;
    addItem(item: Select2Item): void;
    addOption(key: string, text: string, source?: any, disabled?: boolean): void;
    protected addInplaceCreate(addTitle: string, editTitle: string): void;
    protected useInplaceAdd(): boolean;
    protected isAutoComplete(): boolean;
    getCreateSearchChoice(getName: (z: any) => string): (s: string) => {
        id: string;
        text: string;
    };
    setEditValue(source: any, property: PropertyItem): void;
    getEditValue(property: PropertyItem, target: any): void;
    protected get_select2Container(): JQuery;
    protected get_items(): Select2Item[];
    protected get_itemByKey(): {
        [key: string]: Select2Item;
    };
    static filterByText<TItem>(items: TItem[], getText: (item: TItem) => string, term: string): TItem[];
    get_value(): any;
    get value(): string;
    set_value(value: string): void;
    set value(v: string);
    get selectedItem(): TItem;
    get selectedItems(): TItem[];
    protected get_values(): string[];
    get values(): string[];
    protected set_values(value: string[]): void;
    set values(value: string[]);
    protected get_text(): string;
    get text(): string;
    get_readOnly(): boolean;
    get readOnly(): boolean;
    private updateInplaceReadOnly;
    set_readOnly(value: boolean): void;
    set readOnly(value: boolean);
    protected getCascadeFromValue(parent: Widget<any>): any;
    protected cascadeLink: CascadedWidgetLink<Widget<any>>;
    protected setCascadeFrom(value: string): void;
    protected get_cascadeFrom(): string;
    get cascadeFrom(): string;
    protected set_cascadeFrom(value: string): void;
    set cascadeFrom(value: string);
    protected get_cascadeField(): string;
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
    protected cascadeItems(items: TItem[]): TItem[];
    protected filterItems(items: TItem[]): TItem[];
    protected updateItems(): void;
    protected getDialogTypeKey(): string;
    protected createEditDialog(callback: (dlg: IEditDialog) => void): void;
    onInitNewEntity: (entity: TItem) => void;
    protected initNewEntity(entity: TItem): void;
    protected setEditDialogReadOnly(dialog: any): void;
    protected editDialogDataChange(): void;
    protected setTermOnNewEntity(entity: TItem, term: string): void;
    protected inplaceCreateClick(e: JQueryEventObject): void;
    openDialogAsPanel: boolean;
}

declare class SelectEditor extends Select2Editor<SelectEditorOptions, Select2Item> {
    constructor(hidden: JQuery, opt?: SelectEditorOptions);
    getItems(): any[];
    protected emptyItemText(): string;
    updateItems(): void;
}
interface SelectEditorOptions extends Select2CommonOptions {
    items?: any[];
    emptyOptionText?: string;
}

declare class DateYearEditor extends SelectEditor {
    constructor(hidden: JQuery, opt: DateYearEditorOptions);
    getItems(): any[];
}
interface DateYearEditorOptions extends SelectEditorOptions {
    minYear?: string;
    maxYear?: string;
    descending?: boolean;
}

interface EnumEditorOptions extends Select2CommonOptions {
    enumKey?: string;
    enumType?: any;
}
declare class EnumEditor extends Select2Editor<EnumEditorOptions, Select2Item> {
    constructor(hidden: JQuery, opt: EnumEditorOptions);
    protected updateItems(): void;
    protected allowClear(): boolean;
}

interface LookupEditorOptions extends Select2EditorOptions {
    lookupKey?: string;
    async?: boolean;
}
declare abstract class LookupEditorBase<TOptions extends LookupEditorOptions, TItem> extends Select2Editor<TOptions, TItem> {
    constructor(input: JQuery, opt?: TOptions);
    hasAsyncSource(): boolean;
    destroy(): void;
    protected getLookupKey(): string;
    protected lookup: Lookup<TItem>;
    protected getLookupAsync(): PromiseLike<Lookup<TItem>>;
    protected getLookup(): Lookup<TItem>;
    protected getItems(lookup: Lookup<TItem>): TItem[];
    protected getIdField(): any;
    protected getItemText(item: TItem, lookup: Lookup<TItem>): any;
    protected mapItem(item: TItem): Select2Item;
    protected getItemDisabled(item: TItem, lookup: Lookup<TItem>): boolean;
    updateItems(): void;
    protected asyncSearch(query: Select2SearchQuery, results: (result: Select2SearchResult<TItem>) => void): Select2SearchPromise;
    protected getDialogTypeKey(): string;
    protected setCreateTermOnNewEntity(entity: TItem, term: string): void;
    protected editDialogDataChange(): void;
}
declare class LookupEditor extends LookupEditorBase<LookupEditorOptions, any> {
    constructor(hidden: JQuery, opt?: LookupEditorOptions);
}

interface ServiceLookupEditorOptions extends Select2EditorOptions {
    service?: string;
    idField: string;
    textField: string;
    pageSize?: number;
    minimumResultsForSearch?: any;
    sort: string[];
    columnSelection?: ColumnSelection;
    includeColumns?: string[];
    excludeColumns?: string[];
    includeDeleted?: boolean;
    containsField?: string;
    equalityFilter?: any;
    criteria?: any[];
}
declare abstract class ServiceLookupEditorBase<TOptions extends ServiceLookupEditorOptions, TItem> extends Select2Editor<TOptions, TItem> {
    constructor(input: JQuery, opt?: TOptions);
    protected getDialogTypeKey(): string;
    protected getService(): string;
    protected getServiceUrl(): string;
    protected getIncludeColumns(): string[];
    protected getSort(): any[];
    protected getCascadeCriteria(): any[];
    protected getFilterCriteria(): any[];
    protected getIdListCriteria(idList: any[]): any[];
    protected getCriteria(query: Select2SearchQuery): any[];
    protected getListRequest(query: Select2SearchQuery): ListRequest;
    protected getServiceCallOptions(query: Select2SearchQuery, results: (result: Select2SearchResult<TItem>) => void): ServiceOptions<ListResponse<TItem>>;
    protected hasAsyncSource(): boolean;
    protected asyncSearch(query: Select2SearchQuery, results: (result: Select2SearchResult<TItem>) => void): Select2SearchPromise;
}
declare class ServiceLookupEditor extends ServiceLookupEditorBase<ServiceLookupEditorOptions, any> {
    constructor(hidden: JQuery, opt?: ServiceLookupEditorOptions);
}

interface HtmlContentEditorOptions {
    cols?: any;
    rows?: any;
}
interface CKEditorConfig {
}
declare class HtmlContentEditor extends Widget<HtmlContentEditorOptions> implements IStringValue, IReadOnly {
    private _instanceReady;
    constructor(textArea: JQuery, opt?: HtmlContentEditorOptions);
    protected instanceReady(x: any): void;
    protected getLanguage(): string;
    protected getConfig(): CKEditorConfig;
    protected getEditorInstance(): any;
    destroy(): void;
    get_value(): string;
    get value(): string;
    set_value(value: string): void;
    set value(v: string);
    get_readOnly(): boolean;
    set_readOnly(value: boolean): void;
    static CKEditorVer: string;
    static CKEditorBasePath: string;
    static getCKEditorBasePath(): string;
    static includeCKEditor(): void;
}
declare class HtmlNoteContentEditor extends HtmlContentEditor {
    constructor(textArea: JQuery, opt?: HtmlContentEditorOptions);
    protected getConfig(): CKEditorConfig;
}
declare class HtmlReportContentEditor extends HtmlContentEditor {
    constructor(textArea: JQuery, opt?: HtmlContentEditorOptions);
    protected getConfig(): CKEditorConfig;
}

declare class MaskedEditor extends Widget<MaskedEditorOptions> {
    constructor(input: JQuery, opt?: MaskedEditorOptions);
    get value(): string;
    protected get_value(): string;
    set value(value: string);
    protected set_value(value: string): void;
}
interface MaskedEditorOptions {
    mask?: string;
    placeholder?: string;
}

interface RecaptchaOptions {
    siteKey?: string;
    language?: string;
}
declare class Recaptcha extends Widget<RecaptchaOptions> implements IStringValue {
    constructor(div: JQuery, opt: RecaptchaOptions);
    get_value(): string;
    set_value(value: string): void;
}

declare namespace UploadHelper {
    function addUploadInput(options: UploadInputOptions): JQuery;
    function checkImageConstraints(file: UploadResponse, opt: FileUploadConstraints): boolean;
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
    uploadIntent?: string;
    uploadUrl?: string;
    fileDone?: (p1: UploadResponse, p2: string, p3: any) => void;
}
interface UploadResponse extends ServiceResponse {
    TemporaryFile: string;
    Size: number;
    IsImage: boolean;
    Width: number;
    Height: number;
}
interface FileUploadConstraints {
    minWidth?: number;
    maxWidth?: number;
    minHeight?: number;
    maxHeight?: number;
    minSize?: number;
    maxSize?: number;
    allowNonImage?: boolean;
    originalNameProperty?: string;
}

interface FileUploadEditorOptions extends FileUploadConstraints {
    displayFileName?: boolean;
    uploadIntent?: string;
    uploadUrl?: string;
    urlPrefix?: string;
}
interface ImageUploadEditorOptions extends FileUploadEditorOptions {
}
declare class FileUploadEditor extends Widget<FileUploadEditorOptions> implements IReadOnly, IGetEditValue, ISetEditValue, IValidateRequired {
    constructor(div: JQuery, opt: FileUploadEditorOptions);
    protected getUploadInputOptions(): UploadInputOptions;
    protected addFileButtonText(): string;
    protected getToolButtons(): ToolButton[];
    protected populate(): void;
    protected updateInterface(): void;
    get_readOnly(): boolean;
    set_readOnly(value: boolean): void;
    get_required(): boolean;
    set_required(value: boolean): void;
    get_value(): UploadedFile;
    get value(): UploadedFile;
    set_value(value: UploadedFile): void;
    set value(v: UploadedFile);
    getEditValue(property: PropertyItem, target: any): void;
    setEditValue(source: any, property: PropertyItem): void;
    protected entity: UploadedFile;
    protected toolbar: Toolbar;
    protected progress: JQuery;
    protected fileSymbols: JQuery;
    protected uploadInput: JQuery;
    protected hiddenInput: JQuery;
}
declare class ImageUploadEditor extends FileUploadEditor {
    constructor(div: JQuery, opt: ImageUploadEditorOptions);
}
declare class MultipleFileUploadEditor extends Widget<FileUploadEditorOptions> implements IReadOnly, IGetEditValue, ISetEditValue, IValidateRequired {
    private entities;
    private toolbar;
    private fileSymbols;
    private uploadInput;
    protected progress: JQuery;
    protected hiddenInput: JQuery;
    constructor(div: JQuery, opt: ImageUploadEditorOptions);
    protected getUploadInputOptions(): UploadInputOptions;
    protected addFileButtonText(): string;
    protected getToolButtons(): ToolButton[];
    protected populate(): void;
    protected updateInterface(): void;
    get_readOnly(): boolean;
    set_readOnly(value: boolean): void;
    get_required(): boolean;
    set_required(value: boolean): void;
    get_value(): UploadedFile[];
    get value(): UploadedFile[];
    set_value(value: UploadedFile[]): void;
    set value(v: UploadedFile[]);
    getEditValue(property: PropertyItem, target: any): void;
    setEditValue(source: any, property: PropertyItem): void;
    jsonEncodeValue: boolean;
}
declare class MultipleImageUploadEditor extends MultipleFileUploadEditor {
    constructor(div: JQuery, opt: ImageUploadEditorOptions);
}

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

interface QuickFilterBarOptions {
    filters: QuickFilter<Widget<any>, any>[];
    getTitle?: (filter: QuickFilter<Widget<any>, any>) => string;
    idPrefix?: string;
}
declare class QuickFilterBar extends Widget<QuickFilterBarOptions> {
    constructor(container: JQuery, options?: QuickFilterBarOptions);
    addSeparator(): void;
    add<TWidget extends Widget<any>, TOptions>(opt: QuickFilter<TWidget, TOptions>): TWidget;
    addDateRange(field: string, title?: string): DateEditor;
    static dateRange(field: string, title?: string): QuickFilter<DateEditor, DateTimeEditorOptions>;
    addDateTimeRange(field: string, title?: string): DateTimeEditor;
    static dateTimeRange(field: string, title?: string, useUtc?: boolean): QuickFilter<DateTimeEditor, DateTimeEditorOptions>;
    addBoolean(field: string, title?: string, yes?: string, no?: string): SelectEditor;
    static boolean(field: string, title?: string, yes?: string, no?: string): QuickFilter<SelectEditor, SelectEditorOptions>;
    onChange: (e: JQueryEventObject) => void;
    private submitHandlers;
    destroy(): void;
    onSubmit(request: ListRequest): void;
    protected add_submitHandlers(action: (request: ListRequest) => void): void;
    protected remove_submitHandlers(action: (request: ListRequest) => void): void;
    protected clear_submitHandlers(): void;
    find<TWidget>(type: {
        new (...args: any[]): TWidget;
    }, field: string): TWidget;
    tryFind<TWidget>(type: {
        new (...args: any[]): TWidget;
    }, field: string): TWidget;
}

interface QuickSearchField {
    name: string;
    title: string;
}
interface QuickSearchInputOptions {
    typeDelay?: number;
    loadingParentClass?: string;
    filteredParentClass?: string;
    onSearch?: (p1: string, p2: string, p3: (p1: boolean) => void) => void;
    fields?: QuickSearchField[];
}
declare class QuickSearchInput extends Widget<QuickSearchInputOptions> {
    private lastValue;
    private field;
    private fieldChanged;
    private timer;
    constructor(input: JQuery, opt: QuickSearchInputOptions);
    protected checkIfValueChanged(): void;
    get_value(): string;
    get_field(): QuickSearchField;
    set_field(value: QuickSearchField): void;
    protected updateInputPlaceHolder(): void;
    restoreState(value: string, field: QuickSearchField): void;
    protected searchNow(value: string): void;
}

interface FilterOperator {
    key?: string;
    title?: string;
    format?: string;
}
declare namespace FilterOperators {
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
    const toCriteriaOperator: {
        [key: string]: string;
    };
}

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

declare class FilterStore {
    constructor(fields: PropertyItem[]);
    static getCriteriaFor(items: FilterLine[]): any[];
    static getDisplayTextFor(items: FilterLine[]): string;
    private changed;
    private displayText;
    private fields;
    private fieldByName;
    private items;
    get_fields(): PropertyItem[];
    get_fieldByName(): {
        [key: string]: PropertyItem;
    };
    get_items(): FilterLine[];
    raiseChanged(): void;
    add_changed(value: (e: JQueryEventObject, a: any) => void): void;
    remove_changed(value: (e: JQueryEventObject, a: any) => void): void;
    get_activeCriteria(): any[];
    get_displayText(): string;
}

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
declare class IFiltering {
}
interface CriteriaWithText {
    criteria?: any[];
    displayText?: string;
}
interface IQuickFiltering {
    initQuickFilter(filter: QuickFilter<Widget<any>, any>): void;
}
declare class IQuickFiltering {
}
declare abstract class BaseFiltering implements IFiltering, IQuickFiltering {
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
    protected operatorFormat(op: FilterOperator): string;
    protected getTitle(field: PropertyItem): string;
    protected displayText(op: FilterOperator, values?: any[]): string;
    protected getCriteriaField(): string;
    getCriteria(): CriteriaWithText;
    loadState(state: any): void;
    saveState(): any;
    protected argumentNull(): Error;
    validateEditorValue(value: string): string;
    getEditorValue(): string;
    getEditorText(): any;
    initQuickFilter(filter: QuickFilter<Widget<any>, any>): void;
}
declare abstract class BaseEditorFiltering<TEditor extends Widget<any>> extends BaseFiltering {
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
declare class DateFiltering extends BaseEditorFiltering<DateEditor> {
    constructor();
    getOperators(): FilterOperator[];
}
declare class BooleanFiltering extends BaseFiltering {
    getOperators(): FilterOperator[];
}
declare class DateTimeFiltering extends BaseEditorFiltering<DateEditor> {
    constructor();
    getOperators(): FilterOperator[];
    getCriteria(): CriteriaWithText;
}
declare class DecimalFiltering extends BaseEditorFiltering<DecimalEditor> {
    constructor();
    getOperators(): FilterOperator[];
}
declare class EditorFiltering extends BaseEditorFiltering<Widget<any>> {
    constructor();
    editorType: string;
    useRelative: boolean;
    useLike: boolean;
    getOperators(): FilterOperator[];
    protected useEditor(): boolean;
    getEditorOptions(): any;
    createEditor(): void;
    protected useIdField(): boolean;
    initQuickFilter(filter: QuickFilter<Widget<any>, any>): void;
}
declare class EnumFiltering extends BaseEditorFiltering<EnumEditor> {
    constructor();
    getOperators(): FilterOperator[];
}
declare class IntegerFiltering extends BaseEditorFiltering<IntegerEditor> {
    constructor();
    getOperators(): FilterOperator[];
}
declare class LookupFiltering extends BaseEditorFiltering<LookupEditor> {
    constructor();
    getOperators(): FilterOperator[];
    protected useEditor(): boolean;
    protected useIdField(): boolean;
    getEditorText(): string;
}
declare class ServiceLookupFiltering extends BaseEditorFiltering<ServiceLookupEditor> {
    constructor();
    getOperators(): FilterOperator[];
    protected useEditor(): boolean;
    protected useIdField(): boolean;
    getEditorText(): string;
}
declare class StringFiltering extends BaseFiltering {
    getOperators(): FilterOperator[];
    validateEditorValue(value: string): string;
}
declare namespace FilteringTypeRegistry {
    function get(key: string): Function;
}

declare class FilterWidgetBase<TOptions> extends TemplatedWidget<TOptions> {
    private store;
    private onFilterStoreChanged;
    constructor(div: JQuery, opt?: TOptions);
    destroy(): void;
    protected filterStoreChanged(): void;
    get_store(): FilterStore;
    set_store(value: FilterStore): void;
}

declare class FilterPanel extends FilterWidgetBase<any> {
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

declare class FilterDialog extends TemplatedDialog<any> {
    private filterPanel;
    constructor();
    get_filterPanel(): FilterPanel;
    protected getTemplate(): string;
    protected getDialogButtons(): {
        text: string;
        click: () => void;
    }[];
}

declare class FilterDisplayBar extends FilterWidgetBase<any> {
    constructor(div: JQuery);
    protected filterStoreChanged(): void;
    protected getTemplate(): string;
}

declare class SlickPager extends Widget<PagerOptions> {
    constructor(div: JQuery, o: PagerOptions);
    _changePage(ctype: string): boolean;
    _updatePager(): void;
}

interface IDataGrid {
    getElement(): JQuery;
    getGrid(): Grid;
    getView(): RemoteView<any>;
    getFilterStore(): FilterStore;
}

interface GridRowSelectionMixinOptions {
    selectable?: (item: any) => boolean;
}
declare class GridRowSelectionMixin {
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
    static createSelectColumn(getMixin: () => GridRowSelectionMixin): Column;
}
interface GridRadioSelectionMixinOptions {
    selectable?: (item: any) => boolean;
}
declare class GridRadioSelectionMixin {
    private idField;
    private include;
    private grid;
    private options;
    constructor(grid: IDataGrid, options?: GridRadioSelectionMixinOptions);
    private isSelectable;
    clear(): void;
    resetCheckedAndRefresh(): void;
    getSelectedKey(): string;
    getSelectedAsInt32(): number;
    getSelectedAsInt64(): number;
    setSelectedKey(key: string): void;
    static createSelectColumn(getMixin: () => GridRadioSelectionMixin): Column;
}
declare namespace GridSelectAllButtonHelper {
    function update(grid: IDataGrid, getSelected: (p1: any) => boolean): void;
    function define(getGrid: () => IDataGrid, getId: (p1: any) => any, getSelected: (p1: any) => boolean, setSelected: (p1: any, p2: boolean) => void, text?: string, onClick?: () => void): ToolButton;
}
declare namespace GridUtils {
    function addToggleButton(toolDiv: JQuery, cssClass: string, callback: (p1: boolean) => void, hint: string, initial?: boolean): void;
    function addIncludeDeletedToggle(toolDiv: JQuery, view: RemoteView<any>, hint?: string, initial?: boolean): void;
    function addQuickSearchInput(toolDiv: JQuery, view: RemoteView<any>, fields?: QuickSearchField[], onChange?: () => void): QuickSearchInput;
    function addQuickSearchInputCustom(container: JQuery, onSearch: (p1: string, p2: string, done: (p3: boolean) => void) => void, fields?: QuickSearchField[]): QuickSearchInput;
    function makeOrderable(grid: Grid, handleMove: (p1: any, p2: number) => void): void;
    function makeOrderableWithUpdateRequest(grid: IDataGrid, getId: (p1: any) => number, getDisplayOrder: (p1: any) => any, service: string, getUpdateRequest: (p1: number, p2: number) => SaveRequest<any>): void;
}
declare namespace PropertyItemSlickConverter {
    function toSlickColumns(items: PropertyItem[]): Column[];
    function toSlickColumn(item: PropertyItem): Column;
}
declare namespace SlickFormatting {
    function getEnumText(enumKey: string, name: string): string;
    function treeToggle(getView: () => RemoteView<any>, getId: (x: any) => any, formatter: Format): Format;
    function date(format?: string): Format;
    function dateTime(format?: string): Format;
    function checkBox(): Format;
    function number(format: string): Format;
    function getItemType(link: JQuery): string;
    function getItemId(link: JQuery): string;
    function itemLinkText(itemType: string, id: any, text: any, extraClass: string, encode: boolean): string;
    function itemLink<TItem = any>(itemType: string, idField: string, getText: Format<TItem>, cssClass?: Format<TItem>, encode?: boolean): Format<TItem>;
}
declare namespace SlickHelper {
    function setDefaults(columns: Column[], localTextPrefix?: string): any;
}
declare namespace SlickTreeHelper {
    function filterCustom<TItem>(item: TItem, getParent: (x: TItem) => any): boolean;
    function filterById<TItem>(item: TItem, view: RemoteView<TItem>, getParentId: (x: TItem) => any): boolean;
    function setCollapsed<TItem>(items: TItem[], collapsed: boolean): void;
    function setCollapsedFlag<TItem>(item: TItem, collapsed: boolean): void;
    function setIndents<TItem>(items: TItem[], getId: (x: TItem) => any, getParentId: (x: TItem) => any, setCollapsed?: boolean): void;
    function toggleClick<TItem>(e: JQueryEventObject, row: number, cell: number, view: RemoteView<TItem>, getId: (x: TItem) => any): void;
}
declare class ColumnsBase<TRow = any> {
    constructor(items: Column<TRow>[]);
    valueOf(): Column<TRow>[];
}

interface IInitializeColumn {
    initializeColumn(column: Column): void;
}
declare class IInitializeColumn {
}
declare class BooleanFormatter implements Formatter {
    format(ctx: FormatterContext): string;
    falseText: string;
    trueText: string;
}
declare class CheckboxFormatter implements Formatter {
    format(ctx: FormatterContext): string;
}
declare class DateFormatter implements Formatter {
    constructor();
    static format(value: any, format?: string): any;
    displayFormat: string;
    format(ctx: FormatterContext): string;
}
declare class DateTimeFormatter extends DateFormatter {
    constructor();
}
declare class EnumFormatter implements Formatter {
    format(ctx: FormatterContext): string;
    enumKey: string;
    static format(enumType: any, value: any): string;
    static getText(enumKey: string, name: string): string;
    static getName(enumType: any, value: any): string;
}
declare class FileDownloadFormatter implements Formatter, IInitializeColumn {
    format(ctx: FormatterContext): string;
    static dbFileUrl(filename: string): string;
    initializeColumn(column: Column): void;
    displayFormat: string;
    originalNameProperty: string;
    iconClass: string;
}
declare class MinuteFormatter implements Formatter {
    format(ctx: FormatterContext): string;
    static format(value: number): string;
}
declare class NumberFormatter {
    format(ctx: FormatterContext): string;
    static format(value: any, format?: string): string;
    displayFormat: string;
}
declare class UrlFormatter implements Formatter, IInitializeColumn {
    format(ctx: FormatterContext): string;
    initializeColumn(column: Column): void;
    displayProperty: string;
    displayFormat: string;
    urlProperty: string;
    urlFormat: string;
    target: string;
}

declare namespace FormatterTypeRegistry {
    function get(key: string): any;
    function reset(): void;
    function tryGet(key: string): any;
}

type GroupItemMetadataProviderType = typeof GroupItemMetadataProvider;
declare global {
    namespace Slick {
        namespace Data {
            /** @obsolete use the type exported from @serenity-is/sleekgrid */
            const GroupItemMetadataProvider: GroupItemMetadataProviderType;
        }
        interface RowMoveManagerOptions {
            cancelEditOnDrag: boolean;
        }
        class RowMoveManager implements IPlugin {
            constructor(options: RowMoveManagerOptions);
            init(): void;
            onBeforeMoveRows: EventEmitter;
            onMoveRows: EventEmitter;
        }
        class RowSelectionModel implements SelectionModel {
            init(grid: Grid): void;
            destroy?: () => void;
            setSelectedRanges(ranges: Range[]): void;
            onSelectedRangesChanged: EventEmitter<Range[]>;
            refreshSelections?(): void;
        }
    }
}
interface SettingStorage {
    getItem(key: string): string | Promise<string>;
    setItem(key: string, value: string): void | Promise<void>;
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
    quickFilters?: {
        [key: string]: any;
    };
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
declare class DataGrid<TItem, TOptions> extends Widget<TOptions> implements IDataGrid, IReadOnly {
    private _isDisabled;
    private _layoutTimer;
    private _slickGridOnSort;
    private _slickGridOnClick;
    protected titleDiv: JQuery;
    protected toolbar: Toolbar;
    protected filterBar: FilterDisplayBar;
    protected quickFiltersDiv: JQuery;
    protected quickFiltersBar: QuickFilterBar;
    protected slickContainer: JQuery;
    protected allColumns: Column[];
    protected propertyItemsData: PropertyItemsData;
    protected initialSettings: PersistedGridSettings;
    protected restoringSettings: number;
    view: RemoteView<TItem>;
    slickGrid: Grid;
    openDialogsAsPanel: boolean;
    static defaultRowHeight: number;
    static defaultHeaderHeight: number;
    static defaultPersistanceStorage: SettingStorage;
    static defaultColumnWidthScale: number;
    static defaultColumnWidthDelta: number;
    constructor(container: JQuery, options?: TOptions);
    protected internalInit(): void;
    protected initSync(): void;
    protected initAsync(): Promise<void>;
    protected afterInit(): void;
    protected useAsync(): boolean;
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
    protected postProcessColumns(columns: Column[]): Column[];
    protected getColumnWidthDelta(): number;
    protected getColumnWidthScale(): number;
    protected initialPopulate(): void;
    protected canFilterColumn(column: Column): boolean;
    protected initializeFilterBar(): void;
    protected createSlickGrid(): Grid;
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
    protected createView(): RemoteView<TItem>;
    protected getDefaultSortBy(): any[];
    protected usePager(): boolean;
    protected enableFiltering(): boolean;
    protected populateWhenVisible(): boolean;
    protected createFilterBar(): void;
    protected getPagerOptions(): PagerOptions;
    protected createPager(): void;
    protected getViewOptions(): RemoteViewOptions;
    protected createToolbar(buttons: ToolButton[]): void;
    getTitle(): string;
    setTitle(value: string): void;
    protected getItemType(): string;
    protected itemLink(itemType?: string, idField?: string, text?: (ctx: FormatterContext) => string, cssClass?: (ctx: FormatterContext) => string, encode?: boolean): Format<TItem>;
    protected getColumnsKey(): string;
    protected getPropertyItems(): PropertyItem[];
    protected getPropertyItemsData(): PropertyItemsData;
    protected getPropertyItemsDataAsync(): Promise<PropertyItemsData>;
    protected getColumns(): Column<TItem>[];
    protected propertyItemsToSlickColumns(propertyItems: PropertyItem[]): Column[];
    protected getSlickOptions(): GridOptions;
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
    updateInterface(): void;
    protected getRowDefinition(): IRowDefinition;
    private _localTextDbPrefix;
    protected getLocalTextDbPrefix(): string;
    protected getLocalTextPrefix(): string;
    private _idProperty;
    protected getIdProperty(): string;
    protected getIsDeletedProperty(): string;
    private _isActiveProperty;
    protected getIsActiveProperty(): string;
    protected updateDisabledState(): void;
    protected resizeCanvas(): void;
    protected subDialogDataChange(): void;
    protected addFilterSeparator(): void;
    protected determineText(getKey: (prefix: string) => string): string;
    protected addQuickFilter<TWidget extends Widget<any>, TOptions>(opt: QuickFilter<TWidget, TOptions>): TWidget;
    protected addDateRangeFilter(field: string, title?: string): DateEditor;
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
    protected canShowColumn(column: Column): boolean;
    protected getPersistedSettings(): PersistedGridSettings | Promise<PersistedGridSettings>;
    protected restoreSettings(settings?: PersistedGridSettings, flags?: GridPersistanceFlags): void | Promise<void>;
    protected restoreSettingsFrom(settings: PersistedGridSettings, flags?: GridPersistanceFlags): void;
    protected persistSettings(flags?: GridPersistanceFlags): void | Promise<void>;
    protected getCurrentSettings(flags?: GridPersistanceFlags): PersistedGridSettings;
    getElement(): JQuery;
    getGrid(): Grid;
    getView(): RemoteView<TItem>;
    getFilterStore(): FilterStore;
}

declare class ColumnPickerDialog extends TemplatedDialog<any> {
    private ulVisible;
    private ulHidden;
    private colById;
    allColumns: Column[];
    visibleColumns: string[];
    defaultColumns: string[];
    done: () => void;
    constructor();
    static createToolButton(grid: IDataGrid): ToolButton;
    protected getDialogButtons(): {
        text: string;
        click: () => void;
    }[];
    protected getDialogOptions(): any;
    private getTitle;
    private allowHide;
    private createLI;
    private updateListStates;
    protected setupColumns(): void;
    protected onDialogOpen(): void;
    protected getTemplate(): string;
}

/**
 * A mixin that can be applied to a DataGrid for tree functionality
 */
declare class TreeGridMixin<TItem> {
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
    grid: DataGrid<TItem, any>;
    getParentId: (item: TItem) => any;
    toggleField: string;
    initialCollapse?: () => boolean;
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
declare class CheckTreeEditor<TItem extends CheckTreeItem<any>, TOptions> extends DataGrid<TItem, TOptions> implements IGetEditValue, ISetEditValue, IReadOnly {
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
    protected createSlickGrid(): Grid;
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
    protected getColumns(): Column[];
    protected getItemText(ctx: FormatterContext): string;
    protected getSlickOptions(): GridOptions;
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
declare class CheckLookupEditor<TItem = any> extends CheckTreeEditor<CheckTreeItem<TItem>, CheckLookupEditorOptions> {
    private searchText;
    private enableUpdateItems;
    constructor(div: JQuery, options: CheckLookupEditorOptions);
    protected updateItems(): void;
    protected getLookupKey(): string;
    protected getButtons(): ToolButton[];
    protected createToolbarExtensions(): void;
    protected getSelectAllText(): string;
    protected cascadeItems(items: TItem[]): TItem[];
    protected filterItems(items: TItem[]): TItem[];
    protected getLookupItems(lookup: Lookup<TItem>): TItem[];
    protected getTreeItems(): CheckTreeItem<TItem>[];
    protected onViewFilter(item: CheckTreeItem<TItem>): boolean;
    protected moveSelectedUp(): boolean;
    protected get_cascadeFrom(): string;
    get cascadeFrom(): string;
    protected getCascadeFromValue(parent: Widget<any>): any;
    protected cascadeLink: CascadedWidgetLink<Widget<any>>;
    protected setCascadeFrom(value: string): void;
    protected set_cascadeFrom(value: string): void;
    set cascadeFrom(value: string);
    protected get_cascadeField(): string;
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

declare class EntityGrid<TItem, TOptions> extends DataGrid<TItem, TOptions> {
    constructor(container: JQuery, options?: TOptions);
    protected handleRoute(args: HandleRouteEventArgs): void;
    protected usePager(): boolean;
    protected createToolbarExtensions(): void;
    protected getInitialTitle(): string;
    protected getLocalTextPrefix(): string;
    private _entityType;
    protected getEntityType(): string;
    private _displayName;
    protected getDisplayName(): string;
    private _itemName;
    protected getItemName(): string;
    protected getAddButtonCaption(): string;
    protected getButtons(): ToolButton[];
    protected newRefreshButton(noText?: boolean): ToolButton;
    protected addButtonClick(): void;
    protected editItem(entityOrId: any): void;
    protected editItemOfType(itemType: string, entityOrId: any): void;
    private _service;
    protected getService(): string;
    protected getViewOptions(): RemoteViewOptions;
    protected getItemType(): string;
    protected routeDialog(itemType: string, dialog: Widget<any>): void;
    protected getInsertPermission(): string;
    protected hasInsertPermission(): boolean;
    protected transferDialogReadOnly(dialog: Widget<any>): void;
    protected initDialog(dialog: Widget<any>): void;
    protected initEntityDialog(itemType: string, dialog: Widget<any>): void;
    protected createEntityDialog(itemType: string, callback?: (dlg: Widget<any>) => void): Widget<any>;
    protected getDialogOptions(): any;
    protected getDialogOptionsFor(itemType: string): any;
    protected getDialogTypeFor(itemType: string): {
        new (...args: any[]): Widget<any>;
    };
    private _dialogType;
    protected getDialogType(): {
        new (...args: any[]): Widget<any>;
    };
}

declare class EntityDialog<TItem, TOptions> extends TemplatedDialog<TOptions> implements IEditDialog, IReadOnly {
    protected entity: TItem;
    protected entityId: any;
    protected propertyItemsData: PropertyItemsData;
    protected propertyGrid: PropertyGrid;
    protected toolbar: Toolbar;
    protected saveAndCloseButton: JQuery;
    protected applyChangesButton: JQuery;
    protected deleteButton: JQuery;
    protected undeleteButton: JQuery;
    protected cloneButton: JQuery;
    protected editButton: JQuery;
    protected localizationGrid: PropertyGrid;
    protected localizationButton: JQuery;
    protected localizationPendingValue: any;
    protected localizationLastValue: any;
    static defaultLanguageList: () => string[][];
    constructor(opt?: TOptions);
    internalInit(): void;
    protected initSync(): void;
    protected initAsync(): Promise<void>;
    protected afterInit(): void;
    protected useAsync(): boolean;
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
    protected getRowDefinition(): IRowDefinition;
    private _entityType;
    protected getEntityType(): string;
    private _formKey;
    protected getFormKey(): string;
    private _localTextDbPrefix;
    protected getLocalTextDbPrefix(): string;
    protected getLocalTextPrefix(): string;
    private _entitySingular;
    protected getEntitySingular(): string;
    private _nameProperty;
    protected getNameProperty(): string;
    private _idProperty;
    protected getIdProperty(): string;
    private _isActiveProperty;
    protected getIsActiveProperty(): string;
    protected getIsDeletedProperty(): string;
    private _service;
    protected getService(): string;
    load(entityOrId: any, done: () => void, fail?: (ex: Exception) => void): void;
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
    protected initLocalizationGridCommon(pgOptions: PropertyGridOptions): void;
    protected isLocalizationMode(): boolean;
    protected isLocalizationModeAndChanged(): boolean;
    protected localizationButtonClick(): void;
    protected getLanguages(): any[];
    private getLangs;
    protected loadLocalization(): void;
    protected setLocalizationGridCurrentValues(): void;
    protected getLocalizationGridValue(): any;
    protected getPendingLocalizations(): any;
    protected initPropertyGrid(): void;
    protected getPropertyItems(): PropertyItem[];
    protected getPropertyItemsData(): PropertyItemsData;
    protected getPropertyItemsDataAsync(): Promise<PropertyItemsData>;
    protected getPropertyGridOptions(): PropertyGridOptions;
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
    private _readonly;
    get readOnly(): boolean;
    set readOnly(value: boolean);
    get_readOnly(): boolean;
    set_readOnly(value: boolean): void;
    protected getInsertPermission(): string;
    protected getUpdatePermission(): string;
    protected getDeletePermission(): string;
    protected hasDeletePermission(): boolean;
    protected hasInsertPermission(): boolean;
    protected hasUpdatePermission(): boolean;
    protected hasSavePermission(): boolean;
    protected editClicked: boolean;
    protected isViewMode(): boolean;
    protected useViewMode(): boolean;
    protected getFallbackTemplate(): string;
}

type JsxDomWidgetProps<P> = P & WidgetComponentProps<any> & {
    children?: any | undefined;
    class?: string;
};
interface JsxDomWidget<P = {}, TElement extends Element = HTMLElement> {
    (props: JsxDomWidgetProps<P>, context?: any): TElement | null;
}
declare function jsxDomWidget<TWidget extends Widget<TOptions>, TOptions>(type: new (element: JQuery, options?: TOptions) => TWidget): JsxDomWidget<TOptions & {
    ref?: (r: TWidget) => void;
}>;

declare namespace Reporting {
    interface ReportDialogOptions {
        reportKey?: string;
    }
    class ReportDialog extends TemplatedDialog<ReportDialogOptions> {
        constructor(opt: ReportDialogOptions);
        protected propertyGrid: PropertyGrid;
        protected propertyItems: PropertyItem[];
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
    class ReportPage extends Widget<any> {
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

interface ScriptContext {
}
declare class ScriptContext {
}

declare class IAsyncInit {
}

declare namespace WX {
    function getWidget<TWidget>(element: JQuery, type: any): TWidget;
    var getWidgetName: typeof Widget.getWidgetName;
    function hasOriginalEvent(e: any): boolean;
    function change(widget: any, handler: any): void;
    function changeSelect2(widget: any, handler: any): void;
    function getGridField(widget: Widget<any>): JQuery;
}

declare class Flexify extends Widget<FlexifyOptions> {
    private xDifference;
    private yDifference;
    constructor(container: JQuery, options: FlexifyOptions);
    storeInitialSize(): void;
    private getXFactor;
    private getYFactor;
    private resizeElements;
    private resizeElement;
}
interface FlexifyOptions {
    getXFactor?: (p1: JQuery) => any;
    getYFactor?: (p1: JQuery) => any;
    designWidth?: any;
    designHeight?: any;
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
declare class GoogleMap extends Widget<GoogleMapOptions> {
    private map;
    constructor(container: JQuery, opt: GoogleMapOptions);
    get_map(): any;
}

declare class Select2AjaxEditor<TOptions, TItem> extends Widget<TOptions> implements IStringValue {
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
    get value(): string;
    set_value(value: string): void;
    set value(v: string);
}

/**
 * ## Serenity Core Library
 *
 * This is the package containing core TypeScript classes and functions used in Serenity applications.
 *
 * It should be installed by default in your projects created from `Serene` or `StartSharp` template:
 *
 * ```json
 * {
 *   "dependencies": {
 *     // ...
 *     "@serenity-is/corelib": "6.9.0"
 *   }
 * }
 * ```
 *
 * The version number for this package should be equal or as close as possible to Serenity NuGet package versions in your project file.
 *
 * > When using classic namespaces instead of the ESM modules, the types and functions in this module are directly available from the global `Serenity` and `Q` namespaces.
 * > e.g. `Serenity.EntityGrid`
 * @packageDocumentation
 */

type Constructor<T> = new (...args: any[]) => T;

export { AggregateFormatting, Aggregators, AlertOptions, AnyWidgetClass, ArgumentNullException, Authorization, BaseEditorFiltering, BaseFiltering, BooleanEditor, BooleanFiltering, BooleanFormatter, CKEditorConfig, CancellableViewCallback, CaptureOperationType, CascadedWidgetLink, CategoryAttribute, CheckLookupEditor, CheckLookupEditorOptions, CheckTreeEditor, CheckTreeItem, CheckboxFormatter, ColumnPickerDialog, ColumnSelection, ColumnsBase, ColumnsKeyAttribute, CommonDialogOptions, Config, ConfirmOptions, Constructor, CreateWidgetParams, Criteria, CriteriaBuilder, CriteriaOperator, CriteriaWithText, CssClassAttribute, Culture, DataChangeInfo, DataGrid, DateEditor, DateFiltering, DateFormat, DateFormatter, DateTimeEditor, DateTimeEditorOptions, DateTimeFiltering, DateTimeFormatter, DateYearEditor, DateYearEditorOptions, DebouncedFunction, DecimalEditor, DecimalEditorOptions, DecimalFiltering, Decorators, DefaultValueAttribute, DeleteRequest, DeleteResponse, DialogButton, DialogExtensions, DialogTypeAttribute, DialogTypeRegistry, Dictionary, DisplayNameAttribute, EditorAttribute, EditorFiltering, EditorOptionAttribute, EditorTypeAttribute, EditorTypeAttributeBase, EditorTypeRegistry, EditorUtils, ElementAttribute, EmailAddressEditor, EmailEditor, EmailEditorOptions, EntityDialog, EntityGrid, EntityTypeAttribute, Enum, EnumEditor, EnumEditorOptions, EnumFiltering, EnumFormatter, EnumKeyAttribute, EnumTypeRegistry, ErrorHandling, Exception, FileDownloadFormatter, FileUploadConstraints, FileUploadEditor, FileUploadEditorOptions, FilterDialog, FilterDisplayBar, FilterLine, FilterOperator, FilterOperators, FilterPanel, FilterStore, FilterWidgetBase, FilterableAttribute, FilteringTypeRegistry, Flexify, FlexifyAttribute, FlexifyOptions, FormKeyAttribute, Format, Formatter, FormatterTypeRegistry, GeneratedCodeAttribute, GoogleMap, GoogleMapOptions, GridPersistanceFlags, GridRadioSelectionMixin, GridRadioSelectionMixinOptions, GridRowSelectionMixin, GridRowSelectionMixinOptions, GridSelectAllButtonHelper, GridUtils, GroupByElement, GroupByResult, GroupInfo, Grouping, HandleRouteEventArgs, HiddenAttribute, HintAttribute, HtmlContentEditor, HtmlContentEditorOptions, HtmlNoteContentEditor, HtmlReportContentEditor, IAsyncInit, IBooleanValue, IDataGrid, IDialog, IDoubleValue, IEditDialog, IFiltering, IFrameDialogOptions, IGetEditValue, IInitializeColumn, IQuickFiltering, IReadOnly, IRowDefinition, ISetEditValue, ISlickFormatter, IStringValue, IValidateRequired, IdPropertyAttribute, ImageUploadEditor, ImageUploadEditorOptions, InsertableAttribute, IntegerEditor, IntegerEditorOptions, IntegerFiltering, InvalidCastException, Invariant, IsActivePropertyAttribute, ItemNameAttribute, JQBlockUIOptions, JsxDomWidget, LT, LayoutTimer, LazyLoadHelper, ListRequest, ListResponse, LocalTextPrefixAttribute, Locale, Lookup, LookupEditor, LookupEditorBase, LookupEditorOptions, LookupFiltering, LookupOptions, MaskedEditor, MaskedEditorOptions, MaxLengthAttribute, MaximizableAttribute, MemberType, MinuteFormatter, ModalOptions, MultipleFileUploadEditor, MultipleImageUploadEditor, NamePropertyAttribute, NotifyMap, NumberFormat, NumberFormatter, OneWayAttribute, OptionAttribute, OptionsTypeAttribute, PagerOptions, PagingInfo, PagingOptions, PanelAttribute, PasswordEditor, PersistedGridColumn, PersistedGridSettings, PlaceholderAttribute, PopupMenuButton, PopupMenuButtonOptions, PopupToolButton, PopupToolButtonOptions, PostToServiceOptions, PostToUrlOptions, PrefixedContext, PropertyDialog, PropertyGrid, PropertyGridMode, PropertyGridOptions, PropertyItem, PropertyItemSlickConverter, PropertyItemsData, PropertyPanel, QuickFilter, QuickFilterArgs, QuickFilterBar, QuickFilterBarOptions, QuickSearchField, QuickSearchInput, QuickSearchInputOptions, RadioButtonEditor, RadioButtonEditorOptions, ReadOnlyAttribute, Recaptcha, RecaptchaOptions, ReflectionOptionsSetter, ReflectionUtils, RemoteView, RemoteViewAjaxCallback, RemoteViewFilter, RemoteViewOptions, RemoteViewProcessCallback, Reporting, RequiredAttribute, ResizableAttribute, ResponsiveAttribute, RetrieveColumnSelection, RetrieveLocalizationRequest, RetrieveLocalizationResponse, RetrieveRequest, RetrieveResponse, Router, SaveRequest, SaveRequestWithAttachment, SaveResponse, SaveWithLocalizationRequest, ScriptContext, ScriptData, Select2AjaxEditor, Select2CommonOptions, Select2Editor, Select2EditorOptions, Select2FilterOptions, Select2InplaceAddOptions, Select2SearchPromise, Select2SearchQuery, Select2SearchResult, SelectEditor, SelectEditorOptions, ServiceAttribute, ServiceError, ServiceLookupEditor, ServiceLookupEditorBase, ServiceLookupEditorOptions, ServiceLookupFiltering, ServiceOptions, ServiceRequest, ServiceResponse, SettingStorage, SlickFormatting, SlickHelper, SlickPager, SlickTreeHelper, StringEditor, StringFiltering, SubDialogHelper, SummaryOptions, SummaryType, TabsExtensions, TemplatedDialog, TemplatedPanel, TemplatedWidget, TextAreaEditor, TextAreaEditorOptions, TimeEditor, TimeEditorOptions, ToastContainerOptions, Toastr, ToastrOptions, ToolButton, Toolbar, ToolbarOptions, TreeGridMixin, TreeGridMixinOptions, Type, TypeMember, URLEditor, UndeleteRequest, UndeleteResponse, UpdatableAttribute, UploadHelper, UploadInputOptions, UploadResponse, UploadedFile, UrlFormatter, UserDefinition, VX, ValidationHelper, WX, Widget, WidgetClass, WidgetComponentProps, WidgetDialogClass, addAttribute, addEmptyOption, addOption, addTypeMember, addValidationRule, alert, alertDialog, any, attrEncode, baseValidateOptions, blockUI, blockUndo, bsModalMarkup, canLoadScriptData, cast, centerDialog, clearKeys, clearOptions, closePanel, coalesce, compareStringFactory, confirm, confirmDialog, count, datePickerIconSvg, dbText, dbTryText, debounce, deepClone, defaultNotifyOptions, delegateCombine, delegateRemove, dialogButtonToBS, dialogButtonToUI, endsWith, executeEverytimeWhenVisible, executeOnceWhenVisible, extend, fieldsProxy, findElementWithRelativeId, first, format, formatDate, formatDayHourAndMin, formatISODateTimeUTC, formatNumber, getAttributes, getBaseType, getColumns, getColumnsAsync, getColumnsData, getColumnsDataAsync, getCookie, getForm, getFormAsync, getFormData, getFormDataAsync, getGlobalThis, getHighlightTarget, getInstanceType, getLookup, getLookupAsync, getMembers, getNested, getRemoteData, getRemoteDataAsync, getStateStore, getTemplate, getTemplateAsync, getType, getTypeFullName, getTypeNameProp, getTypeShortName, getTypes, groupBy, htmlEncode, iframeDialog, indexOf, information, informationDialog, initFormType, initFullHeightGridPage, initializeTypes, insert, isArray, isAssignableFrom, isBS3, isBS5Plus, isEmptyOrNull, isEnum, isInstanceOfType, isMobileView, isTrimmedEmpty, isValue, jsxDomWidget, keyOf, layoutFillHeight, layoutFillHeightValue, loadValidationErrorMessages, localText, localeFormat, newBodyDiv, notifyError, notifyInfo, notifySuccess, notifyWarning, openPanel, outerHtml, padLeft, parseCriteria, parseDate, parseDayHourAndMin, parseDecimal, parseHourAndMin, parseISODateTime, parseInteger, parseQueryString, positionToastContainer, postToService, postToUrl, prefixedText, prop, proxyTexts, reactPatch, registerClass, registerEditor, registerEnum, registerInterface, reloadLookup, reloadLookupAsync, removeValidationRule, replaceAll, resolveUrl, round, safeCast, serviceCall, serviceRequest, setEquality, setTypeNameProp, single, splitDateString, startsWith, success, successDialog, text, toGrouping, toId, toSingleLine, today, toggleClass, triggerLayoutOnShow, trim, trimEnd, trimStart, trimToEmpty, trimToNull, trunc, tryFirst, tryGetText, turkishLocaleCompare, turkishLocaleToLower, turkishLocaleToUpper, useIdPrefix, validateForm, validateOptions, validatorAbortHandler, warning, warningDialog, zeroPad };
