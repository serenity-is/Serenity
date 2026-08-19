import { Fluent, isArrayLike, isPromiseLike } from "../../base";
import { Select2, Select2Options } from "./select2";

/** The combobox provider type. */
export type ComboboxType = "select2";
/** Result of a combobox formatter. */
export type ComboboxFormatResult = string | Element | DocumentFragment;

/**
 * A single item in a combobox.
 * @typeParam TSource - The source item type.
 */
export interface ComboboxItem<TSource = any> {
    /** Item id. */
    id?: string;
    /** Display text. */
    text?: string;
    /** The source item. */
    source?: TSource;
    /** Whether the item is disabled. */
    disabled?: boolean;
}

/**
 * Query passed to a combobox search callback.
 */
export interface ComboboxSearchQuery {
    /** The search term. */
    searchTerm?: string;
    /** List of ids to initialize the selection from. */
    idList?: string[];
    /** Number of items to skip. */
    skip?: number;
    /** Number of items to take. */
    take?: number;
    /** Whether to check for more results. */
    checkMore?: boolean;
    /** Whether this is an initial selection query. */
    initSelection?: boolean;
    /** Abort signal for cancelling the query. */
    signal?: AbortSignal;
}

/**
 * Result of a combobox search.
 * @typeParam TItem - The item type.
 */
export interface ComboboxSearchResult<TItem> {
    /** The matching items. */
    items: TItem[];
    /** Whether there are more results. */
    more: boolean;
}

/**
 * Options for the {@link Combobox}.
 * @typeParam TSource - The source item type.
 */
export interface ComboboxOptions<TSource = any> {
    /** Whether the selection can be cleared. */
    allowClear?: boolean;
    /** Callback that creates a search choice for arbitrary values. */
    createSearchChoice?: (s: string) => ComboboxItem<TSource>;
    /** The element to attach the combobox to. */
    element?: HTMLInputElement | HTMLSelectElement | Element[];
    /** Allow arbitrary values for items. */
    arbitraryValues?: boolean;
    /** Formatter for the selected item. */
    formatSelection?: (p1: ComboboxItem<TSource>) => ComboboxFormatResult;
    /** Formatter for result items. */
    formatResult?: (p1: ComboboxItem<TSource>) => ComboboxFormatResult;
    /** Minimum results required to show the search box. */
    minimumResultsForSearch?: number;
    /** Whether multiple items can be selected. */
    multiple?: boolean;
    /** Page size to use while loading or displaying results. */
    pageSize?: number;
    /** Placeholder text. */
    placeholder?: string;
    /** Callback to get options specific to the combobox provider type. */
    providerOptions?: (type: ComboboxType, opt: ComboboxOptions) => any;
    /** Callback that performs the search. */
    search?: (query: ComboboxSearchQuery) => (PromiseLike<ComboboxSearchResult<ComboboxItem<TSource>>> | ComboboxSearchResult<ComboboxItem<TSource>>);
    /** Type delay for searching, default is 200. */
    typeDelay?: number;
}

/**
 * A combobox widget that provides searchable selection over a set of items.
 * @typeParam TItem - The item type.
 */
export class Combobox<TItem = any> {
    declare private el: HTMLInputElement | HTMLSelectElement;

    /** Default combobox options. */
    static defaults: ComboboxOptions = {
        pageSize: 100,
        typeDelay: 200
    }

    constructor(opt: ComboboxOptions);
    /**
     * Creates a combobox.
     * @param opt - Combobox options.
     * @param create - When false, only wraps an existing combobox without creating a new one.
     */
    constructor(opt: ComboboxOptions, create: boolean = true) {
        if (isArrayLike(opt?.element))
            this.el = opt.element[0] as HTMLInputElement;
        else if (typeof opt?.element !== "function")
            this.el = opt?.element;

        if (!create)
            return;

        opt = Object.assign({}, Combobox.defaults, opt);
        this.createSelect2(opt);
    }

