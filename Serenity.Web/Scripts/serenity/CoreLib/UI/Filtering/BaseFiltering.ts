declare namespace Serenity {

    class BaseFiltering {
        getOperators(): FilterOperator[];
        appendNullableOperators(list: FilterOperator[]): FilterOperator[];
        appendComparisonOperators(list: FilterOperator[]): FilterOperator[];
        isNullable(): boolean;
        createEditor(): void;
        operatorFormat(op: FilterOperator): string;
        getTitle(field: PropertyItem): string;
        displayText(op: FilterOperator, values: any): string;
        getCriteriaField(): string;
        getCriteria(displayText: any): any[];
        loadState(state: any): void;
        saveState(): any;
        argumentNull(): any;
        validateEditorValue(value: string): any;
        getEditorValue(): any;
        getEditorText(): string;
        get_field(): PropertyItem;
        set_field(value: PropertyItem): void;
        get_container(): JQuery;
        set_container(value: JQuery): void;
        get_operator(): FilterOperator;
        set_operator(value: FilterOperator): void;
    }

}