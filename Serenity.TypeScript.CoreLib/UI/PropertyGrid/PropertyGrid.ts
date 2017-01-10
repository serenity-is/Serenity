declare namespace Serenity {
    class PropertyGrid extends Widget<PropertyGridOptions> {
        constructor(div: JQuery, opt: PropertyGridOptions);
        load(source: any): void;
        static loadEditorValue(editor: Serenity.Widget<any>, item: PropertyItem, source: any): void;
        save(target: any): void;
        static saveEditorValue(editor: Serenity.Widget<any>, item: PropertyItem, target: any): void;
        enumerateItems(callback: (p1: PropertyItem, p2: Serenity.Widget<any>) => void): void;
        static setRequired(widget: Serenity.Widget<any>, isRequired: boolean): void;
        static setReadOnly(widget: Serenity.Widget<any>, isReadOnly: boolean): void;
        static setReadOnly(elements: JQuery, isReadOnly: boolean): JQuery;
        get_editors(): any;
        get_items(): any;
        get_mode(): PropertyGridMode;
        set_mode(value: PropertyGridMode): void;
    }

    const enum PropertyGridMode {
        insert = 1,
        update = 2
    }

    interface PropertyGridOptions {
        idPrefix?: string;
        items?: PropertyItem[];
        useCategories?: boolean;
        categoryOrder?: string;
        defaultCategory?: string;
        localTextPrefix?: string;
        mode?: PropertyGridMode;
    }

    class PropertyItemHelper {
        static getPropertyItemsFor(type: Function): PropertyItem[];
    }
}