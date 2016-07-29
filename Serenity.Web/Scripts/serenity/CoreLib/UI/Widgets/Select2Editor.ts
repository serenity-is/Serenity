declare namespace Serenity {
    class Select2Editor<TOptions, TItem> extends Widget<TOptions> {
        items: Select2Item[];
        itemById: any;
        pageSize: number;
        lastCreateTerm: string;
        constructor(hidden: JQuery, opt: any);
        emptyItemText(): string;
        getSelect2Options(): Select2Options;
        clearItems(): void;
        addItem(item: Select2Item): void;
        addItem(key: string, text: string, source?: any, disabled?: boolean): void;
        addInplaceCreate(addTitle?: string, editTitle?: string): void;
        inplaceCreateClick(e: any): void;
        getCreateSearchChoice(getName?: (p1: any) => string): (p1: string) => any;
        setEditValue(source: any, property: PropertyItem): void;
        getEditValue(property: PropertyItem, target: any): void;
        get_delimited(): boolean;
        get_select2Container(): JQuery;
        get_items(): Select2Item[];
        get_itemByKey(): any;
        value: string;
        values: string[];
        get_text(): string;
    }

    namespace Select2Extensions {
        function select2(element: JQuery): JQuery;
        function select2(element: JQuery, options: Select2Options): JQuery;
        function select2(element: JQuery, action: string): JQuery;
        function select2(element: JQuery, option: string, value: any): JQuery;
        function select2(element: JQuery, option: string): any;
    }

    interface Select2Item {
        id: string;
        text: string;
        source: any;
        disabled: boolean;
    }

    class SelectEditor extends Select2Editor<SelectEditorOptions, Select2Item> {
        constructor(hidden: JQuery, opt: SelectEditorOptions);
        getItems(): any[];
        updateItems(): void;
    }

    interface SelectEditorOptions {
        items?: any[];
        emptyOptionText?: string;
    }
}