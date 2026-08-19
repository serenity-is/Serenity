/** 
 * Adapted from 3.5.x version of Select2 (https://github.com/select2/select2), removing jQuery dependency
 */

import { bindThis } from "@serenity-is/domwise";
import { debounce, Fluent, SelectEditorTexts, serviceCall, stringFormat } from "../../base";

/** Element type that can host a Select2. */
export type Select2Element = HTMLInputElement | HTMLSelectElement;
/** Result of a Select2 formatter. */
export type Select2FormatResult = string | Element | DocumentFragment;

/**
 * Options passed to a Select2 query callback.
 */
export interface Select2QueryOptions {
    /** The element the query is for. */
    element?: Select2Element;
    /** The search term. */
    term?: string;
    /** The page number. */
    page?: number;
    /** Additional context. */
    context?: any;
    /** Callback invoked with the results. */
    callback?: (p1: Select2Result) => void;
    /** Custom matcher function. */
    matcher?: (p1: any, p2: any, p3?: any) => boolean;
}

/**
 * A single Select2 item.
 */
export interface Select2Item {
    /** Item id. */
    id?: string;
    /** Display text. */
    text?: string;
    /** The source item. */
    source?: any;
    /** Child items. */
    children?: Select2Item[];
    /** Whether the item is disabled. */
    disabled?: boolean;
    /** Whether the item is locked. */
    locked?: boolean;
}

/**
 * Result of a Select2 query.
 */
export interface Select2Result {
    /** Whether the query failed. */
    hasError?: boolean;
    /** Error information. */
    errorInfo?: any;
    /** The result items. */
    results: Select2Item[];
    /** Whether there are more results. */
    more?: boolean;
    /** Additional context. */
    context?: any;
}

/**
 * Options for ajax-based Select2 queries.
 */
export interface Select2AjaxOptions extends RequestInit {
    /** Request headers. */
    headers?: Record<string, string>;
    /** The URL or a function returning it. */
    url?: string | ((term: string, page: number, context: any) => string);
    /** Delay in milliseconds before the ajax request. */
    quietMillis?: number;
    /** Callback that builds the request data. */
    data?: (p1: string, p2: number, p3: any) => any;
    /** Callback that processes the response. */
    results?: (p1: any, p2: number, p3: any) => any;
    /** Additional request parameters. */
    params?: (() => any) | any;
    /** Callback invoked on error. */
    onError?(response: any, info?: any): void | boolean;
    /** Callback invoked on success. */
    onSuccess?(response: any): void;
}

/**
 * Options for the {@link Select2} widget.
 */
export interface Select2Options {
    /** The element to attach Select2 to. */
    element?: Select2Element;
    /** Width of the widget. */
    width?: any;
    /** Minimum input length before searching. */
    minimumInputLength?: number;
    /** Maximum input length. */
    maximumInputLength?: number;
    /** Minimum results required to show the search box. */
    minimumResultsForSearch?: number;
    /** Maximum number of selectable items. */
    maximumSelectionSize?: any;
    /** Placeholder text. */
    placeholder?: string;
    /** Placeholder option. */
    placeholderOption?: any;
    /** Separator for multiple values. */
    separator?: string;
    /** Whether the selection can be cleared. */
    allowClear?: boolean;
    /** Whether multiple items can be selected. */
    multiple?: boolean;
    /** Whether to close the dropdown on select. */
    closeOnSelect?: boolean;
    /** Whether to open the dropdown on enter. */
    openOnEnter?: boolean;
    /** Callback that returns the id of an item. */
    id?: (p1: any) => string;
    /** Custom matcher function. */
    matcher?: (p1: string, p2: string, p3: HTMLElement) => boolean;
    /** Callback that sorts results. */
    sortResults?: (p1: any, p2: HTMLElement, p3: any) => any;
    /** Formatter for ajax errors. */
    formatAjaxError?: (p1: any, p2: any) => Select2FormatResult;
    /** Formatter for the matches count. */
    formatMatches?: (matches: number) => Select2FormatResult;
    /** Formatter for selected items. */
    formatSelection?: (p1: any, p2: HTMLElement, p3: (p1: string) => string) => Select2FormatResult;
    /** Formatter for result items. */
    formatResult?: (p1: any, p2: HTMLElement, p3: any, p4: (p1: string) => string) => Select2FormatResult;
    /** Formatter for result CSS classes. */
    formatResultCssClass?: (p1: any) => string;
    /** Formatter for selection CSS classes. */
    formatSelectionCssClass?: (item: Select2Item, container: HTMLElement) => string;
    /** Formatter for no-matches text. */
    formatNoMatches?: (input: string) => Select2FormatResult;
    /** Formatter for load-more text. */
    formatLoadMore?: (pageNumber: number) => Select2FormatResult;
    /** Formatter for searching text. */
    formatSearching?: () => Select2FormatResult;
    /** Formatter for input-too-long text. */
    formatInputTooLong?: (input: string, max: number) => Select2FormatResult;
    /** Formatter for input-too-short text. */
    formatInputTooShort?: (input: string, min: number) => Select2FormatResult;
    /** Formatter for selection-too-big text. */
    formatSelectionTooBig?: (p1: number) => Select2FormatResult;
    /** Callback that creates a search choice. */
    createSearchChoice?: (p1: string) => Select2Item;
    /** Position of the create-search-choice item. */
    createSearchChoicePosition?: string | ((list: Select2Item[], item: Select2Item) => void);
    /** Callback that initializes the selection. */
    initSelection?: (p1: HTMLElement, p2: (p1: any) => void) => void;
    /** Tokenizer function. */
    tokenizer?: (p1: string, p2: any, p3: (p1: any) => any, p4: any) => string;
    /** Token separators. */
    tokenSeparators?: any;
    /** Query callback. */
    query?: (p1: Select2QueryOptions) => void;
    /** Ajax options. */
    ajax?: Select2AjaxOptions;
    /** Static data. */
    data?: any;
    /** Tags for tag mode. */
    tags?: ((string | Select2Item)[]) | (() => (string | Select2Item)[]);
    /** Container CSS. */
    containerCss?: any;
    /** Container CSS class. */
    containerCssClass?: any;
    /** Dropdown CSS. */
    dropdownCss?: any;
    /** Dropdown CSS class. */
    dropdownCssClass?: any;
    /** Whether the dropdown auto-widths. */
    dropdownAutoWidth?: boolean;
    /** Callback that returns the dropdown parent. */
    dropdownParent?: (input: HTMLElement) => HTMLElement;
    /** Callback that adapts the container CSS class. */
    adaptContainerCssClass?: (p1: string) => string;
    /** Callback that adapts the dropdown CSS class. */
    adaptDropdownCssClass?: (p1: string) => string;
    /** Callback that escapes markup. */
    escapeMarkup?: (p1: string) => string;
    /** Placeholder for the search input. */
    searchInputPlaceholder?: string;
    /** Whether to select on blur. */
    selectOnBlur?: boolean;
    /** Whether to blur on change. */
    blurOnChange?: boolean;
    /** Padding for load-more. */
    loadMorePadding?: number;
    /** Callback that returns the next search term. */
    nextSearchTerm?: (p1: any, p2: string) => string;
    /** Callback that populates results. */
    populateResults?: (container: HTMLElement, results: Select2Item[], query: Select2QueryOptions) => void
    /** Callback that determines whether to focus the input. */
    shouldFocusInput?: (p1: any) => boolean;
}

var lastMousePosition = { x: 0, y: 0 };

// Grouped by target: the value is the concatenation of every source character that
// transliterates to that target (e.g. all glyphs that normalize to "A"). The reverse
// lookup map is expanded lazily, only when a non-ASCII character actually needs it.
const SPECIAL_DIACRITICS_GROUPS: Record<string, string> = {
    "A": "\u023A\u24B6\u2C6F\uFF21",
    "B": "\u0181\u0182\u0243\u24B7\uFF22",
    "C": "\u0187\u023B\u24B8\uA73E\uFF23",
    "D": "\u0110\u0189\u018A\u018B\u24B9\uA779\uFF24",
    "E": "\u018E\u0190\u24BA\uFF25",
    "F": "\u0191\u24BB\uA77B\uFF26",
    "G": "\u0193\u01E4\u24BC\uA77D\uA77E\uA7A0\uFF27",
    "H": "\u0126\u24BD\u2C67\u2C75\uA78D\uFF28",
    "I": "\u0197\u24BE\uFF29",
    "J": "\u0248\u24BF\uFF2A",
    "K": "\u0198\u24C0\u2C69\uA740\uA742\uA744\uA7A2\uFF2B",
    "L": "\u013F\u0141\u023D\u24C1\u2C60\u2C62\uA746\uA748\uA780\uFF2C",
    "M": "\u019C\u24C2\u2C6E\uFF2D",
    "N": "\u019D\u0220\u24C3\uA790\uA7A4\uFF2E",
    "O": "\u00D8\u0186\u019F\u01FE\u24C4\uA74A\uA74C\uFF2F",
    "P": "\u01A4\u24C5\u2C63\uA750\uA752\uA754\uFF30",
    "Q": "\u024A\u24C6\uA756\uA758\uFF31",
    "R": "\u024C\u24C7\u2C64\uA75A\uA782\uA7A6\uFF32",
    "S": "\u1E9E\u24C8\u2C7E\uA784\uA7A8\uFF33",
    "T": "\u0166\u01AC\u01AE\u023E\u24C9\uA786\uFF34",
    "U": "\u0244\u24CA\uFF35",
    "V": "\u01B2\u0245\u24CB\uA75E\uFF36",
    "W": "\u24CC\u2C72\uFF37",
    "X": "\u24CD\uFF38",
    "Y": "\u01B3\u024E\u1EFE\u24CE\uFF39",
    "Z": "\u01B5\u0224\u24CF\u2C6B\u2C7F\uA762\uFF3A",
    "a": "\u0250\u1E9A\u24D0\u2C65\uFF41",
    "b": "\u0180\u0183\u0253\u24D1\uFF42",
    "c": "\u0188\u023C\u2184\u24D2\uA73F\uFF43",
    "d": "\u0111\u018C\u0256\u0257\u24D3\uA77A\uFF44",
    "e": "\u01DD\u0247\u025B\u24D4\uFF45",
    "f": "\u0192\u24D5\uA77C\uFF46",
    "g": "\u01E5\u0260\u1D79\u24D6\uA77F\uA7A1\uFF47",
    "h": "\u0127\u0265\u24D7\u2C68\u2C76\uFF48",
    "i": "\u0131\u0268\u24D8\uFF49",
    "j": "\u0249\u24D9\uFF4A",
    "k": "\u0199\u24DA\u2C6A\uA741\uA743\uA745\uA7A3\uFF4B",
    "l": "\u0140\u0142\u017F\u019A\u026B\u24DB\u2C61\uA747\uA749\uA781\uFF4C",
    "m": "\u026F\u0271\u24DC\uFF4D",
    "n": "\u0149\u019E\u0272\u24DD\uA791\uA7A5\uFF4E",
    "o": "\u00F8\u01FF\u0254\u0275\u24DE\uA74B\uA74D\uFF4F",
    "p": "\u01A5\u1D7D\u24DF\uA751\uA753\uA755\uFF50",
    "q": "\u024B\u24E0\uA757\uA759\uFF51",
    "r": "\u024D\u027D\u24E1\uA75B\uA783\uA7A7\uFF52",
    "s": "\u00DF\u023F\u1E9B\u24E2\uA785\uA7A9\uFF53",
    "t": "\u0167\u01AD\u0288\u24E3\u2C66\uA787\uFF54",
    "u": "\u0289\u24E4\uFF55",
    "v": "\u028B\u028C\u24E5\uA75F\uFF56",
    "w": "\u24E6\u2C73\uFF57",
    "x": "\u24E7\uFF58",
    "y": "\u01B4\u024F\u1EFF\u24E8\uFF59",
    "z": "\u01B6\u0225\u0240\u24E9\u2C6C\uA763\uFF5A",
    "aa": "\uA733",
    "AA": "\uA732",
    "ae": "\u00E6\u01E3\u01FD",
    "AE": "\u00C6\u01E2\u01FC",
    "ao": "\uA735",
    "AO": "\uA734",
    "au": "\uA737",
    "AU": "\uA736",
    "av": "\uA739\uA73B",
    "AV": "\uA738\uA73A",
    "ay": "\uA73D",
    "AY": "\uA73C",
    "dz": "\u01C6\u01F3",
    "Dz": "\u01C5\u01F2",
    "DZ": "\u01C4\u01F1",
    "hv": "\u0195",
    "lj": "\u01C9",
    "Lj": "\u01C8",
    "LJ": "\u01C7",
    "nj": "\u01CC",
    "Nj": "\u01CB",
    "NJ": "\u01CA",
    "oi": "\u01A3",
    "OI": "\u01A2",
    "oo": "\uA74F",
    "OO": "\uA74E",
    "ou": "\u0223",
    "OU": "\u0222",
    "tz": "\uA729",
    "TZ": "\uA728",
    "vy": "\uA761",
    "VY": "\uA760",
    "\u03C3": "\u03C2",
};

let specialDiacritics: Record<string, string>;

function getSpecialDiacritics(): Record<string, string> {
    if (specialDiacritics === undefined) {
        const map: Record<string, string> = {};
        for (const target of Object.keys(SPECIAL_DIACRITICS_GROUPS)) {
            const chars = SPECIAL_DIACRITICS_GROUPS[target];
            for (let i = 0; i < chars.length; i++)
                map[chars[i]] = target;
        }
        specialDiacritics = map;
    }
    return specialDiacritics;
}

const nextUid = (function () { var counter = 1; return function () { return counter++; }; }());


function reinsertElement(element: HTMLElement) {
    var placeholder = document.createTextNode('');
    Fluent(placeholder).insertBefore(element);
    Fluent(element).insertBefore(placeholder as any);
    placeholder.remove();
}

function indexOf(value: any, array: any[]) {
    var i = 0, l = array.length;
    for (; i < l; i = i + 1) {
        if (equal(value, array[i])) return i;
    }
    return -1;
}

let scrollBarDimensions: { width: number, height: number } = null;

function getOffset(el: Element) {
    var box = el.getBoundingClientRect();
    var docElem = document.documentElement;
    return {
        top: box.top + window.scrollY - docElem.clientTop,
        left: box.left + window.scrollX - docElem.clientLeft
    };
}

function measureScrollbar() {
    var $template = document.createElement("div");
    $template.classList.add("select2-measure-scrollbar");
    document.body.appendChild($template);

    var dim = {
        width: $template.offsetWidth - $template.clientWidth,
        height: $template.offsetHeight - $template.clientHeight
    };
    $template.remove();

    return dim;
}

/**
 * Compares equality of a and b
 * @param a
 * @param b
 */
function equal(a: any, b: any): boolean {
    if (a === b) return true;
    if (a === undefined || b === undefined) return false;
    if (a === null || b === null) return false;
    // Check whether 'a' or 'b' is a string (primitive or object).
    // The concatenation of an empty string (+'') converts its argument to a string's primitive.
    if (a.constructor === String) return a + '' === b + ''; // a+'' - in case 'a' is a String object
    if (b.constructor === String) return b + '' === a + ''; // b+'' - in case 'b' is a String object
    return false;
}

