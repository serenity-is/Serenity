import { Fluent, isArrayLike, isPromiseLike } from "../../base";
import { Select2, Select2Options } from "./select2";

export type ComboboxType = "select2";
export type ComboboxFormatResult = string | Element | DocumentFragment;

export interface ComboboxItem<TSource = any> {
    id?: string;
    text?: string;
    source?: TSource;
    disabled?: boolean;
}

export interface ComboboxSearchQuery {
    searchTerm?: string;
    idList?: string[];
    skip?: number;
    take?: number;
    checkMore?: boolean;
    initSelection?: boolean;
    signal?: AbortSignal;
}

export interface ComboboxSearchResult<TItem> {
    items: TItem[];
    more: boolean;
}

export interface ComboboxOptions<TSource = any> {
    allowClear?: boolean;
    createSearchChoice?: (s: string) => ComboboxItem<TSource>;
    element?: HTMLInputElement | HTMLSelectElement | Element[];
    /** Allow arbitrary values for items */
    arbitraryValues?: boolean;
    formatSelection?: (p1: ComboboxItem<TSource>) => ComboboxFormatResult;
    formatResult?: (p1: ComboboxItem<TSource>) => ComboboxFormatResult;
    minimumResultsForSearch?: number;
    multiple?: boolean;
    /** Page size to use while loading or displaying results */
    pageSize?: number;
    placeholder?: string;
    /** Callback to get options specific to the combobox provider type */
    providerOptions?: (type: ComboboxType, opt: ComboboxOptions) => any;
    search?: (query: ComboboxSearchQuery) => (PromiseLike<ComboboxSearchResult<ComboboxItem<TSource>>> | ComboboxSearchResult<ComboboxItem<TSource>>);
    /** Type delay for searching, default is 200 */
    typeDelay?: number;
}

export class Combobox<TItem = any> {
    private el: HTMLInputElement | HTMLSelectElement;

    static defaults: ComboboxOptions = {
        pageSize: 100,
        typeDelay: 200
    }

    constructor(opt: ComboboxOptions);
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

    abortInitSelection() {
        if (!this.el)
            return;

        (this.el as any).initSelection && (this.el as any).initSelection?.abort?.();
        delete (this.el as any).initSelection;
    }

    dispose() {
        if (!this.el)
            return;
        this.abortInitSelection();
        this.abortPendingQuery();
        Select2.getInstance(this.el)?.destroy();
        Fluent.off(this.el, "execute-search");
    }

    get container(): HTMLElement {
        if (!this.el)
            return null;
        return Select2.getInstance(this.el)?.container;
    }

    get type(): ComboboxType {
        if (!this.el)
            return null;

        if (Select2.getInstance(this.el))
            return "select2";

        return null;
    }

    get isMultiple(): boolean {
        if (!this.el)
            return false;

        var select2 = Select2.getInstance(this.el);
        if (select2)
            return select2.isMultiple;

        return this.el.getAttribute('multiple') != null;
    }

    getSelectedItem(): ComboboxItem {
        var select2 = Select2.getInstance(this.el);
        if (select2) {
            var item = select2.data;
            if (Array.isArray(item))
                return item[0];
            return item;
        }
    }

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

    setValues(value: string[], triggerChange = false) {
        if (value == null || value.length === 0) {
            this.setValue(null, triggerChange);
            return;
        }

        this.setValue(value.join(','), triggerChange);
    }

    closeDropdown(): void {
        Select2.getInstance(this.el)?.close();
    }

    openDropdown(): void {
        Select2.getInstance(this.el)?.open();
    }

    static getInstance(el: Element | ArrayLike<Element>): Combobox {
        if (!el || !Select2.getInstance((isArrayLike(el) ? el[0] : el) as HTMLInputElement))
            return null;

        return new (Combobox as any)({ element: el }, false);
    }
}

export function stripDiacritics(str: string) {
    if (!str)
        return str;
    return Select2.stripDiacritics(str);
}