    private createSelect2(opt: ComboboxOptions) {
        var select2Opt: Select2Options = {
            element: this.el,
            multiple: opt.multiple,
            placeholder: opt.placeholder || null,
            allowClear: opt.allowClear,
            createSearchChoicePosition: 'bottom'
        }

        select2Opt.query = (query: any) => {
            var pageSize = opt.pageSize;
            var searchQuery: ComboboxSearchQuery = {
                searchTerm: query.term?.trim() || null,
                skip: (query.page - 1) * pageSize,
                take: pageSize,
                checkMore: true
            }

            this.abortPendingQuery();

            var select2 = Select2.getInstance(this.el);

            function setActive(value: boolean) {
                select2?.search?.classList.toggle('select2-active', value);
                select2?.search?.parentElement?.classList.toggle('select2-active', value);
            }

            (this.el as any).typeTimeoutFn = () => {
                this.abortPendingQuery();
                setActive(true);
                searchQuery.signal = ((this.el as any).queryLoading = new AbortController()).signal;

                const cleanup = () => {
                    delete (this.el as any).queryLoading;
                    setActive(false);
                }

                try {
                    const then = (result: ComboboxSearchResult<ComboboxItem<TItem>>) => {
                        delete (this.el as any).queryLoading;
                        setActive(false);
                        query.callback({
                            results: result.items,
                            more: result.more
                        });
                    }

                    var searchResult = opt.search(searchQuery);
                    if (isPromiseLike(searchResult)) {
                        searchResult.then(then, cleanup);
                    }
                    else {
                        cleanup();
                        searchResult && then(searchResult);
                    }
                }
                catch (e) {
                    cleanup();
                    throw e;
                }
            };

            (this.el as any).typeTimeout = setTimeout((this.el as any).typeTimeoutFn, !query.term ? 0 : opt.typeDelay);
        }

        select2Opt.initSelection = (element: ArrayLike<HTMLElement> | HTMLElement, callback: any) => {
            var el = isArrayLike(element) ? element[0] : element;
            var val = (el as any).value;
            if (val == null || val == '') {
                callback(null);
                return;
            }

            var isMultiple = opt.multiple;
            var idList = isMultiple ? (val as string).split(',') : [val as string];
            var searchQuery: ComboboxSearchQuery = {
                idList: idList,
                initSelection: true
            };

            const then = (result: ComboboxSearchResult<ComboboxItem<TItem>>) => {
                cleanup();
                if (isMultiple) {
                    callback(result.items);
                }
                else if (!result.items || !result.items.length) {
                    if (opt.arbitraryValues) {
                        callback({
                            id: val,
                            text: val
                        });
                    }
                    else
                        callback(null);
                }
                else {
                    var item = result.items[0];
                    callback(item);
                }
            }

            const cleanup = () => { delete (el as any).initSelectionLoading; }

            (el as any).initSelectionLoading && (el as any).initSelectionLoading?.abort?.();
            searchQuery.signal = ((el as any).initSelectionLoading = new AbortController()).signal;
            try {
                let searchResult = opt.search(searchQuery);
                if (isPromiseLike(searchResult)) {
                    searchResult.then(then, cleanup);
                }
                else {
                    cleanup();
                    if (searchResult)
                        then(searchResult);
                }
            }
            catch (e) {
                cleanup();
                throw e;
            }
        }

        if (opt.createSearchChoice)
            select2Opt.createSearchChoice = opt.createSearchChoice;

        if (opt.formatResult)
            select2Opt.formatResult = opt.formatResult;

        if (opt.formatSelection)
            select2Opt.formatSelection = opt.formatSelection;

        if (opt.providerOptions)
            select2Opt = Object.assign(select2Opt, opt.providerOptions("select2", opt));

        new Select2(select2Opt);

        Fluent.on(this.el, "execute-search", () => {
            if (!this.el || !(this.el as any).typeTimeout || !(this.el as any).typeTimeoutFn) {
                return;
            }
            (this.el as any).typeTimeoutFn();
            delete (this.el as any).typeTimeout;
            delete (this.el as any).typeTimeoutFn;
        });
    }

    /**
     * Aborts any pending search query.
     */
    abortPendingQuery() {
        if (!this.el)
            return;

        (this.el as any).queryLoading && (this.el as any).queryLoading?.abort?.();
        (this.el as any).queryLoading = false;
        if ((this.el as any).typeTimeout) {
            clearTimeout((this.el as any).typeTimeout);
            delete (this.el as any).typeTimeout;
            delete (this.el as any).typeTimeoutFn;
        }
    }

    /**
     * Aborts any pending initial selection query.
     */
    abortInitSelection() {
        if (!this.el)
            return;

        (this.el as any).initSelectionLoading && (this.el as any).initSelectionLoading?.abort?.();
        delete (this.el as any).initSelectionLoading;
    }

