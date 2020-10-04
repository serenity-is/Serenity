/// <reference types="jquery" />
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
}
declare namespace Serenity {
    interface QuickFilterBarOptions {
        filters: QuickFilter<Widget<any>, any>[];
        getTitle?: (filter: QuickFilter<Widget<any>, any>) => string;
        idPrefix?: string;
    }
    class QuickFilterBar extends Widget<QuickFilterBarOptions> {
        constructor(container: JQuery, options?: QuickFilterBarOptions);
        addSeparator(): void;
        add<TWidget extends Widget<any>, TOptions>(opt: QuickFilter<TWidget, TOptions>): TWidget;
        addDateRange(field: string, title?: string): Serenity.DateEditor;
        static dateRange(field: string, title?: string): QuickFilter<DateEditor, DateTimeEditorOptions>;
        addDateTimeRange(field: string, title?: string): DateTimeEditor;
        static dateTimeRange(field: string, title?: string): QuickFilter<DateTimeEditor, DateTimeEditorOptions>;
        addBoolean(field: string, title?: string, yes?: string, no?: string): SelectEditor;
        static boolean(field: string, title?: string, yes?: string, no?: string): QuickFilter<SelectEditor, SelectEditorOptions>;
        onChange: (e: JQueryEventObject) => void;
        private submitHandlers;
        destroy(): void;
        onSubmit(request: Serenity.ListRequest): void;
        protected add_submitHandlers(action: (request: Serenity.ListRequest) => void): void;
        protected remove_submitHandlers(action: (request: Serenity.ListRequest) => void): void;
        protected clear_submitHandlers(): void;
        find<TWidget>(type: {
            new (...args: any[]): TWidget;
        }, field: string): TWidget;
        tryFind<TWidget>(type: {
            new (...args: any[]): TWidget;
        }, field: string): TWidget;
    }
}
declare namespace Serenity {
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
    class QuickSearchInput extends Widget<QuickSearchInputOptions> {
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
}
