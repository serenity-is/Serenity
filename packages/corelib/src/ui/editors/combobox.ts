import { Fluent, getjQuery, isArrayLike, isPromiseLike, localText, stringFormat } from "@serenity-is/base";

export type ComboboxType = "native" | "select2" | "tomselect";

export interface ComboboxItem<TSource = any> {
    id: string;
    text: string;
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
    createSearchChoice?: (s: string) => ComboboxItem<TSource>;
    element?: HTMLInputElement | HTMLSelectElement | Element[];
    placeholder?: string;
    search?: (query: ComboboxSearchQuery) => (PromiseLike<ComboboxSearchResult<ComboboxItem<TSource>>> | ComboboxSearchResult<ComboboxItem<TSource>>);
    minimumResultsForSearch?: number;
    multiple?: boolean;
    allowClear?: boolean;
    /** Allow arbitrary values for items */
    arbitraryValues?: boolean;
    /** Page size to use while loading or displaying results */
    pageSize?: number;
    /** True to prefer select2 over tomselect when both are available, default is false */
    preferSelect2?: boolean;
    /** Callback to get options specific to the combobox provider type */
    providerOptions?: (type: ComboboxType, opt: ComboboxOptions) => any;
    /** Type delay for searching, default is 200 */
    typeDelay?: number;
}

function getTomSelect(): any {
    // @ts-ignore
    return typeof TomSelect !== "undefined" ? TomSelect : null;
}

