declare namespace Serenity {

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

}