import { getjQuery } from "./environment";
import { Fluent } from "./fluent";
import { stringFormat } from "./formatting";
import { localText } from "./localtext";
import { isArrayLike } from "./system";

export interface ComboboxOptions {
    element?: HTMLInputElement | HTMLSelectElement | Element[];
}

export class Combobox {
    private el: HTMLInputElement | HTMLSelectElement;

    constructor(opt: ComboboxOptions);
    constructor(opt: ComboboxOptions, create: boolean = true) {
        if (isArrayLike(opt?.element))
            this.el = opt.element[0] as HTMLInputElement;
        else if (typeof opt?.element !== "function")
            this.el = opt?.element;

        if (!create)
            return;
        
    }

    static getInstance(el: Element | ArrayLike<Element>): Combobox {
        if (!el)
            return null;
        return new (Combobox as any)({ element: el }, false);
    }
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