/**
 * Splits the string into an array of values, trimming each value. An empty array is returned for nulls or empty
 * strings
 * @param string
 * @param separator
 */
function splitVal(string: string, separator: string) {
    var val, i, l;
    if (string === null || string.length < 1) return [];
    val = string.split(separator);
    for (i = 0, l = val.length; i < l; i = i + 1) val[i] = val[i].trim();
    return val;
}

function parsePx(str: string) {
    var value = parseFloat(str);
    if (isNaN(value))
        return 0;
    return value;
}

function getSideBorderPadding(el: Element) {
    if (!el)
        return 0;

    var style = getComputedStyle(el);
    var p = ["border-left-width", "border-right-width", "padding-left", "padding-right"];
    var delta = 0;
    for (var val of p)
        delta += parsePx(style.getPropertyValue(val)) || 0;
    return delta;
}

function getOuterHeightWithMargins(el: Element) {
    const style = getComputedStyle(el);

    return (
        el.getBoundingClientRect().height +
        parseFloat(style.marginTop) +
        parseFloat(style.marginBottom)
    );
}

function installKeyUpChangeEvent(element: HTMLElement) {
    var key = "keyupChangeValue";
    Fluent.on(element, "keydown", function () {
        if (element.dataset[key] === undefined) {
            element.dataset[key] = (element as any).value;
        }
    });
    Fluent.on(element, "keyup", function () {
        var val = element.dataset[key];
        if (val !== undefined && (element as any).value !== val) {
            delete element.dataset[key];
            Fluent.trigger(element, "keyup-change");
        }
    });
}


/**
 * filters mouse events so an event is fired only if the mouse moved.
 *
 * filters out mouse events that occur when mouse is stationary but
 * the elements under the pointer are scrolled.
 */
function installFilteredMouseMove(element: HTMLElement) {
    Fluent.on(element, "mousemove", function (e) {
        var lastpos = lastMousePosition;
        if (lastpos === undefined || lastpos.x !== e.pageX || lastpos.y !== e.pageY) {
            Fluent.trigger(e.target, "mousemove-filtered", { pageX: e.pageX, pageY: e.pageY });
        }
    });
}

function installDebouncedScroll(threshold: number, element: Element) {
    var notify = debounce(function (args?: any) { Fluent.trigger(element, "scroll-debounced", args); }, threshold);
    Fluent.on(element, "scroll", function (e) {
        if (e.target === element) notify();
    });
}

function focus($el: HTMLElement) {
    if ($el === document.activeElement) return;

    /* set the focus in a 0 timeout - that way the focus is set after the processing
        of the current event has finished - which seems like the only reliable way
        to set focus */
    window.setTimeout(function () {
        var el = $el, pos = ($el as any).value?.length || 0;

        $el.focus();

        /* make sure el received focus so we do not error out when trying to manipulate the caret.
            sometimes modals or others listeners may steal it after its set */
        var isVisible = (el.offsetWidth > 0 || el.offsetHeight > 0);
        if (isVisible && el === document.activeElement) {

            /* after the focus is set move the caret to the end */
            (el as HTMLInputElement).setSelectionRange(pos, pos);
        }
    }, 0);
}

function getCursorInfo(el: HTMLElement) {
    const input = el as HTMLInputElement;
    const offset = input.selectionStart ?? 0;
    const length = (input.selectionEnd ?? offset) - offset;
    return { offset, length };
}

function killEvent(event: Event) {
    event.preventDefault();
    event.stopPropagation();
}
function killEventImmediately(event: Event) {
    event.preventDefault();
    event.stopImmediatePropagation();
}

let sizer: HTMLDivElement;

function measureTextWidth(e: Element) {
    if (!sizer) {
        var style = getComputedStyle(e, null);
        sizer = document.createElement("div");
        Object.assign(sizer.style, {
            position: "absolute",
            left: "-10000px",
            top: "-10000px",
            display: "none",
            fontSize: style.fontSize,
            fontFamily: style.fontFamily,
            fontStyle: style.fontStyle,
            fontWeight: style.fontWeight,
            letterSpacing: style.letterSpacing,
            textTransform: style.textTransform,
            whiteSpace: "nowrap"
        });
        sizer.setAttribute("class", "select2-sizer");
        document.body.appendChild(sizer);
    }
    sizer.textContent = (e as any).value;
    return sizer.offsetWidth;
}

function syncCssClasses(dest: Element, src: Element, adapter: (kls: string) => string) {
    var classes: string, replacements: string[] = [], adapted;

    classes = dest.getAttribute("class")?.trim();

    if (classes) {
        classes = '' + classes; // for IE which returns object

        classes.split(/\s+/).forEach(function (kls) {
            if (kls.indexOf("select2-") === 0) {
                replacements.push(kls);
            }
        });
    }

    classes = src.getAttribute("class")?.trim();

    if (classes) {
        classes = '' + classes; // for IE which returns object

        classes.split(/\s+/).forEach(function (kls) {
            if (kls.indexOf("select2-") !== 0) {
                adapted = adapter(kls);

                if (adapted) {
                    replacements.push(adapted);
                }
            }
        });
    }

    dest.setAttribute("class", replacements.join(" "));
}

function defaultEscapeMarkup(markup: string) {
    var replace_map: Record<string, string> = {
        '\\': '&#92;',
        '&': '&amp;',
        '<': '&lt;',
        '>': '&gt;',
        '"': '&quot;',
        "'": '&#39;',
        "/": '&#47;'
    };

    return String(markup).replace(/[&<>"'\/\\]/g, function (match) {
        return replace_map[match];
    });
}

/**
 * A searchable select widget ported from Select2.
 */
export class Select2 {

    declare private el: Select2Element;

    constructor(opts?: Select2Options)
    /**
     * Creates a Select2 widget.
     * @param opts - Select2 options.
     * @param create - When false, only wraps an existing Select2 without creating a new one.
     */
    constructor(opts?: Select2Options, create: boolean = true) {

        this.el = opts.element;
        if (!create) {
            return;
        }

        opts = Object.assign({}, opts);

        var multiple: boolean;
        if (opts.element.tagName.toLowerCase() === "select") {
            multiple = opts.element.multiple;
        } else {
            multiple = opts.multiple || false;
            if ("tags" in opts) { opts.multiple = multiple = true; }
        }

        var select2 = multiple ? new MultiSelect2() : new SingleSelect2();
        select2.init(opts);
    }

    private get instance(): AbstractSelect2 {
        if (!this.el)
            return null;
        return (this.el as any).select2;
    }

    /**
     * Closes the dropdown.
     */
    close(): void {
        this.instance?.close();
    }

    /**
     * Returns the container element.
     * @returns The container element.
     */
    get container(): HTMLElement {
        return this.instance?.container;
    }

    /**
     * Returns the dropdown element.
     * @returns The dropdown element.
     */
    get dropdown(): HTMLElement {
        return this.instance?.dropdown;
    }

    /**
     * Destroys the Select2 instance.
     */
    destroy(): void {
        this.instance?.destroy();
    }

    /**
     * Returns the current data.
     * @returns The selected item(s).
     */
    get data(): (Select2Item | Select2Item[]) {
        return this.instance?.data() as (Select2Item | Select2Item[]);
    }

    /** Sets the current data. */
    set data(value: Select2Item | Select2Item[]) {
        this.instance?.data(value);
    }

    /**
     * Disables the Select2 widget.
     */
    disable(): void {
        this.instance?.disable();
    }

    /**
     * Enables or disables the Select2 widget.
     * @param enabled - Whether to enable.
     */
    enable(enabled?: boolean): void {
        this.instance?.enable(enabled);
    }

    /**
     * Focuses the search input.
     */
    focus(): void {
        this.instance?.focus();
    }

    /**
     * Whether the widget is focused.
     * @returns True when focused.
     */
    get isFocused(): boolean {
        return this.instance?.isFocused();
    }

    /**
     * Whether the widget allows multiple selection.
     * @returns True when multiple.
     */
    get isMultiple(): boolean {
        return this.instance instanceof MultiSelect2;
    }

    /**
     * Whether the dropdown is open.
     * @returns True when open.
     */
    get opened(): boolean {
        return this.instance?.opened();
    }

    /**
     * Opens the dropdown.
     * @returns True when opened.
     */
    open(): boolean {
        return this.instance?.open();
    }

    /**
     * Repositions the dropdown.
     */
    positionDropdown() {
        this.instance?.positionDropdown();
    }

    /**
     * Sets the read-only state.
     * @param value - Whether to enable read-only mode.
     */
    readonly(value?: boolean): void {
        this.instance?.readonly(value);
    }

    /**
     * Returns the search input element.
     * @returns The search input.
     */
    get search(): HTMLInputElement {
        return this.instance?.search;
    }

    /**
     * Returns the current value.
     * @returns The value.
     */
    get val(): (string | string[]) {
        return this.instance?.val();
    }

    /** Sets the current value. */
    set val(value: string | string[]) {
        this.instance?.val(value);
    }

    /**
     * Returns the Select2 instance attached to an element, or null.
     * @param el - The element.
     * @returns The Select2 instance, or null.
     */
    static getInstance(el: Select2Element): Select2 {
        if (!el || !(el as any).select2)
            return null;
        return new (Select2 as any)({ element: el }, false);
    }

    /** Default ajax options. */
    static readonly ajaxDefaults: Select2AjaxOptions = {
        params: {
            method: "GET",
            cache: false,
            dataType: "json"
        }
    }

    // plugin defaults, accessible to users
    static readonly defaults: Select2Options = {
        width: "copy",
        loadMorePadding: 0,
        closeOnSelect: true,
        openOnEnter: true,
        containerCss: {},
        dropdownCss: {},
        containerCssClass: "",
        dropdownCssClass: "",
        formatAjaxError: () => SelectEditorTexts.AjaxError,
        formatInputTooLong: (input: string, max: number) => stringFormat(SelectEditorTexts.InputTooLong, input.length - max, max, input.length),
        formatInputTooShort: (input: string, min: number) => stringFormat(SelectEditorTexts.InputTooShort, min - input.length, min, input.length),
        formatLoadMore: (pageNumber: number) => stringFormat(SelectEditorTexts.LoadMore, pageNumber),
        formatMatches: (matches: number) => matches === 1 ? SelectEditorTexts.SingleMatch : stringFormat(SelectEditorTexts.MultipleMatches, matches),
        formatNoMatches: () => SelectEditorTexts.NoMatches,
        formatResult: function (result, _, query) {
            return Select2.highlightMatch(result?.text, query?.term);
        },
        formatResultCssClass: function (data) { return data.css; },
        formatSearching: () => SelectEditorTexts.Searching,
        formatSelection: (data) => data ? data.text : undefined,
        formatSelectionCssClass: function () { return undefined; },
        formatSelectionTooBig: (limit: number) => stringFormat(SelectEditorTexts.SelectionTooBig, limit),
        sortResults: results => results,
        minimumResultsForSearch: 0,
        minimumInputLength: 0,
        maximumInputLength: null,
        maximumSelectionSize: 0,
        id: function (e) { return e == undefined ? null : e.id; },
        matcher: (term, text) => Select2.stripDiacritics('' + text).toUpperCase().indexOf(Select2.stripDiacritics('' + term).toUpperCase()) >= 0,
        separator: ",",
        tokenSeparators: [],
        tokenizer: defaultTokenizer,
        escapeMarkup: defaultEscapeMarkup,
        blurOnChange: false,
        selectOnBlur: false,
        adaptContainerCssClass: function (c) { return c; },
        adaptDropdownCssClass: function (c) { return null; },
        nextSearchTerm: function (selectedObject, currentSearchTerm) { return undefined; },
        searchInputPlaceholder: '',
        createSearchChoicePosition: 'top',
        shouldFocusInput: function (instance) {
            // Attempt to detect touch devices
            var supportsTouchEvents = (('ontouchstart' in window) ||
                ('msMaxTouchPoints' in navigator));

            // Only devices which support touch events should be special cased
            if (!supportsTouchEvents) {
                return true;
            }

            // Never focus the input if search is disabled
            if (instance.opts.minimumResultsForSearch < 0) {
                return false;
            }

            return true;
        }
    };

    /**
     * Highlights the matching portion of text for a search term.
     * @param text - The text to highlight.
     * @param term - The search term.
     * @returns The highlighted result.
     */
    static highlightMatch(text: string, term: string): Select2FormatResult {
        if (!text || !term)
            return text;

        const match = Select2.stripDiacritics(text.toUpperCase()).indexOf(Select2.stripDiacritics(term.toUpperCase()));
        if (match < 0)
            return text;

        const tl = term.length;

        return <>{text.substring(0, match)}<span className="select2-match">{text.substring(match, match + tl)}</span>{text.substring(match + tl, text.length)}</>;
    }

    /**
     * Strips diacritics from a string for accent-insensitive matching.
     * @param str - The string to process.
     * @returns The string with diacritics removed.
     */
    static stripDiacritics(str: string) {
        // Curated overrides first (chars whose Unicode decomposition would differ from the
        // table, e.g. U+1E9B long-s-with-dot -> "s", since its decomposition contains
        // U+017F long-s which maps to "l"), then the generic NFD decomposition strips
        // remaining combining marks. The reverse lookup map is expanded lazily from the
        // compact SPECIAL_DIACRITICS_GROUPS table on the first call that needs it.
        const specialDiacritics = getSpecialDiacritics();
        return str.replace(/[^\u0000-\u007E]/g, c => specialDiacritics[c] ?? c)
            .normalize("NFD")
            .replace(/\p{M}/gu, "");
    }
}

/**
 * Produces an ajax-based query function
 *
 * @param options object containing configuration parameters
 * @param options.params parameter map for the transport ajax call, can contain such options as cache, jsonpCallback, etc. see $.ajax
 * @param options.transport function that will be used to execute the ajax request. must be compatible with parameters supported by $.ajax
 * @param options.url url for the data
 * @param options.data a function(searchTerm, pageNumber, context) that should return an object containing query string parameters for the above url.
 * @param options.dataType request data type: ajax, jsonp, other datatypes supported by jQuery's $.ajax function or the transport function if specified
 * @param options.quietMillis (optional) milliseconds to wait before making the ajaxRequest, helps debounce the ajax function if invoked too often
 * @param options.results a function(remoteData, pageNumber, query) that converts data returned form the remote request to the format expected by Select2.
 *      The expected format is an object containing the following keys:
 *      results array of objects that will be used as choices
 *      more (optional) boolean indicating whether there are more results available
 *      Example: {results:[{id:1, text:'Red'},{id:2, text:'Blue'}], more:true}
 */
function ajax(options: Select2AjaxOptions) {
    var timeout: number, // current scheduled but not yet executed request
        quietMillis = options.quietMillis || 100,
        ajaxUrl = options.url,
        self = this;

    return function (query: Select2QueryOptions) {
        clearTimeout(timeout);
        timeout = window.setTimeout(function () {
            var data = options.data, // ajax data function
                url = ajaxUrl, // ajax url string or function
                // deprecated - to be removed in 4.0  - use params instead
                deprecated = {
                    cache: options.cache || false,
                },
                params = Object.assign({}, Select2.ajaxDefaults.params, deprecated);

            data = data ? data.call(self, query.term, query.page, query.context) : null;
            url = (typeof url === 'function') ? url.call(self, query.term, query.page, query.context) : url;

            if (options.params) {
                if (typeof options.params === "function") {
                    Object.assign(params, options.params.call(self));
                } else {
                    Object.assign(params, options.params);
                }
            }

            Object.assign(params, {
                url: url,
                data: data,
                onSuccess: function (response: any) {
                    // TODO - replace query.page with query so users have access to term, page, etc.
                    // added query as third paramter to keep backwards compatibility
                    var results = options.results(response, query.page, query);
                    query.callback(results);
                },
                onError: function (response: any, info: any) {
                    var results: Select2Result = {
                        errorInfo: info,
                        hasError: true,
                        results: response
                    };

                    query.callback(results);
                }
            });
            serviceCall(params);
        }, quietMillis);
    };
}

