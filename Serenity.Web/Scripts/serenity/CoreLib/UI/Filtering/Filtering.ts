declare namespace Serenity {
    interface IFiltering {
        createEditor(): void;
        getCriteria(displayText: any): any[];
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

    class FilterPanel extends FilterWidgetBase<any> {
        static panelTemplate: string;
        static rowTemplate: string;
        constructor(div: JQuery);
        updateRowsFromStore(): void;
        search(): void;
        get_showInitialLine(): boolean;
        set_showInitialLine(value: boolean): void;
        get_showSearchButton(): boolean;
        set_showSearchButton(value: boolean): void;
        get_updateStoreOnReset(): boolean;
        set_updateStoreOnReset(value: boolean): void;
        get_hasErrors(): boolean;
    }

    class FilterStore {
        constructor(fields: any);
        raiseChanged(): void;
        add_Changed(value: any): void;
        remove_Changed(value: any): void;
        get_fields(): PropertyItem[];
        get_fieldByName(): any;
        get_items(): FilterLine[];
        get_activeCriteria(): any[];
        get_displayText(): string;
    }

    class FilterWidgetBase<TOptions> extends TemplatedWidget<TOptions> {
        constructor(div: JQuery, opt: any);
        filterStoreChanged(): void;
        get_store(): FilterStore;
        set_store(value: FilterStore): void;
    }

    class LookupFiltering extends BaseEditorFiltering<LookupEditor> {
    }

    class StringFiltering extends BaseFiltering {
    }

    class FilterDialog extends TemplatedDialog<any> {
        get_filterPanel(): FilterPanel;
    }

    class FilterDisplayBar extends FilterWidgetBase<any> {
        constructor(div: JQuery);
    }

    namespace FilteringTypeRegistry {
        function get(key: string): Function;
        function initialize(): void;
        function reset(): void;
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

    interface QuickFilterArgs<TWidget> {
        field?: string;
        widget?: TWidget;
        request?: ListRequest;
        equalityFilter?: any;
        value?: any;
        active?: boolean;
        handled?: boolean;
    }

    class QuickSearchInput extends Widget<QuickSearchInputOptions> {
        constructor(input: JQuery, opt: QuickSearchInputOptions);
        checkIfValueChanged(): void;
        get_field(): QuickSearchField;
        set_field(value: QuickSearchField): void;
    }

    interface QuickSearchInputOptions {
        typeDelay?: number;
        loadingParentClass?: string;
        filteredParentClass?: string;
        onSearch?: (p1: string, p2: string, p3: (p1: boolean) => void) => void;
        fields?: QuickSearchField[];
    }

}