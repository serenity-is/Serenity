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
 * Formats a string with parameters similar to .NET's String.Format function
 * using current `Culture` locale settings.
 */
declare function stringFormat(format: string, ...prm: any[]): string;
/**
 * Formats a string with parameters similar to .NET's String.Format function
 * using the locale passed as the first argument.
 */
declare function stringFormatLocale(l: Locale, format: string, ...prm: any[]): string;
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
 * Html encodes a string (encodes single and double quotes, & (ampersand), > and < characters)
 * @param s String (or number etc.) to be HTML encoded
 */
declare function htmlEncode(s: any): string;
/**
 * Toggles the class on the element handling spaces like addClass does.
 * @param el the element
 * @param cls the class to toggle
 * @param add if true, the class will be added, if false the class will be removed, otherwise it will be toggled.
 */
declare function toggleClass(el: Element, cls: string, add?: boolean): void;

declare function addLocalText(obj: string | Record<string, string | Record<string, any>> | string, pre?: string): void;
declare function localText(key: string, defaultText?: string): string;
declare function tryGetText(key: string): string;
declare function proxyTexts(o: Record<string, any>, p: string, t: Record<string, any>): Object;

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
declare function positionToastContainer(create: boolean, options?: ToastrOptions): void;
declare function notifyWarning(message: string, title?: string, options?: ToastrOptions): void;
declare function notifySuccess(message: string, title?: string, options?: ToastrOptions): void;
declare function notifyInfo(message: string, title?: string, options?: ToastrOptions): void;
declare function notifyError(message: string, title?: string, options?: ToastrOptions): void;

declare enum SummaryType {
    Disabled = -1,
    None = 0,
    Sum = 1,
    Avg = 2,
    Min = 3,
    Max = 4
}
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

declare function resolveUrl(url: string): string;
declare function resolveServiceUrl(url: string): string;

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

declare function getGlobalThis(): any;
declare function getStateStore(key?: string): any;
declare function getTypeStore(): any;
interface TypeMetadata {
    enumFlags?: boolean;
    attr?: any[];
}
type Type = Function | Object;
declare function ensureMetadata(target: Type): TypeMetadata;
declare function getNested(from: any, name: string): any;
declare function getType(name: string, target?: any): Type;
declare function getTypeNameProp(type: Type): string;
declare function setTypeNameProp(type: Type, value: string): void;
declare function getTypeFullName(type: Type): string;
declare function getTypeShortName(type: Type): string;
declare function getInstanceType(instance: any): any;
declare function isAssignableFrom(target: any, type: Type): boolean;
declare function isInstanceOfType(instance: any, type: Type): boolean;
declare function getBaseType(type: any): any;
declare function registerClass(type: any, name: string, intf?: any[]): void;
declare function registerEnum(type: any, name: string, enumKey?: string): void;
declare function registerInterface(type: any, name: string, intf?: any[]): void;
declare namespace Enum {
    let toString: (enumType: any, value: number) => string;
    let getValues: (enumType: any) => any[];
}
declare let isEnum: (type: any) => boolean;
declare function initFormType(typ: Function, nameWidgetPairs: any[]): void;
declare function fieldsProxy<TRow>(): Readonly<Record<keyof TRow, string>>;

export { ColumnSelection, Config, Criteria, CriteriaBuilder, CriteriaOperator, Culture, type DateFormat, type DebouncedFunction, type DeleteRequest, type DeleteResponse, Enum, Invariant, type ListRequest, type ListResponse, type Locale, Lookup, type LookupOptions, type NotifyMap, type NumberFormat, type PropertyItem, type PropertyItemsData, RetrieveColumnSelection, type RetrieveLocalizationRequest, type RetrieveLocalizationResponse, type RetrieveRequest, type RetrieveResponse, type SaveRequest, type SaveRequestWithAttachment, type SaveResponse, type SaveWithLocalizationRequest, type ServiceError, type ServiceRequest, type ServiceResponse, SummaryType, type ToastContainerOptions, Toastr, type ToastrOptions, type Type, type UndeleteRequest, type UndeleteResponse, addLocalText, compareStringFactory, debounce, defaultNotifyOptions, ensureMetadata, fieldsProxy, formatDate, formatISODateTimeUTC, formatNumber, getBaseType, getGlobalThis, getInstanceType, getNested, getStateStore, getType, getTypeFullName, getTypeNameProp, getTypeShortName, getTypeStore, htmlEncode, initFormType, isAssignableFrom, isEnum, isInstanceOfType, localText, notifyError, notifyInfo, notifySuccess, notifyWarning, parseCriteria, parseDate, parseDecimal, parseISODateTime, parseInteger, positionToastContainer, proxyTexts, registerClass, registerEnum, registerInterface, resolveServiceUrl, resolveUrl, round, setTypeNameProp, splitDateString, stringFormat, stringFormatLocale, toId, toggleClass, trunc, tryGetText };