/**
 * Produces a query function that works with a local array
 *
 * @param options object containing configuration parameters. The options parameter can either be an array or an
 * object.
 *
 * If the array form is used it is assumed that it contains objects with 'id' and 'text' keys.
 *
 * If the object form is used it is assumed that it contains 'data' and 'text' keys. The 'data' key should contain
 * an array of objects that will be used as choices. These objects must contain at least an 'id' key. The 'text'
 * key can either be a String in which case it is expected that each element in the 'data' array has a key with the
 * value of 'text' which will be used to match choices. Alternatively, text can be a function(item) that can extract
 * the text.
 */
function local(options: any) {
    var data: any = options, // data elements
        dataText: string,
        tmp: any,
        text = function (item: Select2Item) { return "" + item.text; }; // function used to retrieve the text portion of a data item that is matched against the search

    if (Array.isArray(data)) {
        tmp = data;
        data = { results: tmp };
    }

    if (typeof data !== "function") {
        tmp = data;
        data = function () { return tmp; };
    }

    var dataItem = data();
    if (dataItem.text) {
        text = dataItem.text;
        // if text is not a function we assume it to be a key name
        if (typeof text !== "function") {
            dataText = dataItem.text; // we need to store this in a separate variable because in the next step data gets reset and data.text is no longer available
            text = function (item: any) { return item[dataText]; };
        }
    }

    return function (query: Select2QueryOptions) {
        var t = query.term, filtered = { results: [] as any[] }, process: any;
        if (t === "") {
            query.callback(data());
            return;
        }

        process = function (datum: any, collection: any[]) {
            var group: any, attr: string;
            datum = datum[0];
            if (datum.children) {
                group = {};
                for (attr in datum) {
                    if (datum.hasOwnProperty(attr)) group[attr] = datum[attr];
                }
                group.children = [];
                datum.children.forEach(function (childDatum: any) { process(childDatum, group.children); });
                if (group.children.length || query.matcher(t, text(group), datum)) {
                    collection.push(group);
                }
            } else {
                if (query.matcher(t, text(datum), datum)) {
                    collection.push(datum);
                }
            }
        };

        data().results.forEach(function (datum: any) { process(datum, filtered.results); });
        query.callback(filtered);
    };
}

// TODO javadoc
function tags(data: any) {
    var isFunc = typeof data === "function";
    return function (query: Select2QueryOptions) {
        var t = query.term, filtered = { results: [] as any[] };
        var result = isFunc ? data(query) : data;
        if (Array.isArray(result)) {
            result.forEach(function (item: Select2Item) {
                var isObject = item.text !== undefined,
                    text = isObject ? item.text : item;
                if (t === "" || query.matcher(t, text)) {
                    filtered.results.push(isObject ? item : { id: item, text: item });
                }
            });
            query.callback(filtered);
        }
    };
}

/**
 * Checks if the formatter function should be used.
 *
 * Throws an error if it is not a function. Returns true if it should be used,
 * false if no formatting should be performed.
 *
 * @param formatter
 */
function checkFormatter(formatter: any, formatterName: string) {
    if (typeof formatter === "function") return true;
    if (!formatter) return false;
    if (typeof (formatter) === 'string') return true;
    throw new Error(formatterName + " must be a string, function, or falsy value");
}

/**
 * Returns a given value
 * If given a function, returns its output
 *
 * @param val string|function
 * @param context value of "this" to be passed to function
 * @returns {*}
 */
function evaluate(val: any, context: any, ..._: any[]): any {
    if (typeof val === "function") {
        var args = Array.prototype.slice.call(arguments, 2);
        return val.apply(context, args);
    }
    return val;
}

function countResults(results: any[]) {
    var count = 0;
    results.forEach(function (item) {
        if (item.children) {
            count += countResults(item.children);
        } else {
            count++;
        }
    });
    return count;
}

/**
 * Default tokenizer. This function uses breaks the input on substring match of any string from the
 * opts.tokenSeparators array and uses opts.createSearchChoice to create the choice object. Both of those
 * two options have to be defined in order for the tokenizer to work.
 *
 * @param input text user has typed so far or pasted into the search field
 * @param selection currently selected choices
 * @param selectCallback function(choice) callback tho add the choice to selection
 * @param opts select2's opts
 * @return undefined/null to leave the current input unchanged, or a string to change the input to the returned value
 */
function defaultTokenizer(input: string, selection: string | any[], selectCallback: (arg0: any) => void, opts: { createSearchChoice: { call: (arg0: any, arg1: any, arg2: any) => any; }; tokenSeparators: string | any[]; id: (arg0: any) => null; }) {
    var original = input, // store the original so we can compare and know if we need to tell the search to update its text
        dupe = false, // check for whether a token we extracted represents a duplicate selected choice
        token, // token
        index, // position at which the separator was found
        i, l, // looping variables
        separator; // the matched separator

    if (!opts.createSearchChoice || !opts.tokenSeparators || opts.tokenSeparators.length < 1) return undefined;

    while (true) {
        index = -1;

        for (i = 0, l = opts.tokenSeparators.length; i < l; i++) {
            separator = opts.tokenSeparators[i];
            index = input.indexOf(separator);
            if (index >= 0) break;
        }

        if (index < 0) break; // did not find any token separator in the input string, bail

        token = input.substring(0, index);
        input = input.substring(index + separator.length);

        if (token.length > 0) {
            token = opts.createSearchChoice.call(this, token, selection);
            if (token !== undefined && token !== null && opts.id(token) !== undefined && opts.id(token) !== null) {
                dupe = false;
                for (i = 0, l = selection.length; i < l; i++) {
                    if (equal(opts.id(token), opts.id(selection[i]))) {
                        dupe = true; break;
                    }
                }

                if (!dupe) selectCallback(token);
            }
        }
    }

    if (original !== input) return input;
}

function cleanupElements() {
    var self = this;

    Array.from(arguments).forEach(function (element) {
        Fluent.remove(self[element]);
        self[element] = null;
    });
}

const resultsSelector = ".select2-results";

abstract class AbstractSelect2 {

    declare private _enabled: boolean;
    declare private _readonly: boolean;
    declare private _touchEvent: boolean;
    declare private _touchMoved: boolean;

    declare protected autofocus: boolean;
    declare container: HTMLElement;
    declare protected containerId: string;
    declare protected containerEventName: string;
    declare protected context: any;
    declare dropdown: HTMLElement;
    declare protected elementTabIndex: string;
    declare protected enabledInterface: boolean;
    declare protected id: (item: any) => string;
    declare protected nextSearchTerm: string;
    declare protected opts: Select2Options;
    declare protected propertyObserver: MutationObserver;
    declare protected queryCount: number;
    declare protected results: HTMLElement;
    declare protected resultsPage: number;
    declare search: HTMLInputElement;
    declare protected selection: HTMLElement;
    declare protected showSearchInput: boolean;

    protected abstract createContainer(): HTMLElement;
    abstract data(value?: Select2Item | Select2Item[], triggerChange?: boolean): Select2Item | Select2Item[] | void;
    abstract focus(): void;
    abstract isFocused(): boolean;
    protected abstract initContainer(): void;
    protected abstract initSelection(): void;
    protected abstract onSelect(item: Select2Item, options?: { noFocus: boolean }): void;
    protected abstract postprocessResults(data: Select2Result, initial: boolean, noHighlightUpdate?: boolean): void;
    abstract val(value?: string | string[]): string | string[];