    /**
     * Disposes the combobox and cleans up its resources.
     */
    dispose() {
        if (!this.el)
            return;
        this.abortInitSelection();
        this.abortPendingQuery();
        Select2.getInstance(this.el)?.destroy();
        Fluent.off(this.el, "execute-search");
    }

    /**
     * Returns the combobox container element.
     * @returns The container element.
     */
    get container(): HTMLElement {
        if (!this.el)
            return null;
        return Select2.getInstance(this.el)?.container;
    }

    /**
     * Returns the combobox provider type.
     * @returns The provider type, or null.
     */
    get type(): ComboboxType {
        if (!this.el)
            return null;

        if (Select2.getInstance(this.el))
            return "select2";

        return null;
    }

    /**
     * Whether the combobox allows multiple selection.
     * @returns True when multiple.
     */
    get isMultiple(): boolean {
        if (!this.el)
            return false;

        var select2 = Select2.getInstance(this.el);
        if (select2)
            return select2.isMultiple;

        return this.el.getAttribute('multiple') != null;
    }

    /**
     * Returns the first selected item.
     * @returns The selected item.
     */
    getSelectedItem(): ComboboxItem {
        var select2 = Select2.getInstance(this.el);
        if (select2) {
            var item = select2.data;
            if (Array.isArray(item))
                return item[0];
            return item;
        }
    }

    /**
     * Returns all selected items.
     * @returns The selected items.
     */
    getSelectedItems(): ComboboxItem[] {
        var select2 = Select2.getInstance(this.el);
        if (select2) {
            var item = select2.data;
            if (Array.isArray(item))
                return item;

            if (!item)
                return [];

            return [item];
        }

        return [];
    }

    /**
     * Returns the current value as a comma-separated string.
     * @returns The value.
     */
    getValue(): string {
        if (!this.el)
            return null;

        var select2 = Select2.getInstance(this.el);
        if (select2) {
            var val = select2.val;
            if (Array.isArray(val))
                return val.join(',');

            return val;
        }

        return this.el.value;
    }

    /**
     * Returns the current values as an array.
     * @returns The values.
     */
    getValues(): string[] {
        if (!this.el)
            return [];

        let val: any;
        let select2 = Select2.getInstance(this.el);
        if (select2)
            val = select2.val;
        else
            val = this.el.value;

        if (Array.isArray(val))
            return val;

        if (val == null || val === "")
            return [];

        return [val];
    }

    /**
     * Sets the current value.
     * @param value - The value to set.
     * @param triggerChange - When true, triggers a change event.
     */
    setValue(value: string, triggerChange = false) {
        if (!this.el)
            return;

        if (value == this.getValue())
            return;

        var val: any = value;
        if (value && this.isMultiple) {
            val = value.split(String.fromCharCode(44))
                .map(x => x?.trim() || null)
                .filter(x1 => x1 != null);
        }

        this.el.dataset.comboboxsettingvalue = "true";
        try {
            let select2 = Select2.getInstance(this.el);
            if (select2) {
                select2.val = val;
            }
            else {
                this.el.value = val;
            }

            if (triggerChange)
                Fluent.trigger(this.el, "change");

        } finally {
            delete this.el.dataset.comboboxsettingvalue;
        }
    }

    /**
     * Sets the current values.
     * @param value - The values to set.
     * @param triggerChange - When true, triggers a change event.
     */
    setValues(value: string[], triggerChange = false) {
        if (value == null || value.length === 0) {
            this.setValue(null, triggerChange);
            return;
        }

        this.setValue(value.join(','), triggerChange);
    }

    /**
     * Closes the dropdown.
     */
    closeDropdown(): void {
        Select2.getInstance(this.el)?.close();
    }

    /**
     * Opens the dropdown.
     */
    openDropdown(): void {
        Select2.getInstance(this.el)?.open();
    }

    /**
     * Returns the combobox instance attached to an element, or null.
     * @param el - The element or collection.
     * @returns The combobox instance, or null.
     */
    static getInstance(el: Element | ArrayLike<Element>): Combobox {
        if (!el || !Select2.getInstance((isArrayLike(el) ? el[0] : el) as HTMLInputElement))
            return null;

        return new (Combobox as any)({ element: el }, false);
    }
}

/**
 * Strips diacritics from a string for accent-insensitive searching.
 * @param str - The string to process.
 * @returns The string with diacritics removed.
 */
export function stripDiacritics(str: string) {
    if (!str)
        return str;
    return Select2.stripDiacritics(str);
}