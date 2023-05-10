export { }

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