    init(opts: Select2Options) {
        var results: HTMLElement, search: HTMLInputElement;

        // prepare options
        this.opts = opts = this.prepareOpts(opts);

        this.id = opts.id;

        // destroy if called on an existing component
        if ((opts.element as any).select2 != null) {
            (opts.element as any).select2.destroy?.();
        }

        this.container = this.createContainer();

        this.containerId = "s2id_" + (opts.element.getAttribute("id") || "autogen" + nextUid());
        this.containerEventName = this.containerId
            .replace(/\\/g, '\\\\')
            .replace(/([.])/g, '_')
            .replace(/([;&,\-\.\+\*\~':"\!\^#$%@\[\]\(\)=>\|])/g, '\\$1');
        this.container.setAttribute("id", this.containerId);

        this.container.setAttribute("title", opts.element.getAttribute("title") ?? "");

        syncCssClasses(this.container, this.opts.element, this.opts.adaptContainerCssClass);

        copyStyle(this.container.style, this.opts.element.style);
        Object.assign(this.container.style, evaluate(opts.containerCss, this.opts.element));
        Fluent.addClass(this.container, evaluate(opts.containerCssClass, this.opts.element));

        this.elementTabIndex = this.opts.element.getAttribute("tabindex");

        // swap container for the element
        (this.opts.element as any).select2 = this;
        this.opts.element.setAttribute("tabindex", "-1");
        Fluent(this.container).insertBefore(this.opts.element);
        Fluent.on(this.opts.element, "click.select2", killEvent); // do not leak click events
        (this.container as any).select2 = this;

        this.dropdown = this.container.querySelector(".select2-drop");

        syncCssClasses(this.dropdown, this.opts.element, this.opts.adaptDropdownCssClass);

        Fluent.addClass(this.dropdown, evaluate(opts.dropdownCssClass, this.opts.element));
        (this.dropdown as any).select2 = this;
        Fluent.on(this.dropdown, "click", killEvent);

        this.results = results = this.container.querySelector(resultsSelector);
        this.search = search = this.container.querySelector("input.select2-input");

        this.queryCount = 0;
        this.resultsPage = 0;
        this.context = null;

        // initialize the container
        this.initContainer();

        Fluent.on(this.container, "click", killEvent);

        installFilteredMouseMove(this.results);

        const boundThis = bindThis(this);

        Fluent.on(this.dropdown, "mousemove-filtered", resultsSelector, boundThis.highlightUnderEvent);

        ["touchstart", "touchmove", "touchend"].forEach(ev => this.dropdown.addEventListener(ev, boundThis.handleDropdownTouchStartMoveEnd, { passive: true }));
        this.dropdown.addEventListener("touchmove", boundThis.handleDropdownTouchMove, { passive: true });
        ["touchstart", "touchend"].forEach(ev => this.dropdown.addEventListener(ev, boundThis.handleDropdownTouchStartEnd), { passive: true });

        // Waiting for a click event on touch devices to select option and hide dropdown
        // otherwise click will be triggered on an underlying element
        Fluent.on(this.dropdown, 'click', boundThis.handleDropdownClick);

        installDebouncedScroll(80, this.results);
        Fluent.on(this.dropdown, "scroll-debounced", resultsSelector, boundThis.loadMoreIfNeeded);

        // do not propagate change event from the search field out of the component
        Fluent.on(this.container, "change", ".select2-input", function (e: Event) { e.stopPropagation(); });
        Fluent.on(this.dropdown, "change", ".select2-input", function (e: Event) { e.stopPropagation(); });

        installKeyUpChangeEvent(search);
        ["keyup-change", "input", "paste"].forEach(ev => Fluent.on(search, ev, boundThis.handleSearchInput));
        Fluent.on(search, "focus", boundThis.handleSearchFocus);
        Fluent.on(search, "blur", boundThis.handleSearchBlur);

        Fluent.on(this.dropdown, "mouseup", resultsSelector, boundThis.handleDropdownMouseUp);

        // trap all mouse events from leaving the dropdown. sometimes there may be a modal that is listening
        // for mouse events outside of itself so it can close itself. since the dropdown is now outside the select2's
        // dom it will trigger the popup close, which is not what we want
        // focusin can cause focus wars between modals and select2 since the dropdown is outside the modal.
        ["click mouseup mousedown touchstart touchend focusin"].forEach(ev => Fluent.on(this.dropdown, ev, handleDropdownTrap));

        this.nextSearchTerm = undefined;

        if (typeof this.opts.initSelection === "function") {
            // initialize selection based on the current value of the source element
            this.initSelection();

            // if the user has provided a function that can set selection based on the value of the source element
            // we monitor the change event on the element and trigger it, allowing for two way synchronization
            this.monitorSource();
        }

        if (opts.maximumInputLength !== null) {
            this.search.setAttribute("maxlength", "" + opts.maximumInputLength);
        }

        var disabled = opts.element.disabled;
        if (disabled === undefined) disabled = false;
        this.enable(!disabled);

        var readonly = (opts.element as any).readOnly;
        if (readonly === undefined) readonly = false;
        this.readonly(readonly);

        // Calculate size of scrollbar
        scrollBarDimensions = scrollBarDimensions || measureScrollbar();

        this.autofocus = (opts.element as any).autofocus;
        (opts.element as any).autofocus = false;
        if (this.autofocus) this.focus();

        this.search.setAttribute("placeholder", opts.searchInputPlaceholder);
    }

    destroy() {
        var element = this.opts.element, select2 = (element as any)?.select2, self = this;

        this.close();

        if (element && (element as any).detachEvent) {
            (element as any).detachEvent("onpropertychange", this.handleMonitorSync);
        }
        if (this.propertyObserver) {
            this.propertyObserver.disconnect();
            this.propertyObserver = null;
        }

        if (select2 !== undefined) {
            select2.container?.remove();
            select2.dropdown?.remove();
            if (element) {
                element.classList.remove("select2-offscreen");
                delete (element as any).select2;
                Fluent.off(element, ".select2");
                element.autofocus = this.autofocus || false;
            }
            if (this.elementTabIndex) {
                element.setAttribute("tabindex", this.elementTabIndex);
            } else {
                element.removeAttribute("tabindex");
            }
            delete element.style.display;
        }

        if (this.search) {
            ["keyup-change", "input", "paste"].forEach(ev => Fluent.off(this.search, ev, this.handleSearchInput));
            Fluent.off(this.search, "focus", this.handleSearchFocus);
            Fluent.off(this.search, "blur", this.handleSearchBlur);
        }

        if (this.dropdown) {
            Fluent.off(this.dropdown, "mousemove-filtered", this.highlightUnderEvent);
            ["click mouseup mousedown touchstart touchend focusin"].forEach(ev => Fluent.off(this.dropdown, ev, handleDropdownTrap));
            Fluent.off(this.dropdown, "mouseup", resultsSelector, this.handleDropdownMouseUp);
        }

        cleanupElements.call(this,
            "container",
            "dropdown",
            "results",
            "search"
        );
    }

    optionToData(element: HTMLOptionElement | HTMLOptGroupElement): Select2Item {
        if (element instanceof HTMLOptionElement) {
            return {
                id: element.value,
                text: element.textContent,
                element: element,
                css: element.getAttribute("class"),
                disabled: element.disabled,
                locked: equal(element.getAttribute("locked"), "locked") || equal(element.getAttribute("locked"), true)
            } as any;
        } else if (element instanceof HTMLOptGroupElement) {
            return {
                text: element.getAttribute("label"),
                children: [],
                element: element,
                css: element.getAttribute("class")
            } as any;
        }
    }

    declare protected select: HTMLSelectElement;

    private handleDropdownMouseUp(e: Event) {
        if ((e.target as HTMLElement).closest(".select2-result-selectable")) {
            this.highlightUnderEvent(e);
            this.selectHighlighted(e);
        }
    }

    private handleDropdownTouchStartMoveEnd(e: Event) {
        if ((e.target as HTMLElement).closest?.(resultsSelector)) {
            this._touchEvent = true;
        }
    }

    private handleDropdownTouchMove(e: Event) {
        (e.target as HTMLElement).closest?.(resultsSelector) && this.touchMoved();
    }

    private handleDropdownTouchStartEnd(e: Event) {
        (e.target as HTMLElement).closest?.(resultsSelector) && this.clearTouchMoved();
    }

    private handleDropdownClick(e: Event) {
        if (this._touchEvent) {
            this._touchEvent = false;
            this.selectHighlighted();
        }
    }

    protected prepareOpts(opts: Select2Options): Select2Options {
        var element: HTMLInputElement | HTMLSelectElement, select: HTMLSelectElement, idKey: string, ajaxUrl: string, self = this;

        element = opts.element;

        if (element.tagName.toLowerCase() === "select") {
            this.select = select = opts.element as HTMLSelectElement;
        }

        if (select) {
            // these options are not allowed when attached to a select because they are picked up off the element itself
            ["id", "multiple", "ajax", "query", "createSearchChoice", "initSelection", "data", "tags"].forEach(function (opt) {
                if (opt in opts && ((opts as any)[opt] != null)) {
                    throw new Error("Option '" + opt + "' is not allowed for Select2 when attached to a <select> element.");
                }
            });
        }

        opts = Object.assign({}, {
            populateResults: function (this: AbstractSelect2, container, results, query) {
                var id = this.opts.id;

                let populate = function (results: Select2Item[], container: HTMLElement, depth: number) {

                    var i, l, result, selectable, disabled, compound, node, label, innerContainer, formatted;

                    results = opts.sortResults(results, container, query);

                    // collect the created nodes for bulk append
                    var nodes: HTMLElement[] = [];
                    for (i = 0, l = results.length; i < l; i = i + 1) {

                        result = results[i];

                        disabled = (result.disabled === true);
                        selectable = (!disabled) && (id(result) !== undefined);

                        compound = result.children && result.children.length > 0;

                        node = document.createElement("li");
                        node.classList.add("select2-results-dept-" + depth);
                        node.classList.add("select2-result");
                        node.classList.add(selectable ? "select2-result-selectable" : "select2-result-unselectable");
                        if (disabled) { node.classList.add("select2-disabled"); }
                        if (compound) { node.classList.add("select2-result-with-children"); }
                        Fluent.addClass(node, self.opts.formatResultCssClass(result));
                        node.setAttribute("role", "presentation");

                        label = document.createElement("div");
                        label.classList.add("select2-result-label");
                        label.setAttribute("id", "select2-result-label-" + nextUid());
                        label.setAttribute("role", "option");

                        formatted = opts.formatResult(result, label, query, self.opts.escapeMarkup);
                        if (formatted !== undefined) {
                            if (formatted instanceof Node) {
                                Fluent.empty(label);
                                label.appendChild(formatted);
                            }
                            else
                                label.textContent = formatted ?? "";
                            node.append(label);
                        }

                        if (compound) {

                            innerContainer = document.createElement("ul");
                            innerContainer.classList.add("select2-result-sub");
                            populate(result.children, innerContainer, depth + 1);
                            node.append(innerContainer);
                        }

                        (node as any).select2data = result;
                        nodes.push(node);
                    }

                    // bulk append the created nodes
                    container.append(...nodes);
                };

                populate(results, container, 0);
            }
        } satisfies Select2Options, Select2.defaults, opts);

        if (typeof (opts.id) !== "function") {
            idKey = opts.id;
            opts.id = function (e) { return e[idKey]; };
        }

        if (opts.element.dataset.select2Tags) {
            if ("tags" in opts) {
                throw new Error("tags specified as both an attribute 'data-select2-tags' and in options of Select2 " + opts.element.getAttribute("id"));
            }
            opts.tags = JSON.parse(opts.element.dataset.select2Tags);
        }

        if (select) {
            opts.query = (query: Select2QueryOptions) => {
                var data: Select2Result = { results: [], more: false },
                    term = query.term,
                    children;

                let process = function (element: HTMLElement, collection: Select2Item[]) {
                    var group: Select2Item;
                    if (element instanceof HTMLOptionElement) {
                        if (query.matcher(term, element.textContent, element)) {
                            collection.push(self.optionToData(element));
                        }
                    } else if (element instanceof HTMLOptGroupElement) {
                        group = self.optionToData(element);
                        Array.from(element.children).forEach(function (elm) { process(elm as HTMLElement, group.children); });
                        if (group.children.length > 0) {
                            collection.push(group);
                        }
                    }
                };

                children = Array.from(element.children);

                // ignore the placeholder option if there is one
                if (this.getPlaceholder() !== undefined && children.length > 0) {
                    var placeholderOption = this.getPlaceholderOption();
                    if (placeholderOption) {
                        children = children.filter(x => x !== placeholderOption);
                    }
                }

                children.forEach(elm => process(elm as HTMLElement, data.results));

                query.callback(data);
            };
            // this is needed because inside val() we construct choices from options and their id is hardcoded
            opts.id = function (e) { return e.id; };
        } else {
            if (!("query" in opts)) {

                if ("ajax" in opts) {
                    ajaxUrl = opts.element.dataset.ajaxUrl;
                    if (ajaxUrl && ajaxUrl.length > 0) {
                        opts.ajax.url = ajaxUrl;
                    }
                    opts.query = ajax.call(opts.element, opts.ajax);
                } else if ("data" in opts) {
                    opts.query = local(opts.data);
                } else if ("tags" in opts) {
                    opts.query = tags(opts.tags);
                    if (opts.createSearchChoice === undefined) {
                        opts.createSearchChoice = function (term) { return { id: term?.trim(), text: term?.trim() }; };
                    }
                    if (opts.initSelection === undefined) {
                        opts.initSelection = function (element, callback) {
                            var data: Select2Item[] = [];
                            splitVal((element as any).value, opts.separator).forEach(function (id) {
                                var obj = { id: id, text: id },
                                    tags = opts.tags;
                                if (typeof tags === "function") tags = tags();
                                tags.forEach(function (tag: any) { if (equal(tag.id, obj.id)) { obj = tag; return false; } });
                                data.push(obj);
                            });

                            callback(data);
                        };
                    }
                }
            }
        }
        if (typeof (opts.query) !== "function") {
            throw new Error("query function not defined for Select2 " + opts.element.getAttribute("id"));
        }

        if (opts.createSearchChoicePosition === 'top') {
            opts.createSearchChoicePosition = function (list, item) { list.unshift(item); };
        }
        else if (opts.createSearchChoicePosition === 'bottom') {
            opts.createSearchChoicePosition = function (list, item) { list.push(item); };
        }
        else if (typeof (opts.createSearchChoicePosition) !== "function") {
            throw new Error("invalid createSearchChoicePosition option must be 'top', 'bottom' or a custom function");
        }

        return opts;
    }

    private handleMonitorFocus(e: Event) {
        this.focus();
    }

    private handleMonitorChange(e: Event) {
        if (this.opts.element.dataset.select2ChangeTriggered !== "true") {
            this.initSelection();
        }
    }

    private handleMonitorSync() {
        // sync enabled state
        const el = this.opts.element;
        var disabled = el.disabled;
        if (disabled === undefined) disabled = false;
        this.enable(!disabled);

        var readonly = (el as any).readOnly;
        if (readonly === undefined) readonly = false;
        this.readonly(readonly);

        syncCssClasses(this.container, this.opts.element, this.opts.adaptContainerCssClass);
        Fluent.addClass(this.container, evaluate(this.opts.containerCssClass, this.opts.element));

        syncCssClasses(this.dropdown, this.opts.element, this.opts.adaptDropdownCssClass);
        Fluent.addClass(this.dropdown, evaluate(this.opts.dropdownCssClass, this.opts.element));
    }

    /**
     * Monitor the original element for changes and update select2 accordingly
     */
    protected monitorSource(): void {
        var el = this.opts.element, observer;

        const boundThis = bindThis(this);
        Fluent.on(el, "change.select2", boundThis.handleMonitorChange);
        Fluent.on(el, "focus.select2", boundThis.handleMonitorFocus);

        // IE8-10 (IE9/10 won't fire propertyChange via attachEventListener)
        if (el && (el as any).attachEvent) {
            (el as any).attachEvent("onpropertychange", boundThis.handleMonitorSync);
        }

        // safari, chrome, firefox, IE11
        observer = window.MutationObserver;
        if (observer !== undefined) {
            if (this.propertyObserver) { delete this.propertyObserver; this.propertyObserver = null; }
            this.propertyObserver = new observer(function (mutations) {
                mutations.forEach(boundThis.handleMonitorSync);
            });
            this.propertyObserver.observe(el, { attributes: true, subtree: false });
        }
    }

    protected triggerSelect(data: Select2Item): boolean {
        var evt = { val: this.id(data), object: data, choice: data };
        var event = Fluent.trigger(this.opts.element, "select2-selecting", evt);
        return !Fluent.isDefaultPrevented(event);
    }

    /**
     * Triggers the change event on the source element
     */
    protected triggerChange(details?: any): void {

        details = details || {};
        details = Object.assign({}, details, { val: this.val() });
        // prevents recursive triggering
        this.opts.element.dataset.select2ChangeTriggered = "true";
        Fluent.trigger(this.opts.element, "change", details);
        delete this.opts.element.dataset.select2ChangeTriggered;

        // some validation frameworks ignore the change event and listen instead to keyup, click for selects
        // so here we trigger the click event manually
        this.opts.element.click();

        // ValidationEngine ignores the change event and listens instead to blur
        // so here we trigger the blur event manually if so desired
        if (this.opts.blurOnChange)
            this.opts.element.blur();
    }

    protected isInterfaceEnabled(): boolean {
        return this.enabledInterface === true;
    }

    protected enableInterface(): boolean {
        var enabled = this._enabled && !this._readonly,
            disabled = !enabled;

        if (enabled === this.enabledInterface) return false;

        this.container.classList.toggle("select2-container-disabled", disabled);
        this.close();
        this.enabledInterface = enabled;

        return true;
    }

    enable(enabled?: boolean): void {
        if (enabled === undefined) enabled = true;
        if (this._enabled === enabled) return;
        this._enabled = enabled;

        this.opts.element.disabled = !enabled;
        this.enableInterface();
    }

    disable() {
        this.enable(false);
    }

    readonly(enabled?: boolean): void {
        if (enabled === undefined) enabled = false;
        if (this._readonly === enabled) return;
        this._readonly = enabled;

        (this.opts.element as any).readOnly = enabled;
        this.enableInterface();
    }

    opened(): boolean {
        return (this.container) ? this.container.classList.contains("select2-dropdown-open") : false;
    }

    positionDropdown() {
        var dropdown = this.dropdown,
            offset = getOffset(this.container),
            height = this.container.offsetHeight,
            width = this.container.offsetWidth,
            dropHeight = dropdown.offsetHeight,
            windowWidth = window.innerWidth,
            windowHeight = window.innerHeight,
            viewPortRight = window.scrollX + windowWidth,
            viewportBottom = window.scrollY + windowHeight,
            dropTop = offset.top + height,
            dropLeft = offset.left,
            enoughRoomBelow = dropTop + dropHeight <= viewportBottom,
            enoughRoomAbove = (offset.top - dropHeight) >= window.scrollY,
            dropWidth = dropdown.offsetWidth,
            enoughRoomOnRight = dropLeft + dropWidth <= viewPortRight,
            aboveNow = dropdown.classList.contains("select2-drop-above"),
            bodyOffset,
            above,
            changeDirection,
            resultsListNode;

        // always prefer the current above/below alignment, unless there is not enough room
        if (aboveNow) {
            above = true;
            if (!enoughRoomAbove && enoughRoomBelow) {
                changeDirection = true;
                above = false;
            }
        } else {
            above = false;
            if (!enoughRoomBelow && enoughRoomAbove) {
                changeDirection = true;
                above = true;
            }
        }

        //if we are changing direction we need to get positions when dropdown is hidden;
        if (changeDirection) {
            Fluent.toggleClass(dropdown, "select2-display-none", true);
            offset = getOffset(this.container);
            height = this.container.offsetHeight;
            width = this.container.offsetWidth;
            viewPortRight = window.scrollX + windowWidth;
            viewportBottom = window.scrollY + windowHeight;
            dropTop = offset.top + height;
            dropLeft = offset.left;
            enoughRoomOnRight = dropLeft + dropWidth <= viewPortRight;
            Fluent.toggleClass(dropdown, "select2-display-none", false);

            // fix so the cursor does not move to the left within the search-textbox in IE
            this.focusSearch();
        }

        if (this.opts.dropdownAutoWidth) {
            resultsListNode = dropdown.querySelector('.select2-results');
            dropdown.classList.add('select2-drop-auto-width');
            dropdown.style.width = "";
            // Add scrollbar width to dropdown if vertical scrollbar is present
            dropWidth = dropdown.offsetWidth + (resultsListNode.scrollHeight === resultsListNode.clientHeight ? 0 : scrollBarDimensions.width);
            dropWidth > width ? width = dropWidth : dropWidth = width;
            dropHeight = dropdown.offsetHeight;
            enoughRoomOnRight = dropLeft + dropWidth <= viewPortRight;
        }
        else {
            this.container.classList.remove('select2-drop-auto-width');
        }

        //console.log("below/ droptop:", dropTop, "dropHeight", dropHeight, "sum", (dropTop+dropHeight)+" viewport bottom", viewportBottom, "enough?", enoughRoomBelow);
        //console.log("above/ offset.top", offset.top, "dropHeight", dropHeight, "top", (offset.top-dropHeight), "scrollTop", this.body.scrollTop(), "enough?", enoughRoomAbove);

        // fix positioning when body has an offset and is not position: static
        if (getComputedStyle(document.body).position !== 'static') {
            bodyOffset = getOffset(document.body);
            dropTop -= bodyOffset.top;
            dropLeft -= bodyOffset.left;
        }

        if (!enoughRoomOnRight) {
            dropLeft = offset.left + this.container.offsetWidth - dropWidth;
        }

        let css: Record<string, any> = {
            left: dropLeft + "px",
            width: width + "px"
        };

        if (above) {
            css.top = (offset.top - dropHeight) + "px";
            css.bottom = 'auto';
            this.container.classList.add("select2-drop-above");
            dropdown.classList.add("select2-drop-above");
        }
        else {
            css.top = dropTop + "px";
            css.bottom = 'auto';
            this.container.classList.remove("select2-drop-above");
            dropdown.classList.remove("select2-drop-above");
        }
        css = Object.assign(css, evaluate(this.opts.dropdownCss, this.opts.element));

        Object.assign(dropdown.style, css);
    }

    protected shouldOpen(): boolean {

        if (this.opened()) return false;

        if (this._enabled === false || this._readonly === true) return false;

        var event = Fluent.trigger(this.opts.element, "select2-opening");
        return !Fluent.isDefaultPrevented(event);
    }

    protected clearDropdownAlignmentPreference() {
        // clear the classes used to figure out the preference of where the dropdown should be opened
        this.container.classList.remove("select2-drop-above");
        this.dropdown.classList.remove("select2-drop-above");
    }

    /**
     * Opens the dropdown
     *
     * @return {Boolean} whether or not dropdown was opened. This method will return false if, for example,
     * the dropdown is already open, or if the 'open' event listener on the element called preventDefault().
     */
    open(): boolean {

        if (!this.shouldOpen()) return false;

        this.opening();

        // Only bind the document mousemove when the dropdown is visible
        Fluent.on(document, "mousemove.select2Event", function (e: MouseEvent) {
            lastMousePosition.x = e.pageX;
            lastMousePosition.y = e.pageY;
        });

        return true;
    }

    /**
     * Performs the opening of the dropdown
     */
    protected opening() {
        var cid = this.containerEventName,
            scroll = "scroll." + cid,
            resize = "resize." + cid,
            orient = "orientationchange." + cid,
            mask: HTMLElement;

        this.container.classList.add("select2-dropdown-open", "select2-container-active");

        this.clearDropdownAlignmentPreference();

        var dropdownParent = (typeof this.opts.dropdownParent === "function" ?
            this.opts.dropdownParent(this.opts.element) : null) ?? document?.body;

        if (dropdownParent && this.dropdown !== dropdownParent.lastElementChild) {
            dropdownParent.appendChild(this.dropdown);
        }

        // create the dropdown mask if doesn't already exist
        mask = document.getElementById("select2-drop-mask");
        if (!mask) {
            mask = document.createElement("div");
            mask.setAttribute("id", "select2-drop-mask");
            mask.setAttribute("class", "select2-drop-mask");
            mask.hidden = true;
            document.body.appendChild(mask);
            ["mousedown", "touchstart", "click"].forEach(ev => mask.addEventListener(ev, function (e) {
                // Prevent IE from generating a click event on the body
                reinsertElement(mask);

                var dropdown = document.getElementById("select2-drop"), self;
                if (dropdown) {
                    self = (dropdown as any).select2;
                    if (self.opts.selectOnBlur) {
                        self.selectHighlighted({ noFocus: true });
                    }
                    self.close();
                    e.type !== "touchstart" && e.preventDefault();
                    e.stopPropagation();
                }
            }, { passive: ev === "touchstart" }));
        }

        // ensure the mask is always right before the dropdown
        if (this.dropdown.previousElementSibling != mask) {
            Fluent(mask).insertBefore(this.dropdown);
        }

        // move the global id to the correct dropdown
        document.getElementById("select2-drop")?.removeAttribute("id");
        this.dropdown.setAttribute("id", "select2-drop");

        // show the elements
        Fluent.toggle(mask, true);

        this.positionDropdown();
        Fluent.toggleClass(this.dropdown, "select2-display-none", false);
        this.positionDropdown();

        this.dropdown.classList.add("select2-drop-active");

        // attach listeners to events that can change the position of the container and thus require
        // the position of the dropdown to be updated as well so it does not come unglued from the container
        var that = this;

        var parent = this.container.parentElement;
        var parents = [];
        while (parent) {
            parents.push(parent);
            parent = parent.parentElement;
        }
        parents.push(window);

        parents.forEach(parent => {
            [resize, scroll, orient].forEach(ev => {
                Fluent.on(parent, ev, function () {
                    if (that.opened()) that.positionDropdown();
                });
            });
        });
    }

    close(): void {
        if (!this.opened()) return;

        var cid = this.containerEventName,
            scroll = "scroll." + cid,
            resize = "resize." + cid,
            orient = "orientationchange." + cid;

        var parent = this.container.parentElement;
        var parents = [];
        while (parent) {
            parents.push(parent);
            parent = parent.parentElement;
        }
        parents.push(window);
        parents.forEach(parent => {
            Fluent.off(parent, scroll);
            Fluent.off(parent, resize);
            Fluent.off(parent, orient);
        });

        this.clearDropdownAlignmentPreference();

        Fluent(document.getElementById("select2-drop-mask")).hide();
        this.dropdown.removeAttribute("id"); // only the active dropdown has the select2-drop id
        Fluent.toggleClass(this.dropdown, "select2-display-none", true);
        this.container.classList.remove("select2-dropdown-open", "select2-container-active");
        Fluent.empty(this.results);

        // Now that the dropdown is closed, unbind the global document mousemove event
        Fluent.off(document, "mousemove.select2Event");

        this.clearSearch();
        this.search.classList.remove("select2-active");
        this.search.parentElement?.classList.remove("select2-active");
        Fluent.trigger(this.opts.element, "select2-close");
    }

    /**
     * Opens control, sets input value, and updates results.
     */
    protected externalSearch(term: string) {
        this.open();
        this.search.value = term;
        this.updateResults(false);
    }

    protected clearSearch() {

    }

    protected getMaximumSelectionSize() {
        return evaluate(this.opts.maximumSelectionSize, this.opts.element);
    }

    protected ensureHighlightVisible() {
        var results = this.results, index, child, hb, rb, y, more, topOffset;

        index = this.highlight();

        if (index < 0) return;

        if (index == 0) {

            // if the first element is highlighted scroll all the way to the top,
            // that way any unselectable headers above it will also be scrolled
            // into view

            results.scrollTop = 0;
            return;
        }

        var children = this.findHighlightableChoices().map(x => x.querySelector('.select2-result-label'));

        child = children[index] as HTMLElement;
        if (!child)
            return;

        topOffset = (getOffset(child) || {}).top || 0;

        hb = topOffset + getOuterHeightWithMargins(child);

        // if this is the last child lets also make sure select2-more-results is visible
        if (index === children.length - 1) {
            more = results.querySelector("li.select2-more-results");
            if (more) {
                hb = getOffset(more).top + getOuterHeightWithMargins(more);
            }
        }

        rb = getOffset(results).top + getOuterHeightWithMargins(results);
        if (hb > rb) {
            results.scrollTop = results.scrollTop + (hb - rb);
        }
        y = topOffset - getOffset(results).top;

        // make sure the top of the element is visible
        if (y < 0 && getComputedStyle(child).display != 'none') {
            results.scrollTop = results.scrollTop + y; // y is negative
        }
    }

    protected findHighlightableChoices() {
        return Array.from(this.results.querySelectorAll(".select2-result-selectable:not(.select2-disabled):not(.select2-selected)"));
    }

    protected moveHighlight(delta: number) {
        var choices = this.findHighlightableChoices(),
            index = this.highlight();

        while (index > -1 && index < choices.length) {
            index += delta;
            var choice = choices[index];
            if (choice && choice.classList.contains("select2-result-selectable") && !choice.classList.contains("select2-disabled") && !choice.classList.contains("select2-selected")) {
                this.highlight(index);
                break;
            }
        }
    }

    protected highlight(index?: number) {
        var choices = this.findHighlightableChoices(),
            choice,
            data;

        if (arguments.length === 0) {
            return choices.findIndex(x => x.classList.contains("select2-highlighted"));
        }

        if (index >= choices.length) index = choices.length - 1;
        if (index < 0) index = 0;

        this.removeHighlight();

        choice = choices[index];
        choice?.classList.add("select2-highlighted");

        // ensure assistive technology can determine the active choice
        this.search.setAttribute("aria-activedescendant", choice?.querySelector(".select2-result-label")?.getAttribute("id"));

        this.ensureHighlightVisible();

        data = (choice as any)?.select2data;
        if (data) {
            Fluent.trigger(this.opts.element, "select2-highlight", { val: this.id(data), choice: data });
        }
    }

    protected removeHighlight() {
        this.results.querySelectorAll(".select2-highlighted").forEach(x => x.classList.remove("select2-highlighted"));
    }

    protected touchMoved() {
        this._touchMoved = true;
    }

    protected clearTouchMoved() {
        this._touchMoved = false;
    }

    protected countSelectableResults() {
        return this.findHighlightableChoices().length;
    }

    protected highlightUnderEvent(event: Event) {
        var el = (event.target as any)?.closest?.(".select2-result-selectable") as HTMLElement;
        if (el && !el.classList.contains("select2-highlighted")) {
            var choices = this.findHighlightableChoices();
            this.highlight(choices.indexOf(el));
        } else if (!el) {
            // if we are over an unselectable item remove all highlights
            this.removeHighlight();
        }
    }

    protected loadMoreIfNeeded() {
        var results = this.results,
            more = results.querySelector<HTMLElement>("li.select2-more-results"),
            below, // pixels the element is below the scroll fold, below==0 is when the element is starting to be visible
            page = this.resultsPage + 1,
            self = this,
            term = this.search.value,
            context = this.context;

        if (!more) return;
        below = getOffset(more).top - getOffset(results).top - results.getBoundingClientRect().height;

        if (below <= this.opts.loadMorePadding) {
            more.classList.add("select2-active");
            this.opts.query({
                element: this.opts.element,
                term: term,
                page: page,
                context: context,
                matcher: this.opts.matcher,
                callback: data => {

                    // ignore a response if the select2 has been closed before it was received
                    if (!self.opened()) return;

                    this.dropdown?.classList.add('select2-position-fixed');
                    try {
                        self.opts.populateResults.call(this, results, data.results, { term: term, page: page, context: context });
                        self.postprocessResults(data, false, false);

                        if (data.more === true) {
                            results.appendChild(more);
                            var loadMore = evaluate(self.opts.formatLoadMore, self.opts.element, page + 1);
                            Fluent.empty(more);
                            if (loadMore instanceof Node)
                                more.appendChild(loadMore);
                            else
                                more.textContent = loadMore ?? "";
                            window.setTimeout(function () { self.loadMoreIfNeeded(); }, 10);
                        } else {
                            more.remove();
                        }
                        self.positionDropdown();
                    }
                    finally {
                        this.dropdown?.classList.remove('select2-position-fixed');
                    }
                    self.resultsPage = page;
                    self.context = data.context;
                    Fluent.trigger(this.opts.element, "select2-loaded", { items: data });
                }
            });
        }
    }

    /**
     * Default tokenizer function which does nothing
     */
    protected tokenize(): string {
        return undefined;
    }

    protected handleSearchInput() {
        this.updateResults();
    }

    protected handleSearchFocus() {
        this.search?.classList.add("select2-focused");
    }

    protected handleSearchBlur() {
        this.search?.classList.remove("select2-focused");
    }

    /**
     * @param initial whether or not this is the call to this method right after the dropdown has been opened
     */
    protected updateResults(initial?: boolean) {
        var search = this.search,
            results = this.results,
            opts = this.opts,
            data,
            self = this,
            input,
            term = search.value,
            lastTerm = this.container.dataset.select2LastTerm,
            // sequence number used to drop out-of-order responses
            queryNumber: number;

        // prevent duplicate queries against the same term
        if (initial !== true && lastTerm && equal(term, lastTerm)) return;

        this.container.dataset.select2LastTerm = term;

        // if the search is currently hidden we do not alter the results
        if (initial !== true && (this.showSearchInput === false || !this.opened())) {
            return;
        }

        function postRender() {
            search.classList.remove("select2-active");
            search.parentElement?.classList.remove("select2-active");
            self.positionDropdown();
        }

        function createLi(klass: string, html?: Select2FormatResult) {
            if (klass == null && html === undefined)
                return;
            var li = document.createElement("li");
            li.classList.add(klass);
            if (html instanceof Node)
                li.appendChild(html);
            else
                li.textContent = html ?? "";
            return li;
        }


        function render(klass: string, html?: Select2FormatResult) {
            self.dropdown?.classList.add("select2-position-fixed");
            try {
                Fluent.empty(results);
                var li = createLi(klass, html);
                if (li != null)
                    results.appendChild(li);
                postRender();
            } finally {
                self.dropdown?.classList.remove("select2-position-fixed");
            }
        }

        queryNumber = ++this.queryCount;

        var maxSelSize = this.getMaximumSelectionSize();
        if (maxSelSize >= 1) {
            data = this.data();
            if (Array.isArray(data) && data.length >= maxSelSize && checkFormatter(opts.formatSelectionTooBig, "formatSelectionTooBig")) {
                render("select2-selection-limit", evaluate(opts.formatSelectionTooBig, opts.element, maxSelSize));
                return;
            }
        }

        if (search.value?.length < opts.minimumInputLength) {
            if (checkFormatter(opts.formatInputTooShort, "formatInputTooShort")) {
                render("select2-no-results", evaluate(opts.formatInputTooShort, opts.element, search.value, opts.minimumInputLength));
            } else {
                render("");
            }
            if (initial && (this as any).showSearch) (this as any).showSearch(true);
            return;
        }

        if (opts.maximumInputLength && search.value?.length > opts.maximumInputLength) {
            if (checkFormatter(opts.formatInputTooLong, "formatInputTooLong")) {
                render("select2-no-results", evaluate(opts.formatInputTooLong, opts.element, search.value, opts.maximumInputLength));
            } else {
                render("");
            }
            return;
        }

        if (opts.formatSearching && this.findHighlightableChoices().length === 0) {
            render("select2-searching", evaluate(opts.formatSearching, opts.element));
        }

        search.classList.add("select2-active");
        search.parentElement?.classList.add("select2-active");

        this.removeHighlight();

        // give the tokenizer a chance to pre-process the input
        input = this.tokenize();
        if (input != undefined && input != null) {
            search.value = input ?? "";
        }

        this.resultsPage = 1;

        opts.query({
            element: opts.element,
            term: search.value,
            page: this.resultsPage,
            context: null,
            matcher: opts.matcher,
            callback: (data: Select2Result) => {
                var def: Select2Item; // default choice

                // ignore old responses
                if (queryNumber != this.queryCount) {
                    return;
                }

                // ignore a response if the select2 has been closed before it was received
                if (!this.opened()) {
                    this.search.classList.remove("select2-active");
                    return;
                }

                // handle ajax error
                if (data.hasError !== undefined && checkFormatter(opts.formatAjaxError, "formatAjaxError")) {
                    render("select2-ajax-error", evaluate(opts.formatAjaxError, opts.element, data));
                    return;
                }

                // save context, if any
                this.context = (data.context === undefined) ? null : data.context;
                // create a default choice and prepend it to the list
                if (this.opts.createSearchChoice && search.value !== "") {
                    def = this.opts.createSearchChoice.call(self, search.value, data.results);
                    if (def !== undefined && def !== null && self.id(def) !== undefined && self.id(def) !== null) {
                        if (data.results.filter(
                            function () {
                                return equal(self.id(this), self.id(def));
                            }).length === 0) {
                            (this.opts.createSearchChoicePosition as any)(data.results, def);
                        }
                    }
                }

                if (data.results.length === 0 && checkFormatter(opts.formatNoMatches, "formatNoMatches")) {
                    render("select2-no-results", evaluate(opts.formatNoMatches, opts.element, search.value));
                    return;
                }

                this.dropdown?.classList.add("select2-position-fixed");
                try {
                    Fluent.empty(results);
                    self.opts.populateResults.call(this, results, data.results, { term: search.value, page: this.resultsPage, context: null });

                    if (data.more === true && checkFormatter(opts.formatLoadMore, "formatLoadMore")) {
                        results.appendChild(createLi("select2-more-results", evaluate(opts.formatLoadMore, opts.element, this.resultsPage)));
                        window.setTimeout(function () { self.loadMoreIfNeeded(); }, 10);
                    }

                    this.postprocessResults(data, initial);

                    postRender();
                }
                finally {
                    this.dropdown?.classList.remove("select2-position-fixed");
                }

                Fluent.trigger(this.opts.element, "select2-loaded", { items: data });
            }
        });
    }

    protected cancel(e?: Event) {
        this.close();
    }

    protected blur() {
        // if selectOnBlur == true, select the currently highlighted option
        if (this.opts.selectOnBlur)
            this.selectHighlighted({ noFocus: true });

        this.close();
        this.container.classList.remove("select2-container-active");
        if (this.search === document.activeElement) { this.search.blur(); }
        this.clearSearch();
        this.selection.querySelectorAll(".select2-search-choice-focus").forEach(x => x.classList.remove("select2-search-choice-focus"));
    }

    protected focusSearch() {
        focus(this.search);
    }

    protected selectHighlighted(options?: any) {
        if (this._touchMoved) {
            this.clearTouchMoved();
            return;
        }
        var index = this.highlight(),
            highlighted = this.results.querySelector(".select2-highlighted"),
            data = (highlighted?.closest('.select2-result') as any)?.select2data;

        if (data) {
            this.highlight(index);
            this.onSelect(data, options);
        } else if (options && options.noFocus) {
            this.close();
        }
    }

    protected getPlaceholder() {
        var placeholderOption;
        return this.opts.element.getAttribute("placeholder") ||
            this.opts.element.dataset.placeholder ||
            this.opts.placeholder ||
            ((placeholderOption = this.getPlaceholderOption()) !== undefined ? placeholderOption?.textContent : undefined);
    }

    protected getPlaceholderOption(): HTMLOptionElement {
        if (this.select) {
            var firstOption = this.select.querySelector<HTMLOptionElement>(':scope > option');
            if (this.opts.placeholderOption !== undefined) {
                //Determine the placeholder option based on the specified placeholderOption setting
                return (this.opts.placeholderOption === "first" && firstOption) ||
                    (typeof this.opts.placeholderOption === "function" && this.opts.placeholderOption(this.select));
            } else if (firstOption.textContent?.trim() === "" && firstOption.value === "") {
                //No explicit placeholder option specified, use the first if it's blank
                return firstOption;
            }
        }
    }

    /**
     * Get the desired width for the container element.  This is
     * derived first from option `width` passed to select2, then
     * the inline 'style' on the original element, and finally
     * falls back to the jQuery calculated element width.
     */
    protected initContainerWidth() {
        function resolveContainerWidth(this: AbstractSelect2) {
            var style, attrs, matches, i, l, attr;

            if (this.opts.width === "off") {
                return null;
            } else if (this.opts.width === "element") {
                return this.opts.element.offsetWidth === 0 ? 'auto' : this.opts.element.offsetWidth + 'px';
            } else if (this.opts.width === "copy" || this.opts.width === "resolve") {
                // check if there is inline style on the element that contains width
                style = this.opts.element.getAttribute('style');
                if (style) {
                    attrs = style.split(';');
                    for (i = 0, l = attrs.length; i < l; i = i + 1) {
                        attr = attrs[i].replace(/\s/g, '');
                        matches = attr.match(/^width:(([-+]?([0-9]*\.)?[0-9]+)(px|em|ex|%|in|cm|mm|pt|pc))/i);
                        if (matches !== null && matches.length >= 1)
                            return matches[1];
                    }
                }

                if (this.opts.width === "resolve") {
                    // next check if css('width') can resolve a width that is percent based, this is sometimes possible
                    // when attached to input type=hidden or elements hidden via css
                    style = getComputedStyle(this.opts.element).width;
                    if (style.indexOf("%") > 0) return style;

                    // finally, fallback on the calculated width of the element
                    return (this.opts.element.offsetWidth === 0 ? 'auto' : this.opts.element.offsetWidth + 'px');
                }

                return null;
            } else if (typeof this.opts.width === "function") {
                return this.opts.width();
            } else {
                return this.opts.width;
            }
        };

        var width = resolveContainerWidth.call(this);
        if (width !== null) {
            this.container.style.width = width;
        }
    }
}

class SingleSelect2 extends AbstractSelect2 {

    declare protected focusser: HTMLInputElement;

    createContainer(): HTMLElement {
        return (
            <div class="select2-container">
                <a role="button" class="select2-choice" tabindex="-1">
                    <span class="select2-chosen">&#160;</span>
                    <abbr class="select2-search-choice-close"></abbr>
                    <span class="select2-arrow" role="presentation"><b role="presentation"></b></span>
                </a>
                <label for="" class="select2-offscreen"></label>
                <input class="select2-focusser select2-offscreen" type="text" aria-haspopup="true" role="button" />
                <div class="select2-drop select2-display-none">
                    <div class="select2-search">
                        <label for="" class="select2-offscreen"></label>
                        <input type="text" autocomplete="off" autocorrect="off" autocapitalize="off" spellcheck="false" class="select2-input" role="combobox" aria-expanded="true" aria-autocomplete="list" />
                    </div>
                    <ul class="select2-results" role="listbox"></ul>
                </div>
            </div>
        ) as HTMLElement;
    }

    protected override enableInterface() {
        var result = super.enableInterface();
        if (result) {
            this.focusser.disabled = !this.isInterfaceEnabled();
        }
        return result;
    }

    protected override opening() {
        var el, len;

        if (this.opts.minimumResultsForSearch >= 0) {
            this.showSearch(true);
        }

        super.opening();

        if (this.showSearchInput !== false) {
            this.search.value = this.focusser.value;
        }
        if (this.opts.shouldFocusInput(this)) {
            this.search.focus();
            // move the cursor to the end after focussing, otherwise it will be at the beginning and
            // new text will appear *before* focusser.val()
            el = this.search;
            if (el.setSelectionRange) {
                len = this.search.value.length;
                el.setSelectionRange(len, len);
            }
        }

        // initializes search's value with nextSearchTerm (if defined by user)
        // ignore nextSearchTerm if the dropdown is opened by the user pressing a letter
        if (this.search.value === "") {
            if (this.nextSearchTerm != undefined) {
                this.search.value = this.nextSearchTerm ?? "";
                this.search.select();
            }
        }

        this.focusser.disabled = true;
        this.focusser.value = "";
        this.updateResults(true);
        Fluent.trigger(this.opts.element, "select2-open");
    }

    override close() {
        if (!this.opened()) return;
        super.close();

        this.focusser.disabled = false;

        if (this.opts.shouldFocusInput(this)) {
            this.focusser.focus();
        }
    }

    override focus() {
        if (this.opened()) {
            this.close();
        } else {
            this.focusser.disabled = false;
            if (this.opts.shouldFocusInput(this)) {
                this.focusser.focus();
            }
        }
    }

    override isFocused(): boolean {
        return this.container.classList.contains("select2-container-active");
    }

    protected override cancel(e?: Event) {
        super.cancel(e);
        this.focusser.disabled = false;

        if (this.opts.shouldFocusInput(this)) {
            this.focusser.focus();
        }
    }

    override destroy() {
        document.querySelector("label[for='" + this.focusser.getAttribute('id') + "']")?.setAttribute(
            'for', this.opts.element.getAttribute("id"));
        super.destroy();

        cleanupElements.call(this,
            "selection",
            "focusser"
        );
    }

    override initContainer() {

        var selection: HTMLElement,
            container = this.container,
            dropdown = this.dropdown,
            idSuffix = nextUid(),
            elementLabel;

        if (this.opts.minimumResultsForSearch < 0) {
            this.showSearch(false);
        } else {
            this.showSearch(true);
        }

        this.selection = selection = container.querySelector(".select2-choice");

        this.focusser = container.querySelector(".select2-focusser");

        // add aria associations
        selection.querySelector(".select2-chosen")?.setAttribute("id", "select2-chosen-" + idSuffix);
        this.focusser.setAttribute("aria-labelledby", "select2-chosen-" + idSuffix);
        this.results.setAttribute("id", "select2-results-" + idSuffix);
        this.search.setAttribute("aria-owns", "select2-results-" + idSuffix);

        // rewrite labels from original element to focusser
        this.focusser.setAttribute("id", "s2id_autogen" + idSuffix);

        elementLabel = document.querySelector("label[for='" + this.opts.element.getAttribute("id") + "']");

        this.focusser.previousElementSibling.textContent = elementLabel?.textContent ?? "";
        this.focusser.previousElementSibling.setAttribute('for', this.focusser.getAttribute('id'));

        // Ensure the original element retains an accessible name
        var originalTitle = this.opts.element.getAttribute("title");
        this.opts.element.setAttribute("title", (originalTitle || (elementLabel?.textContent ?? "")));

        this.focusser.setAttribute("tabindex", this.elementTabIndex);

        // write label for search field using the label from the focusser element
        this.search.setAttribute("id", this.focusser.getAttribute('id') + '_search');

        this.search.previousElementSibling.textContent = document.querySelector("label[for='" + this.focusser.getAttribute('id') + "']")?.textContent ?? "";
        this.search.previousElementSibling.setAttribute('for', this.search.getAttribute('id'));

        Fluent.on(this.search, "keydown", (e) => {
            if (!this.isInterfaceEnabled()) return;

            // skip events while an input method editor is composing text
            if (e.isComposing) return;

            if (e.key === "PageUp" || e.key === "PageDown") {
                // prevent the page from scrolling
                killEvent(e);
                return;
            }

            switch (e.key) {
                case "ArrowUp":
                case "ArrowDown":
                    this.moveHighlight((e.key === "ArrowUp") ? -1 : 1);
                    killEvent(e);
                    return;
                case "Enter":
                    this.selectHighlighted();
                    killEvent(e);
                    return;
                case "Tab":
                    this.selectHighlighted({ noFocus: true });
                    return;
                case "Escape":
                    this.cancel(e);
                    killEvent(e);
                    return;
            }
        });

        Fluent.on(this.search, "blur", e => {
            // a workaround for chrome to keep the search field focussed when the scroll bar is used to scroll the dropdown.
            // without this the search field loses focus which is annoying
            if (document.activeElement === document.body) {
                window.setTimeout(() => {
                    if (this.opened()) {
                        this.search.focus();
                    }
                }, 0);
            }
        });

        Fluent.on(this.focusser, "keydown", e => {
            if (!this.isInterfaceEnabled()) return;

            if (e.key === "Tab" || e.key === "Shift" || e.key === "Control" || e.key === "Alt" || e.metaKey
                || /^F(?:1[0-2]|[1-9])$/.test(e.key) || e.key === "Escape") {
                return;
            }

            if (this.opts.openOnEnter === false && e.key === "Enter") {
                killEvent(e);
                return;
            }

            if (e.key == "ArrowDown" || e.key == "ArrowUp"
                || (e.key == "Enter" && this.opts.openOnEnter)) {

                if (e.altKey || e.ctrlKey || e.shiftKey || e.metaKey) return;

                this.open();
                killEvent(e);
                return;
            }

            if (e.key == "Delete" || e.key == "Backspace") {
                if (this.opts.allowClear) {
                    this.clear();
                }
                killEvent(e);
                return;
            }
        });


        installKeyUpChangeEvent(this.focusser);
        ["keyup-change", "input"].forEach(ev => Fluent.on(this.focusser, ev, e => {
            if (this.opts.minimumResultsForSearch >= 0) {
                e.stopPropagation();
                if (this.opened()) return;
                this.open();
            }
        }));

        Fluent.on(selection, "click", "abbr", (e: Event) => {
            if (!this.isInterfaceEnabled()) return;
            this.clear();
            killEventImmediately(e);
            this.close();
            this.selection.focus();
        });

        Fluent.on(selection, "dragstart", e => { e.preventDefault(); return false });

        Fluent.on(selection, "click", e => {
            if (!this.container.classList.contains("select2-container-active")) {
                Fluent.trigger(this.opts.element, "select2-focus");
            }

            if (this.opened()) {
                this.close();
            } else if (this.isInterfaceEnabled()) {
                this.open();
            }

            killEvent(e);
        });

        ["mousedown", "touchstart"].forEach(ev => dropdown.addEventListener(ev, () => {
            if (this.opts.shouldFocusInput(this)) {
                this.search.focus();
            }
        }, { passive: true }));

        Fluent.on(selection, "focus", e => {
            killEvent(e);
        });

        Fluent.on(this.focusser, "focus", () => {
            if (!this.container.classList.contains("select2-container-active")) {
                Fluent.trigger(this.opts.element, "select2-focus");
            }
            this.container.classList.add("select2-container-active");
        });

        Fluent.on(this.focusser, "blur", () => {
            if (!this.opened()) {
                this.container.classList.remove("select2-container-active");
                Fluent.trigger(this.opts.element, "select2-blur");
            }
        });

        Fluent.on(this.search, "focus", () => {
            if (!this.container.classList.contains("select2-container-active")) {
                Fluent.trigger(this.opts.element, "select2-focus");
            }
            this.container.classList.add("select2-container-active");
        });

        this.initContainerWidth();
        this.opts.element.classList.add("select2-offscreen");
        this.setPlaceholder();
    }

    protected clear(triggerChange?: boolean) {
        var data = (this.selection as any)?.select2data;
        if (data) { // guard against queued quick consecutive clicks
            var evt = Fluent.trigger(this.opts.element, "select2-clearing");
            if (Fluent.isDefaultPrevented(evt)) {
                return;
            }
            var placeholderOption = this.getPlaceholderOption();
            this.opts.element.value = placeholderOption ? placeholderOption.value : "";
            Fluent.empty(this.selection.querySelector(".select2-chosen"));
            delete (this.selection as any).select2data;
            this.setPlaceholder();

            if (triggerChange !== false) {
                Fluent.trigger(this.opts.element, "select2-removed", { val: this.id(data), choice: data });
                this.triggerChange({ removed: data });
            }
        }
    }

    /**
     * Sets selection based on source element's value
     */
    protected initSelection() {
        if (this.isPlaceholderOptionSelected()) {
            this.updateSelection(null);
            this.close();
            this.setPlaceholder();
        } else {
            var self = this;
            this.opts.initSelection.call(null, this.opts.element, function (selected: Select2Item) {
                if (!self.container)
                    return;
                if (selected !== undefined && selected !== null) {
                    self.updateSelection(selected);
                    self.close();
                    self.setPlaceholder();
                    self.nextSearchTerm = self.opts.nextSearchTerm(selected, self.search.value);
                }
            });
        }
    }

    protected isPlaceholderOptionSelected() {
        var placeholderOption;
        if (this.getPlaceholder() === undefined) return false; // no placeholder specified so no option should be considered
        return ((placeholderOption = this.getPlaceholderOption()) !== undefined && placeholderOption.selected)
            || (this.opts.element.value === "")
            || (this.opts.element.value === undefined)
            || (this.opts.element.value === null);
    }

    protected override prepareOpts(opts: Select2Options) {
        opts = super.prepareOpts(opts);
        var self = this;

        if (opts.element.tagName.toLowerCase() === "select") {
            // install the selection initializer
            opts.initSelection = function (element, callback) {
                var selected = element.querySelector<HTMLOptionElement>("option:checked:not(:disabled)");
                // a single select box always has a value, no need to null check 'selected'
                callback(self.optionToData(selected));
            };
        } else if ("data" in opts) {
            // install default initSelection when applied to hidden input and data is local
            opts.initSelection = opts.initSelection || function (element, callback) {
                var id = (element as HTMLInputElement).value;
                //search in data by id, storing the actual matching item
                var match: Select2Item = null;
                opts.query({
                    matcher: function (term, text, el) {
                        var is_match = equal(id, opts.id(el));
                        if (is_match) {
                            match = el;
                        }
                        return is_match;
                    },
                    callback: typeof callback !== "function" ? () => { } : function () {
                        callback(match);
                    }
                });
            };
        }

        return opts;
    }

    protected override getPlaceholder() {
        // if a placeholder is specified on a single select without a valid placeholder option ignore it
        if (this.select) {
            if (this.getPlaceholderOption() === undefined) {
                return undefined;
            }
        }

        return super.getPlaceholder();
    }

    protected setPlaceholder() {
        var placeholder = this.getPlaceholder();

        if (this.isPlaceholderOptionSelected() && placeholder !== undefined) {

            // check for a placeholder option if attached to a select
            if (this.select && this.getPlaceholderOption() === undefined) return;

            var chosen = this.selection.querySelector(".select2-chosen");
            chosen && (chosen.textContent = placeholder);

            this.selection.classList.add("select2-default");

            this.container.classList.remove("select2-allowclear");
        }
    }

    protected override postprocessResults(data: Select2Result, initial: boolean, noHighlightUpdate?: boolean) {
        var selected = 0, self = this;

        // find the selected element in the result list

        this.findHighlightableChoices().forEach((elm, i) => {
            if (equal(self.id((elm as any).select2data), self.opts.element.value)) {
                selected = i;
                return false;
            }
        });

        // and highlight it
        if (noHighlightUpdate !== false) {
            if (initial === true && selected >= 0) {
                this.highlight(selected);
            } else {
                this.highlight(0);
            }
        }

        // hide the search box if this is the first we got the results and there are enough of them for search

        if (initial === true) {
            var min = this.opts.minimumResultsForSearch;
            if (min >= 0) {
                this.showSearch(countResults(data.results) >= min);
            }
        }
    }

    protected showSearch(showSearchInput: boolean) {
        if (this.showSearchInput === showSearchInput) return;

        this.showSearchInput = showSearchInput;

        this.dropdown.querySelector(".select2-search")?.classList.toggle("select2-search-hidden", !showSearchInput);
        this.dropdown.querySelector(".select2-search")?.classList.toggle("select2-offscreen", !showSearchInput);
        //add "select2-with-searchbox" to the container if search box is shown
        this.dropdown.classList.toggle("select2-with-searchbox", showSearchInput);
        this.container.classList.toggle("select2-with-searchbox", showSearchInput);
    }

    protected onSelect(data: Select2Item, options: any) {

        if (!this.triggerSelect(data)) { return; }

        var old = this.opts.element.value,
            oldData = this.data();

        this.opts.element.value = this.id(data);
        this.updateSelection(data);

        Fluent.trigger(this.opts.element, "select2-selected", { val: this.id(data), choice: data });

        this.nextSearchTerm = this.opts.nextSearchTerm(data, this.search.value);
        this.close();

        if ((!options || !options.noFocus) && this.opts.shouldFocusInput(this)) {
            this.focusser.focus();
        }

        if (!equal(old, this.id(data))) {
            this.triggerChange({ added: data, removed: oldData });
        }
    }

    protected updateSelection(data: Select2Item) {

        if (!this.selection)
            return;
        var container = this.selection.querySelector<HTMLElement>(".select2-chosen"), formatted, cssClass;

        (this.selection as any).select2data = data;

        Fluent.empty(container);
        if (data !== null) {
            formatted = this.opts.formatSelection(data, container, this.opts.escapeMarkup);
        }
        if (formatted !== undefined) {
            if (formatted instanceof Node)
                container.appendChild(formatted);
            else
                container.textContent = formatted;
        }
        cssClass = this.opts.formatSelectionCssClass(data, container);
        if (cssClass !== undefined) {
            container?.classList.add(cssClass);
        }

        this.selection.classList.remove("select2-default");

        if (this.opts.allowClear && this.getPlaceholder() !== undefined) {
            this.container?.classList.add("select2-allowclear");
        }
    }

    override val(val?: string, triggerChange?: boolean): string {
        var data = null,
            self = this,
            oldData = this.data();

        if (arguments.length === 0) {
            return this.opts.element.value;
        }

        if (arguments.length > 1) {
            triggerChange = arguments[1];
        }

        if (this.select) {
            this.select.value = val ?? "";
            var selected = this.select.querySelector<HTMLOptionElement>("option:checked");
            if (selected)
                data = self.optionToData(selected);
            this.updateSelection(data);
            this.setPlaceholder();
            if (triggerChange) {
                this.triggerChange({ added: data, removed: oldData });
            }
        } else {
            // val is an id. !val is true for [undefined,null,'',0] - 0 is legal
            if (!val && val as any !== 0) {
                this.clear(triggerChange);
                return;
            }
            if (this.opts.initSelection === undefined) {
                throw new Error("cannot call val() if initSelection() is not defined");
            }
            this.opts.element.value = val;
            this.opts.initSelection(this.opts.element, function (data) {
                self.opts.element.value = !data ? "" : self.id(data);
                self.updateSelection(data);
                self.setPlaceholder();
                if (triggerChange) {
                    self.triggerChange({ added: data, removed: oldData });
                }
            });
        }
    }

    protected override clearSearch() {
        this.search.value = "";
        this.focusser.value = "";
    }

    data(value?: Select2Item): (void | Select2Item) {
        var data,
            triggerChange = false;

        if (arguments.length === 0) {
            data = (this.selection as any)?.select2data;
            if (data == undefined) data = null;
            return data;
        } else {
            if (arguments.length > 1) {
                triggerChange = arguments[1];
            }
            if (!value) {
                this.clear(triggerChange);
            } else {
                data = this.data();
                this.opts.element.value = !value ? "" : this.id(value);
                this.updateSelection(value);
                if (triggerChange) {
                    this.triggerChange({ added: value, removed: data });
                }
            }
        }
    }
}

class MultiSelect2 extends AbstractSelect2 {

    protected createContainer(): HTMLElement {
        return (
            <div class="select2-container select2-container-multi">
                <ul class="select2-choices">
                    <li class="select2-search-field">
                        <label for="" class="select2-offscreen"></label>
                        <input type="text" autocomplete="off" autocorrect="off" autocapitalize="off" spellcheck="false" class="select2-input" />
                    </li>
                </ul>
                <div class="select2-drop select2-drop-multi select2-display-none">
                    <ul class="select2-results"></ul>
                </div>
            </div>
        ) as HTMLElement;
    }

    protected override prepareOpts(opts: Select2Options) {
        opts = super.prepareOpts(opts);
        var self = this;

        // TODO validate placeholder is a string if specified

        if (opts.element.tagName.toLowerCase() === "select") {
            // install the selection initializer
            opts.initSelection = function (element, callback) {

                var data: Select2Item[] = [];

                element.querySelectorAll<HTMLOptionElement>("option:checked:not(:disabled)").forEach(elm => {
                    data.push(self.optionToData(elm));
                });
                callback(data);
            };
        } else if ("data" in opts) {
            // install default initSelection when applied to hidden input and data is local
            opts.initSelection = opts.initSelection || function (element: HTMLInputElement, callback) {
                var ids = splitVal(element.value, opts.separator);
                //search in data by array of ids, storing matching items in a list
                var matches: Select2Item[] = [];
                opts.query({
                    matcher: function (term, text, el) {
                        var is_match = ids.some(id => equal(id, opts.id(el)));
                        if (is_match) {
                            matches.push(el);
                        }
                        return is_match;
                    },
                    callback: typeof callback !== "function" ? () => { } : function () {
                        // reorder matches based on the order they appear in the ids array because right now
                        // they are in the order in which they appear in data array
                        var ordered = [];
                        for (var i = 0; i < ids.length; i++) {
                            var id = ids[i];
                            for (var j = 0; j < matches.length; j++) {
                                var match = matches[j];
                                if (equal(id, opts.id(match))) {
                                    ordered.push(match);
                                    matches.splice(j, 1);
                                    break;
                                }
                            }
                        }
                        callback(ordered);
                    }
                });
            };
        }

        return opts;
    }

    protected selectChoice(choice: HTMLElement) {

        var selected = this.container.querySelector(".select2-search-choice-focus");
        if (selected && choice && choice === selected) {

        } else {
            if (selected) {
                Fluent.trigger(this.opts.element, "choice-deselected", { choice: selected });
                selected.classList.remove("select2-search-choice-focus");
            }
            if (choice) {
                this.close();
                choice.classList.add("select2-search-choice-focus");
                Fluent.trigger(this.opts.element, "choice-selected", { choice });
            }
        }
    }

    override destroy() {
        document.querySelector("label[for='" + this.search.getAttribute('id') + "']")?.setAttribute(
            'for', this.opts.element.getAttribute("id"));

        super.destroy();

        cleanupElements.call(this,
            "searchContainer",
            "selection"
        );
    }

    declare private searchContainer: HTMLElement;
    declare private keydowns: number;

    protected override initContainer() {

        var selector = ".select2-choices";

        this.searchContainer = this.container.querySelector(".select2-search-field");
        var selection: HTMLElement;
        this.selection = selection = this.container.querySelector(selector);

        var _this = this;
        Fluent.on(this.selection, "click", ".select2-search-choice:not(.select2-locked)", (e: Event) => {
            //killEvent(e);
            _this.search.focus();
            _this.selectChoice((e.target as HTMLElement).closest('.select2-search-choice') as HTMLElement);
        });

        // rewrite labels from original element to focusser
        this.search.setAttribute("id", "s2id_autogen" + nextUid());

        this.search.previousElementSibling.textContent = document.querySelector("label[for='" + this.opts.element.getAttribute("id") + "']")?.textContent;
        this.search.previousElementSibling.setAttribute("for", this.search.getAttribute('id'));

        ["input", "paste"].forEach(ev => Fluent.on(this.search, ev, () => {
            if (this.search.getAttribute('placeholder') && this.search.value.length === 0) return;
            if (!this.isInterfaceEnabled()) return;
            if (!this.opened()) {
                this.open();
            }
        }));

        this.search.setAttribute("tabindex", this.elementTabIndex);

        this.keydowns = 0;
        Fluent.on(this.search, "keydown", e => {
            if (!this.isInterfaceEnabled()) return;

            ++this.keydowns;
            var selected = selection.querySelector<HTMLElement>(".select2-search-choice-focus");
            var prev = Fluent(selected).prevSibling(".select2-search-choice:not(.select2-locked)").getNode();
            var next = Fluent(selected).nextSibling(".select2-search-choice:not(.select2-locked)").getNode();
            var pos = getCursorInfo(this.search);

            if (selected &&
                (e.key == "ArrowLeft" || e.key == "ArrowRight" || e.key == "Backspace" || e.key == "Delete" || e.key == "Enter")) {
                var selectedChoice = selected;
                if (e.key == "ArrowLeft" && prev) {
                    selectedChoice = prev;
                }
                else if (e.key == "ArrowRight") {
                    selectedChoice = next ? next : null;
                }
                else if (e.key === "Backspace") {
                    if (this.unselect(selected)) {
                        this.search.style.width = "10px";
                        selectedChoice = prev ? prev : next;
                    }
                } else if (e.key == "Delete") {
                    if (this.unselect(selected)) {
                        this.search.style.width = "10px";
                        selectedChoice = next ? next : null;
                    }
                } else if (e.key == "Enter") {
                    selectedChoice = null;
                }

                this.selectChoice(selectedChoice);
                killEvent(e);
                if (!selectedChoice || !selectedChoice) {
                    this.open();
                }
                return;
            } else if ((e.key === "Backspace" && this.keydowns == 1
                || e.key == "ArrowLeft") && (pos.offset == 0 && !pos.length)) {

                this.selectChoice(Array.from(selection.querySelectorAll<HTMLElement>(".select2-search-choice:not(.select2-locked)")).pop());
                killEvent(e);
                return;
            } else {
                this.selectChoice(null);
            }

            if (this.opened()) {
                switch (e.key) {
                    case "ArrowUp":
                    case "ArrowDown":
                        this.moveHighlight((e.key === "ArrowUp") ? -1 : 1);
                        killEvent(e);
                        return;
                    case "Enter":
                        this.selectHighlighted();
                        killEvent(e);
                        return;
                    case "Tab":
                        this.selectHighlighted({ noFocus: true });
                        this.close();
                        return;
                    case "Escape":
                        this.cancel(e);
                        killEvent(e);
                        return;
                }
            }

            if (e.key === "Tab" || e.key === "Shift" || e.key === "Control" || e.key === "Alt" || e.metaKey
                || /^F(?:1[0-2]|[1-9])$/.test(e.key) || e.key === "Backspace" || e.key === "Escape") {
                return;
            }

            if (e.key === "Enter") {
                if (this.opts.openOnEnter === false) {
                    return;
                } else if (e.altKey || e.ctrlKey || e.shiftKey || e.metaKey) {
                    return;
                }
            }

            this.open();

            if (e.key === "PageUp" || e.key === "PageDown") {
                // prevent the page from scrolling
                killEvent(e);
            }

            if (e.key === "Enter") {
                // prevent form from being submitted
                killEvent(e);
            }

        });

        Fluent.on(this.search, "keyup", e => {
            this.keydowns = 0;
            this.resizeSearch();
        });

        Fluent.on(this.search, "blur", e => {
            this.container.classList.remove("select2-container-active");
            this.search.classList.remove("select2-focused");
            this.selectChoice(null);
            if (!this.opened()) this.clearSearch();
            e.stopImmediatePropagation();
            Fluent.trigger(this.opts.element, "select2-blur");
        });

        Fluent.on(this.container, "click", selector, (e: Event) => {
            if (!this.isInterfaceEnabled()) return;
            if ((e.target as HTMLElement)?.closest(".select2-search-choice")) {
                // clicked inside a select2 search choice, do not open
                return;
            }
            this.selectChoice(null);
            this.clearPlaceholder();
            if (!this.container.classList.contains("select2-container-active")) {
                Fluent.trigger(this.opts.element, "select2-focus");
            }
            this.open();
            this.focusSearch();
            e.preventDefault();
        });

        Fluent.on(this.container, "focus", selector, () => {
            if (!this.isInterfaceEnabled()) return;
            if (!this.container.classList.contains("select2-container-active")) {
                Fluent.trigger(this.opts.element, "select2-focus");
            }
            this.container.classList.add("select2-container-active");
            this.dropdown.classList.add("select2-drop-active");
            this.clearPlaceholder();
        });

        this.initContainerWidth();
        this.opts.element.classList.add("select2-offscreen");

        // set the placeholder if necessary
        this.clearSearch();
    }

    protected override enableInterface() {
        var result = super.enableInterface();
        if (result) {
            this.search.disabled = !this.isInterfaceEnabled();
        }
        return result;
    }

    protected initSelection() {
        if (this.opts.element.value === "" && this.opts.element.textContent === "") {
            this.updateSelection([]);
            this.close();
            // set the placeholder if necessary
            this.clearSearch();
        }
        if (this.select || this.opts.element.value !== "") {
            var self = this;
            this.opts.initSelection.call(null, this.opts.element, function (data: Select2Item[]) {
                if (!self.container)
                    return;
                if (data !== undefined && data !== null) {
                    self.updateSelection(data);
                    self.close();
                    // set the placeholder if necessary
                    self.clearSearch();
                }
            });
        }
    }

    protected override clearSearch() {
        var placeholder = this.getPlaceholder(),
            maxWidth = this.getMaxSearchWidth();

        if (placeholder !== undefined && this.getVal().length === 0 && !this.search.classList.contains("select2-focused")) {
            this.search.value = placeholder ?? "";
            this.search.classList.add("select2-default");
            // stretch the search box to full width of the container so as much of the placeholder is visible as possible
            // we could call this.resizeSearch(), but we do not because that requires a sizer and we do not want to create one so early because of a firefox bug, see #944
            this.search.style.width = maxWidth > 0 ? (maxWidth + "px") : getComputedStyle(this.container).width;
        } else {
            this.search.value = "";
            this.search.style.width = "10px";
        }
    }

    protected clearPlaceholder() {
        if (this.search.classList.contains("select2-default")) {
            this.search.value = "";
            this.search.classList.remove("select2-default");
        }
    }

    protected override opening() {
        this.clearPlaceholder(); // should be done before super so placeholder is not used to search
        this.resizeSearch();

        super.opening();

        this.focusSearch();

        // initializes search's value with nextSearchTerm (if defined by user)
        // ignore nextSearchTerm if the dropdown is opened by the user pressing a letter
        if (this.search.value === "") {
            if (this.nextSearchTerm != undefined) {
                this.search.value = this.nextSearchTerm ?? "";
                this.search.select();
            }
        }

        this.updateResults(true);
        if (this.opts.shouldFocusInput(this)) {
            this.search.focus();
        }
        Fluent.trigger(this.opts.element, "select2-open");
    }

    override close() {
        if (!this.opened()) return;
        super.close();
    }

    override focus() {
        this.close();
        this.search.focus();
    }

    override isFocused() {
        return this.search.classList.contains("select2-focused");
    }

    protected updateSelection(data: Select2Item[]) {
        var ids: string[] = [], filtered: Select2Item[] = [], self = this;

        // filter out duplicates
        data.forEach(function (x) {
            if (indexOf(self.id(x), ids) < 0) {
                ids.push(self.id(x));
                filtered.push(x);
            }
        });
        data = filtered;

        this.selection.querySelectorAll(".select2-search-choice").forEach(el => Fluent.remove(el));
        data.forEach(function (x) {
            self.addSelectedChoice(x);
        });
        self.postprocessResults();
    }

    protected override tokenize(): string {
        var input = this.search.value;
        input = this.opts.tokenizer.call(this, input, this.data(), bindThis(this).onSelect, this.opts);
        if (input != null && input != undefined) {
            this.search.value = input ?? "";
            if (input.length > 0) {
                this.open();
            }
        }
        return undefined;
    }

    protected override onSelect(data: Select2Item, options: any) {

        if (!this.triggerSelect(data) || data.text === "") { return; }

        this.addSelectedChoice(data);

        Fluent.trigger(this.opts.element, "selected", { val: this.id(data), choice: data });

        // keep track of the search's value before it gets cleared
        this.nextSearchTerm = this.opts.nextSearchTerm(data, this.search.value);

        this.clearSearch();
        this.updateResults();

        if (this.select || !this.opts.closeOnSelect) this.postprocessResults(undefined, false, this.opts.closeOnSelect === true);

        if (this.opts.closeOnSelect) {
            this.close();
            this.search.style.width = "10px";
        } else {
            if (this.countSelectableResults() > 0) {
                this.search.style.width = "10px";
                this.resizeSearch();
                if (this.getMaximumSelectionSize() > 0 && this.val().length >= this.getMaximumSelectionSize()) {
                    // if we reached max selection size repaint the results so choices
                    // are replaced with the max selection reached message
                    this.updateResults(true);
                } else {
                    // initializes search's value with nextSearchTerm and update search result
                    if (this.nextSearchTerm != undefined) {
                        this.search.value = this.nextSearchTerm ?? "";
                        this.updateResults();
                        this.search.select();
                    }
                }
                this.positionDropdown();
            } else {
                // if nothing left to select close
                this.close();
                this.search.style.width = "10px";
            }
        }

        // since its not possible to select an element that has already been
        // added we do not need to check if this is a new element before firing change
        this.triggerChange({ added: data });

        if (!options || !options.noFocus)
            this.focusSearch();
    }

    protected override cancel(e?: Event) {
        this.close();
        this.focusSearch();
    }

    protected addSelectedChoice(data: Select2Item) {
        var enableChoice = !data.locked;
        var choice = document.createElement("li"),
            id = this.id(data),
            val = this.getVal(),
            formatted,
            cssClass;

        choice.classList.add("select2-search-choice");
        choice.appendChild(document.createElement("div"));
        if (enableChoice) {
            var a = choice.appendChild(document.createElement("a"));
            a.classList.add("select2-search-choice-close");
            a.setAttribute("href", "#");
            a.setAttribute("tabindex", "-1");
        }
        else {
            choice.classList.add("select2-locked");
        }

        var div = choice.querySelector("div");
        formatted = this.opts.formatSelection(data, div, this.opts.escapeMarkup);
        if (formatted != undefined) {
            if (formatted instanceof Node)
                div.appendChild(formatted);
            else
                div.textContent = formatted ?? "";
        }
        cssClass = this.opts.formatSelectionCssClass(data, div);
        if (cssClass != undefined) {
            Fluent.addClass(choice, cssClass);
        }

        if (enableChoice) {
            var close = choice.querySelector(".select2-search-choice-close");
            Fluent.on(close, "mousedown", killEvent);
            ["click", "dblclick"].forEach(ev => Fluent.on(close, ev, (e) => {
                if (!this.isInterfaceEnabled()) return;

                this.unselect(e.target as HTMLElement);
                this.selection.querySelector(".select2-search-choice-focus")?.classList.remove("select2-search-choice-focus");
                killEvent(e);
                this.close();
                this.focusSearch();
            }));

            Fluent.on(close, "focus", () => {
                if (!this.isInterfaceEnabled()) return;
                this.container.classList.add("select2-container-active");
                this.dropdown.classList.add("select2-drop-active");
            });
        }

        (choice as any).select2data = data;
        Fluent(choice).insertBefore(this.searchContainer);

        val.push(id);
        this.setVal(val);
    }

    protected unselect(selected: HTMLElement) {
        var val = this.getVal(),
            data,
            index;
        selected = selected?.closest(".select2-search-choice") as HTMLElement;

        if (!selected) {
            throw new Error("Invalid argument: must be .select2-search-choice");
        }

        data = (selected as any).select2data;

        if (!data) {
            // prevent a race condition when the 'x' is clicked really fast repeatedly the event can be queued
            // and invoked on an element already removed
            return;
        }

        var evt = Fluent.trigger(this.opts.element, "select2-removing", { val: this.id(data), choice: data });

        if (Fluent.isDefaultPrevented(evt)) {
            return false;
        }

        while ((index = indexOf(this.id(data), val)) >= 0) {
            val.splice(index, 1);
            this.setVal(val);
            if (this.select) this.postprocessResults();
        }

        selected.remove();

        Fluent.trigger(this.opts.element, "select2-removed", { val: this.id(data), choice: data });
        this.triggerChange({ removed: data });

        return true;
    }

    protected override postprocessResults(data?: Select2Result, initial?: boolean, noHighlightUpdate?: boolean) {
        var val = this.getVal(),
            choices = this.results.querySelectorAll(".select2-result"),
            compound = this.results.querySelectorAll(".select2-result-with-children"),
            self = this;

        choices.forEach(function (choice) {
            var id = self.id((choice as any).select2data);
            if (indexOf(id, val) >= 0) {
                choice.classList.add("select2-selected");
                // mark all children of the selected parent as selected
                choice.querySelectorAll(".select2-result-selectable").forEach(x => x.classList.add("select2-selected"));
            }
        });

        compound.forEach(function (choice) {
            // hide an optgroup if it doesn't have any selectable children
            if (!choice.matches('.select2-result-selectable')
                && !choice.querySelector(".select2-result-selectable:not(.select2-selected)")) {
                choice.classList.add("select2-selected");
            }
        });

        if (this.highlight() == -1 && noHighlightUpdate !== false) {
            self.highlight(0);
        }

        //If all results are chosen render formatNoMatches
        if (!this.opts.createSearchChoice && !Array.from(choices).some(x => x.matches('.select2-result:not(.select2-selected)'))) {
            if (!data || data && !data.more && !this.results.querySelector(".select2-no-results")) {
                if (checkFormatter(self.opts.formatNoMatches, "formatNoMatches")) {
                    var noResults = evaluate(self.opts.formatNoMatches, self.opts.element, self.search.value);
                    var li = document.createElement("li");
                    li.classList.add("select2-no-results");
                    if (noResults instanceof Node)
                        li.appendChild(noResults);
                    else
                        li.textContent = noResults ?? "";
                }
            }
        }

    }

    protected getMaxSearchWidth() {
        return this.selection.getBoundingClientRect().width - getSideBorderPadding(this.search);
    }

    protected resizeSearch() {
        var minimumWidth, left, maxWidth, containerLeft, searchWidth,
            sideBorderPadding = getSideBorderPadding(this.search);

        minimumWidth = measureTextWidth(this.search) + 10;

        left = getOffset(this.search).left;

        maxWidth = this.selection.getBoundingClientRect().width;
        containerLeft = getOffset(this.selection).left;

        searchWidth = maxWidth - (left - containerLeft) - sideBorderPadding;

        if (searchWidth < minimumWidth) {
            searchWidth = maxWidth - sideBorderPadding;
        }

        if (searchWidth < 40) {
            searchWidth = maxWidth - sideBorderPadding;
        }

        if (searchWidth <= 0) {
            searchWidth = minimumWidth;
        }

        this.search.style.width = (Math.floor(searchWidth)) + "px";
    }

    protected getVal(): string[] {
        var val;
        if (this.select) {
            val = Array.from(this.select.selectedOptions).map(x => x.value);
            return val === null ? [] : val;
        } else {
            val = this.opts.element.value;
            return splitVal(val, this.opts.separator);
        }
    }

    protected setVal(val: string[]) {
        if (this.select) {
            val ??= [];
            var opt = this.select.options;
            for (var i = 0; i < opt.length; i++) {
                var o = opt.item(i);
                o.selected = val.includes(o.value);
            }
        } else {
            var unique = val || [];
            unique = unique.filter((x, i) => unique.indexOf(x) === i);
            this.opts.element.value = (unique.length === 0 ? "" : unique.join(this.opts.separator));
        }
    }

    protected buildChangeDetails(old: Select2Item[], current: Select2Item[]) {
        var current = current.slice(0),
            old = old.slice(0);

        // remove intersection from each array
        for (var i = 0; i < current.length; i++) {
            for (var j = 0; j < old.length; j++) {
                if (equal(this.opts.id(current[i]), this.opts.id(old[j]))) {
                    current.splice(i, 1);
                    if (i > 0) {
                        i--;
                    }
                    old.splice(j, 1);
                    j--;
                }
            }
        }

        return { added: current, removed: old };
    }

    val(val?: string[], triggerChange?: boolean) {
        var self = this;

        if (arguments.length === 0) {
            return this.getVal();
        }

        var oldData = this.data();
        if (!oldData) oldData = [];

        // val is an id. !val is true for [undefined,null,'',0] - 0 is legal
        if (!val && (val as any) !== 0) {
            this.opts.element.value = "";
            this.updateSelection([]);
            this.clearSearch();
            if (triggerChange) {
                this.triggerChange({ added: this.data(), removed: oldData });
            }
            return;
        }

        // val is a list of ids
        this.setVal(val);

        if (this.select) {
            this.opts.initSelection(this.select, bindThis(this).updateSelection);
            if (triggerChange) {
                this.triggerChange(this.buildChangeDetails(oldData, this.data()));
            }
        } else {
            if (this.opts.initSelection === undefined) {
                throw new Error("val() cannot be called if initSelection() is not defined");
            }

            this.opts.initSelection(this.opts.element, function (data) {
                var ids = data.map(self.id);
                self.setVal(ids);
                self.updateSelection(data);
                self.clearSearch();
                if (triggerChange) {
                    self.triggerChange(self.buildChangeDetails(oldData, self.data()));
                }
            });
        }
        this.clearSearch();
    }

    protected onSortStart() {
        if (this.select) {
            throw new Error("Sorting of elements is not supported when attached to <select>. Attach to <input type='hidden'/> instead.");
        }

        // collapse search field into 0 width so its container can be collapsed as well
        this.search.style.width = "0px";
        // hide the container
        this.searchContainer.hidden = true;
    }

    protected onSortEnd() {

        var val: string[] = [], self = this;

        // show search and move it to the end of the list
        this.searchContainer.hidden = false;
        // make sure the search container is the last item in the list
        Fluent(this.searchContainer.parentElement).append(this.searchContainer);
        // since we collapsed the width in dragStarted, we resize it here
        this.resizeSearch();

        // update selection
        this.selection.querySelectorAll(".select2-search-choice").forEach(el => {
            val.push(self.opts.id((el as any).select2data));
        });
        this.setVal(val);
        this.triggerChange();
    }

    data(): Select2Item[];
    data(values: Select2Item[], triggerChange?: boolean): void;
    data(values?: Select2Item[], triggerChange?: boolean): Select2Item[] | void {
        var self = this, ids, old;
        if (arguments.length === 0) {
            var result: Select2Item[] = [];
            this.selection
                .querySelectorAll(":scope > .select2-search-choice")
                .forEach(x => result.push((x as any).select2data));
            return result;
        } else {
            old = this.data();
            if (!values) { values = []; }
            ids = values.map(self.opts.id);
            this.setVal(ids);
            this.updateSelection(values);
            this.clearSearch();
            if (triggerChange) {
                this.triggerChange(this.buildChangeDetails(old, this.data()));
            }
        }
    }
}


function handleDropdownTrap(e: Event) {
    e.stopPropagation();
}

const safeStyleProperties = [
    'width', 'height', 'min-width', 'min-height', 'max-width', 'max-height',
    'margin', 'margin-top', 'margin-right', 'margin-bottom', 'margin-left',
    'padding', 'padding-top', 'padding-right', 'padding-bottom', 'padding-left',
    'border', 'border-width', 'border-style', 'border-color',
    'border-top', 'border-right', 'border-bottom', 'border-left',
    'border-top-width', 'border-right-width', 'border-bottom-width', 'border-left-width',
    'border-top-style', 'border-right-style', 'border-bottom-style', 'border-left-style',
    'border-top-color', 'border-right-color', 'border-bottom-color', 'border-left-color',
    'box-sizing', 'display', 'position', 'float', 'clear'
];

function copyStyle(from: CSSStyleDeclaration, to: CSSStyleDeclaration) {
    for (var prop of safeStyleProperties) {
        if (from.getPropertyValue(prop)) {
            to.setProperty(prop, from.getPropertyValue(prop));
        }
    }
}
