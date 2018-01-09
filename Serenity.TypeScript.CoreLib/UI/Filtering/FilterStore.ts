declare namespace Serenity {

    class FilterStore {
        constructor(fields: any);
        raiseChanged(): void;
        add_changed(value: any): void;
        remove_changed(value: any): void;
        get_fields(): PropertyItem[];
        get_fieldByName(): any;
        get_items(): FilterLine[];
        get_activeCriteria(): any[];
        get_displayText(): string;
        static getCriteriaFor(items: FilterLine[]): any[];
        static getDisplayTextFor(items: FilterLine[]): string;
    }

}