function getSelect2() {
    // @ts-ignore
    return (typeof Select2 !== "undefined" && getjQuery()?.fn?.select2) ? Select2 : null;
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

        let tomSelect = getTomSelect();

        if (tomSelect && (!opt?.preferSelect2 || !getSelect2())) {
            this.createTomselect(opt);
        }
        else if (getSelect2()) {
            this.createSelect2(opt);
        }
        else {
        }
    }

    private createTomselect(opt: ComboboxOptions) {
        const ts = getTomSelect();

        var settings: any = {
            create: opt.createSearchChoice ? opt.createSearchChoice : false,
            persist: false,
            placeholder: opt.placeholder,
            maxItems: opt.multiple ? null : 1,
            valueField: "id",
            hidePlaceholder: true,            
            loadThrottle: null,
            shouldLoad: (query: string) => {
                if (!query) {
                    var tomselect = (this.el as any).tomselect;
                    if (!tomselect.loading && !tomselect.initialemptyload) {
                        tomselect.initialemptyload = true;
                        tomselect.load();
                    }

                }
                return true;
            }
        }

        settings.load = (query: string, callback: (items?: ComboboxItem<TItem>[]) => void) => {
            var pageSize = opt.pageSize;
            var searchQuery: ComboboxSearchQuery = {
                searchTerm: query?.trim() || null,
                skip: 0,
                take: Math.min(pageSize, 1000), // tomselect does not support paging by default
                checkMore: true
            }

            var tomselect = (this.el as any).tomselect;
            this.abortPendingQuery();

            //select2?.search?.removeClass?.('select2-active').parent().removeClass('select2-active');

            (this.el as any).typeTimeout = setTimeout(() => {
                this.abortPendingQuery();
                //select2?.search?.addClass?.('select2-active').parent().addClass('select2-active');
                searchQuery.signal = ((this.el as any).queryLoading = new AbortController()).signal;

                const cleanup = () => {
                    //tomselect.clearOptions();
                    callback();
                    delete (this.el as any).queryLoading;
                    //select2?.search?.removeClass?.('select2-active').parent().removeClass('select2-active');
                }

                try {
                    const then = (result: ComboboxSearchResult<ComboboxItem<TItem>>) => {
                        delete (this.el as any).queryLoading;
                        //tomselect.clearOptions();
                        //select2?.search?.removeClass?.('select2-active').parent().removeClass('select2-active');
                        callback(result.items);
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
            }, !query ? 0 : opt.typeDelay);
        }

        new ts(this.el, settings);
    }

    private createSelect2(opt: ComboboxOptions) {
        var select2Opt: any = {
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

            let $ = getjQuery();
            var select2 = $ && $(this.el).data('select2');
            select2?.search?.removeClass?.('select2-active').parent().removeClass('select2-active');

            (this.el as any).typeTimeout = setTimeout(() => {
                this.abortPendingQuery();
                select2?.search?.addClass?.('select2-active').parent().addClass('select2-active');
                searchQuery.signal = ((this.el as any).queryLoading = new AbortController()).signal;

                const cleanup = () => {
                    delete (this.el as any).queryLoading;
                    select2?.search?.removeClass?.('select2-active').parent().removeClass('select2-active');
                }

                try {
                    const then = (result: ComboboxSearchResult<ComboboxItem<TItem>>) => {
                        delete (this.el as any).queryLoading;
                        select2?.search?.removeClass?.('select2-active').parent().removeClass('select2-active');
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
            }, !query.term ? 0 : opt.typeDelay);
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

        if (opt.providerOptions)
            select2Opt = Object.assign(select2Opt, opt.providerOptions("select2", opt));

        getjQuery()(this.el).select2(select2Opt);        
    }

    abortPendingQuery() {
        if (!this.el)
            return;

        (this.el as any).queryLoading && (this.el as any).queryLoading?.abort?.();
        (this.el as any).queryLoading = false;
        if ((this.el as any).typeTimeout) {
            clearTimeout((this.el as any).typeTimeout);
            delete (this.el as any).typeTimeout;
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
        let $ = getjQuery();
        $ && $(this.el)?.data?.('select2') && $(this.el).select2?.('destroy');
        this.el = null;
    }

    get type(): ComboboxType {
        if (!this.el)
            return "native";

        if ((this.el as any).tomselect)
            return "tomselect";

        if (getjQuery()?.(this.el)?.data()?.select2)
            return "select2";

        return "native";
    }

    get isMultiple(): boolean {
        if (!this.el)
            return false;

        if ((this.el as any).tomselect)
            return false;

        var select2 = getjQuery()?.(this.el)?.data()?.select2;
        if (select2)
            return !!select2.opts?.multiple;

        return this.el.getAttribute('multiple') != null;
    }

    getValue(): string {
        if (!this.el)
            return null;

        if ((this.el as any).tomselect)
            return null;

        let $ = getjQuery();
        if ($ && $(this.el)?.data()?.select2) {
            var val = $(this.el).select2('val');
            if (Array.isArray(val))
                return val.join(',');

            return val;
        }

        return this.el.value;
    }

    getValues(): string[] {
        if (!this.el)
            return [];

        if ((this.el as any).tomselect)
            return [];

        let $ = getjQuery();
        let val: any;
        if ($ && $(this.el)?.data()?.select2)
            val = $(this.el).select2('val');
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
            if ((this.el as any).tomselect)
                return;

            let $ = getjQuery();
            if ($ && $(this.el)?.data()?.select2) {
                $(this.el).select2('val', val);
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

    static getInstance(el: Element | ArrayLike<Element>): Combobox {
        if (!el)
            return null;
        return new (Combobox as any)({ element: el }, false);
    }
}

export function stripDiacritics(str: string) {
    if (!str)
        return str;
    str.normalize("NFD").replace(/[\u0300-\u036f]/g, "");
}

export function select2LocaleInitialization() {
    let $ = getjQuery();
    if (!$?.fn?.select2)
        return false;

    const txt = (s: string) => localText("Controls.SelectEditor." + s);
    const fmt = (s: string, ...prm: any[]) => stringFormat(localText("Controls.SelectEditor." + s), prm);

    $.fn.select2.locales['current'] = {
        formatMatches: (matches: number) => matches === 1 ? txt("SingleMatch") : fmt("MultipleMatches", matches),
        formatNoMatches: () => txt("NoMatches"),
        formatAjaxError: () => txt("AjaxError"),
        formatInputTooShort: (input: string, min: number) => fmt("InputTooShort", min - input.length, min, input.length),
        formatInputTooLong: (input: string, max: number) => fmt("InputTooLong", input.length - max, max, input.length),
        formatSelectionTooBig: (limit: number) => fmt("SelectionTooBig", limit),
        formatLoadMore: (pageNumber: number) => fmt("LoadMore", pageNumber),
        formatSearching: () => txt("Searching")
    };
    $.extend(($.fn.select2 as any).defaults, ($.fn.select2 as any).locales['current']);
    return true;
}

!select2LocaleInitialization() && Fluent.ready(select2LocaleInitialization);